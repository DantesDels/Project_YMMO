<template>
  <ChartWrapper type="bar" :data="chartData" :options="chartOpts" height="280px" />
</template>

<script setup lang="ts">
import { computed } from 'vue'
import ChartWrapper from '@/components/charts/ChartWrapper.vue'

const props = defineProps<{ features: { feature: string; count: number; percentage: number }[] }>()

const chartData = computed(() => ({
  labels: props.features.map(f => f.feature),
  datasets: [{ label: 'Présence', data: props.features.map(f => f.count), backgroundColor: '#3b6a9e', borderRadius: 6 }],
}))
const chartOpts = { indexAxis: 'y' as const, plugins: { legend: { display: false } }, scales: { x: { beginAtZero: true } } }
</script>
