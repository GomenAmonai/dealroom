namespace DealRoom.Core.DTOs;

public record CreateDealRequest
{
    public string Title { get; init; } = string.Empty;
    public int CounterpartyOrganizationId { get; init;}
}