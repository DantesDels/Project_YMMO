<template>
  <div class="property-card">
    <div class="image-container">
      <img :src="property.image || 'https://via.placeholder.com/300'" :alt="property.title" />
      <div class="badges">
        <span v-if="property.isPromotion" class="badge promo">Promotion en cours</span>
      </div>
    </div>

    <div class="content">
      <h3 class="title">{{ property.title || 'Logement sans titre' }}</h3>
      <p class="location">📍 {{ property.address || 'Adresse non spécifiée' }}</p>
      <p class="specs">
        {{ property.surface || 0 }}m² - {{ property.rooms || 1 }} pièces - {{ property.furnishing || 'Non meublé' }}
      </p>

      <div class="footer">
        <span class="price">à partir de <strong>{{ property.price || 0 }}€</strong> / mois</span>
        <div class="availability text-emerald-500">
          ● Disponible {{ property.availabilityDate || 'immédiatement' }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  property: { type: Object, required: true }
});
</script>

<style scoped>
.property-card { background: white; border-radius: 16px; overflow: hidden; border: 1px solid #e2e8f0; transition: transform 0.2s; }
.property-card:hover { transform: translateY(-5px); box-shadow: 0 10px 15px -3px rgba(0,0,0,0.1); }
.image-container { height: 200px; position: relative; }
.image-container img { width: 100%; height: 100%; object-fit: cover; }
.content { padding: 1.25rem; }
.title { font-weight: 700; color: #1e2956; margin-bottom: 0.5rem; }
.location, .specs { font-size: 0.85rem; color: #64748b; }
.price strong { color: #1e2956; font-size: 1.1rem; }
</style>