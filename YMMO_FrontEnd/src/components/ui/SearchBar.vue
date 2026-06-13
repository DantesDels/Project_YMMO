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

      <AppButton class="search-btn" @click="onSearch">
        Rechercher
      </AppButton>

    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';
import AppButton from '@/components/ui/AppButton.vue';
import SearchField from '@/components/ui/SearchField.vue';
import SearchDropdown from '@/components/ui/SearchDropdown.vue';
import SearchDropdownExtended from '@/components/ui/SearchDropdownExtended.vue';
import { useFilterStore } from '@/stores/filterStore';

const filterStore = useFilterStore();
const emit = defineEmits(['search']);

// ── État local — synchronisé vers le filterStore ──────────────
const city = ref(filterStore.filters.city);
const selectedTypes = ref([...filterStore.filters.types]);

// rooms est un number? singulier dans filterStore → on le wrappe
// dans un tableau pour SearchDropdown, on ne garde que le 1er élément
const selectedRoomCapacity = ref(
    filterStore.filters.rooms ? [String(filterStore.filters.rooms)] : []
);

// condition / energyClass sont des string singuliers → idem
const selectedEnergyClass = ref(
    filterStore.filters.energyClass ? [filterStore.filters.energyClass] : []
);
const selectedPhysicalCondition = ref(
    filterStore.filters.condition ? [filterStore.filters.condition] : []
);

// requiredCriteria est déjà un tableau → multi-select natif
const selectedCriteria = ref([...filterStore.filters.requiredCriteria]);

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

// ── Soumission ─────────────────────────────────────────────────
const onSearch = () => {
  filterStore.updateFilters({
    city: city.value,
    types: selectedTypes.value,
  });
  emit('search');
};

// ── Watchers : synchronisation vers filterStore ─────────────────

// Recherche "live" sur la ville : debounce léger pour ne pas
// spammer le filtre à chaque frappe
let debounceTimer;
watch(city, (val) => {
  clearTimeout(debounceTimer);
  debounceTimer = setTimeout(() => {
    filterStore.updateFilters({ city: val });
    emit('search');
  }, 300);
});

// Le changement de type est appliqué immédiatement (pas de debounce)
watch(selectedTypes, (val) => {
  filterStore.updateFilters({ types: [...val] });
  emit('search');
}, { deep: true });

// rooms : on ne garde que le premier élément (single-select déguisé)
watch(selectedRoomCapacity, (val) => {
  // Si plusieurs valeurs sont cochées d'un coup, ne garder que la dernière
  if (val.length > 1) {
    selectedRoomCapacity.value = [val[val.length - 1]];
    return; // le watcher se redéclenche avec la valeur unique
  }
  filterStore.updateFilters({ rooms: val[0] ? Number(val[0]) : 0 });
  emit('search');
}, { deep: true });

// energyClass : single-select déguisé
watch(selectedEnergyClass, (val) => {
  if (val.length > 1) {
    selectedEnergyClass.value = [val[val.length - 1]];
    return;
  }
  filterStore.updateFilters({ energyClass: val[0] || '' });
  emit('search');
}, { deep: true });

// condition : single-select déguisé
watch(selectedPhysicalCondition, (val) => {
  if (val.length > 1) {
    selectedPhysicalCondition.value = [val[val.length - 1]];
    return;
  }
  filterStore.updateFilters({ condition: val[0] || '' });
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
  max-width: 1100px;
  gap: 1em;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.3);
  flex-wrap: wrap;
}

.search-btn {
  margin-left: auto;
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
</style>