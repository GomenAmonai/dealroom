namespace DealRoom.Core.Entities;

public class Deal {
    public int Id { get; set;}
    public string Title { get; set;} = string.Empty;
    public DealStatus Status {get; set;}
    public int InitiatorOrganizationId { get; set;}
    public Organization InitiatorOrganization { get; set;} = null!;
    public int CounterpartyOrganizationId { get; set; }
    public Organization CounterpartyOrganization { get; set;} = null!;
    public DateTime CreatedAt { get; set;}
    public DateTime? UpdatedAt { get; set;}
}