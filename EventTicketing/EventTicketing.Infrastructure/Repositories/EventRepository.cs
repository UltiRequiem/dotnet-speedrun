using EventTicketing.Application.Interfaces;
using EventTicketing.Domain.Entities;
using EventTicketing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventTicketing.Infrastructure.Repositories;

public class EventRepository(AppDbContext context) : IEventRepository
{
  private readonly AppDbContext _context = context;

  public async Task<Event?> GetByIdAsync(int id)
  {
    return await _context.Events
        .Include(e => e.Tickets) // Eager load tickets
        .FirstOrDefaultAsync(e => e.Id == id);
  }

  public async Task<IEnumerable<Event>> GetAllAsync()
  {
    return await _context.Events
        .Include(e => e.Tickets)
        .OrderBy(e => e.EventDate)
        .ToListAsync();
  }

  public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
  {
    return await _context.Events
        .Where(e => e.EventDate > DateTime.UtcNow)
        .OrderBy(e => e.EventDate)
        .ToListAsync();
  }

  public async Task<Event> CreateAsync(Event @event)
  {
    @event.CreatedAt = DateTime.UtcNow;
    _context.Events.Add(@event);
    await _context.SaveChangesAsync();
    return @event;
  }

  public async Task UpdateAsync(Event @event)
  {
    @event.UpdatedAt = DateTime.UtcNow;
    _context.Events.Update(@event);
    await _context.SaveChangesAsync();
  }

  public async Task DeleteAsync(int id)
  {
    var @event = await _context.Events.FindAsync(id);
    if (@event != null)
    {
      _context.Events.Remove(@event);
      await _context.SaveChangesAsync();
    }
  }

  public async Task<bool> ExistsAsync(int id)
  {
    return await _context.Events.AnyAsync(e => e.Id == id);
  }
}