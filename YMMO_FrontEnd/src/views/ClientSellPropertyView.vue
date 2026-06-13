<template>
  <div class="sell-wrapper">
    <div class="sell-container">
      <h1 class="page-title">Soumettre un bien à la vente</h1>
      <p class="page-desc">
        Décrivez votre bien en détail. Un agent l'étudiera et vous recontactera sous 48h.
      </p>

      <PropertyForm
        submit-label="Soumettre ma demande"
        :loading="submitting"
        @submit="handleSubmit"
      />

      <div v-if="submitted" class="success-card">
        <h3>Demande envoyée !</h3>
        <p>Votre bien a été soumis à notre équipe. Un agent vous contactera très prochainement.</p>
        <router-link to="/dashboard" class="btn-back">Retour au tableau de bord</router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import PropertyForm from '@/components/PropertyForm.vue'
import { useAuthentificationStore } from '@/stores/authentification.store'

const authStore = useAuthentificationStore()
const submitting = ref(false)
const submitted = ref(false)

async function handleSubmit(propertyData: any) {
  submitting.value = true
  try {
    // TODO: appeler le futur endpoint POST /api/sell-request
    // Pour l'instant on simule l'envoi
    await new Promise(resolve => setTimeout(resolve, 1000))
    submitted.value = true
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.sell-wrapper {
  display: flex;
  justify-content: center;
  padding: 2rem 1rem;
  min-height: calc(100vh - 150px);
  background: #f8fafc;
}

.sell-container {
  width: 100%;
  max-width: 760px;
}

.page-title {
  font-size: 1.75rem;
  font-weight: 800;
  color: #1e2956;
  margin-bottom: 0.5rem;
}

.page-desc {
  color: #64748b;
  margin-bottom: 2rem;
  line-height: 1.5;
}

.success-card {
  margin-top: 2rem;
  background: #f0fdf4;
  border: 1px solid #86efac;
  border-radius: 12px;
  padding: 1.5rem;
  text-align: center;
}

.success-card h3 {
  font-size: 1.25rem;
  font-weight: 700;
  color: #166534;
  margin-bottom: 0.5rem;
}

.success-card p {
  color: #15803d;
  margin-bottom: 1rem;
}

.btn-back {
  display: inline-block;
  padding: 0.625rem 1.25rem;
  background: #166534;
  color: white;
  border-radius: 8px;
  font-weight: 600;
  text-decoration: none;
}

.btn-back:hover {
  background: #15803d;
}
</style>
