<template>
  <div class="dash-wrapper">
    <div class="dash-container">
      <DashboardSidebar
        :active-section="activeSection"
        :mobile-nav-open="mobileNavOpen"
        @switch-section="switchSection"
        @close="mobileNavOpen = false"
        @logout="handleLogout"
      />

      <button
        class="mobile-nav-toggle"
        @click="mobileNavOpen = !mobileNavOpen"
        aria-label="Ouvrir la navigation"
      >
        <span class="hamburger-line"></span>
        <span class="hamburger-line"></span>
        <span class="hamburger-line"></span>
      </button>

      <main class="main" role="main">
        <div aria-live="polite" aria-atomic="true" class="sr-only">{{ statusMessage }}</div>

        <section v-if="activeSection === 'profile'" id="panel-profile" class="section-card" role="tabpanel" aria-label="Mon profil" tabindex="0">
          <DashboardProfile />
        </section>

        <section v-if="activeSection === 'sell'" id="panel-sell" class="section-card" role="tabpanel" aria-label="Mes ventes" tabindex="0">
          <DashboardSell />
        </section>

        <section v-if="activeSection === 'wishlist'" id="panel-wishlist" class="section-card" role="tabpanel" aria-label="Mes favoris" tabindex="0">
          <DashboardWishlist />
        </section>

        <section v-if="activeSection === 'offers'" id="panel-offers" class="section-card" role="tabpanel" aria-label="Mes offres" tabindex="0">
          <DashboardOffers />
        </section>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'
import DashboardSidebar from '@/components/dashboard/DashboardSidebar.vue'
import DashboardProfile from '@/components/dashboard/DashboardProfile.vue'
import DashboardSell from '@/components/dashboard/DashboardSell.vue'
import DashboardWishlist from '@/components/dashboard/DashboardWishlist.vue'
import DashboardOffers from '@/components/dashboard/DashboardOffers.vue'

const router = useRouter()
const route = useRoute()
const authStore = useAuthentificationStore()

const activeSection = ref('profile')
const mobileNavOpen = ref(false)
const statusMessage = ref('')

const allNavItems = computed(() => [
  { id: 'profile', label: 'Profil' },
  { id: 'wishlist', label: 'Mes favoris' },
  { id: 'offers', label: 'Mes offres' },
  { id: 'sell', label: 'Mes ventes' },
])

function switchSection(id: string) {
  activeSection.value = id
  mobileNavOpen.value = false
  const label = allNavItems.value.find(i => i.id === id)?.label ?? id
  statusMessage.value = `Section ${label} affichée`
}

watch(() => route.query.section, (val) => {
  if (val && allNavItems.value.some(n => n.id === val)) {
    activeSection.value = val as string
  }
})

function handleLogout() {
  authStore.logout()
  router.push('/')
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
  min-height: calc(100vh - 150px);
  position: relative;
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
  border: none;
  cursor: pointer;
  box-shadow: 0 4px 16px rgba(0,0,0,0.25);
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 5px;
}

.mobile-nav-toggle:focus-visible {
  outline: 3px solid #1e2956;
  outline-offset: 3px;
}

.hamburger-line {
  display: block;
  width: 22px;
  height: 3px;
  background: white;
  border-radius: 2px;
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
  width: 100%;
  box-sizing: border-box;
}

.section-card:focus-visible {
  outline: 2px solid #1e2956;
  outline-offset: 2px;
}

@media (max-width: 1024px) {
  .main {
    padding: 1.5rem;
  }

  .section-card {
    padding: 1.25rem;
  }
}

@media (max-width: 767px) {
  .mobile-nav-toggle {
    display: flex;
  }

  .main {
    padding: 1rem;
  }

  .section-card {
    padding: 1rem;
    border-radius: 10px;
  }
}

@media (max-width: 480px) {
  .main {
    padding: 0.75rem;
  }

  .section-card {
    padding: 0.875rem;
  }
}

@media (min-width: 1281px) {
  .dash-wrapper {
    padding: 0;
  }
}
</style>
