import { defineStore } from 'pinia';
import { jwtDecode } from 'jwt-decode';
import api from '@/api/axios';
import type {
  LoginRequest,
  RegisterRequest,
  AuthentificationUser,
  AuthentificationResponse,
  JwtPayload
} from '@/types';

// Token JWT mock pour le mode démo Google
const MOCK_GOOGLE_JWT = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJDbGllbnQiLCJ1c2VybmFtZSI6Ikdvb2dsZVVzZXIiLCJuYW1laWQiOiIxMjM0NTY3ODkwMTIzNDU2Nzg5MCIsImV4cCI6MTg5MzQ1NjAwMH0.rK0K0K0K0K0K0K0K0K0K0K0K0K0K0K0K0K0K0K0K0K0';

export const useAuthentificationStore = defineStore('authentification', {
  state: () => ({
    user: null as AuthentificationUser | null,
    isLoading: false,
    error: null as string | null,
  }),

  actions: {
    async login(credentials: LoginRequest) {
      this.isLoading = true;
      this.error = null;
      try {
        const { data } = await api.post<AuthentificationResponse>('/authentification/login', credentials);

        // Stockage du token
        localStorage.setItem('token', data.token);

        // Décodage du JWT pour extraire le rôle
        const decoded = jwtDecode<JwtPayload>(data.token);

        // Extraction du rôle selon le schéma Microsoft .NET
        const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

        this.user = {
          token: data.token,
          username: data.username,
          contactId: data.contactID,
          role: role
        };
      } catch (err: any) {
        this.error = "Erreur de connexion : vérifiez vos identifiants.";
      } finally {
        this.isLoading = false;
      }
    },

    async register(details: RegisterRequest) {
      this.isLoading = true;
      this.error = null;
      try {
        await api.post('/authentification/register', details);
      } catch (err: any) {
        this.error = "Erreur lors de l'inscription.";
      } finally {
        this.isLoading = false;
      }
    },

    async googleLogin(idToken?: string) {
      this.isLoading = true;
      this.error = null;
      try {
        // Mode réel : envoi du token Google au backend
        if (idToken) {
          const { data } = await api.post<AuthentificationResponse>('/authentification/google-login', { idToken });
          localStorage.setItem('token', data.token);
          const decoded = jwtDecode<JwtPayload>(data.token);
          const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
          this.user = { token: data.token, username: data.username, contactId: data.contactID, role };
          return;
        }

        // Mode mock : simuler une connexion Google
        await new Promise(r => setTimeout(r, 800));
        localStorage.setItem('token', MOCK_GOOGLE_JWT);
        const decoded = jwtDecode<JwtPayload>(MOCK_GOOGLE_JWT);
        const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        this.user = {
          token: MOCK_GOOGLE_JWT,
          username: 'GoogleUser',
          contactId: '12345678901234567890',
          role,
        };
      } catch (err: any) {
        this.error = "Erreur lors de la connexion avec Google.";
      } finally {
        this.isLoading = false;
      }
    },

    logout() {
      this.user = null;
      localStorage.removeItem('token');
      // Optionnel : redirect vers login
      window.location.href = '/login';
    }
  }
});