using EventManagement.Core.Models;

namespace EventManagement.Core.Interfaces;

// Sučelje repozitorija događaja - dohvat s povezanim podacima
public interface IEventRepository : IRepository<Event>
{
    Task<IEnumerable<Event>> GetAllWithDetailsAsync();
    Task<Event?> GetByIdWithDetailsAsync(int id);
}
