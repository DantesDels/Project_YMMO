<template>
  <div
      class="field dropdown"
      :class="{ active: isOpen || modelValue.length }"
      v-click-outside="close"
  >
    <button class="dropdown-trigger" type="button" @click="toggle">
      <span class="label">{{ label }}</span>
      <span class="value">{{ displayValue }}</span>
      <button v-if="modelValue.length" class="clear-btn" type="button" @click.stop="clear" aria-label="Effacer la sélection">×</button>
      <svg class="chevron" :class="{ open: isOpen }" viewBox="0 0 20 20" fill="currentColor">
        <path fill-rule="evenodd" d="M5.23 7.21a.75.75 0 011.06.02L10 11.168l3.71-3.938a.75.75 0 111.08 1.04l-4.25 4.5a.75.75 0 01-1.08 0l-4.25-4.5a.75.75 0 01.02-1.06z" clip-rule="evenodd" />
      </svg>
    </button>

    <Transition name="popover">
      <div v-if="isOpen" class="popover">
        <label v-for="(optLabel, key) in options" :key="key" class="checkbox-item">
          <input
              type="checkbox"
              :value="key"
              :checked="modelValue.includes(key)"
              @change="toggleOption(key)"
          />
          <span class="checkbox-label">{{ optLabel }}</span>
        </label>

        <div class="popover-footer">
          <button type="button" class="link-btn" @click="clear" :disabled="!modelValue.length">
            Effacer
          </button>
          <button type="button" class="apply-btn" @click="isOpen = false">
            Appliquer
          </button>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { vClickOutside } from '@/utils/useClickOutside';

const props = defineProps({
  // Libellé affiché au-dessus de la valeur (ex: "Type", "Budget")
  label: { type: String, required: true },
  // { clé_backend: 'Libellé affiché' } — ex: { House: 'Maison', ... }
  options: { type: Object, required: true },
  // Tableau des clés sélectionnées (v-model)
  modelValue: { type: Array, default: () => [] },
  // Texte affiché quand rien n'est sélectionné (ex: "Tous", "Aucun")
  emptyLabel: { type: String, default: 'Tous' }
});

const emit = defineEmits(['update:modelValue']);

const isOpen = ref(false);

const toggle = () => { isOpen.value = !isOpen.value; };
const close = () => { isOpen.value = false; };
const clear = () => { emit('update:modelValue', []); };

const toggleOption = (key) => {
  const current = props.modelValue;
  const next = current.includes(key)
      ? current.filter(k => k !== key)
      : [...current, key];
  emit('update:modelValue', next);
};

const displayValue = computed(() => {
  if (props.modelValue.length === 0) return props.emptyLabel;
  const labels = props.modelValue.map(k => props.options[k]);
  return labels.length > 2
      ? `${labels[0]}, ${labels[1]} +${labels.length - 2}`
      : labels.join(', ');
});
</script>

<style scoped>
.field.dropdown {
  cursor: pointer;
  position: relative;
  min-width: 180px;
  background: white;
  padding: 0.8rem 1.2rem;
  border-radius: 999px;
  transition: box-shadow 0.15s ease, background-color 0.15s ease;
}

.field.dropdown:hover {
  box-shadow: 0 0 0 2px rgba(30, 41, 86, 0.08);
}

.field.dropdown.active {
  box-shadow: 0 0 0 2px #1e2956;
}

.dropdown-trigger {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  width: 100%;
  background: none;
  border: none;
  padding: 0;
  cursor: pointer;
  font: inherit;
  text-align: left;
}

.label {
  font-size: 0.8rem;
  color: #94a3b8;
  white-space: nowrap;
}

.value {
  font-weight: 600;
  color: #1e2956;
  font-size: 0.9rem;
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.clear-btn {
  background: #e2e8f0;
  border: none;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  font-size: 0.9rem;
  line-height: 1;
  color: #475569;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  transition: background 0.15s;
}

.clear-btn:hover { background: #cbd5e1; color: #1e2956; }

.chevron {
  width: 16px;
  height: 16px;
  color: #94a3b8;
  flex-shrink: 0;
  transition: transform 0.2s ease;
}

.chevron.open {
  transform: rotate(180deg);
}

.popover {
  position: absolute;
  top: calc(100% + 10px);
  left: 0;
  background: white;
  padding: 1rem;
  border-radius: 16px;
  box-shadow: 0 10px 25px rgba(0,0,0,0.15);
  width: 220px;
  display: flex;
  flex-direction: column;
  gap: 4px;
  z-index: 1000;
}

.checkbox-item {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 0.9rem;
  color: #334155;
  cursor: pointer;
  padding: 0.5rem 0.4rem;
  border-radius: 8px;
  transition: background-color 0.1s ease;
}

.checkbox-item:hover {
  background-color: #f1f5f9;
}

.checkbox-item input {
  width: 16px;
  height: 16px;
  accent-color: #1e2956;
  cursor: pointer;
  flex-shrink: 0;
}

.checkbox-label {
  flex: 1;
}

.popover-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 0.5rem;
  padding-top: 0.75rem;
  border-top: 1px solid #e2e8f0;
}

.link-btn {
  background: none;
  border: none;
  color: #64748b;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  padding: 0.4rem;
}

.link-btn:hover:not(:disabled) {
  color: #1e2956;
}

.link-btn:disabled {
  opacity: 0.4;
  cursor: default;
}

.apply-btn {
  background-color: #1e2956;
  color: white;
  border: none;
  border-radius: 8px;
  padding: 0.5rem 1rem;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.15s ease;
}

.apply-btn:hover {
  background-color: #3b4a8a;
}

.popover-enter-active,
.popover-leave-active {
  transition: opacity 0.15s ease, transform 0.15s ease;
}
.popover-enter-from,
.popover-leave-to {
  opacity: 0;
  transform: translateY(-6px);
}
</style>