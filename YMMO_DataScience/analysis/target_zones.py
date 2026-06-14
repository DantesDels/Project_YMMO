from collections import defaultdict


def zone_analysis(properties):
    by_city = defaultdict(list)
    for p in properties:
        by_city[p['city']].append(p)

    zones = []
    all_avg = sum(p['price'] for p in properties) / len(properties) if properties else 0
    all_avg_m2 = (sum(p['price'] for p in properties) / sum(p['surface'] for p in properties)
                  if sum(p['surface'] for p in properties) > 0 else 0)

    for city, props in sorted(by_city.items()):
        prices = [p['price'] for p in props]
        surfaces = [p['surface'] for p in props]
        avg_price = sum(prices) / len(prices)
        avg_surface = sum(surfaces) / len(surfaces)
        avg_price_m2 = avg_price / avg_surface if avg_surface > 0 else 0
        ratio_to_market = avg_price / all_avg if all_avg > 0 else 0

        zones.append({
            'city': city,
            'count': len(props),
            'avgPrice': round(avg_price, 2),
            'avgPricePerM2': round(avg_price_m2, 2),
            'avgSurface': round(avg_surface, 2),
            'minPrice': min(prices),
            'maxPrice': max(prices),
            'totalVolume': round(sum(prices), 2),
            'ratioToMarket': round(ratio_to_market, 2),
            'typeDistribution': dict(Counter(p['type'] for p in props)),
        })

    zones.sort(key=lambda z: z['count'], reverse=True)

    hot_zones = [z for z in zones if z['ratioToMarket'] > 1.1]
    affordable_zones = [z for z in zones if z['ratioToMarket'] < 0.9]

    return {
        'zones': zones,
        'totalListings': len(properties),
        'globalAvgPrice': round(all_avg, 2),
        'globalAvgPricePerM2': round(all_avg_m2, 2),
        'hotZones': hot_zones[:5],
        'affordableZones': affordable_zones[:5],
    }


from collections import Counter
