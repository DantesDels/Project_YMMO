namespace YMMO.Backend.Domain.Enums;

public enum EnergyClass
{
    A,              // Moins de 50 kWh/m²/an (Très économe)
    B,
    C,
    D,
    E,
    F,              // Passoire thermique
    G,              // Passoire thermique très énergivore
    Exempt          // Non soumis au DPE (ex: Terrain nu, VEFA en cours)
}