// ─────────────────────────────────────────────────────────────
//  src/stores/auth.store.ts
//  Store Pinia d'authentification YMMO
//
//  IMPORTANT : L'AuthentificationResponse C# ne retourne PAS
//  le rôle directement. On le décode depuis le payload JWT.
//  Claim .NET : "http://schemas.microsoft.com/ws/2008/06/
//               identity/claims/role"
// ─────────────────────────────────────────────────────────────

import { defineStore } from 'pinia'
import { ref, computed }  from 'vue'
import api from '@/api/axios'
import type {
  LoginRequest,
  RegisterRequest,
  AuthentificationResponse,
  AuthUser,
} from '@/types'

// Clé du claim de rôle dans un JWT généré par .NET
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

// Décode le payload JWT (base64url) sans librairie externe
function decodeJwt(token: string): Record<string, unknown> {
  try {
    const payload = token.split('.')[1]
    const base64  = payload.replace(/-/g, '+').replace(/_/g, '/')
    return JSON.parse(atob(base64))
  } catch {
    return {}
  }
}

const KEYS = {
  token     : 'ymmo_token',
  username  : 'ymmo_username',
  role      : 'ymmo_role',
  contactId : 'ymmo_contactId',
}

export const useAuthStore = defineStore('auth', () => {

  // ── State (hydraté depuis localStorage au démarrage) ───────
  const token     = ref<string | null>(localStorage.getItem(KEYS.token))
  const username  = ref<string | null>(localStorage.getItem(KEYS.username))
  const role      = ref<string | null>(localStorage.getItem(KEYS.role))
  const contactId = ref<string | null>(localStorage.getItem(KEYS.contactId))

  // ── Getters ────────────────────────────────────────────────
  const isAuthenticated = computed(() => !!token.value)
  const isAgent         = computed(() => ['Agent', 'Manager', 'Admin'].includes(role.value ?? ''))
  const isClient        = computed(() => role.value === 'Client')
  const user            = computed<AuthUser | null>(() =>
    token.value
      ? { token: token.value, username: username.value!, role: role.value!, contactId: contactId.value! }
      : null
  )

  // ── Actions ────────────────────────────────────────────────

  async function login(dto: LoginRequest): Promise<void> {
    const { data } = await api.post<AuthentificationResponse>(
      '/authentification/login', dto
    )
    _hydrateFromResponse(data)
  }

  async function register(dto: RegisterRequest): Promise<void> {
    const { data } = await api.post<AuthentificationResponse>(
      '/authentification/register', dto
    )
    _hydrateFromResponse(data)
  }

  function logout(): void {
    token.value = username.value = role.value = contactId.value = null
    Object.values(KEYS).forEach(k => localStorage.removeItem(k))
  }

  // ── Privé : persiste et hydrate l'état depuis la réponse ───
  function _hydrateFromResponse(data: AuthentificationResponse): void {
    const payload = decodeJwt(data.token)

    token.value     = data.token
    username.value  = data.username
    contactId.value = data.contactID
    role.value      = (payload[ROLE_CLAIM] as string) ?? 'Client'

    localStorage.setItem(KEYS.token,     data.token)
    localStorage.setItem(KEYS.username,  data.username)
    localStorage.setItem(KEYS.contactId, data.contactID)
    localStorage.setItem(KEYS.role,      role.value)
  }

  return {
    // State (read-only depuis l'extérieur)
    token, username, role, contactId,
    // Getters
    isAuthenticated, isAgent, isClient, user,
    // Actions
    login, register, logout,
  }
})
