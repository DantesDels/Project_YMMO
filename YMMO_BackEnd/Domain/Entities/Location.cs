using System.Collections.Generic;

namespace YMMO.Backend.Domain.Entities;

public class Location
{
    public Guid LocationID { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string Region { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
    
    // Complement is optional — not every address has a building or floor detail
    public string? Complement { get; set; }
    
    // 1:N Relationships (Multiple agencies or properties can share the same location)
    public ICollection<Agency> Agencies { get; set; } = new List<Agency>();
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}