import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path' // Assure-toi d'avoir installé @types/node

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
})