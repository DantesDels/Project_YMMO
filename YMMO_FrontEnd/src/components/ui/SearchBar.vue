<!--
  SearchBar.vue
  ─────────────────────────────────────────────────────────────
  N'appelle PAS le backend directement. Écrit dans le filterStore.
  C'est le composant parent (HomeView / SearchView) qui décide
  quoi faire de ces filtres :
    - mode mock   → PropertiesResult filtre generateMockProperties()
    - mode API    → property.store.search(filterStore.toApiCriteria())

  Émet "search" pour signaler au parent qu'une recherche est
  déclenchée (utile en mode API pour lancer le fetch).
-->
<template>
  <div class="search-container">
    <div class="search-bar">

      <SearchField v-model="city" placeholder="Ville, code postal..." />

      <SearchDropdown
          v-model="selectedTypes"
          label="Type"
          :options="propertyTypeMap"
      />

      <BudgetRangeSlider
          v-model="budgetRange"
          label="Budget"
          :min="0"
          :max="1000000"
          :step="5000"
          unit="€"
      />

      <SearchDropdown
          v-model="selectedRoomCapacity"
          label="Pièces"
          :options="roomCapacityMap"
      />

      <SearchDropdown
          v-model="selectedEnergyClass"
          label="DPE"
          :options="energyClassMap"
      />

      <SearchDropdown
          v-model="selectedPhysicalCondition"
          label="État"
          :options="physicalConditionMap"
      />

      <SearchDropdownExtended
          v-model="selectedCriteria"
          label="Critères"
          :options="criteriaMap"
          empty-label="Aucun"
      />

      <div class="actions-group">
        <button v-if="hasActiveFilters" class="clear-all-btn" type="button" @click="resetAllFilters" aria-label="Réinitialiser tous les filtres">
          ×
        </button>
        <AppButton class="search-btn" @click="onSearch">
          Rechercher
        </AppButton>
      </div>
      
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';
import AppButton from '@/components/ui/AppButton.vue';
import SearchField from '@/components/ui/SearchField.vue';
import SearchDropdown from '@/components/ui/SearchDropdown.vue';
import SearchDropdownExtended from '@/components/ui/SearchDropdownExtended.vue';
import BudgetRangeSlider from '@/components/ui/BudgetRangeSlider.vue';
import { useFilterStore, } from '@/stores/filterStore';
import { vClickOutside } from '@/utils/useClickOutside';

const filterStore = useFilterStore();
const emit = defineEmits(['search']);

// ── Budget : v-model simple, synchronisé vers filterStore ──────
const budgetRange = computed({
  get: () => ({
    min: filterStore.filters.minPrice,
    max: filterStore.filters.maxPrice,
  }),
  set: (val) => {
    filterStore.updateFilters({ minPrice: val.min, maxPrice: val.max });
    emit('search');
  },
});

// ── État local — synchronisé vers le filterStore ──────────────
const city = ref(filterStore.filters.city);
const selectedTypes = ref([...filterStore.filters.types]);

// Multi-select : tableaux synchronisés directement
const selectedRoomCapacity = ref([...filterStore.filters.rooms]);
const selectedEnergyClass = ref([...filterStore.filters.energyClasses]);
const selectedPhysicalCondition = ref([...filterStore.filters.conditions]);

// requiredCriteria est déjà un tableau → multi-select natif
const selectedCriteria = ref([...filterStore.filters.requiredCriteria]);

// ── Budget : état local du popover ─────────────────────────────
const isBudgetOpen = ref(false);
// Valeurs en attente, appliquées seulement au clic "Appliquer"
// (cohérent avec le bouton Appliquer des autres dropdowns)
const pendingBudget = ref({
  min: filterStore.filters.minPrice,
  max: filterStore.filters.maxPrice,
});

const toggleBudget = () => { isBudgetOpen.value = !isBudgetOpen.value; };
const closeBudget = () => { isBudgetOpen.value = false; };

const onBudgetChange = (val) => {
  pendingBudget.value = val;
};

const applyBudget = () => {
  filterStore.updateFilters({
    minPrice: pendingBudget.value.min,
    maxPrice: pendingBudget.value.max,
  });
  isBudgetOpen.value = false;
  emit('search');
};

const resetBudget = () => {
  pendingBudget.value = { min: 0, max: 1000000 };
  filterStore.updateFilters({ minPrice: 0, maxPrice: 1000000 });
  emit('search');
};

const budgetDisplay = computed(() => {
  const { minPrice, maxPrice } = filterStore.filters;
  const isDefault = minPrice === 0 && maxPrice === 1000000;
  if (isDefault) return 'Tous';
  return `${formatCompact(minPrice)} - ${formatCompact(maxPrice)}`;
});

function formatCompact(val) {
  if (val >= 1000000) return `${(val / 1000000).toFixed(1).replace('.0', '')}M€`;
  if (val >= 1000) return `${Math.round(val / 1000)}k€`;
  return `${val}€`;
}

// ── Maps d'options (miroir des enums C#) ──────────────────────

const propertyTypeMap = {
  House: 'Maison',
  Apartment: 'Appartement',
  Land: 'Terrain',
  Commercial: 'Local',
  Office: 'Bureau',
  Garage: 'Garage',
  Parking: 'Parking',
};

// Pas de champ "rooms" dans PropertySearchCriteriaDto côté backend —
// filtre purement local (mode mock) pour l'instant
const roomCapacityMap = {
  '1': '1 pièce',
  '2': '2 pièces',
  '3': '3 pièces',
  '4': '4 pièces',
  '5': '5 pièces et +',
};

// Miroir de Domain/Enums/EnergyClass.cs
// IMPORTANT : la clé est "Exempt" (pas "Ex") pour matcher l'enum C#
const energyClassMap = {
  A: 'A',
  B: 'B',
  C: 'C',
  D: 'D',
  E: 'E',
  F: 'F',
  G: 'G',
  Exempt: 'Non soumis au DPE',
};

// Miroir de Domain/Enums/PhysicalCondition.cs
const physicalConditionMap = {
  New: 'Neuf',
  Excellent: 'Excellent état',
  Good: 'Bon état',
  NeedsRefresh: 'À rafraîchir',
  NeedsRenovation: 'À rénover',
  Ruin: 'Ruine',
};

// Miroir de Domain/Entities/Enums/Criteria.cs (27 valeurs)
const criteriaMap = {
  "Extérieurs et Annexes": {
    Balcony: 'Balcon',
    Terrace: 'Terrasse',
    Garden: 'Jardin',
    Garage: 'Garage fermé',
    Parking: 'Parking',
    Cellar: 'Cave',
    SwimmingPool: 'Piscine'
  },
  "Intérieur et Confort": {
    Elevator: 'Ascenseur',
    AirConditioning: 'Climatisation',
    Fireplace: 'Cheminée',
    Furnished: 'Meublé',
    HardwoodFloor: 'Parquet',
    DoubleGlazing: 'Double vitrage',
    FittedKitchen: 'Cuisine équipée'
  },
  "Sécurité et Vues": {
    Digicode: 'Digicode',
    Intercom: 'Interphone',
    AlarmSystem: 'Alarme',
    SecurityDoor: 'Porte blindée',
    Caretaker: 'Gardien',
    SeaView: 'Vue mer',
    MountainView: 'Vue montagne',
    UnobstructedView: 'Vue dégagée',
    SouthFacing: 'Exposition sud'
  },
  "Tech & Énergie": {
    DisabledAccess: 'Accès PMR',
    FiberOptic: 'Fibre optique',
    SmartHome: 'Domotique',
    HeatPump: 'Pompe à chaleur',
    SolarPanels: 'Panneaux solaires'
  }
};

// ── Filtres actifs ? ────────────────────────────────────────────
const hasActiveFilters = computed(() => {
  const f = filterStore.filters
  return !!f.city || f.types.length > 0 || f.conditions.length > 0 ||
      f.energyClasses.length > 0 || f.rooms.length > 0 ||
      f.requiredCriteria.length > 0 ||
      f.minPrice !== 0 || f.maxPrice !== 1000000
})

const resetAllFilters = () => {
  filterStore.resetFilters()
  city.value = ''
  selectedTypes.value = []
  selectedRoomCapacity.value = []
  selectedEnergyClass.value = []
  selectedPhysicalCondition.value = []
  selectedCriteria.value = []
  pendingBudget.value = { min: 0, max: 1000000 }
  emit('search')
}

// ── Soumission ─────────────────────────────────────────────────
const onSearch = () => {
  filterStore.updateFilters({
    city: city.value,
    types: selectedTypes.value,
  });
  emit('search');
};

// ── Watchers : synchronisation vers filterStore ─────────────────

// Synchronisation bidirectionnelle — temps réel
watch(city, (val) => {
  if (val === filterStore.filters.city) return
  filterStore.updateFilters({ city: val });
  emit('search');
});

// ← filterStore → SearchBar (quand changé de l'extérieur, ex: CityGrid)
watch(() => filterStore.filters.city, (val) => {
  if (city.value !== val) city.value = val || ''
}, { immediate: true })

// Le changement de type est appliqué immédiatement (pas de debounce)
watch(selectedTypes, (val) => {
  filterStore.updateFilters({ types: [...val] });
  emit('search');
}, { deep: true });

// Multi-select : synchronisation directe vers filterStore
watch(selectedRoomCapacity, (val) => {
  filterStore.updateFilters({ rooms: [...val] });
  emit('search');
}, { deep: true });

watch(selectedEnergyClass, (val) => {
  filterStore.updateFilters({ energyClasses: [...val] });
  emit('search');
}, { deep: true });

watch(selectedPhysicalCondition, (val) => {
  filterStore.updateFilters({ conditions: [...val] });
  emit('search');
}, { deep: true });

// requiredCriteria : multi-select natif, appliqué directement
watch(selectedCriteria, (val) => {
  filterStore.updateFilters({ requiredCriteria: [...val] });
  emit('search');
}, { deep: true });
</script>

<style scoped>
.search-container {
  display: flex;
  justify-content: center;
  padding: 1em;
  width: 100%;
}

.search-bar {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  background-color: #1e2956;
  padding: 1em;
  border-radius: 16px;
  width: 100%;
  max-width: 1750px;
  gap: 1em;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.3);
  flex-wrap: wrap;
}

.actions-group {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-left: auto;
}

.search-btn {
  background-color: #10b981 !important;
  padding: 0 2rem;
  height: 50px;
  border-radius: 12px;
  font-weight: 600;
  flex: 0;
  white-space: nowrap;
}

.search-btn:hover {
  background-color: #059969 !important;
}

.clear-all-btn {
  background: rgba(255,255,255,0.15);
  border: 1px solid rgba(255,255,255,0.3);
  color: white;
  width: 50px;
  height: 50px;
  border-radius: 12px;
  font-size: 1.4rem;
  font-weight: 700;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  transition: background 0.15s;
}

.clear-all-btn:hover {
  background: rgba(255,255,255,0.3);
}

/* ── Champ Budget — reprend le style .field.dropdown ─────────── */
.budget-field {
  cursor: pointer;
  position: relative;
  min-width: 180px;
  background: white;
  padding: 0.8rem 1.2rem;
  border-radius: 999px;
  transition: box-shadow 0.15s ease, background-color 0.15s ease;
}

.budget-field:hover {
  box-shadow: 0 0 0 2px rgba(30, 41, 86, 0.08);
}

.budget-field.active {
  box-shadow: 0 0 0 2px #1e2956;
}

.dropdown-trigger {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  width: 100%;
  background: none;
  border: none;
  padding: 0;
  cursor: pointer;
  font: inherit;
  text-align: left;
}

.label {
  font-size: 0.8rem;
  color: #94a3b8;
  white-space: nowrap;
}

.value {
  font-weight: 600;
  color: #1e2956;
  font-size: 0.9rem;
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.chevron {
  width: 16px;
  height: 16px;
  color: #94a3b8;
  flex-shrink: 0;
  transition: transform 0.2s ease;
}

.chevron.open {
  transform: rotate(180deg);
}

.popover {
  position: absolute;
  top: calc(100% + 10px);
  left: 0;
  background: white;
  padding: 1rem;
  border-radius: 16px;
  box-shadow: 0 10px 25px rgba(0,0,0,0.15);
  z-index: 1000;
}

.popover--budget {
  width: 340px;
  display: flex;
  flex-direction: column;
  gap: 0;
}

.popover-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 1rem;
  padding-top: 0.75rem;
  border-top: 1px solid #e2e8f0;
}

.link-btn {
  background: none;
  border: none;
  color: #64748b;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  padding: 0.4rem;
}

.link-btn:hover {
  color: #1e2956;
}

.apply-btn {
  background-color: #1e2956;
  color: white;
  border: none;
  border-radius: 8px;
  padding: 0.5rem 1rem;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.15s ease;
}

.apply-btn:hover {
  background-color: #3b4a8a;
}

.popover-enter-active,
.popover-leave-active {
  transition: opacity 0.15s ease, transform 0.15s ease;
}
.popover-enter-from,
.popover-leave-to {
  opacity: 0;
  transform: translateY(-6px);
}
</style>