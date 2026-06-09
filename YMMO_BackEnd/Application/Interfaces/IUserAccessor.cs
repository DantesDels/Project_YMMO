using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Application.Interfaces;

public interface IUserAccessor
{
    Guid GetCurrentUserId();
    ContactRole GetCurrentUserRole();
    Guid GetCurrentAgencyId();
}