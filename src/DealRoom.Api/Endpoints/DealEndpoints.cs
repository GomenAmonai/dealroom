using System.Security.Claims;
using DealRoom.Api.Extensions;
using DealRoom.Core.DTOs;
using DealRoom.Core.Interfaces;

namespace DealRoom.Api.Endpoints;

public static class DealEndpoints
{
    public static void MapDealEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/deals").WithTags("Deals").RequireAuthorization();

        group.MapPost("/", async (CreateDealRequest request, ClaimsPrincipal user, IDealService deals) =>
        {
            var result = await deals.CreateAsync(user.GetOrganizationId(), request);
            return Results.Created($"/deals/{result.Id}", result);
        })
        .Produces<DealResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (ClaimsPrincipal user, IDealService deals) =>
        {
            var result = await deals.GetForOrganizationAsync(user.GetOrganizationId());
            return Results.Ok(result);
        })
        .Produces<IReadOnlyList<DealResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", async (int id, ClaimsPrincipal user, IDealService deals) =>
        {
            var result = await deals.GetByIdAsync(id, user.GetOrganizationId());
            return Results.Ok(result);
        })
        .Produces<DealResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{id:int}/status", async (
            int id, UpdateDealStatusRequest request, ClaimsPrincipal user, IDealService deals) =>
        {
            var result = await deals.UpdateStatusAsync(id, user.GetOrganizationId(), request.Status);
            return Results.Ok(result);
        })
        .Produces<DealResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status422UnprocessableEntity);
    }
}
