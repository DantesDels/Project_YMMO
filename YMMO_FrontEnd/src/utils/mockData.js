export const generateMockProperties = (count) => {
    return Array.from({ length: count }, (_, i) => ({
        id: i + 1,
        title: `Logement ${i + 1} - ${['Kley', 'YNOV', 'Sergic', 'Colocation'][i % 4]}`,
        address: `${['Paris', 'Lyon', 'Bordeaux', 'Toulouse'][i % 4]} (${75000 + i})`,
        surface: 20 + (i % 30),
        rooms: 1 + (i % 3),
        furnishing: 'Meublé',
        price: 500 + (i * 50), // Prix croissant pour tester le tri
        availabilityDate: i % 2 === 0 ? 'immédiatement' : 'août 2026',
        available: i % 3 !== 0,
        isPromotion: i % 5 === 0,
        is360: i % 7 === 0,
        image: `https://picsum.photos/seed/${i}/800/600` // Images aléatoires
    }));
};