namespace DealRoom.Core.Exceptions;

public class InvalidCredentialsException : DomainException
{
    public override int StatusCode => 401;

    public InvalidCredentialsException()
        : base("Invalid email or password.") { }
}
