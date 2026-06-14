<template>
  <div class="results-container">
    <div class="sort-bar">
      <select v-model="sortKey" aria-label="Trier par">
        <option value="price">Prix</option>
        <option value="surface">Surface</option>
        <option value="rooms">Pièces</option>
      </select>
      <select v-model="sortOrder" aria-label="Ordre">
        <option value="asc">Croissant</option>
        <option value="desc">Décroissant</option>
      </select>
      <select v-model="perPage" aria-label="Résultats par page">
        <option :value="12">12 / page</option>
        <option :value="24">24 / page</option>
        <option :value="48">48 / page</option>
      </select>
    </div>

    <p class="result-count">{{ totalCount }} résultat(s)</p>

    <nav class="pagination top-pagination" v-if="totalPages > 1" aria-label="Pagination des résultats">
      <button :disabled="page <= 1" @click="page = 1" aria-label="Première page">&laquo;</button>
      <button :disabled="page <= 1" @click="page = Math.max(1, page - 10)" aria-label="-10 pages">&lsaquo;10</button>
      <button :disabled="page <= 1" @click="page--" aria-label="Page précédente">&lsaquo;</button>

      <template v-for="p in visiblePages" :key="p">
        <span v-if="p === '...'" class="dots">...</span>
        <button v-else :class="{ active: p === page }" @click="page = p" :aria-label="`Page ${p}`" :aria-current="p === page ? 'page' : undefined">{{ p }}</button>
      </template>

      <button :disabled="page >= totalPages" @click="page++" aria-label="Page suivante">&rsaquo;</button>
      <button :disabled="page >= totalPages" @click="page = Math.min(totalPages, page + 10)" aria-label="+10 pages">10&rsaquo;</button>
      <button :disabled="page >= totalPages" @click="page = totalPages" aria-label="Dernière page">&raquo;</button>
    </nav>

    <div class="property-grid">
      <PropertyCard v-for="prop in paginatedProperties" :key="prop.id" :property="prop" />
    </div>

    <nav class="pagination" v-if="totalPages > 1" aria-label="Pagination des résultats">
      <button :disabled="page <= 1" @click="page = 1" aria-label="Première page">&laquo;</button>
      <button :disabled="page <= 1" @click="page = Math.max(1, page - 10)" aria-label="-10 pages">&lsaquo;10</button>
      <button :disabled="page <= 1" @click="page--" aria-label="Page précédente">&lsaquo;</button>

      <template v-for="p in visiblePages" :key="p">
        <span v-if="p === '...'" class="dots">...</span>
        <button v-else :class="{ active: p === page }" @click="page = p" :aria-label="`Page ${p}`" :aria-current="p === page ? 'page' : undefined">{{ p }}</button>
      </template>

      <button :disabled="page >= totalPages" @click="page++" aria-label="Page suivante">&rsaquo;</button>
      <button :disabled="page >= totalPages" @click="page = Math.min(totalPages, page + 10)" aria-label="+10 pages">10&rsaquo;</button>
      <button :disabled="page >= totalPages" @click="page = totalPages" aria-label="Dernière page">&raquo;</button>
    </nav>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useFilterStore } from '@/stores/filterStore'
import { useCustomProperties } from '@/stores/customProperties.store'
import { generateMockProperties } from '@/utils/mockData'
import PropertyCard from '@/components/PropertyCard.vue'

const filterStore = useFilterStore()
const { getAll: getCustomProperties } = useCustomProperties()

const sortKey = ref('price')
const sortOrder = ref('asc')
const perPage = ref(12)
const page = ref(1)

const mockProperties = computed(() => {
  const generated = generateMockProperties(400)
  const custom = getCustomProperties()
  return [...custom, ...generated]
})

const filteredMock = computed(() => {
  const f = filterStore.filters
  return mockProperties.value.filter(p => {
    const q = (f.city || '').toLowerCase()
    const queryMatch = !q || p.title.toLowerCase().includes(q) || p.address.toLowerCase().includes(q)
    const priceMatch = p.price >= f.minPrice && p.price <= f.maxPrice
    const surfaceMatch = p.surface >= f.minSurface && p.surface <= f.maxSurface
    const typeMatch = f.types.length === 0 || f.types.includes(p.type)
    const conditionMatch = f.conditions.length === 0 || f.conditions.includes(p.condition)
    const energyMatch = f.energyClasses.length === 0 || f.energyClasses.includes(p.energyClass)
    const roomsMatch = f.rooms.length === 0 || f.rooms.some(r => p.rooms >= Number(r))
    const furnishingMatch = !f.furnishing || p.furnishing === f.furnishing
    const criteriaMatch = f.requiredCriteria.every(c => p.mainFeatures.includes(c))
    return queryMatch && priceMatch && surfaceMatch && typeMatch &&
        conditionMatch && energyMatch && roomsMatch &&
        furnishingMatch && criteriaMatch
  })
})

const displayedProperties = computed(() => {
  return filteredMock.value.map(p => ({
    ...p,
    image: p.image || (p.pictures?.[0]?.url) || 'https://via.placeholder.com/300',
  }))
})

const sortedProperties = computed(() => {
  const arr = [...displayedProperties.value]
  arr.sort((a, b) => {
    const va = a[sortKey.value] ?? 0
    const vb = b[sortKey.value] ?? 0
    return sortOrder.value === 'asc' ? va - vb : vb - va
  })
  return arr
})

const totalCount = computed(() => sortedProperties.value.length)

const totalPages = computed(() => Math.max(1, Math.ceil(totalCount.value / perPage.value)))

const paginatedProperties = computed(() => {
  const start = (page.value - 1) * perPage.value
  return sortedProperties.value.slice(start, start + perPage.value)
})

const visiblePages = computed(() => {
  const total = totalPages.value
  const cur = page.value
  const maxVisible = 5
  if (total <= maxVisible) {
    return Array.from({ length: total }, (_, i) => i + 1)
  }
  const pages = []
  let start = Math.max(1, cur - 2)
  let end = Math.min(total, start + maxVisible - 1)
  if (end - start < maxVisible - 1) {
    start = Math.max(1, end - maxVisible + 1)
  }
  if (start > 1) pages.push(1)
  if (start > 2) pages.push('...')
  for (let i = start; i <= end; i++) pages.push(i)
  if (end < total - 1) pages.push('...')
  if (end < total) pages.push(total)
  return pages
})
</script>

<style scoped>
.results-container { padding: 2rem 0; }
.sort-bar { display: flex; gap: 1rem; flex-wrap: wrap; margin-bottom: 1rem; align-items: center; }
.sort-bar select { padding: 0.5rem 1rem; border: 1px solid #d1d5db; border-radius: 8px; background: white; font-size: 0.9rem; }
.result-count { font-size: 1rem; color: #6b7280; margin-bottom: 1.5rem; }
.property-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 2rem;
}
.pagination { display: flex; justify-content: center; align-items: center; gap: 0.25rem; margin-top: 2rem; flex-wrap: wrap; }
.pagination button { min-width: 2.25rem; height: 2.25rem; border: 1px solid #d1d5db; border-radius: 6px; background: white; cursor: pointer; font-size: 0.9rem; display: flex; align-items: center; justify-content: center; transition: background 0.15s, border-color 0.15s; }
.pagination button:hover:not(:disabled) { background: #f3f4f6; border-color: #9ca3af; }
.pagination button:disabled { opacity: 0.35; cursor: default; }
.pagination button.active { background: #1e2956; color: white; border-color: #1e2956; font-weight: 700; }
.pagination .dots { min-width: 2.25rem; text-align: center; color: #6b7280; font-size: 0.9rem; }
.top-pagination { margin-bottom: 1.5rem; }
</style>
