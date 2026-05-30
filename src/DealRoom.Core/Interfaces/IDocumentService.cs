using DealRoom.Core.DTOs;

namespace DealRoom.Core.Interfaces;

public interface IDocumentService
{
    Task<DocumentResponse> UploadAsync(
        int dealId, int requestingOrgId, string fileName, string contentType, long size, Stream content);
    Task<IReadOnlyList<DocumentResponse>> GetForDealAsync(int dealId, int requestingOrgId);
    Task<DocumentContent> DownloadAsync(int documentId, int requestingOrgId);
    Task<DocumentResponse> ApproveAsync(int documentId, int requestingOrgId);
    Task<DocumentResponse> RejectAsync(int documentId, int requestingOrgId);
}
