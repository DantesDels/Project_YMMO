<template>
  <div class="dash-wrapper">
    <button
      class="mobile-nav-toggle"
      @click="mobileNavOpen = !mobileNavOpen"
      :aria-label="mobileNavOpen ? 'Fermer la navigation' : 'Ouvrir la navigation'"
      :aria-expanded="mobileNavOpen"
    >
      <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
        <line x1="3" y1="6" x2="21" y2="6" />
        <line x1="3" y1="12" x2="21" y2="12" />
        <line x1="3" y1="18" x2="21" y2="18" />
      </svg>
    </button>

    <div class="dash-container">
      <DashboardSidebar
        :activeSection="activeSection"
        :mobileNavOpen="mobileNavOpen"
        :navItems="agentNavItems"
        :navItemsBottom="agentNavItemsBottom"
        title="Espace Agent"
        @switch-section="switchSection"
        @close="mobileNavOpen = false"
        @logout="handleLogout"
      />

      <main class="main" role="main">
        <div aria-live="polite" class="sr-only">{{ statusMessage }}</div>

        <!-- Profil -->
        <section v-if="activeSection === 'profile'" id="panel-profile" class="section-card" role="tabpanel" aria-labelledby="tab-profile">
          <h3 class="section-title">Mon profil</h3>
          <div class="profile-info">
            <div class="profile-avatar">{{ avatarLetter }}</div>
            <div>
              <p class="profile-name" v-if="authStore.user"><strong>{{ authStore.user.username }}</strong></p>
              <p class="profile-role">{{ roleLabel }}</p>
            </div>
          </div>
        </section>

        <!-- Mes biens -->
        <section v-if="activeSection === 'wishlist'" id="panel-wishlist" class="section-card" role="tabpanel" aria-labelledby="tab-wishlist">
          <div class="section-header">
            <h3 class="section-title">Mes biens</h3>
            <router-link to="/portfolio/new" class="btn-sm">+ Ajouter un bien</router-link>
          </div>
          <div v-if="loading" class="text-muted">Chargement...</div>
          <div v-else-if="properties.length === 0" class="text-muted">Aucun bien pour le moment.</div>
          <div v-else class="property-list">
            <div v-for="p in properties" :key="p.id" class="property-row">
              <div class="prop-info">
                <strong>{{ p.title || p.propertyName }}</strong>
                <span class="prop-meta">{{ p.city || (p.address || '').replace(/\(.*\)/, '').trim() }} — {{ formatPrice(p.price || p.currentPrice) }}</span>
              </div>
              <span class="badge-type">{{ p.type || p.propertyType }}</span>
            </div>
          </div>
        </section>

        <!-- Demandes clients -->
        <section v-if="activeSection === 'offers'" id="panel-offers" class="section-card" role="tabpanel" aria-labelledby="tab-offers">
          <h3 class="section-title">Demandes de vente</h3>
          <p class="text-muted">Aucune demande en attente.</p>
        </section>

        <!-- Offres -->
        <section v-if="activeSection === 'sell'" id="panel-sell" class="section-card" role="tabpanel" aria-labelledby="tab-sell">
          <h3 class="section-title">Offres reçues</h3>
          <p class="text-muted">Aucune offre pour le moment.</p>
        </section>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'
import { useCustomProperties } from '@/stores/customProperties.store'
import { generateMockProperties } from '@/utils/mockData'
import DashboardSidebar from '@/components/dashboard/DashboardSidebar.vue'

const router = useRouter()
const authStore = useAuthentificationStore()
const { getAll: getCustomProperties } = useCustomProperties()

const agentNavItems = [
  { id: 'profile', label: 'Profil' },
  { id: 'wishlist', label: 'Mes biens' },
]

const agentNavItemsBottom = [
  { id: 'offers', label: 'Demandes clients' },
  { id: 'sell', label: 'Offres reçues' },
]

const activeSection = ref('profile')
const mobileNavOpen = ref(false)
const loading = ref(false)

const statusMessage = computed(() => {
  const labels: Record<string, string> = {
    profile: 'Profil affiché',
    wishlist: 'Mes biens affichés',
    offers: 'Demandes clients affichées',
    sell: 'Offres reçues affichées',
  }
  return labels[activeSection.value] || ''
})

const roleLabel = computed(() => {
  switch (authStore.user?.role) {
    case 'Agent': return 'Agent immobilier'
    case 'Admin': return 'Administrateur'
    default: return 'Agent'
  }
})

const avatarLetter = computed(() => (authStore.user?.username ?? 'A').charAt(0).toUpperCase())

const properties = computed(() => {
  const custom = getCustomProperties()
  const mock = generateMockProperties(400)
  const agentId = authStore.user?.contactId
  const agentName = authStore.user?.username
  const all = [...custom, ...mock]
  return all.filter(p => {
    if (agentId && p.agentId === agentId) return true
    if (agentName && (p.agentName === agentName || p.agentName?.includes(agentName))) return true
    return false
  })
})

function switchSection(id: string) {
  activeSection.value = id
  mobileNavOpen.value = false
}

function handleLogout() {
  authStore.logout()
  router.push('/')
}

function formatPrice(val: number): string {
  return new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(val || 0)
}
</script>

<style scoped>
.dash-wrapper {
  background: #f8fafc;
  min-height: calc(100vh - 150px);
}

.dash-container {
  display: flex;
  max-width: 1280px;
  margin: 0 auto;
}

.mobile-nav-toggle {
  display: none;
  position: fixed;
  bottom: 1.5rem;
  right: 1.5rem;
  z-index: 999;
  width: 52px;
  height: 52px;
  border-radius: 50%;
  background: #1e2956;
  color: white;
  border: none;
  cursor: pointer;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 16px rgba(30,41,86,0.3);
  transition: transform 0.15s;
}

.mobile-nav-toggle:hover {
  transform: scale(1.08);
}

.mobile-nav-toggle:focus-visible {
  outline: 2px solid #1e2956;
  outline-offset: 4px;
}

.main {
  flex: 1;
  padding: 2rem;
  overflow-y: auto;
  min-width: 0;
}

.section-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  border: 1px solid #e2e8f0;
  max-width: 920px;
  margin: 0 auto;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.25rem;
  flex-wrap: wrap;
  gap: 0.75rem;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1e2956;
  margin: 0;
}

.btn-sm {
  padding: 0.5rem 1rem;
  background: #1e2956;
  color: white;
  border-radius: 8px;
  font-size: 0.85rem;
  font-weight: 600;
  text-decoration: none;
  transition: background 0.2s;
  display: inline-flex;
  align-items: center;
  gap: 0.35rem;
}

.btn-sm:hover {
  background: #3b4a8a;
}

.text-muted {
  color: #94a3b8;
}

.profile-info {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-top: 0.5rem;
}

.profile-avatar {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background: #1e2956;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1.35rem;
  flex-shrink: 0;
}

.profile-name {
  font-size: 1.1rem;
  color: #1e2956;
  margin: 0;
}

.profile-role {
  color: #64748b;
  font-size: 0.9rem;
  margin: 0.125rem 0 0;
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
  gap: 0.75rem;
}

.prop-info {
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
  min-width: 0;
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
  white-space: nowrap;
  flex-shrink: 0;
}

.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0,0,0,0);
  white-space: nowrap;
  border: 0;
}

@media (max-width: 767px) {
  .mobile-nav-toggle {
    display: flex;
  }

  .main {
    padding: 1.25rem 1rem;
  }

  .section-card {
    padding: 1.25rem;
  }

  .profile-info {
    flex-direction: column;
    text-align: center;
  }
}
</style>
