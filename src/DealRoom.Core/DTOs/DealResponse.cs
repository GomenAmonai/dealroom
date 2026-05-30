namespace DealRoom.Core.DTOs;

public record DealResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string InitiatorOrganizationName {get; init;} = string.Empty;
    public string CounterpartyOrganizationName {get; init;} = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}