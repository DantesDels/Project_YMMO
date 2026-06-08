using YMMO.Backend.Domain.Entities.Enums;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Domain.Filters;

public class PropertySearchCriteria 
{
    public PropertyType? Type { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinSurface { get; set; }
    public decimal? MaxSurface { get; set; }
    
    public string? City { get; set; }
    
    public List<Criteria>? RequiredCriteria { get; set; }
}