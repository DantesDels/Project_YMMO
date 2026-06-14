<template>
  <div class="ama-layout">
    <DashboardSidebar
      activeSection="market-analysis"
      :mobileNavOpen="mobileNavOpen"
      :navItems="navItems"
      :navItemsBottom="[]"
      title="Analyse Marché"
      :tocSections="sections"
      :tocActiveId="tocSectionId"
      @switch-section="switchSection"
      @close="mobileNavOpen = false"
      @logout="handleLogout"
      @toc-navigate="tocSectionId = $event"
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

        <AnalysisModule id="sec-overview" title="Aperçu du Marché" expanded @toggle="onToggle">
          <MarketOverview :data="d.trends.summary" />
        </AnalysisModule>

        <AnalysisModule id="sec-trends" title="Tendance des Prix" :expanded="sMap['sec-trends']" @toggle="onToggle">
          <PriceTrendsSection :data="d.trends" />
        </AnalysisModule>

        <div class="ama-duo">
          <AnalysisModule id="sec-types" title="Types — Camembert" :expanded="sMap['sec-types']" @toggle="onToggle">
            <TypeDistributionSection :types="d.popular.types" />
          </AnalysisModule>

          <AnalysisModule id="sec-avgprice" title="Prix Moyen par Type" :expanded="sMap['sec-avgprice']" @toggle="onToggle">
            <AvgPriceByTypeSection :avgPriceByType="d.popular.avgPriceByType" />
          </AnalysisModule>
        </div>

        <div class="ama-duo">
          <AnalysisModule id="sec-conditions" title="Conditions" :expanded="sMap['sec-conditions']" @toggle="onToggle">
            <ConditionsSection :conditions="d.popular.conditions" />
          </AnalysisModule>

          <AnalysisModule id="sec-features" title="Caractéristiques populaires" :expanded="sMap['sec-features']" @toggle="onToggle">
            <PopularFeaturesSection :features="d.popular.features" />
          </AnalysisModule>
        </div>

        <AnalysisModule id="sec-distribution" title="Distribution des Prix — Histogramme" :expanded="sMap['sec-distribution']" @toggle="onToggle">
          <PriceDistributionSection :distribution="d.priceDistribution.distribution" />
        </AnalysisModule>

        <AnalysisModule id="sec-zones" title="Analyse par Zone" :expanded="sMap['sec-zones']" @toggle="onToggle">
          <ZoneAnalysisSection :zones="d.zones.zones" />
        </AnalysisModule>

        <AnalysisModule id="sec-predictions" title="Prévisions IA" :expanded="sMap['sec-predictions']" @toggle="onToggle">
          <PredictionsSection :data="d.predictions" />
        </AnalysisModule>

        <AnalysisModule id="sec-forecast" title="Tendance par Type de Bien" :expanded="sMap['sec-forecast']" @toggle="onToggle">
          <ForecastByTypeSection :forecasts="d.forecastByType.forecasts" />
        </AnalysisModule>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'
import DashboardSidebar from '@/components/dashboard/DashboardSidebar.vue'
import AnalysisModule from '@/components/analysisModule/AnalysisModule.vue'
import MarketOverview from '@/components/analysisModule/MarketOverview.vue'
import PriceTrendsSection from '@/components/analysisModule/PriceTrendsSection.vue'
import TypeDistributionSection from '@/components/analysisModule/TypeDistributionSection.vue'
import ConditionsSection from '@/components/analysisModule/ConditionsSection.vue'
import AvgPriceByTypeSection from '@/components/analysisModule/AvgPriceByTypeSection.vue'
import PopularFeaturesSection from '@/components/analysisModule/PopularFeaturesSection.vue'
import PriceDistributionSection from '@/components/analysisModule/PriceDistributionSection.vue'
import ZoneAnalysisSection from '@/components/analysisModule/ZoneAnalysisSection.vue'
import PredictionsSection from '@/components/analysisModule/PredictionsSection.vue'
import ForecastByTypeSection from '@/components/analysisModule/ForecastByTypeSection.vue'
import { getFullAnalysis } from '@/api/datascience'

const router = useRouter()
const authStore = useAuthentificationStore()

const loading = ref(true)
const error = ref('')
const mobileNavOpen = ref(false)
const d = ref<any>({})

const MODULES = [
  { id: 'sec-overview', title: 'Aperçu', expanded: true },
  { id: 'sec-trends', title: 'Tendance des Prix', expanded: true },
  { id: 'sec-types', title: 'Types', expanded: true },
  { id: 'sec-avgprice', title: 'Prix / Type', expanded: true },
  { id: 'sec-conditions', title: 'Conditions', expanded: true },
  { id: 'sec-features', title: 'Caractéristiques', expanded: true },
  { id: 'sec-distribution', title: 'Distribution Prix', expanded: true },
  { id: 'sec-zones', title: 'Zones', expanded: true },
  { id: 'sec-predictions', title: 'Prévisions IA', expanded: true },
  { id: 'sec-forecast', title: 'Tendance / Type', expanded: true },
]
const sections = reactive(MODULES)
const sMap = computed(() => {
  const map: Record<string, boolean> = {}
  sections.forEach(s => { map[s.id] = s.expanded })
  return map
})
const tocSectionId = ref('sec-overview')

let _tick = false
function onScroll() {
  if (_tick) return
  _tick = true
  requestAnimationFrame(() => {
    const mid = window.innerHeight / 3
    let best: string | null = null
    let bestDist = Infinity
    for (const s of sections) {
      const el = document.getElementById(s.id)
      if (!el) continue
      const rect = el.getBoundingClientRect()
      const dist = Math.abs(rect.top - mid)
      if (dist < bestDist) { bestDist = dist; best = s.id }
    }
    if (best && best !== tocSectionId.value) tocSectionId.value = best
    _tick = false
  })
}

function onToggle(id: string) {
  const s = sections.find(x => x.id === id)
  if (s) s.expanded = !s.expanded
}

const propertyTypes = ['House', 'Apartment', 'Land', 'Commercial', 'Office', 'Garage', 'Parking']
const cities = ['Paris', 'Marseille', 'Lyon', 'Toulouse', 'Nice', 'Nantes', 'Montpellier', 'Strasbourg', 'Bordeaux', 'Lille', 'Rennes', 'Reims', 'Le Havre', 'Saint-Étienne', 'Toulon']

const navItems = [{ id: 'market-analysis', label: 'Analyse Marché' }, { id: 'dashboard', label: 'Profil Agent' }]

const filters = reactive({ period: 'monthly', property_type: '', city: '', months: 6 })

function translateType(t: string) {
  return ({ House: 'Maison', Apartment: 'Appartement', Land: 'Terrain', Commercial: 'Commercial', Office: 'Bureau', Garage: 'Garage', Parking: 'Parking' } as Record<string, string>)[t] || t
}

function switchSection(id: string) { if (id === 'dashboard') router.push('/agent/dashboard') }
function handleLogout() { authStore.logout(); router.push('/') }

async function loadFullAnalysis() {
  loading.value = true; error.value = ''
  try {
    d.value = await getFullAnalysis({ period: filters.period, property_type: filters.property_type || undefined, city: filters.city || undefined, months: filters.months })
  } catch { error.value = 'Erreur lors du chargement des analyses.' }
  finally { loading.value = false }
}

onMounted(() => {
  loadFullAnalysis()
  window.addEventListener('scroll', onScroll, { passive: true })
})
onUnmounted(() => window.removeEventListener('scroll', onScroll))
</script>

<style scoped>
.ama-layout { display: flex; min-height: 100vh; background: #f5f5f7; }
.ama-content { flex: 1; padding: 2rem; display: flex; flex-direction: column; gap: 1.5rem; min-width: 0; }
.ama-toolbar { display: flex; align-items: center; gap: 1rem; }
.ama-toolbar h1 { font-size: 1.5rem; color: #1e2956; margin: 0; }
.ama-mobile-toggle { display: none; background: none; border: none; cursor: pointer; color: #1e2956; padding: 0.25rem; }
.ama-loading, .ama-error { text-align: center; padding: 3rem; color: #6b7280; }
.ama-error { color: #dc2626; }
.ama-filters { background: #fff; border-radius: 12px; padding: 1.5rem; box-shadow: 0 1px 4px rgba(0,0,0,0.06); }
.ama-filters h2 { margin: 0 0 1rem; font-size: 1.1rem; color: #1e2956; }
.ama-filter-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 1rem; margin-bottom: 1rem; }
.ama-filter-grid label { display: flex; flex-direction: column; gap: 0.3rem; font-size: 0.85rem; color: #374151; }
.ama-filter-grid select { padding: 0.5rem; border: 1px solid #d1d5db; border-radius: 6px; font-size: 0.9rem; }
.ama-apply-btn { padding: 0.5rem 1.5rem; background: #1e2956; color: #fff; border: none; border-radius: 8px; font-weight: 600; cursor: pointer; }
.ama-apply-btn:hover { background: #2d3a6e; }
.ama-duo { display: grid; grid-template-columns: 1fr 1fr; gap: 1.5rem; }
@media (max-width: 767px) {
  .ama-mobile-toggle { display: block; }
  .ama-duo { grid-template-columns: 1fr; }
  .ama-content { padding: 1rem; }
}
</style>
