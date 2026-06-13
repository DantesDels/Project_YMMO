<!--
  BudgetRangeSlider.vue
  ─────────────────────────────────────────────────────────────
  Dropdown autonome (trigger + popover + footer Effacer/Appliquer),
  même pattern que SearchDropdown.vue, mais pour un range min/max
  avec double slider à inversion automatique.

  v-model="{ min: number, max: number }"

  Usage :
    <BudgetRangeSlider
      v-model="budgetRange"
      label="Budget"
      :min="0"
      :max="1000000"
      :step="5000"
      unit="€"
    />
-->
<template>
  <div class="field dropdown" :class="{ active: isOpen || hasValue }" v-click-outside="close">
    <button class="dropdown-trigger" type="button" @click="toggle">
      <span class="label">{{ label }}</span>
      <span class="value">{{ displayValue }}</span>
      <svg class="chevron" :class="{ open: isOpen }" viewBox="0 0 20 20" fill="currentColor">
        <path fill-rule="evenodd" d="M5.23 7.21a.75.75 0 011.06.02L10 11.168l3.71-3.938a.75.75 0 111.08 1.04l-4.25 4.5a.75.75 0 01-1.08 0l-4.25-4.5a.75.75 0 01.02-1.06z" clip-rule="evenodd" />
      </svg>
    </button>

    <Transition name="popover">
      <div v-if="isOpen" class="popover popover--budget">

        <!-- ── Champs numériques ──────────────────────────────── -->
        <div class="inputs-row">
          <label class="input-group">
            <span class="input-label">Min.</span>
            <div class="input-wrap">
              <input
                  type="number"
                  :min="min"
                  :max="max"
                  :step="step"
                  :value="localMin"
                  @input="onMinInput($event)"
                  @blur="commitInputs"
              />
              <span class="unit">{{ unit }}</span>
            </div>
          </label>

          <span class="dash">—</span>

          <label class="input-group">
            <span class="input-label">Max.</span>
            <div class="input-wrap">
              <input
                  type="number"
                  :min="min"
                  :max="max"
                  :step="step"
                  :value="localMax"
                  @input="onMaxInput($event)"
                  @blur="commitInputs"
              />
              <span class="unit">{{ unit }}</span>
            </div>
          </label>
        </div>

        <!-- ── Slider double poignée ──────────────────────────── -->
        <div class="slider-track-wrap">
          <div class="slider-track"></div>

          <div
              class="slider-range"
              :style="{ left: `${minPercent}%`, width: `${maxPercent - minPercent}%` }"
          ></div>

          <!--
            ───────────────────────────────────────────────────────
            GESTION DE L'INVERSION — explication
            ───────────────────────────────────────────────────────
            Deux <input type="range"> superposés via CSS. Chacun
            pilote TOUJOURS sa propre valeur (localMin / localMax) —
            on ne permute jamais QUEL input contrôle quoi.

            Si l'utilisateur tire "Min" au-delà de "Max", on échange
            les VALEURS (onMinChange / onMaxChange) : localMax prend
            l'ancienne valeur de localMin, localMin prend la nouvelle
            valeur du curseur. La poignée suivie par l'utilisateur ne
            saute jamais ; c'est l'AUTRE qui "hérite" de l'ancienne
            position.

            activeThumb pilote le z-index pour que la poignée
            activement déplacée reste toujours cliquable, même
            quand les deux valeurs sont proches ou identiques.
          -->
          <input
              type="range"
              class="slider-input slider-input--min"
              :class="{ 'slider-input--front': activeThumb === 'min' }"
              :min="min"
              :max="max"
              :step="step"
              :value="localMin"
              @input="onMinChange($event)"
              @pointerdown="activeThumb = 'min'"
          />
          <input
              type="range"
              class="slider-input slider-input--max"
              :class="{ 'slider-input--front': activeThumb === 'max' }"
              :min="min"
              :max="max"
              :step="step"
              :value="localMax"
              @input="onMaxChange($event)"
              @pointerdown="activeThumb = 'max'"
          />
        </div>

        <div class="scale-labels">
          <span>{{ formatCompact(min) }}</span>
          <span>{{ formatCompact(max) }}</span>
        </div>

        <!-- ── Footer Effacer / Appliquer ──────────────────────── -->
        <div class="popover-footer">
          <button type="button" class="link-btn" @click="reset">
            Effacer
          </button>
          <button type="button" class="apply-btn" @click="apply">
            Appliquer
          </button>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';
import { vClickOutside } from '@/utils/useClickOutside';

const props = defineProps({
  modelValue: {
    type: Object,
    default: () => ({ min: 0, max: 1000000 }),
  },
  label: { type: String, default: 'Budget' },
  min: { type: Number, default: 0 },
  max: { type: Number, default: 1000000 },
  step: { type: Number, default: 5000 },
  unit: { type: String, default: '€' },
  emptyLabel: { type: String, default: 'Tous' },
});

const emit = defineEmits(['update:modelValue']);

const isOpen = ref(false);
const toggle = () => { isOpen.value = !isOpen.value; };
const close = () => { isOpen.value = false; };

const localMin = ref(props.modelValue.min);
const localMax = ref(props.modelValue.max);
const activeThumb = ref('min');

watch(() => props.modelValue, (val) => {
  localMin.value = val.min;
  localMax.value = val.max;
}, { deep: true });

function apply() {
  emit('update:modelValue', { min: localMin.value, max: localMax.value });
  isOpen.value = false;
}

function reset() {
  localMin.value = props.min;
  localMax.value = props.max;
  emit('update:modelValue', { min: props.min, max: props.max });
}

// ──────────────────────────────────────────────────────────────
// GESTION DE L'INVERSION DES POIGNÉES (sliders)
// ──────────────────────────────────────────────────────────────
//
// PROBLÈME CORRIGÉ : avec deux <input type="range"> partageant les
// mêmes bornes natives [min, max], le navigateur ne "bloque" pas
// le thumb au croisement — mais comme les deux thumbs sont rendus
// au même endroit, l'utilisateur a l'impression que l'un "pousse"
// l'autre, alors qu'en réalité onMaxChange ne se déclenche qu'APRÈS
// le croisement, créant un décalage perceptible d'un "step".
//
// SOLUTION : on ne compare plus newVal à l'ancienne valeur de
// l'autre thumb, mais on détecte le croisement de façon symétrique
// et on échange immédiatement, y compris en cas d'ÉGALITÉ stricte.
// On utilise <= / >= (et non < / >) pour que l'échange se produise
// dès que les deux poignées se touchent, pas seulement quand elles
// se dépassent — ce qui élimine le décalage d'un step.
//
function onMinChange(event) {
  const newVal = Number(event.target.value);

  if (newVal >= localMax.value) {
    // Croisement (ou contact) détecté : échange immédiat.
    // localMin "hérite" de l'ancienne position de localMax,
    // localMax prend la nouvelle valeur du curseur "Min".
    const oldMax = localMax.value;
    localMax.value = newVal;
    localMin.value = oldMax;
    activeThumb.value = 'max';
  } else {
    localMin.value = newVal;
    activeThumb.value = 'min';
  }
}

function onMaxChange(event) {
  const newVal = Number(event.target.value);

  if (newVal <= localMin.value) {
    // Symétrique : localMax hérite de l'ancienne position de
    // localMin, localMin prend la nouvelle valeur du curseur "Max".
    const oldMin = localMin.value;
    localMin.value = newVal;
    localMax.value = oldMin;
    activeThumb.value = 'min';
  } else {
    localMax.value = newVal;
    activeThumb.value = 'max';
  }
}

function onMinInput(event) {
  const raw = Number(event.target.value);
  if (!Number.isNaN(raw)) localMin.value = raw;
}

function onMaxInput(event) {
  const raw = Number(event.target.value);
  if (!Number.isNaN(raw)) localMax.value = raw;
}

function commitInputs() {
  localMin.value = clamp(localMin.value, props.min, props.max);
  localMax.value = clamp(localMax.value, props.min, props.max);

  if (localMin.value > localMax.value) {
    const tmp = localMin.value;
    localMin.value = localMax.value;
    localMax.value = tmp;
  }
}

function clamp(val, lo, hi) {
  return Math.min(Math.max(val, lo), hi);
}

const minPercent = computed(() =>
    ((localMin.value - props.min) / (props.max - props.min)) * 100
);
const maxPercent = computed(() =>
    ((localMax.value - props.min) / (props.max - props.min)) * 100
);

const hasValue = computed(() =>
    props.modelValue.min !== props.min || props.modelValue.max !== props.max
);

const displayValue = computed(() => {
  if (!hasValue.value) return props.emptyLabel;
  return `${formatCompact(props.modelValue.min)} - ${formatCompact(props.modelValue.max)}`;
});

function formatCompact(val) {
  if (val >= 1_000_000) return `${(val / 1_000_000).toFixed(1).replace('.0', '')}M${props.unit}`;
  if (val >= 1_000) return `${Math.round(val / 1000)}k${props.unit}`;
  return `${val}${props.unit}`;
}
</script>

<style scoped>
/* ── Champ — même style que .field.dropdown de SearchDropdown ─── */
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

/* ── Popover ────────────────────────────────────────────────── */
.popover {
  position: absolute;
  top: calc(100% + 10px);
  left: 0;
  background: white;
  padding: 1rem;
  border-radius: 16px;
  box-shadow: 0 10px 25px rgba(0,0,0,0.15);
  z-index: 1000;
}

.popover--budget {
  width: 340px;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

/* ── Champs numériques ─────────────────────────────────────── */
.inputs-row {
  display: flex;
  align-items: flex-end;
  gap: 0.75rem;
}

.input-group {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.input-label {
  font-size: 0.75rem;
  font-weight: 600;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

.input-wrap {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.6rem 0.85rem;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background: #f8fafc;
  transition: border-color 0.15s ease, box-shadow 0.15s ease;
}

.input-wrap:focus-within {
  border-color: #1e2956;
  box-shadow: 0 0 0 3px rgba(30, 41, 86, 0.08);
  background: white;
}

.input-wrap input {
  width: 100%;
  border: none;
  outline: none;
  background: transparent;
  font-size: 0.95rem;
  font-weight: 600;
  color: #1e2956;
  -moz-appearance: textfield;
}
.input-wrap input::-webkit-outer-spin-button,
.input-wrap input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.unit {
  font-size: 0.85rem;
  color: #94a3b8;
  font-weight: 500;
  flex-shrink: 0;
}

.dash {
  color: #cbd5e1;
  padding-bottom: 0.6rem;
  font-weight: 300;
}

/* ── Slider track ──────────────────────────────────────────── */
.slider-track-wrap {
  position: relative;
  height: 20px;
  display: flex;
  align-items: center;
}

.slider-track {
  position: absolute;
  left: 0;
  right: 0;
  height: 4px;
  border-radius: 2px;
  background: #e2e8f0;
}

.slider-range {
  position: absolute;
  height: 4px;
  border-radius: 2px;
  background: #1e2956;
}

.slider-input {
  position: absolute;
  left: 0;
  width: 100%;
  height: 20px;
  margin: 0;
  appearance: none;
  background: transparent;
  pointer-events: none;
  z-index: 2;
}

.slider-input::-webkit-slider-thumb {
  pointer-events: auto;
}
.slider-input::-moz-range-thumb {
  pointer-events: auto;
}

.slider-input--front {
  z-index: 3;
}

.slider-input::-webkit-slider-thumb {
  appearance: none;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: white;
  border: 2px solid #1e2956;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
  cursor: grab;
  transition: transform 0.1s ease, box-shadow 0.1s ease;
}
.slider-input::-webkit-slider-thumb:hover {
  transform: scale(1.1);
}
.slider-input::-webkit-slider-thumb:active {
  cursor: grabbing;
  transform: scale(1.15);
  box-shadow: 0 2px 10px rgba(30, 41, 86, 0.35);
}

.slider-input::-moz-range-thumb {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: white;
  border: 2px solid #1e2956;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
  cursor: grab;
  transition: transform 0.1s ease, box-shadow 0.1s ease;
}
.slider-input::-moz-range-thumb:hover {
  transform: scale(1.1);
}
.slider-input::-moz-range-thumb:active {
  cursor: grabbing;
  transform: scale(1.15);
}

.slider-input::-moz-focus-outer {
  border: 0;
}

/* ── Repères d'échelle ────────────────────────────────────────── */
.scale-labels {
  display: flex;
  justify-content: space-between;
  font-size: 0.75rem;
  color: #94a3b8;
  margin-top: -0.75rem;
}

/* ── Footer ─────────────────────────────────────────────────── */
.popover-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
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

.link-btn:hover {
  color: #1e2956;
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