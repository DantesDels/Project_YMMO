<template>
  <div class="property-card">
    <div class="card-img-wrapper">
      <img src="..." alt="Logement" />
      <span class="card-badge">Promotion</span>
    </div>
    <div class="card-content">
      <h3 class="card-title">Logement en résidence</h3>
      <p class="card-price">650€ / mois</p>
    </div>
  </div>
</template>

<style scoped>
/* CSS local au composant : pas besoin d'importer, c'est appliqué ici uniquement */
.property-card { @apply bg-white border rounded-2xl overflow-hidden hover:shadow-xl transition; }
.card-img-wrapper { @apply h-48 bg-gray-200; }
.card-content { @apply p-4; }
</style>