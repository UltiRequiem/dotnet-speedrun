using EventTicketing.Application.Interfaces;
using EventTicketing.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController(
    ITicketRepository ticketRepository,
    IEventRepository eventRepository,
    ILogger<TicketsController> logger) : ControllerBase
{
  private readonly ITicketRepository _ticketRepository = ticketRepository;
  private readonly IEventRepository _eventRepository = eventRepository;
  private readonly ILogger<TicketsController> _logger = logger;

  // GET: api/tickets
  [HttpGet]
  public async Task<ActionResult<IEnumerable<Ticket>>> GetAllTickets()
  {
    var tickets = await _ticketRepository.GetAllAsync();
    return Ok(tickets);
  }

  // GET: api/tickets/5
  [HttpGet("{id}")]
  public async Task<ActionResult<Ticket>> GetTicket(int id)
  {
    var ticket = await _ticketRepository.GetByIdAsync(id);

    if (ticket == null)
    {
      return NotFound(new { message = $"Ticket with ID {id} not found" });
    }

    return Ok(ticket);
  }

  // GET: api/tickets/event/5
  [HttpGet("event/{eventId}")]
  public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByEvent(int eventId)
  {
    var eventExists = await _eventRepository.ExistsAsync(eventId);
    if (!eventExists)
    {
      return NotFound(new { message = $"Event with ID {eventId} not found" });
    }

    var tickets = await _ticketRepository.GetByEventIdAsync(eventId);
    return Ok(tickets);
  }

  // GET: api/tickets/email/user@example.com
  [HttpGet("email/{email}")]
  public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByEmail(string email)
  {
    var tickets = await _ticketRepository.GetByEmailAsync(email);
    return Ok(tickets);
  }

  // POST: api/tickets
  [HttpPost]
  public async Task<ActionResult<Ticket>> CreateTicket(Ticket ticket)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    // Validate event exists
    var eventExists = await _eventRepository.ExistsAsync(ticket.EventId);
    if (!eventExists)
    {
      return BadRequest(new { message = $"Event with ID {ticket.EventId} not found" });
    }

    var createdTicket = await _ticketRepository.CreateAsync(ticket);

    return CreatedAtAction(
        nameof(GetTicket),
        new { id = createdTicket.Id },
        createdTicket
    );
  }

  // PUT: api/tickets/5
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateTicket(int id, Ticket ticket)
  {
    if (id != ticket.Id)
    {
      return BadRequest(new { message = "ID mismatch" });
    }

    var existingTicket = await _ticketRepository.GetByIdAsync(id);
    if (existingTicket == null)
    {
      return NotFound(new { message = $"Ticket with ID {id} not found" });
    }

    await _ticketRepository.UpdateAsync(ticket);

    return NoContent();
  }

  // DELETE: api/tickets/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteTicket(int id)
  {
    var ticket = await _ticketRepository.GetByIdAsync(id);
    if (ticket == null)
    {
      return NotFound(new { message = $"Ticket with ID {id} not found" });
    }

    await _ticketRepository.DeleteAsync(id);

    return NoContent();
  }
}