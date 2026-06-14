# Technologies — YMMO

## Frontend

### Vue 3 (Composition API)

Framework SPA réactif. Utilisation systématique de `<script setup lang="ts">` avec `defineProps` / `defineEmits` / `defineExpose`. Pas de Options API.

- **Refs** → `ref()`, `reactive()`, `computed()` (pas de `watch` sauf si nécessaire)
- **Lifecycle** → `onMounted`, `onUnmounted`
- **Props typing** → `defineProps<{ prop: Type }>()`

### Vite 8

Build tool. Avantages : HMR instantané, build rapide (esbuild + Rollup), tree-shaking natif. Configuration minimale (`vite.config.js`), pas de Webpack.

- **Plugin** : `@vitejs/plugin-vue`
- **Alias** : `@` → `./src`
- **Proxy dev** : `/api` → `http://localhost:5000`, `/ds-api` → `http://localhost:8000`

### Pinia 3

Store officiel Vue 3. Utilisé pour :
- `authentification.store` → JWT token, user session
- `property.store` → Résultats de recherche
- `filterStore` → Filtres du catalogue
- `wishlist.store` → Favoris (persisté localStorage)
- `customProperties.store` → Propriétés créées par l'agent (local)

### Vue Router 4

- `createWebHistory()` — URLs propres (pas de #)
- Lazy loading — toutes les vues en `() => import(...)`
- Guards — `beforeEach` pour `requiresAuthentification`
- 3 layouts : `PublicLayout`, `ClientLayout`, `LegalLayout`

### Axios

Client HTTP. Intercepteur request pour le token JWT. Intercepteur response pour le 401 → logout. Base URL configurable via `VITE_API_URL`.

### Chart.js 4

Graphiques sans wrapper — composant `<ChartWrapper>` avec lifecycle manuel (`new Chart()` / `.destroy()`). 6 types supportés : bar, line, pie, doughnut, polarArea, radar.

## Backend

### .NET 10

Runtime cible `net10.0`. Utilisation des dernières fonctionnalités : `Program.cs` minimal, Primary Constructors, Collection Expressions.

### ASP.NET Core 10

- Controllers avec attributs `[Route]`, `[Authorize]`
- Middleware pipeline : ErrorHandling → Routing → CORS → Auth → Authorization → Controllers
- JSON serialization : `JsonStringEnumConverter`, `ReferenceHandler.IgnoreCycles`

### Entity Framework Core 10

ORM complet avec :
- **Fluent API** (`IEntityTypeConfiguration<T>`) — configurations séparées par entité
- **TPH** (Table-Per-Hierarchy) pour `Contact` → `Agent` / `Client`
- **Migrations** auto-appliquées au démarrage (`Database.Migrate()`)
- **DeleteBehavior.Restrict** par défaut, exceptions ciblées

### PostgreSQL 16

Base de données relationnelle. Utilisation de :
- Table `text[]` pour les caractéristiques
- `HasConversion<string>()` pour les enums
- Index uniques composites (WishlistItem)
- Image officielle `postgres:16-alpine` pour Docker

### BCrypt

Hash des mots de passe via `BCrypt.Net.BCrypt`. Hash + salt automatique.

### JWT Bearer

Authentification HMAC-SHA256. Claims : `NameIdentifier` (ContactId), `Role`, `AgencyId`.

### AutoMapper

Mapping DTO ↔ Entity. Profiles séparés par module métier. Validation en mode Development.

### MailKit

Envoi d'emails SMTP. Échec silencieux si non configuré (log warning).

## DataScience

### FastAPI

Framework Python type-safe pour API REST. Génération automatique de la doc OpenAPI. CORS activé.

### Uvicorn

Serveur ASGI. Démarrage : `uvicorn main:app --host 0.0.0.0 --port 8000`.

### scikit-learn / NumPy

Déclarés dans `requirements.txt` mais **non utilisés par l'API live**. Disponibles pour les modules d'analyse standalone (`predictions.py`).

## Infrastructure

### Docker

4 conteneurs orchestrains par `docker-compose.yml`. Multi-stage pour .NET et Node.

### Nginx

Reverse proxy + serveur statique. Compression gzip. SPA fallback vers `index.html`.

### Docker Compose

```yaml
services: db (postgres:16-alpine), backend (.NET 10), datascience (Python 3.12), frontend (nginx)
volumes: pgdata
```

## Choix technologiques — Justifications

### Pourquoi Vue 3 plutôt que React ?

| Critère | Vue 3 | React |
|---------|-------|-------|
| Courbe d'apprentissage | Progressive | ↗️ raide avec JSX |
| Réactivité | `ref()` natif | `useState` hooks |
| Performance | Template compiler | Virtual DOM diff |
| TypeScript | `<script setup lang="ts">` | Prop types complexes |

### Pourquoi .NET plutôt que Node.js ?

| Critère | .NET | Node.js |
|---------|------|---------|
| Type safety | C# natif | TypeScript ajouté |
| EF Core ORM | Mature, migrations | TypeORM / Prisma |
| Performance | Compilé AOT/JIT | Interprété |
| Écosystème métier | Rich (AutoMapper, BCrypt) | Nécessite assemblage de libs |

### Pourquoi PostgreSQL plutôt que SQL Server ?

- Image Docker légère (`16-alpine`)
- Tableaux natifs (`text[]` pour les caractéristiques)
- Open source, zéro licence
- Excellent support EF Core via Npgsql

### Pourquoi un service DataScience séparé (Python) ?

- Écosystème scientifique Python (scikit-learn, numpy, pandas) sans alourdir .NET
- Isolation : les dépendances ML n'impactent pas le backend transactionnel
- Déploiement indépendant (scale du DS API sans scale .NET)
- Fallback TypeScript côté frontend pour la résilience

### Pourquoi Nginx plutôt que Kestrel direct ?

- Reverse proxy avec compression gzip
- Serveur statique optimisé (sendfile, cache)
- Séparation des concerns (le frontend n'a pas besoin de connaître Kestrel)
- Possibilité d'ajouter HTTPS, rate limiting, etc. sans toucher au backend
