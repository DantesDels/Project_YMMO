<template>
  <div class="p-8 max-w-3xl mx-auto">
    <h1 class="text-2xl font-bold mb-2">Ajouter une propriété</h1>
    <p class="text-muted mb-6">Remplissez les informations du bien à mettre en vente.</p>
    <PropertyForm
      submit-label="Publier le bien"
      :loading="submitting"
      @submit="handleSubmit"
    />
    <p v-if="success" class="success-msg">Bien publié avec succès !</p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import PropertyForm from '@/components/PropertyForm.vue'
import { propertyService } from '@/services/property.service'
import { useAuthentificationStore } from '@/stores/authentification.store'

const authStore = useAuthentificationStore()
const submitting = ref(false)
const success = ref(false)

async function handleSubmit(propertyData: any) {
  submitting.value = true
  success.value = false
  try {
    await propertyService.create({
      ...propertyData,
      agencyId: '',   // À récupérer depuis le profil agent
      agentId: authStore.user?.contactId ?? '',
      sellerId: authStore.user?.contactId ?? '',
    })
    success.value = true
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.text-muted {
  color: #64748b;
}

.success-msg {
  margin-top: 1rem;
  padding: 0.75rem;
  background: #f0fdf4;
  color: #166534;
  border-radius: 8px;
  font-weight: 600;
  text-align: center;
}
</style>
