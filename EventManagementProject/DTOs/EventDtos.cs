using System.ComponentModel.DataAnnotations;

namespace EventManagementProject.DTOs;

public class EventCreateDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Range(1, 100000)]
    public int MaxParticipants { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int VenueId { get; set; }
}

public class EventUpdateDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Range(1, 100000)]
    public int MaxParticipants { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int VenueId { get; set; }
}

public class EventResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxParticipants { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string VenueName { get; set; } = string.Empty;
    public string OrganizerUsername { get; set; } = string.Empty;
    public int RegistrationCount { get; set; }
}
