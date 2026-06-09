using YMMO.Backend.Domain.Entities.Enums;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Application.DTOs.Property;

public class PropertySearchCriteriaDto
{
    public string? City { get; set; }
    public string? Region { get; set; }
    public PropertyType? Type { get; set; }
    public int? MinRooms { get; set; }
    
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    
    public decimal? MinSurface { get; set; }
    public decimal? MaxSurface { get; set; }
    
    public PhysicalCondition? Condition { get; set; } 
    
    public List<Criteria> RequiredFeatures { get; set; } = new();

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}