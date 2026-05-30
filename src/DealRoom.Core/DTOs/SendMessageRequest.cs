namespace DealRoom.Core.DTOs;

public record SendMessageRequest
{
    public string Content { get; init; } = string.Empty;
}
