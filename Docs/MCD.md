# Modèle Conceptuel de Données (MCD) - Projet Ymmo

## 1. Entités et Attributs

* **LOCATION**
    * **🔑 LocationID** *(Identifiant / Clé Primaire)*
    * `Address`, `City`, `Region`, `PostalCode`, `Complement`, `Country`

* **AGENCY**
    * **🔑 AgencyID** *(Identifiant / Clé Primaire)*
    * `Name`, `PhoneNumber`, `Email`

* **AGENT** *(Hérite de Contact)*
    * **🔑 AgentID** *(Identifiant / Clé Primaire)*
    * `LastName`, `FirstName`, `Email`, `PhoneNumber`, `ContactRole`

* **CLIENT** *(Hérite de Contact)*
    * **🔑 ClientID** *(Identifiant / Clé Primaire)*
    * `LastName`, `FirstName`, `Email`, `PhoneNumber`, `ContactRole`, `Criteria`

* **PROPERTY**
    * **🔑 PropertyID** *(Identifiant / Clé Primaire)*
    * `DateListed`, `DateSold`, `InitialPrice`, `CurrentPrice`, `FinalPrice`, `State`, `PropertyType`, `EnergyClass`, `YearBuilt`, `Surface`

* **OFFER**
    * **🔑 OfferID** *(Identifiant / Clé Primaire)*
    * `DateCreated`, `DateModified`, `DatePriceUpdated`, `OfferPrice`, `Status`

* **WISHLIST**
    * **🔑 WishlistID** *(Identifiant / Clé Primaire)*

---

## 2. Associations et Cardinalités

* **LOCATION** `(0,N)` <--- *Établir* ---> `(1,1)` **AGENCY**
    * Une agence (**AGENCY**) est située dans une et une seule adresse (`1,1`).
    * Une adresse (**LOCATION**) peut abriter zéro ou plusieurs agences (`0,N`).

* **AGENCY** `(0,N)` <--- *Travailler* ---> `(1,1)` **AGENT**
    * Un agent travaille dans une et une seule agence (`1,1`).
    * Une agence emploie zéro ou plusieurs agents (`0,N`).

* **AGENT** `(0,N)` <--- *Gérer* ---> `(0,1)` **CLIENT**
    * Un client est accompagné par zéro ou un agent principal (`0,1`).
    * Un agent peut accompagner zéro ou plusieurs clients (`0,N`).

* **AGENCY** `(0,N)` <--- *Répertorier* ---> `(1,1)` **PROPERTY**
    * Un bien immobilier appartient au catalogue d'une seule agence (`1,1`).
    * Une agence possède zéro ou plusieurs biens dans son catalogue (`0,N`).

* **LOCATION** `(0,N)` <--- *Situer* ---> `(1,1)` **PROPERTY**
    * Un bien immobilier se situe à une seule adresse (`1,1`).
    * Une adresse peut contenir zéro ou plusieurs biens (`0,N`).

* **CLIENT** `(0,N)` <--- *Vendre* ---> `(1,1)` **PROPERTY** *(Relation Vendeur)*
    * Un bien immobilier est mis en vente par un seul client vendeur (`1,1`).
    * Un client peut mettre en vente zéro ou plusieurs biens (`0,N`).

* **CLIENT** `(0,N)` <--- *Acquérir* ---> `(0,1)` **PROPERTY** *(Relation Acheteur)*
    * Un bien immobilier peut être acheté par zéro ou un client acheteur (`0,1`).
    * Un client peut acheter zéro ou plusieurs biens (`0,N`).

* **CLIENT** `(0,N)` <--- *Émettre* ---> `(1,1)` **OFFER**
    * Une offre est émise par un seul client (`1,1`).
    * Un client peut émettre zéro ou plusieurs offres (`0,N`).

* **AGENT** `(0,N)` <--- *Superviser* ---> `(1,1)` **OFFER**
    * Une offre est supervisée ou négociée par un seul agent (`1,1`).
    * Un agent peut superviser zéro ou plusieurs offres (`0,N`).

* **PROPERTY** `(0,N)` <--- *Concerner* ---> `(1,1)` **OFFER**
    * Une offre concerne un et un seul bien immobilier (`1,1`).
    * Un bien immobilier peut recevoir zéro ou plusieurs offres (`0,N`).

* **CLIENT** `(1,1)` <--- *Posséder* ---> `(0,N)` **WISHLIST**
    * Une ligne de favori appartient à un seul client (`1,1`).
    * Un client peut avoir zéro ou plusieurs lignes dans sa liste de favoris (`0,N`).

* **PROPERTY** `(0,N)` <--- *Être ciblé* ---> `(1,1)` **WISHLIST**
    * Une ligne de favori cible un seul bien immobilier (`1,1`).
    * Un bien immobilier peut être ciblé dans zéro ou plusieurs listes de favoris (`0,N`).