<template>
  <div>
    <div v-if="errorToast" class="toast-error toast-sticky" role="alert">{{ errorToast }}</div>
    <div class="section-header">
      <h3 class="section-title">Mon profil</h3>
      <button v-if="!editing" class="btn-edit" @click="startEditing">Modifier</button>
      <div v-else class="edit-actions">
        <button class="btn-save" @click="saveProfile" :disabled="saving">{{ saving ? 'Enregistrement…' : 'Enregistrer' }}</button>
        <button class="btn-cancel" @click="cancelEditing">Annuler</button>
      </div>
    </div>

    <div class="profile-avatar-section">
      <div class="avatar-large" aria-hidden="true">{{ avatarLetter }}</div>
      <div class="profile-summary">
        <p class="profile-name">{{ form.firstName }} {{ form.lastName }}</p>
        <p class="profile-username">@{{ form.username }}</p>
        <p class="profile-role">{{ roleLabel }}</p>
      </div>
    </div>

    <div class="profile-form">
      <div class="form-row">
        <div class="form-group">
          <label for="pf-firstname">Prénom</label>
          <input id="pf-firstname" v-model="form.firstName" :disabled="!editing" placeholder="Votre prénom" />
        </div>
        <div class="form-group">
          <label for="pf-lastname">Nom</label>
          <input id="pf-lastname" v-model="form.lastName" :disabled="!editing" placeholder="Votre nom" />
        </div>
      </div>
      <div class="form-row">
        <div class="form-group">
          <label for="pf-email">Email</label>
          <input id="pf-email" v-model="form.email" :disabled="!editing" type="email" placeholder="email@exemple.fr" />
        </div>
        <div class="form-group">
          <label for="pf-phone">Téléphone</label>
          <input id="pf-phone" v-model="form.phone" :disabled="!editing" placeholder="+33612345678" aria-describedby="pf-phone-hint" :class="{ 'input-invalid': phoneError }" @input="clearPhoneError" @blur="validatePhone" />
          <p id="pf-phone-hint" class="input-hint">Format international : +33 suivi de votre numéro (ex: +33612345678)</p>
          <p v-if="phoneError" class="field-error" role="alert">{{ phoneError }}</p>
        </div>
      </div>
      <div class="form-row form-row-full">
        <div class="form-group">
          <label for="pf-address">Adresse</label>
          <input id="pf-address" v-model="form.address" :disabled="!editing" placeholder="Votre adresse" />
        </div>
      </div>
      <div class="form-row">
        <div class="form-group">
          <label for="pf-zip">Code postal</label>
          <input id="pf-zip" v-model="form.zipCode" :disabled="!editing" placeholder="75001" maxlength="5" />
        </div>
        <div class="form-group">
          <label for="pf-city">Ville</label>
          <input id="pf-city" v-model="form.city" :disabled="!editing" placeholder="Paris" />
        </div>
      </div>
      <div class="form-group">
        <label for="pf-bio">À propos de moi</label>
        <textarea id="pf-bio" v-model="form.bio" :disabled="!editing" rows="4" placeholder="Parlez de vous aux agents…"></textarea>
      </div>
    </div>

    <div v-if="saved" class="toast-success" role="status">Profil mis à jour avec succès.</div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useAuthentificationStore } from '@/stores/authentification.store'

const authStore = useAuthentificationStore()

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

const PHONE_REGEX = /^\+[1-9]\d{1,14}$/
const errorToast = ref('')
const phoneError = ref('')

const PHONE_HINT = 'Le numéro de téléphone doit être au format international (+33…). Exemple : +33612345678'

function showErrorToast(msg: string) {
  errorToast.value = msg
  setTimeout(() => { errorToast.value = '' }, 4000)
}

function clearPhoneError() {
  phoneError.value = ''
}

function validatePhone(): boolean {
  if (!form.phone) return true
  if (!PHONE_REGEX.test(form.phone)) {
    phoneError.value = PHONE_HINT
    return false
  }
  phoneError.value = ''
  return true
}

async function saveProfile() {
  if (!validatePhone()) {
    showErrorToast(PHONE_HINT)
    return
  }
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

.edit-actions {
  display: flex;
  gap: 0.5rem;
}

.btn-save {
  padding: 0.45rem 1rem;
  background: #1e2956;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.15s;
  min-height: 44px;
}

.btn-save:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-save:hover:not(:disabled) {
  background: #3b4a8a;
}

.btn-save:focus-visible {
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
  min-width: 0;
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
  word-break: break-word;
}

.profile-role {
  font-size: 0.8rem;
  color: #94a3b8;
  margin: 0;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.profile-form {
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

.input-hint {
  font-size: 0.75rem;
  color: #94a3b8;
  margin: 0.25rem 0 0 0;
  line-height: 1.4;
}

.input-invalid {
  border-color: #dc2626 !important;
}
.input-invalid:focus {
  box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.15) !important;
}

.field-error {
  margin-top: 0.25rem;
  font-size: 0.8rem;
  color: #dc2626;
  font-weight: 500;
  line-height: 1.4;
}

.toast-sticky {
  position: sticky;
  top: 0;
  z-index: 10;
}

.toast-success, .toast-error {
  margin-top: 1rem;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  font-size: 0.9rem;
  font-weight: 500;
}

.toast-success {
  background: #f0fdf4;
  border: 1px solid #86efac;
  color: #166534;
}

.toast-error {
  background: #fef2f2;
  border: 1px solid #fca5a5;
  color: #991b1b;
}

@media (max-width: 767px) {
  .form-row {
    grid-template-columns: 1fr;
  }

  .profile-avatar-section {
    gap: 1rem;
  }

  .avatar-large {
    width: 52px;
    height: 52px;
    font-size: 1.25rem;
  }

  .section-header {
    flex-direction: column;
    align-items: flex-start;
  }
}

@media (max-width: 480px) {
  .edit-actions {
    width: 100%;
  }

  .edit-actions .btn-save,
  .edit-actions .btn-cancel {
    flex: 1;
  }
}
</style>
