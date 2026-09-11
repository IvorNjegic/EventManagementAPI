using EventManagement.Core.Interfaces;
using EventManagement.Core.Models;
using EventManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Infrastructure.Repositories;

// Repozitorij za događaje - dohvat s uključenim povezanim podacima (Include)
public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Event>> GetAllWithDetailsAsync()
    {
        return await _context.Events
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .ToListAsync();
    }

    public async Task<Event?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Events
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .Include(e => e.Comments)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
