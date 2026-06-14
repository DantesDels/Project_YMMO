<template>
  <div class="form-page">
    <h1 class="form-title">{{ isEdit ? 'Modifier la propriété' : 'Ajouter une propriété' }}</h1>
    <p class="form-subtitle">{{ isEdit ? 'Mettez à jour les informations du bien.' : 'Remplissez les informations du bien à mettre en vente.' }}</p>
    <PropertyForm
      :submit-label="isEdit ? 'Enregistrer les modifications' : 'Publier le bien'"
      :loading="submitting"
      :initial-data="isEdit ? formInitialData : undefined"
      @submit="handleSubmit"
    />
    <p v-if="success" class="success-msg">{{ isEdit ? 'Bien modifié avec succès !' : 'Bien publié avec succès !' }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import PropertyForm from '@/components/PropertyForm.vue'
import { useCustomProperties } from '@/stores/customProperties.store'
import { useAuthentificationStore } from '@/stores/authentification.store'

const route = useRoute()
const router = useRouter()
const authStore = useAuthentificationStore()
const { addProperty, getById, updateProperty } = useCustomProperties()

const isEdit = computed(() => route.name === 'agent-property-edit')
const editId = computed(() => route.params.id as string)

const formInitialData = computed(() => {
  if (!isEdit.value) return undefined
  const p = getById(editId.value)
  if (!p) return undefined
  return {
    propertyName: p.title,
    propertyDescription: p.description,
    propertyType: p.type,
    yearBuilt: p.yearBuilt,
    condition: p.condition,
    energyClass: p.energyClass,
    initialPrice: p.price,
    surface: p.surface,
    rooms: p.rooms,
    furnishing: p.furnishing,
    features: p.mainFeatures,
    photos: p.pictures.map(pic => pic.url),
    location: {
      street: '',
      city: p.address.replace(/\(.*\)/, '').trim(),
      postalCode: (p.address.match(/\(([^)]+)\)/) || [])[1] || '',
      region: '',
      country: 'France',
    },
  }
})

const submitting = ref(false)
const success = ref(false)

async function handleSubmit(propertyData: any) {
  submitting.value = true
  success.value = false
  try {
    const agentName = authStore.user?.username ?? 'Agent YMMO'
    const agencyName = `Agence YMMO ${propertyData.location?.city || ''}`.trim() || 'Agence YMMO'

    const photos = propertyData.photos?.length
      ? propertyData.photos.map((url: string) => ({ url }))
      : [{ url: 'https://picsum.photos/seed/custom/800/600' }]

    if (isEdit.value) {
      updateProperty(editId.value, {
        title: propertyData.propertyName,
        type: propertyData.propertyType,
        condition: propertyData.condition,
        energyClass: propertyData.energyClass || 'D',
        price: propertyData.initialPrice,
        surface: propertyData.surface,
        address: `${propertyData.location.city} (${propertyData.location.postalCode})`,
        mainFeatures: propertyData.features || [],
        rooms: propertyData.rooms || 1,
        furnishing: propertyData.furnishing || '',
        description: propertyData.propertyDescription || '',
        yearBuilt: propertyData.yearBuilt || new Date().getFullYear(),
        image: photos[0]?.url || 'https://picsum.photos/seed/custom/800/600',
        pictures: photos,
      })
      success.value = true
      router.push('/agent/dashboard')
    } else {
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
        image: photos[0]?.url || 'https://picsum.photos/seed/custom/800/600',
        pictures: photos,
      })
      success.value = true
      router.push('/catalog')
    }
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
