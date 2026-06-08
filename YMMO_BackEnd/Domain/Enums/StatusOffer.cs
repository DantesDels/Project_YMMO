namespace YMMO.Backend.Domain.Enums;

public enum StatusOffer
{
    Pending,        // 1. Offre envoyée, en attente d'une réponse
    Negotiation,    // 2. Le vendeur a fait une contre-proposition (En négo)
    Accepted,       // 3. Le vendeur a dit OUI
    Rejected,       // 4. Le vendeur a dit NON
    Canceled        // 5. L'acheteur a retiré son offre
}