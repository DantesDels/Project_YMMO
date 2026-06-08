namespace YMMO.Backend.Domain.Enums;

public enum PhysicalCondition
{
    New,                // 1 Neuf (VEFA ou juste livré)
    Excellent,          // 2 Excellent état (Rénové récemment, aucun travaux)
    Good,               // 3 Bon état (Habitable de suite)
    NeedsRefresh,       // 4 À rafraîchir (Peinture, déco, mais sain)
    NeedsRenovation,    // 5 À rénover (Gros travaux à prévoir)
    Ruin                // 6 Ruine / Terrain à bâtir
}