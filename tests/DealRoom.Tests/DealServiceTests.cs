using DealRoom.Core.DTOs;
using DealRoom.Core.Entities;
using DealRoom.Core.Exceptions;
using DealRoom.Infrastructure.Services;
using DealRoom.Tests.Fakes;

namespace DealRoom.Tests;

public class DealServiceTests
{
    private const int OrgA = 1;
    private const int OrgB = 2;
    private const int OrgC = 3;

    private static (DealService Service, FakeDealRepository Deals) Build()
    {
        var orgs = new Dictionary<int, Organization>
        {
            [OrgA] = new() { Id = OrgA, Name = "Org A" },
            [OrgB] = new() { Id = OrgB, Name = "Org B" },
            [OrgC] = new() { Id = OrgC, Name = "Org C" },
        };
        var dealRepo = new FakeDealRepository(orgs);
        var service = new DealService(dealRepo, new FakeOrganizationRepository(orgs));
        return (service, dealRepo);
    }

    [Fact]
    public async Task CreateAsync_WhenCounterpartyIsInitiator_ThrowsSelfDeal()
    {
        var (service, _) = Build();
        var request = new CreateDealRequest { Title = "X", CounterpartyOrganizationId = OrgA };

        await Assert.ThrowsAsync<SelfDealException>(() => service.CreateAsync(OrgA, request));
    }

    [Fact]
    public async Task CreateAsync_WhenCounterpartyDoesNotExist_ThrowsOrganizationNotFound()
    {
        var (service, _) = Build();
        var request = new CreateDealRequest { Title = "X", CounterpartyOrganizationId = 999 };

        await Assert.ThrowsAsync<OrganizationNotFoundException>(() => service.CreateAsync(OrgA, request));
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_StartsInDraftStatus()
    {
        var (service, _) = Build();
        var request = new CreateDealRequest { Title = "Supply contract", CounterpartyOrganizationId = OrgB };

        var result = await service.CreateAsync(OrgA, request);

        Assert.Equal(nameof(DealStatus.Draft), result.Status);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ResolvesCounterpartyName()
    {
        var (service, _) = Build();
        var request = new CreateDealRequest { Title = "Supply contract", CounterpartyOrganizationId = OrgB };

        var result = await service.CreateAsync(OrgA, request);

        Assert.Equal("Org B", result.CounterpartyOrganizationName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenDealMissing_ThrowsDealNotFound()
    {
        var (service, _) = Build();

        await Assert.ThrowsAsync<DealNotFoundException>(() => service.GetByIdAsync(123, OrgA));
    }

    [Fact]
    public async Task GetByIdAsync_WhenRequesterIsNotParticipant_ThrowsNotDealParticipant()
    {
        var (service, deals) = Build();
        var deal = deals.Seed(NewDeal(DealStatus.Active));

        await Assert.ThrowsAsync<NotDealParticipantException>(() => service.GetByIdAsync(deal.Id, OrgC));
    }

    [Fact]
    public async Task GetForOrganizationAsync_ReturnsOnlyDealsWhereOrgParticipates()
    {
        var (service, deals) = Build();
        deals.Seed(NewDeal(DealStatus.Active));
        deals.Seed(new Deal
        {
            Title = "Other",
            Status = DealStatus.Active,
            InitiatorOrganizationId = OrgB,
            CounterpartyOrganizationId = OrgC,
            CreatedAt = DateTime.UtcNow
        });

        var result = await service.GetForOrganizationAsync(OrgA);

        Assert.Single(result);
    }

    [Fact]
    public async Task UpdateStatusAsync_FromDraftToActive_ReturnsActive()
    {
        var (service, deals) = Build();
        var deal = deals.Seed(NewDeal(DealStatus.Draft));

        var result = await service.UpdateStatusAsync(deal.Id, OrgB, DealStatus.Active);

        Assert.Equal(nameof(DealStatus.Active), result.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_FromActiveToClosed_ReturnsClosed()
    {
        var (service, deals) = Build();
        var deal = deals.Seed(NewDeal(DealStatus.Active));

        var result = await service.UpdateStatusAsync(deal.Id, OrgA, DealStatus.Closed);

        Assert.Equal(nameof(DealStatus.Closed), result.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_FromClosedToActive_ThrowsInvalidTransition()
    {
        var (service, deals) = Build();
        var deal = deals.Seed(NewDeal(DealStatus.Closed));

        await Assert.ThrowsAsync<InvalidStatusTransitionException>(
            () => service.UpdateStatusAsync(deal.Id, OrgA, DealStatus.Active));
    }

    [Fact]
    public async Task UpdateStatusAsync_ByNonParticipant_ThrowsNotDealParticipant()
    {
        var (service, deals) = Build();
        var deal = deals.Seed(NewDeal(DealStatus.Draft));

        await Assert.ThrowsAsync<NotDealParticipantException>(
            () => service.UpdateStatusAsync(deal.Id, OrgC, DealStatus.Active));
    }

    private static Deal NewDeal(DealStatus status) => new()
    {
        Title = "Deal",
        Status = status,
        InitiatorOrganizationId = OrgA,
        CounterpartyOrganizationId = OrgB,
        CreatedAt = DateTime.UtcNow
    };
}
