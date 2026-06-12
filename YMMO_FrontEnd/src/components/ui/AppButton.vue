<template>
  <router-link v-if="to" :to="to" class="btn-primary">
    <slot />
  </router-link>

  <button v-else class="btn-primary" type="button">
    <slot />
  </button>
</template>

<script setup>
defineProps({
  to: { type: [String, Object], default: null },
  variant: { type: String, default: 'primary' }
});
</script>

<style scoped>
.btn-primary {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  white-space: nowrap;
  padding: 0.6rem 1.2rem;
  background-color: #1e2956;
  color: white;
  border-radius: 8px;
  font-weight: 600;
  text-decoration: none;
  transition: all 0.3s ease;
  min-width: fit-content;
  border: none; /* Important pour les boutons */
  cursor: pointer;
}

.btn-primary:hover {
  background-color: #3b4a8a;
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
}
</style>