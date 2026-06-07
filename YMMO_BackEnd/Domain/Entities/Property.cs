using System.Collections.Generic;
using YMMO.Backend.Domain.Enum;

namespace YMMO.Backend.Domain.Entities;

public class Property
{
    public Guid PropertyID { get; set; }
    public DateTime DateListed { get; set; }
    public DateTime? DateSold { get; set; } // Nullable since it might not be sold initially
    
    public decimal InitialPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal? FinalPrice { get; set; }
    
    public State State { get; set; }
    public PropertyType PropertyType { get; set; }
    public EnergyClass EnergyClass { get; set; }
    public int YearBuilt { get; set; }
    public decimal Surface { get; set; }
    
    // --- N:1 Relationships ---
    
    public Guid AgencyID { get; set; }
    public Agency Agency { get; set; }
    
    public Guid LocationID { get; set; }
    public Location Location { get; set; }
    
    // The current owner (seller)
    public Guid SellerID { get; set; }
    public Client Seller { get; set; }
    
    // The buyer (Nullable as long as the property remains unsold)
    public Guid? BuyerID { get; set; }
    public Client Buyer { get; set; }
    
    // --- 1:N Relationships ---
    
    public ICollection<Offer> Offers { get; set; }
    public ICollection<Wishlist> Wishlists { get; set; }
}