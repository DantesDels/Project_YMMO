<template>
  <div class="layout-wrapper">
    <header class="header">
      <div class="container header-content">
        <router-link to="/" class="logo-link">
          <img src="/favicon.svg" alt="Logo YMMO" class="favicon" />
          <span class="logo-text">YMMO</span>
        </router-link>

        <HamburgerMenu v-slot="{ close }">
          <AppButton to="/catalog" @click="close">Catalogue</AppButton>
          <AppButton to="/informations" @click="close">À propos</AppButton>
          <hr class="mobile-divider" />
          <template v-if="isAuthenticated">
            <router-link v-if="isAgent" to="/agent/dashboard" class="mobile-item" @click="close">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>
              Profil Agent
            </router-link>
            <router-link v-else to="/client/dashboard" class="mobile-item" @click="close">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>
              Profil
            </router-link>
            <router-link v-if="isAgent" to="/portfolio/new" class="mobile-item" @click="close">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
              Ajouter un bien
            </router-link>
            <router-link v-if="!isAgent" to="/client/favorites" class="mobile-item" @click="close">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/></svg>
              Favoris
              <span v-if="wishlist.count > 0" class="drop-badge" aria-label="Nombre de favoris">{{ wishlist.count }}</span>
            </router-link>
            <button class="mobile-item mobile-logout" @click="handleLogout">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/><polyline points="16 17 21 12 16 7"/><line x1="21" y1="12" x2="9" y2="12"/></svg>
              Déconnexion
            </button>
          </template>
          <template v-else>
            <AppButton to="/authentification" @click="close">Connexion</AppButton>
          </template>
          <div class="mobile-footer-links">
            <AppButton variant="link" to="/help" @click="close">Aide</AppButton>
            <span class="mobile-underscore" aria-hidden="true">_</span>
            <AppButton variant="link" to="/cgu" @click="close">CGU</AppButton>
            <span class="mobile-underscore" aria-hidden="true">_</span>
            <AppButton variant="link" to="/legal-mentions" @click="close">Mentions légales</AppButton>
            <span class="mobile-underscore" aria-hidden="true">_</span>
            <AppButton variant="link" to="/confidentiality" @click="close">Politique de confidentialité</AppButton>
          </div>
        </HamburgerMenu>

        <nav class="nav-links" aria-label="Navigation principale">
          <AppButton to="/catalog">Catalogue</AppButton>
          <AppButton to="/informations">À propos</AppButton>

          <span class="nav-sep" aria-hidden="true">|</span>

          <template v-if="isAuthenticated">
            <div
              class="user-menu"
              ref="menuRef"
              role="button"
              :aria-haspopup="true"
              :aria-expanded="menuOpen"
              :aria-label="`Menu utilisateur : ${username}`"
              tabindex="0"
              @click.stop="toggleMenu"
              @keydown.enter.prevent="toggleMenu"
              @keydown.space.prevent="toggleMenu"
              @keydown.escape="closeMenu"
            >
              <div class="user-tag" :class="{ 'tag-open': menuOpen }">
                <span class="user-avatar" aria-hidden="true">{{ avatarLetter }}</span>
                <span class="user-name">{{ username }}</span>
                <svg
                  class="chevron-down"
                  :class="{ open: menuOpen }"
                  width="14" height="14" viewBox="0 0 24 24"
                  fill="none" stroke="currentColor" stroke-width="2"
                  aria-hidden="true"
                >
                  <polyline points="6 9 12 15 18 9" />
                </svg>
              </div>
              <Transition name="dropdown">
                <div v-if="menuOpen" class="dropdown-menu" role="menu" :aria-label="`Menu de ${username}`" @click.stop>
                  <router-link
                    v-if="isAgent"
                    to="/agent/dashboard"
                    class="dropdown-item"
                    role="menuitem"
                    tabindex="-1"
                    @click="closeMenu"
                  >
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>
                    Profil Agent
                  </router-link>
                  <router-link
                    v-else
                    to="/client/dashboard"
                    class="dropdown-item"
                    role="menuitem"
                    tabindex="-1"
                    @click="closeMenu"
                  >
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>
                    Profil
                  </router-link>
                  <router-link
                    v-if="!isAgent"
                    to="/client/favorites"
                    class="dropdown-item"
                    role="menuitem"
                    tabindex="-1"
                    @click="closeMenu"
                  >
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/></svg>
                    Favoris
                    <span v-if="wishlist.count > 0" class="drop-badge" aria-label="Nombre de favoris">{{ wishlist.count }}</span>
                  </router-link>
                  <router-link
                    v-if="isAgent"
                    to="/portfolio/new"
                    class="dropdown-item"
                    role="menuitem"
                    tabindex="-1"
                    @click="closeMenu"
                  >
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
                    Ajouter un bien
                  </router-link>
                  <div class="dropdown-divider" role="separator"></div>
                  <button
                    class="dropdown-item dropdown-logout"
                    role="menuitem"
                    tabindex="-1"
                    @click="handleLogout"
                  >
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/><polyline points="16 17 21 12 16 7"/><line x1="21" y1="12" x2="9" y2="12"/></svg>
                    Déconnexion
                  </button>
                </div>
              </Transition>
            </div>
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

    <Teleport to="body">
      <div v-if="wishlist.toastVisible" class="fav-toast">{{ wishlist.toastMessage }}</div>
    </Teleport>

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
import { computed, ref, onMounted, onUnmounted, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'
import { useWishlistStore } from '@/stores/wishlist.store'
import AppButton from '@/components/ui/AppButton.vue'
import HamburgerMenu from '@/components/ui/HamburgerMenu.vue'

const router = useRouter()
const authStore = useAuthentificationStore()
const wishlist = useWishlistStore()

const isAuthenticated = computed(() => !!authStore.user)
const isAgent = computed(() => authStore.user?.role === 'Agent')

const username = computed(() => authStore.user?.username ?? 'User')
const avatarLetter = computed(() => username.value.charAt(0).toUpperCase())

const menuOpen = ref(false)
const menuRef = ref<HTMLElement | null>(null)

function toggleMenu() {
  menuOpen.value = !menuOpen.value
  if (menuOpen.value) {
    nextTick(() => focusItem(0))
  }
}

function closeMenu() {
  menuOpen.value = false
  nextTick(() => menuRef.value?.focus())
}

function focusItem(index: number) {
  const items = menuRef.value?.querySelectorAll<HTMLElement>('[role="menuitem"]')
  if (items && items[index]) items[index].focus()
}

function handleClickOutside(e: MouseEvent) {
  if (menuOpen.value) {
    const target = e.target as HTMLElement
    if (!target.closest('.user-menu')) {
      closeMenu()
    }
  }
}

function handleLogout() {
  closeMenu()
  authStore.logout()
  router.push('/')
}

onMounted(() => document.addEventListener('click', handleClickOutside))
onUnmounted(() => document.removeEventListener('click', handleClickOutside))
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
  padding: 1rem;
  overflow-x: hidden;
}

@media (max-width: 767px) {
  .layout-wrapper { padding: 0.5rem; }
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

.nav-sep {
  color: #cbd5e1;
  font-weight: 300;
}

.user-menu {
  position: relative;
  outline: none;
}

.user-tag {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.35rem 0.75rem;
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

.user-tag.tag-open {
  border-color: #1e2956;
  background: #f1f5f9;
}

.user-avatar {
  width: 26px;
  height: 26px;
  border-radius: 50%;
  background: #1e2956;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 700;
  flex-shrink: 0;
  user-select: none;
}

.user-name {
  font-size: 0.85rem;
  font-weight: 600;
  color: #1e2956;
}

.chevron-down {
  color: #94a3b8;
  transition: transform 0.2s;
}

.chevron-down.open {
  transform: rotate(180deg);
}

.dropdown-menu {
  position: absolute;
  top: calc(100% + 8px);
  right: 0;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.12);
  min-width: 200px;
  padding: 0.5rem;
  z-index: 1000;
}

.dropdown-item {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  padding: 0.6rem 0.75rem;
  border-radius: 8px;
  font-size: 0.9rem;
  font-weight: 600;
  color: #475569;
  text-decoration: none;
  cursor: pointer;
  background: none;
  border: none;
  width: 100%;
  text-align: left;
  transition: all 0.12s;
  font-family: inherit;
}

.dropdown-item:hover {
  background: #f1f5f9;
  color: #1e2956;
}

.dropdown-logout:hover {
  background: #fef2f2;
  color: #dc2626;
}

.dropdown-divider {
  height: 1px;
  background: #e2e8f0;
  margin: 0.3rem 0.5rem;
}

.drop-badge {
  margin-left: auto;
  background: #ef4444;
  color: white;
  font-size: 0.65rem;
  font-weight: 700;
  min-width: 18px;
  height: 18px;
  border-radius: 999px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 4px;
}

.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.15s ease-out;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-4px);
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

.nav-icon-link {
  display: flex;
  align-items: center;
  gap: 0.35rem;
  text-decoration: none;
  color: #475569;
  font-weight: 600;
  font-size: 0.9rem;
  padding: 0.4rem 0.75rem;
  border-radius: 8px;
  transition: all 0.15s;
}

.nav-icon-link:hover {
  background: #f1f5f9;
  color: #1e2956;
}

.heart-icon-wrapper {
  position: relative;
  display: flex;
}

.fav-badge {
  position: absolute;
  top: -6px;
  right: -8px;
  background: #ef4444;
  color: white;
  font-size: 0.65rem;
  font-weight: 700;
  min-width: 16px;
  height: 16px;
  border-radius: 999px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 3px;
  line-height: 1;
}

.mobile-divider {
  border: none;
  border-top: 1px solid #e2e8f0;
  margin: 0.75rem 0;
}

.mobile-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  border-radius: 10px;
  font-size: 1rem;
  font-weight: 600;
  color: #475569;
  text-decoration: none;
  background: none;
  border: none;
  cursor: pointer;
  text-align: left;
  transition: background 0.15s;
  font-family: inherit;
}

.mobile-item:hover {
  background: #f1f5f9;
  color: #1e2956;
}

.mobile-logout:hover {
  background: #fef2f2;
  color: #dc2626;
}

.mobile-footer-links {
  margin-top: auto;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding-top: 0.5rem;
  text-transform: uppercase;
  font-size: 0.75rem;
}

.mobile-underscore {
  color: #d1d5db;
  user-select: none;
  line-height: 1;
}

@media (max-width: 767px) {
  .nav-links { display: none; }
}
</style>

<style>
.fav-toast {
  position: fixed;
  bottom: 2rem;
  left: 50%;
  translate: -50% 0;
  background: #1e2956;
  color: white;
  padding: 0.65rem 1.5rem;
  border-radius: 999px;
  font-size: 0.9rem;
  font-weight: 600;
  z-index: 99999;
  box-shadow: 0 4px 16px rgba(0,0,0,0.2);
  animation: toast-in 0.25s ease-out;
  pointer-events: none;
}
@keyframes toast-in {
  from { opacity: 0; translate: -50% 1rem; }
  to   { opacity: 1; translate: -50% 0; }
}
</style>
