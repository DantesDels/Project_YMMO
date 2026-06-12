import axios from 'axios';

const api = axios.create({ baseURL: 'https://localhost:5001/api' });

export const propertyService = {
    // Envoie les critères de recherche au format PropertySearchCriteriaDto
    searchProperties(criteria) {
        return api.post('/properties/search', criteria);
    },

    // Récupère le détail selon PropertyDetailDto
    getById(id) {
        return api.get(`/properties/${id}`);
    }
};