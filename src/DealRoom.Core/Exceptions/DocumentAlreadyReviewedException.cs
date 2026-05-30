namespace DealRoom.Core.Exceptions;

public class DocumentAlreadyReviewedException : DomainException
{
    public override int StatusCode => 409;

    public DocumentAlreadyReviewedException(int documentId)
        : base($"Document {documentId} has already been reviewed") { }
}
