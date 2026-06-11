
namespace YMMO.Backend.Application.DTOs.Property;

public class PropertySummaryDto
{
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal Surface { get; set; }
    
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    
    public List<string> MainFeatures { get; set; } = new();
}