<template>
  <LegalLayout>
    <header class="legal-header">
      <h1>Politique de Confidentialité</h1>
      <p class="subtitle">Nous protégeons vos données immobilières</p>
    </header>

    <main class="legal-content">
      <section class="info-box">
        <p>Chez YMMO, la protection de vos données personnelles est une priorité. Cette politique vous explique quelles données nous collectons et comment nous les traitons.</p>
      </section>

      <h2>1. Données collectées</h2>
      <p>Nous collectons les informations que vous nous fournissez directement : Nom, prénom, adresse e-mail, numéro de téléphone et critères de recherche immobilière.</p>

      <h2>2. Utilisation des données</h2>
      <p>Vos données sont utilisées pour :</p>
      <ul>
        <li>Gérer votre compte utilisateur</li>
        <li>Vous mettre en relation avec des agents</li>
        <li>Améliorer nos algorithmes de recommandation de biens</li>
      </ul>

      <h2>3. Vos droits (RGPD)</h2>
      <p>Conformément à la réglementation, vous disposez d'un droit d'accès, de rectification, d'opposition et de suppression de vos données personnelles.</p>
    </main>
  </LegalLayout>
</template>

<script setup>
import LegalLayout from '@/layouts/LegalLayout.vue';
</script>

<style scoped>
.legal-header { margin-bottom: 3rem; }
.legal-header h1 { font-size: 2.5rem; color: #1e2956; margin-bottom: 0.5rem; }
.subtitle { color: #64748b; font-size: 1.1rem; }

.info-box {
  background: #f0fdf4;
  border-radius: 12px;
  padding: 1.5rem;
  border: 1px solid #bbf7d0;
  color: #166534;
  margin-bottom: 2rem;
}

h2 { color: #10b981; margin-top: 2rem; }
.legal-content p { color: #475569; line-height: 1.7; }
</style>