namespace BelezaApp.API.Helpers;

using System.Security.Claims;

public static class TokenClaimsHelper
{
    public static int ObterUsuarioId(ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) return 0;

        return int.TryParse(claim.Value, out var id) ? id : 0;
    }

    public static string? ObterEmail(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }

    public static string? ObterRole(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value;
    }
}
