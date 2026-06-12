<template>
  <div class="bg-gray-50 min-h-screen">
    <HeroSection />
    <CityGrid />

    <main class="max-w-7xl mx-auto px-4 py-8">
      <PropertiesResult :properties="mockProperties" />
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import HeroSection from '@/components/HeroSection.vue';
import CityGrid from '@/components/CityGrid.vue';
import PropertiesResult from '@/components/PropertiesResult.vue';
import { generateMockProperties } from '@/utils/mockData';

const mockProperties = ref([]);

onMounted(() => {
  mockProperties.value = generateMockProperties(40);
});
</script>