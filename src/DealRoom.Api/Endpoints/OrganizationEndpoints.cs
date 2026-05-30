using System.Security.Claims;
using DealRoom.Api.Extensions;
using DealRoom.Core.DTOs;
using DealRoom.Core.Exceptions;
using DealRoom.Core.Interfaces;

namespace DealRoom.Api.Endpoints;

public static class OrganizationEndpoints
{
    public static void MapOrganizationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/organizations").WithTags("Organizations").RequireAuthorization();

        group.MapGet("/me", async (ClaimsPrincipal user, IOrganizationRepository organizations) =>
        {
            var orgId = user.GetOrganizationId();
            var organization = await organizations.GetByIdAsync(orgId)
                ?? throw new OrganizationNotFoundException(orgId);

            return Results.Ok(new OrganizationResponse
            {
                Id = organization.Id,
                Name = organization.Name,
                CreatedAt = organization.CreatedAt
            });
        })
        .Produces<OrganizationResponse>(StatusCodes.Status200OK);
    }
}
