import sys
import os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from fastapi import FastAPI, Query
from fastapi.middleware.cors import CORSMiddleware
from data.mock_data import generate_mock_properties
from analysis.market_trends import compute_trends, price_distribution
from analysis.popular_properties import popular_types, popular_features, popular_conditions, avg_price_by_type
from analysis.target_zones import zone_analysis
from analysis.predictions import predict_prices, price_forecast_by_type

app = FastAPI(title="YMMO Data Science API", version="1.0.0")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

PROPERTIES = generate_mock_properties(400)


@app.get("/ds-api/health")
def health():
    return {"status": "ok", "propertiesCount": len(PROPERTIES)}


@app.get("/ds-api/market-analysis/summary")
def market_summary():
    prices = [p['price'] for p in PROPERTIES]
    surfaces = [p['surface'] for p in PROPERTIES]
    cities = set(p['city'] for p in PROPERTIES)

    return {
        'totalListings': len(PROPERTIES),
        'globalAvgPrice': round(sum(prices) / len(prices), 2),
        'globalAvgPricePerM2': round(sum(prices) / sum(surfaces), 2) if sum(surfaces) > 0 else 0,
        'globalAvgSurface': round(sum(surfaces) / len(surfaces), 2),
        'minPrice': min(prices),
        'maxPrice': max(prices),
        'totalCities': len(cities),
        'typeDistribution': popular_types(PROPERTIES)['types'],
        'topZones': zone_analysis(PROPERTIES)['zones'][:5],
        'popularFeatures': popular_features(PROPERTIES)['features'][:5],
    }


@app.get("/ds-api/market-analysis/trends")
def get_trends(
    period: str = Query('monthly', regex='^(monthly|quarterly|yearly)$'),
    property_type: str = Query(None),
    city: str = Query(None),
):
    return compute_trends(PROPERTIES, period=period, property_type=property_type, city=city)


@app.get("/ds-api/market-analysis/zones")
def get_zones():
    return zone_analysis(PROPERTIES)


@app.get("/ds-api/market-analysis/popular")
def get_popular():
    return {
        'types': popular_types(PROPERTIES),
        'features': popular_features(PROPERTIES),
        'conditions': popular_conditions(PROPERTIES),
        'avgPriceByType': avg_price_by_type(PROPERTIES),
    }


@app.get("/ds-api/market-analysis/predictions")
def get_predictions(months: int = Query(6, ge=1, le=24)):
    return predict_prices(PROPERTIES, months=months)


@app.get("/ds-api/market-analysis/forecast-by-type")
def get_forecast_by_type(months: int = Query(6, ge=1, le=24)):
    return price_forecast_by_type(PROPERTIES, months=months)


@app.get("/ds-api/market-analysis/price-distribution")
def get_price_distribution(bins: int = Query(10, ge=5, le=50)):
    return price_distribution(PROPERTIES, bins=bins)


@app.get("/ds-api/market-analysis/full")
def get_full_analysis(
    period: str = Query('monthly', regex='^(monthly|quarterly|yearly)$'),
    property_type: str = Query(None),
    city: str = Query(None),
    months: int = Query(6, ge=1, le=24),
):
    trends = compute_trends(PROPERTIES, period=period, property_type=property_type, city=city)
    zones = zone_analysis(PROPERTIES)
    popular = {
        'types': popular_types(PROPERTIES),
        'features': popular_features(PROPERTIES),
        'conditions': popular_conditions(PROPERTIES),
        'avgPriceByType': avg_price_by_type(PROPERTIES),
    }
    predictions = predict_prices(PROPERTIES, months=months)
    forecast_by_type = price_forecast_by_type(PROPERTIES, months=months)
    dist = price_distribution(PROPERTIES)

    return {
        'trends': trends,
        'zones': zones,
        'popular': popular,
        'predictions': predictions,
        'forecastByType': forecast_by_type,
        'priceDistribution': dist,
    }
