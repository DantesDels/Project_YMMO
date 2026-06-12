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

    logout() {
      this.user = null;
      localStorage.removeItem('token');
      // Optionnel : redirect vers login
      window.location.href = '/login';
    }
  }
});