namespace EventManagement.Core.Models;

// Vezna tablica za M:N relaciju između korisnika i uloga
public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}