<template>
  <LegalLayout>
    <header class="help-header">
      <h1>Centre d'Aide</h1>
      <p class="subtitle">Comment pouvons-nous vous aider aujourd'hui ?</p>
    </header>

    <div class="faq-section">
      <div
          v-for="(item, index) in faqs"
          :key="index"
          class="faq-item"
          :class="{ active: activeIndex === index }"
          @click="toggleFaq(index)"
      >
        <div class="faq-question">
          <span>{{ item.question }}</span>
          <span class="icon">{{ activeIndex === index ? '−' : '+' }}</span>
        </div>
        <div class="faq-answer" v-if="activeIndex === index">
          <p>{{ item.answer }}</p>
        </div>
      </div>
    </div>

    <section class="contact-card">
      <h3>Vous ne trouvez pas de réponse ?</h3>
      <p class="updated">Notre équipe support est disponible du lundi au vendredi.</p>
      <AppButton to="/support" variant="primary">Contacter le Support</AppButton>
    </section>
  </LegalLayout>
</template>

<script setup>
import LegalLayout from '@/layouts/LegalLayout.vue';
import AppButton from '@/components/ui/AppButton.vue';
import { ref } from 'vue';

const activeIndex = ref(null);

const faqs = ref([
  {
    question: "Comment créer une alerte immobilière ?",
    answer: "Pour créer une alerte, effectuez une recherche avec vos critères préférés, puis cliquez sur le bouton 'Enregistrer la recherche' en haut des résultats."
  },
  {
    question: "Les services YMMO sont-ils gratuits ?",
    answer: "Oui, la consultation des annonces et la mise en relation avec les agents sont totalement gratuites pour les particuliers."
  },
  {
    question: "Comment contacter un agent immobilier ?",
    answer: "Sur chaque fiche de bien, vous trouverez un bouton 'Contacter l'agent' qui vous permettra d'envoyer un message ou de voir son numéro de téléphone."
  },
  {
    question: "Comment modifier mes informations personnelles ?",
    answer: "Rendez-vous dans votre espace 'Profil' après vous être connecté pour modifier votre email, mot de passe ou téléphone."
  }
]);

const toggleFaq = (index) => {
  activeIndex.value = activeIndex.value === index ? null : index;
};
</script>

<style scoped>
.help-header { text-align: center; margin-bottom: 3rem; }
.help-header h1 { color: #1e2956; font-size: 2.5rem; }
.help-header p { color: #64748b; font-size: 1.1rem; }

.faq-section { display: flex; flex-direction: column; gap: 1rem; }
.faq-item {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s;
}
.faq-item:hover { border-color: #10b981; }
.faq-item.active { border-color: #10b981; box-shadow: 0 4px 12px rgba(16, 185, 129, 0.1); }

.faq-question {
  padding: 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: 700;
  color: #1e2956;
}
.faq-answer { padding: 0 1.5rem 1.5rem; color: #475569; border-top: 1px solid #f1f5f9; padding-top: 1rem; }

.contact-card {
  margin-top: 4rem;
  background: #1e2956;
  color: white;
  padding: 2.5rem;
  border-radius: 20px;
  text-align: center;
}
.contact-card h3 { font-size: 1.5rem; margin-bottom: 1rem; }
</style>