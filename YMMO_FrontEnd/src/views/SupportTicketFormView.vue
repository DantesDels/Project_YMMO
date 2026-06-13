<template>
  <div class="support-container">
    <header class="support-header">
      <h1>Support YMMO</h1>
      <p>Une question ou un problème ? Notre équipe est là pour vous aider.</p>
    </header>

    <div class="support-grid">
      <section class="ticket-form">
        <h2>Créer un nouveau ticket</h2>
        <form @submit.prevent="submitTicket">
          <div class="form-group">
            <label for="subject">Objet</label>
            <input id="subject" v-model="form.subject" type="text" placeholder="Ex: Problème de connexion" required />
          </div>

          <div class="form-group">
            <label for="category">Catégorie</label>
            <select id="category" v-model="form.category" required>
              <option value="technical">Technique</option>
              <option value="account">Mon compte</option>
              <option value="other">Autre</option>
            </select>
          </div>

          <div class="form-group">
            <label for="message">Description</label>
            <textarea id="message" v-model="form.message" rows="5" placeholder="Décrivez votre situation..." required></textarea>
          </div>

          <button type="submit" class="btn-submit">Envoyer le ticket</button>
        </form>
      </section>

      <section class="ticket-list">
        <h2>Vos tickets en cours</h2>
        <div v-if="tickets.length === 0" class="empty-list">Aucun ticket actif.</div>
        <div v-for="ticket in tickets" :key="ticket.id" class="ticket-card">
          <div class="ticket-info">
            <span class="ticket-id">#{{ ticket.id }}</span>
            <h3>{{ ticket.subject }}</h3>
            <span :class="['status', ticket.status]">{{ ticket.statusLabel }}</span>
          </div>
          <p>{{ ticket.date }}</p>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const form = ref({ subject: '', category: 'technical', message: '' });
const tickets = ref([
  { id: '1024', subject: 'Problème de filtre Budget', status: 'pending', statusLabel: 'En attente', date: '13/06/2026' }
]);

const submitTicket = () => {
  // Logique d'appel API ici
  alert('Ticket envoyé avec succès !');
  form.value = { subject: '', category: 'technical', message: '' };
};
</script>

<style scoped>
.support-container { max-width: 1000px; margin: 4rem auto; padding: 0 2rem; }
.support-header { text-align: center; margin-bottom: 3rem; }
.support-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 3rem; }

.form-group { margin-bottom: 1.5rem; display: flex; flex-direction: column; }
label { font-weight: 600; margin-bottom: 0.5rem; color: #1e2956; }
input, select, textarea { padding: 0.8rem; border: 1px solid #e2e8f0; border-radius: 8px; }

.btn-submit { background: #10b981; color: white; border: none; padding: 1rem; border-radius: 8px; font-weight: 600; cursor: pointer; width: 100%; }

.ticket-card { padding: 1.5rem; border: 1px solid #e2e8f0; border-radius: 12px; margin-bottom: 1rem; }
.status { font-size: 0.75rem; padding: 2px 8px; border-radius: 4px; font-weight: 700; }
.status.pending { background: #fef3c7; color: #92400e; }
</style>