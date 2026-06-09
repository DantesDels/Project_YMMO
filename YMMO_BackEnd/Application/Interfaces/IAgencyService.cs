using YMMO.Backend.Application.DTOs.Agency;

namespace YMMO.Backend.Application.Interfaces;

public interface IAgencyService
{
    // Retrieve all agencies (Public access)
    Task<IEnumerable<AgencyDto>> GetAllAgenciesAsync();
    
    // Admin/Manager operations
    Task<AgencyDto> UpdateAgencyAsync(Guid id, UpdateAgencyDto dto);
}