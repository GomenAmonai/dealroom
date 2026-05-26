namespace DealRoom.Core.Exceptions;

public class InvalidRefreshTokenException : DomainException
{
    public override int StatusCode => 401;

    public InvalidRefreshTokenException()
        : base("Refresh token is invalid, expired or revoked.") { }
}
