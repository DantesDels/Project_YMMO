const frenchCities = [
  { name: 'Paris', zipCode: '75000' },
  { name: 'Marseille', zipCode: '13000' },
  { name: 'Lyon', zipCode: '69000' },
  { name: 'Toulouse', zipCode: '31000' },
  { name: 'Nice', zipCode: '06000' },
  { name: 'Nantes', zipCode: '44000' },
  { name: 'Montpellier', zipCode: '34000' },
  { name: 'Strasbourg', zipCode: '67000' },
  { name: 'Bordeaux', zipCode: '33000' },
  { name: 'Lille', zipCode: '59000' },
  { name: 'Rennes', zipCode: '35000' },
  { name: 'Reims', zipCode: '51100' },
  { name: 'Le Havre', zipCode: '76600' },
  { name: 'Saint-Étienne', zipCode: '42000' },
  { name: 'Toulon', zipCode: '83000' },
];
const propertyTypes = ['House', 'Apartment', 'Land', 'Commercial', 'Office', 'Garage', 'Parking'];
const conditions = ['New', 'Excellent', 'Good', 'NeedsRefresh', 'NeedsRenovation', 'Ruin'];
const energyClasses = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'Ex'];
const allCriteria = ['Studio', 'Balcony', 'Terrace', 'Garden', 'Garage', 'Parking', 'Cellar', 'SwimmingPool', 'Elevator', 'AirConditioning', 'FiberOptic', 'SmartHome'];

const agentFirstNames = ['Sophie', 'Pierre', 'Julie', 'Marc', 'Antoine', 'Camille', 'Thomas', 'Léa', 'Nicolas', 'Élodie'];
const agentLastNames = ['Martin', 'Bernard', 'Dubois', 'Moreau', 'Laurent', 'Lefebvre', 'Roux', 'Girard', 'Bonnet', 'Faure'];

const descriptions = [
  'Magnifique bien situé en plein centre-ville, à proximité de tous les commerces et transports. Prestations de qualité et finitions soignées.',
  'Propriété récente offrant un cadre de vie exceptionnel. Lumineuse et spacieuse, elle séduira les amateurs de belles surfaces.',
  'À vendre charmant bien avec terrasse et jardin. Calme et verdure assurés dans ce quartier résidentiel prisé.',
  'Rare sur le marché ! Prestations haut de gamme, vue dégagée, et une localisation idéale pour les familles.',
  'Bel espace à rénover selon vos goûts. Potentiel énorme dans un quartier en pleine revitalisation.',
  'Appartement de standing avec prestations luxueuses : cuisine équipée, parquet massif, double vitrage partout.',
  'Corps de ferme entièrement rénové alliant charme de l\'ancien et confort moderne. Poutres apparentes et cheminée.',
  'Studio idéal pour investisseur ou primo-accédant. Proche université et transports en commun.',
];

const cityCoords = {
  'Paris':         { lat: 48.8566, lng: 2.3522 },
  'Marseille':     { lat: 43.2965, lng: 5.3698 },
  'Lyon':          { lat: 45.7640, lng: 4.8357 },
  'Toulouse':      { lat: 43.6047, lng: 1.4442 },
  'Nice':          { lat: 43.7102, lng: 7.2620 },
  'Nantes':        { lat: 47.2184, lng: -1.5536 },
  'Montpellier':   { lat: 43.6108, lng: 3.8767 },
  'Strasbourg':    { lat: 48.5833, lng: 7.7500 },
  'Bordeaux':      { lat: 44.8378, lng: -0.5792 },
  'Lille':         { lat: 50.6292, lng: 3.0573 },
  'Rennes':        { lat: 48.1173, lng: -1.6778 },
  'Reims':         { lat: 49.2578, lng: 4.0317 },
  'Le Havre':      { lat: 49.4938, lng: 0.1077 },
  'Saint-Étienne': { lat: 45.4397, lng: 4.3872 },
  'Toulon':        { lat: 43.1250, lng: 5.9300 },
};

const scheduleTemplate = {
  Lundi:     '09:00 - 12:00 | 14:00 - 18:00',
  Mardi:     '09:00 - 12:00 | 14:00 - 18:00',
  Mercredi:  '09:00 - 12:00 | 14:00 - 17:00',
  Jeudi:     '09:00 - 12:00 | 14:00 - 18:00',
  Vendredi:  '09:00 - 12:00 | 14:00 - 17:00',
  Samedi:    '10:00 - 12:00 | 14:00 - 16:00',
  Dimanche:  'Fermé',
};

const getRandom = (arr) => arr[Math.floor(Math.random() * arr.length)];

const getRandomCriteria = () => allCriteria.filter(() => Math.random() > 0.7);

const varyZip = (base) => {
  const prefix = base.slice(0, -2);
  const suffix = String(Math.floor(Math.random() * 100)).padStart(2, '0');
  return `${prefix}${suffix}`;
};

// ── Agents ─────────────────────────────────────────────────────
let cachedAgents = null;

export const generateMockAgents = (count = 30) => {
  if (cachedAgents) return cachedAgents;

  cachedAgents = Array.from({ length: count }, (_, i) => {
    const cityObj = frenchCities[i % frenchCities.length];
    const first = getRandom(agentFirstNames);
    const last = getRandom(agentLastNames);
    return {
      id: `agent-${i}`,
      name: `${first} ${last}`,
      email: `${first.toLowerCase()}.${last.toLowerCase()}@ymmo.fr`,
      phone: `0${6 + Math.floor(Math.random() * 4)} ${String(Math.floor(Math.random() * 100)).padStart(2, '0')} ${String(Math.floor(Math.random() * 100)).padStart(2, '0')} ${String(Math.floor(Math.random() * 100)).padStart(2, '0')} ${String(Math.floor(Math.random() * 100)).padStart(2, '0')}`,
      city: cityObj.name,
      agencyName: `Agence YMMO ${cityObj.name}`,
      schedule: { ...scheduleTemplate },
    };
  });

  return cachedAgents;
};

export const getMockAgentById = (id) => {
  const agents = generateMockAgents();
  return agents.find(a => a.id === id) || null;
};

// ── Propriétés ──────────────────────────────────────────────────
let cached = null;

export const generateMockProperties = (count = 400) => {
  if (cached) return cached;

  const agents = generateMockAgents();

  cached = Array.from({ length: count }, (_, i) => {
    const cityObj = getRandom(frenchCities);
    const city = cityObj.name;
    const coords = cityCoords[city];
    const lat = coords.lat + (Math.random() - 0.5) * 0.08;
    const lng = coords.lng + (Math.random() - 0.5) * 0.08;
    const seed = i;
    const picCount = Math.floor(Math.random() * 4) + 3;
    const agent = agents[Math.floor(Math.random() * agents.length)];

    return {
      id: `prop-${i}`,
      title: `${getRandom(propertyTypes)}`,
      type: getRandom(propertyTypes),
      condition: getRandom(conditions),
      energyClass: getRandom(energyClasses),
      price: Math.floor(Math.random() * 900000) + 50000,
      surface: Math.floor(Math.random() * 200) + 20,
      address: `${city} (${varyZip(cityObj.zipCode)})`,
      image: `https://picsum.photos/seed/${seed}/400/300`,
      mainFeatures: getRandomCriteria(),
      available: true,
      availabilityDate: 'août 2026',
      rooms: Math.floor(Math.random() * 6) + 1,
      furnishing: Math.random() > 0.5 ? 'Meublé' : 'Non meublé',
      description: getRandom(descriptions),
      yearBuilt: Math.floor(Math.random() * 80) + 1945,
      latitude: lat,
      longitude: lng,
      agencyName: agent.agencyName,
      agentName: agent.name,
      agentId: agent.id,
      pictures: Array.from({ length: picCount }, (_, j) => ({
        url: `https://picsum.photos/seed/${seed}-${j}/800/600`,
      })),
    };
  });

  return cached;
};

export const getMockPropertyById = (id) => {
  const all = generateMockProperties();
  return all.find(p => p.id === id) || null;
};
