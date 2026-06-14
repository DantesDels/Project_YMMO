<template>
  <ChartWrapper type="bar" :data="chartData" :options="chartOpts" height="300px" />
</template>

<script setup lang="ts">
import { computed } from 'vue'
import ChartWrapper from '@/components/charts/ChartWrapper.vue'

const props = defineProps<{ distribution: { range: string; low: number; high: number; count: number }[] }>()
const fp = (v: number) => v ? new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(v) : '0 €'

const chartData = computed(() => {
  const top = (props.distribution || []).slice(0, 30)
  return {
    labels: top.map((b: any) => fp(b.low)),
    datasets: [{ label: 'Biens', data: top.map((b: any) => b.count), backgroundColor: '#4a8ac2', borderRadius: 4 }],
  }
})
const chartOpts = { plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } }
</script>
