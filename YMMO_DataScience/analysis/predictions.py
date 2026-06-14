import numpy as np
from sklearn.linear_model import LinearRegression
from datetime import datetime


def predict_prices(properties, months=6):
    filtered = [p for p in properties if p['createdAt']]
    if len(filtered) < 5:
        return {'error': 'Not enough data for predictions (need at least 5 properties)'}

    X = []
    y = []
    for p in filtered:
        dt = datetime.fromisoformat(p['createdAt'])
        days_since_epoch = (dt - datetime(2020, 1, 1)).days
        X.append([days_since_epoch, p['surface'], p['rooms']])
        y.append(p['price'])

    X = np.array(X)
    y = np.array(y)

    model = LinearRegression()
    model.fit(X, y)

    last_date = max(datetime.fromisoformat(p['createdAt']) for p in filtered)
    predictions = []
    for i in range(1, months + 1):
        m = last_date.month + i
        yr = last_date.year + (m - 1) // 12
        m = ((m - 1) % 12) + 1
        try:
            future_date = last_date.replace(year=yr, month=m)
        except ValueError:
            import calendar
            last_day = calendar.monthrange(yr, m)[1]
            future_date = last_date.replace(year=yr, month=m, day=min(last_date.day, last_day))
        days_since_epoch = (future_date - datetime(2020, 1, 1)).days
        future_X = np.array([[days_since_epoch, np.mean(X[:, 1]), np.mean(X[:, 2])]])
        pred_price = model.predict(future_X)[0]
        predictions.append({
            'month': future_date.strftime('%Y-%m'),
            'predictedAvgPrice': round(float(pred_price), 2),
        })

    confidence = model.score(X, y)

    return {
        'predictions': predictions,
        'confidence': round(float(confidence), 4),
        'model': 'LinearRegression',
        'features': ['daysSinceEpoch', 'surface', 'rooms'],
        'dataPoints': len(filtered),
        'monthsForecast': months,
    }


def price_forecast_by_type(properties, months=6):
    from collections import defaultdict
    by_type = defaultdict(list)
    for p in properties:
        dt = datetime.fromisoformat(p['createdAt'])
        days = (dt - datetime(2020, 1, 1)).days
        by_type[p['type']].append({'days': days, 'price': p['price']})

    results = []
    for prop_type, data in by_type.items():
        if len(data) < 3:
            continue
        X = np.array([[d['days']] for d in data])
        y = np.array([d['price'] for d in data])
        model = LinearRegression()
        model.fit(X, y)

        last_days = max(d['days'] for d in data)
        trend = 'up' if model.coef_[0] > 0 else 'down'

        results.append({
            'type': prop_type,
            'trend': trend,
            'coefficient': round(float(model.coef_[0]), 4),
            'currentAvgPrice': round(float(y.mean()), 2),
        })

    results.sort(key=lambda r: abs(r['coefficient']), reverse=True)
    return {'forecasts': results, 'totalTypes': len(results)}
