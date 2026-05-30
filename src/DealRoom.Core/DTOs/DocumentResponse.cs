namespace DealRoom.Core.DTOs;

public record DocumentResponse
{
    public int Id { get; init; }
    public int DealId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public string Status { get; init; } = string.Empty;
    public int UploadedByOrganizationId { get; init; }
    public DateTime UploadedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }
}
