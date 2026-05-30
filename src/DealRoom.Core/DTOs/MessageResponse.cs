namespace DealRoom.Core.DTOs;

public record MessageResponse
{
    public int Id { get; init; }
    public int DealId { get; init; }
    public int SenderUserId { get; init; }
    public int SenderOrganizationId { get; init; }
    public string SenderName { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime SentAt { get; init; }
}
