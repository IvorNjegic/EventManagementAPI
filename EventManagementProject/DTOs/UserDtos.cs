namespace EventManagementProject.DTOs;

// DTO za prikaz korisnika s pripadajućim ulogama
public class UserResponseDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<string> Roles { get; set; } = new();
}
