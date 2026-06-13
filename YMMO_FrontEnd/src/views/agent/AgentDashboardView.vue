<template>
  <div class="dashboard">
    <aside class="sidebar">
      <h2 class="sidebar-title">Espace Agent</h2>
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
      <!-- Profil -->
      <section v-if="activeSection === 'profile'" class="section-card">
        <h3 class="section-title">Mon profil</h3>
        <p v-if="authStore.user">Bienvenue, <strong>{{ authStore.user.username }}</strong></p>
        <p>Rôle : {{ authStore.user?.role }}</p>
      </section>

      <!-- Mes biens -->
      <section v-if="activeSection === 'properties'" class="section-card">
        <div class="section-header">
          <h3 class="section-title">Mes biens</h3>
          <router-link to="/portfolio/new" class="btn-sm">+ Ajouter</router-link>
        </div>
        <div v-if="loading" class="text-muted">Chargement...</div>
        <div v-else-if="myProperties.length === 0" class="text-muted">Aucun bien pour le moment.</div>
        <div v-else class="property-list">
          <div v-for="p in myProperties" :key="p.propertyId" class="property-row">
            <div class="prop-info">
              <strong>{{ p.propertyName }}</strong>
              <span class="prop-meta">{{ p.city }} - {{ formatPrice(p.currentPrice) }}</span>
            </div>
            <span class="badge-type">{{ p.propertyType }}</span>
          </div>
        </div>
      </section>

      <!-- Demandes clients -->
      <section v-if="activeSection === 'requests'" class="section-card">
        <h3 class="section-title">Demandes de vente</h3>
        <p class="text-muted">Aucune demande en attente.</p>
      </section>

      <!-- Offres -->
      <section v-if="activeSection === 'offers'" class="section-card">
        <h3 class="section-title">Offres reçues</h3>
        <p class="text-muted">Aucune offre pour le moment.</p>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'
import { propertyService } from '@/services/property.service'
import type { PropertySummaryDto } from '@/types'

const router = useRouter()
const authStore = useAuthentificationStore()

const activeSection = ref('profile')
const myProperties = ref<PropertySummaryDto[]>([])
const loading = ref(false)

const navItems = [
  { id: 'profile', label: 'Profil' },
  { id: 'properties', label: 'Mes biens' },
  { id: 'requests', label: 'Demandes clients' },
  { id: 'offers', label: 'Offres' },
]

onMounted(async () => {
  if (authStore.user?.role === 'Agent') {
    loading.value = true
    try {
      myProperties.value = await propertyService.getAll()
    } catch {
      myProperties.value = []
    } finally {
      loading.value = false
    }
  }
})

function handleLogout() {
  authStore.logout()
  router.push('/')
}

function formatPrice(price: number): string {
  return new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR' }).format(price)
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

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1e2956;
  margin: 0;
}

.btn-sm {
  padding: 0.375rem 0.875rem;
  background: #1e2956;
  color: white;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 600;
  text-decoration: none;
  transition: background 0.2s;
}

.btn-sm:hover {
  background: #3b4a8a;
}

.text-muted {
  color: #94a3b8;
}

.property-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.property-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background: #f8fafc;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
}

.prop-info {
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
}

.prop-meta {
  font-size: 0.85rem;
  color: #64748b;
}

.badge-type {
  font-size: 0.75rem;
  font-weight: 600;
  background: #eef2ff;
  color: #4338ca;
  padding: 0.25rem 0.625rem;
  border-radius: 999px;
}
</style>
