<template>
  <div class="auth-wrapper">
    <div class="auth-card">
      <div class="tabs">
        <button
          :class="['tab', { active: activeTab === 'login' }]"
          @click="activeTab = 'login'"
        >Connexion</button>
        <button
          :class="['tab', { active: activeTab === 'register' }]"
          @click="activeTab = 'register'"
        >Inscription</button>
      </div>

      <AuthentificationForm
        v-if="activeTab === 'login'"
        title="Connectez-vous"
        button-text="Se connecter"
        :loading="authStore.isLoading"
        @submit="handleLogin"
      >
        <template #fields="{ form }">
          <div>
            <label class="label" for="login-email">Email</label>
            <input
              id="login-email"
              v-model="form.email"
              type="email"
              class="input"
              placeholder="vous@exemple.fr"
              required
              autocomplete="email"
            />
          </div>
          <div>
            <label class="label" for="login-password">Mot de passe</label>
            <input
              id="login-password"
              v-model="form.password"
              type="password"
              class="input"
              placeholder="••••••••"
              required
              autocomplete="current-password"
            />
          </div>
        </template>
        <template #footer>
          Pas encore de compte ?
          <button class="link" @click="activeTab = 'register'">Inscrivez-vous</button>
        </template>
      </AuthentificationForm>

      <div v-if="activeTab === 'login'" class="divider">
        <span>ou</span>
      </div>

      <GoogleSignInButton v-if="activeTab === 'login'" :loading="authStore.isLoading" @click="handleGoogleLogin" />

      <AuthentificationForm
        v-if="activeTab === 'register'"
        title="Créez votre compte"
        button-text="S'inscrire"
        :loading="authStore.isLoading"
        @submit="handleRegister"
      >
        <template #fields="{ form }">
          <div class="row">
            <div class="flex-1">
              <label class="label" for="reg-firstname">Prénom</label>
              <input
                id="reg-firstname"
                v-model="form.firstName"
                type="text"
                class="input"
                placeholder="Jean"
                required
              />
            </div>
            <div class="flex-1">
              <label class="label" for="reg-lastname">Nom</label>
              <input
                id="reg-lastname"
                v-model="form.lastName"
                type="text"
                class="input"
                placeholder="Dupont"
                required
              />
            </div>
          </div>
          <div>
            <label class="label" for="reg-username">Nom d'utilisateur</label>
            <input
              id="reg-username"
              v-model="form.username"
              type="text"
              class="input"
              placeholder="jdupont"
              required
            />
          </div>
          <div>
            <label class="label" for="reg-email">Email</label>
            <input
              id="reg-email"
              v-model="form.email"
              type="email"
              class="input"
              placeholder="vous@exemple.fr"
              required
              autocomplete="email"
            />
          </div>
          <div>
            <label class="label" for="reg-phone">Téléphone</label>
            <input
              id="reg-phone"
              v-model="form.phoneNumber"
              type="tel"
              class="input"
              placeholder="06 12 34 56 78"
              required
            />
          </div>
          <div>
            <label class="label" for="reg-password">Mot de passe</label>
            <input
              id="reg-password"
              v-model="form.password"
              type="password"
              class="input"
              placeholder="Minimum 8 caractères"
              required
              minlength="8"
              autocomplete="new-password"
            />
            <p class="password-hint">Astuce : majuscule + chiffre + symbole (ex: @, #, $) renforce la sécurité.</p>
          </div>
          <div>
            <label class="label" for="reg-password-confirm">Confirmer le mot de passe</label>
            <input
              id="reg-password-confirm"
              v-model="form.passwordConfirm"
              type="password"
              class="input"
              :class="{ 'input-error': passwordError }"
              placeholder="Retaper le mot de passe"
              required
              minlength="8"
              autocomplete="new-password"
              @input="clearPasswordError"
            />
            <p v-if="passwordError" class="field-error">Les mots de passe ne correspondent pas.</p>
          </div>
        </template>
        <template #footer>
          Déjà inscrit ?
          <button class="link" @click="activeTab = 'login'">Connectez-vous</button>
        </template>
      </AuthentificationForm>

      <div v-if="activeTab === 'register'" class="divider">
        <span>ou</span>
      </div>

      <GoogleSignInButton v-if="activeTab === 'register'" :loading="authStore.isLoading" @click="handleGoogleLogin" />

      <p v-if="authStore.error" class="error-msg">{{ authStore.error }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'
import AuthentificationForm from '@/components/AuthentificationForm.vue'
import GoogleSignInButton from '@/components/ui/GoogleSignInButton.vue'
import type { LoginRequest, RegisterRequest } from '@/types'

const router = useRouter()
const authStore = useAuthentificationStore()
const activeTab = ref<'login' | 'register'>('login')
const passwordError = ref(false)

const redirectPath = computed(() => {
  const role = authStore.user?.role
  if (role === 'Agent') return '/agent/dashboard'
  if (role === 'Admin') return '/dashboard'
  return '/client/dashboard'
})

async function handleLogin(form: Record<string, any>) {
  const credentials: LoginRequest = {
    email: form.email,
    password: form.password,
  }
  await authStore.login(credentials)
  if (!authStore.error && authStore.user) {
    router.push(redirectPath.value)
  }
}

async function handleGoogleLogin() {
  await authStore.googleLogin()
  if (!authStore.error && authStore.user) {
    router.push(redirectPath.value)
  }
}

function clearPasswordError() {
  passwordError.value = false
}

async function handleRegister(form: Record<string, any>) {
  if (form.password !== form.passwordConfirm) {
    passwordError.value = true
    return
  }
  passwordError.value = false
  const details: RegisterRequest = {
    username: form.username,
    lastName: `${form.firstName} ${form.lastName}`,
    email: form.email,
    phoneNumber: form.phoneNumber,
    password: form.password,
  }
  await authStore.register(details)
  if (!authStore.error) {
    activeTab.value = 'login'
  }
}
</script>

<style scoped>
.auth-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 150px);
  padding: 2rem 1rem;
}

.auth-card {
  width: 100%;
  max-width: 440px;
}

.tabs {
  display: flex;
  margin-bottom: 1.5rem;
  border-radius: 12px;
  overflow: hidden;
  border: 1px solid #e2e8f0;
}

.tab {
  flex: 1;
  padding: 0.75rem;
  font-size: 1rem;
  font-weight: 600;
  border: none;
  cursor: pointer;
  background: #f8fafc;
  color: #64748b;
  transition: all 0.2s;
}

.tab.active {
  background: #1e2956;
  color: white;
}

.tab:not(.active):hover {
  background: #e2e8f0;
}

.row {
  display: flex;
  gap: 1rem;
}

.flex-1 {
  flex: 1;
}

.label {
  display: block;
  font-size: 0.875rem;
  font-weight: 600;
  color: #334155;
  text-align: left;
  margin-bottom: 1px;
  padding-top: 15px;
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

.link {
  background: none;
  border: none;
  color: #1e2956;
  font-weight: 600;
  cursor: pointer;
  text-decoration: underline;
  font-size: inherit;
  padding: 0;
}

.link:hover {
  color: #3b4a8a;
}

.divider {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin: 1.25rem 0;
}

.divider::before,
.divider::after {
  content: '';
  flex: 1;
  height: 1px;
  background: #e2e8f0;
}

.divider span {
  font-size: 0.85rem;
  color: #94a3b8;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.input-error {
  border-color: #dc2626 !important;
}

.input-error:focus {
  box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.15) !important;
}

.field-error {
  margin-top: 0.3rem;
  font-size: 0.8rem;
  color: #dc2626;
  font-weight: 500;
}

.password-hint {
  margin-top: 0.3rem;
  font-size: 0.75rem;
  color: #94a3b8;
  font-style: italic;
}

.error-msg {
  margin-top: 1rem;
  text-align: center;
  color: #dc2626;
  font-size: 0.875rem;
  background: #fef2f2;
  padding: 0.625rem;
  border-radius: 8px;
}
</style>
