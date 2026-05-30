using DealRoom.Api.Extensions;
using DealRoom.Core.Exceptions;
using DealRoom.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DealRoom.Api.Hubs;

[Authorize]
public class DealChatHub : Hub
{
    private readonly IMessageService _messages;
    private readonly IDealRepository _deals;

    public DealChatHub(IMessageService messages, IDealRepository deals)
    {
        _messages = messages;
        _deals = deals;
    }

    public static string GroupName(int dealId) => $"deal-{dealId}";

    public async Task JoinDeal(int dealId)
    {
        await EnsureParticipantAsync(dealId);
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(dealId));
    }

    public Task LeaveDeal(int dealId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(dealId));

    public async Task SendMessage(int dealId, string content)
    {
        var message = await _messages.SendAsync(
            dealId, Context.User!.GetUserId(), Context.User!.GetOrganizationId(), content);

        await Clients.Group(GroupName(dealId)).SendAsync("ReceiveMessage", message);
    }

    private async Task EnsureParticipantAsync(int dealId)
    {
        var orgId = Context.User!.GetOrganizationId();
        var deal = await _deals.GetByIdAsync(dealId) ?? throw new DealNotFoundException(dealId);

        if (deal.InitiatorOrganizationId != orgId && deal.CounterpartyOrganizationId != orgId)
            throw new NotDealParticipantException(orgId, dealId);
    }
}
