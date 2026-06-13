// ─────────────────────────────────────────────────────────────
//  YMMO — Types TypeScript
//  Miroir exact des DTOs C# backend .NET 10
//  Mis à jour depuis les controllers & DTOs réels
// ─────────────────────────────────────────────────────────────

// ── Enums (miroir des enums Domain C#) ───────────────────────

export enum PropertyType {
  Apartment  = 'Apartment',
  House      = 'House',
  Commercial = 'Commercial',
  Land       = 'Land',
}

export enum PhysicalCondition {
  New          = 'New',
  GoodCondition = 'GoodCondition',
  ToRenovate   = 'ToRenovate',
  ToDestroy    = 'ToDestroy',
}

export enum EnergyClass {
  A = 'A',
  B = 'B',
  C = 'C',
  D = 'D',
  E = 'E',
  F = 'F',
  G = 'G',
}

export enum StatusOffer {
  Pending  = 'Pending',
  Accepted = 'Accepted',
  Rejected = 'Rejected',
  Revised  = 'Revised',
}

export enum Criteria {
  Parking   = 'Parking',
  Garden    = 'Garden',
  Pool      = 'Pool',
  Terrace   = 'Terrace',
  Elevator  = 'Elevator',
  Cellar    = 'Cellar',
  Furnished = 'Furnished',
}

// ── Auth ─────────────────────────────────────────────────────
// POST /api/authentification/register
export interface RegisterRequest {
  username    : string
  lastName    : string
  email       : string
  phoneNumber : string
  password    : string
}

// POST /api/authentification/login
export interface LoginRequest {
  email    : string
  password : string
}

// Réponse login & register  — ContactID = userId dans le JWT
// ATTENTION : pas de "role" dans la réponse → on le décode depuis le JWT
export interface AuthentificationResponse {
  token     : string
  username  : string
  contactID : string   // Guid sérialisé en string
}

// Payload décodé du JWT (claims .NET)
export interface JwtPayload {
  username              : string
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string
  nameid                : string
  exp                   : number
}

// État auth côté store
export interface AuthentificationUser {
  token     : string
  username  : string
  contactId : string
  role      : string   // 'Client' | 'Agent' | 'Manager' | 'Admin'
  firstName?: string
  lastName? : string
  email?    : string
  phone?    : string
  address?  : string
  zipCode?  : string
  city?     : string
  bio?      : string
}

// ── Location ─────────────────────────────────────────────────
export interface LocationDto {
  street     : string
  city       : string
  postalCode : string
  region     : string
  country    : string
}

export interface CreateLocationDto {
  street     : string
  city       : string
  postalCode : string
  region     : string
  country    : string
}

// ── Property Picture ─────────────────────────────────────────
export interface PropertyPictureDto {
  propertyPictureId : string
  url               : string
  displayOrder      : number
  isMain            : boolean
}

// ── Agent Contact (imbriqué dans PropertyDetailDto) ──────────
export interface AgentContactDto {
  agentId     : string
  firstName   : string
  lastName    : string
  email       : string
  phoneNumber : string
  agencyName  : string
}

// ── Property Summary (liste / search results) ────────────────
// Retourné par GET /api/property et GET /api/search
export interface PropertySummaryDto {
  propertyId   : string
  propertyName : string
  propertyType : string       // sérialisé en string par .NET
  condition    : string
  currentPrice : number
  surface      : number
  city         : string
  postalCode   : string
  mainFeatures : string[]
}

// ── Property Detail (fiche complète) ─────────────────────────
// Retourné par GET /api/property/{id}
export interface PropertyDetailDto {
  propertyId          : string
  propertyName        : string
  propertyDescription : string | null
  yearBuilt           : number
  propertyType        : string
  condition           : string
  energyClass         : string
  currentPrice        : number
  initialPrice        : number
  surface             : number
  dateListed          : string   // ISO date
  features            : string[]
  location            : LocationDto
  agent               : AgentContactDto
  pictures            : PropertyPictureDto[]
}

// ── Create Property ──────────────────────────────────────────
// POST /api/property  — Rôle Agent/Manager/Admin
export interface CreatePropertyDto {
  propertyName        : string
  propertyDescription : string | null
  propertyType        : PropertyType
  yearBuilt           : number
  condition           : PhysicalCondition
  energyClass         : EnergyClass
  initialPrice        : number
  surface             : number
  features            : Criteria[]
  location            : CreateLocationDto
  agencyId            : string   // Guid
  agentId             : string   // Guid
  sellerId            : string   // Guid
}

// ── Update Property ──────────────────────────────────────────
// PUT /api/property/{id}
export interface UpdatePropertyDto {
  propertyName        : string
  propertyDescription : string | null
  currentPrice        : number
  condition           : PhysicalCondition
  energyClass         : EnergyClass
  features            : Criteria[]
}

// ── Search Criteria ──────────────────────────────────────────
// GET /api/search?city=...&minPrice=...  (query params)
export interface PropertySearchCriteriaDto {
  city             ?: string
  region           ?: string
  type             ?: PropertyType
  minPrice         ?: number
  maxPrice         ?: number
  minSurface       ?: number
  maxSurface       ?: number
  conditions       ?: PhysicalCondition[]
  energyClasses    ?: EnergyClass[]
  rooms            ?: number[]
  requiredCriteria ?: Criteria[]
  pageNumber        : number
  pageSize          : number
}

// ── Offer ────────────────────────────────────────────────────
// POST /api/offer
export interface CreateOfferDto {
  propertyID : string   // Guid
  clientID   : string   // Guid — userId depuis le store auth
  offerPrice : number
}

// PUT /api/offer/{id}/status
export interface UpdateStatusOfferDto {
  newStatus : StatusOffer
}

// Réponse offre (GET /api/offer/{id}?propertyId=...)
export interface OfferResponseDto {
  offerID          : string
  propertyID       : string
  clientID         : string
  clientLastName   : string
  clientFirstName  : string
  clientPhoneNumber: string
  offerPrice       : number
  statusOffer      : string   // "Pending" | "Accepted" | "Rejected"
  propertyCity     : string
  propertyRegion   : string
}

// ── Client ───────────────────────────────────────────────────
// POST /api/client/register  (public, sans JWT)
export interface RegisterClientDto {
  username    : string
  email       : string
  phoneNumber : string
  password    : string
}

export interface UpdateClientDto {
  username    ?: string
  email       ?: string
  phoneNumber ?: string
}

// ── Wishlist ─────────────────────────────────────────────────
// POST /api/client/wishlist
export interface AddWishlistDto {
  propertyID : string   // Guid — casse exacte du DTO C#
  clientID   : string   // Guid
}

// DELETE /api/client/wishlist
export interface RemoveWishlistDto {
  propertyID : string   // Guid
  clientID   : string   // Guid
}

// ── Agency ───────────────────────────────────────────────────
export interface AgencyDto {
  agencyId : string
  name     : string
  city     : string
  address  : string
  phone    : string
}

export interface UpdateAgencyDto {
  name    ?: string
  city    ?: string
  address ?: string
  phone   ?: string
}

// ── Analytics (données mockées côté frontend pour lundi) ─────
export interface MarketTrendDto {
  month            : string   // "2025-01"
  averagePrice     : number
  transactionCount : number
  city             : string
}

export interface ZoneScoreDto {
  city           : string
  demandScore    : number   // 0-100
  averagePrice   : number
  priceEvolution : number   // % sur 12 mois
}

export interface AiMessageDto {
  role    : 'user' | 'assistant'
  content : string
}
