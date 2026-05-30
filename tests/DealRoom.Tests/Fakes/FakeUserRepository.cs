using DealRoom.Core.Entities;
using DealRoom.Core.Interfaces;

namespace DealRoom.Tests.Fakes;

public class FakeUserRepository : IUserRepository
{
    private readonly Dictionary<int, User> _store;
    private int _nextId;

    public FakeUserRepository(Dictionary<int, User> store)
    {
        _store = store;
        _nextId = store.Count + 1;
    }

    public Task<User?> GetByEmailAsync(string email)
        => Task.FromResult(_store.Values.FirstOrDefault(u => u.Email == email));

    public Task<User?> GetByIdAsync(int id)
        => Task.FromResult(_store.TryGetValue(id, out var user) ? user : null);

    public Task<User> CreateAsync(User user)
    {
        user.Id = _nextId++;
        _store[user.Id] = user;
        return Task.FromResult(user);
    }
}
