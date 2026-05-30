namespace DealRoom.Core.Entities;

public class Message
{
    public int Id { get; set; }
    public int DealId { get; set; }
    public Deal Deal { get; set; } = null!;
    public int SenderUserId { get; set; }
    public int SenderOrganizationId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
