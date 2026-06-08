using YMMO.Backend.Application.DTOs.Location;
using YMMO.Backend.Application.DTOs.Agent;

namespace YMMO.Backend.Application.DTOs.Property;

public class PropertyDetailDto
{
    public Guid PropertyID { get; set; }
    public string PropertyType { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string EnergyClass { get; set; } = string.Empty;
    
    public decimal CurrentPrice { get; set; }
    public decimal InitialPrice { get; set; }
    public decimal Surface { get; set; }
    public DateTime DateListed { get; set; }
    
    public List<string> Features { get; set; } = new();

    public LocationDto Location { get; set; } = null!;
    public AgentContactDto Agent { get; set; } = null!;
}