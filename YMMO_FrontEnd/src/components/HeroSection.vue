<template>
  <section class="hero-search">
    <SearchBar @search="handleSearch" />
  </section>
</template>

<script setup>
import SearchBar from '@/components/ui/SearchBar.vue';

const handleSearch = (filters) => {
  console.log("Recherche lancée avec :", filters);
};
</script>

<style scoped>
.hero-search {
  padding: 2rem 1rem;
}

@media (max-width: 767px) {
  .hero-search {
    padding: 1rem 0.5rem;
  }
}
</style>
