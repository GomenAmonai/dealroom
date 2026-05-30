using DealRoom.Core.DTOs;
using DealRoom.Core.Entities;
using DealRoom.Core.Exceptions;
using DealRoom.Core.Interfaces;

namespace DealRoom.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messages;
    private readonly IDealRepository _deals;
    private readonly IUserRepository _users;

    public MessageService(IMessageRepository messages, IDealRepository deals, IUserRepository users)
    {
        _messages = messages;
        _deals = deals;
        _users = users;
    }

    public async Task<MessageResponse> SendAsync(int dealId, int senderUserId, int requestingOrgId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new EmptyMessageContentException();

        await EnsureParticipantAsync(dealId, requestingOrgId);

        var sender = await _users.GetByIdAsync(senderUserId)
            ?? throw new InvalidOperationException($"Authenticated user {senderUserId} no longer exists.");

        var message = await _messages.AddAsync(new Message
        {
            DealId = dealId,
            SenderUserId = senderUserId,
            SenderOrganizationId = requestingOrgId,
            SenderName = sender.Name,
            Content = content.Trim(),
            SentAt = DateTime.UtcNow
        });

        return ToResponse(message);
    }

    public async Task<IReadOnlyList<MessageResponse>> GetForDealAsync(int dealId, int requestingOrgId)
    {
        await EnsureParticipantAsync(dealId, requestingOrgId);
        var messages = await _messages.GetByDealAsync(dealId);
        return messages.Select(ToResponse).ToList();
    }

    private async Task EnsureParticipantAsync(int dealId, int requestingOrgId)
    {
        var deal = await _deals.GetByIdAsync(dealId) ?? throw new DealNotFoundException(dealId);

        if (deal.InitiatorOrganizationId != requestingOrgId
            && deal.CounterpartyOrganizationId != requestingOrgId)
            throw new NotDealParticipantException(requestingOrgId, dealId);
    }

    private static MessageResponse ToResponse(Message message) => new()
    {
        Id = message.Id,
        DealId = message.DealId,
        SenderUserId = message.SenderUserId,
        SenderOrganizationId = message.SenderOrganizationId,
        SenderName = message.SenderName,
        Content = message.Content,
        SentAt = message.SentAt
    };
}
