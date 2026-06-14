<template>
  <div class="ma-page">
    <div class="ma-hero">
      <h1>Analyse du Marché Immobilier</h1>
      <p>Tendances, zones populaires et prévisions — basé sur 2M de biens (2024–2026)</p>
    </div>

    <div v-if="loading" class="ma-loading">Chargement des analyses…</div>
    <div v-else-if="error" class="ma-error">{{ error }}</div>

    <template v-else>
      <MarketOverview :data="summary" />

      <div class="ma-grid">
        <AnalysisModule id="ma-types" title="Répartition par type de bien" :expanded="expanded['ma-types']" @toggle="toggle">
          <TypeDistributionSection :types="summary.typeDistribution" />
        </AnalysisModule>

        <AnalysisModule id="ma-conditions" title="État du bien" :expanded="expanded['ma-conditions']" @toggle="toggle">
          <ConditionsSection :conditions="summary.topConditions" />
        </AnalysisModule>

        <AnalysisModule id="ma-features" title="Caractéristiques populaires" :expanded="expanded['ma-features']" @toggle="toggle">
          <PopularFeaturesSection :features="summary.popularFeatures" />
        </AnalysisModule>
      </div>

      <AnalysisModule id="ma-zones" title="Top zones les plus actives" :expanded="expanded['ma-zones']" @toggle="toggle">
        <div class="ma-zone-grid">
          <div v-for="z in summary.topZones" :key="z.city" class="ma-zone-card">
            <strong>{{ z.city }}</strong><span>{{ formatNumber(z.count) }} annonces</span><span>{{ formatPrice(z.avgPrice) }} moy.</span>
          </div>
        </div>
      </AnalysisModule>

      <div class="ma-cta">
        <p>Vous êtes agent immobilier ?</p>
        <router-link to="/agent/market-analysis-agent" class="ma-cta-btn">Accéder à l'analyse complète</router-link>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getMarketSummary } from '@/api/datascience'
import AnalysisModule from '@/components/analysisModule/AnalysisModule.vue'
import MarketOverview from '@/components/analysisModule/MarketOverview.vue'
import TypeDistributionSection from '@/components/analysisModule/TypeDistributionSection.vue'
import ConditionsSection from '@/components/analysisModule/ConditionsSection.vue'
import PopularFeaturesSection from '@/components/analysisModule/PopularFeaturesSection.vue'

const loading = ref(true)
const error = ref('')
const summary = ref<any>({})
const expanded = reactive<Record<string, boolean>>({ 'ma-types': true, 'ma-conditions': true, 'ma-features': true, 'ma-zones': true })
function toggle(id: string) { expanded[id] = !expanded[id] }

function formatPrice(v: number) { return v ? new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(v) : '0 €' }
function formatNumber(v: number) { return new Intl.NumberFormat('fr-FR').format(v || 0) }

onMounted(async () => {
  try { summary.value = await getMarketSummary() }
  catch { error.value = 'Impossible de charger les analyses.' }
  finally { loading.value = false }
})
</script>

<style scoped>
.ma-page { max-width: 1100px; margin: 0 auto; padding: 2rem 1rem; }
.ma-hero { text-align: center; margin-bottom: 2.5rem; }
.ma-hero h1 { font-size: 2rem; color: #1e2956; margin-bottom: 0.5rem; }
.ma-hero p { color: #6b7280; font-size: 1.05rem; }
.ma-loading, .ma-error { text-align: center; padding: 3rem; color: #6b7280; }
.ma-error { color: #dc2626; }
.ma-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 1.5rem; margin-top: 1.5rem; }
.ma-zone-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 1rem; }
.ma-zone-card { background: #fff; border-radius: 10px; padding: 1rem; box-shadow: 0 1px 4px rgba(0,0,0,0.06); display: flex; flex-direction: column; gap: 0.25rem; }
.ma-zone-card strong { color: #1e2956; font-size: 1rem; }
.ma-zone-card span { font-size: 0.85rem; color: #6b7280; }
.ma-cta { text-align: center; margin-top: 2rem; padding: 2rem; background: linear-gradient(135deg, #1e2956, #2d3a6e); border-radius: 12px; color: #fff; }
.ma-cta p { margin-bottom: 1rem; font-size: 1.1rem; }
.ma-cta-btn { display: inline-block; padding: 0.75rem 2rem; background: #fff; color: #1e2956; font-weight: 700; border-radius: 8px; text-decoration: none; }
.ma-cta-btn:hover { transform: scale(1.05); }
@media (max-width: 767px) { .ma-grid { grid-template-columns: 1fr; } }
</style>
