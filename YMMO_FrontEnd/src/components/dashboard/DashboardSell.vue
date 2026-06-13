<template>
  <div>
    <div class="section-header">
      <h3 class="section-title">Mes ventes</h3>
      <button v-if="!showSellForm" class="btn-edit" @click="showSellForm = true">Nouvelle demande</button>
      <button v-else class="btn-cancel" @click="showSellForm = false">Annuler</button>
    </div>

    <div v-if="!showSellForm" class="sell-history">
      <p class="text-muted" v-if="sellRequests.length === 0">Aucune demande de vente pour le moment.</p>
      <div v-else class="sell-list">
        <div v-for="req in sellRequests" :key="req.id" class="sell-history-card">
          <div class="sell-history-header">
            <h4>{{ req.title }}</h4>
            <span :class="['sell-status', req.status]">{{ sellStatusLabel(req.status) }}</span>
          </div>
          <div class="sell-history-body">
            <span>{{ req.type }} · {{ req.surface }} m² · {{ req.city }} ({{ req.zipCode }})</span>
            <span>Prix souhaité : <strong>{{ req.price }} €</strong></span>
          </div>
          <p class="sell-history-date">Soumis le {{ req.createdAt }}</p>
        </div>
      </div>
    </div>

    <div v-else>
      <p class="section-desc">
        Décrivez votre bien en détail et joignez des photos. Votre demande sera étudiée par nos agents.
      </p>

      <form class="sell-form" @submit.prevent="submitSellRequest" novalidate>
        <div class="form-group">
          <label for="sell-title">Titre de l'annonce *</label>
          <input id="sell-title" v-model="sellForm.title" placeholder="Ex : Appartement 3 pièces centre-ville" required />
        </div>
        <div class="form-row">
          <div class="form-group">
            <label for="sell-type">Type *</label>
            <select id="sell-type" v-model="sellForm.type" required>
              <option value="">Sélectionnez…</option>
              <option value="Appartement">Appartement</option>
              <option value="Maison">Maison</option>
              <option value="Villa">Villa</option>
              <option value="Terrain">Terrain</option>
              <option value="Local commercial">Local commercial</option>
            </select>
          </div>
          <div class="form-group">
            <label for="sell-surface">Surface (m²) *</label>
            <input id="sell-surface" v-model.number="sellForm.surface" type="number" min="1" placeholder="75" required />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label for="sell-rooms">Pièces</label>
            <input id="sell-rooms" v-model.number="sellForm.rooms" type="number" min="1" placeholder="3" />
          </div>
          <div class="form-group">
            <label for="sell-price">Prix souhaité (€) *</label>
            <input id="sell-price" v-model.number="sellForm.price" type="number" min="1" placeholder="250000" required />
          </div>
        </div>
        <div class="form-row form-row-full">
          <div class="form-group">
            <label for="sell-address">Adresse du bien *</label>
            <input id="sell-address" v-model="sellForm.address" placeholder="Numéro et rue" required />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label for="sell-zip">Code postal *</label>
            <input id="sell-zip" v-model="sellForm.zipCode" placeholder="75001" maxlength="5" required />
          </div>
          <div class="form-group">
            <label for="sell-city">Ville *</label>
            <input id="sell-city" v-model="sellForm.city" placeholder="Paris" required />
          </div>
        </div>
        <div class="form-group">
          <label for="sell-desc">Description détaillée</label>
          <textarea id="sell-desc" v-model="sellForm.description" rows="5" placeholder="Décrivez le bien, son état, ses atouts…"></textarea>
        </div>

        <div class="form-group">
          <label>Photos du bien</label>
          <div class="photo-upload" role="button" tabindex="0" @click="triggerFileInput" @keydown.enter.prevent="triggerFileInput" @keydown.space.prevent="triggerFileInput" aria-label="Ajouter des photos">
            <div v-if="sellForm.photos.length === 0" class="upload-placeholder">
              <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="#94a3b8" stroke-width="1.5" aria-hidden="true">
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
                <button type="button" class="photo-remove" @click.stop="removePhoto(idx)" :aria-label="'Supprimer ' + photo.name">×</button>
              </div>
              <button v-if="sellForm.photos.length < 10" type="button" class="photo-add" @click.stop="triggerFileInput" aria-label="Ajouter une photo">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#94a3b8" stroke-width="1.5" aria-hidden="true">
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

      <div v-if="sellSubmitted" class="success-card" role="status">
        <h3>Demande envoyée !</h3>
        <p>Votre bien a été soumis à nos agences partenaires. Un agent vous contactera sous 48h.</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'

interface SellPhoto {
  url: string
  name: string
}

interface SellRequest {
  id: number
  title: string
  type: string
  surface: number
  price: number
  city: string
  zipCode: string
  status: 'pending' | 'in_review' | 'accepted' | 'rejected'
  createdAt: string
}

const showSellForm = ref(false)
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

const sellRequests = reactive<SellRequest[]>([])

function sellStatusLabel(status: string) {
  const labels: Record<string, string> = {
    pending: 'En attente',
    in_review: 'À l\'étude',
    accepted: 'Acceptée',
    rejected: 'Refusée',
  }
  return labels[status] ?? status
}

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

onMounted(() => {
  sellRequests.push(
    { id: 1, title: 'Appartement 3 pièces centre-ville', type: 'Appartement', surface: 72, price: 245000, city: 'Lyon', zipCode: '69003', status: 'in_review', createdAt: '12/06/2026' },
    { id: 2, title: 'Maison de campagne', type: 'Maison', surface: 120, price: 320000, city: 'Bordeaux', zipCode: '33000', status: 'pending', createdAt: '10/06/2026' },
  )
})
</script>

<style scoped>
.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.25rem;
  gap: 0.75rem;
  flex-wrap: wrap;
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

.text-muted {
  color: #94a3b8;
}

.btn-edit {
  padding: 0.45rem 1rem;
  border: 1px solid #1e2956;
  background: transparent;
  color: #1e2956;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.15s;
  min-height: 44px;
  white-space: nowrap;
}

.btn-edit:hover {
  background: #1e2956;
  color: white;
}

.btn-edit:focus-visible {
  outline: 2px solid #1e2956;
  outline-offset: 2px;
}

.btn-cancel {
  padding: 0.45rem 1rem;
  border: 1px solid #e2e8f0;
  background: white;
  color: #64748b;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 500;
  cursor: pointer;
  min-height: 44px;
}

.btn-cancel:hover {
  background: #f8fafc;
}

.btn-cancel:focus-visible {
  outline: 2px solid #64748b;
  outline-offset: 2px;
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
  min-width: 0;
}

.form-group label {
  font-size: 0.85rem;
  font-weight: 600;
  color: #475569;
}

.form-group input,
.form-group select,
.form-group textarea {
  padding: 0.65rem 0.75rem;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  font-size: 0.95rem;
  color: #1e293b;
  background: white;
  transition: border-color 0.15s, box-shadow 0.15s;
  font-family: inherit;
  width: 100%;
  box-sizing: border-box;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #1e2956;
  box-shadow: 0 0 0 3px rgba(30, 41, 86, 0.12);
}

.form-group textarea {
  resize: vertical;
  min-height: 80px;
}

.sell-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.sell-history {
  margin-top: 0.5rem;
}

.sell-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.sell-history-card {
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 1rem;
}

.sell-history-card:hover {
  border-color: #cbd5e1;
}

.sell-history-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
  margin-bottom: 0.25rem;
}

.sell-history-header h4 {
  font-size: 1rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
  word-break: break-word;
}

.sell-status {
  font-size: 0.75rem;
  font-weight: 600;
  padding: 0.2rem 0.6rem;
  border-radius: 99px;
  text-transform: capitalize;
  white-space: nowrap;
}

.sell-status.pending {
  background: #fef9c3;
  color: #854d0e;
}

.sell-status.in_review {
  background: #dbeafe;
  color: #1e40af;
}

.sell-status.accepted {
  background: #dcfce7;
  color: #166534;
}

.sell-status.rejected {
  background: #fef2f2;
  color: #991b1b;
}

.sell-history-body {
  display: flex;
  flex-direction: column;
  gap: 0.15rem;
  font-size: 0.85rem;
  color: #64748b;
  margin-bottom: 0.35rem;
}

.sell-history-date {
  font-size: 0.8rem;
  color: #94a3b8;
  margin: 0;
}

.photo-upload {
  border: 2px dashed #d1d5db;
  border-radius: 12px;
  padding: 2rem 1rem;
  text-align: center;
  cursor: pointer;
  transition: all 0.15s;
  background: #fafafa;
}

.photo-upload:hover {
  border-color: #1e2956;
  background: #f1f5f9;
}

.photo-upload:focus-visible {
  outline: 2px solid #1e2956;
  outline-offset: 2px;
  border-color: #1e2956;
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
  justify-content: center;
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
  width: 28px;
  height: 28px;
  border-radius: 50%;
  border: none;
  background: rgba(0,0,0,0.6);
  color: white;
  font-size: 1.1rem;
  line-height: 1;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.15s;
}

.photo-remove:hover {
  background: rgba(0,0,0,0.8);
}

.photo-remove:focus-visible {
  outline: 2px solid white;
  outline-offset: 2px;
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

.photo-add:focus-visible {
  outline: 2px solid #1e2956;
  outline-offset: 2px;
}

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
  min-height: 48px;
}

.btn-submit-sell:hover:not(:disabled) {
  background: #3b4a8a;
}

.btn-submit-sell:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-submit-sell:focus-visible {
  outline: 2px solid #1e2956;
  outline-offset: 2px;
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

@media (max-width: 767px) {
  .form-row {
    grid-template-columns: 1fr;
  }

  .section-header {
    flex-direction: column;
    align-items: flex-start;
  }
}

@media (max-width: 480px) {
  .photo-preview,
  .photo-add {
    width: 80px;
    height: 80px;
  }
}
</style>
