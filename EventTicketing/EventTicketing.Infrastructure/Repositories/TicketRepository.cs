using EventTicketing.Application.Interfaces;
using EventTicketing.Domain.Entities;
using EventTicketing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventTicketing.Infrastructure.Repositories;

public class TicketRepository(AppDbContext context) : ITicketRepository
{
  private readonly AppDbContext _context = context;

  public async Task<Ticket?> GetByIdAsync(int id)
  {
    return await _context.Tickets
        .Include(t => t.Event) // Eager load event details
        .FirstOrDefaultAsync(t => t.Id == id);
  }

  public async Task<IEnumerable<Ticket>> GetAllAsync()
  {
    return await _context.Tickets
        .Include(t => t.Event)
        .OrderByDescending(t => t.PurchasedAt)
        .ToListAsync();
  }

  public async Task<IEnumerable<Ticket>> GetByEventIdAsync(int eventId)
  {
    return await _context.Tickets
        .Where(t => t.EventId == eventId)
        .OrderBy(t => t.SeatNumber)
        .ToListAsync();
  }

  public async Task<IEnumerable<Ticket>> GetByEmailAsync(string email)
  {
    return await _context.Tickets
        .Include(t => t.Event)
        .Where(t => t.AttendeeEmail == email)
        .OrderByDescending(t => t.PurchasedAt)
        .ToListAsync();
  }

  public async Task<Ticket> CreateAsync(Ticket ticket)
  {
    ticket.PurchasedAt = DateTime.UtcNow;
    _context.Tickets.Add(ticket);
    await _context.SaveChangesAsync();
    return ticket;
  }

  public async Task UpdateAsync(Ticket ticket)
  {
    _context.Tickets.Update(ticket);
    await _context.SaveChangesAsync();
  }

  public async Task DeleteAsync(int id)
  {
    var ticket = await _context.Tickets.FindAsync(id);
    if (ticket != null)
    {
      _context.Tickets.Remove(ticket);
      await _context.SaveChangesAsync();
    }
  }

  public async Task<int> GetTicketCountByEventIdAsync(int eventId)
  {
    return await _context.Tickets
        .CountAsync(t => t.EventId == eventId);
  }
}
