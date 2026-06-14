<template>
  <div>
    <div class="za-grid">
      <div v-for="z in zones" :key="z.city" class="za-card" :class="{ 'za-hot': z.ratioToMarket > 1.1, 'za-aff': z.ratioToMarket < 0.9 }">
        <div class="za-hdr">
          <strong>{{ z.city }}</strong>
          <span class="za-badge" v-if="z.ratioToMarket > 1.1">🔥 Chaude</span>
          <span class="za-badge za-badge-green" v-else-if="z.ratioToMarket < 0.9">💰 Abordable</span>
        </div>
        <div class="za-body"><span>{{ fn(z.count) }} annonces</span><span>{{ fp(z.avgPrice) }}</span><span>{{ fp(z.avgPricePerM2) }}/m²</span><span>Ratio: {{ z.ratioToMarket }}x</span></div>
      </div>
    </div>
    <ChartWrapper type="radar" :data="radarData" :options="radarOpts" height="320px" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import ChartWrapper from '@/components/charts/ChartWrapper.vue'

const props = defineProps<{ zones: any[] }>()
const fp = (v: number) => v ? new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(v) : '0 €'
const fn = (v: number) => new Intl.NumberFormat('fr-FR').format(v || 0)

const radarData = computed(() => {
  const z = props.zones || []
  return {
    labels: z.map((x: any) => x.city),
    datasets: [
      { label: 'Prix moyen', data: z.map((x: any) => x.avgPrice), borderColor: '#1e2956', backgroundColor: 'rgba(30,41,86,0.2)', pointRadius: 4 },
      { label: 'Prix / m²', data: z.map((x: any) => x.avgPricePerM2), borderColor: '#059669', backgroundColor: 'rgba(5,150,105,0.2)', pointRadius: 4 },
    ],
  }
})
const radarOpts = { responsive: true, plugins: { legend: { position: 'top' as const } }, scales: { r: { beginAtZero: true, ticks: { callback: (v: any) => fp(v), stepSize: 50000 } } } }
</script>

<style scoped>
.za-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 0.75rem; margin-bottom: 1.5rem; }
.za-card { padding: 0.75rem; border-radius: 8px; background: #f9fafb; border-left: 4px solid #9ca3af; }
.za-hot { border-left-color: #ef4444; }
.za-aff { border-left-color: #10b981; }
.za-hdr { display: flex; justify-content: space-between; align-items: center; margin-bottom: 0.4rem; }
.za-card strong { color: #1e2956; font-size: 0.95rem; }
.za-badge { font-size: 0.7rem; padding: 0.15rem 0.4rem; border-radius: 4px; background: #fee2e2; color: #dc2626; font-weight: 700; }
.za-badge-green { background: #d1fae5; color: #059669; }
.za-body { display: flex; flex-wrap: wrap; gap: 0.25rem 0.75rem; font-size: 0.8rem; color: #6b7280; }
</style>
