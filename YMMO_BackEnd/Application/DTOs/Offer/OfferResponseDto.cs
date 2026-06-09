namespace YMMO.Backend.Application.DTOs.Offer;

public class OfferResponseDto
{
    public Guid OfferID { get; set; }
    public Guid PropertyID { get; set; }
    public Guid ClientID { get; set; }
    public string ClientLastName { get; set; } = string.Empty;
    public string ClientFirstName { get; set; } = string.Empty;
    public string ClientPhoneNumber { get; set; } = string.Empty;
    
    public decimal OfferPrice { get; set; }
    public string StatusOffer { get; set; } = string.Empty; // "Pending", "Accepted", "Rejected"
    
    // Flattening Data for Vue.js
    public string PropertyCity { get; set; } = string.Empty;
    public string PropertyRegion { get; set; } = string.Empty;
}