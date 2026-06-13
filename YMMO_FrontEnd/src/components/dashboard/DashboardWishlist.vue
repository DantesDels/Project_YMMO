<template>
  <div>
    <h3 class="section-title">Mes favoris</h3>
    <p class="text-muted" v-if="favorites.length === 0">
      Vous n'avez encore aucun favori. Parcourez le
      <router-link to="/catalog" class="link">catalogue</router-link> pour en ajouter.
    </p>
    <div v-else class="favorites-grid" role="list">
      <div v-for="fav in favorites" :key="fav.id" class="fav-card" role="listitem">
        <img :src="fav.image" :alt="fav.title" class="fav-img" />
        <div class="fav-info">
          <h4>{{ fav.title }}</h4>
          <p class="fav-price">{{ fav.price }} €</p>
          <p class="fav-city">{{ fav.city }}</p>
        </div>
        <button class="fav-remove" @click="removeFavorite(fav.id)" :aria-label="'Retirer ' + fav.title + ' des favoris'">×</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useWishlistStore } from '@/stores/wishlist.store'
import { generateMockProperties } from '@/utils/mockData'

interface Favorite {
  id: string
  title: string
  price: number
  city: string
  image: string
}

const wishlistStore = useWishlistStore()
const allMockProperties = generateMockProperties()

const favorites = computed(() => {
  const ids = wishlistStore.favorites
  if (ids.length === 0) return []
  return allMockProperties
    .filter(p => ids.includes(p.id))
    .map(p => ({
      id: p.id,
      title: p.title,
      price: p.price,
      city: p.address ? p.address.replace(/\(.*\)/, '').trim() : '',
      image: p.image || 'https://placehold.co/400x300/1e2956/ffffff?text=YMMO',
    }))
})

function removeFavorite(id: string) {
  wishlistStore.removeFavorite(id)
}
</script>

<style scoped>
.section-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1e2956;
  margin: 0 0 1.25rem;
}

.text-muted {
  color: #94a3b8;
}

.link {
  color: #1e2956;
  font-weight: 600;
}

.favorites-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: 1rem;
}

.fav-card {
  display: flex;
  gap: 0.75rem;
  padding: 0.75rem;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  position: relative;
  transition: border-color 0.15s;
}

.fav-card:hover,
.fav-card:focus-within {
  border-color: #1e2956;
}

.fav-img {
  width: 100px;
  height: 75px;
  border-radius: 8px;
  object-fit: cover;
  flex-shrink: 0;
}

.fav-info {
  min-width: 0;
  flex: 1;
}

.fav-info h4 {
  margin: 0 0 0.25rem;
  font-size: 0.95rem;
  font-weight: 600;
  color: #1e293b;
  word-break: break-word;
}

.fav-price {
  font-weight: 700;
  color: #1e2956;
  margin: 0 0 0.15rem;
  font-size: 0.9rem;
}

.fav-city {
  color: #94a3b8;
  font-size: 0.8rem;
  margin: 0;
}

.fav-remove {
  position: absolute;
  top: 6px;
  right: 6px;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  border: none;
  background: #fef2f2;
  color: #dc2626;
  font-size: 1.2rem;
  line-height: 1;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.15s, opacity 0.15s;
}

.fav-remove:hover {
  background: #fee2e2;
}

.fav-remove:focus-visible {
  outline: 2px solid #dc2626;
  outline-offset: 2px;
  opacity: 1;
}

@media (max-width: 767px) {
  .favorites-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 480px) {
  .fav-card {
    flex-direction: column;
    align-items: flex-start;
  }

  .fav-img {
    width: 100%;
    height: 140px;
  }
}
</style>
