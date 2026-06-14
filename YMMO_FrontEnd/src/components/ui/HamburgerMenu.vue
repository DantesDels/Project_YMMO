<template>
  <button
    class="hamburger"
    :class="{ open: isOpen }"
    @click="toggle"
    aria-label="Menu de navigation"
    :aria-expanded="isOpen"
  >
    <span></span><span></span><span></span>
  </button>

  <Transition name="mobile-nav">
    <nav v-if="isOpen" class="mobile-nav" aria-label="Navigation principale" @click.self="close">
      <div class="mobile-nav-inner">
        <slot :close="close" />
      </div>
    </nav>
  </Transition>
</template>

<script setup lang="ts">
import { ref, watch, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const isOpen = ref(false)

function toggle() { isOpen.value = !isOpen.value }
function close() { isOpen.value = false }

watch(isOpen, (val) => {
  document.body.style.overflow = val ? 'hidden' : ''
})

const unregister = router.afterEach(() => { close() })
onUnmounted(() => { unregister() })
</script>

<style scoped>
.hamburger {
  display: none;
  flex-direction: column;
  gap: 5px;
  background: none;
  border: none;
  cursor: pointer;
  padding: 8px;
  z-index: 1001;
}

.hamburger span {
  display: block;
  width: 24px;
  height: 2px;
  background: #1e2956;
  border-radius: 2px;
  transition: all 0.25s ease;
}

.hamburger.open span:nth-child(1) {
  transform: translateY(7px) rotate(45deg);
}
.hamburger.open span:nth-child(2) {
  opacity: 0;
}
.hamburger.open span:nth-child(3) {
  transform: translateY(-7px) rotate(-45deg);
}

.mobile-nav {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.4);
  z-index: 1000;
  display: flex;
  justify-content: flex-end;
}

.mobile-nav-inner {
  width: 220px;
  max-width: 75vw;
  background: white;
  height: 100%;
  padding: 5rem 1.5rem 2rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  box-shadow: -4px 0 20px rgba(0,0,0,0.15);
  overflow-y: auto;
}

.mobile-nav-enter-active,
.mobile-nav-leave-active {
  transition: opacity 0.2s ease;
}
.mobile-nav-enter-from,
.mobile-nav-leave-to {
  opacity: 0;
}
.mobile-nav-enter-active .mobile-nav-inner,
.mobile-nav-leave-active .mobile-nav-inner {
  transition: transform 0.2s ease;
}
.mobile-nav-enter-from .mobile-nav-inner,
.mobile-nav-leave-to .mobile-nav-inner {
  transform: translateX(100%);
}

@media (max-width: 767px) {
  .hamburger { display: flex; }
}

@media (min-width: 768px) {
  .hamburger { display: none !important; }
  .mobile-nav { display: none !important; }
}
</style>
