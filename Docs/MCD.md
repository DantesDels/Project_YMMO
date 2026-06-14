# Modèle Conceptuel de Données (MCD) — YMMO

## 1. Entités et Attributs

### **LOCATION**
- **🔑 LocationID** *(Guid, PK)*
- `Address` *(string)*
- `City` *(string)*
- `Region` *(string)*
- `PostalCode` *(string)*
- `Complement` *(string, nullable)*
- `Country` *(string)*

---

### **AGENCY**
- **🔑 AgencyID** *(Guid, PK)*
- `Name` *(string)*
- `PhoneNumber` *(string)*
- `Email` *(string)*
- `LocationId` *(Guid, FK → Location)*

---

### **CONTACT** *(abstract, TPH — Table Per Hierarchy)*
- **🔑 ContactID** *(Guid, PK)*
- `LastName` *(string)*
- `FirstName` *(string)*
- `Email` *(string, index unique)*
- `PhoneNumber` *(string)*
- `PasswordHash` *(string)*
- `ContactRole` *(enum : Client, Agent, Manager, Admin)*
- `Discriminator: ContactType` *(string, TPH)*

#### **AGENT** *(hérite de Contact)*
- `AgencyId` *(Guid, FK → Agency)*
- `Properties` *(nav)*
- `LinkedClients` *(nav)*
- `ManagedOffers` *(nav)*

#### **CLIENT** *(hérite de Contact)*
- `CreatedAt` *(DateTime)*
- `Criteria` *(string, nullable — JSON)*
- `AgentId` *(Guid, nullable, FK → Agent)*
- `OwnedProperties` *(nav, seller)*
- `BoughtProperties` *(nav, buyer)*
- `Offers` *(nav)*
- `WishlistItems` *(nav)*

---

### **PROPERTY**
- **🔑 PropertyID** *(Guid, PK)*
- `PropertyName` *(string)*
- `Description` *(string, nullable)*
- `DateListed` *(DateTime)*
- `DateSold` *(DateTime, nullable)*
- `InitialPrice` *(decimal)*
- `CurrentPrice` *(decimal)*
- `FinalPrice` *(decimal, nullable)*
- `PropertyType` *(enum : House, Apartment, Land, Commercial, Office, Garage, Parking)*
- `Condition` *(enum : New, Excellent, Good, NeedsRefresh, NeedsRenovation, Ruin)*
- `EnergyClass` *(enum : A–G, Ex)*
- `YearBuilt` *(int)*
- `Surface` *(decimal)*
- `Features` *(string[] — PostgreSQL text[])*
- `AgencyId` *(Guid, FK → Agency)*
- `AgentId` *(Guid, nullable, FK → Agent)*
- `LocationId` *(Guid, FK → Location)*
- `SellerId` *(Guid, FK → Client)*
- `BuyerId` *(Guid, nullable, FK → Client)*

---

### **OFFER**
- **🔑 OfferID** *(Guid, PK)*
- `DateCreated` *(DateTime)*
- `DateModified` *(DateTime, nullable)*
- `DatePriceUpdated` *(DateTime, nullable)*
- `OfferPrice` *(decimal)*
- `StatusOffer` *(enum : Pending, Negotiation, Accepted, Rejected, Canceled)*
- `ClientID` *(Guid, FK → Client)*
- `AgentID` *(Guid, FK → Agent)*
- `PropertyID` *(Guid, FK → Property)*

---

### **PROPERTYPICTURE**
- **🔑 PropertyPictureId** *(Guid, PK)*
- `Url` *(string)*
- `DisplayOrder` *(int)*
- `IsMain` *(bool)*
- `PropertyId` *(Guid, FK → Property)*

---

### **WISHLISTITEM**
- **🔑 WishlistItemID** *(Guid, PK)*
- `ClientID` *(Guid, FK → Client)*
- `PropertyID` *(Guid, FK → Property)*
- **Index unique composite** : `(ClientID, PropertyID)`

---

## 2. Associations et Cardinalités

```
LOCATION (0,N) ——— Établir ——— (1,1) AGENCY
  Une agence est située à une adresse (1,1).
  Une adresse peut héberger plusieurs agences (0,N).

AGENCY (0,N) ——— Employer ——— (1,1) AGENT
  Un agent travaille dans une agence (1,1).
  Une agence emploie plusieurs agents (0,N).

AGENT (0,N) ——— Gérer ——— (0,1) CLIENT
  Un client est suivi par 0 ou 1 agent principal (0,1).
  Un agent peut suivre plusieurs clients (0,N).

AGENCY (0,N) ——— Répertorier ——— (1,1) PROPERTY
  Un bien appartient au catalogue d'une agence (1,1).
  Une agence possède plusieurs biens (0,N).

LOCATION (0,N) ——— Situer ——— (1,1) PROPERTY
  Un bien est situé à une adresse (1,1).
  Une adresse peut contenir plusieurs biens (0,N).

CLIENT vendeur (0,N) ——— Vendre ——— (1,1) PROPERTY
  Un bien est mis en vente par un client vendeur (1,1).
  Un client peut vendre plusieurs biens (0,N).

CLIENT acheteur (0,N) ——— Acquérir ——— (0,1) PROPERTY
  Un bien peut être acheté par 0 ou 1 client (0,1).
  Un client peut acheter plusieurs biens (0,N).

CLIENT (0,N) ——— Émettre ——— (1,1) OFFER
  Une offre est émise par un client (1,1).
  Un client peut émettre plusieurs offres (0,N).

AGENT (0,N) ——— Superviser ——— (1,1) OFFER
  Une offre est supervisée par un agent (1,1).
  Un agent peut superviser plusieurs offres (0,N).

PROPERTY (0,N) ——— Concerne ——— (1,1) OFFER
  Une offre concerne un bien (1,1).
  Un bien peut recevoir plusieurs offres (0,N).

PROPERTY (0,N) ——— Illustrer ——— (1,1) PROPERTYPICTURE
  Une photo illustre un bien (1,1).
  Un bien peut avoir plusieurs photos (0,N).

CLIENT (1,1) ——— Posséder ——— (0,N) WISHLISTITEM
  Un favori appartient à un client (1,1).
  Un client peut avoir plusieurs favoris (0,N).

PROPERTY (0,N) ——— Être ciblé ——— (1,1) WISHLISTITEM
  Un favori cible un bien (1,1).
  Un bien peut être dans plusieurs favoris (0,N).
```

## 3. Règles de gestion

| Entité | DeleteBehavior | Détail |
|--------|---------------|--------|
| Property → PropertyPicture | **Cascade** | Supprimer un bien supprime ses photos |
| Property → Offer | **Cascade** | Supprimer un bien supprime ses offres |
| WishlistItem → Client, Property | **Cascade** | Supprimer client/property supprime ses favoris |
| Agent → Properties | **SetNull** | L'agent supprimé : les biens restent (AgentId=NULL) |
| Client.BuyerId | **SetNull** | Client acheteur supprimé : les biens gardent BuyerId=NULL |
| Toutes les autres | **Restrict** | Pas de cascade automatique |

## 4. Contraintes techniques

- **TPH** : `Contact` → `Agent` / `Client` dans la table `Contacts` avec discriminateur `ContactType`
- **Features** : stocké en `text[]` PostgreSQL (liste de chaînes)
- **Enums** : stockés en `string` via `.HasConversion<string>()`
- **Index unique composite** : `WishlistItem(ClientID, PropertyID)` — pas de doublons
- **Index unique** : `Contact.Email`
- **ID** : `Guid.NewGuid()` généré côté application
