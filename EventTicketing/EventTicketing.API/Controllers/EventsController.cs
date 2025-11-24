using EventTicketing.Application.Interfaces;
using EventTicketing.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(IEventRepository eventRepository, ILogger<EventsController> logger) : ControllerBase
{
  private readonly IEventRepository _eventRepository = eventRepository;
  private readonly ILogger<EventsController> _logger = logger;

  // GET: api/events
  [HttpGet]
  public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
  {
    var events = await _eventRepository.GetAllAsync();
    return Ok(events);
  }

  // GET: api/events/5
  [HttpGet("{id}")]
  public async Task<ActionResult<Event>> GetEvent(int id)
  {
    var @event = await _eventRepository.GetByIdAsync(id);

    if (@event == null)
    {
      return NotFound(new { message = $"Event with ID {id} not found" });
    }

    return Ok(@event);
  }

  // GET: api/events/upcoming
  [HttpGet("upcoming")]
  public async Task<ActionResult<IEnumerable<Event>>> GetUpcomingEvents()
  {
    var events = await _eventRepository.GetUpcomingEventsAsync();
    return Ok(events);
  }

  // POST: api/events
  [HttpPost]
  public async Task<ActionResult<Event>> CreateEvent(Event @event)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var createdEvent = await _eventRepository.CreateAsync(@event);

    return CreatedAtAction(
        nameof(GetEvent),
        new { id = createdEvent.Id },
        createdEvent
    );
  }

  // PUT: api/events/5
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateEvent(int id, Event @event)
  {
    if (id != @event.Id)
    {
      return BadRequest(new { message = "ID mismatch" });
    }

    var exists = await _eventRepository.ExistsAsync(id);

    if (!exists)
    {
      return NotFound(new { message = $"Event with ID {id} not found" });
    }

    await _eventRepository.UpdateAsync(@event);

    return NoContent();
  }

  // DELETE: api/events/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteEvent(int id)
  {
    var exists = await _eventRepository.ExistsAsync(id);
    if (!exists)
    {
      return NotFound(new { message = $"Event with ID {id} not found" });
    }

    await _eventRepository.DeleteAsync(id);

    return NoContent();
  }
}