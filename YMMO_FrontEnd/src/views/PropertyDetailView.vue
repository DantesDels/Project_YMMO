<template>
  <div class="detail-page">
    <div v-if="loading" class="loading">Chargement...</div>

    <div v-else-if="!property" class="loading">Bien introuvable.</div>

    <template v-else>
      <div class="back-bar">
        <button class="back-btn" @click="$router.back()">← Retour</button>
      </div>

      <div class="detail-layout">
        <div class="gallery-section">
          <div class="main-image">
            <img :src="currentImage" :alt="property.title" />
            <button v-if="property.pictures?.length > 1" class="chevron chevron-left" @click="prevPhoto" aria-label="Photo précédente">‹</button>
            <button v-if="property.pictures?.length > 1" class="chevron chevron-right" @click="nextPhoto" aria-label="Photo suivante">›</button>
            <span v-if="property.pictures?.length > 1" class="photo-counter">{{ activeImage + 1 }} / {{ property.pictures.length }}</span>
          </div>
          <div class="thumbnails" v-if="property.pictures?.length > 1">
            <button
              v-for="(pic, i) in property.pictures"
              :key="i"
              :class="['thumb', { active: activeImage === i }]"
              @click="activeImage = i"
            >
              <img :src="pic.url" :alt="`Photo ${i + 1}`" />
            </button>
          </div>
        </div>

        <div class="info-section">
          <div class="info-header">
            <h1>{{ property.title }}</h1>
            <span class="price">{{ formatPrice(property.price) }}</span>
          </div>

          <div class="meta-tags">
            <span class="tag">{{ translateType(property.type) }}</span>
            <span class="tag">{{ property.surface }} m²</span>
            <span v-if="property.rooms" class="tag">{{ property.rooms }} pièces</span>
            <span class="tag">{{ translateCondition(property.condition) }}</span>
            <span v-if="property.energyClass" :class="['tag', 'energy', `e-${property.energyClass.toLowerCase()}`]">
              DPE {{ property.energyClass }}
            </span>
          </div>

          <div class="location-badge">
            <span class="loc-icon">📍</span>
            <span class="loc-text">{{ locationCity }} <span class="loc-code">{{ locationPostal }}</span></span>
          </div>

          <div class="section-block">
            <h3>Description</h3>
            <p class="description">{{ property.description || 'Aucune description disponible.' }}</p>
          </div>

          <div v-if="property.mainFeatures?.length" class="section-block">
            <h3>Caractéristiques</h3>
            <div class="features-grid">
              <span v-for="f in property.mainFeatures" :key="f" class="feature-item">
                {{ translateCriteria(f) }}
              </span>
            </div>
          </div>

          <div class="section-block detail-grid">
            <div class="detail-item">
              <span class="detail-label">Année de construction</span>
              <span class="detail-value">{{ property.yearBuilt || 'N/R' }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">Meuble</span>
              <span class="detail-value">{{ property.furnishing || 'N/R' }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">Disponibilité</span>
              <span class="detail-value">{{ property.availabilityDate || 'Immédiate' }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">Référence</span>
              <span class="detail-value">{{ property.id?.slice(0, 8) || '—' }}</span>
            </div>
          </div>

          <div class="section-block">
            <h3>Localisation</h3>
            <div class="map-container">
              <iframe
                :src="mapSrc"
                width="100%"
                height="300"
                style="border:0; border-radius: 12px;"
                allowfullscreen
                loading="lazy"
                referrerpolicy="no-referrer-when-downgrade"
                title="Carte du bien"
              ></iframe>
            </div>
          </div>

          <div class="agent-card" v-if="agent">
            <div class="agent-avatar">{{ agent.name.charAt(0) }}</div>
            <div class="agent-info">
              <strong>{{ agent.name }}</strong>
              <span>{{ agent.agencyName }}</span>
            </div>
            <button class="contact-btn" @click="showContactPopup = true">Contacter</button>
          </div>
        </div>
      </div>

      <Teleport to="body">
        <div v-if="showContactPopup" class="popup-overlay" @click.self="showContactPopup = false">
          <div class="popup-card">
            <button class="popup-close" @click="showContactPopup = false">×</button>
            <h3>Contacter l'agent</h3>

            <div class="popup-agent">
              <div class="popup-avatar">{{ agent?.name?.charAt(0) }}</div>
              <div>
                <strong>{{ agent?.name }}</strong>
                <span class="popup-agency">{{ agent?.agencyName }}</span>
              </div>
            </div>

            <div class="popup-details">
              <div class="popup-row">
                <span class="popup-label">Email</span>
                <a :href="`mailto:${agent?.email}`">{{ agent?.email }}</a>
              </div>
              <div class="popup-row">
                <span class="popup-label">Téléphone</span>
                <a :href="`tel:${agent?.phone}`">{{ agent?.phone }}</a>
              </div>
            </div>

            <div class="popup-schedule">
              <h4>Horaires d'ouverture</h4>
              <div v-for="(hours, day) in agent?.schedule" :key="day" class="schedule-row">
                <span class="schedule-day">{{ day }}</span>
                <span class="schedule-hours" :class="{ closed: hours === 'Fermé' }">{{ hours }}</span>
              </div>
            </div>
          </div>
        </div>
      </Teleport>
    </template>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { getMockAgentById } from '@/utils/mockData'
import { useRoute } from 'vue-router'
import { getMockPropertyById } from '@/utils/mockData'

const route = useRoute()
const property = ref(null)
const loading = ref(true)
const activeImage = ref(0)
const showContactPopup = ref(false)

const agent = computed(() => {
  if (!property.value?.agentId) return null
  return getMockAgentById(property.value.agentId)
})

const currentImage = computed(() => {
  if (!property.value?.pictures?.length) return property.value?.image || ''
  return property.value.pictures[activeImage.value].url
})

const mapSrc = computed(() => {
  if (!property.value) return ''
  const { latitude, longitude } = property.value
  if (!latitude || !longitude) return ''
  const d = 0.08
  return `https://www.openstreetmap.org/export/embed.html?bbox=${longitude - d}%2C${latitude - d}%2C${longitude + d}%2C${latitude + d}&layer=mapnik`
})

const locationCity = computed(() => {
  if (!property.value?.address) return ''
  return property.value.address.replace(/\(.*\)/, '').trim()
})

const locationPostal = computed(() => {
  const m = property.value?.address?.match(/\((\d+)\)/)
  return m ? m[1] : ''
})

function prevPhoto() {
  if (!property.value?.pictures?.length) return
  activeImage.value = (activeImage.value - 1 + property.value.pictures.length) % property.value.pictures.length
}

function nextPhoto() {
  if (!property.value?.pictures?.length) return
  activeImage.value = (activeImage.value + 1) % property.value.pictures.length
}

function formatPrice(val) {
  return new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(val || 0)
}

const TYPE_LABELS = {
  House: 'Maison', Apartment: 'Appartement', Land: 'Terrain',
  Commercial: 'Local commercial', Office: 'Bureau', Garage: 'Garage', Parking: 'Parking',
}

const translateType = (key) => TYPE_LABELS[key] || key

const CRITERIA_LABELS = {
  Balcony: 'Balcon', Terrace: 'Terrasse', Garden: 'Jardin', Garage: 'Garage fermé',
  Parking: 'Parking', Cellar: 'Cave', SwimmingPool: 'Piscine',
  Elevator: 'Ascenseur', AirConditioning: 'Climatisation', FiberOptic: 'Fibre optique',
  SmartHome: 'Domotique', Studio: 'Studio',
}

const translateCriteria = (key) => CRITERIA_LABELS[key] || key

const CONDITION_LABELS = {
  New: 'Neuf', Excellent: 'Excellent état', Good: 'Bon état',
  NeedsRefresh: 'À rafraîchir', NeedsRenovation: 'À rénover', Ruin: 'Ruine',
}

const translateCondition = (key) => CONDITION_LABELS[key] || key

onMounted(() => {
  const id = route.params.id
  property.value = getMockPropertyById(id)
  loading.value = false
})
</script>

<style scoped>
.detail-page {
  max-width: 1200px;
  margin: 0 auto;
  padding: 1.5rem 1rem 3rem;
}

.loading {
  text-align: center;
  padding: 4rem;
  color: #64748b;
  font-size: 1.1rem;
}

.back-bar {
  margin-bottom: 1rem;
}

.back-btn {
  background: none;
  border: 1px solid #e2e8f0;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  font-size: 0.9rem;
  font-weight: 600;
  color: #475569;
  cursor: pointer;
  transition: all 0.15s;
}

.back-btn:hover {
  background: #f1f5f9;
  color: #1e2956;
}

.detail-layout {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
}

@media (max-width: 900px) {
  .detail-layout { grid-template-columns: 1fr; }
}

.gallery-section {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.main-image {
  border-radius: 16px;
  overflow: hidden;
  background: #f1f5f9;
  aspect-ratio: 4 / 3;
}

.main-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.main-image { position: relative; }

.chevron {
  position: absolute;
  top: 50%;
  translate: 0 -50%;
  background: rgba(0,0,0,0.45);
  color: white;
  border: none;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  font-size: 1.5rem;
  line-height: 1;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.15s;
  z-index: 2;
}

.chevron:hover { background: rgba(0,0,0,0.7); }

.chevron-left  { left: 0.75rem; }
.chevron-right { right: 0.75rem; }

.photo-counter {
  position: absolute;
  bottom: 0.75rem;
  right: 0.75rem;
  background: rgba(0,0,0,0.55);
  color: white;
  font-size: 0.8rem;
  font-weight: 600;
  padding: 0.25rem 0.6rem;
  border-radius: 999px;
  z-index: 2;
}

.thumbnails {
  display: flex;
  gap: 0.5rem;
  overflow-x: auto;
}

.thumb {
  flex-shrink: 0;
  width: 72px;
  height: 54px;
  border: 2px solid transparent;
  border-radius: 8px;
  overflow: hidden;
  cursor: pointer;
  padding: 0;
  background: #f1f5f9;
  transition: border-color 0.15s;
}

.thumb.active {
  border-color: #1e2956;
}

.thumb img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.info-section {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.info-header h1 {
  font-size: 1.75rem;
  font-weight: 800;
  color: #1e2956;
  margin: 0 0 0.25rem;
}

.price {
  font-size: 1.5rem;
  font-weight: 800;
  color: #10b981;
}

.meta-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.tag {
  background: #f1f5f9;
  color: #475569;
  padding: 0.3rem 0.75rem;
  border-radius: 999px;
  font-size: 0.85rem;
  font-weight: 600;
}

.tag.energy {
  color: white;
}

.e-a { background: #16a34a; }
.e-b { background: #4ade80; color: #1e2956; }
.e-c { background: #a3e635; color: #1e2956; }
.e-d { background: #facc15; color: #1e2956; }
.e-e { background: #fb923c; }
.e-f { background: #f87171; }
.e-g { background: #dc2626; }
.e-ex { background: #94a3b8; }

.location-badge {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: #f0fdf4;
  border: 1px solid #bbf7d0;
  border-radius: 12px;
  padding: 0.75rem 1rem;
}

.loc-icon { font-size: 1.25rem; }

.loc-text {
  font-size: 1.1rem;
  font-weight: 700;
  color: #166534;
}

.loc-code {
  font-weight: 500;
  color: #4ade80;
  background: rgba(74,222,128,0.2);
  padding: 0.1rem 0.45rem;
  border-radius: 6px;
  font-size: 0.85rem;
}

.section-block h3 {
  font-size: 1.1rem;
  font-weight: 700;
  color: #1e2956;
  margin: 0 0 0.5rem;
}

.description {
  color: #475569;
  line-height: 1.7;
  font-size: 0.95rem;
  margin: 0;
}

.features-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.feature-item {
  background: #eef2ff;
  color: #4338ca;
  padding: 0.3rem 0.75rem;
  border-radius: 8px;
  font-size: 0.85rem;
  font-weight: 500;
}

.detail-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.75rem;
}

.detail-item {
  background: #f8fafc;
  padding: 0.75rem;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
}

.detail-label {
  display: block;
  font-size: 0.75rem;
  color: #94a3b8;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.detail-value {
  font-size: 0.95rem;
  font-weight: 600;
  color: #1e2956;
}

.map-container {
  border-radius: 12px;
  overflow: hidden;
}

.agent-card {
  display: flex;
  align-items: center;
  gap: 1rem;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 1rem;
  margin-top: 0.5rem;
}

.agent-avatar {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  background: #1e2956;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1.2rem;
  flex-shrink: 0;
}

.agent-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
}

.agent-info strong {
  color: #1e2956;
  font-size: 0.95rem;
}

.agent-info span {
  color: #64748b;
  font-size: 0.85rem;
}

.contact-btn {
  background: #10b981;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.85rem;
  cursor: pointer;
  transition: background 0.15s;
}

.contact-btn:hover {
  background: #059669;
}

.contact-btn { position: relative; z-index: 1; }

/* ── Popup ──────────────────────────────────────────────────── */
.popup-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10000;
  padding: 1rem;
}

.popup-card {
  background: white;
  border-radius: 20px;
  padding: 2rem;
  width: 100%;
  max-width: 460px;
  position: relative;
  box-shadow: 0 20px 60px rgba(0,0,0,0.2);
}

.popup-close {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  background: #f1f5f9;
  border: none;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  font-size: 1.2rem;
  line-height: 1;
  color: #64748b;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.15s;
}

.popup-close:hover { background: #e2e8f0; color: #1e2956; }

.popup-card h3 {
  margin: 0 0 1.25rem;
  font-size: 1.15rem;
  font-weight: 800;
  color: #1e2956;
}

.popup-agent {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1.25rem;
  padding-bottom: 1.25rem;
  border-bottom: 1px solid #e2e8f0;
}

.popup-avatar {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  background: #1e2956;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1.2rem;
  flex-shrink: 0;
}

.popup-agent strong {
  display: block;
  color: #1e2956;
  font-size: 0.95rem;
}

.popup-agency {
  color: #64748b;
  font-size: 0.85rem;
}

.popup-details {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-bottom: 1.25rem;
}

.popup-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.popup-label {
  font-size: 0.8rem;
  font-weight: 700;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  min-width: 80px;
}

.popup-row a {
  color: #1e2956;
  font-weight: 600;
  text-decoration: none;
  font-size: 0.9rem;
}

.popup-row a:hover { text-decoration: underline; }

.popup-schedule h4 {
  font-size: 0.9rem;
  font-weight: 700;
  color: #1e2956;
  margin: 0 0 0.5rem;
}

.schedule-row {
  display: flex;
  justify-content: space-between;
  padding: 0.35rem 0;
  font-size: 0.85rem;
  border-bottom: 1px solid #f1f5f9;
}

.schedule-row:last-child { border-bottom: none; }

.schedule-day {
  font-weight: 600;
  color: #475569;
}

.schedule-hours {
  color: #64748b;
}

.schedule-hours.closed {
  color: #ef4444;
  font-weight: 600;
}
</style>
