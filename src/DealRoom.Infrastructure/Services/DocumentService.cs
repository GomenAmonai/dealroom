using DealRoom.Core.DTOs;
using DealRoom.Core.Entities;
using DealRoom.Core.Exceptions;
using DealRoom.Core.Interfaces;

namespace DealRoom.Infrastructure.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documents;
    private readonly IDealRepository _deals;
    private readonly IFileStorage _storage;

    public DocumentService(IDocumentRepository documents, IDealRepository deals, IFileStorage storage)
    {
        _documents = documents;
        _deals = deals;
        _storage = storage;
    }

    public async Task<DocumentResponse> UploadAsync(
        int dealId, int requestingOrgId, string fileName, string contentType, long size, Stream content)
    {
        await EnsureParticipantAsync(dealId, requestingOrgId);

        var storageKey = $"deals/{dealId}/{Guid.NewGuid():N}";
        await _storage.UploadAsync(storageKey, content, size, contentType);

        var document = await _documents.AddAsync(new Document
        {
            DealId = dealId,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = size,
            StorageKey = storageKey,
            Status = DocumentStatus.Pending,
            UploadedByOrganizationId = requestingOrgId,
            UploadedAt = DateTime.UtcNow
        });

        return ToResponse(document);
    }

    public async Task<IReadOnlyList<DocumentResponse>> GetForDealAsync(int dealId, int requestingOrgId)
    {
        await EnsureParticipantAsync(dealId, requestingOrgId);
        var documents = await _documents.GetByDealAsync(dealId);
        return documents.Select(ToResponse).ToList();
    }

    public async Task<DocumentContent> DownloadAsync(int documentId, int requestingOrgId)
    {
        var document = await GetParticipantDocumentAsync(documentId, requestingOrgId);
        var stream = await _storage.DownloadAsync(document.StorageKey);
        return new DocumentContent(stream, document.FileName, document.ContentType);
    }

    public Task<DocumentResponse> ApproveAsync(int documentId, int requestingOrgId)
        => ReviewAsync(documentId, requestingOrgId, DocumentStatus.Approved);

    public Task<DocumentResponse> RejectAsync(int documentId, int requestingOrgId)
        => ReviewAsync(documentId, requestingOrgId, DocumentStatus.Rejected);

    private async Task<DocumentResponse> ReviewAsync(int documentId, int requestingOrgId, DocumentStatus outcome)
    {
        var document = await GetParticipantDocumentAsync(documentId, requestingOrgId);

        if (document.UploadedByOrganizationId == requestingOrgId)
            throw new CannotReviewOwnDocumentException(documentId);

        if (document.Status != DocumentStatus.Pending)
            throw new DocumentAlreadyReviewedException(documentId);

        document.Status = outcome;
        document.ReviewedAt = DateTime.UtcNow;
        await _documents.UpdateAsync(document);

        return ToResponse(document);
    }

    private async Task<Document> GetParticipantDocumentAsync(int documentId, int requestingOrgId)
    {
        var document = await _documents.GetByIdAsync(documentId)
            ?? throw new DocumentNotFoundException(documentId);

        await EnsureParticipantAsync(document.DealId, requestingOrgId);
        return document;
    }

    private async Task EnsureParticipantAsync(int dealId, int requestingOrgId)
    {
        var deal = await _deals.GetByIdAsync(dealId) ?? throw new DealNotFoundException(dealId);

        if (deal.InitiatorOrganizationId != requestingOrgId
            && deal.CounterpartyOrganizationId != requestingOrgId)
            throw new NotDealParticipantException(requestingOrgId, dealId);
    }

    private static DocumentResponse ToResponse(Document document) => new()
    {
        Id = document.Id,
        DealId = document.DealId,
        FileName = document.FileName,
        ContentType = document.ContentType,
        SizeBytes = document.SizeBytes,
        Status = document.Status.ToString(),
        UploadedByOrganizationId = document.UploadedByOrganizationId,
        UploadedAt = document.UploadedAt,
        ReviewedAt = document.ReviewedAt
    };
}
