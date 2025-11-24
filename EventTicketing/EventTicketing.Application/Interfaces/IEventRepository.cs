using EventTicketing.Domain.Entities;

namespace EventTicketing.Application.Interfaces;

public interface IEventRepository
{
  // Read operations
  Task<Event?> GetByIdAsync(int id);
  Task<IEnumerable<Event>> GetAllAsync();
  Task<IEnumerable<Event>> GetUpcomingEventsAsync();

  // Write operations
  Task<Event> CreateAsync(Event @event);
  Task UpdateAsync(Event @event);
  Task DeleteAsync(int id);

  // Business logic queries
  Task<bool> ExistsAsync(int id);
}
