# YMMO — Backend .NET 10

API RESTful pour la plateforme immobilière YMMO : gestion des biens, clients, agents, offres, authentification et recherche.

## Stack

| Technologie | Version |
|------------|---------|
| .NET / ASP.NET Core | 10.0 |
| Entity Framework Core | 10.0.4 |
| PostgreSQL (Npgsql) | 16 / 10.0.2 |
| AutoMapper | 16.1.1 |
| BCrypt.Net | 4.2.0 |
| JWT Bearer | 10.0.8 |
| MailKit | 4.17.0 |
| Swagger (Swashbuckle) | 10.2.1 |

## Architecture

Clean Architecture en 4 couches :

```
API (Controllers, Middleware)
  └── Application (Services, DTOs, Mappings)
        └── Domain (Entities, Enums, Repository Interfaces)
              └── Infrastructure (EF Core, Repositories, Email, Auth)
```

## Démarrage

```bash
# Développement local (nécessite PostgreSQL)
cd YMMO_BackEnd
dotnet user-secrets set "Jwt:Key" "VotreSecret32CaractèresMin"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=YMMO_Dev;Username=postgres;Password=xxx"
dotnet run
# → http://localhost:5000
# → http://localhost:5000/swagger

# Docker
docker compose up -d db backend
# → http://localhost:5000
```

## Structure

```
API/Controllers/          # 7 controllers
API/Middleware/            # ErrorHandlingMiddleware
Application/DTOs/          # Agency, Agent, Auth, Client, Location, Offer, Property, PropertyPicture, Wishlist
Application/Mappings/      # Profiles AutoMapper
Application/Services/      # 7 services métier
Application/Interfaces/    # Contrats des services
Domain/Entities/           # Agency, Agent, Client, Contact, Location, Offer, Property, PropertyPicture, WishlistItem
Domain/Enums/              # ContactRole, Criteria, EnergyClass, PhysicalCondition, PropertyType, StatusOffer
Domain/Repositories/       # Interfaces des repositories
Infrastructure/Data/       # YmmoDbContext + configurations EF Core (Fluent API)
Infrastructure/Repositories/ # Implémentations
Infrastructure/Security/   # UserAccessor (JWT claims)
Infrastructure/Services/   # EmailService, PasswordHasher
```

## Documentation

Voir [`Docs/backend.md`](../Docs/backend.md) pour la documentation détaillée.
Voir [`Docs/api-reference.md`](../Docs/api-reference.md) pour tous les endpoints.
Voir [`Docs/MCD.md`](../Docs/MCD.md) pour le modèle de données.
