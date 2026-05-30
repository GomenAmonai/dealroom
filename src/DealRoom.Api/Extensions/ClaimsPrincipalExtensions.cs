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
}
