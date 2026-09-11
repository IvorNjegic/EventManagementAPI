namespace EventManagement.Core.Models;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 1:N — komentar pripada jednom korisniku
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // 1:N — komentar pripada jednom događaju
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
}