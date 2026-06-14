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
          <div v-else-if="customAgentProperties.length === 0 && mockAgentProperties.length === 0" class="text-muted">Aucun bien pour le moment.</div>
          <div v-else class="property-list">
            <router-link v-for="p in customAgentProperties" :key="p.id" :to="`/property/${p.id}`" custom v-slot="{ navigate }">
              <div class="property-row clickable" @click="navigate" @keydown.enter="navigate" role="link" tabindex="0">
                <div class="prop-info">
                  <strong>{{ p.title }}</strong>
                  <span class="prop-meta">{{ (p.address || '').replace(/\(.*\)/, '').trim() }} — {{ formatPrice(p.price) }}</span>
                </div>
                <div class="prop-actions">
                  <span class="badge-custom">Créé</span>
                  <router-link :to="`/portfolio/edit/${p.id}`" class="btn-edit" @click.stop>Modifier</router-link>
                  <button v-if="deleteConfirmId !== p.id" class="btn-delete" @click.stop="confirmDelete(p.id)" aria-label="Supprimer ce bien">Supprimer</button>
                  <span v-else class="confirm-group">
                    <button class="btn-confirm-yes" @click.stop="executeDelete(p.id)" aria-label="Confirmer la suppression">Oui</button>
                    <button class="btn-confirm-no" @click.stop="cancelDelete()" aria-label="Annuler">Non</button>
                  </span>
                </div>
              </div>
            </router-link>
            <router-link v-for="p in mockAgentProperties" :key="p.id" :to="`/property/${p.id}`" custom v-slot="{ navigate }">
              <div class="property-row clickable" @click="navigate" @keydown.enter="navigate" role="link" tabindex="0">
                <div class="prop-info">
                  <strong>{{ p.title }}</strong>
                  <span class="prop-meta">{{ (p.address || '').replace(/\(.*\)/, '').trim() }} — {{ formatPrice(p.price) }}</span>
                </div>
                <span class="badge-type">{{ p.type }}</span>
              </div>
            </router-link>
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
const { getAll: getCustomProperties, removeProperty: removeCustomProperty } = useCustomProperties()

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

const deleteConfirmId = ref<string | null>(null)

const customAgentProperties = computed(() => {
  const all = getCustomProperties()
  const agentId = authStore.user?.contactId
  const agentName = authStore.user?.username
  return all.filter(p => {
    if (agentId && p.agentId === agentId) return true
    if (agentName && (p.agentName === agentName || p.agentName?.includes(agentName))) return true
    return false
  })
})

const mockAgentProperties = computed(() => {
  const all = generateMockProperties(400)
  const agentId = authStore.user?.contactId
  const agentName = authStore.user?.username
  return all.filter(p => {
    if (agentId && p.agentId === agentId) return true
    if (agentName && (p.agentName === agentName || p.agentName?.includes(agentName))) return true
    return false
  })
})

function confirmDelete(id: string) {
  deleteConfirmId.value = id
}

function cancelDelete() {
  deleteConfirmId.value = null
}

function executeDelete(id: string) {
  removeCustomProperty(id)
  deleteConfirmId.value = null
}

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

.property-row.clickable {
  cursor: pointer;
  transition: border-color 0.15s, background 0.15s;
  text-decoration: none;
  color: inherit;
}
.property-row.clickable:hover {
  border-color: #1e2956;
  background: #eef2ff;
}
.property-row.clickable:focus-visible {
  outline: 2px solid #1e2956;
  outline-offset: 2px;
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

.prop-actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-shrink: 0;
}

.badge-custom {
  font-size: 0.7rem;
  font-weight: 700;
  background: #dcfce7;
  color: #166534;
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  white-space: nowrap;
}

.btn-edit {
  font-size: 0.75rem;
  padding: 0.3rem 0.625rem;
  background: #eef2ff;
  color: #4338ca;
  border: 1px solid #c7d2fe;
  border-radius: 6px;
  font-weight: 600;
  text-decoration: none;
  transition: background 0.15s;
}
.btn-edit:hover {
  background: #e0e7ff;
}
.btn-delete {
  font-size: 0.75rem;
  padding: 0.3rem 0.625rem;
  background: #fef2f2;
  color: #dc2626;
  border: 1px solid #fecaca;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: background 0.15s;
}

.btn-delete:hover {
  background: #fee2e2;
}

.confirm-group {
  display: flex;
  gap: 0.35rem;
  align-items: center;
}

.btn-confirm-yes {
  font-size: 0.75rem;
  padding: 0.3rem 0.625rem;
  background: #dc2626;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 700;
}

.btn-confirm-yes:hover {
  background: #b91c1c;
}

.btn-confirm-no {
  font-size: 0.75rem;
  padding: 0.3rem 0.625rem;
  background: #e2e8f0;
  color: #475569;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
}

.btn-confirm-no:hover {
  background: #cbd5e1;
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
