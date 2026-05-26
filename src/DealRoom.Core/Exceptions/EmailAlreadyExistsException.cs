namespace DealRoom.Core.Exceptions;

public class EmailAlreadyExistsException : DomainException
{
    public override int StatusCode => 409;

    public EmailAlreadyExistsException(string email)
        : base($"User with email '{email}' already exists.") { }
}
