using EventManagement.Core.Models;

namespace EventManagement.Core.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<IEnumerable<Event>> GetAllWithDetailsAsync();
    Task<Event?> GetByIdWithDetailsAsync(int id);
}
