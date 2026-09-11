using System.Security.Claims;
using EventManagement.Core.Models;
using EventManagement.Infrastructure.Data;
using EventManagementProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagementProject.Controllers;

// Kontroler za prijave korisnika na događaje
[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<RegistrationsController> _logger;

    public RegistrationsController(AppDbContext context, ILogger<RegistrationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<RegistrationResponseDto>>> GetAll()
    {
        _logger.LogInformation("Dohvacanje svih registracija");
        var registrations = await _context.Registrations
            .Include(r => r.User)
            .Include(r => r.Event)
            .ToListAsync();

        var response = registrations.Select(r => new RegistrationResponseDto
        {
            Id = r.Id,
            RegisteredAt = r.RegisteredAt,
            Status = r.Status,
            Username = r.User.Username,
            EventTitle = r.Event.Title
        });

        return Ok(response);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<RegistrationResponseDto>>> GetMyRegistrations()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        _logger.LogInformation("Dohvacanje registracija korisnika: {UserId}", userId);

        var registrations = await _context.Registrations
            .Where(r => r.UserId == userId)
            .Include(r => r.User)
            .Include(r => r.Event)
            .ToListAsync();

        var response = registrations.Select(r => new RegistrationResponseDto
        {
            Id = r.Id,
            RegisteredAt = r.RegisteredAt,
            Status = r.Status,
            Username = r.User.Username,
            EventTitle = r.Event.Title
        });

        return Ok(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RegistrationResponseDto>> Create(RegistrationCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        _logger.LogInformation("Registracija korisnika {UserId} na dogadjaj {EventId}", userId, dto.EventId);

        var eventExists = await _context.Events.AnyAsync(e => e.Id == dto.EventId);
        if (!eventExists)
            return NotFound(new { Message = "Dogadjaj nije pronadjen." });

        var alreadyRegistered = await _context.Registrations
            .AnyAsync(r => r.UserId == userId && r.EventId == dto.EventId);
        if (alreadyRegistered)
            return BadRequest(new { Message = "Vec ste registrirani na ovaj dogadjaj." });

        var evt = await _context.Events.FindAsync(dto.EventId);
        var currentCount = await _context.Registrations.CountAsync(r => r.EventId == dto.EventId);
        if (currentCount >= evt!.MaxParticipants)
            return BadRequest(new { Message = "Dogadjaj je popunjen." });

        var registration = new Registration
        {
            UserId = userId,
            EventId = dto.EventId,
            RegisteredAt = DateTime.UtcNow,
            Status = "Confirmed"
        };

        _context.Registrations.Add(registration);
        await _context.SaveChangesAsync();

        var created = await _context.Registrations
            .Include(r => r.User)
            .Include(r => r.Event)
            .FirstAsync(r => r.Id == registration.Id);

        _logger.LogInformation("Registracija uspjesna: {RegistrationId}", registration.Id);

        return CreatedAtAction(nameof(GetMyRegistrations), new RegistrationResponseDto
        {
            Id = created.Id,
            RegisteredAt = created.RegisteredAt,
            Status = created.Status,
            Username = created.User.Username,
            EventTitle = created.Event.Title
        });
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var registration = await _context.Registrations.FindAsync(id);

        if (registration == null)
            return NotFound(new { Message = "Registracija nije pronadjena." });

        if (registration.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        _context.Registrations.Remove(registration);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Registracija otkazana: {RegistrationId}", id);
        return NoContent();
    }
}
