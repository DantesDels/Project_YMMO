<template>
  <div class="border rounded-lg p-4 shadow hover:shadow-lg transition">
    <h3 class="text-xl font-semibold">{{ property.propertyName }}</h3>
    <p class="text-gray-600">{{ property.city }} ({{ property.postalCode }})</p>
    <p class="mt-2 text-blue-600 font-bold">{{ property.currentPrice }} €</p>
    <div class="mt-2 flex flex-wrap gap-1">
      <span v-for="feat in property.mainFeatures" :key="feat"
            class="bg-gray-200 text-xs px-2 py-1 rounded">{{ feat }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { PropertySummaryDto } from '@/types'

defineProps<{
  property: PropertySummaryDto
}>()
</script>