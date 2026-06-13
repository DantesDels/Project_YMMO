<template>
  <div>
    <h3 class="section-title">Mes offres</h3>
    <p class="text-muted" v-if="offers.length === 0">Aucune offre en cours pour le moment.</p>
    <div v-else class="offers-list" role="list">
      <div v-for="offer in offers" :key="offer.id" class="offer-card" role="listitem">
        <div class="offer-header">
          <h4>{{ offer.propertyTitle }}</h4>
          <span :class="['offer-status', offer.status]">{{ statusLabel(offer.status) }}</span>
        </div>
        <div class="offer-body">
          <div class="offer-meta">
            <span>Agent : <strong>{{ offer.agentName }}</strong></span>
            <span>Agence : {{ offer.agency }}</span>
            <span>Prix proposé : <strong>{{ offer.proposedPrice }} €</strong></span>
          </div>
          <p class="offer-message" v-if="offer.message">{{ offer.message }}</p>
        </div>
        <div class="offer-footer" v-if="offer.status === 'pending'">
          <button class="btn-accept" @click="respondOffer(offer.id, 'accepted')">Accepter</button>
          <button class="btn-decline" @click="respondOffer(offer.id, 'declined')">Refuser</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, onMounted } from 'vue'

interface Offer {
  id: number
  propertyTitle: string
  status: 'pending' | 'accepted' | 'declined' | 'negotiation'
  agentName: string
  agency: string
  proposedPrice: number
  message: string
}

const offers = reactive<Offer[]>([])

function statusLabel(status: string) {
  const labels: Record<string, string> = {
    pending: 'En attente',
    accepted: 'Acceptée',
    declined: 'Refusée',
    negotiation: 'En négociation',
  }
  return labels[status] ?? status
}

function respondOffer(id: number, status: 'accepted' | 'declined') {
  const offer = offers.find(o => o.id === id)
  if (offer) {
    offer.status = status
  }
}

onMounted(() => {
  offers.push(
    { id: 1, propertyTitle: 'Appartement 3 pièces - Paris 11e', status: 'pending', agentName: 'Sophie Martin', agency: 'Agence du Centre', proposedPrice: 325000, message: 'Bonjour, nous avons étudié votre demande et vous proposons une estimation à 325 000 €. Contactez-nous pour visiter.' },
    { id: 2, propertyTitle: 'Studio rénové - Lyon 3e', status: 'negotiation', agentName: 'Lucas Bernard', agency: 'ImmoLyon', proposedPrice: 142000, message: 'Nous sommes intéressés. Pouvons-nous fixer un rendez-vous cette semaine ?' },
  )
})
</script>

<style scoped>
.section-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1e2956;
  margin: 0 0 1.25rem;
}

.text-muted {
  color: #94a3b8;
}

.offers-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.offer-card {
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 1.25rem;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  transition: border-color 0.15s;
}

.offer-card:hover,
.offer-card:focus-within {
  border-color: #1e2956;
}

.offer-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.offer-header h4 {
  margin: 0;
  font-size: 1rem;
  font-weight: 700;
  color: #1e293b;
  word-break: break-word;
}

.offer-status {
  font-size: 0.75rem;
  font-weight: 700;
  padding: 0.25rem 0.625rem;
  border-radius: 999px;
  text-transform: uppercase;
  letter-spacing: 0.3px;
  white-space: nowrap;
}

.offer-status.pending {
  background: #fef9c3;
  color: #a16207;
}

.offer-status.accepted {
  background: #dcfce7;
  color: #166534;
}

.offer-status.declined {
  background: #fef2f2;
  color: #dc2626;
}

.offer-status.negotiation {
  background: #dbeafe;
  color: #1e40af;
}

.offer-body {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.offer-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem 1.5rem;
  font-size: 0.88rem;
  color: #64748b;
}

.offer-message {
  margin: 0;
  padding: 0.75rem;
  background: #f8fafc;
  border-radius: 8px;
  font-size: 0.88rem;
  color: #475569;
  font-style: italic;
  border-left: 3px solid #1e2956;
}

.offer-footer {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.btn-accept {
  padding: 0.5rem 1.25rem;
  background: #166534;
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.85rem;
  cursor: pointer;
  transition: background 0.15s;
  min-height: 44px;
}

.btn-accept:hover {
  background: #15803d;
}

.btn-accept:focus-visible {
  outline: 2px solid #166534;
  outline-offset: 2px;
}

.btn-decline {
  padding: 0.5rem 1.25rem;
  background: white;
  color: #dc2626;
  border: 1px solid #fca5a5;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.85rem;
  cursor: pointer;
  transition: all 0.15s;
  min-height: 44px;
}

.btn-decline:hover {
  background: #fef2f2;
}

.btn-decline:focus-visible {
  outline: 2px solid #dc2626;
  outline-offset: 2px;
}
</style>
