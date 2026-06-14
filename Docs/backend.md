# Backend .NET 10 — YMMO

## Stack technique

| Technologie | Version | Usage |
|------------|---------|-------|
| .NET | 10.0 | Runtime & SDK |
| ASP.NET Core | 10.0 | API REST, Middleware, Auth |
| Entity Framework Core | 10.0.4 | ORM PostgreSQL |
| Npgsql | 10.0.2 | Driver PostgreSQL |
| AutoMapper | 16.1.1 | Mapping Entity ↔ DTO |
| BCrypt.Net-Next | 4.2.0 | Hash mots de passe |
| JWT Bearer | 10.0.8 | Authentification |
| MailKit | 4.17.0 | Envoi d'emails |
| Swashbuckle | 10.2.1 | Swagger / OpenAPI |

## Structure du projet

```
YMMO_BackEnd/
├── API/
│   ├── Controllers/
│   │   ├── AgencyController.cs
│   │   ├── AgentController.cs
│   │   ├── AuthentificationController.cs
│   │   ├── ClientController.cs
│   │   ├── OfferController.cs
│   │   ├── PropertyController.cs
│   │   └── SearchController.cs
│   └── Middleware/
│       └── ErrorHandlingMiddleware.cs
├── Application/
│   ├── DTOs/          (Agency, Agent, Auth, Client, Location, Offer, Property, PropertyPicture, Wishlist)
│   ├── Interfaces/    (IAgencyService, IAgentService, IAuthService, IClientService, …)
│   ├── Mappings/      (Profiles AutoMapper)
│   └── Services/      (AgencyService, AgentService, AuthentificationService, …)
├── Domain/
│   ├── Entities/      (Agency, Agent, Client, Contact, Location, Offer, Property, PropertyPicture, WishlistItem)
│   ├── Enums/         (ContactRole, Criteria, EnergyClass, PhysicalCondition, PropertyType, StatusOffer)
│   ├── Filters/       (PropertySearchCriteria)
│   ├── Interfaces/    (IPasswordHasher)
│   └── Repositories/  (Interfaces: IBaseRepository, IAgencyRepository, …)
├── Infrastructure/
│   ├── Data/
│   │   ├── YmmoDbContext.cs
│   │   └── Configurations/ (Fluent API EF Core)
│   ├── Repositories/  (Implémentations: BaseRepository<T>, AgencyRepository, …)
│   ├── Security/      (UserAccessor)
│   └── Services/      (EmailService, PasswordHasher)
├── Migrations/        (EF Core — auto-appliquées au démarrage)
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── Dockerfile
└── .dockerignore
```

## Architecture Clean / DDD

```
┌──────────────────────────────────────────┐
│           API Layer (Controllers)         │
│  Points d'entrée HTTP, validation,        │
│  sérialisation JSON                       │
├──────────────────────────────────────────┤
│         Application Layer (Services)      │
│  Orchestration métier, DTOs, mapping      │
│  Dépend : Domain                          │
├──────────────────────────────────────────┤
│           Domain Layer (Core)             │
│  Entities, Enums, Repository Interfaces   │
│  Aucune dépendance externe                │
├──────────────────────────────────────────┤
│       Infrastructure Layer (Tech)         │
│  EF Core, PostgreSQL, Email, JWT, BCrypt  │
│  Dépend : Domain                          │
└──────────────────────────────────────────┘
```

## Modèle de données

### Entités principales

```
Contact (abstract, TPH)
├── Client  → OwnedProperties, BoughtProperties, Offers, WishlistItems
└── Agent   → Agency, LinkedClients, ManagedOffers, Properties

Agency      → Location, Agents, Properties
Location    → Agencies, Properties
Property    → Location, Agency, Agent, Seller(Client), Buyer(Client),
              Pictures, Offers, WishlistItems
Offer       → Property, Client, Agent
PropertyPicture → Property
WishlistItem → Client, Property
```

### Règles de suppression (DeleteBehavior)

- **Restrict** par défaut (pas de cascade)
- **Cascade** : Property → Pictures, Property → Offers, WishlistItem → Client & Property
- **SetNull** : Agent → Properties (quand l'agent est supprimé), Client.BuyerId

### Index

- **Index unique composite** sur `WishlistItem(ClientID, PropertyID)` — pas de doublons dans les favoris
- **Features** stocké en `text[]` (array PostgreSQL)
- **Enums** stockés en string via `.HasConversion<string>()`

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=YMMO_Dev;Username=postgres"
  },
  "Jwt": {
    "Issuer": "YMMO_Backend",
    "Audience": "YMMO_Frontend"
  },
  "CORS": {
    "Origins": "http://localhost:5173,http://localhost:8080"
  },
  "EmailSettings": {
    "Host": "",
    "Port": 587,
    "User": "",
    "Password": "",
    "SenderEmail": "noreply@ymmo.fr"
  }
}
```

### Développement (User Secrets)

```json
{
  "Jwt:Key": "YmmoDevJwtSecretKey2024MinLength32Chars",
  "Debug:AgentEmail": "agent@ymmo.fr",
  "Debug:AgentPassword": "agent123"
}
```

## Pipeline middleware

```
Program.cs order :
1. ErrorHandlingMiddleware (try/catch global)
2. UseRouting()
3. UseCors("AllowFrontend")
4. UseAuthentication()
5. UseAuthorization()
6. MapControllers()

Développement seulement :
  - AutoMapper config validation
  - Swagger + Swagger UI (/swagger/index.html)
```

## Démarrage

```bash
# Développement (local — nécessite PostgreSQL)
cd YMMO_BackEnd
dotnet user-secrets set "Jwt:Key" "VotreSecretIci"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=YMMO_Dev;Username=postgres;Password=xxx"
dotnet run

# Production (Docker — voir docker-deployment.md)
docker compose up -d db backend
```

## Dépendances DI (toutes Scoped)

**Repositories** : Contact, Agent, Client, Agency, Offer, Property, WishlistItem
**Services** : Agent, Client, Property, Offer, Search, Agency, Email
**Autres** : UserAccessor, AuthentificationService, PasswordHasher, YmmoDbContext, AutoMapper, IHttpContextAccessor

## Rôles et autorisations

| Rôle | Accès |
|------|-------|
| `Client` | Son profil, ses offres, sa wishlist |
| `Agent` | Ses biens, ses clients, les offres supervisées |
| `Manager` | Agence, agents, tous les biens |
| `Admin` | Tout |

## Migrations EF Core

Auto-appliquées au démarrage via `db.Database.Migrate()`. Pas besoin de commande manuelle en production.

```bash
# Créer une migration (développement)
dotnet ef migrations add MaMigration
```
