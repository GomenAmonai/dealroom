namespace DealRoom.Core.Exceptions;

public class CannotReviewOwnDocumentException : DomainException
{
    public override int StatusCode => 403;

    public CannotReviewOwnDocumentException(int documentId)
        : base($"Document {documentId} cannot be reviewed by the organization that uploaded it") { }
}
