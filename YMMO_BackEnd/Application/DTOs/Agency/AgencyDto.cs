using YMMO.Backend.Application.DTOs.Location;

namespace YMMO.Backend.Application.DTOs.Agency;

public class AgencyDto
{
    public Guid AgencyID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    
    public LocationDto Location { get; set; } = null!;
}