# Architecture, Modélisation et Sécurité : Choix Techniques et Bonnes Pratiques

Ce document rassemble l'ensemble des décisions d'architecture (Domain-Driven Design, Repository Pattern), l'étude comparative des SGBD, la stratégie de gestion des secrets, le typage sémantique des données ainsi que les règles de persistance et de versioning sous .NET.

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

---

## 3. Mécanismes Fondamentaux C# / .NET

### Le Pipeline DI (Dependency Injection)

L'Injection de Dépendances (DI) est le mécanisme par lequel .NET gère la création et la fourniture des objets dont une classe a besoin, évitant ainsi le couplage fort.

**Avec DI (La bonne pratique) :**
```csharp
// ✅ Couplage faible, totalement testable unitairement
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
* **Règle de sécurité absolue :** Cette chaîne ne doit **jamais** être écrite en dur dans le code source C#. Elle doit être placée dans la configuration (`appsettings.json` ou *User Secrets*) pour rester sécurisée.

### Les Migrations EF Core (Entity Framework Core Migrations)

Les migrations représentent l'outil d'EF Core permettant de **propager les modifications du code C# vers le schéma de la base de données SQL**. La philosophie est simple :
* **Les entités C#** sont la vérité absolue.
* **Les migrations (`add`)** sont le journal horodaté des changements (le "Git" de la base de données).
* **Le Database Update (`update`)** est le déploiement de ces changements sur le serveur réel.

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

## 4. Gestion des Secrets et Configuration par Couches

### Pourquoi ne pas utiliser BCrypt pour la Connection String ?
BCrypt est un algorithme de **hachage unidirectionnel** (il est impossible de retrouver la valeur originale depuis le hash). C'est parfait pour sécuriser les mots de passe des utilisateurs car le système compare deux hashes, sans jamais manipuler le mot de passe en clair.

Une *connection string* doit cependant être transmise telle quelle à PostgreSQL. Le driver `Npgsql` a besoin du mot de passe en clair pour s'authentifier. Hacher la chaîne de connexion la rendrait totalement inutilisable. Le problème n'est donc pas de "cacher" le mot de passe à PostgreSQL, mais de **ne pas l'exposer dans le code source versionné sur Git**.

### Le système de configuration par couches de .NET
.NET charge la configuration dans un ordre précis. Chaque couche **écrase** la précédente si elle définit la même clé :

```text
[Moins prioritaire]  1. appsettings.json                 ← Base (commité sur Git)
                     2. appsettings.Development.json     ← Surcharge dev (peut être commité)
                     3. User Secrets                     ← Surcharge locale (jamais commité)
                     4. Variables d'environnement        ← Surcharge serveur (production)
[Plus prioritaire]   5. Azure Key Vault / AWS Secrets    ← Coffre-fort cloud (entreprise)
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

## 5. Choix du Système de Gestion de Base de Données (SGBD) : Étude Comparative

| Critère | SQL Server | PostgreSQL | MySQL / MariaDB | SQLite |
| :--- | :--- | :--- | :--- | :--- |
| **Licence / Coût** | Propriétaire (très cher en Prod) | Open Source (Gratuit) | Open Source (Gratuit) | Open Source (Gratuit) |
| **Architecture** | Client/Serveur | Client/Serveur | Client/Serveur | Fichier local (Embarqué) |
| **Hébergement** | Azure natif, Windows | Universel (Linux natif, Cloud) | Universel (Linux, Cloud) | Serveur local / Mobile |
| **Provider EF Core** | Natif (Microsoft) | `Npgsql` (Excellent, très mature) | `Pomelo` (Bon) | `Microsoft.Data.Sqlite` |

### Décision Architecturale : PostgreSQL
* **Gratuité et Portabilité :** Totalement gratuit et natif sur l'environnement Linux prévu pour l'hébergement de Ymmo.
* **Maturité :** Son intégration avec Entity Framework Core via `Npgsql` est la plus mature de l'écosystème open-source.
* **Refus du NoSQL (ex: MongoDB) :** Le Modèle Conceptuel de Données (MCD) de Ymmo est **fortement relationnel**. Le relationnel garantit l'intégrité référentielle et les transactions ACID (crucial pour éviter les erreurs sur des offres financières).
* **Refus de SQLite :** Bien que simple, SQLite verrouille le fichier entier en écriture. Avec 12 agences et le siège requêtant la plateforme simultanément, il s'effondrerait sous les problèmes de concurrence.

---

## 6. Typage des Propriétés : Sémantique et Exploitation des Données

> *Règle d'or : Le type d'une propriété doit refléter sa nature sémantique, pas juste "ça compile".*

* **Montants financiers (`InitialPrice`, `CurrentPrice`, etc.)** : Remplacement du type `float` par **`decimal`**. Le type `float` introduit des erreurs d'arrondi sur les montants exacts. Le type `decimal` est conçu spécifiquement pour les calculs financiers sans perte de précision.
* **Année de construction (`YearBuilt`)** : Passage de `string` à **`int`**. Permet de réaliser des comparaisons numériques (ex: `YearBuilt > 2010`) et de valider des plages.
* **Surface de la Propriété (`Surface`)** : Passage au type **`decimal`**. Permet la réalisation de calculs mathématiques précis (ex: calcul du prix au mètre carré).

---

## 7. Choix de Persistance Structurants (Points clés pour l'oral)

Ces choix appliqués via l'API Fluent dans le `DbContext` démontrent une maîtrise technique rigoureuse :

* **`OnDelete(DeleteBehavior.Restrict)` appliqué de manière systématique :** Évite les suppressions en cascade non maîtrisées (ex: supprimer une agence ne doit pas supprimer ses agents).
* **Stratégie TPH (Table-Per-Hierarchy) pour l'héritage de `Contact` :** Une unique table `Contacts` est générée en base de données avec une colonne `ContactType` pour distinguer un `Client` d'un `Agent`. C'est extrêmement performant.
* **Index Unique Composite sur `Wishlist(ClientID, PropertyID)` :** Applique l'unicité directement en base de données, empêchant la création de doublons (un client ne peut pas ajouter deux fois le même bien en favori).

---

## 8. Nullable Reference Types en C# — Aide-mémoire

Le compilateur distingue :
* Les types qui **ne peuvent pas** être null → `string`, `Agency`
* Les types qui **peuvent** être null → `string?`, `Agency?`

### Les 4 patterns et leur lien avec le MCD

| Pattern C# | Cas d'usage métier | Correspondance MCD |
| :--- | :--- | :--- |
| `required string Name` | Valeur obligatoire (doit être fournie à la création). | Attribut obligatoire |
| `DateTime? DateSold` | Valeur optionnelle (n'existe pas encore). | Cardinalité `(0,1)` |
| `Agency Agency { get; set; } = null!;` | Navigation obligatoire gérée par l'ORM. | Cardinalité `(1,1)` |
| `ICollection<Offer> Offers { get; set; } = new List<Offer>();` | Collection (évite les NullReferenceException). | Cardinalité `(0,N)` |