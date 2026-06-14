<template>
  <div class="form-page">
    <h1 class="form-title">Ajouter une propriété</h1>
    <p class="form-subtitle">Remplissez les informations du bien à mettre en vente.</p>
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
import { useRouter } from 'vue-router'
import PropertyForm from '@/components/PropertyForm.vue'
import { useCustomProperties } from '@/stores/customProperties.store'
import { useAuthentificationStore } from '@/stores/authentification.store'

const router = useRouter()
const authStore = useAuthentificationStore()
const { addProperty } = useCustomProperties()

const submitting = ref(false)
const success = ref(false)

async function handleSubmit(propertyData: any) {
  submitting.value = true
  success.value = false
  try {
    const agentName = authStore.user?.username ?? 'Agent YMMO'
    const agencyName = `Agence YMMO ${propertyData.location?.city || ''}`.trim() || 'Agence YMMO'

    const lat = 48.8566 + (Math.random() - 0.5) * 0.08
    const lng = 2.3522 + (Math.random() - 0.5) * 0.08

    addProperty({
      id: `custom-${Date.now()}`,
      title: propertyData.propertyName,
      type: propertyData.propertyType,
      condition: propertyData.condition,
      energyClass: propertyData.energyClass || 'D',
      price: propertyData.initialPrice,
      surface: propertyData.surface,
      address: `${propertyData.location.city} (${propertyData.location.postalCode})`,
      mainFeatures: propertyData.features || [],
      available: true,
      availabilityDate: 'Immédiat',
      rooms: propertyData.rooms || 1,
      furnishing: propertyData.furnishing || '',
      description: propertyData.propertyDescription || '',
      yearBuilt: propertyData.yearBuilt || new Date().getFullYear(),
      latitude: lat,
      longitude: lng,
      agencyName,
      agentName,
      agentId: authStore.user?.contactId ?? '',
      image: 'https://picsum.photos/seed/custom/800/600',
      pictures: [{ url: 'https://picsum.photos/seed/custom/800/600' }],
    })
    success.value = true
    router.push('/catalog')
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.form-page {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem 1rem 4rem;
}

.form-title {
  font-size: 1.75rem;
  font-weight: 800;
  color: #1e2956;
  margin: 0 0 0.25rem;
  text-align: center;
}

.form-subtitle {
  color: #64748b;
  text-align: center;
  margin-bottom: 2rem;
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
