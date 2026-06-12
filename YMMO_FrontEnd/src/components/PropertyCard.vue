<template>
  <div class="property-card">
    <div class="image-container">
      <img :src="property.image" :alt="property.title" />
      <div class="badges">
        <span v-if="property.isPromotion" class="badge promo">Promotion en cours</span>
        <span v-if="property.is360" class="badge-icon">360°</span>
      </div>
    </div>

    <div class="content">
      <h3 class="title">{{ property.title }}</h3>
      <p class="location">📍 {{ property.address }}</p>
      <p class="specs">{{ property.surface }}m² - {{ property.rooms }} chambres - {{ property.furnishing }}</p>

      <div class="footer">
        <span class="price">à partir de <strong>{{ property.price }}€</strong> / mois</span>
        <div class="availability" :class="property.available ? 'text-emerald-500' : 'text-slate-500'">
          ● Disponible {{ property.availabilityDate }}
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
.badges { position: absolute; top: 10px; left: 10px; display: flex; gap: 5px; }
.badge { background: #3b82f6; color: white; padding: 4px 8px; border-radius: 6px; font-size: 0.75rem; }
.badge-icon { background: #1e2956; color: white; padding: 4px 8px; border-radius: 6px; font-size: 0.75rem; }
.content { padding: 1.25rem; }
.title { font-weight: 700; color: #1e2956; margin-bottom: 0.5rem; }
.location { font-size: 0.85rem; color: #64748b; margin-bottom: 0.25rem; }
.specs { font-size: 0.85rem; color: #64748b; margin-bottom: 1rem; }
.price { font-size: 0.9rem; color: #64748b; }
.price strong { color: #1e2956; font-size: 1.1rem; }
.availability { font-size: 0.8rem; margin-top: 0.5rem; }
</style>