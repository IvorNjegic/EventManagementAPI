using EventManagement.Core.Interfaces;
using EventManagement.Core.Models;
using EventManagementProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementProject.Controllers;

// Kontroler za upravljanje lokacijama događaja
[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IRepository<Venue> _venueRepository;
    private readonly ILogger<VenuesController> _logger;

    public VenuesController(IRepository<Venue> venueRepository, ILogger<VenuesController> logger)
    {
        _venueRepository = venueRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VenueResponseDto>>> GetAll()
    {
        _logger.LogInformation("Dohvacanje svih lokacija");
        var venues = await _venueRepository.GetAllAsync();

        var response = venues.Select(v => new VenueResponseDto
        {
            Id = v.Id,
            Name = v.Name,
            Address = v.Address,
            Capacity = v.Capacity
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VenueResponseDto>> GetById(int id)
    {
        var venue = await _venueRepository.GetByIdAsync(id);
        if (venue == null)
            return NotFound(new { Message = "Lokacija nije pronadjena." });

        return Ok(new VenueResponseDto
        {
            Id = venue.Id,
            Name = venue.Name,
            Address = venue.Address,
            Capacity = venue.Capacity
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<VenueResponseDto>> Create(VenueCreateDto dto)
    {
        _logger.LogInformation("Kreiranje lokacije: {Name}", dto.Name);
        var venue = new Venue
        {
            Name = dto.Name,
            Address = dto.Address,
            Capacity = dto.Capacity
        };

        await _venueRepository.AddAsync(venue);

        return CreatedAtAction(nameof(GetById), new { id = venue.Id }, new VenueResponseDto
        {
            Id = venue.Id,
            Name = venue.Name,
            Address = venue.Address,
            Capacity = venue.Capacity
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, VenueCreateDto dto)
    {
        var venue = await _venueRepository.GetByIdAsync(id);
        if (venue == null)
            return NotFound(new { Message = "Lokacija nije pronadjena." });

        venue.Name = dto.Name;
        venue.Address = dto.Address;
        venue.Capacity = dto.Capacity;

        await _venueRepository.UpdateAsync(venue);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var venue = await _venueRepository.GetByIdAsync(id);
        if (venue == null)
            return NotFound(new { Message = "Lokacija nije pronadjena." });

        await _venueRepository.DeleteAsync(id);
        return NoContent();
    }
}
