<template>
  <router-link v-if="to" :to="to" :class="['btn', `btn-${variant}`]">
    <slot />
  </router-link>

  <button v-else :class="['btn', `btn-${variant}`]" :type="type" :disabled="disabled">
    <template v-if="loading">Chargement...</template>
    <slot v-else />
  </button>
</template>

<script setup>
defineProps({
  to: { type: [String, Object], default: null },
  variant: { type: String, default: 'primary' },
  type: { type: String, default: 'button' },
  disabled: { type: Boolean, default: false },
  loading: { type: Boolean, default: false },
});
</script>

<style scoped>
.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  white-space: nowrap;
  border-radius: 8px;
  font-weight: 600;
  text-decoration: none;
  transition: all 0.3s ease;
  min-width: fit-content;
  border: none;
  cursor: pointer;
}

.btn:focus-visible {
  outline: 2px solid var(--color-secondary, #004ecc);
  outline-offset: 2px;
}

.btn:disabled {
  opacity: 0.5;
  cursor: default;
  pointer-events: none;
}

.btn-primary {
  padding: 0.6rem 1.2rem;
  background-color: #1e2956;
  color: white;
  font-size: 18px;
  box-shadow: 0 5px 7px rgba(0,0,0,0.2);
}

.btn-primary:hover:not(:disabled) {
  background-color: #3b4a8a;
  transform: translateY(-2px);
  box-shadow: 0 8px 12px rgba(0,0,0,0.8);
}

.btn-primary:active:not(:disabled) {
  background-color: #161d40;
  transform: translateY(1px) scale(0.98);
  box-shadow: 0 2px 4px rgba(0,0,0,0.3);
  transition: all 0.05s ease;
}

.btn-secondary {
  padding: 0.6rem 1.2rem;
  background-color: rgba(255, 255, 255, 0.08);
  color: white;
  font-size: 18px;
  border: 1.5px solid rgba(255, 255, 255, 0.6);
  backdrop-filter: blur(4px);
}

.btn-secondary:hover:not(:disabled) {
  background-color: rgba(255, 255, 255, 0.18);
  border-color: white;
  transform: translateY(-2px);
}

.btn-secondary:active:not(:disabled) {
  background-color: rgba(255, 255, 255, 0.25);
  transform: translateY(1px) scale(0.98);
  transition: all 0.05s ease;
}

.btn-ghost {
  padding: 0.4rem 0;
  background-color: transparent;
  color: #64748b;
  font-size: 0.95rem;
  font-weight: 500;
  box-shadow: none;
}

.btn-ghost:hover:not(:disabled) {
  background-color: transparent;
  color: #1e2956;
  transform: translateX(-2px);
  box-shadow: none;
}

.btn-ghost:active:not(:disabled) {
  background-color: transparent;
  transform: translateX(-2px) scale(0.98);
  box-shadow: none;
  transition: all 0.05s ease;
}

.btn-link {
  padding: 0.3rem 0.75rem;
  background: none;
  color: #94a3b8;
  font-weight: 500;
  box-shadow: none;
  border-radius: 6px;
}

.btn-link:hover:not(:disabled) {
  background: #f8fafc;
  color: #64748b;
  transform: none;
  box-shadow: none;
}

.btn-link:active:not(:disabled) {
  background: #f1f5f9;
  transform: none;
  box-shadow: none;
}
</style>
