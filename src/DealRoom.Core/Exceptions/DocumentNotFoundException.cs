namespace DealRoom.Core.Exceptions;

public class DocumentNotFoundException : DomainException
{
    public override int StatusCode => 404;

    public DocumentNotFoundException(int documentId)
        : base($"Document {documentId} not found") { }
}
