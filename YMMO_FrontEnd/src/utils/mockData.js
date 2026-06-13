// ─────────────────────────────────────────────────────────────
//  Données de démo pour le mode "mock" (sans backend).
//  Les valeurs (type, condition, energyClass, mainFeatures)
//  correspondent aux enums réels du backend C#.
// ─────────────────────────────────────────────────────────────

const cities = ['Paris', 'Bordeaux', 'Lille', 'Lyon', 'Toulouse', 'Marseille', 'Nantes', 'Montpellier', 'Rennes', 'Grenoble'];
const propertyTypes = ['House', 'Apartment', 'Land', 'Commercial', 'Office', 'Garage', 'Parking'];
const conditions = ['New', 'Excellent', 'Good', 'NeedsRefresh', 'NeedsRenovation', 'Ruin'];
const energyClasses = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'Ex'];
const allCriteria = ['Studio', 'Balcony', 'Terrace', 'Garden', 'Garage', 'Parking', 'Cellar', 'SwimmingPool', 'Elevator', 'AirConditioning', 'FiberOptic', 'SmartHome'];

const getRandom = (arr) => arr[Math.floor(Math.random() * arr.length)];

const getRandomCriteria = () => {
    return allCriteria.filter(() => Math.random() > 0.7);
};

export const generateMockProperties = (count) => {
    return Array.from({ length: count }, (_, i) => ({
        id: crypto.randomUUID(),
        title: `${getRandom(propertyTypes)}`,
        type: getRandom(propertyTypes),
        condition: getRandom(conditions),
        energyClass: getRandom(energyClasses),
        price: Math.floor(Math.random() * 900000) + 50000,
        surface: Math.floor(Math.random() * 200) + 20,
        address: `${getRandom(cities)} (${Math.floor(Math.random() * 90) + 100})`,
        image: `https://picsum.photos/seed/${i}/400/300`,
        mainFeatures: getRandomCriteria(),
        available: true,
        availabilityDate: 'août 2026',
        rooms: Math.floor(Math.random() * 6) + 1,
        furnishing: Math.random() > 0.5 ? 'Meublé' : 'Non meublé',
    }));
};