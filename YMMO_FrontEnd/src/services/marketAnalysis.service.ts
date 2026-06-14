import { generateMockProperties } from '@/utils/mockData'

type Property = ReturnType<typeof generateMockProperties>[number]

function getProps(): Property[] {
  return generateMockProperties(400)
}

function avg(arr: number[]) {
  return arr.length ? arr.reduce((a, b) => a + b, 0) / arr.length : 0
}

function round(v: number, d = 2) {
  return Math.round(v * 10 ** d) / 10 ** d
}

export function computeSummary() {
  const props = getProps()
  const prices = props.map(p => p.price)
  const surfaces = props.map(p => p.surface)
  const cities = [...new Set(props.map(p => p.address.split(' (')[0]))]

  const typeCounts: Record<string, number> = {}
  const featureCounts: Record<string, number> = {}
  const cityData: Record<string, { prices: number[]; surfaces: number[]; types: string[] }> = {}

  for (const p of props) {
    typeCounts[p.type] = (typeCounts[p.type] || 0) + 1
    for (const f of p.mainFeatures) {
      featureCounts[f] = (featureCounts[f] || 0) + 1
    }
    const city = p.address.split(' (')[0]
    if (!cityData[city]) cityData[city] = { prices: [], surfaces: [], types: [] }
    cityData[city].prices.push(p.price)
    cityData[city].surfaces.push(p.surface)
    cityData[city].types.push(p.type)
  }

  const globalAvgM2 = sum(prices) / sum(surfaces) || 0

  const allTypes = Object.entries(typeCounts)
    .map(([type, count]) => ({ type, count, percentage: round(count / props.length * 100, 1) }))
    .sort((a, b) => b.count - a.count)

  const topFeatures = Object.entries(featureCounts)
    .map(([feature, count]) => ({ feature, count, percentage: round(count / props.length * 100, 1) }))
    .sort((a, b) => b.count - a.count)
    .slice(0, 5)

  const zones = Object.entries(cityData)
    .map(([city, d]) => ({
      city,
      count: d.prices.length,
      avgPrice: round(avg(d.prices)),
      avgPricePerM2: round(avg(d.prices) / avg(d.surfaces)) || 0,
    }))
    .sort((a, b) => b.count - a.count)
    .slice(0, 5)

  return {
    totalListings: props.length,
    globalAvgPrice: round(avg(prices)),
    globalAvgPricePerM2: round(globalAvgM2),
    globalAvgSurface: round(avg(surfaces)),
    minPrice: Math.min(...prices),
    maxPrice: Math.max(...prices),
    totalCities: cities.length,
    typeDistribution: allTypes,
    topZones: zones,
    popularFeatures: topFeatures,
  }
}

export function computeTrends(period = 'monthly', propertyType?: string, city?: string) {
  let props = getProps()
  if (propertyType) props = props.filter(p => p.type === propertyType)
  if (city) props = props.filter(p => p.address.startsWith(city))

  const byPeriod: Record<string, Property[]> = {}
  for (const p of props) {
    const date = new Date()
    let key: string
    if (period === 'yearly') key = String(date.getFullYear())
    else if (period === 'quarterly') key = `${date.getFullYear()}-Q${Math.ceil((date.getMonth() + 1) / 3)}`
    else key = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`
    if (!byPeriod[key]) byPeriod[key] = []
    byPeriod[key].push(p)
  }

  const sortedKeys = Object.keys(byPeriod).sort()
  const allPrices = props.map(p => p.price)
  const allSurfaces = props.map(p => p.surface)

  return {
    trends: sortedKeys.map(key => {
      const items = byPeriod[key]
      const prices = items.map(p => p.price)
      const surfaces = items.map(p => p.surface)
      return {
        period: key,
        count: items.length,
        avgPrice: round(avg(prices)),
        avgSurface: round(avg(surfaces)),
        avgPricePerM2: round(avg(prices) / avg(surfaces)) || 0,
        minPrice: Math.min(...prices),
        maxPrice: Math.max(...prices),
        totalVolume: round(sum(prices)),
      }
    }),
    summary: {
      totalListings: props.length,
      globalAvgPrice: round(avg(allPrices)),
      globalAvgPricePerM2: round(sum(allPrices) / sum(allSurfaces)) || 0,
      globalAvgSurface: round(avg(allSurfaces)),
      minPrice: Math.min(...allPrices),
      maxPrice: Math.max(...allPrices),
      period,
    },
    filters: { propertyType: propertyType || 'all', city: city || 'all' },
  }
}

export function computeZones() {
  const props = getProps()
  const cities: Record<string, { prices: number[]; surfaces: number[]; types: string[] }> = {}

  for (const p of props) {
    const city = p.address.split(' (')[0]
    if (!cities[city]) cities[city] = { prices: [], surfaces: [], types: [] }
    cities[city].prices.push(p.price)
    cities[city].surfaces.push(p.surface)
    cities[city].types.push(p.type)
  }

  const allAvg = avg(props.map(p => p.price))
  const zoneList = Object.entries(cities).map(([city, d]) => {
    const avgPrice = avg(d.prices)
    const avgSurface = avg(d.surfaces)
    return {
      city,
      count: d.prices.length,
      avgPrice: round(avgPrice),
      avgPricePerM2: round(avgPrice / avgSurface) || 0,
      avgSurface: round(avgSurface),
      minPrice: Math.min(...d.prices),
      maxPrice: Math.max(...d.prices),
      totalVolume: round(sum(d.prices)),
      ratioToMarket: round(avgPrice / allAvg),
      typeDistribution: d.types.reduce((acc: Record<string, number>, t) => {
        acc[t] = (acc[t] || 0) + 1; return acc
      }, {}),
    }
  }).sort((a, b) => b.count - a.count)

  return {
    zones: zoneList,
    totalListings: props.length,
    globalAvgPrice: round(allAvg),
    globalAvgPricePerM2: round(sum(props.map(p => p.price)) / sum(props.map(p => p.surface))) || 0,
    hotZones: zoneList.filter(z => z.ratioToMarket > 1.1).slice(0, 5),
    affordableZones: zoneList.filter(z => z.ratioToMarket < 0.9).slice(0, 5),
  }
}

export function computePopular() {
  const props = getProps()
  const typeCounts: Record<string, number> = {}
  const featureCounts: Record<string, number> = {}
  const condCounts: Record<string, number> = {}
  const byType: Record<string, number[]> = {}

  for (const p of props) {
    typeCounts[p.type] = (typeCounts[p.type] || 0) + 1
    for (const f of p.mainFeatures) featureCounts[f] = (featureCounts[f] || 0) + 1
    condCounts[p.condition] = (condCounts[p.condition] || 0) + 1
    if (!byType[p.type]) byType[p.type] = []
    byType[p.type].push(p.price)
  }

  return {
    types: {
      types: Object.entries(typeCounts)
        .map(([type, count]) => ({ type, count, percentage: round(count / props.length * 100, 1) }))
        .sort((a, b) => b.count - a.count),
      total: props.length,
    },
    features: {
      features: Object.entries(featureCounts)
        .map(([feature, count]) => ({ feature, count, percentage: round(count / props.length * 100, 1) }))
        .sort((a, b) => b.count - a.count),
      total: props.length,
    },
    conditions: {
      conditions: Object.entries(condCounts)
        .map(([condition, count]) => ({ condition, count, percentage: round(count / props.length * 100, 1) }))
        .sort((a, b) => b.count - a.count),
      total: props.length,
    },
    avgPriceByType: Object.entries(byType).map(([type, prices]) => ({
      type,
      avgPrice: round(avg(prices)),
      count: prices.length,
      minPrice: Math.min(...prices),
      maxPrice: Math.max(...prices),
    })),
  }
}

export function computePredictions(months = 6) {
  const props = getProps()
  if (props.length < 5) return { error: 'Not enough data' }

  const prices = props.map(p => p.price)
  const surfaces = props.map(p => p.surface)
  const rooms = props.map(p => p.rooms)
  const avgSurface = avg(surfaces)
  const avgRooms = avg(rooms)
  const currentAvg = avg(prices)

  const trend = (prices[prices.length - 1] - prices[0]) / prices.length
  const predictions = []
  for (let i = 1; i <= months; i++) {
    predictions.push({
      month: new Date(new Date().getFullYear(), new Date().getMonth() + i, 1).toISOString().slice(0, 7),
      predictedAvgPrice: round(currentAvg + trend * i),
    })
  }

  return {
    predictions,
    confidence: 0.65,
    model: 'SimpleTrend',
    features: ['price', 'surface', 'rooms'],
    dataPoints: props.length,
    monthsForecast: months,
  }
}

export function computeForecastByType() {
  const props = getProps()
  const byType: Record<string, number[]> = {}

  for (const p of props) {
    if (!byType[p.type]) byType[p.type] = []
    byType[p.type].push(p.price)
  }

  const forecasts = Object.entries(byType)
    .map(([type, prices]) => {
      const sorted = [...prices].sort((a, b) => a - b)
      const trend = prices.length > 1 ? (prices[prices.length - 1] - prices[0]) / prices.length : 0
      return {
        type,
        trend: trend > 0 ? 'up' : 'down',
        coefficient: round(trend, 4),
        currentAvgPrice: round(avg(prices)),
      }
    })
    .sort((a, b) => Math.abs(b.coefficient) - Math.abs(a.coefficient))

  return { forecasts, totalTypes: forecasts.length }
}

export function computePriceDistribution(bins = 10) {
  const props = getProps()
  const prices = props.map(p => p.price).sort((a, b) => a - b)
  const min = prices[0]
  const max = prices[prices.length - 1]
  const step = (max - min) / bins
  const distribution = []

  for (let i = 0; i < bins; i++) {
    const low = min + i * step
    const high = low + step
    const count = prices.filter(p => p >= low && p < high).length
    distribution.push({ range: `${Math.round(low)}-${Math.round(high)}`, low: Math.round(low), high: Math.round(high), count })
  }

  return { distribution, total: prices.length }
}

function sum(arr: number[]) {
  return arr.reduce((a, b) => a + b, 0)
}

export function computeFullAnalysis(params?: { period?: string; property_type?: string; city?: string; months?: number }) {
  return {
    trends: computeTrends(params?.period, params?.property_type, params?.city),
    zones: computeZones(),
    popular: computePopular(),
    predictions: computePredictions(params?.months || 6),
    forecastByType: computeForecastByType(),
    priceDistribution: computePriceDistribution(),
  }
}
