# API Reference — YMMO

Deux API distinctes : **Backend .NET** (données transactionnelles) et **DataScience Python** (analyse marché).

---

## 1. Backend API (YMMO Backend /api)

**Base URL** (dev) : `http://localhost:5000/api`  
**Base URL** (docker) : `http://localhost:8080/api`  
**Auth** : `Authorization: Bearer <JWT>`

### Authentification

| Méthode | Endpoint | Auth | Description |
|---------|----------|------|-------------|
| POST | `/api/authentification/register` | Public | Créer un compte (Client ou Agent) |
| POST | `/api/authentification/login` | Public | Connexion → `{ token, user }` |
| POST | `/api/authentification/logout` | Authorize | Déconnexion (no-op) |

**Register body** :
```json
{
  "email": "user@example.com",
  "password": "****",
  "lastName": "Dupont",
  "firstName": "Jean",
  "phoneNumber": "0600000000",
  "contactRole": "Client"
}
```

**Login body** :
```json
{ "email": "user@example.com", "password": "****" }
```

### Biens (Properties)

| Méthode | Endpoint | Auth | Description |
|---------|----------|------|-------------|
| GET | `/api/property` | Public | Lister les biens (avec filtres) |
| GET | `/api/property/{id}` | Public | Détail d'un bien |
| POST | `/api/property` | Agent, Manager, Admin | Créer un bien |
| PUT | `/api/property/{id}` | Agent, Manager, Admin | Modifier un bien |
| DELETE | `/api/property/{id}` | Agent, Manager, Admin | Supprimer un bien |

**Filtres GET /api/property** :
```
?propertyType=House&city=Paris&minPrice=200000&maxPrice=500000&minSurface=50&maxSurface=200
&condition=Good&page=1&pageSize=20
```

### Recherche

| Méthode | Endpoint | Auth | Description |
|---------|----------|------|-------------|
| GET | `/api/search` | Public | Recherche full-text |
| GET | `/api/search/suggestions/{propertyId}` | Public | Biens similaires |

### Clients

| Méthode | Endpoint | Auth | Description |
|---------|----------|------|-------------|
| POST | `/api/client/register` | Public | Inscription client |
| GET | `/api/client/{id}` | Client, Admin | Profil client |
| PUT | `/api/client/{id}` | Client, Admin | Modifier profil |
| POST | `/api/client/wishlist` | Client | Ajouter favori |
| DELETE | `/api/client/wishlist` | Client | Supprimer favori |

**Wishlist body** :
```json
{ "propertyID": "guid" }
```

### Agents

| Méthode | Endpoint | Auth | Description |
|---------|----------|------|-------------|
| GET | `/api/agent/{id}` | Agent, Manager, Admin | Détail agent |
| PUT | `/api/agent/{id}` | Agent, Admin | Modifier agent |
| GET | `/api/agent/{id}/properties` | Agent, Manager, Admin | Biens de l'agent |

### Offres (Offers)

| Méthode | Endpoint | Auth | Description |
|---------|----------|------|-------------|
| POST | `/api/offer` | Client, Agent, Manager, Admin | Créer offre |
| GET | `/api/offer` | Agent, Manager, Admin | Lister (avec `?propertyId=`) |
| PUT | `/api/offer/{id}/status` | Agent, Manager, Admin | Màj statut |

**Créer offre body** :
```json
{
  "propertyID": "guid",
  "offerPrice": 250000,
  "clientID": "guid"
}
```

**Statuts** : `Pending`, `Negotiation`, `Accepted`, `Rejected`, `Canceled`

### Agences

| Méthode | Endpoint | Auth | Description |
|---------|----------|------|-------------|
| GET | `/api/agency` | Public | Lister les agences |
| PUT | `/api/agency/{id}` | Manager, Admin | Modifier agence |

### Erreurs

Toutes les erreurs retournent :
```json
{
  "statusCode": 400,
  "message": "Description de l'erreur",
  "details": "Stack trace (développement uniquement)"
}
```

### Codes HTTP

| Code | Signification |
|------|--------------|
| 200 | Succès |
| 201 | Créé |
| 400 | Requête invalide |
| 401 | Non authentifié |
| 403 | Non autorisé (rôle insuffisant) |
| 404 | Ressource introuvable |
| 409 | Conflit (doublon, contrainte) |
| 500 | Erreur interne |

---

## 2. DataScience API (Python FastAPI /ds-api)

**Base URL** (dev) : `http://localhost:8000/ds-api`  
**Base URL** (docker) : `http://localhost:8080/ds-api`  
**Auth** : Aucune (publique)  
**Timeout conseillé** : 5s (fallback TypeScript au-delà)

### Health

```
GET /ds-api/health
→ { "status": "ok", "propertiesCount": 2000000 }
```

### Résumé marché

```
GET /ds-api/market-analysis/summary
→ {
    "totalListings": 2000000,
    "avgPrice": 285123,
    "avgPricePerM2": 3562,
    "avgSurface": 82,
    "minPrice": 20000,
    "maxPrice": 1500000,
    "citiesCount": 15,
    "topCities": [{"city": "Paris", "count": 210123, "percentage": 10.5}],
    "typeDistribution": [{"type": "House", "count": 500000, "percentage": 25.0}],
    "conditionDistribution": [{"condition": "Good", "count": 450000, "percentage": 22.5}],
    "topFeatures": [{"feature": "Balcony", "count": 800000, "percentage": 40.0}]
  }
```

### Tendances

```
GET /ds-api/market-analysis/trends
  ?period=monthly       # monthly | quarterly | yearly
  &property_type=House  # (optionnel)
  &city=Paris           # (optionnel)
→ {
    "trends": [
      { "period": "2024-01", "count": 55000, "avgPrice": 280000,
        "avgSurface": 80, "avgPricePerM2": 3500,
        "minPrice": 25000, "maxPrice": 1400000, "totalVolume": 1.54e10 }
    ],
    "summary": {
      "totalListings": 2000000,
      "globalAvgPrice": 285123,
      "globalAvgPricePerM2": 3562,
      "globalAvgSurface": 82,
      "minPrice": 20000,
      "maxPrice": 1500000,
      "period": "monthly"
    },
    "filters": { "propertyType": "all", "city": "all" }
  }
```

### Analyse par zone

```
GET /ds-api/market-analysis/zones
→ {
    "zones": [
      { "city": "Paris", "count": 210123, "avgPrice": 420000,
        "avgPricePerM2": 5200, "avgSurface": 65, "minPrice": 35000,
        "maxPrice": 1500000, "totalVolume": 8.8e10, "ratioToMarket": 1.47 }
    ],
    "totalListings": 2000000,
    "globalAvgPrice": 285123,
    "globalAvgPricePerM2": 3562,
    "hotZones": [ /* ratio > 1.1 */ ],
    "affordableZones": [ /* ratio < 0.9 */ ]
  }
```

### Données populaires

```
GET /ds-api/market-analysis/popular
→ {
    "types": [
      { "type": "House", "count": 500000, "percentage": 25.0 }
    ],
    "features": [
      { "feature": "Balcony", "count": 800000, "percentage": 40.0 }
    ],
    "conditions": [
      { "condition": "Good", "count": 450000, "percentage": 22.5 }
    ],
    "avgPriceByType": [
      { "type": "House", "avgPrice": 350000, "count": 500000, "minPrice": 30000, "maxPrice": 1500000 }
    ]
  }
```

### Prévisions

```
GET /ds-api/market-analysis/predictions
  ?months=6    # 1–24, défaut 6
→ {
    "predictions": [
      { "month": "2026-02", "predictedAvgPrice": 292000 }
    ],
    "confidence": 0.72,
    "model": "LinearRegression (mois)",
    "features": ["price", "surface", "rooms", "date"],
    "dataPoints": 25,
    "monthsForecast": 6
  }
```

### Tendance par type

```
GET /ds-api/market-analysis/forecast-by-type
  ?months=6    # 1–24, défaut 6
→ {
    "forecasts": [
      { "type": "House", "trend": "up", "coefficient": 1500,
        "currentAvgPrice": 350000 }
    ],
    "totalTypes": 7
  }
```

### Distribution des prix

```
GET /ds-api/market-analysis/price-distribution
  ?bins=20     # 5–50, défaut 20
→ {
    "distribution": [
      { "range": "200000-250000", "low": 200000, "high": 250000, "count": 180000 }
    ],
    "total": 2000000
  }
```

### Analyse complète (full)

```
GET /ds-api/market-analysis/full
  ?period=monthly
  &property_type=House
  &city=Paris
  &months=6
→ Combine les 6 endpoints ci-dessus en un seul objet JSON.
  Utilisé par le frontend pour la page d'analyse marché.
```

---

## 3. Fallback TypeScript

Quand l'API DataScience est indisponible (timeout > 5s), le frontend bascule automatiquement sur `marketAnalysis.service.ts` qui génère les mêmes données en local.

**Garantie** : Le seed pseudo-aléatoire est identique en Python et TypeScript → résultats cohérents.
