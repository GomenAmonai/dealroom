using DealRoom.Core.DTOs;
using DealRoom.Core.Entities;

namespace DealRoom.Core.Interfaces;

public interface IDealService
{
    Task<DealResponse> CreateAsync(int initiatorOrgId, CreateDealRequest request);
    Task<IReadOnlyList<DealResponse>> GetForOrganizationAsync(int organizationId);
    Task<DealResponse> GetByIdAsync(int dealId, int requestingOrgId);
    Task<DealResponse> UpdateStatusAsync(int dealId, int requestingOrgId, DealStatus newStatus);
}
