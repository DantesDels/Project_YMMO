# Déploiement Docker — YMMO

## Architecture des conteneurs

```
┌───────────────────────────────────────────────────┐
│                   docker-compose                   │
│                                                    │
│  ┌──────────┐  ┌──────────┐  ┌──────────────────┐ │
│  │   db      │  │ backend  │  │   datascience     │ │
│  │postgres:16│  │.NET 10   │  │  Python 3.12      │ │
│  │:5432      │  │:8080     │  │  FastAPI :8000    │ │
│  └─────┬─────┘  └────┬─────┘  └────────┬─────────┘ │
│        │              │                 │           │
│        └──────────────┴─────────────────┘           │
│                         │                            │
│                    ┌────▼─────┐                      │
│                    │ frontend │                      │
│                    │  nginx   │                      │
│                    │  :80     │                      │
│                    └──────────┘                      │
└─────────────────────────────────────────────────────┘
```

## Services

### db — PostgreSQL 16

```yaml
db:
  image: postgres:16-alpine
  environment:
    POSTGRES_DB: YMMO_Dev
    POSTGRES_USER: postgres
    POSTGRES_PASSWORD: postgres
  volumes:
    - pgdata:/var/lib/postgresql/data
  healthcheck:
    test: ["CMD-SHELL", "pg_isready -U postgres"]
    interval: 5s
    retries: 10
```

| Détail | Valeur |
|--------|--------|
| Port host | 5432 |
| Volume | `pgdata` (persistant) |
| Healthcheck | `pg_isready` toutes les 5s |

### backend — .NET 10

```yaml
backend:
  build:
    context: YMMO_BackEnd
    dockerfile: Dockerfile
  depends_on:
    db: { condition: service_healthy }
  environment:
    ASPNETCORE_ENVIRONMENT: Production
    ASPNETCORE_URLS: http://+:8080
    ConnectionStrings__DefaultConnection: Host=db;Database=YMMO_Dev;Username=postgres;Password=postgres
    Jwt__Issuer: YMMO_Backend
    Jwt__Audience: YMMO_Frontend
    Jwt__Key: YmmoDevJwtSecretKey2024MinLength32Chars
    CORS__Origins: http://localhost:5173,http://localhost:8080
```

| Détail | Valeur |
|--------|--------|
| Port host:container | 5000:8080 |
| Dépend | db (healthy) |
| Environnement | Production |
| Connection string | Host=db (nom du service Docker) |

### datascience — Python FastAPI

```yaml
datascience:
  build:
    context: YMMO_DataScience
    dockerfile: Dockerfile
```

| Détail | Valeur |
|--------|--------|
| Port host:container | 8000:8000 |
| Démarrage | `generate_aggregated()` à l'import (~1-2s) |
| Pas de dépendance | Autonome (pas de DB) |

### frontend — Nginx

```yaml
frontend:
  build:
    context: YMMO_FrontEnd
    dockerfile: Dockerfile
  depends_on:
    - backend
    - datascience
```

| Détail | Valeur |
|--------|--------|
| Port host:container | 8080:80 |
| Build | Node 22 build → nginx:alpine runtime |
| Dépend | backend, datascience |

## Dockerfiles

### backend/Dockerfile (multi-stage)

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY YMMO.Backend.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "YMMO.Backend.dll"]
```

### frontend/Dockerfile (multi-stage)

```dockerfile
FROM node:22-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine AS runtime
EXPOSE 80
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
```

### datascience/Dockerfile

```dockerfile
FROM python:3.12-slim
WORKDIR /app
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt
COPY . .
EXPOSE 8000
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
```

## Nginx (reverse proxy)

```
listen 80
root /usr/share/nginx/html

/api/           → proxy_pass http://backend:8080/api/
/swagger/       → proxy_pass http://backend:8080/swagger/
/ds-api/        → proxy_pass http://datascience:8000/ds-api/
/               → try_files $uri $uri/ /index.html  (SPA fallback)
```

Gzip activé sur : CSS, JS, JSON, SVG (min 256 bytes).

## Commandes

```bash
# Tout builder et démarrer
docker compose up -d

# Démarrer un service spécifique
docker compose up -d db backend

# Voir les logs
docker compose logs -f backend

# Rebuild un service après modification
docker compose build backend && docker compose up -d backend

# Arrêter
docker compose down

# Arrêter + supprimer les volumes (DB reset)
docker compose down -v

# Accès
# Frontend : http://localhost:8080
# Swagger  : http://localhost:8080/swagger/
# DS API   : http://localhost:8000/ds-api/health
```

## Variables d'environnement

### Backend

| Variable | Description | Dev | Docker |
|----------|-------------|-----|--------|
| `ASPNETCORE_ENVIRONMENT` | Environnement | Development | Production |
| `ConnectionStrings__DefaultConnection` | Chaine de connexion | Host=localhost;… | Host=db;… |
| `Jwt__Key` | Clé JWT (32+ chars) | User Secrets | Variable env |
| `CORS__Origins` | Origines autorisées | localhost:5173 | localhost:5173,:8080 |

### Frontend

| Variable | Description |
|----------|-------------|
| `VITE_API_URL` | URL API (défaut : `/api` — proxy Nginx) |

## Environnements

### Développement (local)

```bash
# Backend (nécessite PostgreSQL local)
cd YMMO_BackEnd
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "DevKey32CharsMinLengthHere!!"
dotnet run          # http://localhost:5000

# Frontend
cd YMMO_FrontEnd
npm run dev         # http://localhost:5173

# Proxy Vite : /api → localhost:5000, /ds-api → localhost:8000
```

**Note** : Le service DataScience ne fonctionne **pas** en local (pas de Python). Le frontend utilise le fallback TypeScript. Pour tester l'API DS : `docker compose up datascience`.

### Production (Docker)

```bash
docker compose up -d
# http://localhost:8080
```

## Sécurité Docker

- **Aucun port exposé inutilement** : seul le frontend (port 80) et la DB (port 5432) sont exposés
- **Pas de HTTPS** en Docker (terminé au niveau du reverse proxy externe)
- **Healthcheck** DB : le backend attend que PostgreSQL soit prêt
- **.dockerignore** : exclut `node_modules/`, `dist/`, `bin/`, `obj/`, `.git/`
