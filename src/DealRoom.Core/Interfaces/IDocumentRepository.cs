using DealRoom.Core.Entities;

namespace DealRoom.Core.Interfaces;

public interface IDocumentRepository
{
    Task<Document> AddAsync(Document document);
    Task<Document?> GetByIdAsync(int id);
    Task<IReadOnlyList<Document>> GetByDealAsync(int dealId);
    Task UpdateAsync(Document document);
}
