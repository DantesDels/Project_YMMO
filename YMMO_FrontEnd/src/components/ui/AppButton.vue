<template>
  <router-link v-if="to" :to="to" :class="['btn', `btn-${variant}`]">
    <slot />
  </router-link>

  <button v-else :class="['btn', `btn-${variant}`]" type="button">
    <slot />
  </button>
</template>

<script setup>
defineProps({
  to: { type: [String, Object], default: null },
  variant: { type: String, default: 'primary' } // 'primary' | 'ghost' | 'secondary'
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

/* ── Variante primary ────────────────────────────────────────── */
.btn-primary {
  padding: 0.6rem 1.2rem;
  background-color: #1e2956;
  color: white;
  font-size: 18px;
  box-shadow: 0 5px 7px rgba(0,0,0,0.2);
}

.btn-primary:hover {
  background-color: #3b4a8a;
  transform: translateY(-2px);
  box-shadow: 0 8px 12px rgba(0,0,0,0.8);
}

.btn-primary:active {
  background-color: #161d40;
  transform: translateY(1px) scale(0.98);
  box-shadow: 0 2px 4px rgba(0,0,0,0.3);
  transition: all 0.05s ease;
}

/* ── Variante secondary — bouton "outline" sur fond sombre ─────
   Contraste vérifié : blanc sur transparent + bordure visible ≥ 3:1
   (WCAG 1.4.11 pour composants UI), texte blanc plein = 21:1 sur noir. */
.btn-secondary {
  padding: 0.6rem 1.2rem;
  background-color: rgba(255, 255, 255, 0.08);
  color: white;
  font-size: 18px;
  border: 1.5px solid rgba(255, 255, 255, 0.6);
  backdrop-filter: blur(4px);
}

.btn-secondary:hover {
  background-color: rgba(255, 255, 255, 0.18);
  border-color: white;
  transform: translateY(-2px);
}

.btn-secondary:active {
  background-color: rgba(255, 255, 255, 0.25);
  transform: translateY(1px) scale(0.98);
  transition: all 0.05s ease;
}

/* ── Variante ghost ───────────────────────────────────────────── */
.btn-ghost {
  padding: 0.4rem 0;
  background-color: transparent;
  color: #64748b;
  font-size: 0.95rem;
  font-weight: 500;
  box-shadow: none;
}

.btn-ghost:hover {
  background-color: transparent;
  color: #1e2956;
  transform: translateX(-2px);
  box-shadow: none;
}

.btn-ghost:active {
  background-color: transparent;
  transform: translateX(-2px) scale(0.98);
  box-shadow: none;
  transition: all 0.05s ease;
}
</style>