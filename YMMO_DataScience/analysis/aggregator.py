import math
from datetime import datetime

TOTAL = 2_000_000
CITIES = [
    {'name': 'Paris', 'mult': 2.5}, {'name': 'Marseille', 'mult': 1.3}, {'name': 'Lyon', 'mult': 1.8},
    {'name': 'Toulouse', 'mult': 1.4}, {'name': 'Nice', 'mult': 1.9}, {'name': 'Nantes', 'mult': 1.3},
    {'name': 'Montpellier', 'mult': 1.2}, {'name': 'Strasbourg', 'mult': 1.3}, {'name': 'Bordeaux', 'mult': 1.6},
    {'name': 'Lille', 'mult': 1.2}, {'name': 'Rennes', 'mult': 1.2}, {'name': 'Reims', 'mult': 1.0},
    {'name': 'Le Havre', 'mult': 0.9}, {'name': 'Saint-Étienne', 'mult': 0.7}, {'name': 'Toulon', 'mult': 1.1},
]
TYPES = ['House', 'Apartment', 'Land', 'Commercial', 'Office', 'Garage', 'Parking']
CONDS = ['New', 'Excellent', 'Good', 'NeedsRefresh', 'NeedsRenovation', 'Ruin']
FEATURES = ['Studio', 'Balcony', 'Terrace', 'Garden', 'Garage', 'Parking', 'Cellar', 'SwimmingPool', 'Elevator', 'AirConditioning', 'FiberOptic', 'SmartHome']
BASE_PRICES = {'House': 350000, 'Apartment': 250000, 'Land': 150000, 'Commercial': 400000, 'Office': 300000, 'Garage': 50000, 'Parking': 30000}
START_TS = int(datetime(2024, 1, 1).timestamp())
END_TS = int(datetime(2026, 12, 31, 23, 59, 59).timestamp())
RANGE_TS = END_TS - START_TS

def _rand(seed):
    x = math.sin(seed * 9301 + 49297) * 49297
    return x - math.floor(x)


def _gen_one(i):
    s = i
    city_obj = CITIES[int(_rand(s + 1) * len(CITIES))]
    ptype = TYPES[int(_rand(s + 2) * len(TYPES))]
    cond = CONDS[int(_rand(s + 3) * len(CONDS))]
    surface = int(_rand(s + 4) * 200) + 20
    rooms = max(1, min(7, int(surface / 30) + int(_rand(s + 5) * 4) - 1))
    base = BASE_PRICES[ptype]
    price = max(20000, int(base * city_obj['mult'] * (surface / 80) * (0.7 + _rand(s + 6) * 0.6)))
    ts = START_TS + int(_rand(s + 7) * RANGE_TS)
    dt = datetime.fromtimestamp(ts)
    month_key = dt.strftime('%Y-%m')
    return ptype, cond, surface, rooms, price, month_key, city_obj['name']


def generate_aggregated():
    count = 0
    sum_price = 0
    sum_surface = 0
    min_price = float('inf')
    max_price = float('-inf')
    cities = set()

    type_count = {t: 0 for t in TYPES}
    type_sum_price = {t: 0 for t in TYPES}
    type_sum_surface = {t: 0 for t in TYPES}
    cond_count = {c: 0 for c in CONDS}
    feat_count = {f: 0 for f in FEATURES}

    city_data = {}

    monthly = {}

    for i in range(TOTAL):
        ptype, cond, surface, rooms, price, month_key, city = _gen_one(i)

        count += 1
        sum_price += price
        sum_surface += surface
        if price < min_price: min_price = price
        if price > max_price: max_price = price
        cities.add(city)

        type_count[ptype] += 1
        type_sum_price[ptype] += price
        type_sum_surface[ptype] += surface
        cond_count[cond] += 1

        f_seed = int(_rand(i + 8) * 100)
        for fi, feat in enumerate(FEATURES):
            if f_seed + fi * 7 > 68 + int(_rand(i + 10 + fi) * 22):
                feat_count[feat] += 1

        if city not in city_data:
            city_data[city] = {'sum_price': 0, 'sum_surface': 0, 'count': 0}
        city_data[city]['sum_price'] += price
        city_data[city]['sum_surface'] += surface
        city_data[city]['count'] += 1

        if month_key not in monthly:
            monthly[month_key] = {'sum_price': 0, 'sum_surface': 0, 'count': 0, 'min': float('inf'), 'max': float('-inf')}
        m = monthly[month_key]
        m['sum_price'] += price
        m['sum_surface'] += surface
        m['count'] += 1
        if price < m['min']: m['min'] = price
        if price > m['max']: m['max'] = price

    return {
        'count': count,
        'sumPrice': sum_price,
        'sumSurface': sum_surface,
        'minPrice': min_price,
        'maxPrice': max_price,
        'cities': list(cities),
        'typeCounts': type_count,
        'typeSumPrice': type_sum_price,
        'typeSumSurface': type_sum_surface,
        'condCounts': cond_count,
        'featureCounts': feat_count,
        'cityData': city_data,
        'monthly': monthly,
    }
