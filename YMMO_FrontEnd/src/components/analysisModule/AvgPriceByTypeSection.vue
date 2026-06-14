<template>
  <ChartWrapper type="bar" :data="chartData" :options="chartOpts" height="280px" />
</template>

<script setup lang="ts">
import { computed } from 'vue'
import ChartWrapper from '@/components/charts/ChartWrapper.vue'

const props = defineProps<{ avgPriceByType: { type: string; avgPrice: number; count: number }[] }>()
const COLORS = ['#1e2956', '#2d4a7a', '#3b6a9e', '#4a8ac2', '#59aae6', '#78bce8', '#97cef0']
const tt = (t: string) => ({ House: 'Maison', Apartment: 'Appartement', Land: 'Terrain', Commercial: 'Commercial', Office: 'Bureau', Garage: 'Garage', Parking: 'Parking' }[t] || t)
const fp = (v: number) => v ? new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(v) : '0 €'

const chartData = computed(() => ({
  labels: props.avgPriceByType.map(t => tt(t.type)),
  datasets: [{ label: 'Prix moyen', data: props.avgPriceByType.map(t => t.avgPrice), backgroundColor: COLORS, borderRadius: 6 }],
}))
const chartOpts = { plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true, ticks: { callback: (v: any) => fp(v) } } } }
</script>
