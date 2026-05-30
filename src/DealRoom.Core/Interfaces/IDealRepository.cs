using DealRoom.Core.Entities;

namespace DealRoom.Core.Interfaces;

public interface IDealRepository
{
    Task<Deal?> GetByIdAsync(int id);
    Task<IReadOnlyList<Deal>> GetByOrganizationAsync(int organizationId);
    Task<Deal> AddAsync(Deal deal);
    Task UpdateAsync(Deal deal);
}
