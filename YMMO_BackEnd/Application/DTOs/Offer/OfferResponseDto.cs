namespace YMMO.Backend.Application.DTOs.Offer;

public class OfferResponseDto
{
    public Guid OfferID { get; set; }
    public Guid PropertyID { get; set; }
    public Guid ClientID { get; set; }
    
    public decimal OfferPrice { get; set; }
    public string Status { get; set; } = string.Empty; // "Pending", "Accepted", "Rejected"
    
    // Flattening Data for Vue.js
    public string PropertyCity { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
}