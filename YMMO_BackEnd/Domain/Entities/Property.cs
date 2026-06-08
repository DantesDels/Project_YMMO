using System.Collections.Generic;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Domain.Entities;

public class Property
{
    public Guid PropertyID { get; set; }
    public required DateTime DateListed { get; set; }
    
    // Nullable since it might not be sold initially
    public DateTime? DateSold { get; set; } 
    
    public required decimal InitialPrice { get; set; }
    public required decimal CurrentPrice { get; set; }
    // FinalPrice is null until the sale is closed
    public decimal? FinalPrice { get; set; }
    
    public required State State { get; set; }
    public required PropertyType PropertyType { get; set; }
    public required EnergyClass EnergyClass { get; set; }
    public required int YearBuilt { get; set; }
    public required decimal Surface { get; set; }
    public required PhysicalCondition Condition { get; set; }
    public List<Criteria> Features { get; set; } = new List<Criteria>();
    
    // --- N:1 Relationships --- A property always belongs to an agency and has a location (1,1)
    
    // The agency having the property responsibility
    public Guid AgencyID { get; set; }
    public Agency Agency { get; set; } = null!;
    
    // The agent responsible for managing this property listing
    public Guid? AgentID { get; set; }
    public Agent? Agent { get; set; }
    
    public Guid LocationID { get; set; }
    public Location Location { get; set; } = null!;
    
    // A property always has a seller (1,1)
    public Guid SellerID { get; set; }
    public Client Seller { get; set; } = null!;
    
    // Buyer is null until the property is sold (0,1)
    public Guid? BuyerID { get; set; }
    public Client? Buyer { get; set; }
    
    
    // --- 1:N Relationships ---
    
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}