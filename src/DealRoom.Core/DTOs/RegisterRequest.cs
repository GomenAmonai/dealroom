namespace DealRoom.Core.DTOs;

public record RegisterRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string OrganizationName { get; init; } = string.Empty;
}
