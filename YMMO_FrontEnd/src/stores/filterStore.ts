import { defineStore } from 'pinia';
import { ref } from 'vue';

interface FilterState {
    city: string;
    types: string[];
    minPrice: number;
    maxPrice: number;
    requiredCriteria: string[];
}

export const useFilterStore = defineStore('filter', () => {
    const filters = ref<FilterState>({
        city: '',
        types: [],
        minPrice: 0,
        maxPrice: 5000000,
        requiredCriteria: []
    });

    const updateFilters = (newFilters: Partial<FilterState>) => {
        filters.value = { ...filters.value, ...newFilters };
    };

    return { filters, updateFilters };
});