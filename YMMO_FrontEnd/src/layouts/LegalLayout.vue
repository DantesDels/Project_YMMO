<template>
  <main class="legal-page">
    <div class="legal-container">
      <AppButton to="/" variant="ghost" class="back-link">
        ← Retour à l'accueil
      </AppButton>

      <div class="legal-content">
        <slot />
      </div>
    </div>
  </main>
</template>

<script setup>
import AppButton from '@/components/ui/AppButton.vue';
</script>

<style>
.legal-page { padding: 4rem 1rem; background: #f8fafc; min-height: 100vh; }

.legal-container {
  max-width: 800px; margin: 0 auto; background: white;
  padding: 3rem; border-radius: 16px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1);
}

.back-link { margin-bottom: 2rem; display: inline-block; }

.legal-content h1 {
  font-size: 2.5rem; color: #1e2956; margin-bottom: 0.5rem;
  text-align: center; /* On garde le titre de page centré */
}

.legal-content .subtitle {
  color: #64748b; text-align: center; margin-bottom: 3rem;
}

.legal-content .updated {
  text-align: center;
}

.legal-content h2 {
  color: #10b981;
  margin-top: 2.5rem;
  border-left: 4px solid #10b981;
  padding-left: 1rem;
  text-align: left;
}

.legal-content p { margin: 1rem 0; color: #475569; line-height: 1.7; text-align: left; }
.legal-content ul { padding-left: 1.5rem; color: #475569; text-align: left; }
</style>