import axios from 'axios'

const ds = axios.create({
  baseURL: '/ds-api',
  headers: { 'Content-Type': 'application/json' },
})

export async function getMarketSummary() {
  const { data } = await ds.get('/market-analysis/summary')
  return data
}

export async function getTrends(params?: {
  period?: string
  property_type?: string
  city?: string
}) {
  const { data } = await ds.get('/market-analysis/trends', { params })
  return data
}

export async function getZones() {
  const { data } = await ds.get('/market-analysis/zones')
  return data
}

export async function getPopular() {
  const { data } = await ds.get('/market-analysis/popular')
  return data
}

export async function getPredictions(months = 6) {
  const { data } = await ds.get('/market-analysis/predictions', { params: { months } })
  return data
}

export async function getForecastByType(months = 6) {
  const { data } = await ds.get('/market-analysis/forecast-by-type', { params: { months } })
  return data
}

export async function getPriceDistribution(bins = 10) {
  const { data } = await ds.get('/market-analysis/price-distribution', { params: { bins } })
  return data
}

export async function getFullAnalysis(params?: {
  period?: string
  property_type?: string
  city?: string
  months?: number
}) {
  const { data } = await ds.get('/market-analysis/full', { params })
  return data
}
