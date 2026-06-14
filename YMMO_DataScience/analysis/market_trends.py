from collections import defaultdict
from datetime import datetime


def compute_trends(properties, period='monthly', property_type=None, city=None):
    filtered = properties
    if property_type:
        filtered = [p for p in filtered if p['type'] == property_type]
    if city:
        filtered = [p for p in filtered if p['city'] == city]

    if not filtered:
        return {'error': 'No data for the given filters'}

    by_period = defaultdict(list)
    for p in filtered:
        dt = datetime.fromisoformat(p['createdAt'])
        if period == 'yearly':
            key = dt.strftime('%Y')
        elif period == 'quarterly':
            quarter = (dt.month - 1) // 3 + 1
            key = f"{dt.year}-Q{quarter}"
        else:
            key = dt.strftime('%Y-%m')

        by_period[key].append(p)

    sorted_keys = sorted(by_period.keys())
    trend_data = []
    for key in sorted_keys:
        props = by_period[key]
        prices = [p['price'] for p in props]
        surfaces = [p['surface'] for p in props]
        avg_price = sum(prices) / len(prices)
        avg_surface = sum(surfaces) / len(surfaces)
        avg_price_per_m2 = avg_price / avg_surface if avg_surface > 0 else 0

        trend_data.append({
            'period': key,
            'count': len(props),
            'avgPrice': round(avg_price, 2),
            'avgSurface': round(avg_surface, 2),
            'avgPricePerM2': round(avg_price_per_m2, 2),
            'minPrice': min(prices),
            'maxPrice': max(prices),
            'totalVolume': sum(prices),
        })

    all_prices = [p['price'] for p in filtered]
    all_surfaces = [p['surface'] for p in filtered]

    return {
        'trends': trend_data,
        'summary': {
            'totalListings': len(filtered),
            'globalAvgPrice': round(sum(all_prices) / len(all_prices), 2),
            'globalAvgPricePerM2': round(sum(all_prices) / sum(all_surfaces), 2) if sum(all_surfaces) > 0 else 0,
            'globalAvgSurface': round(sum(all_surfaces) / len(all_surfaces), 2),
            'minPrice': min(all_prices),
            'maxPrice': max(all_prices),
            'period': period,
        },
        'filters': {
            'propertyType': property_type or 'all',
            'city': city or 'all',
        },
    }


def price_distribution(properties, bins=10):
    prices = [p['price'] for p in properties]
    if not prices:
        return {'error': 'No data'}
    min_p = min(prices)
    max_p = max(prices)
    step = (max_p - min_p) / bins if bins > 0 else 1
    distribution = []
    for i in range(bins):
        low = min_p + i * step
        high = low + step
        count = sum(1 for p in prices if low <= p < high)
        distribution.append({
            'range': f"{int(low)}-{int(high)}",
            'low': int(low),
            'high': int(high),
            'count': count,
        })
    return {'distribution': distribution, 'total': len(prices)}
