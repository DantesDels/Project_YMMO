<template>
  <div>
    <div class="pr-meta"><span>Modèle: {{ data.model }}</span><span>Confiance: {{ (data.confidence * 100).toFixed(1) }}%</span><span>Données: {{ fn(data.dataPoints) }}</span></div>
    <ChartWrapper type="line" :data="lineData" :options="lineOpts" height="280px" />
    <div class="pr-wrap">
      <table class="pr-table"><thead><tr><th>Mois</th><th>Prix prévu</th></tr></thead>
        <tbody><tr v-for="p in data.predictions" :key="p.month"><td>{{ p.month }}</td><td>{{ fp(p.predictedAvgPrice) }}</td></tr></tbody>
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
  labels: (props.data.predictions || []).map((p: any) => p.month),
  datasets: [{ label: 'Prix prévu', data: (props.data.predictions || []).map((p: any) => p.predictedAvgPrice), borderColor: '#7c3aed', backgroundColor: 'rgba(124,58,237,0.1)', fill: true, tension: 0.3, pointRadius: 5, pointBackgroundColor: '#7c3aed' }],
}))
const lineOpts = { responsive: true, plugins: { legend: { position: 'top' as const } }, scales: { y: { beginAtZero: true, ticks: { callback: (v: any) => fp(v) } } } }
</script>

<style scoped>
.pr-meta { display: flex; gap: 1.5rem; font-size: 0.85rem; color: #6b7280; margin-bottom: 1rem; }
.pr-wrap { overflow-x: auto; margin-top: 1rem; }
.pr-table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
.pr-table th { background: #f3f4f6; text-align: left; padding: 0.5rem; font-weight: 600; color: #374151; }
.pr-table td { padding: 0.4rem 0.5rem; border-bottom: 1px solid #f3f4f6; color: #4b5563; }
</style>
