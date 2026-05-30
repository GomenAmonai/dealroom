using DealRoom.Core.Entities;

namespace DealRoom.Core.Interfaces;

public interface IMessageRepository
{
    Task<Message> AddAsync(Message message);
    Task<IReadOnlyList<Message>> GetByDealAsync(int dealId);
}
