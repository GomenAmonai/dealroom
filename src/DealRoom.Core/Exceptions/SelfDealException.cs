using DealRoom.Core.Entities;

namespace DealRoom.Core.Exceptions;

public class SelfDealException : DomainException
{
    public override int StatusCode => 400;

    public SelfDealException(int orgId) : 
        base($"Organization {orgId} attempted to create a deal with itself") {}
}