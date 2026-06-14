<template>
  <div class="ma-page">
    <div class="ma-hero">
      <h1>Analyse du Marché Immobilier</h1>
      <p>Tendances, zones populaires et prévisions — alimenté par l'IA</p>
    </div>

    <div v-if="loading" class="ma-loading">Chargement des analyses…</div>
    <div v-else-if="error" class="ma-error">{{ error }}</div>

    <template v-else>
      <div class="ma-summary-cards">
        <div class="ma-card">
          <span class="ma-card-value">{{ formatPrice(summary.globalAvgPrice) }}</span>
          <span class="ma-card-label">Prix moyen</span>
        </div>
        <div class="ma-card">
          <span class="ma-card-value">{{ formatPrice(summary.globalAvgPricePerM2) }}/m²</span>
          <span class="ma-card-label">Prix au m² moyen</span>
        </div>
        <div class="ma-card">
          <span class="ma-card-value">{{ summary.totalListings }}</span>
          <span class="ma-card-label">Annonces actives</span>
        </div>
        <div class="ma-card">
          <span class="ma-card-value">{{ summary.totalCities }}</span>
          <span class="ma-card-label">Villes analysées</span>
        </div>
      </div>

      <section class="ma-section">
        <h2>Répartition par type de bien</h2>
        <div class="ma-bar-chart">
          <div
            v-for="t in summary.typeDistribution"
            :key="t.type"
            class="ma-bar-row"
          >
            <span class="ma-bar-label">{{ translateType(t.type) }}</span>
            <div class="ma-bar-track">
              <div
                class="ma-bar-fill"
                :style="{ width: t.percentage + '%' }"
                :aria-label="`${t.type}: ${t.percentage}%`"
              ></div>
            </div>
            <span class="ma-bar-pct">{{ t.percentage }}%</span>
          </div>
        </div>
      </section>

      <section class="ma-section">
        <h2>Top 5 zones les plus actives</h2>
        <div class="ma-zone-grid">
          <div v-for="z in summary.topZones" :key="z.city" class="ma-zone-card">
            <strong>{{ z.city }}</strong>
            <span>{{ z.count }} annonces</span>
            <span>{{ formatPrice(z.avgPrice) }} moy.</span>
          </div>
        </div>
      </section>

      <section class="ma-section">
        <h2>Caractéristiques les plus recherchées</h2>
        <div class="ma-feature-list">
          <div v-for="f in summary.popularFeatures" :key="f.feature" class="ma-feature-tag">
            <span class="ma-feature-name">{{ translateCriteria(f.feature) }}</span>
            <span class="ma-feature-pct">{{ f.percentage }}%</span>
          </div>
        </div>
      </section>

      <div class="ma-cta">
        <p>Vous êtes agent immobilier ?</p>
        <router-link to="/agent/market-analysis-agent" class="ma-cta-btn">
          Accéder à l'analyse complète
        </router-link>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getMarketSummary } from '@/api/datascience'

const loading = ref(true)
const error = ref('')
const summary = ref<any>({})

function formatPrice(v: number) {
  if (!v) return '0 €'
  return new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(v)
}

function translateType(t: string) {
  const map: Record<string, string> = { House: 'Maison', Apartment: 'Appartement', Land: 'Terrain', Commercial: 'Commercial', Office: 'Bureau', Garage: 'Garage', Parking: 'Parking' }
  return map[t] || t
}

function translateCriteria(c: string) {
  const map: Record<string, string> = { Studio: 'Studio', Balcony: 'Balcon', Terrace: 'Terrasse', Garden: 'Jardin', Garage: 'Garage', Parking: 'Parking', Cellar: 'Cave', SwimmingPool: 'Piscine', Elevator: 'Ascenseur', AirConditioning: 'Climatisation', FiberOptic: 'Fibre', SmartHome: 'Domotique' }
  return map[c] || c
}

onMounted(async () => {
  try {
    summary.value = await getMarketSummary()
  } catch (e: any) {
    error.value = 'Impossible de charger les analyses. Veuillez réessayer.'
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.ma-page {
  max-width: 1100px;
  margin: 0 auto;
  padding: 2rem 1rem;
}
.ma-hero {
  text-align: center;
  margin-bottom: 2.5rem;
}
.ma-hero h1 {
  font-size: 2rem;
  color: #1e2956;
  margin-bottom: 0.5rem;
}
.ma-hero p {
  color: #6b7280;
  font-size: 1.05rem;
}
.ma-loading,
.ma-error {
  text-align: center;
  padding: 3rem;
  color: #6b7280;
}
.ma-error {
  color: #dc2626;
}
.ma-summary-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: 1rem;
  margin-bottom: 2.5rem;
}
.ma-card {
  background: #fff;
  border-radius: 12px;
  padding: 1.5rem;
  text-align: center;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.08);
}
.ma-card-value {
  display: block;
  font-size: 1.4rem;
  font-weight: 800;
  color: #1e2956;
}
.ma-card-label {
  display: block;
  font-size: 0.85rem;
  color: #6b7280;
  margin-top: 0.3rem;
}
.ma-section {
  margin-bottom: 2.5rem;
}
.ma-section h2 {
  font-size: 1.3rem;
  color: #1e2956;
  margin-bottom: 1rem;
  border-left: 4px solid #1e2956;
  padding-left: 0.75rem;
}
.ma-bar-chart {
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
}
.ma-bar-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}
.ma-bar-label {
  width: 120px;
  font-size: 0.9rem;
  color: #374151;
}
.ma-bar-track {
  flex: 1;
  height: 20px;
  background: #e5e7eb;
  border-radius: 10px;
  overflow: hidden;
}
.ma-bar-fill {
  height: 100%;
  background: linear-gradient(90deg, #1e2956, #3b4a8a);
  border-radius: 10px;
  transition: width 0.6s ease;
}
.ma-bar-pct {
  width: 45px;
  text-align: right;
  font-size: 0.85rem;
  color: #6b7280;
}
.ma-zone-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
}
.ma-zone-card {
  background: #fff;
  border-radius: 10px;
  padding: 1rem;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.06);
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}
.ma-zone-card strong {
  color: #1e2956;
  font-size: 1rem;
}
.ma-zone-card span {
  font-size: 0.85rem;
  color: #6b7280;
}
.ma-feature-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
}
.ma-feature-tag {
  background: #eef2ff;
  color: #1e2956;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.9rem;
  display: flex;
  gap: 0.4rem;
  align-items: center;
}
.ma-feature-pct {
  background: #1e2956;
  color: #fff;
  padding: 0.1rem 0.5rem;
  border-radius: 10px;
  font-size: 0.75rem;
  font-weight: 700;
}
.ma-cta {
  text-align: center;
  margin-top: 3rem;
  padding: 2rem;
  background: linear-gradient(135deg, #1e2956, #2d3a6e);
  border-radius: 12px;
  color: #fff;
}
.ma-cta p {
  margin-bottom: 1rem;
  font-size: 1.1rem;
}
.ma-cta-btn {
  display: inline-block;
  padding: 0.75rem 2rem;
  background: #fff;
  color: #1e2956;
  font-weight: 700;
  border-radius: 8px;
  text-decoration: none;
  transition: transform 0.2s;
}
.ma-cta-btn:hover {
  transform: scale(1.05);
}
</style>
