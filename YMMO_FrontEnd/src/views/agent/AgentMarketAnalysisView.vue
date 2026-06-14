<template>
  <div class="ama-layout">
    <DashboardSidebar
      activeSection="market-analysis"
      :mobileNavOpen="mobileNavOpen"
      :navItems="navItems"
      :navItemsBottom="[]"
      title="Analyse Marché"
      @switch-section="switchSection"
      @close="mobileNavOpen = false"
      @logout="handleLogout"
    />

    <div class="ama-content">
      <div class="ama-toolbar">
        <button class="ama-mobile-toggle" @click="mobileNavOpen = true" aria-label="Ouvrir le menu">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="18" x2="21" y2="18"/></svg>
        </button>
        <h1>Analyse de Marché Approfondie</h1>
      </div>

      <div v-if="loading" class="ama-loading">Analyse de 2M de biens en cours…</div>
      <div v-else-if="error" class="ama-error">{{ error }}</div>

      <template v-else>
        <section class="ama-filters">
          <h2>Filtres</h2>
          <div class="ama-filter-grid">
            <label>Période <select v-model="filters.period"><option value="monthly">Mensuel</option><option value="quarterly">Trimestriel</option><option value="yearly">Annuel</option></select></label>
            <label>Type <select v-model="filters.property_type"><option value="">Tous</option><option v-for="t in propertyTypes" :key="t" :value="t">{{ translateType(t) }}</option></select></label>
            <label>Ville <select v-model="filters.city"><option value="">Toutes</option><option v-for="c in cities" :key="c" :value="c">{{ c }}</option></select></label>
            <label>Prévisions <select v-model.number="filters.months"><option :value="3">3 mois</option><option :value="6">6 mois</option><option :value="12">12 mois</option><option :value="24">24 mois</option></select></label>
          </div>
          <button class="ama-apply-btn" @click="loadFullAnalysis">Appliquer</button>
        </section>

        <div class="ama-grid">
          <section class="ama-card ama-card-full">
            <h2>Aperçu du Marché</h2>
            <div class="ama-stats">
              <div class="ama-stat"><span>Total annonces</span><strong>{{ formatNumber(d.trends.summary.totalListings) }}</strong></div>
              <div class="ama-stat"><span>Prix moyen</span><strong>{{ formatPrice(d.trends.summary.globalAvgPrice) }}</strong></div>
              <div class="ama-stat"><span>Prix / m²</span><strong>{{ formatPrice(d.trends.summary.globalAvgPricePerM2) }}</strong></div>
              <div class="ama-stat"><span>Surface moyenne</span><strong>{{ formatNumber(d.trends.summary.globalAvgSurface) }} m²</strong></div>
              <div class="ama-stat"><span>Min → Max</span><strong>{{ formatPrice(d.trends.summary.minPrice) }} → {{ formatPrice(d.trends.summary.maxPrice) }}</strong></div>
            </div>
          </section>

          <section class="ama-card ama-card-full">
            <h2>Tendance des Prix ({{ filters.period }})</h2>
            <div v-if="d.trends.trends.length">
              <ChartWrapper type="line" :data="trendLineData" :options="trendLineOpts" height="280px" />
              <div class="ama-trend-table-wrap">
                <table class="ama-trend-table"><thead><tr><th>Période</th><th>Annonces</th><th>Prix moy.</th><th>Prix/m²</th><th>Volume</th></tr></thead>
                  <tbody><tr v-for="t in d.trends.trends" :key="t.period"><td>{{ t.period }}</td><td>{{ formatNumber(t.count) }}</td><td>{{ formatPrice(t.avgPrice) }}</td><td>{{ formatPrice(t.avgPricePerM2) }}</td><td>{{ formatPrice(t.totalVolume) }}</td></tr></tbody>
                </table>
              </div>
            </div>
            <p v-else class="ama-no-data">Aucune donnée pour ces filtres.</p>
          </section>

          <section class="ama-card">
            <h2>Types — Camembert</h2>
            <ChartWrapper type="doughnut" :data="typeDonutData" height="260px" />
          </section>

          <section class="ama-card">
            <h2>Types — Barres</h2>
            <ChartWrapper type="bar" :data="typeBarData" :options="typeBarOpts" height="260px" />
          </section>

          <section class="ama-card">
            <h2>Conditions</h2>
            <ChartWrapper type="polarArea" :data="condPolarData" height="260px" />
          </section>

          <section class="ama-card">
            <h2>Prix Moyen par Type</h2>
            <ChartWrapper type="bar" :data="avgPriceBarData" :options="avgPriceBarOpts" height="260px" />
          </section>

          <section class="ama-card ama-card-full">
            <h2>Caractéristiques populaires</h2>
            <ChartWrapper type="bar" :data="featBarData" :options="featBarOpts" height="300px" />
          </section>

          <section class="ama-card ama-card-full">
            <h2>Distribution des Prix — Histogramme</h2>
            <div v-if="d.priceDistribution.distribution.length">
              <ChartWrapper type="bar" :data="distHistData" :options="distHistOpts" height="300px" />
            </div>
          </section>

          <section class="ama-card ama-card-full">
            <h2>Analyse par Zone</h2>
            <div class="ama-zone-grid">
              <div v-for="z in d.zones.zones" :key="z.city" class="ama-zone-card" :class="{ 'ama-zone-hot': z.ratioToMarket > 1.1, 'ama-zone-affordable': z.ratioToMarket < 0.9 }">
                <div class="ama-zone-header"><strong>{{ z.city }}</strong>
                  <span class="ama-zone-badge" v-if="z.ratioToMarket > 1.1">🔥 Chaude</span>
                  <span class="ama-zone-badge ama-zone-badge-green" v-else-if="z.ratioToMarket < 0.9">💰 Abordable</span>
                </div>
                <div class="ama-zone-details"><span>{{ formatNumber(z.count) }} annonces</span><span>{{ formatPrice(z.avgPrice) }}</span><span>{{ formatPrice(z.avgPricePerM2) }}/m²</span><span>Ratio: {{ z.ratioToMarket }}x</span></div>
              </div>
            </div>
            <div class="ama-zone-chart"><ChartWrapper type="radar" :data="zoneRadarData" :options="zoneRadarOpts" height="320px" /></div>
          </section>

          <section class="ama-card ama-card-full">
            <h2>Prévisions IA ({{ filters.months }} mois)</h2>
            <div v-if="d.predictions.predictions.length">
              <div class="ama-prediction-meta"><span>Modèle: {{ d.predictions.model }}</span><span>Confiance: {{ (d.predictions.confidence * 100).toFixed(1) }}%</span><span>Données: {{ formatNumber(d.predictions.dataPoints) }}</span></div>
              <ChartWrapper type="line" :data="predLineData" :options="predLineOpts" height="280px" />
              <div class="ama-trend-table-wrap">
                <table class="ama-trend-table"><thead><tr><th>Mois</th><th>Prix prévu</th></tr></thead>
                  <tbody><tr v-for="p in d.predictions.predictions" :key="p.month"><td>{{ p.month }}</td><td>{{ formatPrice(p.predictedAvgPrice) }}</td></tr></tbody>
                </table>
              </div>
            </div>
            <p v-else class="ama-no-data">{{ d.predictions.error || 'Aucune prévision.' }}</p>
          </section>

          <section class="ama-card ama-card-full">
            <h2>Tendance par Type de Bien</h2>
            <div v-if="d.forecastByType.forecasts.length">
              <div class="ama-forecast-grid">
                <div v-for="f in d.forecastByType.forecasts" :key="f.type" class="ama-forecast-card">
                  <strong>{{ translateType(f.type) }}</strong>
                  <span class="ama-forecast-trend" :class="f.trend === 'up' ? 'trend-up' : 'trend-down'">{{ f.trend === 'up' ? '📈' : '📉' }} {{ f.trend === 'up' ? 'Hausse' : 'Baisse' }}</span>
                  <span>{{ formatPrice(f.currentAvgPrice) }}</span>
                  <span>Coeff: {{ f.coefficient.toFixed(2) }}</span>
                </div>
              </div>
            </div>
            <p v-else class="ama-no-data">Aucune tendance disponible.</p>
          </section>
        </div>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'
import DashboardSidebar from '@/components/dashboard/DashboardSidebar.vue'
import ChartWrapper from '@/components/charts/ChartWrapper.vue'
import { getFullAnalysis } from '@/api/datascience'

const router = useRouter()
const authStore = useAuthentificationStore()

const loading = ref(true)
const error = ref('')
const mobileNavOpen = ref(false)
const d = ref<any>({ trends: { trends: [], summary: {} }, zones: { zones: [] }, popular: { types: [], features: [], conditions: [], avgPriceByType: [] }, predictions: { predictions: [] }, forecastByType: { forecasts: [] }, priceDistribution: { distribution: [] } })
const propertyTypes = ['House', 'Apartment', 'Land', 'Commercial', 'Office', 'Garage', 'Parking']
const cities = ['Paris', 'Marseille', 'Lyon', 'Toulouse', 'Nice', 'Nantes', 'Montpellier', 'Strasbourg', 'Bordeaux', 'Lille', 'Rennes', 'Reims', 'Le Havre', 'Saint-Étienne', 'Toulon']

const navItems = [{ id: 'market-analysis', label: 'Analyse Marché' }, { id: 'dashboard', label: 'Profil Agent' }]

const filters = reactive({ period: 'monthly', property_type: '', city: '', months: 6 })

const COLORS = ['#1e2956', '#2d4a7a', '#3b6a9e', '#4a8ac2', '#59aae6', '#78bce8', '#97cef0']
const COND_COLORS = ['#059669', '#10b981', '#f59e0b', '#f97316', '#ef4444', '#991b1b']

function formatPrice(v: number) { return v ? new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(v) : '0 €' }
function formatNumber(v: number) { return new Intl.NumberFormat('fr-FR').format(v || 0) }
function translateType(t: string) { const map: Record<string, string> = { House: 'Maison', Apartment: 'Appartement', Land: 'Terrain', Commercial: 'Commercial', Office: 'Bureau', Garage: 'Garage', Parking: 'Parking' }; return map[t] || t }
function translateCondition(c: string) { const map: Record<string, string> = { New: 'Neuf', Excellent: 'Excellent', Good: 'Bon', NeedsRefresh: 'À rafraîchir', NeedsRenovation: 'À rénover', Ruin: 'Ruine' }; return map[c] || c }

const trendLineData = computed(() => ({
  labels: d.value.trends.trends.map((t: any) => t.period),
  datasets: [
    { label: 'Prix moyen', data: d.value.trends.trends.map((t: any) => t.avgPrice), borderColor: '#1e2956', backgroundColor: 'rgba(30,41,86,0.1)', fill: true, tension: 0.3, pointRadius: 4 },
    { label: 'Prix / m²', data: d.value.trends.trends.map((t: any) => t.avgPricePerM2), borderColor: '#059669', backgroundColor: 'rgba(5,150,105,0.1)', fill: true, tension: 0.3, pointRadius: 3 },
  ],
}))
const trendLineOpts = { responsive: true, plugins: { legend: { position: 'top' as const } }, scales: { y: { beginAtZero: true, ticks: { callback: (v: any) => formatPrice(v) } } } }

const typeDonutData = computed(() => ({
  labels: d.value.popular.types.map((t: any) => translateType(t.type)),
  datasets: [{ data: d.value.popular.types.map((t: any) => t.count), backgroundColor: COLORS, borderWidth: 1 }],
}))

const typeBarData = computed(() => ({
  labels: d.value.popular.types.map((t: any) => translateType(t.type)),
  datasets: [{ label: 'Nombre', data: d.value.popular.types.map((t: any) => t.count), backgroundColor: COLORS, borderRadius: 6 }],
}))
const typeBarOpts = { plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } }

const condPolarData = computed(() => ({
  labels: d.value.popular.conditions.map((c: any) => translateCondition(c.condition)),
  datasets: [{ data: d.value.popular.conditions.map((c: any) => c.count), backgroundColor: COND_COLORS, borderWidth: 1 }],
}))

const avgPriceBarData = computed(() => ({
  labels: d.value.popular.avgPriceByType.map((t: any) => translateType(t.type)),
  datasets: [{ label: 'Prix moyen', data: d.value.popular.avgPriceByType.map((t: any) => t.avgPrice), backgroundColor: COLORS, borderRadius: 6 }],
}))
const avgPriceBarOpts = { plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true, ticks: { callback: (v: any) => formatPrice(v) } } } }

const featBarData = computed(() => ({
  labels: d.value.popular.features.map((f: any) => f.feature),
  datasets: [{ label: 'Présence', data: d.value.popular.features.map((f: any) => f.count), backgroundColor: '#3b6a9e', borderRadius: 6 }],
}))
const featBarOpts = { indexAxis: 'y' as const, plugins: { legend: { display: false } }, scales: { x: { beginAtZero: true } } }

const distHistData = computed(() => {
  const dist = d.value.priceDistribution.distribution || []
  const labels = dist.slice(0, 30).map((b: any) => formatPrice(b.low))
  return {
    labels,
    datasets: [{ label: 'Biens', data: dist.slice(0, 30).map((b: any) => b.count), backgroundColor: '#4a8ac2', borderRadius: 4 }],
  }
})
const distHistOpts = { plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } }

const zoneRadarData = computed(() => {
  const zones = d.value.zones.zones || []
  return {
    labels: zones.map((z: any) => z.city),
    datasets: [
      { label: 'Prix moyen', data: zones.map((z: any) => z.avgPrice), borderColor: '#1e2956', backgroundColor: 'rgba(30,41,86,0.2)', pointRadius: 4 },
      { label: 'Prix / m²', data: zones.map((z: any) => z.avgPricePerM2), borderColor: '#059669', backgroundColor: 'rgba(5,150,105,0.2)', pointRadius: 4 },
    ],
  }
})
const zoneRadarOpts = { responsive: true, plugins: { legend: { position: 'top' as const } }, scales: { r: { beginAtZero: true, ticks: { callback: (v: any) => formatPrice(v), stepSize: 50000 } } } }

const predLineData = computed(() => ({
  labels: d.value.predictions.predictions.map((p: any) => p.month),
  datasets: [{ label: 'Prix prévu', data: d.value.predictions.predictions.map((p: any) => p.predictedAvgPrice), borderColor: '#7c3aed', backgroundColor: 'rgba(124,58,237,0.1)', fill: true, tension: 0.3, pointRadius: 5, pointBackgroundColor: '#7c3aed' }],
}))
const predLineOpts = { responsive: true, plugins: { legend: { position: 'top' as const } }, scales: { y: { beginAtZero: true, ticks: { callback: (v: any) => formatPrice(v) } } } }

function switchSection(id: string) { if (id === 'dashboard') router.push('/agent/dashboard') }
function handleLogout() { authStore.logout(); router.push('/') }

async function loadFullAnalysis() {
  loading.value = true; error.value = ''
  try {
    d.value = await getFullAnalysis({ period: filters.period, property_type: filters.property_type || undefined, city: filters.city || undefined, months: filters.months })
  } catch { error.value = 'Erreur lors du chargement des analyses.' }
  finally { loading.value = false }
}

onMounted(loadFullAnalysis)
</script>

<style scoped>
.ama-layout { display: flex; min-height: 100vh; background: #f5f5f7; }
.ama-content { flex: 1; padding: 2rem; overflow-x: auto; }
.ama-toolbar { display: flex; align-items: center; gap: 1rem; margin-bottom: 1.5rem; }
.ama-toolbar h1 { font-size: 1.5rem; color: #1e2956; margin: 0; }
.ama-mobile-toggle { display: none; background: none; border: none; cursor: pointer; color: #1e2956; padding: 0.25rem; }
.ama-loading, .ama-error { text-align: center; padding: 3rem; color: #6b7280; }
.ama-error { color: #dc2626; }
.ama-filters { background: #fff; border-radius: 12px; padding: 1.5rem; margin-bottom: 2rem; box-shadow: 0 1px 4px rgba(0,0,0,0.06); }
.ama-filters h2 { margin: 0 0 1rem; font-size: 1.1rem; color: #1e2956; }
.ama-filter-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 1rem; margin-bottom: 1rem; }
.ama-filter-grid label { display: flex; flex-direction: column; gap: 0.3rem; font-size: 0.85rem; color: #374151; }
.ama-filter-grid select { padding: 0.5rem; border: 1px solid #d1d5db; border-radius: 6px; font-size: 0.9rem; }
.ama-apply-btn { padding: 0.5rem 1.5rem; background: #1e2956; color: #fff; border: none; border-radius: 8px; font-weight: 600; cursor: pointer; }
.ama-apply-btn:hover { background: #2d3a6e; }
.ama-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 1.5rem; }
.ama-card-full { grid-column: 1 / -1; }
.ama-card { background: #fff; border-radius: 12px; padding: 1.5rem; box-shadow: 0 1px 4px rgba(0,0,0,0.06); }
.ama-card h2 { font-size: 1.1rem; color: #1e2956; margin: 0 0 1rem; border-left: 3px solid #1e2956; padding-left: 0.6rem; }
.ama-stats { display: grid; grid-template-columns: repeat(auto-fit, minmax(140px, 1fr)); gap: 1rem; }
.ama-stat { text-align: center; padding: 0.75rem; background: #f9fafb; border-radius: 8px; }
.ama-stat span { display: block; font-size: 0.8rem; color: #6b7280; }
.ama-stat strong { font-size: 1.15rem; color: #1e2956; }
.ama-chart-bars { display: flex; align-items: flex-end; gap: 4px; min-height: 120px; padding: 0.5rem 0; }
.ama-chart-col { flex: 1; display: flex; flex-direction: column; align-items: center; gap: 0.25rem; }
.ama-chart-bar { width: 100%; min-height: 4px; background: linear-gradient(360deg, #1e2956, #4a5a9a); border-radius: 4px 4px 0 0; transition: height 0.4s ease; }
.ama-chart-label { font-size: 0.65rem; color: #6b7280; white-space: nowrap; }
.ama-trend-table-wrap { overflow-x: auto; margin-top: 1rem; }
.ama-trend-table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
.ama-trend-table th { background: #f3f4f6; text-align: left; padding: 0.5rem; font-weight: 600; color: #374151; }
.ama-trend-table td { padding: 0.4rem 0.5rem; border-bottom: 1px solid #f3f4f6; color: #4b5563; }
.ama-zone-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 0.75rem; margin-bottom: 1.5rem; }
.ama-zone-card { padding: 0.75rem; border-radius: 8px; background: #f9fafb; border-left: 4px solid #9ca3af; }
.ama-zone-hot { border-left-color: #ef4444; }
.ama-zone-affordable { border-left-color: #10b981; }
.ama-zone-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 0.4rem; }
.ama-zone-card strong { color: #1e2956; font-size: 0.95rem; }
.ama-zone-badge { font-size: 0.7rem; padding: 0.15rem 0.4rem; border-radius: 4px; background: #fee2e2; color: #dc2626; font-weight: 700; }
.ama-zone-badge-green { background: #d1fae5; color: #059669; }
.ama-zone-details { display: flex; flex-wrap: wrap; gap: 0.25rem 0.75rem; font-size: 0.8rem; color: #6b7280; }
.ama-zone-chart { margin-top: 1rem; }
.ama-prediction-meta { display: flex; gap: 1.5rem; font-size: 0.85rem; color: #6b7280; margin-bottom: 1rem; }
.ama-forecast-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(160px, 1fr)); gap: 0.75rem; }
.ama-forecast-card { padding: 0.75rem; border-radius: 8px; background: #f9fafb; display: flex; flex-direction: column; gap: 0.25rem; }
.ama-forecast-card strong { color: #1e2956; font-size: 0.9rem; }
.ama-forecast-trend { font-size: 0.85rem; }
.trend-up { color: #059669; }
.trend-down { color: #dc2626; }
.ama-forecast-card span:last-child { font-size: 0.75rem; color: #9ca3af; }
.ama-no-data { color: #9ca3af; font-style: italic; }
@media (max-width: 767px) { .ama-mobile-toggle { display: block; } .ama-grid { grid-template-columns: 1fr; } .ama-content { padding: 1rem; } }
</style>
