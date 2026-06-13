using System.Collections.Generic;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Domain.Entities;

public class Agent : Contact
{
    public Agent()
    {
        ContactRole = ContactRole.Agent;
    }
    
    private readonly List<Property> _properties = new();
    
    // N:1 Relationship - An agent works in a specific agency
    public Guid AgencyId { get; set; }
    public Agency Agency { get; set; } = null!;
    
    // 1:N Relationships
    public ICollection<Client> LinkedClients { get; set; } = new List<Client>();
    public ICollection<Offer> ManagedOffers { get; set; } = new List<Offer>();
    public IReadOnlyCollection<Property> Properties => _properties.AsReadOnly();

    
    public void AddProperty(Property property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        _properties.Add(property);
    }
    
    public void RemoveProperty(Property property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        
        if (!_properties.Contains(property))
            throw new InvalidOperationException("Cette propriété ne fait pas partie du catalogue de cet agent.");

        _properties.Remove(property);
    }
}