namespace BelezaApp.API.Helpers;

using System.Security.Claims;

public static class TokenClaimsHelper
{
    public static string? ObterEmail(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }

    public static string? ObterRole(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value;
    }
}
