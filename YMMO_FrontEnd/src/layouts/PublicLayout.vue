<template>
  <div class="flex flex-col min-h-screen bg-[#f3f4f6]">
    <nav class="sticky top-0 z-50 bg-[#16171d] text-white p-4">
      <div class="max-w-[1400px] mx-auto flex items-center justify-between">
        <RouterLink to="/" class="text-2xl font-bold">YMMO</RouterLink>
        <div class="space-x-4">
          <RouterLink to="/authentification" class="px-4 py-2 bg-white text-gray-900 rounded-lg">Se connecter</RouterLink>
        </div>
      </div>
    </nav>

    <main class="flex-grow">
      <RouterView />
    </main>

    <footer class="text-center p-8 bg-[#16171d] text-white mt-12">
      © 2026 YMMO - Application Immobilière
    </footer>
  </div>
</template>