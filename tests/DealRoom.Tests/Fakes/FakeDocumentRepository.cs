using DealRoom.Core.Entities;
using DealRoom.Core.Interfaces;

namespace DealRoom.Tests.Fakes;

public class FakeDocumentRepository : IDocumentRepository
{
    private readonly List<Document> _store = new();
    private int _nextId = 1;

    public Task<Document> AddAsync(Document document)
    {
        document.Id = _nextId++;
        _store.Add(document);
        return Task.FromResult(document);
    }

    public Task<Document?> GetByIdAsync(int id)
        => Task.FromResult(_store.FirstOrDefault(d => d.Id == id));

    public Task<IReadOnlyList<Document>> GetByDealAsync(int dealId)
        => Task.FromResult<IReadOnlyList<Document>>(_store.Where(d => d.DealId == dealId).ToList());

    public Task UpdateAsync(Document document) => Task.CompletedTask;
}
