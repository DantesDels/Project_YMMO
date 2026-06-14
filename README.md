# YMMO — Groupe Immobilier

Plateforme web d'achat, vente et analyse de biens immobiliers avec intelligence artificielle.

## Architecture

```
Frontend (Vue 3) ──► Nginx ──┬──► Backend .NET 10 ──► PostgreSQL 16
                              └──► DataScience Python ──► Agrégation 2M biens
```

Trois projets indépendants, orchestrés par Docker Compose :

| Projet | Technologie | Rôle |
|--------|------------|------|
| `YMMO_BackEnd/` | .NET 10 + EF Core + PostgreSQL | API REST, auth JWT, CRUD biens/clients/offres |
| `YMMO_FrontEnd/` | Vue 3 + Vite + Pinia + Chart.js | SPA, catalogue, analyse marché, dashboard agent/client |
| `YMMO_DataScience/` | Python FastAPI + scikit-learn | Analyse marché 2M biens, prévisions IA |

## Démarrage rapide

```bash
# Tout en Docker
docker compose up -d
# → http://localhost:8080

# Ou en développement
cd YMMO_FrontEnd && npm run dev  # → http://localhost:5173
cd YMMO_BackEnd && dotnet run    # → http://localhost:5000
```

## Documentation

Toute la documentation technique se trouve dans [`Docs/`](Docs/) :

| Document | Description |
|----------|-------------|
| [architecture.md](Docs/architecture.md) | Architecture globale, flux front↔back, sécurité |
| [backend.md](Docs/backend.md) | Structure .NET 10, Clean Architecture, endpoints |
| [frontend.md](Docs/frontend.md) | Vue 3, Pinia, Router, stores, composants |
| [datascience.md](Docs/datascience.md) | Python FastAPI, agrégateur 2M, fallback pattern |
| [docker-deployment.md](Docs/docker-deployment.md) | Docker Compose, builds, nginx, déploiement |
| [technologies.md](Docs/technologies.md) | Stack complète avec justifications |
| [api-reference.md](Docs/api-reference.md) | Tous les endpoints REST |
| [MCD.md](Docs/MCD.md) | Modèle Conceptuel de Données |
| [Launch.md](Docs/Launch.md) | Guide de démarrage |
| [.PRD.md](Docs/.PRD.md) | Product Requirements Document |
| [Justification_Choix.md](Docs/Justification_Choix.md) | Décisions architecturales |

## Licence

MIT
