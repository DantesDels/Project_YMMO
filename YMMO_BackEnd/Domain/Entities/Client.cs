using System.Collections.Generic;
using YMMO.Backend.Domain.Entities.Enums;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Domain.Entities;

public class Client : Contact
{
    public Client()
    {
        ContactRole = ContactRole.Client; 
    }
    
    public required DateTime CreatedAt { get; set; }

    // Criteria is optional — a client may not have defined search criteria yet
    public Criteria? Criteria { get; set; }
    
    // N:1 Relationship - A client is linked to one main agent
    public Guid? AgentID { get; set; }
    public Agent? LinkedAgent { get; set; }
    
    // 1:N Relationships
    public ICollection<Property> OwnedProperties { get; set; } = new List<Property>(); // Client is a Seller
    public ICollection<Property> BoughtProperties { get; set; } = new List<Property>(); // Client is a Buyer
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();}