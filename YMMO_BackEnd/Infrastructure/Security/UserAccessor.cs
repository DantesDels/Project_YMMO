using System.Security.Claims;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Infrastructure.Security;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessor(IHttpContextAccessor httpContextAccessor) 
        => _httpContextAccessor = httpContextAccessor;

    public Guid GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("Utilisateur non identifié.");
        return Guid.Parse(userId);
    }

    public ContactRole GetCurrentUserRole()
    {
        var roleString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        
        if (string.IsNullOrEmpty(roleString))
            throw new UnauthorizedAccessException("Rôle introuvable dans le token.");

        return Enum.Parse<ContactRole>(roleString);
    }
    
    public Guid GetCurrentAgencyId()
    {
        var agencyIdString = _httpContextAccessor.HttpContext?.User.FindFirst("AgencyId")?.Value;
    
        if (string.IsNullOrEmpty(agencyIdString))
            throw new UnauthorizedAccessException("Agence introuvable dans le contexte utilisateur.");
        
        return Guid.Parse(agencyIdString);
    }
}