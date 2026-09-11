using System.ComponentModel.DataAnnotations;

namespace EventManagementProject.DTOs;

// DTO za kreiranje i ažuriranje lokacije
public class VenueCreateDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Range(1, 1000000)]
    public int Capacity { get; set; }
}

// DTO za prikaz lokacije
public class VenueResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int EventCount { get; set; }
}
