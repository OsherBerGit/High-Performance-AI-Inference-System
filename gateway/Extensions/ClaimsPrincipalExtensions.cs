using System.Security.Claims;

namespace Gateway.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        
        if (claim is null)
            throw new UnauthorizedAccessException("User ID not found in token");

        return int.Parse(claim.Value);
    }
}