using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Domain.Entities;

// 'abstract' because we only instantiate Client or Agent
public abstract class Contact
{
    public Guid ContactID { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required ContactRole ContactRole { get; set; }
}