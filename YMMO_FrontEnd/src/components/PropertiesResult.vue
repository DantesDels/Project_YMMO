<template>
  <div class="results-container">
    <div class="results-header">
      <h2>{{ totalCount }} logement{{ totalCount === 1 ? '' : 's' }} disponible{{ totalCount === 1 ? '' : 's' }}</h2>
      <div class="header-right">
        <select v-model="perPage" class="per-page-select" @change="currentPage = 1">
          <option :value="10">10 / page</option>
          <option :value="20">20 / page</option>
          <option :value="40">40 / page</option>
          <option :value="100">100 / page</option>
        </select>
        <select v-model="sortOrder" class="sort-select">
          <option value="default">Meilleurs résultats</option>
          <option value="asc">Prix croissant</option>
          <option value="desc">Prix décroissant</option>
        </select>
      </div>
    </div>

    <div v-if="isLoading" class="loading-state">
      Recherche en cours...
    </div>

    <template v-else-if="totalCount > 0">
      <div class="properties-grid">
        <PropertyCard
            v-for="prop in paginatedProperties"
            :key="prop.id"
            :property="prop"
        />
      </div>

      <div v-if="totalPages > 1" class="pagination">
        <button class="page-btn" :disabled="currentPage <= 1" @click="currentPage = 1" title="Première page">
          «
        </button>
        <button class="page-btn" :disabled="currentPage <= 1" @click="jumpPage(-10)">
          ‹
        </button>
        <button class="page-btn" :disabled="currentPage <= 1" @click="currentPage = currentPage - 1">
          ←
        </button>

        <button
          v-for="p in visiblePages"
          :key="p"
          :class="['page-btn', { active: p === currentPage }]"
          @click="currentPage = p"
        >
          {{ p }}
        </button>

        <button class="page-btn" :disabled="currentPage >= totalPages" @click="currentPage = currentPage + 1">
          →
        </button>
        <button class="page-btn" :disabled="currentPage >= totalPages" @click="jumpPage(10)">
          ›
        </button>
        <button class="page-btn" :disabled="currentPage >= totalPages" @click="currentPage = totalPages" title="Dernière page">
          »
        </button>
      </div>
    </template>

    <div v-else class="no-results">
      Aucun bien trouvé pour ces critères.
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';
import PropertyCard from './PropertyCard.vue';
import { useFilterStore } from '@/stores/filterStore';
import { generateMockProperties } from '@/utils/mockData';

const props = defineProps({
  mode: { type: String, default: 'mock' },
  allProperties: { type: Array, default: () => [] },
  isLoading: { type: Boolean, default: false },
  mockCount: { type: Number, default: 400 },
});

const filterStore = useFilterStore();
const sortOrder = ref('default');
const perPage = ref(20);
const currentPage = ref(1);

const mockProperties = generateMockProperties(props.mockCount);

function normalizeProperty(p) {
  if (props.mode === 'api') {
    return {
      id: p.propertyId,
      title: p.propertyName,
      type: p.propertyType,
      condition: p.condition,
      price: p.currentPrice,
      surface: p.surface,
      address: `${p.city} (${p.postalCode})`,
      mainFeatures: p.mainFeatures ?? [],
      image: `https://picsum.photos/seed/${p.propertyId}/400/300`,
      availabilityDate: null,
      rooms: null,
      furnishing: null,
      energyClass: null,
    };
  }
  return p;
}

const filteredMock = computed(() => {
  const f = filterStore.filters;

  return mockProperties.filter(p => {
    const q = (f.city || '').toLowerCase();
    const queryMatch = !q || p.title.toLowerCase().includes(q) || p.address.toLowerCase().includes(q);
    const priceMatch = p.price >= f.minPrice && p.price <= f.maxPrice;
    const surfaceMatch = p.surface >= f.minSurface && p.surface <= f.maxSurface;
    const typeMatch = f.types.length === 0 || f.types.includes(p.type);
    const conditionMatch = f.conditions.length === 0 || f.conditions.includes(p.condition);
    const energyMatch = f.energyClasses.length === 0 || f.energyClasses.includes(p.energyClass);
    const roomsMatch = f.rooms.length === 0 || f.rooms.some(r => p.rooms >= Number(r));
    const furnishingMatch = !f.furnishing || p.furnishing === f.furnishing;
    const criteriaMatch = f.requiredCriteria.every(c => p.mainFeatures.includes(c));

    return queryMatch && priceMatch && surfaceMatch && typeMatch &&
        conditionMatch && energyMatch && roomsMatch &&
        furnishingMatch && criteriaMatch;
  });
});

const displayedProperties = computed(() => {
  const raw = props.mode === 'api' ? props.allProperties : filteredMock.value;
  return raw.map(normalizeProperty);
});

const sortedProperties = computed(() => {
  const list = [...displayedProperties.value];
  if (sortOrder.value === 'asc') return list.sort((a, b) => a.price - b.price);
  if (sortOrder.value === 'desc') return list.sort((a, b) => b.price - a.price);
  return list;
});

const totalCount = computed(() => sortedProperties.value.length);

const totalPages = computed(() => Math.ceil(totalCount.value / perPage.value) || 1);

const paginatedProperties = computed(() => {
  const start = (currentPage.value - 1) * perPage.value;
  return sortedProperties.value.slice(start, start + perPage.value);
});

const visiblePages = computed(() => {
  const total = totalPages.value;
  const current = currentPage.value;
  const pages = [];
  const maxVisible = 5;

  if (total <= maxVisible + 2) {
    for (let i = 1; i <= total; i++) pages.push(i);
  } else {
    pages.push(1);
    const half = Math.floor((maxVisible - 1) / 2);
    let start = Math.max(2, current - half);
    let end = Math.min(total - 1, current + half);

    if (end - start + 1 < maxVisible - 1) {
      if (start === 2) end = start + maxVisible - 2;
      else start = end - maxVisible + 2;
    }

    if (start > 2) pages.push('...');
    for (let i = start; i <= end; i++) pages.push(i);
    if (end < total - 1) pages.push('...');
    pages.push(total);
  }
  return pages;
});

function jumpPage(offset) {
  const next = currentPage.value + offset;
  currentPage.value = Math.max(1, Math.min(totalPages.value, next));
}

watch([filterStore.filters, sortOrder], () => {
  currentPage.value = 1;
}, { deep: true });
</script>

<style scoped>
.results-container { padding: 2rem 0; width: 100%; }

.results-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
  gap: 0.75rem;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.per-page-select,
.sort-select {
  padding: 0.5rem 0.75rem;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
  cursor: pointer;
  background: white;
  font-size: 0.85rem;
  color: #475569;
}

.properties-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
  width: 100%;
}

.loading-state, .no-results {
  text-align: center;
  padding: 3rem 0;
  color: #64748b;
  font-size: 0.95rem;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0.35rem;
  margin-top: 2rem;
}

.page-btn {
  min-width: 36px;
  height: 36px;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  background: white;
  color: #475569;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.15s;
  padding: 0 0.5rem;
}

.page-btn:hover:not(:disabled):not(.active) {
  background: #f1f5f9;
  border-color: #cbd5e1;
}

.page-btn.active {
  background: #1e2956;
  border-color: #1e2956;
  color: white;
}

.page-btn:disabled {
  opacity: 0.4;
  cursor: default;
}
</style>
