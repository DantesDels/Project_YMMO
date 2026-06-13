<template>
  <div class="layout-wrapper">
    <header class="header">
      <div class="container header-content">
        <router-link to="/" class="logo-link">
          <img src="/favicon.svg" alt="Logo YMMO" class="favicon" />
          <span class="logo-text">YMMO</span>
        </router-link>

        <nav class="nav-links">
          <AppButton to="/louer">Louer</AppButton>
          <template v-if="authStore.user">
            <AppButton v-if="authStore.user.role === 'Agent'" to="/agent/dashboard">Dashboard</AppButton>
            <AppButton v-else to="/client/dashboard">Mon compte</AppButton>
            <button class="btn-logout" @click="handleLogout">Déconnexion</button>
          </template>
          <AppButton v-else to="/authentification">Se connecter</AppButton>
        </nav>
      </div>
    </header>

    <main class="main-content">
      <router-view />
    </main>

    <footer class="footer">
      <div class="container">
        <div class="footer-section">
          <router-link to="/aide">Aide</router-link>
          <router-link to="/cgu">CGU</router-link>
          <router-link to="/mentions-legales">Mentions légales</router-link>
          <router-link to="/confidentialite">Politique de confidentialité</router-link>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'
import AppButton from '@/components/ui/AppButton.vue'

const router = useRouter()
const authStore = useAuthentificationStore()

function handleLogout() {
  authStore.logout()
  router.push('/')
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

/* Header */
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

.btn-logout {
  background: none;
  border: 1px solid #e2e8f0;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  font-size: 0.9rem;
  font-weight: 500;
  color: #64748b;
  cursor: pointer;
  transition: all 0.15s;
}

.btn-logout:hover {
  background: #fef2f2;
  border-color: #fca5a5;
  color: #dc2626;
}

/* Footer */
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
