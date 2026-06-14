<template>
  <section class="section-wrapper bg-white">
    <button
      class="toggle-header"
      @click="toggle"
      :aria-expanded="isOpen"
      aria-controls="city-grid-panel"
    >
      <h2 class="section-title">
        Trouvez votre bien dans l'une de ces villes
      </h2>
      <svg
        class="chevron"
        :class="{ open: isOpen }"
        width="20"
        height="20"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="2"
        aria-hidden="true"
      >
        <polyline points="6 9 12 15 18 9" />
      </svg>
    </button>

    <Transition name="city-toggle">
      <div v-if="isOpen" id="city-grid-panel" class="property-grid-cities">
        <div
          v-for="city in cities"
          :key="city"
          class="city-card"
          role="button"
          tabindex="0"
          :aria-label="`Voir les annonces à ${city}`"
          @click="goToCity(city)"
          @keydown.enter="goToCity(city)"
          @keydown.space.prevent="goToCity(city)"
        >
          <img
            :src="getCityImageUrl(city)"
            :alt="city"
            class="city-img"
            loading="lazy"
            width="281"
            height="128"
            @error="handleImageError"
          />
          <div class="city-overlay">
            <span class="city-name">{{ city }}</span>
          </div>
        </div>
      </div>
    </Transition>
  </section>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { getCityImageUrl } from '../utils/imageLoader';

const router = useRouter()
const isOpen = ref(true)

const cities = ['Paris', 'Marseille', 'Lyon', 'Toulouse', 'Nice', 'Nantes', 'Montpellier', 'Strasbourg', 'Bordeaux', 'Lille', 'Rennes', 'Reims', 'Le Havre', 'Saint-Étienne', 'Toulon']

function toggle() { isOpen.value = !isOpen.value }

const handleImageError = (event) => {
  event.target.src = '/images/ui/default-city.webp'
}

const goToCity = (city) => {
  router.push({ name: 'catalog', query: { city } })
}
</script>

<style scoped>
.section-wrapper {
  padding: 2rem 0;
}

.toggle-header {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  width: 100%;
  background: none;
  border: none;
  cursor: pointer;
  padding: 0.5rem 1rem;
  font-family: inherit;
}

.section-title {
  margin: 0;
  color: #1e2956;
}

.chevron {
  color: #94a3b8;
  transition: transform 0.25s ease;
  flex-shrink: 0;
}

.chevron.open {
  transform: rotate(180deg);
}

.property-grid-cities {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 1rem;
  max-width: 1200px;
  margin: 1.5rem auto 0;
  padding: 0 1rem;
}

.city-card {
  position: relative;
  height: 128px;
  width: calc(20% - 0.8rem);
  min-width: 150px;
  max-width: 100%;
  border-radius: 12px;
  overflow: hidden;
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

@media (max-width: 480px) {
  .city-card { height: 100px; min-width: 130px; }
}

@media (max-width: 360px) {
  .city-card { height: 90px; min-width: 110px; }
}

.city-card:focus-visible {
  outline: 2px solid var(--color-secondary, #004ecc);
  outline-offset: 2px;
}

.city-card:hover {
  transform: scale(1.04);
  box-shadow: 0 8px 20px rgba(30,41,86,0.25);
}

.city-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center;
}

.city-overlay {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0,0,0,0.3);
  pointer-events: none;
}

.city-name {
  font-weight: 700;
  font-size: 1.125rem;
  color: white;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.5);
  background: rgba(30, 41, 86, 0.7);
  padding: 0.25rem 1rem;
  border-radius: 0.5rem;
  backdrop-filter: blur(4px);
  transition: all 0.3s ease;
}

.city-card:hover .city-name {
  background: rgba(30, 41, 86, 0.9);
  transform: scale(1.05);
  box-shadow: 3px 4px 12px rgba(30, 41, 86, 0.6);
}

.city-toggle-enter-active,
.city-toggle-leave-active {
  transition: all 0.25s ease;
  overflow: hidden;
}

.city-toggle-enter-from,
.city-toggle-leave-to {
  opacity: 0;
  max-height: 0;
  margin-top: 0;
}

.city-toggle-enter-to,
.city-toggle-leave-from {
  opacity: 1;
}
</style>
