using DealRoom.Core.Entities;
using DealRoom.Core.Interfaces;

namespace DealRoom.Tests.Fakes;

public class FakeOrganizationRepository : IOrganizationRepository
{
    private readonly Dictionary<int, Organization> _store;
    private int _nextId;

    public FakeOrganizationRepository(Dictionary<int, Organization> store)
    {
        _store = store;
        _nextId = store.Count + 1;
    }

    public Task<Organization> CreateAsync(Organization organization)
    {
        organization.Id = _nextId++;
        _store[organization.Id] = organization;
        return Task.FromResult(organization);
    }

    public Task<Organization?> GetByIdAsync(int id)
        => Task.FromResult(_store.TryGetValue(id, out var org) ? org : null);
}
