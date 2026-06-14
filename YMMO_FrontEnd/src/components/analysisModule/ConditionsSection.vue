<template>
  <ChartWrapper type="polarArea" :data="chartData" height="300px" />
</template>

<script setup lang="ts">
import { computed } from 'vue'
import ChartWrapper from '@/components/charts/ChartWrapper.vue'

const props = defineProps<{ conditions: { condition: string; count: number }[] }>()
const tc = (c: string) => ({ New: 'Neuf', Excellent: 'Excellent', Good: 'Bon', NeedsRefresh: 'À rafraîchir', NeedsRenovation: 'À rénover', Ruin: 'Ruine' }[c] || c)
const COND_COLORS = ['#059669', '#10b981', '#f59e0b', '#f97316', '#ef4444', '#991b1b']

const chartData = computed(() => ({
  labels: props.conditions.map(c => tc(c.condition)),
  datasets: [{ data: props.conditions.map(c => c.count), backgroundColor: COND_COLORS, borderWidth: 1 }],
}))
</script>
