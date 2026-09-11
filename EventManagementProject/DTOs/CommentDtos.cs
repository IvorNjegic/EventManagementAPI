using System.ComponentModel.DataAnnotations;

namespace EventManagementProject.DTOs;

// DTO za kreiranje komentara
public class CommentCreateDto
{
    [Required]
    [StringLength(1000)]
    public string Text { get; set; } = string.Empty;

    [Required]
    public int EventId { get; set; }
}

// DTO za prikaz komentara
public class CommentResponseDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Username { get; set; } = string.Empty;
    public string EventTitle { get; set; } = string.Empty;
}
