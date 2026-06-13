<template>
  <div class="field location">
    <span class="search-icon">{{ icon }}</span>
    <input
        type="text"
        :placeholder="placeholder"
        :value="modelValue"
        @input="$emit('update:modelValue', $event.target.value)"
    />
  </div>
</template>

<script setup>
defineProps({
  modelValue: { type: String, default: '' },
  placeholder: { type: String, default: 'Rechercher...' },
  icon: { type: String, default: '🔍' }
});
defineEmits(['update:modelValue']);
</script>

<style scoped>
.field {
  display: flex;
  align-items: center;
  background: white;
  padding: 0.8rem 1.2rem;
  border-radius: 18px;
}

.field.location {
  gap: 10px;
}

.search-icon {
  color: #64748b !important;
  font-size: 1.1rem;
  display: flex;
  align-items: center;
}

.field.location input {
  border: none;
  outline: none;
  font-size: 0.95rem;
  color: #1e2956;
  width: 100%;
}
</style>