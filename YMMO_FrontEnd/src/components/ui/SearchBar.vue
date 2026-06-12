<template>
  <div class="search-container" v-click-outside="closeMenu">
    <div class="search-bar">
      <div class="field location">
        <label>Localisation</label>
        <input type="text" placeholder="Ville, quartier..." v-model="filters.city" @click.stop />
      </div>

      <div class="field" @click="toggleMenu('type')">
        <label>Type</label>
        <span class="value truncated">{{ displayTypes }}</span>
        <div v-if="activeMenu === 'type'" class="popover" @click.stop>
          <label v-for="(label, key) in propertyTypeMap" :key="key" class="checkbox-item">
            <input type="checkbox" :value="key" v-model="filters.types" /> {{ label }}
          </label>
        </div>
      </div>

      <div class="field" @click="toggleMenu('price')">
        <label>Budget</label>
        <span class="value">{{ formatNumber(filters.minPrice) }}€ - {{ formatNumber(filters.maxPrice) }}€</span>
        <div v-if="activeMenu === 'price'" class="popover" @click.stop>
          <div class="range-inputs">
            <div class="input-wrapper">
              <label class="input-label">Budget min</label>
              <div class="input-with-symbol">
                <input type="text" :value="formatNumber(filters.minPrice)" @input="e => updateMin(e.target.value)" />
                <span>€</span>
              </div>
            </div>
            <div class="input-wrapper">
              <label class="input-label">Budget max</label>
              <div class="input-with-symbol">
                <input type="text" :value="formatNumber(filters.maxPrice)" @input="e => updateMax(e.target.value)" />
                <span>€</span>
              </div>
            </div>
          </div>
          <div class="slider-container">
            <div class="slider-track"></div>
            <input type="range" :value="filters.minPrice" @input="e => updateMin(e.target.value)" min="0" max="5000000" step="1000" />
            <input type="range" :value="filters.maxPrice" @input="e => updateMax(e.target.value)" min="0" max="5000000" step="1000" />
          </div>
        </div>
      </div>

      <div class="field" @click="toggleMenu('criteria')">
        <label>Critères</label>
        <span class="value">{{ filters.requiredCriteria.length }} sélectionnés</span>
        <div v-if="activeMenu === 'criteria'" class="popover scrollable" @click.stop>
          <template v-for="(group, key) in criteriaGroups" :key="key">
            <div class="group-title">{{ key }}</div>
            <label v-for="c in group" :key="c" class="checkbox-item">
              <input type="checkbox" :value="c" v-model="filters.requiredCriteria" />
              {{ translateCriteria(c) }}
            </label>
            <div class="separator"></div>
          </template>
        </div>
      </div>

      <AppButton @click="onSearch" class="search-btn">Rechercher</AppButton>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue';
import AppButton from '@/components/ui/AppButton.vue';
import { formatNumber, parseNumber } from '@/utils/formatters';

const activeMenu = ref(null);
const closeMenu = () => activeMenu.value = null;

const vClickOutside = {
  mounted(el, binding) {
    el.clickOutsideEvent = (event) => {
      if (!(el === event.target || el.contains(event.target))) binding.value();
    };
    document.body.addEventListener('click', el.clickOutsideEvent);
  },
  unmounted(el) { document.body.removeEventListener('click', el.clickOutsideEvent); }
};

const filters = reactive({ city: '', types: [], minPrice: 200000, maxPrice: 5000000, requiredCriteria: [] });

const displayTypes = computed(() => {
  if (filters.types.length === 0) return 'Tous';
  const labels = filters.types.map(t => propertyTypeMap[t]);
  return labels.length > 3 ? labels.slice(0, 3).join(', ') + '...' : labels.join(', ');
});

const updateMin = (val) => { const p = parseNumber(val); if (p <= filters.maxPrice) filters.minPrice = p; };
const updateMax = (val) => { const p = parseNumber(val); if (p >= filters.minPrice) filters.maxPrice = p; };
const toggleMenu = (menu) => activeMenu.value = (activeMenu.value === menu ? null : menu);
const onSearch = () => console.log('Recherche :', filters);

const propertyTypeMap = { 'House': 'Maison', 'Apartment': 'Appart', 'Land': 'Terrain', 'Commercial': 'Local', 'Office': 'Bureau', 'Garage': 'Box', 'Parking': 'Parking' };
const criteriaGroups = { 'Typologie': ['Studio', 'T2', 'T3', 'T4', 'T5Plus'], 'Extérieurs': ['Balcony', 'Terrace', 'Garden', 'Garage', 'Parking', 'Cellar', 'SwimmingPool'], 'Confort': ['Elevator', 'AirConditioning', 'Fireplace', 'Furnished', 'HardwoodFloor', 'DoubleGlazing', 'FittedKitchen'], 'Sécurité': ['Digicode', 'Intercom', 'AlarmSystem', 'SecurityDoor', 'DisabledAccess', 'Caretaker'], 'Vues': ['SeaView', 'MountainView', 'UnobstructedView', 'SouthFacing'], 'Tech & Énergie': ['FiberOptic', 'SmartHome', 'HeatPump', 'SolarPanels'] };
const translateCriteria = (key) => ({ 'Balcony': 'Balcon', 'Terrace': 'Terrasse', 'Garden': 'Jardin', 'Garage': 'Garage', 'Parking': 'Parking', 'Cellar': 'Cave', 'SwimmingPool': 'Piscine', 'Elevator': 'Ascenseur', 'AirConditioning': 'Climatisation', 'Fireplace': 'Cheminée', 'Furnished': 'Meublé', 'HardwoodFloor': 'Parquet', 'DoubleGlazing': 'Double vitrage', 'FittedKitchen': 'Cuisine équipée', 'Digicode': 'Digicode', 'Intercom': 'Interphone', 'AlarmSystem': 'Alarme', 'SecurityDoor': 'Porte blindée', 'DisabledAccess': 'Accès PMR', 'Caretaker': 'Gardien', 'SeaView': 'Vue mer', 'MountainView': 'Vue montagne', 'UnobstructedView': 'Vue dégagée', 'SouthFacing': 'Plein sud', 'FiberOptic': 'Fibre optique', 'SmartHome': 'Domotique', 'HeatPump': 'Pompe à chaleur', 'SolarPanels': 'Panneaux solaires', 'Studio': 'Studio', 'T2': 'T2', 'T3': 'T3', 'T4': 'T4', 'T5Plus': 'T5+' }[key] || key);
</script>

<style scoped>
.search-container { padding: 0.5em; position: relative; z-index: 100; }
.search-bar { display: flex; background: white; padding: 0.5rem; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.05), 0 0 0 1px #e2e8f0; height: 70px; align-items: center; }
.field { padding: 0 1rem; border-right: 1px solid #e2e8f0; display: flex; flex-direction: column; justify-content: center; position: relative; min-width: 120px; cursor: pointer; height: 100%; }
.value { white-space: nowrap; overflow: hidden; text-overflow: ellipsis; font-weight: 500; color: #1e2956; font-size: 0.9rem; }
.truncated { max-width: 140px; }
.popover { position: absolute; top: 80px; left: 0; background: white; padding: 1.25rem; border-radius: 12px; box-shadow: 0 10px 25px rgba(0,0,0,0.15); width: 280px; max-height: 400px; z-index: 999; overflow-y: auto; cursor: default; }
.checkbox-item { display: flex; align-items: center; gap: 0.75rem; padding: 0.5rem 0; cursor: pointer; }
.group-title { font-size: 0.75rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-top: 1rem; margin-bottom: 0.5rem; }
.separator { height: 1px; background: #e2e8f0; margin: 0.5rem 0; }
.search-btn { margin-left: auto; border-radius: 999px !important; height: 50px; padding: 0 2rem !important; background: #10b981 !important; z-index: 100; }
.input-with-symbol { display: flex; align-items: center; border: 1px solid #e2e8f0; border-radius: 8px; padding: 0.5rem; background: #f8fafc; }
.slider-container { position: relative; height: 40px; margin-top: 1rem; }
.slider-track { position: absolute; top: 50%; transform: translateY(-50%); width: 100%; height: 6px; background: #e2e8f0; border-radius: 3px; z-index: 1; }
.slider-container input { position: absolute; width: 100%; background: none; appearance: none; top: 10px; z-index: 2; pointer-events: none; }
input[type=range]::-webkit-slider-thumb { pointer-events: auto; appearance: none; height: 18px; width: 18px; border-radius: 50%; background: #10b981; cursor: pointer; }
</style>