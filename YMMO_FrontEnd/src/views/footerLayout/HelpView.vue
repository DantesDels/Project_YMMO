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
      <p class="schedule-title">Horaires d'ouverture de nos agences</p>
      <div class="schedule">
        <div
          v-for="day in schedule"
          :key="day.label"
          :class="['day-row', { today: day.isToday, active: day.isOpen }]"
        >
          <span class="day-label">{{ day.label }}</span>
          <span class="day-hours">{{ day.hours }}</span>
          <span v-if="day.isToday && day.isOpen" class="status-badge open">Ouvert</span>
          <span v-if="day.isToday && !day.isOpen" class="status-badge closed">Fermé</span>
        </div>
      </div>
      <AppButton to="/support" variant="primary">Contacter le Support</AppButton>
    </section>
  </LegalLayout>
</template>

<script setup>
import LegalLayout from '@/layouts/LegalLayout.vue';
import AppButton from '@/components/ui/AppButton.vue';
import { ref, computed } from 'vue';

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

const days = ['Dimanche', 'Lundi', 'Mardi', 'Mercredi', 'Jeudi', 'Vendredi', 'Samedi'];
const todayIndex = new Date().getDay();

const schedule = computed(() => [
  { label: 'Lundi',     hours: '09h00 – 12h30 · 14h00 – 18h00', isOpen: true  },
  { label: 'Mardi',     hours: '09h00 – 12h30 · 14h00 – 18h00', isOpen: true  },
  { label: 'Mercredi',  hours: '09h00 – 12h30 · 14h00 – 18h00', isOpen: true  },
  { label: 'Jeudi',     hours: '09h00 – 12h30 · 14h00 – 18h00', isOpen: true  },
  { label: 'Vendredi',  hours: '09h00 – 12h30 · 14h00 – 17h00', isOpen: true  },
  { label: 'Samedi',    hours: '10h00 – 13h00',                 isOpen: true  },
  { label: 'Dimanche',  hours: 'Fermé',                         isOpen: false },
].map((d, i) => ({ ...d, isToday: i + 1 === todayIndex || (i === 6 && todayIndex === 0) })));
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
  padding: 1.75rem 2rem;
  border-radius: 20px;
  text-align: center;
}
.contact-card h3 { font-size: 1.5rem; margin-bottom: 1rem; }

.schedule-title {
  font-size: 1.1rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.95);
  text-align: center;
  margin-bottom: 1rem;
}

.schedule {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin: 0 auto 1.5rem;
  max-width: 420px;
}

.day-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.625rem 1rem;
  border-radius: 10px;
  background: rgba(255, 255, 255, 0.06);
  transition: background 0.2s;
}

.day-row.today {
  background: rgba(16, 185, 129, 0.15);
  outline: 1px solid rgba(16, 185, 129, 0.3);
}

.day-label {
  width: 5.5rem;
  font-weight: 600;
  font-size: 0.9rem;
  text-align: left;
  color: rgba(255, 255, 255, 0.9);
}

.day-hours {
  flex: 1;
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.65);
  text-align: center;
}

.status-badge {
  font-size: 0.7rem;
  font-weight: 700;
  padding: 0.2rem 0.6rem;
  border-radius: 999px;
  white-space: nowrap;
}

.status-badge.open {
  background: #10b981;
  color: white;
}

.status-badge.closed {
  background: #ef4444;
  color: white;
}
</style>