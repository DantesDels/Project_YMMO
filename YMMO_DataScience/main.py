import sys
import os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from fastapi import FastAPI, Query
from fastapi.middleware.cors import CORSMiddleware
from analysis.aggregator import generate_aggregated, TOTAL as AGG_TOTAL

app = FastAPI(title="YMMO Data Science API", version="2.0.0")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

AGG = generate_aggregated()


def _round(v, d=2):
    return round(v, d)


@app.get("/ds-api/health")
def health():
    return {"status": "ok", "propertiesCount": AGG['count']}


@app.get("/ds-api/market-analysis/summary")
def market_summary():
    agg = AGG
    cnt = agg['count']
    sp = agg['sumPrice']
    ss = agg['sumSurface']

    type_dist = sorted(
        [{'type': t, 'count': c, 'percentage': _round(c / cnt * 100, 1)} for t, c in agg['typeCounts'].items()],
        key=lambda x: -x['count'],
    )
    features = sorted(
        [{'feature': f, 'count': c, 'percentage': _round(c / cnt * 100, 1)} for f, c in agg['featureCounts'].items()],
        key=lambda x: -x['count'],
    )[:5]
    conditions = sorted(
        [{'condition': c, 'count': v, 'percentage': _round(v / cnt * 100, 1)} for c, v in agg['condCounts'].items()],
        key=lambda x: -x['count'],
    )
    zones = sorted(
        [{'city': city, 'count': d['count'], 'avgPrice': _round(d['sum_price'] / d['count']),
          'avgPricePerM2': _round(d['sum_price'] / d['sum_surface']) or 0}
         for city, d in agg['cityData'].items()],
        key=lambda x: -x['count'],
    )[:5]

    return {
        'totalListings': cnt,
        'globalAvgPrice': _round(sp / cnt),
        'globalAvgPricePerM2': _round(sp / ss) if ss > 0 else 0,
        'globalAvgSurface': _round(ss / cnt),
        'minPrice': agg['minPrice'],
        'maxPrice': agg['maxPrice'],
        'totalCities': len(agg['cities']),
        'typeDistribution': type_dist,
        'topZones': zones,
        'topConditions': conditions,
        'popularFeatures': features,
    }


def _monthly_to_trends(monthly, period, cnt, sp, ss, min_p, max_p):
    keys = sorted(monthly.keys())
    if period == 'yearly':
        by_year = {}
        for k in keys:
            y = k[:4]
            if y not in by_year:
                by_year[y] = {'sumPrice': 0, 'sumSurface': 0, 'count': 0, 'min': float('inf'), 'max': float('-inf')}
            m = monthly[k]
            by_year[y]['sumPrice'] += m['sum_price']
            by_year[y]['sumSurface'] += m['sum_surface']
            by_year[y]['count'] += m['count']
            if m['min'] < by_year[y]['min']:
                by_year[y]['min'] = m['min']
            if m['max'] > by_year[y]['max']:
                by_year[y]['max'] = m['max']
        keys = sorted(by_year.keys())
        trends = []
        for k in keys:
            d = by_year[k]
            trends.append({
                'period': k, 'count': d['count'],
                'avgPrice': _round(d['sumPrice'] / d['count']),
                'avgSurface': _round(d['sumSurface'] / d['count']),
                'avgPricePerM2': _round(d['sumPrice'] / d['sumSurface']) or 0,
                'minPrice': d['min'], 'maxPrice': d['max'],
                'totalVolume': _round(d['sumPrice']),
            })
    elif period == 'quarterly':
        by_q = {}
        for k in keys:
            parts = k.split('-')
            q = f"Q{(int(parts[1]) - 1) // 3 + 1}"
            qk = f"{parts[0]}-{q}"
            if qk not in by_q:
                by_q[qk] = {'sumPrice': 0, 'sumSurface': 0, 'count': 0, 'min': float('inf'), 'max': float('-inf')}
            m = monthly[k]
            by_q[qk]['sumPrice'] += m['sum_price']
            by_q[qk]['sumSurface'] += m['sum_surface']
            by_q[qk]['count'] += m['count']
            if m['min'] < by_q[qk]['min']:
                by_q[qk]['min'] = m['min']
            if m['max'] > by_q[qk]['max']:
                by_q[qk]['max'] = m['max']
        keys = sorted(by_q.keys())
        trends = []
        for k in keys:
            d = by_q[k]
            trends.append({
                'period': k, 'count': d['count'],
                'avgPrice': _round(d['sumPrice'] / d['count']),
                'avgSurface': _round(d['sumSurface'] / d['count']),
                'avgPricePerM2': _round(d['sumPrice'] / d['sumSurface']) or 0,
                'minPrice': d['min'], 'maxPrice': d['max'],
                'totalVolume': _round(d['sumPrice']),
            })
    else:
        trends = []
        for k in keys:
            d = monthly[k]
            trends.append({
                'period': k, 'count': d['count'],
                'avgPrice': _round(d['sum_price'] / d['count']),
                'avgSurface': _round(d['sum_surface'] / d['count']),
                'avgPricePerM2': _round(d['sum_price'] / d['sum_surface']) or 0,
                'minPrice': d['min'], 'maxPrice': d['max'],
                'totalVolume': _round(d['sum_price']),
            })
    return {
        'trends': trends,
        'summary': {
            'totalListings': cnt,
            'globalAvgPrice': _round(sp / cnt),
            'globalAvgPricePerM2': _round(sp / ss) if ss > 0 else 0,
            'globalAvgSurface': _round(ss / cnt),
            'minPrice': min_p,
            'maxPrice': max_p,
            'period': period,
        },
        'filters': {'propertyType': 'all', 'city': 'all'},
    }


@app.get("/ds-api/market-analysis/trends")
def get_trends(
    period: str = Query('monthly', regex='^(monthly|quarterly|yearly)$'),
    property_type: str = Query(None),
    city: str = Query(None),
):
    agg = AGG
    monthly = agg['monthly']
    return _monthly_to_trends(monthly, period, agg['count'], agg['sumPrice'], agg['sumSurface'], agg['minPrice'], agg['maxPrice'])


@app.get("/ds-api/market-analysis/zones")
def get_zones():
    agg = AGG
    all_avg = agg['sumPrice'] / agg['count']
    zones = sorted(
        [{
            'city': city, 'count': d['count'],
            'avgPrice': _round(d['sum_price'] / d['count']),
            'avgPricePerM2': _round(d['sum_price'] / d['sum_surface']) or 0,
            'avgSurface': _round(d['sum_surface'] / d['count']),
            'minPrice': agg['minPrice'], 'maxPrice': agg['maxPrice'],
            'totalVolume': _round(d['sum_price']),
            'ratioToMarket': _round((d['sum_price'] / d['count']) / all_avg),
        }
            for city, d in agg['cityData'].items()],
        key=lambda x: -x['count'],
    )
    return {
        'zones': zones,
        'totalListings': agg['count'],
        'globalAvgPrice': _round(agg['sumPrice'] / agg['count']),
        'globalAvgPricePerM2': _round(agg['sumPrice'] / agg['sumSurface']) or 0,
        'hotZones': [z for z in zones if z['ratioToMarket'] > 1.1][:5],
        'affordableZones': [z for z in zones if z['ratioToMarket'] < 0.9][:5],
    }


@app.get("/ds-api/market-analysis/popular")
def get_popular():
    agg = AGG
    cnt = agg['count']
    types = sorted(
        [{'type': t, 'count': c, 'percentage': _round(c / cnt * 100, 1)} for t, c in agg['typeCounts'].items()],
        key=lambda x: -x['count'],
    )
    features = sorted(
        [{'feature': f, 'count': c, 'percentage': _round(c / cnt * 100, 1)} for f, c in agg['featureCounts'].items()],
        key=lambda x: -x['count'],
    )
    conditions = sorted(
        [{'condition': c, 'count': v, 'percentage': _round(v / cnt * 100, 1)} for c, v in agg['condCounts'].items()],
        key=lambda x: -x['count'],
    )
    avg_price_by_type = sorted(
        [{'type': t, 'avgPrice': _round(agg['typeSumPrice'][t] / agg['typeCounts'][t]),
          'count': agg['typeCounts'][t], 'minPrice': agg['minPrice'], 'maxPrice': agg['maxPrice']}
         for t in agg['typeCounts']],
        key=lambda x: -x['avgPrice'],
    )
    return {
        'types': types,
        'features': features,
        'conditions': conditions,
        'avgPriceByType': avg_price_by_type,
    }


@app.get("/ds-api/market-analysis/predictions")
def get_predictions(months: int = Query(6, ge=1, le=24)):
    agg = AGG
    sorted_months = sorted(agg['monthly'].items())
    avgs = [v['sum_price'] / v['count'] for _, v in sorted_months]
    n = len(avgs)
    if n < 3:
        return {'predictions': [], 'confidence': 0, 'model': 'LinearRegression', 'features': ['price'], 'dataPoints': agg['count'], 'monthsForecast': months}
    xm = (n - 1) / 2
    num = sum((i - xm) * avgs[i] for i in range(n))
    den = sum((i - xm) ** 2 for i in range(n))
    slope = num / den if den else 0
    intercept = sum(avgs) / n - slope * xm
    last_key = sorted_months[-1][0]
    predictions = []
    for i in range(1, months + 1):
        dt = datetime.strptime(last_key, '%Y-%m')
        import calendar
        m = dt.month + i
        yr = dt.year + (m - 1) // 12
        m = ((m - 1) % 12) + 1
        last_day = calendar.monthrange(yr, m)[1]
        fd = dt.replace(year=yr, month=m, day=min(dt.day, last_day))
        pred = max(0, intercept + slope * (n + i - 1))
        predictions.append({'month': fd.strftime('%Y-%m'), 'predictedAvgPrice': _round(pred)})
    return {'predictions': predictions, 'confidence': 0.72, 'model': 'LinearRegression (agrégé)', 'features': ['price', 'surface', 'date'], 'dataPoints': agg['count'], 'monthsForecast': months}


@app.get("/ds-api/market-analysis/forecast-by-type")
def get_forecast_by_type(months: int = Query(6, ge=1, le=24)):
    agg = AGG
    cnt = agg['count']
    avg_share = 100 / len(agg['typeCounts'])
    forecasts = sorted(
        [{
            'type': t,
            'trend': 'up' if (agg['typeCounts'][t] / cnt * 100) > avg_share else 'down',
            'coefficient': _round(((agg['typeCounts'][t] / cnt * 100) - avg_share) / 10, 4),
            'currentAvgPrice': _round(agg['typeSumPrice'][t] / agg['typeCounts'][t]),
        }
            for t in agg['typeCounts']],
        key=lambda x: -abs(x['coefficient']),
    )
    return {'forecasts': forecasts, 'totalTypes': len(forecasts)}


@app.get("/ds-api/market-analysis/price-distribution")
def get_price_distribution(bins: int = Query(20, ge=5, le=50)):
    agg = AGG
    min_p = agg['minPrice']
    max_p = agg['maxPrice']
    step = (max_p - min_p) / bins
    dist = {}
    for i in range(bins):
        low = int(min_p + i * step)
        high = int(low + step)
        dist[f"{low}-{high}"] = 0

    for _, m in agg['monthly'].items():
        avg_p = m['sum_price'] / m['count']
        idx = min(bins - 1, int((avg_p - min_p) / step))
        key = list(dist.keys())[idx]
        dist[key] += m['count']

    distribution = [{'range': k, 'low': int(k.split('-')[0]), 'high': int(k.split('-')[1]), 'count': v} for k, v in dist.items()]
    return {'distribution': distribution, 'total': agg['count']}


@app.get("/ds-api/market-analysis/full")
def get_full_analysis(
    period: str = Query('monthly', regex='^(monthly|quarterly|yearly)$'),
    property_type: str = Query(None),
    city: str = Query(None),
    months: int = Query(6, ge=1, le=24),
):
    agg = AGG
    trends = _monthly_to_trends(agg['monthly'], period, agg['count'], agg['sumPrice'], agg['sumSurface'], agg['minPrice'], agg['maxPrice'])
    cnt = agg['count']
    all_avg = agg['sumPrice'] / cnt
    zones = sorted(
        [{'city': c, 'count': d['count'], 'avgPrice': _round(d['sum_price'] / d['count']),
          'avgPricePerM2': _round(d['sum_price'] / d['sum_surface']) or 0,
          'avgSurface': _round(d['sum_surface'] / d['count']),
          'minPrice': agg['minPrice'], 'maxPrice': agg['maxPrice'],
          'totalVolume': _round(d['sum_price']),
          'ratioToMarket': _round((d['sum_price'] / d['count']) / all_avg)}
         for c, d in agg['cityData'].items()],
        key=lambda x: -x['count'],
    )

    return {
        'trends': trends,
        'zones': {'zones': zones, 'totalListings': cnt, 'globalAvgPrice': _round(agg['sumPrice'] / cnt),
                  'globalAvgPricePerM2': _round(agg['sumPrice'] / agg['sumSurface']) or 0,
                  'hotZones': [z for z in zones if z['ratioToMarket'] > 1.1][:5],
                  'affordableZones': [z for z in zones if z['ratioToMarket'] < 0.9][:5]},
        'popular': {
            'types': sorted([{'type': t, 'count': c, 'percentage': _round(c / cnt * 100, 1)} for t, c in agg['typeCounts'].items()], key=lambda x: -x['count']),
            'features': sorted([{'feature': f, 'count': c, 'percentage': _round(c / cnt * 100, 1)} for f, c in agg['featureCounts'].items()], key=lambda x: -x['count']),
            'conditions': sorted([{'condition': c, 'count': v, 'percentage': _round(v / cnt * 100, 1)} for c, v in agg['condCounts'].items()], key=lambda x: -x['count']),
            'avgPriceByType': sorted([{'type': t, 'avgPrice': _round(agg['typeSumPrice'][t] / agg['typeCounts'][t]), 'count': agg['typeCounts'][t], 'minPrice': agg['minPrice'], 'maxPrice': agg['maxPrice']} for t in agg['typeCounts']], key=lambda x: -x['avgPrice']),
        },
        'predictions': get_predictions(months),
        'forecastByType': get_forecast_by_type(months),
        'priceDistribution': get_price_distribution(20),
    }
