using EventTicketing.Domain.Entities;

namespace EventTicketing.Application.Interfaces;

public interface ITicketRepository
{
  // Read operations
  Task<Ticket?> GetByIdAsync(int id);
  Task<IEnumerable<Ticket>> GetAllAsync();
  Task<IEnumerable<Ticket>> GetByEventIdAsync(int eventId);
  Task<IEnumerable<Ticket>> GetByEmailAsync(string email);

  // Write operations
  Task<Ticket> CreateAsync(Ticket ticket);
  Task UpdateAsync(Ticket ticket);
  Task DeleteAsync(int id);

  // Business logic queries
  Task<int> GetTicketCountByEventIdAsync(int eventId);
}