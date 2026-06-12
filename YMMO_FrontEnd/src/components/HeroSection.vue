<template>
  <section class="...">
    <SearchBar @search="handleSearch" class="mt-8 max-w-3xl mx-auto" />
  </section>
</template>

<script setup>
import SearchBar from '@/components/ui/SearchBar.vue';

const handleSearch = (filters) => {
  console.log("Recherche lancée avec :", filters);
};
</script>