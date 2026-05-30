using DealRoom.Core.Entities;
using DealRoom.Core.Interfaces;

namespace DealRoom.Tests.Fakes;

public class FakeMessageRepository : IMessageRepository
{
    private readonly List<Message> _store = new();
    private int _nextId = 1;

    public Task<Message> AddAsync(Message message)
    {
        message.Id = _nextId++;
        _store.Add(message);
        return Task.FromResult(message);
    }

    public Task<IReadOnlyList<Message>> GetByDealAsync(int dealId)
        => Task.FromResult<IReadOnlyList<Message>>(
            _store.Where(m => m.DealId == dealId).OrderBy(m => m.SentAt).ToList());
}
