# YMMO — Frontend Vue 3

Application web SPA pour la plateforme immobilière YMMO : catalogue, analyse de marché, dashboard client et agent.

## Stack

| Technologie | Version |
|------------|---------|
| Vue 3 (Composition API) | 3.5.34 |
| Vite | 8.0 |
| TypeScript | — |
| Pinia | 3.0.4 |
| Vue Router | 4.6.4 |
| Axios | 1.17.0 |
| Chart.js | 4.5.1 |
| jwt-decode | 4.0.0 |

## Démarrage

```bash
# Développement
cd YMMO_FrontEnd
npm install
npm run dev
# → http://localhost:5173

# Build production
npm run build
# → dist/

# Docker
docker compose up -d frontend
# → http://localhost:8080
```

## Scripts

| Commande | Description |
|----------|-------------|
| `npm run dev` | Serveur de dev Vite (HMR) |
| `npm run build` | Build production → `dist/` |
| `npm run preview` | Prévisualisation du build |

## Structure

```
src/
├── api/              # Axios + endpoints (authentification, datascience)
├── assets/           # CSS globaux (variables, reset)
├── components/       # UI, charts, dashboard, analysis modules
├── layouts/          # PublicLayout, ClientLayout, LegalLayout
├── router/           # Routes, guards, 3 layouts
├── services/         # Market analysis, property, offer
├── stores/           # Pinia : auth, property, filter, wishlist, customProperties
├── types/            # Interfaces TypeScript (DTOs C#)
├── utils/            # Formatters, imageLoader, mockData, clickOutside
└── views/            # Pages (agent/, footerLayout/, headerLayout/)
```

## Analyse Marché — Fallback Pattern

L'analyse de marché génère **2 millions de biens mock** côté navigateur (TypeScript) avec le même seed que le service Python. Si l'API DataScience est disponible, elle est utilisée ; sinon le calcul tombe en local automatiquement (timeout 5s).

## Documentation

Voir [`Docs/frontend.md`](../Docs/frontend.md) pour la documentation détaillée.
Voir [`Docs/architecture.md`](../Docs/architecture.md) pour les flux front↔back.
