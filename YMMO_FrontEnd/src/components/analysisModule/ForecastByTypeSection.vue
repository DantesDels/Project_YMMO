<template>
  <div class="fc-grid">
    <div v-for="f in forecasts" :key="f.type" class="fc-card">
      <strong>{{ tt(f.type) }}</strong>
      <span class="fc-trend" :class="f.trend === 'up' ? 'up' : 'down'">{{ f.trend === 'up' ? '📈 Hausse' : '📉 Baisse' }}</span>
      <span>{{ fp(f.currentAvgPrice) }}</span>
      <span>Coeff: {{ f.coefficient.toFixed(2) }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{ forecasts: { type: string; trend: string; coefficient: number; currentAvgPrice: number }[] }>()
const fp = (v: number) => v ? new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(v) : '0 €'
const tt = (t: string) => ({ House: 'Maison', Apartment: 'Appartement', Land: 'Terrain', Commercial: 'Commercial', Office: 'Bureau', Garage: 'Garage', Parking: 'Parking' }[t] || t)
</script>

<style scoped>
.fc-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(160px, 1fr)); gap: 0.75rem; }
.fc-card { padding: 0.75rem; border-radius: 8px; background: #f9fafb; display: flex; flex-direction: column; gap: 0.25rem; }
.fc-card strong { color: #1e2956; font-size: 0.9rem; }
.fc-trend { font-size: 0.85rem; }
.up { color: #059669; }
.down { color: #dc2626; }
.fc-card span:last-child { font-size: 0.75rem; color: #9ca3af; }
</style>
