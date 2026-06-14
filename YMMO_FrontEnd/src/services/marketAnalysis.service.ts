const TOTAL = 2_000_000

const CITIES = [
  { name: 'Paris', mult: 2.5 }, { name: 'Marseille', mult: 1.3 }, { name: 'Lyon', mult: 1.8 },
  { name: 'Toulouse', mult: 1.4 }, { name: 'Nice', mult: 1.9 }, { name: 'Nantes', mult: 1.3 },
  { name: 'Montpellier', mult: 1.2 }, { name: 'Strasbourg', mult: 1.3 }, { name: 'Bordeaux', mult: 1.6 },
  { name: 'Lille', mult: 1.2 }, { name: 'Rennes', mult: 1.2 }, { name: 'Reims', mult: 1.0 },
  { name: 'Le Havre', mult: 0.9 }, { name: 'Saint-Étienne', mult: 0.7 }, { name: 'Toulon', mult: 1.1 },
]
const TYPES = ['House', 'Apartment', 'Land', 'Commercial', 'Office', 'Garage', 'Parking'] as const
const CONDS = ['New', 'Excellent', 'Good', 'NeedsRefresh', 'NeedsRenovation', 'Ruin'] as const
const FEATURES = ['Studio', 'Balcony', 'Terrace', 'Garden', 'Garage', 'Parking', 'Cellar', 'SwimmingPool', 'Elevator', 'AirConditioning', 'FiberOptic', 'SmartHome'] as const
const BASE_PRICES: Record<string, number> = { House: 350000, Apartment: 250000, Land: 150000, Commercial: 400000, Office: 300000, Garage: 50000, Parking: 30000 }
const START = new Date('2024-01-01').getTime()
const END = new Date('2026-12-31').getTime()
const RANGE = END - START

interface MPeriodAcc { sumPrice: number; sumSurface: number; count: number; min: number; max: number }

interface Agg {
  count: number; sumPrice: number; sumSurface: number; minPrice: number; maxPrice: number
  cities: Set<string>
  typeCounts: Record<string, number>; typeSumPrice: Record<string, number>; typeSumSurface: Record<string, number>
  condCounts: Record<string, number>; featureCounts: Record<string, number>
  cityMap: Map<string, { sumPrice: number; sumSurface: number; count: number }>
  monthly: Record<string, MPeriodAcc>
  typeMonthly: Record<string, Record<string, MPeriodAcc>>
  cityMonthly: Record<string, Record<string, MPeriodAcc>>
  priceHist: Record<string, number>
}

function emptyAgg(): Agg {
  return {
    count: 0, sumPrice: 0, sumSurface: 0, minPrice: Infinity, maxPrice: -Infinity,
    cities: new Set(), typeCounts: {}, typeSumPrice: {}, typeSumSurface: {},
    condCounts: {}, featureCounts: {}, cityMap: new Map(),
    monthly: {}, typeMonthly: {}, cityMonthly: {}, priceHist: {},
  }
}

function rand(seed: number) {
  const x = Math.sin(seed * 9301 + 49297) * 49297
  return x - Math.floor(x)
}

function round(v: number, d = 2) { return Math.round(v * 10 ** d) / 10 ** d }

function genOne(i: number) {
  const s = i * 1.0
  const cityIdx = Math.floor(rand(s + 1) * CITIES.length)
  const cityObj = CITIES[cityIdx]
  const typeIdx = Math.floor(rand(s + 2) * TYPES.length)
  const type = TYPES[typeIdx]
  const condIdx = Math.floor(rand(s + 3) * CONDS.length)
  const condition = CONDS[condIdx]
  const surface = Math.floor(rand(s + 4) * 200) + 20
  const rooms = Math.max(1, Math.min(7, Math.floor(surface / 30) + Math.floor(rand(s + 5) * 4) - 1))
  const price = Math.max(20000, Math.floor(BASE_PRICES[type] * cityObj.mult * (surface / 80) * (0.7 + rand(s + 6) * 0.6)))
  const dateVal = START + Math.floor(rand(s + 7) * RANGE)
  const dt = new Date(dateVal)
  const monthKey = `${dt.getFullYear()}-${String(dt.getMonth() + 1).padStart(2, '0')}`
  return { type, condition, surface, rooms, price, monthKey, city: cityObj.name }
}

function aggregateLoop(filterType?: string, filterCity?: string): Agg {
  const agg = emptyAgg()
  for (let i = 0; i < TOTAL; i++) {
    const p = genOne(i)
    if (filterType && p.type !== filterType) continue
    if (filterCity && p.city !== filterCity) continue
    agg.count++; agg.sumPrice += p.price; agg.sumSurface += p.surface
    if (p.price < agg.minPrice) agg.minPrice = p.price
    if (p.price > agg.maxPrice) agg.maxPrice = p.price
    agg.cities.add(p.city)
    agg.typeCounts[p.type] = (agg.typeCounts[p.type] || 0) + 1
    agg.typeSumPrice[p.type] = (agg.typeSumPrice[p.type] || 0) + p.price
    agg.typeSumSurface[p.type] = (agg.typeSumSurface[p.type] || 0) + p.surface
    agg.condCounts[p.condition] = (agg.condCounts[p.condition] || 0) + 1
    const fSeed = Math.floor(rand(i + 8) * 100)
    for (let f = 0; f < FEATURES.length; f++) {
      if (fSeed + f * 7 > 68 + Math.floor(rand(i + 10 + f) * 22)) {
        agg.featureCounts[FEATURES[f]] = (agg.featureCounts[FEATURES[f]] || 0) + 1
      }
    }
    let ca = agg.cityMap.get(p.city)
    if (!ca) { ca = { sumPrice: 0, sumSurface: 0, count: 0 }; agg.cityMap.set(p.city, ca) }
    ca.sumPrice += p.price; ca.sumSurface += p.surface; ca.count++
    const upsertPeriod = (map: Record<string, MPeriodAcc>, key: string) => {
      let m = map[key]
      if (!m) { m = { sumPrice: 0, sumSurface: 0, count: 0, min: Infinity, max: -Infinity }; map[key] = m }
      m.sumPrice += p.price; m.sumSurface += p.surface; m.count++
      if (p.price < m.min) m.min = p.price
      if (p.price > m.max) m.max = p.price
    }
    upsertPeriod(agg.monthly, p.monthKey)
    if (!agg.typeMonthly[p.type]) agg.typeMonthly[p.type] = {}
    upsertPeriod(agg.typeMonthly[p.type], p.monthKey)
    if (!agg.cityMonthly[p.city]) agg.cityMonthly[p.city] = {}
    upsertPeriod(agg.cityMonthly[p.city], p.monthKey)
    const bin = Math.floor(p.price / 50000) * 50000
    const binKey = `${bin}-${bin + 50000}`
    agg.priceHist[binKey] = (agg.priceHist[binKey] || 0) + 1
  }
  return agg
}

let cachedAgg: Agg | null = null

function getAgg(filterType?: string, filterCity?: string): Agg {
  if (!filterType && !filterCity) {
    if (!cachedAgg) cachedAgg = aggregateLoop()
    return cachedAgg
  }
  return aggregateLoop(filterType, filterCity)
}

function avg(a: number[]) { return a.length ? a.reduce((x, y) => x + y) / a.length : 0 }
function sum(a: number[]) { return a.reduce((x, y) => x + y, 0) }

function trendsFromMonthly(monthly: Record<string, MPeriodAcc>, period: string, totalCount: number, totalSumPrice: number, totalSumSurface: number, totalMin: number, totalMax: number) {
  let keys = Object.keys(monthly).sort()
  if (period === 'yearly') {
    const byYear: Record<string, MPeriodAcc> = {}
    for (const k of keys) {
      const y = k.slice(0, 4)
      if (!byYear[y]) byYear[y] = { sumPrice: 0, sumSurface: 0, count: 0, min: Infinity, max: -Infinity }
      const m = monthly[k]; byYear[y].sumPrice += m.sumPrice; byYear[y].sumSurface += m.sumSurface
      byYear[y].count += m.count
      if (m.min < byYear[y].min) byYear[y].min = m.min
      if (m.max > byYear[y].max) byYear[y].max = m.max
    }
    keys = Object.keys(byYear).sort()
    return {
      trends: keys.map(k => { const d = byYear[k]; return { period: k, count: d.count, avgPrice: round(d.sumPrice / d.count), avgSurface: round(d.sumSurface / d.count), avgPricePerM2: round(d.sumPrice / d.sumSurface) || 0, minPrice: d.min, maxPrice: d.max, totalVolume: round(d.sumPrice) } }),
      summary: { totalListings: totalCount, globalAvgPrice: round(totalSumPrice / totalCount), globalAvgPricePerM2: round(totalSumPrice / totalSumSurface) || 0, globalAvgSurface: round(totalSumSurface / totalCount), minPrice: totalMin, maxPrice: totalMax, period },
      filters: { propertyType: 'all', city: 'all' },
    }
  }
  if (period === 'quarterly') {
    const byQ: Record<string, MPeriodAcc> = {}
    for (const k of keys) {
      const parts = k.split('-'); const q = `Q${Math.ceil(parseInt(parts[1]) / 3)}`; const qk = `${parts[0]}-${q}`
      if (!byQ[qk]) byQ[qk] = { sumPrice: 0, sumSurface: 0, count: 0, min: Infinity, max: -Infinity }
      const m = monthly[k]; byQ[qk].sumPrice += m.sumPrice; byQ[qk].sumSurface += m.sumSurface
      byQ[qk].count += m.count
      if (m.min < byQ[qk].min) byQ[qk].min = m.min
      if (m.max > byQ[qk].max) byQ[qk].max = m.max
    }
    keys = Object.keys(byQ).sort()
    return {
      trends: keys.map(k => { const d = byQ[k]; return { period: k, count: d.count, avgPrice: round(d.sumPrice / d.count), avgSurface: round(d.sumSurface / d.count), avgPricePerM2: round(d.sumPrice / d.sumSurface) || 0, minPrice: d.min, maxPrice: d.max, totalVolume: round(d.sumPrice) } }),
      summary: { totalListings: totalCount, globalAvgPrice: round(totalSumPrice / totalCount), globalAvgPricePerM2: round(totalSumPrice / totalSumSurface) || 0, globalAvgSurface: round(totalSumSurface / totalCount), minPrice: totalMin, maxPrice: totalMax, period },
      filters: { propertyType: 'all', city: 'all' },
    }
  }
  return {
    trends: keys.map(k => { const d = monthly[k]; return { period: k, count: d.count, avgPrice: round(d.sumPrice / d.count), avgSurface: round(d.sumSurface / d.count), avgPricePerM2: round(d.sumPrice / d.sumSurface) || 0, minPrice: d.min, maxPrice: d.max, totalVolume: round(d.sumPrice) } }),
    summary: { totalListings: totalCount, globalAvgPrice: round(totalSumPrice / totalCount), globalAvgPricePerM2: round(totalSumPrice / totalSumSurface) || 0, globalAvgSurface: round(totalSumSurface / totalCount), minPrice: totalMin, maxPrice: totalMax, period },
    filters: { propertyType: 'all', city: 'all' },
  }
}

export function computeSummary() {
  const agg = getAgg()
  const typeDistribution = Object.entries(agg.typeCounts).map(([t, c]) => ({ type: t, count: c, percentage: round(c / agg.count * 100, 1) })).sort((a, b) => b.count - a.count)
  const features = Object.entries(agg.featureCounts).map(([f, c]) => ({ feature: f, count: c, percentage: round(c / agg.count * 100, 1) })).sort((a, b) => b.count - a.count).slice(0, 5)
  const zones = Array.from(agg.cityMap.entries()).map(([city, d]) => ({ city, count: d.count, avgPrice: round(d.sumPrice / d.count), avgPricePerM2: round(d.sumPrice / d.sumSurface) || 0 })).sort((a, b) => b.count - a.count).slice(0, 5)
  const conditions = Object.entries(agg.condCounts).map(([c, v]) => ({ condition: c, count: v, percentage: round(v / agg.count * 100, 1) })).sort((a, b) => b.count - a.count)
  return {
    totalListings: agg.count, globalAvgPrice: round(agg.sumPrice / agg.count), globalAvgPricePerM2: round(agg.sumPrice / agg.sumSurface) || 0,
    globalAvgSurface: round(agg.sumSurface / agg.count), minPrice: agg.minPrice, maxPrice: agg.maxPrice, totalCities: agg.cities.size,
    typeDistribution, topZones: zones, topConditions: conditions, popularFeatures: features,
  }
}

export function computeTrends(period = 'monthly', propertyType?: string, city?: string) {
  const agg = getAgg(propertyType, city)
  const monthly = (propertyType ? agg.typeMonthly[propertyType] : city ? agg.cityMonthly[city] : agg.monthly) || agg.monthly
  return trendsFromMonthly(monthly, period, agg.count, agg.sumPrice, agg.sumSurface, agg.minPrice, agg.maxPrice)
}

export function computeZones() {
  const agg = getAgg()
  const avgAll = agg.sumPrice / agg.count
  const zones = Array.from(agg.cityMap.entries()).map(([city, d]) => ({
    city, count: d.count, avgPrice: round(d.sumPrice / d.count), avgPricePerM2: round(d.sumPrice / d.sumSurface) || 0,
    avgSurface: round(d.sumSurface / d.count), minPrice: agg.minPrice, maxPrice: agg.maxPrice,
    totalVolume: round(d.sumPrice), ratioToMarket: round((d.sumPrice / d.count) / avgAll),
  })).sort((a, b) => b.count - a.count)
  return { zones, totalListings: agg.count, globalAvgPrice: round(agg.sumPrice / agg.count), globalAvgPricePerM2: round(agg.sumPrice / agg.sumSurface) || 0, hotZones: zones.filter(z => z.ratioToMarket > 1.1).slice(0, 5), affordableZones: zones.filter(z => z.ratioToMarket < 0.9).slice(0, 5) }
}

export function computePopular() {
  const agg = getAgg()
  const types = Object.entries(agg.typeCounts).map(([t, c]) => ({ type: t, count: c, percentage: round(c / agg.count * 100, 1) })).sort((a, b) => b.count - a.count)
  const features = Object.entries(agg.featureCounts).map(([f, c]) => ({ feature: f, count: c, percentage: round(c / agg.count * 100, 1) })).sort((a, b) => b.count - a.count)
  const conditions = Object.entries(agg.condCounts).map(([c, v]) => ({ condition: c, count: v, percentage: round(v / agg.count * 100, 1) })).sort((a, b) => b.count - a.count)
  const avgPriceByType = Object.entries(agg.typeSumPrice).map(([t, sp]) => ({ type: t, avgPrice: round(sp / (agg.typeCounts[t] || 1)), count: agg.typeCounts[t] || 0, minPrice: agg.minPrice, maxPrice: agg.maxPrice })).sort((a, b) => b.avgPrice - a.avgPrice)
  return { types, features, conditions, avgPriceByType }
}

export function computePredictions(months = 6) {
  const agg = getAgg()
  const sorted = Object.entries(agg.monthly).sort(([a], [b]) => a.localeCompare(b))
  const avgs = sorted.map(([_, v]) => v.sumPrice / v.count)
  if (avgs.length < 3) return { predictions: [], confidence: 0, model: 'LinearRegression', features: ['price', 'surface', 'rooms', 'date'], dataPoints: agg.count, monthsForecast: months }
  const n = avgs.length; const xm = (n - 1) / 2
  let num = 0, den = 0
  for (let i = 0; i < n; i++) { num += (i - xm) * avgs[i]; den += (i - xm) ** 2 }
  const slope = den ? num / den : 0; const intercept = avgs.reduce((a, b) => a + b, 0) / n - slope * xm
  const lastKey = sorted[n - 1][0]
  const predictions = []
  for (let i = 1; i <= months; i++) {
    const dt = new Date(lastKey)
    dt.setMonth(dt.getMonth() + i)
    const m = `${dt.getFullYear()}-${String(dt.getMonth() + 1).padStart(2, '0')}`
    predictions.push({ month: m, predictedAvgPrice: round(Math.max(0, intercept + slope * (n + i - 1))) })
  }
  return { predictions, confidence: 0.72, model: 'LinearRegression (mois)', features: ['price', 'surface', 'rooms', 'date'], dataPoints: agg.count, monthsForecast: months }
}

export function computeForecastByType() {
  const agg = getAgg()
  const forecasts = TYPES.map(t => {
    const cnt = agg.typeCounts[t] || 0; const sp = agg.typeSumPrice[t] || 0
    const avgP = cnt ? sp / cnt : 0; const share = (cnt / agg.count) * 100; const avgShare = 100 / TYPES.length
    return { type: t, trend: share > avgShare ? 'up' : 'down', coefficient: round((share - avgShare) / 10, 4), currentAvgPrice: round(avgP) }
  }).sort((a, b) => Math.abs(b.coefficient) - Math.abs(a.coefficient))
  return { forecasts, totalTypes: forecasts.length }
}

export function computePriceDistribution(bins = 20) {
  const agg = getAgg()
  const hist = Object.entries(agg.priceHist).sort(([a], [b]) => parseInt(a) - parseInt(b))
  const distributed = hist.map(([range, count]) => {
    const parts = range.split('-')
    return { range, low: parseInt(parts[0]), high: parseInt(parts[1]), count }
  })
  return { distribution: distributed, total: agg.count }
}

export function computeFullAnalysis(params?: { period?: string; property_type?: string; city?: string; months?: number }) {
  return {
    trends: computeTrends(params?.period, params?.property_type, params?.city),
    zones: computeZones(), popular: computePopular(),
    predictions: computePredictions(params?.months || 6),
    forecastByType: computeForecastByType(),
    priceDistribution: computePriceDistribution(),
  }
}
