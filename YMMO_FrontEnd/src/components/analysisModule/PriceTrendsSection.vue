<template>
  <div>
    <ChartWrapper v-if="data.trends?.length" type="line" :data="lineData" :options="lineOpts" height="280px" />
    <div class="pt-wrap">
      <table class="pt-table"><thead><tr><th>Période</th><th>Annonces</th><th>Prix moy.</th><th>Prix/m²</th><th>Volume</th></tr></thead>
        <tbody><tr v-for="t in data.trends" :key="t.period"><td>{{ t.period }}</td><td>{{ fn(t.count) }}</td><td>{{ fp(t.avgPrice) }}</td><td>{{ fp(t.avgPricePerM2) }}</td><td>{{ fp(t.totalVolume) }}</td></tr></tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import ChartWrapper from '@/components/charts/ChartWrapper.vue'

const props = defineProps<{ data: any }>()
const fp = (v: number) => v ? new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(v) : '0 €'
const fn = (v: number) => new Intl.NumberFormat('fr-FR').format(v || 0)

const lineData = computed(() => ({
  labels: (props.data.trends || []).map((t: any) => t.period),
  datasets: [
    { label: 'Prix moyen', data: (props.data.trends || []).map((t: any) => t.avgPrice), borderColor: '#1e2956', backgroundColor: 'rgba(30,41,86,0.1)', fill: true, tension: 0.3, pointRadius: 4 },
    { label: 'Prix / m²', data: (props.data.trends || []).map((t: any) => t.avgPricePerM2), borderColor: '#059669', backgroundColor: 'rgba(5,150,105,0.1)', fill: true, tension: 0.3, pointRadius: 3 },
  ],
}))
const lineOpts = { responsive: true, plugins: { legend: { position: 'top' as const } }, scales: { y: { beginAtZero: true, ticks: { callback: (v: any) => fp(v) } } } }
</script>

<style scoped>
.pt-wrap { overflow-x: auto; margin-top: 1rem; }
.pt-table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
.pt-table th { background: #f3f4f6; text-align: left; padding: 0.5rem; font-weight: 600; color: #374151; }
.pt-table td { padding: 0.4rem 0.5rem; border-bottom: 1px solid #f3f4f6; color: #4b5563; }
</style>
