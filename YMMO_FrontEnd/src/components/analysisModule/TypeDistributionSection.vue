<template>
  <div class="td-row">
    <div class="td-pie"><ChartWrapper type="doughnut" :data="donutData" height="240px" /></div>
    <div class="td-bars">
      <div v-for="t in types" :key="t.type" class="td-bar-row">
        <span class="td-bar-label">{{ tt(t.type) }}</span>
        <div class="td-bar-track"><div class="td-bar-fill" :style="{ width: t.percentage + '%' }"></div></div>
        <span class="td-bar-pct">{{ t.percentage }}%</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import ChartWrapper from '@/components/charts/ChartWrapper.vue'

const props = defineProps<{ types: { type: string; count: number; percentage: number }[] }>()
const COLORS = ['#1e2956', '#2d4a7a', '#3b6a9e', '#4a8ac2', '#59aae6', '#78bce8', '#97cef0']
const tt = (t: string) => ({ House: 'Maison', Apartment: 'Appartement', Land: 'Terrain', Commercial: 'Commercial', Office: 'Bureau', Garage: 'Garage', Parking: 'Parking' }[t] || t)

const donutData = computed(() => ({
  labels: props.types.map(t => tt(t.type)),
  datasets: [{ data: props.types.map(t => t.count), backgroundColor: COLORS, borderWidth: 1 }],
}))
</script>

<style scoped>
.td-row { display: grid; grid-template-columns: 1fr 1fr; gap: 1.5rem; }
.td-pie { max-width: 280px; margin: 0 auto; }
.td-bars { display: flex; flex-direction: column; gap: 0.6rem; justify-content: center; }
.td-bar-row { display: flex; align-items: center; gap: 0.6rem; }
.td-bar-label { width: 100px; font-size: 0.85rem; color: #374151; }
.td-bar-track { flex: 1; height: 20px; background: #e5e7eb; border-radius: 10px; overflow: hidden; }
.td-bar-fill { height: 100%; background: linear-gradient(90deg, #1e2956, #4a5a9a); border-radius: 10px; transition: width 0.6s ease; }
.td-bar-pct { width: 45px; text-align: right; font-size: 0.85rem; color: #6b7280; }
@media (max-width: 600px) { .td-row { grid-template-columns: 1fr; } }
</style>
