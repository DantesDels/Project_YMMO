<template>
  <div class="field dropdown" :class="{ active: isOpen }" v-click-outside="close">
    <button class="dropdown-trigger" type="button" @click="toggle">
      <span class="label">{{ label }}</span>
      <span class="value">{{ displayValue }}</span>
      <button v-if="modelValue.length" class="clear-btn" type="button" @click.stop="clear" aria-label="Effacer la sélection">×</button>
      <svg class="chevron" :class="{ open: isOpen }" viewBox="0 0 20 20" fill="currentColor">
        <path fill-rule="evenodd" d="M5.23 7.21a.75.75 0 011.06.02L10 11.168l3.71-3.938a.75.75 0 111.08 1.04l-4.25 4.5a.75.75 0 01-1.08 0l-4.25-4.5a.75.75 0 01.02-1.06z" clip-rule="evenodd" />
      </svg>
    </button>

    <Transition name="popover">
      <div v-if="isOpen" class="popover extended">

        <div class="scroll-area">
          <div v-for="(group, groupName) in options" :key="groupName" class="group-container">
            <h4 class="group-title">{{ groupName }}</h4>
            <div class="grid-layout">
              <label v-for="(label, key) in group" :key="key" class="checkbox-item">
                <input
                    type="checkbox"
                    :value="key"
                    :checked="modelValue.includes(key)"
                    @change="toggleOption(key)"
                />
                <span class="checkbox-label">{{ label }}</span>
              </label>
            </div>
          </div>
        </div>

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
  label: String,
  options: Object, // Attend un objet: { "Catégorie": { "key": "Label" } }
  modelValue: { type: Array, default: () => [] },
  emptyLabel: { type: String, default: 'Tous' }
});

const emit = defineEmits(['update:modelValue']);
const isOpen = ref(false);

const toggle = () => { isOpen.value = !isOpen.value; };
const close = () => { isOpen.value = false; };
const clear = () => { emit('update:modelValue', []); };

const toggleOption = (key) => {
  const next = props.modelValue.includes(key)
      ? props.modelValue.filter(k => k !== key)
      : [...props.modelValue, key];
  emit('update:modelValue', next);
};

const displayValue = computed(() => {
  if (props.modelValue.length === 0) return props.emptyLabel;
  // Aplatit les groupes pour trouver les labels
  const allOptions = Object.values(props.options).reduce((acc, group) => ({ ...acc, ...group }), {});
  const labels = props.modelValue.map(k => allOptions[k]);
  return labels.length > 2 ? `${labels[0]}, ${labels[1]} +${labels.length - 2}` : labels.join(', ');
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
  transition: box-shadow 0.15s ease;
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

.label { font-size: 0.8rem; color: #94a3b8; white-space: nowrap; }
.value { font-weight: 600; color: #1e2956; font-size: 0.9rem; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }

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

.popover.extended {
  position: absolute;
  top: calc(100% + 12px); /* Un léger espace sous le bouton */
  left: 0;
  width: 500px;
  background: white;
  border-radius: 16px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.2);
  z-index: 2000;
  padding: 1.5rem; /* Un peu plus d'espace interne */
}

.scroll-area {
max-height: 350px;
overflow-y: auto;
padding-right: 5px;
}

.group-title {
font-size: 1rem;
color: #94a3b8;
text-transform: uppercase;
padding-bottom: 0.25rem;
border-bottom: 1px solid #f1f5f9;
letter-spacing: 0.05em;
  margin: 0 0 5px 0; /* Élimine la marge haute (top) pour remonter le titre */
}

.grid-layout {
  display: grid;
  grid-template-columns: 1fr 1fr; /* Deux colonnes égales */
  gap: 12px 20px; /* Plus d'espace horizontal entre les colonnes */
  margin-bottom: 20px;
}

.checkbox-label {
  font-size: 0.85rem;
  white-space: normal; /* Permet au texte long de passer à la ligne */
  line-height: 1.2;
}

.checkbox-item {
display: flex;
align-items: center;
gap: 8px;
padding: 6px;
border-radius: 6px;
cursor: pointer;
transition: background 0.1s;
}

.checkbox-item:hover { background: #f8fafc; }

.popover-footer {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  align-items: center;
  margin-top: 0.5rem;
  padding-top:1rem;
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