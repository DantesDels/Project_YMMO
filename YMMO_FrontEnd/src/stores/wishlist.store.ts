import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

const STORAGE_KEY = 'ymmo_wishlist'

function loadWishlist(): string[] {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    return raw ? JSON.parse(raw) : []
  } catch {
    return []
  }
}

function saveWishlist(ids: string[]) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(ids))
}

export const useWishlistStore = defineStore('wishlist', () => {
  const favoriteIds = ref<string[]>(loadWishlist())
  const toastMessage = ref('')
  const toastVisible = ref(false)
  let toastTimer: ReturnType<typeof setTimeout> | null = null

  const favorites = computed(() => favoriteIds.value)
  const count = computed(() => favoriteIds.value.length)

  function isFavorite(id: string): boolean {
    return favoriteIds.value.includes(id)
  }

  function showToast(msg: string) {
    if (toastTimer) clearTimeout(toastTimer)
    toastMessage.value = msg
    toastVisible.value = true
    toastTimer = setTimeout(() => {
      toastVisible.value = false
      toastTimer = null
    }, 2000)
  }

  function toggleFavorite(id: string) {
    const idx = favoriteIds.value.indexOf(id)
    if (idx === -1) {
      favoriteIds.value.push(id)
      showToast('Ajouté aux favoris')
    } else {
      favoriteIds.value.splice(idx, 1)
      showToast('Retiré des favoris')
    }
    saveWishlist(favoriteIds.value)
  }

  function removeFavorite(id: string) {
    const idx = favoriteIds.value.indexOf(id)
    if (idx !== -1) {
      favoriteIds.value.splice(idx, 1)
      saveWishlist(favoriteIds.value)
    }
  }

  function clear() {
    favoriteIds.value = []
    saveWishlist([])
  }

  return { favorites, count, isFavorite, toggleFavorite, removeFavorite, clear, toastMessage, toastVisible }
})
