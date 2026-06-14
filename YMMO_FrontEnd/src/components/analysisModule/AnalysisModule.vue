<template>
  <section class="am-module" :class="{ 'am-collapsed': !expanded }" :id="id">
    <button class="am-header" @click="$emit('toggle', id)" :aria-expanded="expanded">
      <span class="am-title">{{ title }}</span>
      <span class="am-toggle">{{ expanded ? '−' : '+' }}</span>
    </button>
    <div class="am-body" v-show="expanded">
      <slot />
    </div>
  </section>
</template>

<script setup lang="ts">
defineProps<{ id: string; title: string; expanded?: boolean }>()
defineEmits<{ (e: 'toggle', id: string): void }>()
</script>

<style scoped>
.am-module {
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 1px 4px rgba(0,0,0,0.06);
  overflow: hidden;
}
.am-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  padding: 1rem 1.5rem;
  border: none;
  background: none;
  cursor: pointer;
  color: #1e2956;
  font-size: 1.05rem;
  font-weight: 700;
  text-align: left;
  transition: background 0.15s;
}
.am-header:hover { background: #f8f9fb; }
.am-toggle {
  font-size: 1.3rem;
  line-height: 1;
  color: #6b7280;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  transition: background 0.15s;
}
.am-header:hover .am-toggle { background: #e5e7eb; }
.am-body { padding: 0 1.5rem 1.5rem; }
</style>
