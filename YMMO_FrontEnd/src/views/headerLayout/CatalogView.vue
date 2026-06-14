<template>
  <div class="bg-gray-50 min-h-screen">
    <HeroSection />
    <CityGrid />
    <main class="max-w-7xl mx-auto px-4 py-8">
      <PropertiesResult />
    </main>
  </div>
</template>

<script setup>
import { watch } from 'vue'
import { useRoute } from 'vue-router'
import HeroSection from '@/components/HeroSection.vue'
import CityGrid from '@/components/CityGrid.vue'
import PropertiesResult from '@/components/PropertiesResult.vue'
import { useFilterStore } from '@/stores/filterStore'

const route = useRoute()
const filterStore = useFilterStore()

watch(() => route.query.city, (city) => {
  filterStore.updateFilters({ city: city || '' })
}, { immediate: true })
</script>
