<template>
  <div class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100">
    <h3 class="text-xl font-bold text-indigo-900 mb-4">{{ title }}</h3>
    <p class="text-gray-600 leading-relaxed">{{ description }}</p>
  </div>
</template>

<script setup>
defineProps(['title', 'description']);
</script>