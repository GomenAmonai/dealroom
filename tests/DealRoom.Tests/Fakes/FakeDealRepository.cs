using DealRoom.Core.Entities;
using DealRoom.Core.Interfaces;

namespace DealRoom.Tests.Fakes;

// Mirrors EF Core's eager-loading: GetBy* populate the organization navigation
// properties so callers can read their names, the way DealRepository's .Include does.
public class FakeDealRepository : IDealRepository
{
    private readonly Dictionary<int, Organization> _orgs;
    private readonly List<Deal> _store = new();
    private int _nextId = 1;

    public FakeDealRepository(Dictionary<int, Organization> orgs) => _orgs = orgs;

    public Deal Seed(Deal deal)
    {
        deal.Id = _nextId++;
        _store.Add(deal);
        return deal;
    }

    public Task<Deal> AddAsync(Deal deal)
    {
        deal.Id = _nextId++;
        _store.Add(deal);
        return Task.FromResult(deal);
    }

    public Task<Deal?> GetByIdAsync(int id)
    {
        var deal = _store.FirstOrDefault(d => d.Id == id);
        if (deal is not null) Populate(deal);
        return Task.FromResult(deal);
    }

    public Task<IReadOnlyList<Deal>> GetByOrganizationAsync(int organizationId)
    {
        var list = _store
            .Where(d => d.InitiatorOrganizationId == organizationId
                     || d.CounterpartyOrganizationId == organizationId)
            .OrderByDescending(d => d.CreatedAt)
            .ToList();
        list.ForEach(Populate);
        return Task.FromResult<IReadOnlyList<Deal>>(list);
    }

    public Task UpdateAsync(Deal deal) => Task.CompletedTask;

    private void Populate(Deal deal)
    {
        if (_orgs.TryGetValue(deal.InitiatorOrganizationId, out var initiator))
            deal.InitiatorOrganization = initiator;
        if (_orgs.TryGetValue(deal.CounterpartyOrganizationId, out var counterparty))
            deal.CounterpartyOrganization = counterparty;
    }
}
