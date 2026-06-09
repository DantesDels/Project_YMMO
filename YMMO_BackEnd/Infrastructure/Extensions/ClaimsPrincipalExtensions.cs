using System.Security.Claims;
using YMMO.Backend.Domain.Enums;

public static class ClaimsPrincipalExtensions
{
    public static bool IsInRole(this ClaimsPrincipal user, ContactRole role)
    {
        return user.IsInRole(role.ToString());
    }
}