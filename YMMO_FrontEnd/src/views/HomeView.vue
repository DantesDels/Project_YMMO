<template>
  <div class="bg-gray-50 min-h-screen">
    <HeroSection />
    <CityGrid />

    <main class="max-w-7xl mx-auto px-4 py-8">
      <PropertiesResult :allProperties="mockProperties" />
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import HeroSection from '@/components/HeroSection.vue';
import CityGrid from '@/components/CityGrid.vue';
import PropertiesResult from '@/components/PropertiesResult.vue';
import { generateMockProperties } from '@/utils/mockData';

const mockProperties = ref(null); // Initialise à null

onMounted(async () => {
  // Utilise un setTimeout pour laisser le DOM se peindre avant le calcul lourd
  setTimeout(() => {
    mockProperties.value = generateMockProperties(); // Nb de Résultats géré par PropertiesResult.vue
  }, 0);
});
</script>