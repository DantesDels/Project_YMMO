namespace YMMO.Backend.Domain.Enums;

public enum ContactRole
{
    Client,         // Un utilisateur standard (Acheteur / Vendeur)
    Agent,          // Un agent immobilier (accès à ses dossiers)
    Manager,        // Un directeur d'agence (accès à toute son agence)
    Admin           // Administrateur système (accès total)
}