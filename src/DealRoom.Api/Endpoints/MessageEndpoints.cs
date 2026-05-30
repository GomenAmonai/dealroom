using System.Security.Claims;
using DealRoom.Api.Extensions;
using DealRoom.Api.Hubs;
using DealRoom.Core.DTOs;
using DealRoom.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DealRoom.Api.Endpoints;

public static class MessageEndpoints
{
    public static void MapMessageEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/deals/{dealId:int}/messages")
            .WithTags("Messages")
            .RequireAuthorization();

        group.MapGet("/", async (int dealId, ClaimsPrincipal user, IMessageService messages) =>
        {
            var result = await messages.GetForDealAsync(dealId, user.GetOrganizationId());
            return Results.Ok(result);
        })
        .Produces<IReadOnlyList<MessageResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden);

        group.MapPost("/", async (
            int dealId, SendMessageRequest request, ClaimsPrincipal user,
            IMessageService messages, IHubContext<DealChatHub> hub) =>
        {
            var message = await messages.SendAsync(
                dealId, user.GetUserId(), user.GetOrganizationId(), request.Content);

            await hub.Clients.Group(DealChatHub.GroupName(dealId)).SendAsync("ReceiveMessage", message);

            return Results.Created($"/deals/{dealId}/messages/{message.Id}", message);
        })
        .Produces<MessageResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status403Forbidden);
    }
}
