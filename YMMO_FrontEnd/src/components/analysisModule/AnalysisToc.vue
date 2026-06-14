<template>
  <div class="atoc-container">
    <button class="atoc-hamburger" @click="open = true" aria-label="Ouvrir le sommaire">
      <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="18" x2="21" y2="18"/></svg>
    </button>

    <Teleport to="body">
      <div v-if="open" class="atoc-overlay" @click.self="open = false">
        <nav class="atoc-panel">
          <div class="atoc-top">
            <span class="atoc-title">Sommaire</span>
            <button class="atoc-close" @click="open = false" aria-label="Fermer">✕</button>
          </div>
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
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

const props = defineProps<{
  sections: { id: string; title: string; expanded: boolean }[]
}>()

const emit = defineEmits<{ (e: 'update:activeId', id: string): void }>()
const activeId = ref('')
const open = ref(false)

function scrollTo(id: string) {
  open.value = false
  emit('update:activeId', id)
  activeId.value = id
  const el = document.getElementById(id)
  if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' })
}

let ticking = false
function onScroll() {
  if (ticking) return
  ticking = true
  requestAnimationFrame(() => {
    const mid = window.innerHeight / 3
    let best: string | null = null
    let bestDist = Infinity
    for (const s of props.sections) {
      const el = document.getElementById(s.id)
      if (!el) continue
      const rect = el.getBoundingClientRect()
      const dist = Math.abs(rect.top - mid)
      if (dist < bestDist) { bestDist = dist; best = s.id }
    }
    if (best && best !== activeId.value) { activeId.value = best; emit('update:activeId', best) }
    ticking = false
  })
}

onMounted(() => window.addEventListener('scroll', onScroll, { passive: true }))
onUnmounted(() => window.removeEventListener('scroll', onScroll))
</script>

<style scoped>
.atoc-container { display: contents; }
.atoc-hamburger {
  background: none; border: none; cursor: pointer;
  color: #1e2956; padding: 0.25rem; line-height: 0;
  display: flex; align-items: center;
}
</style>

<style>
.atoc-overlay {
  position: fixed; inset: 0; z-index: 9999;
  background: rgba(0,0,0,0.25);
  display: flex;
}
.atoc-panel {
  width: 260px; max-width: 80vw;
  background: #fff; height: 100%;
  padding: 0; overflow-y: auto;
  display: flex; flex-direction: column;
  box-shadow: 2px 0 12px rgba(0,0,0,0.15);
  animation: atoc-slide 0.2s ease-out;
}
@keyframes atoc-slide {
  from { transform: translateX(-100%); }
  to { transform: translateX(0); }
}
.atoc-top {
  display: flex; justify-content: space-between; align-items: center;
  padding: 1rem 1rem 0.5rem; border-bottom: 1px solid #e5e7eb;
}
.atoc-title {
  font-size: 0.8rem; font-weight: 700; text-transform: uppercase;
  letter-spacing: 0.05em; color: #9ca3af;
}
.atoc-close {
  background: none; border: none; cursor: pointer;
  font-size: 1.1rem; color: #6b7280; padding: 0.25rem; line-height: 1;
}
.atoc-item {
  display: flex; align-items: center; gap: 0.5rem;
  width: 100%; padding: 0.6rem 1rem;
  border: none; background: none; cursor: pointer;
  text-align: left; font-size: 0.85rem; color: #374151;
  transition: background 0.1s, color 0.1s;
}
.atoc-item:hover { background: #f3f4f6; color: #1e2956; }
.atoc-active { color: #1e2956; font-weight: 600; background: #f0f2f8; }
.atoc-dot {
  width: 8px; height: 8px; border-radius: 50%;
  background: #d1d5db; flex-shrink: 0; transition: background 0.2s;
}
.atoc-dot-on { background: #1e2956; }
</style>
