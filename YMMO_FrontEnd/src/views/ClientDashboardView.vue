<template>
  <div class="dashboard">
    <aside class="sidebar">
      <h2 class="sidebar-title">Mon compte</h2>
      <nav class="sidebar-nav">
        <button
          v-for="item in navItems"
          :key="item.id"
          :class="['nav-btn', { active: activeSection === item.id }]"
          @click="switchSection(item.id)"
        >
          {{ item.label }}
        </button>
      </nav>

      <div class="sidebar-footer">
        <router-link to="/" class="btn-public-site">← Site public</router-link>
        <button class="logout-btn" @click="handleLogout">Déconnexion</button>
      </div>
    </aside>

    <main class="main">
      <!-- ─── Profil ─── -->
      <section v-if="activeSection === 'profile'" class="section-card">
        <div class="section-header">
          <h3 class="section-title">Mon profil</h3>
          <button v-if="!editing" class="btn-edit" @click="startEditing">Modifier</button>
          <div v-else class="edit-actions">
            <button class="btn-save" @click="saveProfile" :disabled="saving">{{ saving ? 'Enregistrement…' : 'Enregistrer' }}</button>
            <button class="btn-cancel" @click="cancelEditing">Annuler</button>
          </div>
        </div>

        <div class="profile-avatar-section">
          <div class="avatar-large">{{ avatarLetter }}</div>
          <div class="profile-summary">
            <p class="profile-name">{{ form.firstName }} {{ form.lastName }}</p>
            <p class="profile-username">@{{ form.username }}</p>
            <p class="profile-role">{{ roleLabel }}</p>
          </div>
        </div>

        <div class="profile-form">
          <div class="form-row">
            <div class="form-group">
              <label>Prénom</label>
              <input v-model="form.firstName" :disabled="!editing" placeholder="Votre prénom" />
            </div>
            <div class="form-group">
              <label>Nom</label>
              <input v-model="form.lastName" :disabled="!editing" placeholder="Votre nom" />
            </div>
          </div>
          <div class="form-row">
            <div class="form-group">
              <label>Email</label>
              <input v-model="form.email" :disabled="!editing" type="email" placeholder="email@exemple.fr" />
            </div>
            <div class="form-group">
              <label>Téléphone</label>
              <input v-model="form.phone" :disabled="!editing" placeholder="06 12 34 56 78" />
            </div>
          </div>
          <div class="form-group">
            <label>Adresse</label>
            <input v-model="form.address" :disabled="!editing" placeholder="Votre adresse" />
          </div>
          <div class="form-group">
            <label>Code postal</label>
            <input v-model="form.zipCode" :disabled="!editing" placeholder="75001" maxlength="5" />
          </div>
          <div class="form-group">
            <label>Ville</label>
            <input v-model="form.city" :disabled="!editing" placeholder="Paris" />
          </div>
          <div class="form-group">
            <label>À propos de moi</label>
            <textarea v-model="form.bio" :disabled="!editing" rows="4" placeholder="Parlez de vous aux agents…"></textarea>
          </div>
        </div>

        <div v-if="saved" class="toast-success">Profil mis à jour avec succès.</div>
      </section>

      <!-- ─── Vendre ─── -->
      <section v-if="activeSection === 'sell'" class="section-card">
        <h3 class="section-title">Mettre un bien en vente</h3>
        <p class="section-desc">
          Décrivez votre bien en détail et joignez des photos. Votre demande sera étudiée par nos agents.
        </p>

        <form class="sell-form" @submit.prevent="submitSellRequest">
          <div class="form-group">
            <label>Titre de l'annonce *</label>
            <input v-model="sellForm.title" placeholder="Ex : Appartement 3 pièces centre-ville" required />
          </div>
          <div class="form-row">
            <div class="form-group">
              <label>Type *</label>
              <select v-model="sellForm.type" required>
                <option value="">Sélectionnez…</option>
                <option value="Appartement">Appartement</option>
                <option value="Maison">Maison</option>
                <option value="Villa">Villa</option>
                <option value="Terrain">Terrain</option>
                <option value="Local commercial">Local commercial</option>
              </select>
            </div>
            <div class="form-group">
              <label>Surface (m²) *</label>
              <input v-model.number="sellForm.surface" type="number" min="1" placeholder="75" required />
            </div>
          </div>
          <div class="form-row">
            <div class="form-group">
              <label>Pièces</label>
              <input v-model.number="sellForm.rooms" type="number" min="1" placeholder="3" />
            </div>
            <div class="form-group">
              <label>Prix souhaité (€) *</label>
              <input v-model.number="sellForm.price" type="number" min="1" placeholder="250000" required />
            </div>
          </div>
          <div class="form-group">
            <label>Adresse du bien *</label>
            <input v-model="sellForm.address" placeholder="Numéro et rue" required />
          </div>
          <div class="form-row">
            <div class="form-group">
              <label>Code postal *</label>
              <input v-model="sellForm.zipCode" placeholder="75001" maxlength="5" required />
            </div>
            <div class="form-group">
              <label>Ville *</label>
              <input v-model="sellForm.city" placeholder="Paris" required />
            </div>
          </div>
          <div class="form-group">
            <label>Description détaillée</label>
            <textarea v-model="sellForm.description" rows="5" placeholder="Décrivez le bien, son état, ses atouts…"></textarea>
          </div>

          <div class="form-group">
            <label>Photos du bien</label>
            <div class="photo-upload" @click="triggerFileInput">
              <div v-if="sellForm.photos.length === 0" class="upload-placeholder">
                <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="#94a3b8" stroke-width="1.5">
                  <rect x="3" y="3" width="18" height="18" rx="2" />
                  <circle cx="8.5" cy="8.5" r="1.5" />
                  <path d="M21 15l-5-5L5 21" />
                </svg>
                <p>Cliquez pour ajouter des photos</p>
                <p class="upload-hint">Jusqu'à 10 photos</p>
              </div>
              <div v-else class="photo-previews">
                <div v-for="(photo, idx) in sellForm.photos" :key="idx" class="photo-preview">
                  <img :src="photo.url" :alt="photo.name" />
                  <button type="button" class="photo-remove" @click.stop="removePhoto(idx)">×</button>
                </div>
                <button v-if="sellForm.photos.length < 10" type="button" class="photo-add" @click.stop="triggerFileInput">
                  <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#94a3b8" stroke-width="1.5">
                    <line x1="12" y1="5" x2="12" y2="19" />
                    <line x1="5" y1="12" x2="19" y2="12" />
                  </svg>
                </button>
              </div>
              <input ref="fileInput" type="file" accept="image/*" multiple class="file-input-hidden" @change="handleFileUpload" />
            </div>
          </div>

          <div class="sell-actions">
            <button type="submit" class="btn-submit-sell" :disabled="sellSubmitting">
              {{ sellSubmitting ? 'Envoi en cours…' : 'Soumettre ma demande aux agences' }}
            </button>
          </div>
        </form>

        <div v-if="sellSubmitted" class="success-card">
          <h3>Demande envoyée !</h3>
          <p>Votre bien a été soumis à nos agences partenaires. Un agent vous contactera sous 48h.</p>
        </div>
      </section>

      <!-- ─── Favoris ─── -->
      <section v-if="activeSection === 'wishlist'" class="section-card">
        <h3 class="section-title">Mes favoris</h3>
        <p class="text-muted" v-if="favorites.length === 0">Vous n'avez encore aucun favori. Parcourez le <router-link to="/catalog" class="link">catalogue</router-link> pour en ajouter.</p>
        <div v-else class="favorites-grid">
          <div v-for="fav in favorites" :key="fav.id" class="fav-card">
            <img :src="fav.image" :alt="fav.title" class="fav-img" />
            <div class="fav-info">
              <h4>{{ fav.title }}</h4>
              <p class="fav-price">{{ fav.price }} €</p>
              <p class="fav-city">{{ fav.city }}</p>
            </div>
            <button class="fav-remove" @click="removeFavorite(fav.id)">×</button>
          </div>
        </div>
      </section>

      <!-- ─── Mes offres ─── -->
      <section v-if="activeSection === 'offers'" class="section-card">
        <h3 class="section-title">Mes offres</h3>
        <p class="text-muted" v-if="offers.length === 0">Aucune offre en cours pour le moment.</p>
        <div v-else class="offers-list">
          <div v-for="offer in offers" :key="offer.id" class="offer-card">
            <div class="offer-header">
              <h4>{{ offer.propertyTitle }}</h4>
              <span :class="['offer-status', offer.status]">{{ statusLabel(offer.status) }}</span>
            </div>
            <div class="offer-body">
              <div class="offer-meta">
                <span>Agent : <strong>{{ offer.agentName }}</strong></span>
                <span>Agence : {{ offer.agency }}</span>
                <span>Prix proposé : <strong>{{ offer.proposedPrice }} €</strong></span>
              </div>
              <p class="offer-message" v-if="offer.message">{{ offer.message }}</p>
            </div>
            <div class="offer-footer" v-if="offer.status === 'pending'">
              <button class="btn-accept" @click="respondOffer(offer.id, 'accepted')">Accepter</button>
              <button class="btn-decline" @click="respondOffer(offer.id, 'declined')">Refuser</button>
            </div>
          </div>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'

interface SellPhoto {
  url: string
  name: string
}

interface Offer {
  id: number
  propertyTitle: string
  status: 'pending' | 'accepted' | 'declined' | 'negotiation'
  agentName: string
  agency: string
  proposedPrice: number
  message: string
}

interface Favorite {
  id: number
  title: string
  price: number
  city: string
  image: string
}

const router = useRouter()
const route = useRoute()
const authStore = useAuthentificationStore()

// ── Sections ──
const navItems = [
  { id: 'profile', label: 'Profil' },
  { id: 'sell', label: 'Vendre' },
  { id: 'wishlist', label: 'Favoris' },
  { id: 'offers', label: 'Mes offres' },
]

const activeSection = ref('profile')

function switchSection(id: string) {
  activeSection.value = id
}

// Read initial section from query param
watch(() => route.query.section, (val) => {
  if (val && navItems.some(n => n.id === val)) {
    activeSection.value = val as string
  }
})

// ── Profile ──
const editing = ref(false)
const saving = ref(false)
const saved = ref(false)

const form = reactive({
  firstName: '',
  lastName: '',
  username: '',
  email: '',
  phone: '',
  address: '',
  zipCode: '',
  city: '',
  bio: '',
})

const initialForm = reactive({ ...form })

const roleLabel = computed(() => {
  switch (authStore.user?.role) {
    case 'Client': return 'Client'
    case 'Agent': return 'Agent immobilier'
    case 'Admin': return 'Administrateur'
    default: return 'Visiteur'
  }
})

const avatarLetter = computed(() => {
  const name = form.firstName || form.username || 'U'
  return name.charAt(0).toUpperCase()
})

function loadProfile() {
  const u = authStore.user
  if (u) {
    form.firstName = u.firstName ?? ''
    form.lastName = u.lastName ?? ''
    form.username = u.username ?? ''
    form.email = u.email ?? ''
    form.phone = u.phone ?? ''
    form.address = u.address ?? ''
    form.zipCode = u.zipCode ?? ''
    form.city = u.city ?? ''
    form.bio = u.bio ?? ''
    Object.assign(initialForm, form)
  }
}

onMounted(loadProfile)

function startEditing() {
  saved.value = false
  editing.value = true
}

function cancelEditing() {
  Object.assign(form, initialForm)
  editing.value = false
  saved.value = false
}

async function saveProfile() {
  saving.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 600))
    authStore.updateProfile({ ...form })
    Object.assign(initialForm, form)
    editing.value = false
    saved.value = true
    setTimeout(() => { saved.value = false }, 3000)
  } finally {
    saving.value = false
  }
}

// ── Sell ──
const fileInput = ref<HTMLInputElement | null>(null)
const sellSubmitting = ref(false)
const sellSubmitted = ref(false)

const sellForm = reactive({
  title: '',
  type: '',
  surface: null as number | null,
  rooms: null as number | null,
  price: null as number | null,
  address: '',
  zipCode: '',
  city: '',
  description: '',
  photos: [] as SellPhoto[],
})

function triggerFileInput() {
  fileInput.value?.click()
}

function handleFileUpload(e: Event) {
  const target = e.target as HTMLInputElement
  if (!target.files) return
  const files = Array.from(target.files)
  const remaining = 10 - sellForm.photos.length
  for (const file of files.slice(0, remaining)) {
    sellForm.photos.push({
      url: URL.createObjectURL(file),
      name: file.name,
    })
  }
  target.value = ''
}

function removePhoto(idx: number) {
  sellForm.photos.splice(idx, 1)
}

async function submitSellRequest() {
  sellSubmitting.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 1200))
    sellSubmitted.value = true
    Object.assign(sellForm, {
      title: '', type: '', surface: null, rooms: null, price: null,
      address: '', zipCode: '', city: '', description: '', photos: [],
    })
  } finally {
    sellSubmitting.value = false
  }
}

// ── Favorites (mock) ──
const favorites = reactive<Favorite[]>([])

function removeFavorite(id: number) {
  const idx = favorites.findIndex(f => f.id === id)
  if (idx !== -1) favorites.splice(idx, 1)
}

// ── Offers (mock) ──
const offers = reactive<Offer[]>([])

function statusLabel(status: string) {
  const labels: Record<string, string> = {
    pending: 'En attente',
    accepted: 'Acceptée',
    declined: 'Refusée',
    negotiation: 'En négociation',
  }
  return labels[status] ?? status
}

function respondOffer(id: number, status: 'accepted' | 'declined') {
  const offer = offers.find(o => o.id === id)
  if (offer) {
    offer.status = status
  }
}

// ── Logout ──
function handleLogout() {
  authStore.logout()
  router.push('/')
}

// ── Init mock data ──
onMounted(() => {
  offers.push(
    { id: 1, propertyTitle: 'Appartement 3 pièces - Paris 11e', status: 'pending', agentName: 'Sophie Martin', agency: 'Agence du Centre', proposedPrice: 325000, message: 'Bonjour, nous avons étudié votre demande et vous proposons une estimation à 325 000 €. Contactez-nous pour visiter.' },
    { id: 2, propertyTitle: 'Studio rénové - Lyon 3e', status: 'negotiation', agentName: 'Lucas Bernard', agency: 'ImmoLyon', proposedPrice: 142000, message: 'Nous sommes intéressés. Pouvons-nous fixer un rendez-vous cette semaine ?' },
  )

  favorites.push(
    { id: 101, title: 'Appartement haussmannien', price: 489000, city: 'Paris', image: 'https://placehold.co/400x300/1e2956/ffffff?text=Paris' },
    { id: 102, title: 'Villa avec piscine', price: 725000, city: 'Nice', image: 'https://placehold.co/400x300/1e2956/ffffff?text=Nice' },
  )
})
</script>

<style scoped>
.dashboard {
  display: flex;
  min-height: calc(100vh - 150px);
}

.sidebar {
  width: 240px;
  background: white;
  border-right: 1px solid #e2e8f0;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.sidebar-title {
  font-size: 1.15rem;
  font-weight: 700;
  color: #1e2956;
  margin-bottom: 1rem;
}

.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  flex: 1;
}

.nav-btn {
  text-align: left;
  padding: 0.625rem 0.875rem;
  border: none;
  background: transparent;
  border-radius: 8px;
  font-size: 0.95rem;
  font-weight: 500;
  color: #475569;
  cursor: pointer;
  transition: all 0.15s;
}

.nav-btn:hover {
  background: #f1f5f9;
  color: #1e2956;
}

.nav-btn.active {
  background: #1e2956;
  color: white;
  font-weight: 600;
}

.sidebar-footer {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-top: auto;
}

.btn-public-site {
  padding: 0.5rem 0.875rem;
  font-size: 0.85rem;
  color: #64748b;
  text-decoration: none;
  border-radius: 8px;
  transition: all 0.15s;
}

.btn-public-site:hover {
  background: #f1f5f9;
  color: #1e2956;
}

.logout-btn {
  padding: 0.625rem;
  border: 1px solid #e2e8f0;
  background: white;
  border-radius: 8px;
  font-size: 0.9rem;
  color: #dc2626;
  cursor: pointer;
  transition: all 0.15s;
}

.logout-btn:hover {
  background: #fef2f2;
  border-color: #fca5a5;
}

.main {
  flex: 1;
  padding: 2rem;
  background: #f8fafc;
  overflow-y: auto;
}

.section-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  border: 1px solid #e2e8f0;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.25rem;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1e2956;
  margin: 0;
}

.section-desc {
  color: #64748b;
  margin-bottom: 1.5rem;
  line-height: 1.5;
}

.btn-edit {
  padding: 0.4rem 1rem;
  border: 1px solid #1e2956;
  background: transparent;
  color: #1e2956;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.15s;
}

.btn-edit:hover {
  background: #1e2956;
  color: white;
}

.edit-actions {
  display: flex;
  gap: 0.5rem;
}

.btn-save {
  padding: 0.4rem 1rem;
  background: #1e2956;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.15s;
}

.btn-save:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-save:hover:not(:disabled) {
  background: #3b4a8a;
}

.btn-cancel {
  padding: 0.4rem 1rem;
  border: 1px solid #e2e8f0;
  background: white;
  color: #64748b;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 500;
  cursor: pointer;
}

.btn-cancel:hover {
  background: #f8fafc;
}

/* Profile avatar */
.profile-avatar-section {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  padding-bottom: 1.5rem;
  margin-bottom: 1.5rem;
  border-bottom: 1px solid #e2e8f0;
}

.avatar-large {
  width: 64px;
  height: 64px;
  border-radius: 50%;
  background: #1e2956;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  font-weight: 700;
  flex-shrink: 0;
}

.profile-summary {
  display: flex;
  flex-direction: column;
  gap: 0.15rem;
}

.profile-name {
  font-size: 1.15rem;
  font-weight: 700;
  color: #1e2956;
  margin: 0;
}

.profile-username {
  font-size: 0.9rem;
  color: #64748b;
  margin: 0;
}

.profile-role {
  font-size: 0.8rem;
  color: #94a3b8;
  margin: 0;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Form */
.profile-form, .sell-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

.form-group label {
  font-size: 0.85rem;
  font-weight: 600;
  color: #475569;
}

.form-group input,
.form-group select,
.form-group textarea {
  padding: 0.6rem 0.75rem;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  font-size: 0.9rem;
  color: #1e293b;
  background: white;
  transition: border-color 0.15s;
  font-family: inherit;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #1e2956;
  box-shadow: 0 0 0 3px rgba(30, 41, 86, 0.1);
}

.form-group input:disabled,
.form-group textarea:disabled {
  background: #f8fafc;
  color: #94a3b8;
  cursor: not-allowed;
}

.form-group textarea {
  resize: vertical;
  min-height: 80px;
}

/* Toast */
.toast-success {
  margin-top: 1rem;
  padding: 0.75rem 1rem;
  background: #f0fdf4;
  border: 1px solid #86efac;
  border-radius: 8px;
  color: #166534;
  font-size: 0.9rem;
  font-weight: 500;
}

/* Photo upload */
.photo-upload {
  border: 2px dashed #d1d5db;
  border-radius: 12px;
  padding: 2rem;
  text-align: center;
  cursor: pointer;
  transition: all 0.15s;
  background: #fafafa;
}

.photo-upload:hover {
  border-color: #1e2956;
  background: #f1f5f9;
}

.upload-placeholder p {
  margin: 0.5rem 0 0;
  color: #64748b;
  font-size: 0.95rem;
}

.upload-hint {
  font-size: 0.8rem !important;
  color: #94a3b8 !important;
}

.file-input-hidden {
  display: none;
}

.photo-previews {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
}

.photo-preview {
  position: relative;
  width: 100px;
  height: 100px;
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid #e2e8f0;
}

.photo-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.photo-remove {
  position: absolute;
  top: 4px;
  right: 4px;
  width: 22px;
  height: 22px;
  border-radius: 50%;
  border: none;
  background: rgba(0,0,0,0.6);
  color: white;
  font-size: 1rem;
  line-height: 1;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
}

.photo-add {
  width: 100px;
  height: 100px;
  border: 2px dashed #d1d5db;
  border-radius: 8px;
  background: #fafafa;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.15s;
}

.photo-add:hover {
  border-color: #1e2956;
  background: #f1f5f9;
}

/* Sell actions */
.sell-actions {
  margin-top: 0.5rem;
}

.btn-submit-sell {
  width: 100%;
  padding: 0.875rem;
  background: #1e2956;
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 1rem;
  font-weight: 700;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-submit-sell:hover:not(:disabled) {
  background: #3b4a8a;
}

.btn-submit-sell:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.success-card {
  margin-top: 1.5rem;
  background: #f0fdf4;
  border: 1px solid #86efac;
  border-radius: 12px;
  padding: 1.5rem;
  text-align: center;
}

.success-card h3 {
  font-size: 1.15rem;
  font-weight: 700;
  color: #166534;
  margin-bottom: 0.5rem;
}

.success-card p {
  color: #15803d;
  margin-bottom: 0;
}

/* Favorites */
.favorites-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1rem;
}

.fav-card {
  display: flex;
  gap: 0.75rem;
  padding: 0.75rem;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  position: relative;
  transition: border-color 0.15s;
}

.fav-card:hover {
  border-color: #1e2956;
}

.fav-img {
  width: 100px;
  height: 75px;
  border-radius: 8px;
  object-fit: cover;
  flex-shrink: 0;
}

.fav-info h4 {
  margin: 0 0 0.25rem;
  font-size: 0.95rem;
  font-weight: 600;
  color: #1e293b;
}

.fav-price {
  font-weight: 700;
  color: #1e2956;
  margin: 0 0 0.15rem;
  font-size: 0.9rem;
}

.fav-city {
  color: #94a3b8;
  font-size: 0.8rem;
  margin: 0;
}

.fav-remove {
  position: absolute;
  top: 6px;
  right: 6px;
  width: 24px;
  height: 24px;
  border-radius: 50%;
  border: none;
  background: #fef2f2;
  color: #dc2626;
  font-size: 1.1rem;
  line-height: 1;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.15s;
}

.fav-card:hover .fav-remove {
  opacity: 1;
}

/* Offers */
.offers-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.offer-card {
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 1.25rem;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  transition: border-color 0.15s;
}

.offer-card:hover {
  border-color: #1e2956;
}

.offer-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.offer-header h4 {
  margin: 0;
  font-size: 1rem;
  font-weight: 700;
  color: #1e293b;
}

.offer-status {
  font-size: 0.75rem;
  font-weight: 700;
  padding: 0.25rem 0.625rem;
  border-radius: 999px;
  text-transform: uppercase;
  letter-spacing: 0.3px;
}

.offer-status.pending {
  background: #fef9c3;
  color: #a16207;
}

.offer-status.accepted {
  background: #dcfce7;
  color: #166534;
}

.offer-status.declined {
  background: #fef2f2;
  color: #dc2626;
}

.offer-status.negotiation {
  background: #dbeafe;
  color: #1e40af;
}

.offer-body {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.offer-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem 1.5rem;
  font-size: 0.88rem;
  color: #64748b;
}

.offer-message {
  margin: 0;
  padding: 0.75rem;
  background: #f8fafc;
  border-radius: 8px;
  font-size: 0.88rem;
  color: #475569;
  font-style: italic;
  border-left: 3px solid #1e2956;
}

.offer-footer {
  display: flex;
  gap: 0.75rem;
}

.btn-accept {
  padding: 0.5rem 1.25rem;
  background: #166534;
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.85rem;
  cursor: pointer;
  transition: background 0.15s;
}

.btn-accept:hover {
  background: #15803d;
}

.btn-decline {
  padding: 0.5rem 1.25rem;
  background: white;
  color: #dc2626;
  border: 1px solid #fca5a5;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.85rem;
  cursor: pointer;
  transition: all 0.15s;
}

.btn-decline:hover {
  background: #fef2f2;
}

.text-muted {
  color: #94a3b8;
}

.link {
  color: #1e2956;
  font-weight: 600;
}
</style>
