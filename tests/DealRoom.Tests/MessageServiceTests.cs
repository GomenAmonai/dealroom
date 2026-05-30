using DealRoom.Core.Entities;
using DealRoom.Core.Exceptions;
using DealRoom.Infrastructure.Services;
using DealRoom.Tests.Fakes;

namespace DealRoom.Tests;

public class MessageServiceTests
{
    private const int OrgA = 1;
    private const int OrgB = 2;
    private const int Outsider = 3;
    private const int AliceId = 10;

    private static (MessageService Service, int DealId) Build()
    {
        var orgs = new Dictionary<int, Organization>
        {
            [OrgA] = new() { Id = OrgA, Name = "Org A" },
            [OrgB] = new() { Id = OrgB, Name = "Org B" },
            [Outsider] = new() { Id = Outsider, Name = "Outsider" },
        };
        var deals = new FakeDealRepository(orgs);
        var deal = deals.Seed(new Deal
        {
            Title = "Deal",
            Status = DealStatus.Active,
            InitiatorOrganizationId = OrgA,
            CounterpartyOrganizationId = OrgB,
            CreatedAt = DateTime.UtcNow
        });
        var users = new FakeUserRepository(new Dictionary<int, User>
        {
            [AliceId] = new() { Id = AliceId, Name = "Alice", OrganizationId = OrgA }
        });
        var service = new MessageService(new FakeMessageRepository(), deals, users);
        return (service, deal.Id);
    }

    [Fact]
    public async Task SendAsync_ByParticipant_DenormalizesSenderName()
    {
        var (service, dealId) = Build();

        var result = await service.SendAsync(dealId, AliceId, OrgA, "hello");

        Assert.Equal("Alice", result.SenderName);
    }

    [Fact]
    public async Task SendAsync_TrimsContent()
    {
        var (service, dealId) = Build();

        var result = await service.SendAsync(dealId, AliceId, OrgA, "  hi  ");

        Assert.Equal("hi", result.Content);
    }

    [Fact]
    public async Task SendAsync_WithBlankContent_ThrowsEmptyMessageContent()
    {
        var (service, dealId) = Build();

        await Assert.ThrowsAsync<EmptyMessageContentException>(
            () => service.SendAsync(dealId, AliceId, OrgA, "   "));
    }

    [Fact]
    public async Task SendAsync_ByNonParticipant_ThrowsNotDealParticipant()
    {
        var (service, dealId) = Build();

        await Assert.ThrowsAsync<NotDealParticipantException>(
            () => service.SendAsync(dealId, AliceId, Outsider, "hi"));
    }

    [Fact]
    public async Task GetForDealAsync_ReturnsSentMessages()
    {
        var (service, dealId) = Build();
        await service.SendAsync(dealId, AliceId, OrgA, "one");
        await service.SendAsync(dealId, AliceId, OrgA, "two");

        var result = await service.GetForDealAsync(dealId, OrgB);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetForDealAsync_ByNonParticipant_ThrowsNotDealParticipant()
    {
        var (service, dealId) = Build();

        await Assert.ThrowsAsync<NotDealParticipantException>(
            () => service.GetForDealAsync(dealId, Outsider));
    }
}
