import axios from 'axios'
import {
  computeSummary,
  computeTrends,
  computeZones,
  computePopular,
  computePredictions,
  computeForecastByType,
  computePriceDistribution,
  computeFullAnalysis,
} from '@/services/marketAnalysis.service'

const ds = axios.create({
  baseURL: '/ds-api',
  headers: { 'Content-Type': 'application/json' },
  timeout: 5000,
})

async function withFallback<T>(apiCall: () => Promise<T>, fallback: () => T): Promise<T> {
  try {
    const result = await apiCall()
    return result
  } catch {
    return fallback()
  }
}

export async function getMarketSummary() {
  return withFallback(
    async () => { const { data } = await ds.get('/market-analysis/summary'); return data },
    () => computeSummary(),
  )
}

export async function getTrends(params?: { period?: string; property_type?: string; city?: string }) {
  return withFallback(
    async () => { const { data } = await ds.get('/market-analysis/trends', { params }); return data },
    () => computeTrends(params?.period, params?.property_type, params?.city),
  )
}

export async function getZones() {
  return withFallback(
    async () => { const { data } = await ds.get('/market-analysis/zones'); return data },
    () => computeZones(),
  )
}

export async function getPopular() {
  return withFallback(
    async () => { const { data } = await ds.get('/market-analysis/popular'); return data },
    () => computePopular(),
  )
}

export async function getPredictions(months = 6) {
  return withFallback(
    async () => { const { data } = await ds.get('/market-analysis/predictions', { params: { months } }); return data },
    () => computePredictions(months),
  )
}

export async function getForecastByType(_months = 6) {
  return withFallback(
    async () => { const { data } = await ds.get('/market-analysis/forecast-by-type', { params: { months: _months } }); return data },
    () => computeForecastByType(),
  )
}

export async function getPriceDistribution(bins = 10) {
  return withFallback(
    async () => { const { data } = await ds.get('/market-analysis/price-distribution', { params: { bins } }); return data },
    () => computePriceDistribution(bins),
  )
}

export async function getFullAnalysis(params?: { period?: string; property_type?: string; city?: string; months?: number }) {
  return withFallback(
    async () => { const { data } = await ds.get('/market-analysis/full', { params }); return data },
    () => computeFullAnalysis(params),
  )
}
