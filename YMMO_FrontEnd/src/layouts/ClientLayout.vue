<template>
  <div class="layout-wrapper">
    <header class="header">
      <div class="container header-content">
        <router-link to="/client/dashboard" class="logo-link">
          <img src="/favicon.svg" alt="Logo YMMO" class="favicon" />
          <span class="logo-text">YMMO</span>
        </router-link>

        <nav class="nav-links">
          <AppButton to="/client/favorites">Favoris</AppButton>
          <AppButton to="/catalog">Catalogue</AppButton>
          <AppButton to="/informations">À propos</AppButton>

          <span class="nav-sep">|</span>

          <div class="user-tag" @click="goToDashboard">
            <span class="user-avatar">{{ avatarLetter }}</span>
            <span class="user-name">{{ username }}</span>
            <svg class="chevron-down" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="6 9 12 15 18 9" />
            </svg>
          </div>
        </nav>
      </div>
    </header>

    <main class="main-content">
      <router-view />
    </main>

    <footer class="footer">
      <div class="container">
        <div class="footer-section">
          <router-link to="/help">Aide</router-link>
          <router-link to="/cgu">CGU</router-link>
          <router-link to="/legal-mentions">Mentions légales</router-link>
          <router-link to="/confidentiality">Politique de confidentialité</router-link>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'
import AppButton from '@/components/ui/AppButton.vue'

const router = useRouter()
const authStore = useAuthentificationStore()

const username = computed(() => authStore.user?.username ?? 'User')
const avatarLetter = computed(() => username.value.charAt(0).toUpperCase())

function goToDashboard() {
  router.push('/client/dashboard')
}
</script>

<style scoped>
@keyframes pulse-effect {
  0% { transform: scale(1); box-shadow: 0 0 0 0 rgba(30, 41, 86, 0.4); }
  50% { transform: scale(1.05); box-shadow: 0 0 0 10px rgba(30, 41, 86, 0); }
  100% { transform: scale(1); box-shadow: 0 0 0 0 rgba(30, 41, 86, 0); }
}

.layout-wrapper {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.container {
  max-width: 1200px;
  width: 100%;
  margin-left: auto;
  margin-right: auto;
  padding-left: 1rem;
  padding-right: 1rem;
  box-sizing: border-box;
}

.header {
  border-bottom: 1px solid #e5e7eb;
  padding: 1rem 0;
  background: white;
  box-shadow: 0 1px 5px rgba(0,0,0,0.2);
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.logo-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  text-decoration: none;
}

.logo-text {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1e2956;
}

.favicon {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  transition: all 0.3s ease;
  cursor: pointer;
}

.favicon:hover {
  animation: pulse-effect 1.5s infinite;
}

.favicon:active {
  transform: scale(0.95);
}

.nav-links {
  display: flex;
  gap: 1.5rem;
  align-items: center;
}

.nav-sep {
  color: #cbd5e1;
  font-weight: 300;
}

.user-tag {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.4rem 0.875rem;
  border: 1px solid #e2e8f0;
  border-radius: 999px;
  cursor: pointer;
  transition: all 0.15s;
  background: white;
}

.user-tag:hover {
  border-color: #1e2956;
  background: #f8fafc;
}

.user-avatar {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: #1e2956;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
  font-weight: 700;
}

.user-name {
  font-size: 0.9rem;
  font-weight: 600;
  color: #1e2956;
}

.chevron-down {
  color: #94a3b8;
}

.footer {
  margin-top: auto;
  background: #f9fafb;
  padding: 2rem 0;
  border-top: 1px solid #e5e7eb;
}

.footer-section {
  display: flex;
  justify-content: center;
  gap: 2rem;
  flex-wrap: wrap;
  color: #6b7280;
}

.footer-section a {
  color: #6b7280;
  text-decoration: none;
}

.footer-section a:hover {
  text-decoration: underline;
  color: #1e2956;
}
</style>
