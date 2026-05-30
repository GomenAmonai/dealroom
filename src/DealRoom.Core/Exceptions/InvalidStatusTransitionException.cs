using DealRoom.Core.Entities;

namespace DealRoom.Core.Exceptions;

public class InvalidStatusTransitionException : DomainException
{
    public override int StatusCode => 422;

    public InvalidStatusTransitionException(int dealId, DealStatus from, DealStatus to)
        : base($"Deal {dealId} cannot transition from {from} to {to}") { }
}
