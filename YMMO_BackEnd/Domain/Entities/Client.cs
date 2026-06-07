using System.Collections.Generic;
using YMMO.Backend.Domain.Enum;

namespace YMMO.Backend.Domain.Entities;

public class Client : Contact
{
    public Criteria Criteria { get; set; }
    
    // N:1 Relationship - A client is linked to one main agent
    public Guid? AgentID { get; set; }
    public Agent LinkedAgent { get; set; }
    
    // 1:N Relationships
    public ICollection<Property> OwnedProperties { get; set; } // If the client is a seller
    public ICollection<Property> BoughtProperties { get; set; } // If the client is a buyer
    public ICollection<Offer> Offers { get; set; }
    public ICollection<Wishlist> Wishlists { get; set; }
}