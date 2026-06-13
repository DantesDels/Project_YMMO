<template>
  <div class="dashboard">
    <aside class="sidebar">
      <h2 class="sidebar-title">Mon compte</h2>
      <nav class="sidebar-nav">
        <button
          v-for="item in navItems"
          :key="item.id"
          :class="['nav-btn', { active: activeSection === item.id }]"
          @click="activeSection = item.id"
        >
          {{ item.label }}
        </button>
      </nav>
      <button class="logout-btn" @click="handleLogout">Déconnexion</button>
    </aside>

    <main class="main">
      <section v-if="activeSection === 'profile'" class="section-card">
        <h3 class="section-title">Mon profil</h3>
        <p v-if="authStore.user">Bienvenue, <strong>{{ authStore.user.username }}</strong></p>
        <p>Rôle : {{ authStore.user?.role }}</p>
      </section>

      <section v-if="activeSection === 'sell'" class="section-card">
        <h3 class="section-title">Mettre un bien en vente</h3>
        <p class="section-desc">
          Décrivez votre bien et soumettez-le à notre équipe d'agents.
        </p>
        <router-link to="/client/sell" class="btn-action">Commencer</router-link>
      </section>

      <section v-if="activeSection === 'wishlist'" class="section-card">
        <h3 class="section-title">Mes favoris</h3>
        <p class="text-muted">Aucun favori pour le moment.</p>
      </section>

      <section v-if="activeSection === 'offers'" class="section-card">
        <h3 class="section-title">Mes offres</h3>
        <p class="text-muted">Aucune offre pour le moment.</p>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'

const router = useRouter()
const authStore = useAuthentificationStore()

const activeSection = ref('profile')

const navItems = [
  { id: 'profile', label: 'Profil' },
  { id: 'sell', label: 'Vendre' },
  { id: 'wishlist', label: 'Favoris' },
  { id: 'offers', label: 'Mes offres' },
]

function handleLogout() {
  authStore.logout()
  router.push('/')
}
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

.logout-btn {
  margin-top: auto;
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
}

.section-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  border: 1px solid #e2e8f0;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1e2956;
  margin-bottom: 0.75rem;
}

.section-desc {
  color: #64748b;
  margin-bottom: 1rem;
}

.btn-action {
  display: inline-block;
  padding: 0.625rem 1.25rem;
  background: #1e2956;
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  text-decoration: none;
  transition: background 0.2s;
}

.btn-action:hover {
  background: #3b4a8a;
}

.text-muted {
  color: #94a3b8;
}
</style>
