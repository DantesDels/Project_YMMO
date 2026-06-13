<template>
  <div class="dashboard">
    <div class="container">
      <h1 class="title">Tableau de bord</h1>

      <div v-if="!authStore.user" class="card">
        <p>Connectez-vous pour accéder à votre espace personnel.</p>
        <router-link to="/authentification" class="btn">Se connecter</router-link>
      </div>

      <div v-else class="role-redirect">
        <p>Redirection vers votre espace {{ roleLabel }}...</p>
        <router-link v-if="authStore.user.role === 'Agent'" to="/agent/dashboard" class="btn">
          Accéder à mon espace agent
        </router-link>
        <router-link v-else to="/client/dashboard" class="btn">
          Accéder à mon espace client
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAuthentificationStore } from '@/stores/authentification.store'

const authStore = useAuthentificationStore()

const roleLabel = computed(() => {
  const role = authStore.user?.role
  if (role === 'Agent') return 'agent'
  if (role === 'Client') return 'client'
  if (role === 'Admin' || role === 'Manager') return 'administration'
  return ''
})
</script>

<style scoped>
.dashboard {
  padding: 3rem 1rem;
  min-height: calc(100vh - 150px);
  background: #f8fafc;
}

.container {
  max-width: 800px;
  margin: 0 auto;
}

.title {
  font-size: 1.75rem;
  font-weight: 800;
  color: #1e2956;
  margin-bottom: 1.5rem;
}

.card {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 2rem;
  text-align: center;
}

.card p {
  color: #64748b;
  margin-bottom: 1rem;
}

.btn {
  display: inline-block;
  padding: 0.625rem 1.25rem;
  background: #1e2956;
  color: white;
  border-radius: 8px;
  font-weight: 600;
  text-decoration: none;
}

.btn:hover {
  background: #3b4a8a;
}

.role-redirect {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 2rem;
  text-align: center;
}

.role-redirect p {
  color: #64748b;
  margin-bottom: 1rem;
}
</style>
