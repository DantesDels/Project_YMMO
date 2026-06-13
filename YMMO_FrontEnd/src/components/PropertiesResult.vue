<!--
  PropertiesResult.vue
  ─────────────────────────────────────────────────────────────
  Deux modes, contrôlés par la prop "mode" :

  - mode="mock" (défaut) : filtre generateMockProperties() en
    local selon filterStore.filters. Fonctionne hors-ligne,
    utile pour la démo si le backend n'est pas joignable.

  - mode="api" : affiche directement property.store.properties
    (alimenté par property.store.search() via le parent).
    Le tri reste géré ici, mais le filtrage est fait côté backend.

  Le format affiché par PropertyCard diffère légèrement entre
  les deux modes (mock = champs plats type "type/price/address",
  api = PropertySummaryDto type "propertyType/currentPrice/city").
  PropertyCard doit donc accepter les deux formes — voir le
  composant normalizeProperty() ci-dessous qui uniformise.
-->
<template>
  <div class="results-container">
    <div class="results-header">
      <h2>{{ displayedProperties.length }} logement{{ displayedProperties.length === 1 ? '' : 's' }} disponible{{ displayedProperties.length === 1 ? '' : 's' }}</h2>
      <select v-model="sortOrder" class="sort-select">
        <option value="default">Meilleurs résultats</option>
        <option value="asc">Prix croissant</option>
        <option value="desc">Prix décroissant</option>
      </select>
    </div>

    <div v-if="isLoading" class="loading-state">
      Recherche en cours...
    </div>

    <div v-else class="properties-grid">
      <PropertyCard
          v-for="prop in sortedProperties"
          :key="prop.id"
          :property="prop"
      />
    </div>

    <div v-if="!isLoading && displayedProperties.length === 0" class="no-results">
      Aucun bien trouvé pour ces critères.
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import PropertyCard from './PropertyCard.vue';
import { useFilterStore } from '@/stores/filterStore';
import { generateMockProperties } from '@/utils/mockData';

const props = defineProps({
  // 'mock' : filtrage local sur des données générées
  // 'api'  : utilise allProperties tel que fourni par le parent
  //          (déjà filtré par le backend via property.store.search)
  mode: { type: String, default: 'mock' },

  // En mode 'api', le parent passe property.store.properties (PropertySummaryDto[])
  // En mode 'mock', ignoré — les données sont générées localement
  allProperties: { type: Array, default: () => [] },

  isLoading: { type: Boolean, default: false },

  // Nombre de biens mock à générer (uniquement mode 'mock')
  mockCount: { type: Number, default: 400 },
});

const filterStore = useFilterStore();
const sortOrder = ref('default');

// ── Données mock générées une seule fois ────────────────────
const mockProperties = generateMockProperties(props.mockCount);

// ── Normalisation : uniformise mock + PropertySummaryDto ─────
// PropertyCard ne reçoit toujours que ce format, peu importe la source
function normalizeProperty(p) {
  if (props.mode === 'api') {
    // PropertySummaryDto → format unifié
    return {
      id: p.propertyId,
      title: p.propertyName,
      type: p.propertyType,
      condition: p.condition,
      price: p.currentPrice,
      surface: p.surface,
      address: `${p.city} (${p.postalCode})`,
      mainFeatures: p.mainFeatures ?? [],
      // Champs absents du summary backend → valeurs par défaut
      image: `https://picsum.photos/seed/${p.propertyId}/400/300`,
      availabilityDate: null,
      rooms: null,
      furnishing: null,
      energyClass: null,
    };
  }
  // Mock déjà au format unifié
  return p;
}

// ── Filtrage (mode mock uniquement) ──────────────────────────
const filteredMock = computed(() => {
  const f = filterStore.filters;

  return mockProperties.filter(p => {
    const cityMatch = !f.city || p.address.toLowerCase().includes(f.city.toLowerCase());
    const priceMatch = p.price >= f.minPrice && p.price <= f.maxPrice;
    const surfaceMatch = p.surface >= f.minSurface && p.surface <= f.maxSurface;
    const typeMatch = f.types.length === 0 || f.types.includes(p.type);
    const conditionMatch = f.conditions.length === 0 || f.conditions.includes(p.condition);
    const energyMatch = f.energyClasses.length === 0 || f.energyClasses.includes(p.energyClass);
    const roomsMatch = f.rooms.length === 0 || f.rooms.some(r => p.rooms >= Number(r));
    const furnishingMatch = !f.furnishing || p.furnishing === f.furnishing;
    const criteriaMatch = f.requiredCriteria.every(c => p.mainFeatures.includes(c));

    return cityMatch && priceMatch && surfaceMatch && typeMatch &&
        conditionMatch && energyMatch && roomsMatch &&
        furnishingMatch && criteriaMatch;
  });
});

// ── Résultats affichés selon le mode ─────────────────────────
const displayedProperties = computed(() => {
  const raw = props.mode === 'api' ? props.allProperties : filteredMock.value;
  return raw.map(normalizeProperty);
});

// ── Tri (commun aux deux modes) ──────────────────────────────
const sortedProperties = computed(() => {
  const list = [...displayedProperties.value];
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
.loading-state, .no-results {
  text-align: center;
  padding: 3rem 0;
  color: #64748b;
  font-size: 0.95rem;
}
</style>