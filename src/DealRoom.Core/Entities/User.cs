namespace DealRoom.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
}
