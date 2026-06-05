using System.Collections.Generic;

namespace YMMO.Backend.Domain.Entities;

public class Agency
{
    public Guid AgencyID { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    
    // N:1 Relationship - An agency is located at one specific Location
    public Guid LocationID { get; set; }
    public Location Location { get; set; }
    
    // 1:N Relationships
    public ICollection<Agent> Agents { get; set; }
    public ICollection<Property> Properties { get; set; }
}