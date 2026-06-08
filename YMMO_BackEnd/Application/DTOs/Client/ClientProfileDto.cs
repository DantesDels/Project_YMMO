namespace YMMO.Backend.Application.DTOs.Client;

public class ClientProfileDto
{
    public Guid ClientID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    
    // Counters for Dashboard
    public int ActiveOffersCount { get; set; }
    public int WishlistItemsCount { get; set; }
}