<template>
  <div class="results-container">
    <div class="results-header">
      <h2>{{ properties.length }} logements disponibles</h2>

      <select v-model="sortOrder" class="sort-select">
        <option value="default">Meilleurs résultats</option>
        <option value="asc">Prix croissant</option>
        <option value="desc">Prix décroissant</option>
      </select>
    </div>

    <div class="properties-grid">
      <PropertyCard
          v-for="prop in sortedProperties"
          :key="prop.id"
          :property="prop"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import PropertyCard from './PropertyCard.vue';

const props = defineProps({
  properties: { type: Array, required: true }
});

const sortOrder = ref('default');

const sortedProperties = computed(() => {
  let sorted = [...props.properties];

  if (sortOrder.value === 'asc') {
    return sorted.sort((a, b) => a.price - b.price);
  } else if (sortOrder.value === 'desc') {
    return sorted.sort((a, b) => b.price - a.price);
  }

  return sorted;
});
</script>

<style scoped>
.results-container { padding: 2rem 0; }
.results-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
.properties-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}
.sort-select { padding: 0.5rem; border-radius: 8px; border: 1px solid #e2e8f0; cursor: pointer; }
</style>