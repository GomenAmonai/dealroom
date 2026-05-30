using DealRoom.Core.Entities;

namespace DealRoom.Core.DTOs;

public record UpdateDealStatusRequest
{
    public DealStatus Status { get; init; }
}