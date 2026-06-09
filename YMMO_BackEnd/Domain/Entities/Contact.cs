using System.ComponentModel.DataAnnotations;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Domain.Entities;

// 'abstract' because we only instantiate Client or Agent
public abstract class Contact
{
    private string _passwordHash = string.Empty;
    
    
    public Guid ContactId { get; set; } =  Guid.NewGuid();
    
    [MaxLength(100)]
    public required string LastName { get; set; }
    
    [MaxLength(100)]
    public required string FirstName { get; set; }
    
    [MaxLength(255)]
    public required string Email { get; set; }
    
    [MaxLength(20)] // Format E.164 (max 15 numbers + '+')
    public required string PhoneNumber { get; set; }
    
    public ContactRole ContactRole { get; set; }
    
    [MaxLength(255)]
    public string PasswordHash { get; internal set; } = string.Empty;
    
    
    public void SetRole(ContactRole role)
    {
        ContactRole = role;
    }
    
    public void UpdatePassword(string newHashedPassword)
    {
        if (string.IsNullOrWhiteSpace(newHashedPassword))
            throw new ArgumentException("Le hash ne peut pas être vide.");
            
        _passwordHash = newHashedPassword;
    }
}