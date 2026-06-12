<template>
  <div class="results-container">
    <div class="results-header">
      <h2>{{ filteredProperties.length }} logements disponibles</h2>
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

    <div v-if="filteredProperties.length === 0" class="no-results">
      Aucun bien trouvé pour ces critères (Prix max: {{ filterStore.filters.maxPrice }})
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import PropertyCard from './PropertyCard.vue';
import { useFilterStore } from '@/stores/filterStore';

const filterStore = useFilterStore();
const props = defineProps({
  allProperties: { type: Array, default: () => [] }
});

const sortOrder = ref('default');

const filteredProperties = computed(() => {
  const f = filterStore.filters;

  return props.allProperties.filter(p => {
    // 1. Filtrage Ville : Vérifie si la ville saisie est contenue dans l'adresse
    // .trim() et .toLowerCase() pour éviter les erreurs de saisie
    const cityMatch = !f.city ||
        f.city.trim() === '' ||
        p.address.toLowerCase().includes(f.city.toLowerCase().trim());

    // 2. Filtrage Prix (avec sécurité)
    const price = Number(p.price) || 0;
    const priceMatch = price >= (f.minPrice || 0) && price <= (f.maxPrice || 5000000);

    // 3. Filtrage Type
    const typeMatch = f.types.length === 0 || f.types.includes(p.type || p.propertyType);
    
    return cityMatch && priceMatch && typeMatch;
  });
});

const sortedProperties = computed(() => {
  const list = [...filteredProperties.value];
  if (sortOrder.value === 'asc') return list.sort((a, b) => a.price - b.price);
  if (sortOrder.value === 'desc') return list.sort((a, b) => b.price - a.price);
  return list;
});
</script>

<style scoped>
.results-container { padding: 2rem 0; width: 100%; }
.results-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
.properties-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
  width: 100%;
}
.sort-select { padding: 0.5rem; border-radius: 8px; border: 1px solid #e2e8f0; cursor: pointer; }
</style>