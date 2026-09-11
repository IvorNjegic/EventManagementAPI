using EventManagement.Core.Models;

namespace EventManagement.Core.Interfaces;

// Sučelje servisa za rad s događajima
public interface IEventService
{
    Task<IEnumerable<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(int id);
    Task<Event> CreateAsync(Event entity);
    Task UpdateAsync(Event entity);
    Task DeleteAsync(int id);
}