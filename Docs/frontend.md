# Frontend Vue 3 — YMMO

## Stack technique

| Technologie | Version | Usage |
|------------|---------|-------|
| Vue 3 | 3.5.34 | Framework SPA |
| Vite | 8.0 | Build, dev server, HMR |
| TypeScript | via vue-tsc | Typage statique |
| Pinia | 3.0.4 | Gestion d'état |
| Vue Router | 4.6.4 | Routing SPA |
| Axios | 1.17.0 | Requêtes HTTP |
| Chart.js | 4.5.1 | Graphiques d'analyse |
| jwt-decode | 4.0.0 | Décodage JWT client |

## Structure

```
src/
├── api/              # Axios instances + endpoints
│   ├── axios.ts      # Instance Axios avec intercepteur JWT
│   └── datascience.ts # API analyse marché + fallback
├── assets/
│   └── css/
│       └── style.css # Variables CSS, reset, utilitaires
├── components/
│   ├── analysisModule/ # 10 composants modulaires d'analyse
│   ├── charts/         # ChartWrapper.vue (6 types Chart.js)
│   ├── dashboard/      # DashboardSidebar, Profile, Wishlist, …
│   └── ui/             # AppButton, SearchBar, BudgetRangeSlider, …
├── layouts/
│   ├── PublicLayout.vue   # Header + footer
│   ├── ClientLayout.vue   # Sidebar dashboard
│   └── LegalLayout.vue    # Footer uniquement
├── router/
│   └── index.ts       # Routes, guards, 3 layouts
├── services/
│   ├── marketAnalysis.service.ts # 2M data + agrégation TypeScript
│   ├── offer.service.ts
│   └── property.service.ts
├── stores/
│   ├── authentification.store.ts # JWT, session, login/logout
│   ├── customProperties.store.ts # Propriétés agent (local)
│   ├── filterStore.ts            # Filtres recherche
│   ├── property.store.ts         # Résultats recherche
│   └── wishlist.store.ts         # Favoris localStorage
├── types/
│   └── index.ts       # 600+ lignes — correspond aux DTOs C#
├── utils/
│   ├── formatters.js  # formatPrice, formatNumber, formatDate
│   ├── imageLoader.js # Composable chargement images
│   ├── mockData.js    # Générateur propriétés mock
│   └── useClickOutside.js # Directive v-click-outside
├── views/
│   ├── agent/         # AgentDashboardView, AgentMarketAnalysisView, PropertyFormView
│   ├── footerLayout/  # Cgu, Confidentiality, Help, LegalMentions
│   └── headerLayout/  # Home, Catalog, MarketAnalysis, User, Dashboard
│   (plus HomeView, PropertyDetailView, Authentification, …)
├── App.vue            # <RouterView /> uniquement
└── main.ts            # Pinia, restoreSession, mount
```

## Routing

Layouts et lazy loading :

```typescript
const routes = [
  {
    path: '/',
    component: PublicLayout,
    children: [
      { path: '', name: 'home', component: () => import('@/views/headerLayout/HomeView.vue') },
      { path: 'catalog', name: 'catalog', component: () => import(...) },
      { path: 'market-analysis', name: 'market-analysis', component: () => import(...) },
      { path: 'property/:id', name: 'property-detail', component: () => import(...) },
      { path: 'login', name: 'login', component: () => import(...) },
      { path: 'register', name: 'register', component: () => import(...) },
      { path: 'informations', name: 'informations', component: () => import(...) },
      // Routes protégées
      { path: 'profile', meta: { requiresAuthentification: true }, component: () => import(...) },
      { path: 'dashboard', meta: { requiresAuthentification: true }, component: () => import(...) },
      { path: 'client/:section', meta: { requiresAuthentification: true }, component: () => import(...) },
      { path: 'agent/:section', meta: { requiresAuthentification: true }, component: () => import(...) },
      { path: 'portfolio/:section', meta: { requiresAuthentification: true }, component: () => import(...) },
    ]
  },
  // LegalLayout (footer links)
  { path: '/', component: LegalLayout, children: [...] }
]
```

## Stores Pinia

### authentification.store.ts

```typescript
state: {
  token: string | null        // localStorage 'token'
  user: DecodedJwt | null     // décodé depuis le JWT
  isAuthenticated: boolean
}
actions: {
  login(email, password)      // POST /api/authentification/login
  register(data)              // POST /api/authentification/register
  logout()                    // clear token + navigate /
  restoreSession()            // appelé dans main.ts au startup
}
```

### filterStore.ts

```typescript
state: {
  search: string
  propertyType: string
  city: string
  minPrice: number
  maxPrice: number
  minSurface: number
  maxSurface: number
  condition: string
}
// Utilisé par SearchBar & CatalogView
```

### wishlist.store.ts

```typescript
state: { items: WishlistItem[] }  // localStorage 'ymmo_wishlist'
actions: { toggle(propertyId), isInWishlist(id), clear() }
```

## API Layer

### axios.ts

```typescript
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '/api'
})
// Intercepteur request : ajoute Authorization: Bearer <token>
// Intercepteur response : 401 → logout
```

### datascience.ts — Fallback Pattern

```typescript
async function withFallback<T>(
  api: () => Promise<T>,
  fallback: () => T
): Promise<T> {
  try {
    return await api()       // Fetch API Python (5s timeout)
  } catch {
    return fallback()        // Données TypeScript locales
  }
}
```

## Composants Clés

### ChartWrapper

Composant générique Chart.js :

```vue
<ChartWrapper
  type="bar"          // bar | line | pie | doughnut | polarArea | radar
  :chartData="data"   // { labels, datasets }
  :options="opts"     // Chart.js options
/>
```

Gère le lifecycle : `onMounted` → `new Chart()`, `onUnmounted` → `.destroy()`, expose l'instance via `defineExpose`.

### DashboardSidebar

Navigation gauche réutilisable sur toutes les pages agent/client. Supporte :
- `navItems` / `navItemsBottom` (props)
- `tocSections` pour l'analyse marché (sommaire scroll spy)
- Mobile : overlay hamburger avec `sidebar-open` class

### AnalysisModule

Wrapper collapsible `+/-` pour chaque section d'analyse :

```vue
<AnalysisModule id="sec-trends" :expanded="isOpen" @toggle="...">
  <PriceTrendsSection :data="..." />
</AnalysisModule>
```

## Analyse Marché — Modularité

### 10 modules d'analyse

| Module | Contenu |
|--------|---------|
| MarketOverview | KPI globaux : biens, prix moyen, surface, prix/m² |
| PriceTrendsSection | Graphique lignes : évolution des prix |
| TypeDistributionSection | Camembert : répartition par type |
| ConditionsSection | Barres : état des biens |
| AvgPriceByTypeSection | Barres : prix moyen par type |
| PopularFeaturesSection | Barres horizontales : caractéristiques |
| PriceDistributionSection | Histogramme : distribution des prix |
| ZoneAnalysisSection | Tableau et barres : analyse par ville |
| PredictionsSection | Graphique lignes : prévisions IA |
| ForecastByTypeSection | Barres : tendance par type |

### Sommaire (scroll spy)

Les sections sont listées dans la sidebar sous "Sommaire". Au scroll, la section la plus proche du tiers supérieur du viewport est mise en surbrillance (via `requestAnimationFrame` throttle).

## Authentification

### JWT Flow

1. `POST /api/authentification/login` → reçoit `{ token }`
2. Token stocké dans `localStorage`
3. `restoreSession()` au démarrage : décode le JWT, peuple le store
4. Intercepteur Axios ajoute `Authorization: Bearer <token>`
5. 401 → auto-logout

### Protection routes

```typescript
router.beforeEach((to) => {
  if (to.meta.requiresAuthentification && !authStore.isAuthenticated)
    return { name: 'login', query: { redirect: to.fullPath } }
})
```

## Déploiement

```bash
# Dev
npm run dev           # Vite dev server (port 5173)

# Build production
npm run build         # dist/ → fichiers statiques

# Docker
docker compose up -d frontend
```
