<template>
  <nav class="atoc">
    <div class="atoc-header">Sommaire</div>
    <button
      v-for="s in sections"
      :key="s.id"
      class="atoc-item"
      :class="{ 'atoc-active': activeId === s.id }"
      @click="scrollTo(s.id)"
    >
      <span class="atoc-dot" :class="{ 'atoc-dot-on': s.expanded }"></span>
      <span class="atoc-label">{{ s.title }}</span>
    </button>
  </nav>
</template>

<script setup lang="ts">
const props = defineProps<{
  sections: { id: string; title: string; expanded: boolean }[]
  activeId?: string
}>()

function scrollTo(id: string) {
  const el = document.getElementById(id)
  if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' })
}
</script>

<style scoped>
.atoc {
  width: 220px;
  flex-shrink: 0;
  background: #fff;
  border-right: 1px solid #e5e7eb;
  padding: 1rem 0;
  overflow-y: auto;
  position: sticky;
  top: 0;
  height: 100vh;
  align-self: flex-start;
}
.atoc-header {
  padding: 0.5rem 1rem;
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: #9ca3af;
}
.atoc-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  width: 100%;
  padding: 0.5rem 1rem;
  border: none;
  background: none;
  cursor: pointer;
  text-align: left;
  font-size: 0.85rem;
  color: #374151;
  transition: background 0.1s, color 0.1s;
}
.atoc-item:hover { background: #f3f4f6; color: #1e2956; }
.atoc-active { color: #1e2956; font-weight: 600; }
.atoc-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #d1d5db;
  flex-shrink: 0;
  transition: background 0.2s;
}
.atoc-dot-on { background: #1e2956; }
</style>
