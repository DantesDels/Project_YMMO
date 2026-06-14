<template>
  <form @submit.prevent="handleSubmit" class="property-form">
    <div v-if="error" class="error-banner">{{ error }}</div>

    <fieldset class="fieldset">
      <legend class="legend">Informations générales</legend>
      <div class="field">
        <label class="label" for="prop-name">Nom du bien</label>
        <input id="prop-name" v-model="form.propertyName" type="text" class="input" required />
      </div>
      <div class="field">
        <label class="label" for="prop-desc">Description</label>
        <textarea id="prop-desc" v-model="form.propertyDescription" class="input textarea" rows="4" />
      </div>
      <div class="row">
        <div class="field flex-1">
          <label class="label" for="prop-type">Type</label>
          <select id="prop-type" v-model="form.propertyType" class="input" required>
            <option value="" disabled>Sélectionner</option>
            <option v-for="t in propertyTypes" :key="t.value" :value="t.value">{{ t.label }}</option>
          </select>
        </div>
        <div class="field flex-1">
          <label class="label" for="prop-year">Année de construction</label>
          <input id="prop-year" v-model.number="form.yearBuilt" type="number" class="input" min="1800" :max="currentYear" />
        </div>
      </div>
    </fieldset>

    <fieldset class="fieldset">
      <legend class="legend">Caractéristiques</legend>
      <div class="row">
        <div class="field flex-1">
          <label class="label" for="prop-surface">Surface (m²)</label>
          <input id="prop-surface" v-model.number="form.surface" type="number" class="input" min="1" required />
        </div>
        <div class="field flex-1">
          <label class="label" for="prop-price">Prix (€)</label>
          <input id="prop-price" v-model.number="form.initialPrice" type="number" class="input" min="0" required />
        </div>
      </div>
      <div class="row">
        <div class="field flex-1">
          <label class="label" for="prop-rooms">Pièces</label>
          <input id="prop-rooms" v-model.number="form.rooms" type="number" class="input" min="1" max="50" />
        </div>
        <div class="field flex-1">
          <label class="label" for="prop-furnishing">Meublé</label>
          <select id="prop-furnishing" v-model="form.furnishing" class="input">
            <option value="">Non spécifié</option>
            <option value="Meublé">Meublé</option>
            <option value="Non meublé">Non meublé</option>
          </select>
        </div>
      </div>
      <div class="row">
        <div class="field flex-1">
          <label class="label" for="prop-condition">État</label>
          <select id="prop-condition" v-model="form.condition" class="input" required>
            <option value="" disabled>Sélectionner</option>
            <option v-for="c in conditions" :key="c.value" :value="c.value">{{ c.label }}</option>
          </select>
        </div>
        <div class="field flex-1">
          <label class="label" for="prop-energy">Classe énergétique</label>
          <select id="prop-energy" v-model="form.energyClass" class="input">
            <option value="" disabled>Sélectionner</option>
            <option v-for="e in energyClasses" :key="e.value" :value="e.value">{{ e.label }}</option>
          </select>
        </div>
      </div>
    </fieldset>

    <fieldset class="fieldset">
      <legend class="legend">Localisation</legend>
      <div class="field">
        <label class="label" for="prop-street">Adresse</label>
        <input id="prop-street" v-model="form.location.street" type="text" class="input" required />
      </div>
      <div class="row">
        <div class="field flex-1">
          <label class="label" for="prop-city">Ville</label>
          <input id="prop-city" v-model="form.location.city" type="text" class="input" required />
        </div>
        <div class="field flex-1">
          <label class="label" for="prop-zip">Code postal</label>
          <input id="prop-zip" v-model="form.location.postalCode" type="text" class="input" required />
        </div>
      </div>
      <div class="row">
        <div class="field flex-1">
          <label class="label" for="prop-region">Région</label>
          <input id="prop-region" v-model="form.location.region" type="text" class="input" />
        </div>
        <div class="field flex-1">
          <label class="label" for="prop-country">Pays</label>
          <input id="prop-country" v-model="form.location.country" type="text" class="input" value="France" />
        </div>
      </div>
    </fieldset>

    <fieldset class="fieldset">
      <legend class="legend">Extérieurs et Annexes</legend>
      <div class="checkbox-grid">
        <label v-for="c in criteriaGroups.exterieur" :key="c.value" class="checkbox-label">
          <input type="checkbox" :value="c.value" :checked="form.features.includes(c.value)" @change="toggleCriteria(c.value)" />
          {{ c.label }}
        </label>
      </div>
    </fieldset>

    <fieldset class="fieldset">
      <legend class="legend">Intérieur et Confort</legend>
      <div class="checkbox-grid">
        <label v-for="c in criteriaGroups.interieur" :key="c.value" class="checkbox-label">
          <input type="checkbox" :value="c.value" :checked="form.features.includes(c.value)" @change="toggleCriteria(c.value)" />
          {{ c.label }}
        </label>
      </div>
    </fieldset>

    <fieldset class="fieldset">
      <legend class="legend">Sécurité et Vues</legend>
      <div class="checkbox-grid">
        <label v-for="c in criteriaGroups.securite" :key="c.value" class="checkbox-label">
          <input type="checkbox" :value="c.value" :checked="form.features.includes(c.value)" @change="toggleCriteria(c.value)" />
          {{ c.label }}
        </label>
      </div>
    </fieldset>

    <fieldset class="fieldset">
      <legend class="legend">Tech & Énergie</legend>
      <div class="checkbox-grid">
        <label v-for="c in criteriaGroups.tech" :key="c.value" class="checkbox-label">
          <input type="checkbox" :value="c.value" :checked="form.features.includes(c.value)" @change="toggleCriteria(c.value)" />
          {{ c.label }}
        </label>
      </div>
    </fieldset>

    <button type="submit" class="btn-submit" :disabled="loading">
      {{ loading ? 'Enregistrement...' : submitLabel }}
    </button>
  </form>
</template>

<script setup lang="ts">
import { reactive, computed } from 'vue'
import { Criteria } from '@/types'

const props = withDefaults(defineProps<{
  submitLabel?: string
  loading?: boolean
  initialData?: Partial<{
    propertyName: string
    propertyDescription: string | null
    propertyType: string
    yearBuilt: number
    condition: string
    energyClass: string
    initialPrice: number
    surface: number
    rooms: number
    furnishing: string
    features: string[]
    location: Partial<{
      street: string
      city: string
      postalCode: string
      region: string
      country: string
    }>
  }>
}>(), {
  submitLabel: 'Enregistrer',
  loading: false,
})

const emit = defineEmits<{
  submit: [data: any]
}>()

const currentYear = new Date().getFullYear()
const error = ''

const propertyTypes = [
  { value: 'Apartment', label: 'Appartement' },
  { value: 'House', label: 'Maison' },
  { value: 'Commercial', label: 'Local commercial' },
  { value: 'Land', label: 'Terrain' },
  { value: 'Office', label: 'Bureau' },
  { value: 'Garage', label: 'Garage' },
  { value: 'Parking', label: 'Parking' },
]

const conditions = [
  { value: 'New', label: 'Neuf' },
  { value: 'Excellent', label: 'Excellent état' },
  { value: 'Good', label: 'Bon état' },
  { value: 'NeedsRefresh', label: 'À rafraîchir' },
  { value: 'NeedsRenovation', label: 'À rénover' },
  { value: 'Ruin', label: 'Ruine' },
]

const energyClasses = [
  { value: 'A', label: 'A' },
  { value: 'B', label: 'B' },
  { value: 'C', label: 'C' },
  { value: 'D', label: 'D' },
  { value: 'E', label: 'E' },
  { value: 'F', label: 'F' },
  { value: 'G', label: 'G' },
  { value: 'Ex', label: 'Exempt' },
]

const criteriaGroups = {
  exterieur: [
    { value: Criteria.Balcony, label: 'Balcon' },
    { value: Criteria.Terrace, label: 'Terrasse' },
    { value: Criteria.Garden, label: 'Jardin' },
    { value: Criteria.Garage, label: 'Garage fermé' },
    { value: Criteria.Parking, label: 'Parking' },
    { value: Criteria.Cellar, label: 'Cave' },
    { value: Criteria.SwimmingPool, label: 'Piscine' },
  ],
  interieur: [
    { value: Criteria.Elevator, label: 'Ascenseur' },
    { value: Criteria.AirConditioning, label: 'Climatisation' },
    { value: Criteria.Fireplace, label: 'Cheminée' },
    { value: Criteria.Furnished, label: 'Meublé' },
    { value: Criteria.HardwoodFloor, label: 'Parquet' },
    { value: Criteria.DoubleGlazing, label: 'Double vitrage' },
    { value: Criteria.FittedKitchen, label: 'Cuisine équipée' },
    { value: Criteria.Studio, label: 'Studio' },
  ],
  securite: [
    { value: Criteria.Digicode, label: 'Digicode' },
    { value: Criteria.Intercom, label: 'Interphone' },
    { value: Criteria.AlarmSystem, label: 'Alarme' },
    { value: Criteria.SecurityDoor, label: 'Porte blindée' },
    { value: Criteria.Caretaker, label: 'Gardien' },
    { value: Criteria.SeaView, label: 'Vue mer' },
    { value: Criteria.MountainView, label: 'Vue montagne' },
    { value: Criteria.UnobstructedView, label: 'Vue dégagée' },
    { value: Criteria.SouthFacing, label: 'Exposition sud' },
  ],
  tech: [
    { value: Criteria.DisabledAccess, label: 'Accès PMR' },
    { value: Criteria.FiberOptic, label: 'Fibre optique' },
    { value: Criteria.SmartHome, label: 'Domotique' },
    { value: Criteria.HeatPump, label: 'Pompe à chaleur' },
    { value: Criteria.SolarPanels, label: 'Panneaux solaires' },
  ],
}

const form = reactive({
  propertyName: props.initialData?.propertyName ?? '',
  propertyDescription: props.initialData?.propertyDescription ?? '',
  propertyType: props.initialData?.propertyType ?? '',
  yearBuilt: props.initialData?.yearBuilt ?? currentYear,
  condition: props.initialData?.condition ?? '',
  energyClass: props.initialData?.energyClass ?? '',
  initialPrice: props.initialData?.initialPrice ?? 0,
  surface: props.initialData?.surface ?? 0,
  rooms: props.initialData?.rooms ?? 1,
  furnishing: props.initialData?.furnishing ?? '',
  features: props.initialData?.features ?? ([] as string[]),
  location: reactive({
    street: props.initialData?.location?.street ?? '',
    city: props.initialData?.location?.city ?? '',
    postalCode: props.initialData?.location?.postalCode ?? '',
    region: props.initialData?.location?.region ?? '',
    country: props.initialData?.location?.country ?? 'France',
  }),
})

function toggleCriteria(value: string) {
  const idx = form.features.indexOf(value)
  if (idx >= 0) form.features.splice(idx, 1)
  else form.features.push(value)
}

function handleSubmit() {
  emit('submit', {
    propertyName: form.propertyName,
    propertyDescription: form.propertyDescription || null,
    propertyType: form.propertyType,
    yearBuilt: form.yearBuilt,
    condition: form.condition,
    energyClass: form.energyClass,
    initialPrice: form.initialPrice,
    surface: form.surface,
    rooms: form.rooms,
    furnishing: form.furnishing,
    features: form.features,
    location: { ...form.location },
  })
}
</script>

<style scoped>
.property-form {
  max-width: 720px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.error-banner {
  background: #fef2f2;
  color: #dc2626;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 500;
}

.fieldset {
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 1.25rem;
  background: white;
}

.legend {
  font-weight: 700;
  font-size: 1.05rem;
  color: #1e2956;
  padding: 0 0.5rem;
}

.field {
  margin-bottom: 1rem;
}

.field:last-child {
  margin-bottom: 0;
}

.label {
  display: block;
  font-size: 0.875rem;
  font-weight: 600;
  color: #334155;
  margin-bottom: 0.375rem;
}

.input {
  width: 100%;
  padding: 0.625rem 0.875rem;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  font-size: 0.95rem;
  transition: border-color 0.2s;
  background: white;
  box-sizing: border-box;
}

.input:focus {
  outline: none;
  border-color: #1e2956;
  box-shadow: 0 0 0 3px rgba(30, 41, 86, 0.1);
}

.textarea {
  resize: vertical;
  min-height: 80px;
}

.row {
  display: flex;
  gap: 1rem;
}

.flex-1 {
  flex: 1;
}

.checkbox-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 0.5rem;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  color: #334155;
  cursor: pointer;
}

.btn-submit {
  width: 100%;
  padding: 0.75rem;
  background: #1e2956;
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 1rem;
  font-weight: 700;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-submit:hover {
  background: #3b4a8a;
}

.btn-submit:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

select.input {
  appearance: auto;
}

@media (max-width: 480px) {
  .row { flex-direction: column; gap: 0; }
  .property-form { padding: 0 0.5rem; }
}
</style>
