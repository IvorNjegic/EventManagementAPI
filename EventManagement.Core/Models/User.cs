namespace EventManagement.Core.Models;

// Model korisnika sustava
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigacijska svojstva
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}