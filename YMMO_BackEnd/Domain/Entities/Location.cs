using System.Collections.Generic;

namespace YMMO.Backend.Domain.Entities;

public class Location
{
    public Guid LocationID { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string PostalCode { get; set; }
    public string Complement { get; set; }
    public string Country { get; set; }
    
    // 1:N Relationships (Multiple agencies or properties can share the same location)
    public ICollection<Agency> Agencies { get; set; }
    public ICollection<Property> Properties { get; set; }
}