<template>
  <div class="search-bar-wrapper">
    <div class="search-container">
      <div class="input-group">
        <input
            type="text"
            placeholder="Région, Ville ou Critères..."
            class="search-input"
        />
      </div>

      <div class="input-group">
        <input type="date" class="search-input" />
      </div>

      <select class="search-input">
        <option>Type de logement</option>
        <option>Appartement</option>
        <option>Studio</option>
        <option>Colocation</option>
      </select>

      <button class="search-btn">
        Rechercher
      </button>
    </div>
  </div>
</template>

<style scoped>
/* Conteneur global qui gère le décalage vers le haut */
.search-bar-wrapper {
  @apply -mt-16 px-4 relative z-20;
}

/* Le design de la barre */
.search-container {
  @apply bg-white p-4 rounded-xl shadow-xl max-w-5xl mx-auto flex flex-wrap gap-3 border border-gray-100;
}

/* Style des inputs */
.input-group { @apply flex-grow; }
.search-input {
  @apply w-full px-4 py-3 border border-gray-200 rounded-lg outline-none transition-all;
  @apply focus:border-blue-500 focus:ring-2 focus:ring-blue-100;
}

/* Style du bouton */
.search-btn {
  @apply bg-[#1e2956] text-white px-8 py-3 rounded-lg font-bold transition-all;
  @apply hover:bg-[#2d3a6e] hover:shadow-lg active:scale-95;
}
</style>