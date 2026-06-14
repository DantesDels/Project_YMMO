<template>
  <div class="chart-wrapper" :style="{ width, height }">
    <canvas ref="canvasRef"></canvas>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'
import { Chart, registerables } from 'chart.js'

Chart.register(...registerables)

const props = withDefaults(defineProps<{
  type: 'bar' | 'line' | 'pie' | 'doughnut' | 'radar' | 'polarArea' | 'scatter'
  data: any
  options?: any
  width?: string
  height?: string
}>(), { width: '100%', height: '300px' })

const canvasRef = ref<HTMLCanvasElement | null>(null)
let chart: Chart | null = null

function createChart() {
  if (!canvasRef.value) return
  if (chart) chart.destroy()
  chart = new Chart(canvasRef.value, {
    type: props.type,
    data: props.data,
    options: {
      responsive: true,
      maintainAspectRatio: false,
      ...props.options,
    },
  })
}

onMounted(createChart)
onUnmounted(() => chart?.destroy())

watch(() => [props.data, props.type, props.options], createChart, { deep: true })
</script>

<style scoped>
.chart-wrapper {
  position: relative;
}
.chart-wrapper canvas {
  width: 100% !important;
  height: 100% !important;
}
</style>
