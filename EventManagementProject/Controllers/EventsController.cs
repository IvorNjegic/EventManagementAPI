using System.Security.Claims;
using EventManagement.Core.Interfaces;
using EventManagement.Core.Models;
using EventManagementProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementProject.Controllers;

// Kontroler za upravljanje događajima (CRUD)
[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IEventRepository eventRepository, ILogger<EventsController> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetAll()
    {
        _logger.LogInformation("Dohvacanje svih dogadjaja");
        var events = await _eventRepository.GetAllWithDetailsAsync();

        var response = events.Select(e => new EventResponseDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            MaxParticipants = e.MaxParticipants,
            CategoryName = e.Category.Name,
            VenueName = e.Venue.Name,
            OrganizerUsername = e.Organizer.Username,
            RegistrationCount = e.Registrations.Count
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EventResponseDto>> GetById(int id)
    {
        _logger.LogInformation("Dohvacanje dogadjaja ID: {EventId}", id);
        var e = await _eventRepository.GetByIdWithDetailsAsync(id);

        if (e == null)
        {
            _logger.LogWarning("Dogadjaj nije pronadjen: {EventId}", id);
            return NotFound(new { Message = "Dogadjaj nije pronadjen." });
        }

        return Ok(new EventResponseDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            MaxParticipants = e.MaxParticipants,
            CategoryName = e.Category.Name,
            VenueName = e.Venue.Name,
            OrganizerUsername = e.Organizer.Username,
            RegistrationCount = e.Registrations.Count
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EventResponseDto>> Create(EventCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        _logger.LogInformation("Kreiranje dogadjaja: {Title} od korisnika {UserId}", dto.Title, userId);

        var newEvent = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            MaxParticipants = dto.MaxParticipants,
            CategoryId = dto.CategoryId,
            VenueId = dto.VenueId,
            OrganizerId = userId
        };

        await _eventRepository.AddAsync(newEvent);
        var created = await _eventRepository.GetByIdWithDetailsAsync(newEvent.Id);

        _logger.LogInformation("Dogadjaj kreiran: {EventId}", newEvent.Id);

        return CreatedAtAction(nameof(GetById), new { id = newEvent.Id }, new EventResponseDto
        {
            Id = created!.Id,
            Title = created.Title,
            Description = created.Description,
            StartDate = created.StartDate,
            EndDate = created.EndDate,
            MaxParticipants = created.MaxParticipants,
            CategoryName = created.Category.Name,
            VenueName = created.Venue.Name,
            OrganizerUsername = created.Organizer.Username,
            RegistrationCount = created.Registrations.Count
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, EventUpdateDto dto)
    {
        _logger.LogInformation("Azuriranje dogadjaja: {EventId}", id);
        var existingEvent = await _eventRepository.GetByIdAsync(id);

        if (existingEvent == null)
        {
            _logger.LogWarning("Dogadjaj za azuriranje nije pronadjen: {EventId}", id);
            return NotFound(new { Message = "Dogadjaj nije pronadjen." });
        }

        existingEvent.Title = dto.Title;
        existingEvent.Description = dto.Description;
        existingEvent.StartDate = dto.StartDate;
        existingEvent.EndDate = dto.EndDate;
        existingEvent.MaxParticipants = dto.MaxParticipants;
        existingEvent.CategoryId = dto.CategoryId;
        existingEvent.VenueId = dto.VenueId;

        await _eventRepository.UpdateAsync(existingEvent);

        _logger.LogInformation("Dogadjaj azuriran: {EventId}", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Brisanje dogadjaja: {EventId}", id);
        var existingEvent = await _eventRepository.GetByIdAsync(id);

        if (existingEvent == null)
        {
            _logger.LogWarning("Dogadjaj za brisanje nije pronadjen: {EventId}", id);
            return NotFound(new { Message = "Dogadjaj nije pronadjen." });
        }

        await _eventRepository.DeleteAsync(id);
        _logger.LogInformation("Dogadjaj obrisan: {EventId}", id);
        return NoContent();
    }
}
