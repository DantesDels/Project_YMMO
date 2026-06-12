<template>
  <div class="max-w-md w-full mx-auto p-8 bg-white rounded-xl shadow-lg border border-gray-100">
    <h2 class="text-2xl font-bold text-gray-800 mb-6 text-center">{{ title }}</h2>
    <form @submit.prevent="$emit('submit', form)" class="space-y-4">
      <slot name="fields" :form="form"></slot>
      <button
          type="submit"
          :disabled="loading"
          class="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2.5 rounded-lg transition-colors disabled:opacity-50"
      >
        {{ loading ? 'Chargement...' : buttonText }}
      </button>
    </form>
    <p class="mt-4 text-center text-sm text-gray-600">
      <slot name="footer"></slot>
    </p>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'

const props = defineProps<{ title: string, buttonText: string, loading: boolean }>()
defineEmits(['submit'])

// On définit un objet de formulaire générique
const form = reactive({} as any)
</script>