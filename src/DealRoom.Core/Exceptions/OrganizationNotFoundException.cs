namespace DealRoom.Core.Exceptions;

public class OrganizationNotFoundException : DomainException
{
    public override int StatusCode => 404;

    public OrganizationNotFoundException(int organizationId)
        : base($"Organization {organizationId} not found") { }
}
