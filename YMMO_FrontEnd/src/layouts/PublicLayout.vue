<template>
  <div class="layout-wrapper">
    <header class="header">
      <div class="container header-content">
        <router-link to="/" class="logo-link">
          <img src="/favicon.svg" alt="Logo YMMO" class="favicon" />
          <span class="logo-text">YMMO</span>
        </router-link>

        <nav class="nav-links">
          <AppButton to="/catalog">Catalogue</AppButton>
          <AppButton to="/informations">À propos</AppButton>
          
          <span>|</span>
          
          <template v-if="isAuthenticated">
            <AppButton :to="profileRoute">Profil</AppButton>
            <AppButton @click="authStore.logout">Déconnexion</AppButton>
          </template>

          <template v-else>
            <AppButton to="/authentification">Connexion</AppButton>
          </template>
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

<script setup>
import { computed } from 'vue';
import { useAuthentificationStore } from '@/stores/authentification.store';
import AppButton from '@/components/ui/AppButton.vue';

const authStore = useAuthentificationStore();

// Vérifie si le token existe et si l'utilisateur est chargé
const isAuthenticated = computed(() => !!authStore.token);

// Optionnel : si tu veux rediriger vers des pages différentes selon le rôle
const profileRoute = computed(() => {
  const role = authStore.user?.role;
  if (role === 'Agent') return '/dashboard'; // ou /agent/profile
  if (role === 'Admin') return '/admin';
  return '/profile'; // Client
});
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