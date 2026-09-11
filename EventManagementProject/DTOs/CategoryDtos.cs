using System.ComponentModel.DataAnnotations;

namespace EventManagementProject.DTOs;

// DTO za kreiranje i ažuriranje kategorije
public class CategoryCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}

// DTO za prikaz kategorije
public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EventCount { get; set; }
}
