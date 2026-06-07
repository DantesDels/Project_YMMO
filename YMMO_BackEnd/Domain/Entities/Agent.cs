using System.Collections.Generic;
using YMMO.Backend.Domain.Enum;

namespace YMMO.Backend.Domain.Entities;

public class Agent : Contact
{
    // N:1 Relationship - An agent works in a specific agency
    public Guid AgencyID { get; set; }
    public Agency Agency { get; set; }
    
    // 1:N Relationships
    public ICollection<Client> LinkedClients { get; set; }
    public ICollection<Property> SoldProperties { get; set; }
    public ICollection<Offer> ManagedOffers { get; set; }
}