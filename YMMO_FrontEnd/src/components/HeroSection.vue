<template>
  <section class="relative h-[500px] flex items-center justify-center text-white">
    <div class="absolute inset-0 bg-cover bg-center" style="background-image: url('https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?q=80&w=2000');">
      <div class="absolute inset-0 bg-black/50"></div>
    </div>

    <div class="relative z-10 text-center px-4">
      <h1 class="text-4xl md:text-5xl font-bold mb-4">Votre vie étudiante commence ici</h1>
      <p class="text-xl mb-8">Trouvez votre appartement en 48h chrono, même sans garant.</p>

      <div class="bg-white p-2 rounded-full shadow-lg max-w-xl mx-auto flex">
        <input type="text" placeholder="Où souhaitez-vous habiter ?" class="flex-grow px-6 py-3 text-gray-800 rounded-full outline-none" />
        <button class="bg-emerald-500 hover:bg-emerald-600 px-8 py-3 rounded-full font-bold transition">
          Rechercher
        </button>
      </div>
    </div>
  </section>
</template>