namespace DealRoom.Core.Exceptions;

public class DealNotFoundException : DomainException
{
    public override int StatusCode => 404;

    public DealNotFoundException(int dealId) :
        base($"Deal {dealId} not found") { }
}