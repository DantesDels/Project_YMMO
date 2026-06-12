<template>
  <div class="bg-gray-50">
    <HeroSection />

    <div class="max-w-4xl mx-auto -mt-8 bg-white p-6 rounded-xl shadow-lg flex justify-around mb-12">
      <div><strong>1.5 million</strong><br/>utilisateurs</div>
      <div><strong>+180</strong><br/>écoles</div>
      <div><strong>+180,000</strong><br/>annonces</div>
    </div>

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