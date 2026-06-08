using System.Collections.Generic;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Domain.Entities;

public class Agent : Contact
{
    public Agent()
    {
        ContactRole = ContactRole.Agent;
    }
    
    // N:1 Relationship - An agent works in a specific agency
    public Guid AgencyID { get; set; }
    public Agency Agency { get; set; } = null!;
    
    // 1:N Relationships
    public ICollection<Client> LinkedClients { get; set; } = new List<Client>();
    public ICollection<Property> SoldProperties { get; set; } = new List<Property>();
    public ICollection<Offer> ManagedOffers { get; set; } = new List<Offer>();
}