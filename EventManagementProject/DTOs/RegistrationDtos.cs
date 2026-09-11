using System.ComponentModel.DataAnnotations;

namespace EventManagementProject.DTOs;

// DTO za prijavu korisnika na događaj
public class RegistrationCreateDto
{
    [Required]
    public int EventId { get; set; }
}

// DTO za prikaz prijave
public class RegistrationResponseDto
{
    public int Id { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string EventTitle { get; set; } = string.Empty;
}
