# Architecture, Modélisation et Sécurité : Choix Techniques et Bonnes Pratiques

Ce document rassemble l'ensemble des décisions d'architecture (Domain-Driven Design, Repository Pattern), l'étude comparative des SGBD, la stratégie de gestion des secrets, le typage sémantique des données ainsi que les règles de persistance, de navigation et de versioning sous .NET.

---

## 1. Architecture Globale : Domain-Driven Design (DDD)

Le projet adopte l'approche DDD, structurant l'application en couches distinctes pour isoler la logique métier des détails techniques.

* **API** : Points d'entrée de l'application (Controllers).
* **Application** : Orchestration des cas d'utilisation (Use Cases).
* **Domain** : Cœur du métier (Entités, Énumérations, Contrats métier). **Indépendant de toute technologie**.
* **Infrastructure** : Implémentations techniques (Accès base de données via EF Core, appels API externes).

---

## 2. Le Repository Pattern : Abstraction de l'Accès aux Données

Pour isoler le domaine des détails de la base de données (comme Entity Framework Core), nous utilisons le **Repository Pattern**.

### Le Principe
Une abstraction est un contrat définissant *quoi* faire, sans dicter *comment* le faire. En C#, une interface est le mécanisme du langage pour exprimer ce contrat.

* **Le contrat (l'interface C#)** appartient au domaine. Il dicte les besoins du métier.
* **L'implémentation (la classe C#)** appartient à l'infrastructure. Elle gère la communication réelle avec la base de données.

> Le Domain ne sait pas qu'Entity Framework Core existe. Il sait juste qu'il peut demander une action (ex: `GetByIdAsync`).

### Organisation des Dossiers : Privilégier le rôle technique

**Pourquoi un dossier `Repositories` et non `Interfaces` ?**
* Le nom d'un dossier doit répondre à la question : "qu'est-ce qu'il y a dedans, conceptuellement ?" et non "quel mécanisme technique utilise-t-on ?".
* `Interfaces/` décrit un outil. `Repositories/` décrit un rôle architectural (l'accès aux données).
* Demain, le `Domain` pourrait contenir des `Services` ou des `Events`. Si tout était groupé dans un dossier générique `Interfaces/`, l'organisation perdrait son sens architectural.

**Structure attendue :**

```text
📁 Domain
└── 📁 Repositories
    └── 📄 IAgencyRepository.cs  (Le contrat - Le "Quoi")

📁 Infrastructure
└── 📁 Repositories
    └── 📄 AgencyRepository.cs   (L'implémentation EF Core - Le "Comment")
```

#### Exemple de Contrat (Domain)
```csharp
// Domain/Repositories/IAgencyRepository.cs
// -> The "what" : defines what the domain needs to do with Agency
public interface IAgencyRepository
{
    Task<Agency?> GetByIdAsync(Guid id);
    Task<IEnumerable<Agency>> GetAllAsync();
    Task AddAsync(Agency agency);
    Task UpdateAsync(Agency agency);
    Task DeleteAsync(Guid id);
}
```

#### Exemple d'Implémentation (Infrastructure)
```csharp
// Infrastructure/Repositories/AgencyRepository.cs
// -> The "how" : EF Core, SQL, DbContext — technical details
public class AgencyRepository : IAgencyRepository
{
    // Concrete implementation using EF Core DbContext goes here
}
```

---

## 3. Mécanismes Fondamentaux C# / .NET

### Le Pipeline DI (Dependency Injection)

L'Injection de Dépendances (DI) est le mécanisme par lequel .NET gère la création et la fourniture des objets dont une classe a besoin, évitant ainsi le couplage fort.

**Sans DI (À éviter) :**
```csharp
// ❌ Strong coupling, impossible to unit test
public class AgencyService
{
    // La classe crée sa propre dépendance directement
    private AgencyRepository _repo = new AgencyRepository(); 
}
```

**Avec DI (La bonne pratique) :**
```csharp
// ✅ Loose coupling, fully testable
public class AgencyService
{
    private IAgencyRepository _repo;
    
    // La dépendance est injectée via le constructeur
    public AgencyService(IAgencyRepository repo) 
    { 
        _repo = repo; 
    } 
}
```
L'enregistrement de ces services auprès du conteneur DI de .NET s'effectue au démarrage de l'application, dans le fichier `Program.cs`.

### La Connection String (Chaîne de connexion)

La *Connection String* est la chaîne de caractères qui indique à l'ORM (Entity Framework Core) où se trouve la base de données et comment s'y connecter.

**Exemple typique :**
```text
Server=localhost;Database=YmmoDB;Trusted_Connection=True;
```
* `Server` : L'adresse du serveur (ici, la machine locale).
* `Database` : Le nom de la base de données.
* `Trusted_Connection=True` : Utilisation de l'authentification Windows (pas de mot de passe en clair).

**Règle de sécurité absolue :** Cette chaîne ne doit **jamais** être écrite en dur dans le code source C#. Elle doit être placée dans le fichier de configuration `appsettings.json` afin de rester sécurisée et modulable selon l'environnement (développement, test, production).

### Les Migrations EF Core (Entity Framework Core Migrations)

Les migrations représentent l'outil d'EF Core permettant de **propager les modifications de ton code C# (modèle Code-First) vers le schéma de la base de données SQL**, de manière incrémentale et sécurisée.

**À quoi ça sert concrètement ?**
1. **Évolution Synchrone** : Lorsque tu ajoutes une propriété, une entité, ou que tu modifies un type en C#, les migrations génèrent le script nécessaire pour mettre à jour la base de données sans perdre les données existantes.
2. **Historisation et Git** : Chaque modification est traduite dans un fichier C# horodaté (une migration). Ces fichiers sont poussés sur GitHub. Ton équipe peut ainsi récupérer le projet et exécuter la mise à jour pour avoir exactement la même structure de base de données que toi.
3. **Automatisation (CI/CD)** : En production ou sur un serveur de test, les migrations permettent d'automatiser le déploiement de la base de données sans intervention manuelle ni scripts SQL exécutés à la volée.

**Les commandes indispensables (CLI .NET) :**
```bash
# 1. Create a new migration snapshot after code changes
dotnet ef migrations add AddPropertyTable

# 2. Apply all pending migrations to the target database
dotnet ef database update
```

#### Le Workflow Quotidien (Évolution incrémentale)
En développement continu ou en production, on ne supprime jamais la base de données, on la fait évoluer.
1. **Modifier le code C#** (ex: ajout d'une propriété).
2. **Créer une nouvelle migration (le plan)** avec un nom clair :
   ```bash
   dotnet ef migrations add AddPropertyDescription
   ```
3. **Appliquer la migration (le déploiement)** :
   ```bash
   dotnet ef database update
   ```

#### Le "Hard Reset" (Phase de maquettage uniquement)
En tout début de projet, si la base est vide et qu'une erreur de conception a été introduite dans la première migration, il est préférable de réinitialiser complètement l'historique plutôt que d'empiler des correctifs.
**⚠️ Attention : Séquence strictement interdite en production ou en équipe.**

```bash
# 1. Détruit physiquement la base de données actuelle pour effacer le schéma erroné
dotnet ef database drop --force

# 2. Supprime les fichiers C# de migration du dossier /Migrations
dotnet ef migrations remove

# 3. Génère un nouveau plan de construction propre basé sur le code actuel
dotnet ef migrations add InitialCreate

# 4. Reconstruit la base de données de zéro avec le nouveau plan
dotnet ef database update
```

---

## 4. Entity Framework Core : Clé Étrangère vs Propriété de Navigation

Comprendre la différence entre un ID brut et l'objet complet est une étape fondamentale pour bien structurer une architecture logicielle avec Entity Framework Core.

### La Clé Étrangère : `public Guid? AgentID { get; set; }`
C'est le lien de ton architecture **côté Base de Données (SQL)**.
* **Ce que c'est en réalité :** Une véritable colonne physique nommée `AgentID` dans ta table SQL `Contacts`.
* **Ce qu'elle contient :** Uniquement une donnée brute (un identifiant unique de type `Guid`).
* **Son avantage principal :** Elle est extrêmement légère à manipuler en mémoire.

> **L'analogie :** C'est comme avoir le numéro de téléphone de ton agent noté sur un bout de papier. C'est facile à lire, ça ne prend pas de place, et si tu changes d'agent, tu as juste à remplacer l'identifiant pour en associer un nouveau.

### La Propriété de Navigation : `public Agent? LinkedAgent { get; set; }`
C'est le lien de ton architecture **côté Code (C# Orienté Objet)**.
* **Ce que c'est en réalité :** Un concept purement C#. Cette propriété n'existe pas en tant que colonne dans ta base de données SQL.
* **Ce qu'elle contient :** L'objet `Agent` dans son intégralité (avec toutes ses propriétés : nom, prénom, email, rôle, etc.).
* **Son avantage principal :** Elle te permet de "naviguer" fluidement dans tes données sans avoir à écrire des requêtes de jointure SQL complexes.

> **L'analogie :** C'est avoir ton agent physiquement avec toi dans la pièce. Tu peux interagir avec lui et lui poser des questions directement. Mais pour qu'il vienne dans la pièce, Entity Framework a d'abord besoin de l'appeler en utilisant sa clé étrangère (`AgentID`) pour exécuter un `JOIN` en arrière-plan.

### Pourquoi est-il recommandé de garder les deux ?
Dans EF Core, avoir à la fois la clé étrangère et la propriété de navigation s'appelle configurer une **relation pleinement définie**. C'est une excellente pratique pour des raisons de performance et d'optimisation lors des écritures.

**Cas pratique : Tu souhaites modifier l'agent assigné à un client.**

#### ✅ La méthode optimisée (Grâce à l'ID)
Puisque tu disposes de la propriété `AgentID`, l'opération est instantanée. Tu n'as pas besoin d'interroger la base de données au préalable.
```csharp
// Fast: directly updates the column value in the database
client.AgentID = newGuidAgent; 
```

#### ❌ La méthode lourde (Si tu n'avais que la propriété de navigation)
Si tu avais choisi de retirer `AgentID` de ta classe pour ne garder que la propriété de navigation, Entity Framework t'obligerait à charger l'objet entier depuis la base de données juste pour pouvoir mettre à jour le lien en C#.
```csharp
// Heavy: requires a full SELECT query to fetch the agent entity
var agent = dbContext.Agents.Find(newGuidAgent); 

// ... only to assign it to the client entity in memory
client.LinkedAgent = agent; 
```
Garder les deux propriétés offre la légèreté et la rapidité du SQL pour les mises à jour simples, tout en conservant la puissance de l'orienté objet pour la lecture et l'exploration des données.

---

## 5. Gestion des Secrets et Configuration par Couches

### Pourquoi ne pas utiliser BCrypt pour la Connection String ?
BCrypt est un algorithme de **hachage unidirectionnel** (il est impossible de retrouver la valeur originale depuis le hash). C'est parfait pour sécuriser les mots de passe des utilisateurs car le système compare deux hashes, sans jamais manipuler le mot de passe en clair.

Une *connection string* doit cependant être transmise telle quelle à PostgreSQL. Le driver `Npgsql` a besoin du mot de passe en clair pour s'authentifier auprès du SGBD. Hacher la chaîne de connexion la rendrait totalement inutilisable. Le problème n'est donc pas de "cacher" le mot de passe à PostgreSQL, mais de **ne pas l'exposer dans le code source versionné sur Git**.

### Le système de configuration par couches de .NET
.NET charge la configuration dans un ordre précis. Chaque couche **écrase** la précédente si elle définit la même clé :

```text
[Moins prioritaire]  1. appsettings.json                 ← Base (commité sur Git)
                     2. appsettings.Development.json     ← Surcharge dev (peut être commité)
                     3. User Secrets                     ← Surcharge locale (jamais commité)
                     4. Variables d'environnement        ← Surcharge serveur (production)
[Plus prioritaire]   5. Azure Key Vault / AWS Secrets    ← Coffre-fort cloud (entreprise)
```

#### Couche 1 — `appsettings.json`
Fichier de base, commité sur Git. Contient uniquement des configurations non sensibles et laisse la chaîne de connexion vide.
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### Couche 2 — `appsettings.Development.json`
Surcharge spécifique à l'environnement de développement (URLs locales, logs verbeux). Ne doit pas contenir de mots de passe de production ni de credentials locaux partagés.
```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

#### Couche 3 — User Secrets (Développement Local)
Mécanisme natif .NET stockant les secrets **en dehors du répertoire du projet** (dans le profil utilisateur de la machine locale), les mettant à l'abri de tout commit accidentel.

* **Initialisation** (à lancer dans le dossier du projet contenant le `.csproj`) :
  ```powershell
  dotnet user-secrets init
  ```
  Cette commande injecte un identifiant unique `<UserSecretsId>` (GUID) dans ton fichier `.csproj`.
* **Ajout d'un secret local (ex: la chaîne de connexion PostgreSQL de dev)** :
  ```powershell
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=YmmoDB;Username=postgres;Password=monmdp"
  ```
* **Gestion en ligne de commande** :
  ```powershell
  dotnet user-secrets list
  dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
  ```
> .NET charge les User Secrets automatiquement **uniquement en environnement Development**. `WebApplication.CreateBuilder(args)` s'en occupe nativement sans modifier `Program.cs`.

#### Couche 4 — Variables d'Environnement (Production)
En production (sur un serveur Linux ou dans un conteneur), les secrets sont injectés directement en mémoire via des variables d'environnement.

* **Configuration sur un serveur Linux** :
  ```bash
  export ConnectionStrings__DefaultConnection="Host=prod-server;Port=5432;Database=YmmoDB;Username=ymmo_user;Password=mdp_complexe"
  ```
  *Note : .NET utilise le double underscore `__` comme séparateur hiérarchique pour remplacer le `:` des fichiers JSON.*
* **Configuration dans Docker Compose (`docker-compose.yml`)** :
  ```yaml
  services:
    backend:
      environment:
        - ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=YmmoDB;Username=ymmo_user;Password=mdp_complexe
  ```

#### Couche 5 — Coffre-fort Cloud (Enterprise)
Pour une sécurité maximale en production cloud, les secrets sont centralisés dans des gestionnaires dédiés externes avec chiffrement au repos et rotation automatique.

```csharp
// Requires package: Azure.Extensions.AspNetCore.Configuration.Secrets
// Enrolling Azure Key Vault into the configuration pipeline inside Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri("[https://ymmo-vault.vault.azure.net/](https://ymmo-vault.vault.azure.net/)"),
    new DefaultAzureCredential());
```

### Tableau de Décision Rapide (Secrets)

| Contexte | Solution | Commité sur Git ? |
| :--- | :--- | :--- |
| **Dev local (1 dev / Équipe)** | User Secrets (chacun le sien via `dotnet user-secrets`) | ❌ Non |
| **CI/CD (GitHub Actions)** | Secrets GitHub | ❌ Non |
| **Production serveur** | Variables d'environnement | ❌ Non |
| **Production cloud** | Key Vault / Secrets Manager | ❌ Non |
| **Valeurs non sensibles** | `appsettings.json` | ✅ Oui |

---

## 6. Choix du Système de Gestion de Base de Données (SGBD) : Étude Comparative

| Critère | SQL Server | PostgreSQL | MySQL / MariaDB | SQLite |
| :--- | :--- | :--- | :--- | :--- |
| **Licence / Coût** | Propriétaire (très cher en Prod) | Open Source (Gratuit) | Open Source (Gratuit) | Open Source (Gratuit) |
| **Architecture** | Client/Serveur | Client/Serveur | Client/Serveur | Fichier local (Embarqué) |
| **Hébergement** | Azure natif, Windows | Universel (Linux natif, Cloud) | Universel (Linux, Cloud) | Serveur local / Mobile |
| **Gestion Concurrence** | Excellente | Excellente (MVCC) | Bonne | Très faible (Verrouillage fichier) |
| **Provider EF Core** | Natif (Microsoft) | `Npgsql` (Excellent, très mature) | `Pomelo` (Bon) | `Microsoft.Data.Sqlite` |
| **Cas d'usage idéal** | Écosystème 100% Microsoft | Applications d'entreprise complexes | Web classique, CMS | POC, Tests unitaires, Mobile |

### Analyse détaillée des alternatives pour Ymmo

#### 1. PostgreSQL (Le Choix Retenu & Recommandé)
* **Pourquoi :** Totalement gratuit, natif sur l'environnement Linux prévu pour l'hébergement, ultra-robuste.
* **Le gros point fort :** Son intégration avec Entity Framework Core via `Npgsql` est la plus mature de l'écosystème open-source. De plus, Postgres gère nativement le format JSON, ce qui sera un atout majeur pour la phase "Big Data / IA" du projet Ymmo (pour stocker des données non structurées de scraping par exemple).

#### 2. SQL Server (L'Alternative Microsoft)
* **Pourquoi l'écarter :** Bien que l'intégration avec .NET soit parfaite, le coût de licence en production est prohibitif pour une architecture standard. De plus, bien qu'il puisse tourner sous Linux via Docker, il reste un système fondamentalement pensé pour l'écosystème Windows.

#### 3. SQLite (Le Piège)
* **Pourquoi l'écarter :** SQLite n'est pas un vrai serveur de base de données, c'est un simple fichier physique. Lors d'une écriture (ex: création d'une offre immobilière), la base entière est verrouillée. Avec 12 agences et le siège requêtant la plateforme simultanément, SQLite s'effondrerait sous les problèmes de concurrence (locks).
* **Utilité :** Il ne sera utilisé que pour l'environnement de développement ou l'exécution rapide des tests unitaires/intégration.

#### 4. MySQL / MariaDB (Le Concurrent Direct)
* **Pourquoi l'écarter :** C'est une excellente alternative. Cependant, PostgreSQL est historiquement plus strict sur l'intégrité des données et respecte mieux les standards SQL complexes. Dans le monde .NET / Entreprise, lorsqu'on quitte SQL Server, PostgreSQL est le choix "de facto".

#### Note architecturale : Pourquoi pas le NoSQL (ex: MongoDB) ?
Le Modèle Conceptuel de Données (MCD) de Ymmo est **fortement relationnel** (Un Client émet une Offre qui concerne une Propriété gérée par une Agence). Le relationnel garantit l'intégrité référentielle et les transactions ACID (crucial pour éviter les erreurs sur des offres financières immobilières). Le NoSQL (orienté document) n'est pas adapté au cœur de métier de Ymmo.

---

## 7. Typage des Propriétés : Sémantique et Exploitation des Données

> *Règle d'or : Le type d'une propriété doit refléter sa nature sémantique, pas juste "ça compile".*

Des ajustements de types ont été réalisés sur les entités pour garantir la précision, particulièrement dans une optique de traitement analytique futur (Big Data) :

* **Montants financiers (`InitialPrice`, `CurrentPrice`, etc.)** : Remplacement du type `float` par **`decimal`**. Le type `float` est une virgule flottante binaire, ce qui introduit des erreurs d'arrondi sur les montants exacts. Le type `decimal` est conçu spécifiquement pour les calculs financiers sans perte de précision.
* **Année de construction de la Propriété (`YearBuilt`)** : Passage de `string` à **`int`**. Permet de réaliser des comparaisons numériques directes (ex: `YearBuilt > 2010`), de trier chronologiquement et de valider des plages d'années.
* **Surface de la Propriété (`Surface`)** : Passage au type **`decimal`**. Permet la réalisation de calculs mathématiques précis (ex: calcul du prix au mètre carré, agrégations statistiques).

---

## 8. Choix de Persistance Structurants (Points clés pour l'oral)

Ces trois choix de configuration de base de données (à appliquer via l'API Fluent dans le `DbContext`) traduisent des contraintes métiers fortes et démontrent une maîtrise technique rigoureuse :

* **`OnDelete(DeleteBehavior.Restrict)` appliqué de manière systématique**
   * *Raison :* Éviter les suppressions en cascade non maîtrisées. Si une entité parente est supprimée (par exemple, une `Agency`), le système ne doit pas supprimer silencieusement toutes les entités dépendantes associées (comme ses `Agent`). C'est une règle de sécurité et d'intégrité métier fondamentale.

* **Stratégie TPH (Table-Per-Hierarchy) pour l'héritage de `Contact`**
   * *Raison :* TPH est le comportement par défaut d'Entity Framework Core. Une unique table `Contacts` est générée en base de données, utilisant une colonne de discrimination automatique (`ContactType` ou `Discriminator`) pour distinguer un `Client` d'un `Agent`. Cette approche est extrêmement performante et simple.

* **Index Unique Composite sur `Wishlist(ClientID, PropertyID)`**
   * *Raison :* Contrainte d'unicité métier stricte : un client ne peut pas ajouter le même bien immobilier plusieurs fois dans sa liste de favoris. L'application de cette règle directement au niveau de la base de données via un index unique garantit l'intégrité absolue des données, empêchant les doublons même en cas de requêtes concurrentes.

---

## 9. Nullable Reference Types en C# — Aide-mémoire

### Contexte
Depuis C# 8, il est possible d'activer l'analyse statique de nullabilité via `<Nullable>enable</Nullable>` dans le `.csproj`.
Le compilateur distingue deux mondes :
* Les types qui **ne peuvent pas** être null → `string`, `Agency`, `int`
* Les types qui **peuvent** être null → `string?`, `Agency?`, `int?`

### Les 4 patterns et quand les utiliser

#### 1. `required string` — valeur obligatoire connue à la création
La propriété est obligatoire et sa valeur est fournie par l'appelant dès l'instanciation. Le compilateur exige qu'elle soit renseignée via un initialiseur d'objet.
```csharp
// Domain entity example - Name is strictly mandatory at creation
public required string Name { get; set; }
```

#### 2. `Type?` — valeur optionnelle, peut légitimement être null
La propriété peut ne pas exister selon l'état métier de l'entité. Le `?` est un contrat explicite : "cette valeur peut être absente, gère ce cas."
```csharp
// Unsold properties will not have these fields populated initially
public DateTime? DateSold { get; set; }
public decimal? FinalPrice { get; set; }
public Client? Buyer { get; set; }
```

#### 3. `= null!` — obligatoire mais géré par EF Core
La propriété de navigation est obligatoire côté métier (cardinalité `1,1` dans le MCD), mais EF Core l'instancie via réflexion, pas via un constructeur. On indique au compilateur "fais-moi confiance".
```csharp
// An agent always belongs to an agency in the business model
public Agency Agency { get; set; } = null!;
```

#### 4. `= new List<T>()` — collection toujours initialisée
Pour toutes les propriétés `ICollection<T>`. Une collection vide est toujours préférable à null — cela évite de vérifier la nullité avant chaque itération `foreach`.
```csharp
// Avoids NullReferenceException when iterating over a new or empty collection
public ICollection<Agent> Agents { get; set; } = new List<Agent>();
```

### Lien avec le MCD

La cardinalité du MCD dicte directement le pattern C# :
* **MCD (1,1)** → navigation obligatoire → `= null!`
* **MCD (0,1)** → navigation optionnelle → `Type?`
* **MCD (0,N)** → collection → `= new List<T>()`

---

## 10. Séparation des Responsabilités : Entités vs DTOs (Data Transfer Objects)

Une règle fondamentale de l'architecture propre (DDD) est de ne jamais mélanger la réalité physique de la base de données avec les requêtes et interactions des utilisateurs.

### L'Entité (`Property`) = La Réalité Physique
La classe d'entité représente le bien immobilier tel qu'il existe dans la vraie vie et dans la base de données. Elle décrit ses attributs intrinsèques.

* **Exemple :** `public List<Criteria> Features { get; set; }` décrit simplement la liste des équipements réels de cette maison (ex: un balcon et un garage). L'entité se fiche totalement de savoir si quelqu'un est en train de la rechercher.

### L'Objet de Recherche (`PropertySearchCriteria`) = La Demande du Client
Cette classe n'est pas une entité et n'existe pas en base de données. C'est un **DTO (Data Transfer Object)** utilisé pour transporter les filtres sélectionnés par l'utilisateur depuis l'interface web (ex: "prix maximum de 300 000 € ET présence d'un garage").

> **L'Analogie de l'Agence :**
> * **L'Entité `Property`**, c'est la carte d'identité de la maison accrochée dans la vitrine de l'agence.
> * **Le DTO `PropertySearchCriteria`**, c'est le bout de papier avec lequel le client rentre dans l'agence en disant : "Bonjour, je cherche ça". La maison en vitrine n'a pas besoin de connaître l'existence de ce bout de papier.

### Le Rôle du Repository (Le Pont)
L'entité et le DTO ne doivent pas être liés directement dans le code du domaine. C'est le **Repository** (`PropertyRepository`) qui fait le lien. Il agit comme l'agent immobilier : il prend les critères du client (`PropertySearchCriteria`), regarde toutes les fiches en vitrine (`Property`), et construit une requête SQL dynamique pour ne renvoyer que les biens qui correspondent à la demande.

---

## 11. Recherche par Numéro de Téléphone : Enjeux Métiers et Contraintes Techniques

L'ajout d'une méthode de recherche par numéro de téléphone dans le portefeuille client (`IClientRepository`) répond à des besoins opérationnels forts tout en introduisant une contrainte de normalisation technique au niveau de la couche Infrastructure.

### A. Les Enjeux Métiers

#### 1. Le Cas d'Usage "CRM" (Pour l'Agent Immobilier)
Le téléphone reste l'outil de communication principal d'un agent sur le terrain. En cas d'appel manqué ou de note manuscrite, l'agent doit pouvoir retrouver instantanément la fiche complète d'un client (incluant ses offres en cours et sa liste de favoris) sur son tableau de bord back-office en saisissant simplement le numéro de téléphone.

#### 2. La Prévention des Doublons (Intégrité des Données)
Pour éviter qu'un agent ne recrée manuellement une fiche pour un client qui se serait déjà inscrit de lui-même sur la plateforme web, l'application utilise `GetByPhoneNumberAsync` (en complément de la vérification par email) comme barrière de contrôle stricte avant toute nouvelle persistance en base de données.

#### 3. Évolution vers l'Authentification Forte (Futur)
Cette méthode pose les fondations architecturales nécessaires pour l'intégration future de mécanismes d'authentification modernes, tels que la connexion par code OTP (One-Time Password) envoyé par SMS ou l'activation de la double authentification (2FA).

### B. La Contrainte Technique : Le "Piège" du Formatage (Infrastructure)

La recherche par numéro de téléphone introduit un défi classique de cohérence des données. Un utilisateur ou un agent peut saisir un numéro sous de multiples formats :
* `0612345678` (Format local compact)
* `06 12 34 56 78` (Format local avec espaces)
* `+33612345678` (Format international standard)

#### Règle d'implémentation :
Pour garantir l'efficacité de la clause `WHERE` générée par Entity Framework Core lors de l'appel à `GetByPhoneNumberAsync`, le système doit appliquer un **nettoyage et une normalisation stricte au format international (ex: E.164)** avant l'écriture en base de données. Les espaces, points ou tirets doivent être purgés pour stocker une chaîne brute standardisée (ex: `+33612345678`).

