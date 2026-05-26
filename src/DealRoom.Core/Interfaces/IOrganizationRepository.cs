using DealRoom.Core.Entities;

namespace DealRoom.Core.Interfaces;

public interface IOrganizationRepository
{
    Task<Organization> CreateAsync(Organization organization);
    Task<Organization?> GetByIdAsync(int id);
}
