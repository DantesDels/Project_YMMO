using System.Collections.Generic;

namespace YMMO.Backend.Domain.Entities;

public class Agency
{
    public Guid AgencyId { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    
    // N:1 Relationship - An agency is located at one specific Location
    public Guid LocationId { get; set; }
    public Location Location { get; set; } = null!;
    
    // 1:N Relationships
    public ICollection<Agent> Agents { get; set; } = new List<Agent>();
    public ICollection<Property> Properties { get; set; } = new List<Property>();
    
    public void UpdateDetails(string name, string email, string phoneNumber)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name : Name;
        Email = !string.IsNullOrWhiteSpace(email) ? email : Email;
        PhoneNumber = !string.IsNullOrWhiteSpace(phoneNumber) ? phoneNumber : PhoneNumber;
    }
}