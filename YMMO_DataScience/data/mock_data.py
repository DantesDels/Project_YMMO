import random
from datetime import datetime, timedelta

random.seed(42)

french_cities = [
    {'name': 'Paris', 'zipCode': '75000', 'lat': 48.8566, 'lng': 2.3522, 'mult': 2.5},
    {'name': 'Marseille', 'zipCode': '13000', 'lat': 43.2965, 'lng': 5.3698, 'mult': 1.3},
    {'name': 'Lyon', 'zipCode': '69000', 'lat': 45.7640, 'lng': 4.8357, 'mult': 1.8},
    {'name': 'Toulouse', 'zipCode': '31000', 'lat': 43.6047, 'lng': 1.4442, 'mult': 1.4},
    {'name': 'Nice', 'zipCode': '06000', 'lat': 43.7102, 'lng': 7.2620, 'mult': 1.9},
    {'name': 'Nantes', 'zipCode': '44000', 'lat': 47.2184, 'lng': -1.5536, 'mult': 1.3},
    {'name': 'Montpellier', 'zipCode': '34000', 'lat': 43.6108, 'lng': 3.8767, 'mult': 1.2},
    {'name': 'Strasbourg', 'zipCode': '67000', 'lat': 48.5833, 'lng': 7.7500, 'mult': 1.3},
    {'name': 'Bordeaux', 'zipCode': '33000', 'lat': 44.8378, 'lng': -0.5792, 'mult': 1.6},
    {'name': 'Lille', 'zipCode': '59000', 'lat': 50.6292, 'lng': 3.0573, 'mult': 1.2},
    {'name': 'Rennes', 'zipCode': '35000', 'lat': 48.1173, 'lng': -1.6778, 'mult': 1.2},
    {'name': 'Reims', 'zipCode': '51100', 'lat': 49.2578, 'lng': 4.0317, 'mult': 1.0},
    {'name': 'Le Havre', 'zipCode': '76600', 'lat': 49.4938, 'lng': 0.1077, 'mult': 0.9},
    {'name': 'Saint-Étienne', 'zipCode': '42000', 'lat': 45.4397, 'lng': 4.3872, 'mult': 0.7},
    {'name': 'Toulon', 'zipCode': '83000', 'lat': 43.1250, 'lng': 5.9300, 'mult': 1.1},
]

property_types = ['House', 'Apartment', 'Land', 'Commercial', 'Office', 'Garage', 'Parking']
conditions = ['New', 'Excellent', 'Good', 'NeedsRefresh', 'NeedsRenovation', 'Ruin']
energy_classes = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'Ex']
all_criteria = ['Studio', 'Balcony', 'Terrace', 'Garden', 'Garage', 'Parking', 'Cellar', 'SwimmingPool', 'Elevator', 'AirConditioning', 'FiberOptic', 'SmartHome']
furnishing_options = ['Meublé', 'Non meublé']

base_prices = {'House': 350000, 'Apartment': 250000, 'Land': 150000, 'Commercial': 400000, 'Office': 300000, 'Garage': 50000, 'Parking': 30000}
start_ts = int(datetime(2024, 1, 1).timestamp())
end_ts = int(datetime(2026, 12, 31, 23, 59, 59).timestamp())
range_ts = end_ts - start_ts


def vary_zip(base):
    prefix = base[:-2]
    suffix = f"{random.randint(0, 99):02d}"
    return f"{prefix}{suffix}"


def generate_mock_properties(count=2_000_000):
    properties = []

    for i in range(count):
        city_obj = random.choice(french_cities)
        city = city_obj['name']
        lat = city_obj['lat'] + (random.random() - 0.5) * 0.08
        lng = city_obj['lng'] + (random.random() - 0.5) * 0.08

        prop_type = random.choice(property_types)
        base_price = base_prices[prop_type]
        city_mult = city_obj['mult']
        surface = random.randint(20, 220)
        rooms = max(1, min(7, int(surface / 30) + random.randint(-1, 2)))
        price = int(base_price * city_mult * (surface / 80) * (0.7 + random.random() * 0.6))
        price = max(20000, price)
        pic_count = random.randint(3, 6)

        ts = random.randint(start_ts, end_ts)
        created_at = datetime.fromtimestamp(ts)

        properties.append({
            'id': f'prop-{i}',
            'title': prop_type,
            'type': prop_type,
            'condition': random.choice(conditions),
            'energyClass': random.choice(energy_classes),
            'price': price,
            'surface': surface,
            'address': f"{city} ({vary_zip(city_obj['zipCode'])})",
            'city': city,
            'postalCode': vary_zip(city_obj['zipCode']),
            'rooms': rooms,
            'bedrooms': max(1, rooms - random.randint(1, 3)),
            'furnishing': random.choice(furnishing_options),
            'yearBuilt': random.randint(1945, 2025),
            'latitude': lat,
            'longitude': lng,
            'mainFeatures': [c for c in all_criteria if random.random() > 0.7],
            'available': True,
            'createdAt': created_at.isoformat(),
            'typeCategory': 'residential' if prop_type in ['House', 'Apartment'] else 'commercial',
        })

    return properties
