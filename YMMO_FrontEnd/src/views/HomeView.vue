<template>
  <div class="bg-gray-50">
    <HeroSection />

    <CityGrid />

    <section class="max-w-6xl mx-auto py-12 px-4 grid md:grid-cols-2 gap-8">
      <FeatureCard title="Votre location est simplifiée" description="Du dossier à la visite, tout est en ligne." />
      <FeatureCard title="Votre location est sécurisée" description="Une protection complète pour vous et le propriétaire." />
    </section>
  </div>
</template>

<script setup>
import HeroSection from '@/components/HeroSection.vue';
import CityGrid from '@/components/CityGrid.vue';
import FeatureCard from '@/components/FeatureCard.vue';
</script>