using System.ComponentModel.DataAnnotations;
using YMMO.Backend.Application.DTOs.Location;
using YMMO.Backend.Domain.Entities.Enums;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Application.DTOs.Property;

public class CreatePropertyDto
{
    [Required]
    public PropertyType PropertyType { get; set; }
    
    [Required]
    public int YearBuilt { get; set; }
    
    [Required]
    public PhysicalCondition Condition { get; set; }
    
    public EnergyClass EnergyClass { get; set; }

    [Required]
    [Range(1, 100_000_000)]
    public decimal InitialPrice { get; set; }
    
    [Required]
    [Range(9, 10_000)]
    public decimal Surface { get; set; }

    public List<Criteria> Features { get; set; } = new();

    [Required]
    public CreateLocationDto Location { get; set; } = null!;

    [Required]
    public Guid AgencyID { get; set; }
    
    [Required]
    public Guid AgentID { get; set; }
    
    [Required]
    public Guid SellerID { get; set; }
}