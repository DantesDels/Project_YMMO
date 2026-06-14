from collections import Counter


def popular_types(properties):
    type_counts = Counter(p['type'] for p in properties)
    total = len(properties)
    return {
        'types': [
            {'type': t, 'count': c, 'percentage': round(c / total * 100, 1)}
            for t, c in type_counts.most_common()
        ],
        'total': total,
    }


def popular_features(properties):
    feature_counter = Counter()
    for p in properties:
        for f in p['mainFeatures']:
            feature_counter[f] += 1
    total = len(properties)
    return {
        'features': [
            {'feature': f, 'count': c, 'percentage': round(c / total * 100, 1)}
            for f, c in feature_counter.most_common()
        ],
        'total': total,
    }


def popular_conditions(properties):
    cond_counts = Counter(p['condition'] for p in properties)
    total = len(properties)
    return {
        'conditions': [
            {'condition': c, 'count': cnt, 'percentage': round(cnt / total * 100, 1)}
            for c, cnt in cond_counts.most_common()
        ],
        'total': total,
    }


def avg_price_by_type(properties):
    from collections import defaultdict
    by_type = defaultdict(list)
    for p in properties:
        by_type[p['type']].append(p['price'])
    return [
        {'type': t, 'avgPrice': round(sum(v) / len(v), 2), 'count': len(v), 'minPrice': min(v), 'maxPrice': max(v)}
        for t, v in sorted(by_type.items())
    ]
