<template>
  <section class="hero">
    <img
        src="/images/hero-bg.webp"
        alt="Vue d'une propriété de prestige YMMO"
        class="hero-bg-img"
        fetchpriority="high"
        loading="eager"
        width="1920"
        height="1080"
    />
    <div class="hero-overlay"></div>

    <div class="hero-content">
      <h1>
        L'Excellence
        <span class="highlight">Immobilière</span>
      </h1>

      <span class="badge">Groupe Immobilier depuis 2025</span>

      <p class="subtitle">
        YMMO vous accompagne dans l'achat, la vente et l'analyse de vos biens
        grâce à une plateforme centralisée et des outils d'intelligence artificielle.
      </p>

      <div class="cta-group">
        <AppButton to="/catalog" variant="primary" class="cta-main">
          Découvrir le catalogue
        </AppButton>
        <AppButton to="/market-analysis" variant="secondary" class="cta-secondary">
          Analyser le marché
        </AppButton>
      </div>
    </div>

    <div class="hero-stats">
      <div class="stat-item" v-for="stat in stats" :key="stat.label">
        <span class="stat-value">{{ stat.value }}</span>
        <span class="stat-label">{{ stat.label }}</span>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import AppButton from '@/components/ui/AppButton.vue';
import { getFullAnalysis } from '@/api/datascience';

const stats = ref<{ value: string; label: string }[]>([
  { value: '…', label: 'Biens analysés' },
  { value: '…', label: 'Prix moyen' },
  { value: '…', label: 'Villes couvertes' },
  { value: '…', label: 'Types de biens' },
])

onMounted(async () => {
  try {
    const d = await getFullAnalysis({ period: 'yearly' })
    const s = d.trends.summary
    const fmt = (n: number) => n >= 1_000_000 ? (n / 1_000_000).toFixed(1) + 'M' : n >= 1_000 ? (n / 1_000).toFixed(0) + 'k' : String(n)
    stats.value = [
      { value: fmt(s.totalListings), label: 'Biens analysés' },
      { value: s.globalAvgPrice.toLocaleString('fr-FR') + ' €', label: 'Prix moyen' },
      { value: String(d.zones.zones.length), label: 'Villes couvertes' },
      { value: String(d.popular.types.length), label: 'Types de biens' },
    ]
  } catch {
    /* laisse les '…' si erreur */
  }
})
</script>

<style scoped>
.hero {
  position: relative;
  min-height: 85vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  color: white;
  text-align: center;
  padding: 2rem;
  overflow: hidden;
}

/* Remplacement du .hero-bg par .hero-bg-img */
.hero-bg-img {
  position: absolute;
  top: 0; left: 0;
  width: 100%; height: 100%;
  object-fit: cover;
  object-position: center;
  z-index: -2;
}

.hero-overlay {
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  background: linear-gradient(
      180deg,
      rgba(10, 14, 30, 0.35) 0%,
      rgba(10, 14, 30, 0.55) 45%,
      rgba(10, 14, 30, 0.75) 100%
  );
  z-index: -1;
}

.hero-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  max-width: 720px;
}

.badge {
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.25);
  color: rgba(255, 255, 255, 0.95);
  padding: 0.45rem 1.1rem;
  border-radius: 999px;
  font-size: 0.8rem;
  font-weight: 600;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  margin-bottom: 1.75rem;
  display: inline-block;
  backdrop-filter: blur(8px);
}

h1 {
  font-family: 'Playfair Display', serif;
  font-size: clamp(2.75rem, 6vw, 4.75rem);
  font-weight: 700;
  color: white;
  line-height: 1.1;
  margin: 0 0 1.5rem;
  text-shadow: 0 2px 24px rgba(0, 0, 0, 0.45);
}

.highlight {
  display: block;
  color: #e8c87e;
}

.subtitle {
  max-width: 580px;
  font-size: 1.15rem;
  font-weight: 400;
  color: rgba(255, 255, 255, 0.92);
  margin-bottom: 2.75rem;
  line-height: 1.7;
}

.cta-group {
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
}

.hero-stats {
  position: absolute;
  bottom: 2.5rem;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: clamp(1.5rem, 4vw, 4rem);
  background: rgba(10, 14, 30, 0.55);
  backdrop-filter: blur(20px);
  padding: 1.25rem clamp(1.5rem, 4vw, 3rem);
  border-radius: 14px;
  border: 1px solid rgba(255, 255, 255, 0.15);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.stat-value {
  font-size: 1.7rem;
  font-weight: 800;
  display: block;
  color: white;
}

.stat-label {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.12em;
  color: rgba(255, 255, 255, 0.75);
  margin-top: 0.2rem;
}

/* ── Responsive ─────────────────────────────────────────────── */
@media (max-width: 640px) {
  .hero-stats {
    flex-wrap: wrap;
    justify-content: center;
    gap: 1.5rem 2rem;
    bottom: 1.5rem;
  }
  .cta-group {
    flex-direction: column;
    width: 100%;
  }
  .cta-group :deep(.btn) {
    width: 100%;
  }
}
</style>