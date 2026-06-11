// ─────────────────────────────────────────────────────────────
//  src/services/property.service.ts
//  Mappé sur PropertyController.cs + SearchController.cs
// ─────────────────────────────────────────────────────────────

import api from '@/api/axios'
import type {
  PropertySummaryDto,
  PropertyDetailDto,
  CreatePropertyDto,
  UpdatePropertyDto,
  PropertyPictureDto,
  PropertySearchCriteriaDto,
} from '@/types'

export const propertyService = {

  // GET /api/property
  getAll(): Promise<PropertySummaryDto[]> {
    return api.get<PropertySummaryDto[]>('/property').then(r => r.data)
  },

  // GET /api/property/{id}
  getById(id: string): Promise<PropertyDetailDto> {
    return api.get<PropertyDetailDto>(`/property/${id}`).then(r => r.data)
  },

  // POST /api/property  — Agent/Manager/Admin
  create(dto: CreatePropertyDto): Promise<PropertyDetailDto> {
    return api.post<PropertyDetailDto>('/property', dto).then(r => r.data)
  },

  // PUT /api/property/{id}
  update(id: string, dto: UpdatePropertyDto): Promise<void> {
    return api.put(`/property/${id}`, dto).then(() => undefined)
  },

  // DELETE /api/property/{id}
  delete(id: string): Promise<void> {
    return api.delete(`/property/${id}`).then(() => undefined)
  },

  // POST /api/property/{id}/pictures
  addPicture(propertyId: string, dto: PropertyPictureDto): Promise<{ id: string }> {
    return api.post<{ id: string }>(`/property/${propertyId}/pictures`, dto).then(r => r.data)
  },

  // DELETE /api/property/{id}/pictures/{pictureId}
  deletePicture(propertyId: string, pictureId: string): Promise<void> {
    return api.delete(`/property/${propertyId}/pictures/${pictureId}`).then(() => undefined)
  },
}

// ─────────────────────────────────────────────────────────────
//  src/services/search.service.ts
//  Mappé sur SearchController.cs
//  IMPORTANT : critères en query params ([FromQuery])
// ─────────────────────────────────────────────────────────────

export const searchService = {

  // GET /api/search?city=...&minPrice=...
  search(criteria: Partial<PropertySearchCriteriaDto>): Promise<PropertySummaryDto[]> {
    // On filtre les valeurs undefined pour ne pas polluer l'URL
    const params = Object.fromEntries(
      Object.entries(criteria).filter(([, v]) => v !== undefined && v !== null && v !== '')
    )
    return api.get<PropertySummaryDto[]>('/search', { params }).then(r => r.data)
  },

  // GET /api/search/suggestions/{propertyId}
  getSuggestions(propertyId: string): Promise<PropertySummaryDto[]> {
    return api.get<PropertySummaryDto[]>(`/search/suggestions/${propertyId}`).then(r => r.data)
  },
}
