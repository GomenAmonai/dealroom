namespace DealRoom.Core.DTOs;

public record OrganizationResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
