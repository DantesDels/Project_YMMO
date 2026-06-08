using System.Collections.Generic;
using YMMO.Backend.Domain.Enum;

namespace YMMO.Backend.Domain.Entities;

public class Client : Contact
{
    // Criteria is optional — a client may not have defined search criteria yet
    public Criteria? Criteria { get; set; }
    
    // N:1 Relationship - A client is linked to one main agent
    public Guid? AgentID { get; set; }
    public Agent? LinkedAgent { get; set; }
    
    // 1:N Relationships
    public ICollection<Property> OwnedProperties { get; set; } = new List<Property>(); // Client is a Seller
    public ICollection<Property> BoughtProperties { get; set; } = new List<Property>(); // Client is a Buyer
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}