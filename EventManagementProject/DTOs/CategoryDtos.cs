using System.ComponentModel.DataAnnotations;

namespace EventManagementProject.DTOs;

public class CategoryCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EventCount { get; set; }
}
