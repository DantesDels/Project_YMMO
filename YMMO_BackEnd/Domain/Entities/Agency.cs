using System.Collections.Generic;

namespace YMMO.Backend.Domain.Entities;

public class Agency
{
    public Guid AgencyID { get; set; }
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    
    // N:1 Relationship - An agency is located at one specific Location
    public Guid LocationID { get; set; }
    public Location Location { get; set; } = null!;
    
    // 1:N Relationships
    public ICollection<Agent> Agents { get; set; } = new List<Agent>();
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}