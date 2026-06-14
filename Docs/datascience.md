# Service DataScience — Python FastAPI

## Stack

| Technologie | Version |
|------------|---------|
| Python | 3.12 (slim) |
| FastAPI | 0.115.12 |
| Uvicorn | 0.34.2 |
| NumPy | 2.2.6 |
| scikit-learn | 1.7.2 |

## Structure

```
YMMO_DataScience/
├── main.py                      # FastAPI app, 8 endpoints
├── requirements.txt
├── Dockerfile
├── analysis/
│   ├── __init__.py
│   ├── aggregator.py            # Générateur 2M + agrégation (utilisé)
│   ├── market_trends.py         # Analyse tendances (standalone)
│   ├── popular_properties.py    # Stats populaires (standalone)
│   ├── predictions.py           # ML scikit-learn (standalone)
│   └── target_zones.py          # Analyse par zone (standalone)
└── data/
    ├── __init__.py
    └── mock_data.py             # Générateur alternatif (compatibilité)
```

## Agrégateur 2M (core engine)

### Principe

`analysis/aggregator.py` génère **2 millions de biens** en **single-pass** — aucun objet en mémoire, uniquement des accumulations.

### Algorithme

```python
TOTAL = 2_000_000

def _rand(seed):
    x = math.sin(seed * 9301 + 49297) * 49297
    return x - math.floor(x)   # [0, 1)

def _gen_one(i):
    seed = i + 1
    ptype = ['House','Apartment','Land','Commercial','Office','Garage','Parking']
    cond = ['New','Excellent','Good','NeedsRefresh','NeedsRenovation','Ruin']
    surface = 25 + int(_rand(seed) * 175)       # 25–200
    rooms = int(_rand(seed + 1000) * 5) + 1      # 1–5
    base = [...] base prices per type
    city = 15 French cities with price multipliers
    price = int(base * city_mult * (surface/80) * (0.7 + _rand(seed+2000)*0.6))
    month_key = monthly bucket from 2024-01 to 2026-12
    ...
```

### Accumulation

```
generate_aggregated() → {
  totalListings,
  avgPrice, minPrice, maxPrice,
  citiesCount,
  typeCounts: { House: n, Apartment: n, ... },
  condCounts: { New: n, ... },
  featureCounts: { Balcony: n, ... },
  cityData: { Paris: { sum_price, sum_surface, count }, ... },
  monthly: { "2024-01": { sum_price, sum_surface, count, min, max }, ... }
}
```

### Déterminisme

Même seed `9301 + 49297` utilisé en Python **et** en TypeScript → données identiques. Le frontend peut basculer entre API Python et fallback TS sans incohérence.

## Endpoints API

| Endpoint | Méthode | Paramètres | Description |
|----------|---------|------------|-------------|
| `/ds-api/health` | GET | — | Healthcheck, renvoie `{ status, propertiesCount }` |
| `/ds-api/market-analysis/summary` | GET | — | KPI globaux + répartition types/villes |
| `/ds-api/market-analysis/trends` | GET | `period`, `property_type`, `city` | Séries temporelles |
| `/ds-api/market-analysis/zones` | GET | — | Analyse par ville (hot/affordable zones) |
| `/ds-api/market-analysis/popular` | GET | — | Types, features, conditions populaires |
| `/ds-api/market-analysis/predictions` | GET | `months` (1–24, def. 6) | Prévisions par régression linéaire |
| `/ds-api/market-analysis/forecast-by-type` | GET | `months` (1–24, def. 6) | Tendance up/down par type |
| `/ds-api/market-analysis/price-distribution` | GET | `bins` (5–50, def. 20) | Histogramme des prix |
| `/ds-api/market-analysis/full` | GET | `period`, `property_type`, `city`, `months` | Agrégation complète (appelle les 6 endpoints) |

### Formats de réponse

Tous les endpoints renvoient du JSON. Exemple `summary` :

```json
{
  "totalListings": 2000000,
  "avgPrice": 285000,
  "avgPricePerM2": 3562,
  "avgSurface": 82,
  "minPrice": 20000,
  "maxPrice": 1500000,
  "citiesCount": 15,
  "topCities": [{"city": "Paris", "count": 210123, "percentage": 10.5}],
  "typeDistribution": [{"type": "House", "count": 500000, "percentage": 25.0}],
  "conditionDistribution": [...],
  "topFeatures": [{"feature": "Balcony", "count": 800000, "percentage": 40.0}]
}
```

## Modules standalone (non utilisés par l'API)

Les fichiers `market_trends.py`, `popular_properties.py`, `target_zones.py`, `predictions.py` sont des **utilitaires réutilisables** qui opèrent sur des listes d'objets complets. Ils ne sont pas appelés par `main.py` (qui travaille directement depuis le dictionnaire agrégé). Ils existent comme bibliothèque d'analyse pour usage futur ou externe.

## Déploiement

```bash
# Local (nécessite Python 3.12)
cd YMMO_DataScience
pip install -r requirements.txt
uvicorn main:app --port 8000

# Docker
docker compose up -d datascience
```

## Points d'attention

| Point | Détail |
|-------|--------|
| **Mémoire** | Les 2M biens sont agrégés, jamais stockés — ~50Mo RAM |
| **Démarrage** | `generate_aggregated()` appelé à l'import → ~1-2s |
| **scikit-learn** | Déclaré mais non utilisé par l'API live (uniquement standalone) |
| **Timeout** | Le frontend attend max **5s** avant fallback TypeScript |
| **Python local** | Python **non installé** sur la machine de dev → le service DS ne répond que sous Docker |
