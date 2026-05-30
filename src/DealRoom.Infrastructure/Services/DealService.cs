using DealRoom.Core.DTOs;
using DealRoom.Core.Entities;
using DealRoom.Core.Exceptions;
using DealRoom.Core.Interfaces;

namespace DealRoom.Infrastructure.Services;

public class DealService : IDealService
{
    // Finite state machine: which statuses each status may transition into.
    private static readonly IReadOnlyDictionary<DealStatus, DealStatus[]> AllowedTransitions =
        new Dictionary<DealStatus, DealStatus[]>
        {
            [DealStatus.Draft] = new[] { DealStatus.Negotiation, DealStatus.Active, DealStatus.Cancelled },
            [DealStatus.Negotiation] = new[] { DealStatus.Active, DealStatus.Cancelled },
            [DealStatus.Active] = new[] { DealStatus.Closed, DealStatus.Cancelled },
            [DealStatus.Closed] = Array.Empty<DealStatus>(),
            [DealStatus.Cancelled] = Array.Empty<DealStatus>(),
        };

    private readonly IDealRepository _dealRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public DealService(IDealRepository dealRepository, IOrganizationRepository organizationRepository)
    {
        _dealRepository = dealRepository;
        _organizationRepository = organizationRepository;
    }

    public async Task<DealResponse> CreateAsync(int initiatorOrgId, CreateDealRequest request)
    {
        if (request.CounterpartyOrganizationId == initiatorOrgId)
            throw new SelfDealException(initiatorOrgId);

        _ = await _organizationRepository.GetByIdAsync(request.CounterpartyOrganizationId)
            ?? throw new OrganizationNotFoundException(request.CounterpartyOrganizationId);

        var deal = await _dealRepository.AddAsync(new Deal
        {
            Title = request.Title,
            Status = DealStatus.Draft,
            InitiatorOrganizationId = initiatorOrgId,
            CounterpartyOrganizationId = request.CounterpartyOrganizationId,
            CreatedAt = DateTime.UtcNow
        });

        var created = await _dealRepository.GetByIdAsync(deal.Id);
        return ToResponse(created!);
    }

    public async Task<IReadOnlyList<DealResponse>> GetForOrganizationAsync(int organizationId)
    {
        var deals = await _dealRepository.GetByOrganizationAsync(organizationId);
        return deals.Select(ToResponse).ToList();
    }

    public async Task<DealResponse> GetByIdAsync(int dealId, int requestingOrgId)
    {
        var deal = await GetParticipantDealAsync(dealId, requestingOrgId);
        return ToResponse(deal);
    }

    public async Task<DealResponse> UpdateStatusAsync(int dealId, int requestingOrgId, DealStatus newStatus)
    {
        var deal = await GetParticipantDealAsync(dealId, requestingOrgId);

        if (!AllowedTransitions[deal.Status].Contains(newStatus))
            throw new InvalidStatusTransitionException(dealId, deal.Status, newStatus);

        deal.Status = newStatus;
        deal.UpdatedAt = DateTime.UtcNow;
        await _dealRepository.UpdateAsync(deal);

        return ToResponse(deal);
    }

    private async Task<Deal> GetParticipantDealAsync(int dealId, int requestingOrgId)
    {
        var deal = await _dealRepository.GetByIdAsync(dealId)
            ?? throw new DealNotFoundException(dealId);

        if (deal.InitiatorOrganizationId != requestingOrgId
            && deal.CounterpartyOrganizationId != requestingOrgId)
            throw new NotDealParticipantException(requestingOrgId, dealId);

        return deal;
    }

    private static DealResponse ToResponse(Deal deal) => new()
    {
        Id = deal.Id,
        Title = deal.Title,
        Status = deal.Status.ToString(),
        InitiatorOrganizationName = deal.InitiatorOrganization.Name,
        CounterpartyOrganizationName = deal.CounterpartyOrganization.Name,
        CreatedAt = deal.CreatedAt,
        UpdatedAt = deal.UpdatedAt
    };
}
