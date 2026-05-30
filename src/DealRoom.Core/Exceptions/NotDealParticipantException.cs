namespace DealRoom.Core.Exceptions;

public class NotDealParticipantException : DomainException
{
    public override int StatusCode => 403;

    public NotDealParticipantException(int orgId, int dealId) :
        base($"Organization {orgId} is not a participant of deal {dealId}") { }
}