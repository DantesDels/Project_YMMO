namespace YMMO.Backend.Domain.Enums;

public enum PropertyStatus
{
    Available,          // 1. La maison est sur le marché, visible par tous
    UnderOffer,         // 2. Une offre est "Accepted", la maison est bloquée (En négo chez le notaire)
    CompromiseSigned,   // 3. Le compromis de vente est signé
    Sold,               // 4. La vente est actée, les clés sont remises
    Withdrawn           // 5. Le vendeur a retiré son bien de la vente
}