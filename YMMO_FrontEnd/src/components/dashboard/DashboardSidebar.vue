<template>
  <aside class="sidebar" :class="{ 'sidebar-open': mobileNavOpen }">
    <div class="sidebar-header">
      <h2 class="sidebar-title">{{ title }}</h2>
      <button class="sidebar-close" @click="$emit('close')" aria-label="Fermer la navigation">×</button>
    </div>
    <nav class="sidebar-nav" role="tablist" aria-label="Sections du compte" @keydown="onTabKeydown">
      <button
        v-for="item in navItems"
        :key="item.id"
        :id="`tab-${item.id}`"
        role="tab"
        :aria-selected="activeSection === item.id"
        :aria-controls="`panel-${item.id}`"
        :tabindex="activeSection === item.id ? 0 : -1"
        :class="['nav-btn', { active: activeSection === item.id }]"
        @click="$emit('switchSection', item.id)"
      >
        {{ item.label }}
      </button>
      <div class="sidebar-separator" role="separator"></div>
      <button
        v-for="item in navItemsBottom"
        :key="item.id"
        :id="`tab-${item.id}`"
        role="tab"
        :aria-selected="activeSection === item.id"
        :aria-controls="`panel-${item.id}`"
        :tabindex="activeSection === item.id ? 0 : -1"
        :class="['nav-btn', { active: activeSection === item.id }]"
        @click="$emit('switchSection', item.id)"
      >
        {{ item.label }}
      </button>
    </nav>
    <div class="sidebar-footer">
      <router-link to="/catalog" class="btn-public-site">Catalogue</router-link>
      <router-link to="/" class="btn-public-site">← Site public</router-link>
      <button class="logout-btn" @click="$emit('logout')">Déconnexion</button>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface NavItem {
  id: string
  label: string
}

const props = withDefaults(defineProps<{
  activeSection: string
  mobileNavOpen: boolean
  navItems?: NavItem[]
  navItemsBottom?: NavItem[]
  title?: string
}>(), {
  navItems: () => [
    { id: 'profile', label: 'Profil' },
    { id: 'wishlist', label: 'Mes favoris' },
  ],
  navItemsBottom: () => [
    { id: 'offers', label: 'Mes offres' },
    { id: 'sell', label: 'Mes ventes' },
  ],
  title: 'Mon compte',
})

defineEmits<{
  (e: 'switchSection', id: string): void
  (e: 'close'): void
  (e: 'logout'): void
}>()

const allNavItems = computed(() => [...props.navItems, ...props.navItemsBottom])

function onTabKeydown(e: KeyboardEvent) {
  const items = allNavItems.value
  const idx = items.findIndex(i => i.id === props.activeSection)
  let next = idx

  switch (e.key) {
    case 'ArrowDown':
    case 'ArrowRight':
      e.preventDefault()
      next = (idx + 1) % items.length
      break
    case 'ArrowUp':
    case 'ArrowLeft':
      e.preventDefault()
      next = (idx - 1 + items.length) % items.length
      break
    case 'Home':
      e.preventDefault()
      next = 0
      break
    case 'End':
      e.preventDefault()
      next = items.length - 1
      break
    default:
      return
  }

  const tab = document.getElementById(`tab-${items[next].id}`)
  tab?.focus()
}
</script>

<style scoped>
.sidebar {
  width: 240px;
  min-width: 240px;
  background: white;
  border-right: 1px solid #e2e8f0;
  padding: 1.5rem 1rem;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.sidebar-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 0.75rem;
}

.sidebar-close {
  display: none;
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #64748b;
  cursor: pointer;
  padding: 0.25rem;
  line-height: 1;
  border-radius: 4px;
}

.sidebar-close:focus-visible {
  outline: 2px solid #1e2956;
  outline-offset: 2px;
}

.sidebar-title {
  font-size: 1.15rem;
  font-weight: 700;
  color: #1e2956;
  margin: 0;
}

.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  flex: 1;
}

.nav-btn {
  text-align: left;
  padding: 0.65rem 0.875rem;
  border: none;
  background: transparent;
  border-radius: 8px;
  font-size: 0.95rem;
  font-weight: 500;
  color: #475569;
  cursor: pointer;
  transition: all 0.15s;
  min-height: 44px;
}

.nav-btn:hover {
  background: #f1f5f9;
  color: #1e2956;
}

.nav-btn:focus-visible {
  outline: 2px solid #1e2956;
  outline-offset: 2px;
}

.nav-btn.active {
  background: #1e2956;
  color: white;
  font-weight: 600;
}

.sidebar-separator {
  height: 1px;
  background: #e2e8f0;
  margin: 0.5rem 0.875rem;
}

.sidebar-footer {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-top: auto;
  padding-top: 0.5rem;
}

.btn-public-site {
  padding: 0.5rem 0.875rem;
  font-size: 0.85rem;
  color: #64748b;
  text-decoration: none;
  border-radius: 8px;
  transition: all 0.15s;
  min-height: 44px;
  display: flex;
  align-items: center;
}

.btn-public-site:hover {
  background: #f1f5f9;
  color: #1e2956;
}

.btn-public-site:focus-visible {
  outline: 2px solid #1e2956;
  outline-offset: 2px;
}

.logout-btn {
  padding: 0.625rem;
  border: 1px solid #e2e8f0;
  background: white;
  border-radius: 8px;
  font-size: 0.9rem;
  color: #dc2626;
  cursor: pointer;
  transition: all 0.15s;
  min-height: 44px;
}

.logout-btn:hover {
  background: #fef2f2;
  border-color: #fca5a5;
}

.logout-btn:focus-visible {
  outline: 2px solid #dc2626;
  outline-offset: 2px;
}

@media (max-width: 1024px) {
  .sidebar {
    width: 200px;
    min-width: 200px;
    padding: 1.25rem 0.75rem;
  }
}

@media (max-width: 767px) {
  .sidebar {
    position: fixed;
    top: 0;
    left: 0;
    bottom: 0;
    z-index: 1000;
    width: 280px;
    min-width: auto;
    transform: translateX(-100%);
    transition: transform 0.25s ease;
    box-shadow: none;
    border-right: 1px solid #e2e8f0;
  }

  .sidebar.sidebar-open {
    transform: translateX(0);
    box-shadow: 4px 0 24px rgba(0,0,0,0.15);
  }

  .sidebar-close {
    display: block;
  }
}
</style>
