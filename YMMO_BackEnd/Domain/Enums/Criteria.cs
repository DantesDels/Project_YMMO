namespace YMMO.Backend.Domain.Entities.Enums;

public enum Criteria
{
    // --- Typologie (Number of rooms) ---
    Studio,             // Studio (T1/T1bis)
    T2,                 // T2
    T3,                 // T3
    T4,                 // T4
    T5Plus,             // T5 et plus
    
    // --- Extérieurs et Annexes (Outdoors & Annexes) ---
    Balcony,            // Balcon
    Terrace,            // Terrasse
    Garden,             // Jardin
    Garage,             // Garage fermé
    Parking,            // Place de parking ouverte/couverte
    Cellar,             // Cave
    SwimmingPool,       // Piscine

    // --- Intérieur et Confort (Interior & Comfort) ---
    Elevator,           // Ascenseur
    AirConditioning,    // Climatisation
    Fireplace,          // Cheminée
    Furnished,          // Vendu/Loué meublé
    HardwoodFloor,      // Parquet
    DoubleGlazing,      // Double vitrage
    FittedKitchen,      // Cuisine équipée

    // --- Sécurité et Accès (Security & Access) ---
    Digicode,           // Digicode
    Intercom,           // Interphone / Visiophone
    AlarmSystem,        // Système d'alarme
    SecurityDoor,       // Porte blindée
    DisabledAccess,     // Accès PMR (Personnes à Mobilité Réduite)
    Caretaker,          // Gardien / Concierge

    // --- Vues et Environnement (Views & Environment) ---
    SeaView,            // Vue mer
    MountainView,       // Vue montagne
    UnobstructedView,   // Vue dégagée / Sans vis-à-vis
    SouthFacing,        // Plein sud / Très lumineux

    // --- Énergie et Technologie (Energy & Tech) ---
    FiberOptic,         // Raccordé à la fibre optique
    SmartHome,          // Domotique (Maison connectée)
    HeatPump,           // Pompe à chaleur
    SolarPanels         // Panneaux solaires
}