<template>
  <router-link :to="`/property/${property.id}`" class="property-card-link">
    <div class="property-card">
      <div class="image-container">
        <img :src="property.image || 'https://via.placeholder.com/300'" :alt="property.title" loading="lazy" />
        <button class="fav-btn" :class="{ active: isFav }" @click.stop="toggleFav" :aria-label="isFav ? 'Retirer des favoris' : 'Ajouter aux favoris'">
          <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z" />
          </svg>
        </button>
        <div class="badges-top">
          <span v-if="property.type" class="badge type-badge">{{ translateType(property.type) }}</span>
          <span v-if="property.isPromotion" class="badge promo">Promotion en cours</span>
        </div>
        <div v-if="property.energyClass" class="badges-bottom">
          <span class="badge energy" :class="`energy-${property.energyClass.toLowerCase()}`">
            {{ property.energyClass }}
          </span>
        </div>
      </div>

      <div class="content">
        <div class="title-row">
          <h3 class="title">{{ property.title || 'Logement sans titre' }}</h3>
          <span v-if="property.condition" class="condition-badge">{{ translateCondition(property.condition) }}</span>
        </div>

        <p class="location">{{ property.address || 'Adresse non spécifiée' }}</p>
        <p class="specs">
          {{ property.surface || 0 }}m²<span v-if="property.rooms"> · {{ property.rooms }} pièces</span><span v-if="property.furnishing"> · {{ property.furnishing }}</span>
        </p>

        <div v-if="displayCriteria.length > 0" class="criteria-list">
          <span v-for="c in displayCriteria" :key="c" class="criteria-badge">
            {{ translateCriteria(c) }}
          </span>
        </div>
      </div>

      <div class="footer">
        <span class="price">
          <strong>{{ formatNumber(property.price || 0) }}€</strong>
        </span>
        <div v-if="property.availabilityDate" class="availability">
          ● Disponible {{ property.availabilityDate }}
        </div>
      </div>
    </div>
  </router-link>
</template>

<script setup>
import { computed } from 'vue';
import { formatNumber } from '@/utils/formatters';
import { useWishlistStore } from '@/stores/wishlist.store';

const props = defineProps({
  property: { type: Object, required: true }
});

const wishlist = useWishlistStore()
const isFav = computed(() => wishlist.isFavorite(props.property.id))

function toggleFav() {
  wishlist.toggleFavorite(props.property.id)
}

// Miroir de Domain/Enums/PropertyType.cs
const TYPE_LABELS = {
  House: 'Maison',
  Apartment: 'Appartement',
  Land: 'Terrain',
  Commercial: 'Local commercial',
  Office: 'Bureau',
  Garage: 'Garage',
  Parking: 'Parking',
};

const ENERGY_LABELS = {
  A: 'A', B: 'B', C: 'C', D: 'D', E: 'E', F: 'F', G: 'G',
  Ex: 'Ex'
};

// Miroir complet de Domain/Entities/Enums/Criteria.cs (27 valeurs)
const CRITERIA_LABELS = {
  Balcony: 'Balcon', Terrace: 'Terrasse', Garden: 'Jardin', Garage: 'Garage fermé',
  Parking: 'Parking', Cellar: 'Cave', SwimmingPool: 'Piscine',
  Elevator: 'Ascenseur', AirConditioning: 'Climatisation', Fireplace: 'Cheminée',
  Furnished: 'Meublé', HardwoodFloor: 'Parquet', DoubleGlazing: 'Double vitrage',
  FittedKitchen: 'Cuisine équipée', Digicode: 'Digicode', Intercom: 'Interphone',
  AlarmSystem: 'Alarme', SecurityDoor: 'Porte blindée', DisabledAccess: 'Accès PMR',
  Caretaker: 'Gardien', SeaView: 'Vue mer', MountainView: 'Vue montagne',
  UnobstructedView: 'Vue dégagée', SouthFacing: 'Exposition sud',
  FiberOptic: 'Fibre optique', SmartHome: 'Domotique', HeatPump: 'Pompe à chaleur',
  SolarPanels: 'Panneaux solaires',
};

// Miroir de Domain/Enums/PhysicalCondition.cs
const CONDITION_LABELS = {
  New: 'Neuf',
  Excellent: 'Excellent état',
  Good: 'Bon état',
  NeedsRefresh: 'À rafraîchir',
  NeedsRenovation: 'À rénover',
  Ruin: 'Ruine',
};

const translateType = (key) => TYPE_LABELS[key] || key;
const translateEnergy = (key) => ENERGY_LABELS[key] || key;
const translateCriteria = (key) => CRITERIA_LABELS[key] || key;
const translateCondition = (key) => CONDITION_LABELS[key] || key;

const displayCriteria = computed(() => {
  return props.property.mainFeatures ? props.property.mainFeatures.slice(0, 3) : [];
});
</script>

<style scoped>
.property-card-link { text-decoration: none; color: inherit; display: block; }
.property-card { background: white; border-radius: 16px; overflow: hidden; border: 1px solid #e2e8f0; transition: transform 0.2s; display: flex; flex-direction: column; }
.property-card-link:hover .property-card { transform: translateY(-5px); box-shadow: 0 10px 15px -3px rgba(0,0,0,0.1); }

.image-container { height: 200px; position: relative; }
.image-container img { width: 100%; height: 100%; object-fit: cover; }

.fav-btn {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  z-index: 3;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: none;
  background: rgba(255,255,255,0.9);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.15s;
  color: #94a3b8;
  backdrop-filter: blur(4px);
}

.fav-btn:hover {
  background: white;
  color: #ef4444;
  transform: scale(1.1);
}

.fav-btn.active {
  color: #ef4444;
  background: white;
}

.fav-btn.active svg {
  fill: #ef4444;
}

.badges-top {
  position: absolute;
  top: 0.75rem;
  left: 0.75rem;
  right: 0.75rem;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  pointer-events: none;
}

.badges-bottom {
  position: absolute;
  bottom: 0.75rem;
  right: 0.75rem;
  display: flex;
  z-index: 2;
}

.badge.promo {
  background: #ef4444;
  color: white;
  padding: 4px 10px;
  border-radius: 6px;
  font-size: 0.75rem;
  font-weight: 600;
}

.badge.type-badge {
  background: rgba(30, 41, 86, 0.85);
  color: white;
  padding: 5px 12px;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 600;
  backdrop-filter: blur(2px);
}

.badge.energy {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  font-size: 1.25rem;
  font-weight: 800;
  color: white;
  box-shadow: 0 2px 6px rgba(0,0,0,0.2);
}
.energy-a { background: #16a34a; }
.energy-b { background: #4ade80; color: #1e2956; }
.energy-c { background: #a3e635; color: #1e2956; }
.energy-d { background: #facc15; color: #1e2956; }
.energy-e { background: #fb923c; }
.energy-f { background: #f87171; }
.energy-g { background: #dc2626; }
.energy-exempt { background: #94a3b8; }

.content { padding: 0.5rem 1.25rem; flex-grow: 1; display: flex; flex-direction: column; gap: 0.05rem ; }

.title-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.title { font-weight: 700; color: #1e2956; font-size: 1.6rem; }
.condition-badge {
  flex-shrink: 0;
  background: #eef2ff;
  color: #4338ca;
  padding: 4px 12px;
  border-radius: 999px;
  font-size: 0.8rem;
  font-weight: 700;
  white-space: nowrap;
}

.location, .specs { 
  font-size: 1rem; 
  color: #64748b;  
  line-height: 2;
  margin: 0; 
}

.criteria-list { 
  display: flex; 
  gap: 8px; 
  flex-wrap: wrap; 
}

.criteria-badge {
  background: #f1f5f9;
  color: #475569;
  padding: 2px 8px;
  border-radius: 4px;
  font-size:1rem;
  font-weight: 500;
  margin-top: 0.3rem;
  margin-bottom: 0.5rem;
}

.footer {
  background-color: #2d3a6e;
  padding: 0.9rem 1.25rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
}
.price { font-size: 0.9rem; color: #cbd5e1; }
.price strong { color: white; font-size: 1.15rem; font-weight: 700; }
.availability { font-size: 0.8rem; color: #6ee7b7; white-space: nowrap; }
</style>