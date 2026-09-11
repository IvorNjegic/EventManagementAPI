using EventManagement.Core.Interfaces;
using EventManagement.Core.Models;

namespace EventManagement.Core.Services;

public class EventService : IEventService
{
    private readonly IRepository<Event> _repository;

    public EventService(IRepository<Event> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
        => await _repository.GetAllAsync();

    public async Task<Event?> GetByIdAsync(int id)
        => await _repository.GetByIdAsync(id);

    public async Task<Event> CreateAsync(Event entity)
    {
        await _repository.AddAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(Event entity)
        => await _repository.UpdateAsync(entity);

    public async Task DeleteAsync(int id)
        => await _repository.DeleteAsync(id);
}