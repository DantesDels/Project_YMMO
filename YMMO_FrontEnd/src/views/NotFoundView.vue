<template>
  <div class="not-found-container">
    <div class="error-wrapper">
      <h1 class="error-code">404</h1>
      <h2 class="error-title">Oups ! Cette page s'est envolé.</h2>
      <p class="error-description">
        Il semble que la page que vous recherchez n'existe plus ou a été déplacée.
        Pas de panique, nous pouvons vous ramener à bon port.
      </p>

      <div class="actions">
        <router-link to="/" class="btn-primary">Retour à l'accueil</router-link>
        <button @click="$router.go(-1)" class="btn-secondary">Page précédente</button>
      </div>

      <div class="icon-wrapper">
        <svg viewBox="0 0 24 24" class="house-icon">
          <path d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" stroke-width="1.5" fill="none" stroke="currentColor"/>
        </svg>
      </div>
    </div>
  </div>
</template>

<style scoped>
.not-found-container {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 70vh; /* Ajuste selon la hauteur de ton header/footer */
  width: 100%;
  padding: 2rem;
}

.error-wrapper {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  z-index: 10; /* Force le passage au-dessus des éléments décoratifs */
}

.error-code {
  font-size: 8rem;
  font-weight: 900;
  margin: 0;
  line-height: 1;
  color: #1e2956;
}

.error-title {
  font-size: 2rem;
  color: #1e2956;
  margin: 1rem 0;
}

.error-description {
  color: #64748b;
  max-width: 450px;
  margin-bottom: 2rem;
}

.actions {
  display: flex;
  gap: 1rem;
  margin-bottom: 3rem;
}

.btn-primary {
  background: #1e2956;
  color: white;
  padding: 0.8rem 1.5rem;
  border-radius: 12px;
  text-decoration: none;
  font-weight: 600;
}

.btn-secondary {
  background: #f1f5f9;
  border: none;
  padding: 0.8rem 1.5rem;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  color: #1e2956;
}

.house-icon {
  width: 64px;
  height: 64px;
  stroke: #10b981;
}
</style>