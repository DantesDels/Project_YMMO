// ─────────────────────────────────────────────────────────────
//  src/services/offer.service.ts
//  Mappé sur OfferController.cs
// ─────────────────────────────────────────────────────────────

import api from '@/api/axios'
import type { CreateOfferDto, OfferResponseDto, UpdateStatusOfferDto } from '@/types'

export const offerService = {

  // POST /api/offer  — Client, Agent, Manager, Admin
  create(dto: CreateOfferDto): Promise<OfferResponseDto> {
    return api.post<OfferResponseDto>('/offer', dto).then(r => r.data)
  },

  // GET /api/offer/{id}?propertyId=...  — Agent/Manager/Admin
  // ATTENTION : propertyId est un query param obligatoire côté backend
  getById(offerId: string, propertyId: string): Promise<OfferResponseDto> {
    return api
      .get<OfferResponseDto>(`/offer/${offerId}`, { params: { propertyId } })
      .then(r => r.data)
  },

  // PUT /api/offer/{id}/status  — Agent/Manager/Admin
  updateStatus(offerId: string, dto: UpdateStatusOfferDto): Promise<void> {
    return api.put(`/offer/${offerId}/status`, dto).then(() => undefined)
  },
}

// ─────────────────────────────────────────────────────────────
//  src/services/client.service.ts
//  Mappé sur ClientController.cs
// ─────────────────────────────────────────────────────────────

import type {
  RegisterClientDto,
  UpdateClientDto,
  AddWishlistDto,
  RemoveWishlistDto,
} from '@/types'

export const clientService = {

  // POST /api/client/register  (public, sans JWT)
  register(dto: RegisterClientDto): Promise<void> {
    return api.post('/client/register', dto).then(() => undefined)
  },

  // GET /api/client/{id}  — Client, Admin
  getProfile(id: string): Promise<unknown> {
    return api.get(`/client/${id}`).then(r => r.data)
  },

  // PUT /api/client/{id}
  updateProfile(id: string, dto: UpdateClientDto): Promise<unknown> {
    return api.put(`/client/${id}`, dto).then(r => r.data)
  },

  // POST /api/client/wishlist  — Client
  addToWishlist(dto: AddWishlistDto): Promise<void> {
    return api.post('/client/wishlist', dto).then(() => undefined)
  },

  // DELETE /api/client/wishlist  — Client
  removeFromWishlist(dto: RemoveWishlistDto): Promise<void> {
    return api.delete('/client/wishlist', { data: dto }).then(() => undefined)
  },
}

// ─────────────────────────────────────────────────────────────
//  src/services/agency.service.ts
//  Mappé sur AgencyController.cs
// ─────────────────────────────────────────────────────────────

import type { AgencyDto, UpdateAgencyDto } from '@/types'

export const agencyService = {

  // GET /api/agency  — public
  getAll(): Promise<AgencyDto[]> {
    return api.get<AgencyDto[]>('/agency').then(r => r.data)
  },

  // PUT /api/agency/{id}  — Manager, Admin
  update(id: string, dto: UpdateAgencyDto): Promise<void> {
    return api.put(`/agency/${id}`, dto).then(() => undefined)
  },
}
