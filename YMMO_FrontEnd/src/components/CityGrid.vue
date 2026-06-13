<template>
  <section class="section-wrapper bg-white">
    <h2 class="section-title text-center">
      Trouvez votre bien dans l'une de ces villes
    </h2>

    <div class="property-grid-cities">
      <div v-for="city in cities" :key="city" class="city-card">
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
  </section>
</template>

<script setup>
import { getCityImageUrl } from '../utils/imageLoader';

const cities = ['Paris', 'Bordeaux', 'Lille', 'Lyon', 'Toulouse', 'Marseille', 'Nantes', 'Montpellier', 'Rennes', 'Grenoble'];

const handleImageError = (event) => {
  event.target.src = '/images/ui/default-city.webp';
};
</script>

<style scoped>
.section-wrapper {
  padding: 2rem 0;
}

.section-title {
  margin-bottom: 2rem;
  color: #1e2956;
}

.property-grid-cities {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 1rem;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

.city-card {
  position: relative;
  height: 128px;
  width: calc(20% - 0.8rem);
  min-width: 150px;
  border-radius: 12px;
  overflow: hidden;
  display: flex;
  justify-content: center;
  align-items: center;
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
</style>