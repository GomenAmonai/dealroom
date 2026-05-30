using DealRoom.Core.DTOs;
using DealRoom.Core.Entities;
using DealRoom.Core.Exceptions;
using DealRoom.Infrastructure.Services;
using DealRoom.Tests.Fakes;

namespace DealRoom.Tests;

public class DocumentServiceTests
{
    private const int Uploader = 1;
    private const int Counterparty = 2;
    private const int Outsider = 3;

    private static (DocumentService Service, int DealId) Build()
    {
        var orgs = new Dictionary<int, Organization>
        {
            [Uploader] = new() { Id = Uploader, Name = "Uploader" },
            [Counterparty] = new() { Id = Counterparty, Name = "Counterparty" },
            [Outsider] = new() { Id = Outsider, Name = "Outsider" },
        };
        var deals = new FakeDealRepository(orgs);
        var deal = deals.Seed(new Deal
        {
            Title = "Deal",
            Status = DealStatus.Active,
            InitiatorOrganizationId = Uploader,
            CounterpartyOrganizationId = Counterparty,
            CreatedAt = DateTime.UtcNow
        });
        var service = new DocumentService(new FakeDocumentRepository(), deals, new FakeFileStorage());
        return (service, deal.Id);
    }

    private static Task<DocumentResponse> Upload(DocumentService service, int dealId, int orgId, byte[] content)
        => service.UploadAsync(dealId, orgId, "contract.pdf", "application/pdf", content.Length, new MemoryStream(content));

    [Fact]
    public async Task UploadAsync_ByParticipant_StartsPending()
    {
        var (service, dealId) = Build();

        var result = await Upload(service, dealId, Uploader, new byte[] { 1, 2, 3 });

        Assert.Equal(nameof(DocumentStatus.Pending), result.Status);
    }

    [Fact]
    public async Task UploadAsync_ByNonParticipant_ThrowsNotDealParticipant()
    {
        var (service, dealId) = Build();

        await Assert.ThrowsAsync<NotDealParticipantException>(
            () => Upload(service, dealId, Outsider, new byte[] { 1 }));
    }

    [Fact]
    public async Task DownloadAsync_ReturnsStoredContent()
    {
        var (service, dealId) = Build();
        var content = new byte[] { 10, 20, 30, 40 };
        var document = await Upload(service, dealId, Uploader, content);

        var downloaded = await service.DownloadAsync(document.Id, Counterparty);
        using var buffer = new MemoryStream();
        await downloaded.Stream.CopyToAsync(buffer);

        Assert.Equal(content, buffer.ToArray());
    }

    [Fact]
    public async Task DownloadAsync_ByNonParticipant_ThrowsNotDealParticipant()
    {
        var (service, dealId) = Build();
        var document = await Upload(service, dealId, Uploader, new byte[] { 1 });

        await Assert.ThrowsAsync<NotDealParticipantException>(
            () => service.DownloadAsync(document.Id, Outsider));
    }

    [Fact]
    public async Task ApproveAsync_ByCounterparty_SetsApproved()
    {
        var (service, dealId) = Build();
        var document = await Upload(service, dealId, Uploader, new byte[] { 1 });

        var result = await service.ApproveAsync(document.Id, Counterparty);

        Assert.Equal(nameof(DocumentStatus.Approved), result.Status);
    }

    [Fact]
    public async Task ApproveAsync_ByUploader_ThrowsCannotReviewOwn()
    {
        var (service, dealId) = Build();
        var document = await Upload(service, dealId, Uploader, new byte[] { 1 });

        await Assert.ThrowsAsync<CannotReviewOwnDocumentException>(
            () => service.ApproveAsync(document.Id, Uploader));
    }

    [Fact]
    public async Task RejectAsync_ByCounterparty_SetsRejected()
    {
        var (service, dealId) = Build();
        var document = await Upload(service, dealId, Uploader, new byte[] { 1 });

        var result = await service.RejectAsync(document.Id, Counterparty);

        Assert.Equal(nameof(DocumentStatus.Rejected), result.Status);
    }

    [Fact]
    public async Task ReviewAsync_WhenAlreadyReviewed_ThrowsAlreadyReviewed()
    {
        var (service, dealId) = Build();
        var document = await Upload(service, dealId, Uploader, new byte[] { 1 });
        await service.ApproveAsync(document.Id, Counterparty);

        await Assert.ThrowsAsync<DocumentAlreadyReviewedException>(
            () => service.RejectAsync(document.Id, Counterparty));
    }

    [Fact]
    public async Task GetForDealAsync_ReturnsUploadedDocuments()
    {
        var (service, dealId) = Build();
        await Upload(service, dealId, Uploader, new byte[] { 1 });

        var result = await service.GetForDealAsync(dealId, Counterparty);

        Assert.Single(result);
    }
}
