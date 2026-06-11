// ─────────────────────────────────────────────────────────────
//  src/api/axios.ts
//  Instance Axios centralisée pour YMMO
//  - baseURL depuis .env (VITE_API_URL)
//  - Injection automatique du Bearer token
//  - Auto-logout sur 401
// ─────────────────────────────────────────────────────────────

import axios from 'axios'
import type { AxiosInstance } from 'axios'

const api: AxiosInstance = axios.create({
  baseURL : import.meta.env.VITE_API_URL ?? 'http://localhost:5000/api',
  headers : { 'Content-Type': 'application/json' },
})

// ── Intercepteur REQUEST : injecte le JWT ────────────────────
// On lit directement le localStorage pour éviter l'import
// circulaire (store → axios → store)
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('ymmo_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// ── Intercepteur RESPONSE : gère les erreurs globales ────────
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expiré ou invalide → on nettoie et redirige
      ;['ymmo_token', 'ymmo_username', 'ymmo_role', 'ymmo_contactId']
        .forEach(k => localStorage.removeItem(k))
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default api
