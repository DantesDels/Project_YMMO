using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Domain.Entities;

public class Offer
{
    public Guid OfferID { get; set; }
    public required DateTime DateCreated { get; set; }
    
    // Modified and price update dates are null until those events occur
    public DateTime? DateModified { get; set; }
    public DateTime? DatePriceUpdated { get; set; }
    
    public required decimal OfferPrice { get; set; }
    public required StatusOffer Status { get; set; }
    
    // N:1 Relationships - An offer always has a client, an agent and a property (1,1)
    public Guid ClientID { get; set; }
    public Client Client { get; set; } = null!;

    public Guid AgentID { get; set; }
    public Agent Agent { get; set; } = null!;

    public Guid PropertyID { get; set; }
    public Property Property { get; set; } = null!;
}