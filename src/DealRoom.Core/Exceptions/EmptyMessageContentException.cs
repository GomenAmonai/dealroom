namespace DealRoom.Core.Exceptions;

public class EmptyMessageContentException : DomainException
{
    public override int StatusCode => 400;

    public EmptyMessageContentException()
        : base("Message content must not be empty") { }
}
