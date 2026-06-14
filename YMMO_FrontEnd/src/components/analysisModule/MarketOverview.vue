<template>
  <div class="mo-stats">
    <div class="mo-stat"><span>Total annonces</span><strong>{{ formatNumber(data.totalListings) }}</strong></div>
    <div class="mo-stat"><span>Prix moyen</span><strong>{{ formatPrice(data.globalAvgPrice) }}</strong></div>
    <div class="mo-stat"><span>Prix / m²</span><strong>{{ formatPrice(data.globalAvgPricePerM2) }}</strong></div>
    <div class="mo-stat"><span>Surface moyenne</span><strong>{{ formatNumber(data.globalAvgSurface) }} m²</strong></div>
    <div class="mo-stat"><span v-if="data.totalCities">Villes</span><strong v-if="data.totalCities">{{ data.totalCities }}</strong></div>
    <div class="mo-stat"><span>Min → Max</span><strong>{{ formatPrice(data.minPrice) }} → {{ formatPrice(data.maxPrice) }}</strong></div>
  </div>
</template>

<script setup lang="ts">
defineProps<{ data: any }>()
const formatPrice = (v: number) => v ? new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(v) : '0 €'
const formatNumber = (v: number) => new Intl.NumberFormat('fr-FR').format(v || 0)
</script>

<style scoped>
.mo-stats { display: grid; grid-template-columns: repeat(auto-fit, minmax(140px, 1fr)); gap: 1rem; }
.mo-stat { text-align: center; padding: 0.75rem; background: #f9fafb; border-radius: 8px; }
.mo-stat span { display: block; font-size: 0.8rem; color: #6b7280; }
.mo-stat strong { font-size: 1.15rem; color: #1e2956; }
</style>
