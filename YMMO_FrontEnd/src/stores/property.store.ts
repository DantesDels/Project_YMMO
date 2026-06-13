// ─────────────────────────────────────────────────────────────
//  src/stores/property.store.ts
// ─────────────────────────────────────────────────────────────

import { defineStore } from 'pinia'
import { ref } from 'vue'
import { propertyService, searchService } from '@/services/property.service'
import type {
  PropertySummaryDto,
  PropertyDetailDto,
  PropertySearchCriteriaDto,
  CreatePropertyDto,
  UpdatePropertyDto,
} from '@/types'

export const usePropertyStore = defineStore('property', () => {

  // ── State ──────────────────────────────────────────────────
  const properties      = ref<PropertySummaryDto[]>([])
  const currentProperty = ref<PropertyDetailDto | null>(null)
  const suggestions     = ref<PropertySummaryDto[]>([])
  const isLoading       = ref(false)
  const error           = ref<string | null>(null)

  // ── Actions ────────────────────────────────────────────────

  async function fetchAll(): Promise<void> {
    _setLoading()
    try {
      properties.value = await propertyService.getAll()
    } catch (e) { _setError(e) }
  }

  async function search(criteria: Partial<PropertySearchCriteriaDto>): Promise<void> {
    _setLoading()
    try {
      properties.value = await searchService.search(criteria)
      isLoading.value = false
    } catch (e) { _setError(e) }
  }

  // Lance un appel par critère (utilisé quand plusieurs PropertyType
  // sont sélectionnés — voir filterStore.toApiCriteria()), fusionne
  // et déduplique les résultats sur propertyId.
  async function searchMultiple(criteriaList: Partial<PropertySearchCriteriaDto>[]): Promise<void> {
    _setLoading()
    try {
      const results = await Promise.all(
          criteriaList.map(c => searchService.search(c))
      )
      const merged = new Map<string, PropertySummaryDto>()
      results.flat().forEach(p => merged.set(p.propertyId, p))
      properties.value = Array.from(merged.values())
      isLoading.value = false
    } catch (e) { _setError(e) }
  }

  async function fetchById(id: string): Promise<void> {
    _setLoading()
    try {
      currentProperty.value = await propertyService.getById(id)
      // Charge les suggestions en parallèle, sans bloquer
      searchService.getSuggestions(id)
          .then(s => { suggestions.value = s })
          .catch(() => {})
    } catch (e) { _setError(e) }
  }

  async function create(dto: CreatePropertyDto): Promise<PropertyDetailDto> {
    _setLoading()
    try {
      const created = await propertyService.create(dto)
      return created
    } catch (e) { _setError(e); throw e }
  }

  async function update(id: string, dto: UpdatePropertyDto): Promise<void> {
    _setLoading()
    try {
      await propertyService.update(id, dto)
    } catch (e) { _setError(e); throw e }
  }

  async function remove(id: string): Promise<void> {
    _setLoading()
    try {
      await propertyService.delete(id)
      properties.value = properties.value.filter(p => p.propertyId !== id)
    } catch (e) { _setError(e); throw e }
  }

  // ── Privé ──────────────────────────────────────────────────
  function _setLoading() { isLoading.value = true; error.value = null }
  function _setError(e: unknown) {
    isLoading.value = false
    error.value = e instanceof Error ? e.message : 'Une erreur est survenue'
  }

  return {
    properties, currentProperty, suggestions, isLoading, error,
    fetchAll, search, searchMultiple, fetchById, create, update, remove,
  }
})