using EventManagement.Core.Interfaces;
using EventManagement.Core.Models;
using EventManagementProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementProject.Controllers;

// Kontroler za upravljanje kategorijama događaja
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IRepository<EventCategory> _categoryRepository;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IRepository<EventCategory> categoryRepository, ILogger<CategoriesController> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAll()
    {
        _logger.LogInformation("Dohvacanje svih kategorija");
        var categories = await _categoryRepository.GetAllAsync();

        var response = categories.Select(c => new CategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> GetById(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return NotFound(new { Message = "Kategorija nije pronadjena." });

        return Ok(new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResponseDto>> Create(CategoryCreateDto dto)
    {
        _logger.LogInformation("Kreiranje kategorije: {Name}", dto.Name);
        var category = new EventCategory
        {
            Name = dto.Name,
            Description = dto.Description
        };

        await _categoryRepository.AddAsync(category);

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CategoryCreateDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return NotFound(new { Message = "Kategorija nije pronadjena." });

        category.Name = dto.Name;
        category.Description = dto.Description;

        await _categoryRepository.UpdateAsync(category);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return NotFound(new { Message = "Kategorija nije pronadjena." });

        await _categoryRepository.DeleteAsync(id);
        return NoContent();
    }
}
