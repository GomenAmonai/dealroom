using DealRoom.Core.DTOs;

namespace DealRoom.Core.Interfaces;

public interface IMessageService
{
    Task<MessageResponse> SendAsync(int dealId, int senderUserId, int requestingOrgId, string content);
    Task<IReadOnlyList<MessageResponse>> GetForDealAsync(int dealId, int requestingOrgId);
}
