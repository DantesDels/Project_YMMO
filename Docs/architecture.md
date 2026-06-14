# Architecture Globale — YMMO

## Vue d'ensemble

```
┌───────────────────────────────────────────────────────────┐
│                    Navigateur Client                        │
│  Vue 3 SPA · TypeScript · Chart.js · localStorage Auth    │
└──────────────────────────┬────────────────────────────────┘
                           │ 80 / 443
                           ▼
┌───────────────────────────────────────────────────────────┐
│                   Nginx (reverse proxy)                     │
│  /api/* ──────────────────────────────────────────┐        │
│  /swagger/* ────────────────┐                     │        │
│  /ds-api/* ───────┐         │                     │        │
│  /* → index.html  │         │                     │        │
└───────────────────┼─────────┼─────────────────────┼────────┘
                    │         │                     │
                    ▼         ▼                     ▼
┌───────────────────┐  ┌──────────┐  ┌──────────────────────┐
│   Backend .NET 10  │  │ Swagger  │  │  DataScience Python  │
│   Kestrel:8080     │  │   UI     │  │  FastAPI:8000        │
├───────────────────┤  └──────────┘  ├──────────────────────┤
│ Controllers →      │               │ /ds-api/market-       │
│ Services →          │               │   analysis/* (8 endpoints)
│ Repositories →      │               │ Agrégateur 2M biens  │
│ EF Core → PostgreSQL│               │ en mémoire            │
└─────────┬───────────┘               └──────────────────────┘
          │ 5432
          ▼
┌───────────────────┐
│ PostgreSQL 16      │
│ Docker volume:     │
│ pgdata             │
└───────────────────┘
```

## Principes d'architecture

### Clean Architecture (4 couches)

| Couche | Rôle | Dépendances |
|--------|------|------------|
| **API** | Controllers, Middleware, Routing | Application |
| **Application** | Services métier, DTOs, Mapping (AutoMapper) | Domain |
| **Domain** | Entities, Enums, Repository Interfaces, Business Rules | — (noyau pur) |
| **Infrastructure** | EF Core DbContext, Repositories, Email, Auth (JWT/BCrypt) | Domain |

### Frontend — Architecture MVVM-like

| Rôle | Technologie |
|------|------------|
| **Vues** (pages) | `src/views/` — composables + template |
| **Composants** | `src/components/` — UI réutilisable |
| **Stores** (état) | Pinia (`src/stores/`) — authentification, filtres, wishlist |
| **Services** (données) | `src/services/` — appels API avec fallback mock |
| **API** | `src/api/` — instances Axios, intercepteurs JWT |
| **Router** | `src/router/` — lazy loading, guards, layouts |

## Flux de navigation

```
PublicLayout                        ClientLayout
  ├── / → HomeView                    ├── /client/dashboard
  ├── /catalog → CatalogView          ├── /client/wishlist
  ├── /market-analysis                ├── /client/offers
  ├── /property/:id                   └── /profile
  ├── /login /register
  └── /informations
                                      AgentLayout
                                        ├── /agent/dashboard
LegalLayout                            ├── /agent/market-analysis
  ├── /cgu                            ├── /agent/properties/new
  ├── /confidentiality                 └── /agent/properties/:id/edit
  ├── /help
  ├── /legal-mentions
  └── /support
```

## Interactions Frontend ↔ Backend

### Authentification (JWT)

```
Frontend                          Backend
   │                                │
   │  POST /api/authentification/   │
   │       register                 │
   ├───────────────────────────────►│  201 Created
   │◄───────────────────────────────┤  { token, user }
   │                                │
   │  localStorage.setItem('token') │
   │                                │
   │  GET /api/client/me            │
   │  Authorization: Bearer <jwt>   │
   ├───────────────────────────────►│  200 OK
   │◄───────────────────────────────┤  { profile }
```

### Catalogue / Recherche

```
Frontend                          Backend
   │                                │
   │  GET /api/property?            │
   │  &type=House&city=Paris        │
   │  &minPrice=200000&maxPrice=    │
   ├───────────────────────────────►│  PropertyController
   │◄───────────────────────────────┤  [{PropertySummaryDto}]
   │                                │
   │  GET /api/search?keyword=...   │
   ├───────────────────────────────►│  SearchController
   │◄───────────────────────────────┤  [{PropertySummaryDto}]
```

### Analyse Marché (Fallback Pattern)

```
Frontend                          DataScience API (Python)
   │                                │
   │  GET /ds-api/market-analysis/  │
   │       full                     │
   ├───────────────────────────────►│
   │  ┌─── 5s timeout ────┐        │
   │  │ si timeout ou erreur│        │
   │  │   ↓                 │        │
   │  │ computeFullAnalysis()│       │
   │  │ (TypeScript local)  │        │
   │  │ 2M biens en mémoire │        │
   │  └─────────────────────┘        │
```

## Flux Données — Analyse Marché 2M

```
Python (Docker)                            TypeScript (Fallback)
generate_aggregated()                      computeFullAnalysis()
  │                                           │
  ├─ _gen_one(i)  × 2_000_000                 ├─ generateOne(i)  × 2_000_000
  │   └─ _rand(seed)                          │   └─ Math.sin(seed*9301+49297)*49297
  │                                           │
  └─ Agrégation directe                       └─ Agrégation directe
     (pas de stockage mémoire                    (sessionStorage cache)
      des 2M objets)
```

**Seed identique** : Python et TypeScript utilisent le même algorithme de génération pseudo-aléatoire (`Math.sin` × constantes magiques), garantissant des données identiques quelle que soit la source.

## Gestion des erreurs

### Backend — ErrorHandlingMiddleware

| Exception | Status |
|-----------|--------|
| `ArgumentNullException` | 400 |
| `KeyNotFoundException` | 404 |
| `UnauthorizedAccessException` | 401 |
| `InvalidOperationException` | 409 |
| `DbUpdateException` | 409 |
| Autres | 500 |

### Frontend — Fallback Pattern

```typescript
async function withFallback<T>(
  api: () => Promise<T>,
  fallback: () => T
): Promise<T> {
  try {
    return await api()      // 5s timeout
  } catch {
    return fallback()       // données mock locales
  }
}
```

## Sécurité

- **JWT** HMAC-SHA256, stocké dans `localStorage`
- **BCrypt** pour le hash des mots de passe
- **CORS** configurable via `CORS:Origins`
- **Environnement `Production`** : pas de Swagger, pas de debug-login
- **Rôles** : `Client`, `Agent`, `Manager`, `Admin` — autorisation par attribut `[Authorize(Roles = "...")]`
