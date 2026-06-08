
namespace YMMO.Backend.Application.DTOs.Properties;

public class PropertySummaryDto
{
    public Guid PropertyId { get; set; }
    public string PropertyType { get; set; } = string.Empty; // ex: "House", "Appartement"
    public string Condition { get; set; } = string.Empty; // ex: "New", "Ruin"
    public decimal CurrentPrice { get; set; }
    public decimal Surface { get; set; }
    
    // From Location Entity (flattening data)
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    
    public List<string> MainFeatures { get; set; } = new(); // ex: "Balcony", "Garage"
}