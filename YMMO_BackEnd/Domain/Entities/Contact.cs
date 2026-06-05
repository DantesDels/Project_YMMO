namespace YMMO.Backend.Domain.Entities;

// 'abstract' because we only instantiate Client or Agent
public abstract class Contact
{
    public Guid ContactID { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}