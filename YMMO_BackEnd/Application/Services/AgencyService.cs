using AutoMapper;
using YMMO.Backend.Application.DTOs.Agency;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Domain.Repositories;

namespace YMMO.Backend.Application.Services;

public class AgencyService : IAgencyService
{
    private readonly IAgencyRepository _agencyRepository;
    private readonly IMapper _mapper;

    public AgencyService(IAgencyRepository agencyRepository, IMapper mapper)
    {
        _agencyRepository = agencyRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AgencyDto>> GetAllAgenciesAsync()
    {
        var agencies = await _agencyRepository.GetAllAsync();
        
        // BEFORE MAPPER
        //return agencies.Select(a => new AgencyDto
        //{
        //    AgencyID = a.AgencyID,
        //    Name = a.Name,
        //    Email = a.Email,
        //    PhoneNumber = a.PhoneNumber,
        //    Location = new LocationDto 
        //    { 
        //        Address = a.Location.Address, 
        //        City = a.Location.City 
        //    }
        //});
        
        // AFTER MAPPER
        return _mapper.Map<IEnumerable<AgencyDto>>(agencies);
    }

    public async Task<AgencyDto> UpdateAgencyAsync(Guid id, UpdateAgencyDto dto)
    {
        var agency = await _agencyRepository.GetByIdAsync(id);
        if (agency == null) throw new KeyNotFoundException("Agence introuvable.");
        
        _mapper.Map(dto, agency);
        _mapper.Map(dto.NewAddress, agency.Location); 

        await _agencyRepository.UpdateAsync(agency);
        return _mapper.Map<AgencyDto>(agency);
    }
}