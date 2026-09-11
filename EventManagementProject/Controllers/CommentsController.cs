using System.Security.Claims;
using EventManagement.Core.Models;
using EventManagement.Infrastructure.Data;
using EventManagementProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagementProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(AppDbContext context, ILogger<CommentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("event/{eventId}")]
    public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetByEvent(int eventId)
    {
        _logger.LogInformation("Dohvacanje komentara za dogadjaj: {EventId}", eventId);
        var comments = await _context.Comments
            .Where(c => c.EventId == eventId)
            .Include(c => c.User)
            .Include(c => c.Event)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        var response = comments.Select(c => new CommentResponseDto
        {
            Id = c.Id,
            Text = c.Text,
            CreatedAt = c.CreatedAt,
            Username = c.User.Username,
            EventTitle = c.Event.Title
        });

        return Ok(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CommentResponseDto>> Create(CommentCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        _logger.LogInformation("Kreiranje komentara od korisnika {UserId} na dogadjaj {EventId}", userId, dto.EventId);

        var eventExists = await _context.Events.AnyAsync(e => e.Id == dto.EventId);
        if (!eventExists)
            return NotFound(new { Message = "Dogadjaj nije pronadjen." });

        var comment = new Comment
        {
            Text = dto.Text,
            EventId = dto.EventId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        var created = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Event)
            .FirstAsync(c => c.Id == comment.Id);

        _logger.LogInformation("Komentar kreiran: {CommentId}", comment.Id);

        return CreatedAtAction(nameof(GetByEvent), new { eventId = dto.EventId }, new CommentResponseDto
        {
            Id = created.Id,
            Text = created.Text,
            CreatedAt = created.CreatedAt,
            Username = created.User.Username,
            EventTitle = created.Event.Title
        });
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var comment = await _context.Comments.FindAsync(id);

        if (comment == null)
            return NotFound(new { Message = "Komentar nije pronadjen." });

        if (comment.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Komentar obrisan: {CommentId}", id);
        return NoContent();
    }
}
