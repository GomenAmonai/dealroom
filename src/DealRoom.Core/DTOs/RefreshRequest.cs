namespace DealRoom.Core.DTOs;

public record RefreshRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
