<template>
  <LegalLayout>
    <header class="legal-header">
    <h1>Mentions Légales</h1>
    <p class="updated">Dernière mise à jour : 13 juin 2026</p>
    </header>
    
    <h2>1. Éditeur du site</h2>
    <p>Le site YMMO est édité par la société YMMO SAS, au capital de 10 000€...</p>

    <h2>2. Hébergement</h2>
    <p>Le site est hébergé par [Nom de l'hébergeur], situé à [Adresse].</p>

    <h2>3. Propriété intellectuelle</h2>
    <p>Tous les contenus présents sur le site YMMO sont protégés...</p>
  </LegalLayout>
</template>

<script setup>
import LegalLayout from '@/layouts/LegalLayout.vue';
</script>

<style scoped>
h1 {
  color: #1e2956;
  margin-bottom: 0.5rem;
  font-size: clamp(1.5rem, 4.5vw, 2.5rem);
  overflow-wrap: break-word;
  word-break: break-word;
}
h2 { color: #10b981; margin-top: 2rem; margin-bottom: 1rem; }
p { color: #475569; line-height: 1.7; }
.updated { font-size: 0.9rem; color: #94a3b8; margin-bottom: 2rem; }
</style>