// ─────────────────────────────────────────────────────────────
//  État des filtres de recherche, partagé entre SearchBar
//  et PropertiesResult (mode mock ET mode API).
// ─────────────────────────────────────────────────────────────

import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { PropertySearchCriteriaDto, PropertyType, Criteria, PhysicalCondition } from '@/types'

export interface FilterState {
    city: string
    types: string[]            // PropertyType[] — multi-select côté UI
    minPrice: number
    maxPrice: number
    minSurface: number
    maxSurface: number
    requiredCriteria: string[] // Criteria[]
    condition?: string         // PhysicalCondition
    energyClass?: string
    rooms?: number
    furnishing?: string
}

const DEFAULT_FILTERS: FilterState = {
    city: '',
    types: [],
    minPrice: 0,
    maxPrice: 1_000_000,
    minSurface: 0,
    maxSurface: 500,
    requiredCriteria: [],
    condition: '',
    energyClass: '',
    rooms: 0,
    furnishing: '',
}

export const useFilterStore = defineStore('filter', () => {
    const filters = ref<FilterState>({ ...DEFAULT_FILTERS })

    function updateFilters(newFilters: Partial<FilterState>): void {
        filters.value = { ...filters.value, ...newFilters }
    }

    function resetFilters(): void {
        filters.value = { ...DEFAULT_FILTERS }
    }

    // ── Conversion vers le format attendu par le backend ────────
    // Utilisé en mode API. Renvoie un tableau de critères : un par
    // type sélectionné (car PropertySearchCriteriaDto.type est
    // singulier), ou un seul critère sans "type" si aucun type choisi.
    function toApiCriteria(): Partial<PropertySearchCriteriaDto>[] {
        const f = filters.value

        const base: Partial<PropertySearchCriteriaDto> = {
            city             : f.city || undefined,
            minPrice         : f.minPrice > 0 ? f.minPrice : undefined,
            maxPrice         : f.maxPrice < DEFAULT_FILTERS.maxPrice ? f.maxPrice : undefined,
            minSurface       : f.minSurface > 0 ? f.minSurface : undefined,
            maxSurface       : f.maxSurface < DEFAULT_FILTERS.maxSurface ? f.maxSurface : undefined,
            condition        : (f.condition as PhysicalCondition) || undefined,
            requiredFeatures : f.requiredCriteria.length ? (f.requiredCriteria as Criteria[]) : undefined,
            pageNumber       : 1,
            pageSize         : 50,
        }

        if (f.types.length === 0) return [base]

        return f.types.map(type => ({ ...base, type: type as PropertyType }))
    }

    return { filters, updateFilters, resetFilters, toApiCriteria }
})