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
    public Guid AgencyId { get; set; }
    public Agency Agency { get; set; } = null!;
    
    // 1:N Relationships
    public ICollection<Client> LinkedClients { get; set; } = new List<Client>();
    public ICollection<Offer> ManagedOffers { get; set; } = new List<Offer>();
    public ICollection<Property> Properties { get; set; } = new List<Property>();

    
    public void AddProperty(Property property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        Properties.Add(property);
    }
    
    public void RemoveProperty(Property property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        
        if (!Properties.Contains(property))
            throw new InvalidOperationException("Cette propriété ne fait pas partie du catalogue de cet agent.");

        Properties.Remove(property);
    }
}