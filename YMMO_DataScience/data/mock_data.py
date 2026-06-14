import random
import math
from datetime import datetime, timedelta

random.seed(42)

french_cities = [
    {'name': 'Paris', 'zipCode': '75000', 'lat': 48.8566, 'lng': 2.3522},
    {'name': 'Marseille', 'zipCode': '13000', 'lat': 43.2965, 'lng': 5.3698},
    {'name': 'Lyon', 'zipCode': '69000', 'lat': 45.7640, 'lng': 4.8357},
    {'name': 'Toulouse', 'zipCode': '31000', 'lat': 43.6047, 'lng': 1.4442},
    {'name': 'Nice', 'zipCode': '06000', 'lat': 43.7102, 'lng': 7.2620},
    {'name': 'Nantes', 'zipCode': '44000', 'lat': 47.2184, 'lng': -1.5536},
    {'name': 'Montpellier', 'zipCode': '34000', 'lat': 43.6108, 'lng': 3.8767},
    {'name': 'Strasbourg', 'zipCode': '67000', 'lat': 48.5833, 'lng': 7.7500},
    {'name': 'Bordeaux', 'zipCode': '33000', 'lat': 44.8378, 'lng': -0.5792},
    {'name': 'Lille', 'zipCode': '59000', 'lat': 50.6292, 'lng': 3.0573},
    {'name': 'Rennes', 'zipCode': '35000', 'lat': 48.1173, 'lng': -1.6778},
    {'name': 'Reims', 'zipCode': '51100', 'lat': 49.2578, 'lng': 4.0317},
    {'name': 'Le Havre', 'zipCode': '76600', 'lat': 49.4938, 'lng': 0.1077},
    {'name': 'Saint-Étienne', 'zipCode': '42000', 'lat': 45.4397, 'lng': 4.3872},
    {'name': 'Toulon', 'zipCode': '83000', 'lat': 43.1250, 'lng': 5.9300},
]

property_types = ['House', 'Apartment', 'Land', 'Commercial', 'Office', 'Garage', 'Parking']
conditions = ['New', 'Excellent', 'Good', 'NeedsRefresh', 'NeedsRenovation', 'Ruin']
energy_classes = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'Ex']
all_criteria = ['Studio', 'Balcony', 'Terrace', 'Garden', 'Garage', 'Parking', 'Cellar', 'SwimmingPool', 'Elevator', 'AirConditioning', 'FiberOptic', 'SmartHome']
furnishing_options = ['Meublé', 'Non meublé']


def vary_zip(base):
    prefix = base[:-2]
    suffix = f"{random.randint(0, 99):02d}"
    return f"{prefix}{suffix}"


def generate_mock_properties(count=400):
    properties = []
    now = datetime.now()

    for i in range(count):
        city_obj = random.choice(french_cities)
        city = city_obj['name']
        lat = city_obj['lat'] + (random.random() - 0.5) * 0.08
        lng = city_obj['lng'] + (random.random() - 0.5) * 0.08

        prop_type = random.choice(property_types)
        base_price = {
            'House': 350000, 'Apartment': 250000, 'Land': 150000,
            'Commercial': 400000, 'Office': 300000, 'Garage': 50000, 'Parking': 30000
        }[prop_type]
        city_mult = {
            'Paris': 2.5, 'Marseille': 1.3, 'Lyon': 1.8, 'Toulouse': 1.4,
            'Nice': 1.9, 'Nantes': 1.3, 'Montpellier': 1.2, 'Strasbourg': 1.3,
            'Bordeaux': 1.6, 'Lille': 1.2, 'Rennes': 1.2, 'Reims': 1.0,
            'Le Havre': 0.9, 'Saint-Étienne': 0.7, 'Toulon': 1.1
        }[city]
        surface = random.randint(20, 220)
        rooms = max(1, min(7, int(surface / 30) + random.randint(-1, 2)))
        price = int(base_price * city_mult * (surface / 80) * (0.7 + random.random() * 0.6))
        price = max(20000, price)

        pic_count = random.randint(3, 6)
        created_at = now - timedelta(days=random.randint(0, 365))

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
