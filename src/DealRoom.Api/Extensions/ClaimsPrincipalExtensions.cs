using System.Security.Claims;

namespace DealRoom.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetOrganizationId(this ClaimsPrincipal user)
    {
        var raw = user.FindFirst("orgId")?.Value;
        if (!int.TryParse(raw, out var orgId))
            throw new InvalidOperationException("Authenticated user is missing a valid 'orgId' claim.");

        return orgId;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        // JwtBearer remaps "sub" to ClaimTypes.NameIdentifier by default; accept either.
        var raw = user.FindFirst("sub")?.Value
                  ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(raw, out var userId))
            throw new InvalidOperationException("Authenticated user is missing a valid 'sub' claim.");

        return userId;
    }
}
