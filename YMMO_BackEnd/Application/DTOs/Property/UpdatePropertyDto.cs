using System.ComponentModel.DataAnnotations;
using YMMO.Backend.Domain.Entities.Enums;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Application.DTOs.Property;

public class UpdatePropertyDto
{
    [Required]
    [Range(1, 100_000_000)]
    public decimal CurrentPrice { get; set; } // Agent can lower the price

    [Required]
    public PhysicalCondition Condition { get; set; } // State can be improved if work has been carried out

    public EnergyClass EnergyClass { get; set; } // New DPE realised

    public List<Criteria> Features { get; set; } = new(); // New swimming pool added ?
}