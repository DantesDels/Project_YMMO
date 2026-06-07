using YMMO.Backend.Domain.Enum;

namespace YMMO.Backend.Domain.Entities;

public class Offer
{
    public Guid OfferID { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
    public DateTime DatePriceUpdated { get; set; }
    
    public decimal OfferPrice { get; set; }
    public StatusOffer Status { get; set; }
    
    // N:1 Relationships
    public Guid ClientID { get; set; }
    public Client Client { get; set; }
    
    public Guid AgentID { get; set; }
    public Agent Agent { get; set; }
    
    public Guid PropertyID { get; set; }
    public Property Property { get; set; }
}