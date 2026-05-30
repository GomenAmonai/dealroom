using System.Security.Claims;
using DealRoom.Api.Extensions;
using DealRoom.Core.DTOs;
using DealRoom.Core.Interfaces;

namespace DealRoom.Api.Endpoints;

public static class DocumentEndpoints
{
    public static void MapDocumentEndpoints(this WebApplication app)
    {
        var dealDocuments = app.MapGroup("/deals/{dealId:int}/documents")
            .WithTags("Documents")
            .RequireAuthorization();

        dealDocuments.MapPost("/", async (
            int dealId, IFormFile file, ClaimsPrincipal user, IDocumentService documents) =>
        {
            await using var stream = file.OpenReadStream();
            var result = await documents.UploadAsync(
                dealId, user.GetOrganizationId(), file.FileName, file.ContentType, file.Length, stream);
            return Results.Created($"/documents/{result.Id}", result);
        })
        .DisableAntiforgery()
        .Produces<DocumentResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        dealDocuments.MapGet("/", async (int dealId, ClaimsPrincipal user, IDocumentService documents) =>
        {
            var result = await documents.GetForDealAsync(dealId, user.GetOrganizationId());
            return Results.Ok(result);
        })
        .Produces<IReadOnlyList<DocumentResponse>>(StatusCodes.Status200OK);

        var single = app.MapGroup("/documents")
            .WithTags("Documents")
            .RequireAuthorization();

        single.MapGet("/{id:int}/content", async (int id, ClaimsPrincipal user, IDocumentService documents) =>
        {
            var content = await documents.DownloadAsync(id, user.GetOrganizationId());
            return Results.File(content.Stream, content.ContentType, content.FileName);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        single.MapPost("/{id:int}/approve", async (int id, ClaimsPrincipal user, IDocumentService documents) =>
        {
            var result = await documents.ApproveAsync(id, user.GetOrganizationId());
            return Results.Ok(result);
        })
        .Produces<DocumentResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status409Conflict);

        single.MapPost("/{id:int}/reject", async (int id, ClaimsPrincipal user, IDocumentService documents) =>
        {
            var result = await documents.RejectAsync(id, user.GetOrganizationId());
            return Results.Ok(result);
        })
        .Produces<DocumentResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status409Conflict);
    }
}
