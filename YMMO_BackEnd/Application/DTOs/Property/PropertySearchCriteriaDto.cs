using System.ComponentModel.DataAnnotations;
using YMMO.Backend.Domain.Entities.Enums;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Application.DTOs.Property;

public class PropertySearchCriteriaDto
{
    public string? City { get; set; }
    public string? Region { get; set; }
    public PropertyType? Type { get; set; }
    
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    
    public decimal? MinSurface { get; set; }
    public decimal? MaxSurface { get; set; }
    
    public List<PhysicalCondition> Conditions { get; set; } = new();
    public List<EnergyClass> EnergyClasses { get; set; } = new();
    
    public List<Criteria> RequiredCriteria { get; set; } = new();

    [Range(1, int.MaxValue, ErrorMessage = "PageNumber doit être >= 1")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize doit être entre 1 et 100")]
    public int PageSize { get; set; } = 20;
}