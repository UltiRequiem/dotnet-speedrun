namespace EventTicketing.Domain.Entities;

public class Ticket
{
    public int Id { get; set; }
    public int EventId { get; set; }  // Foreign key
    public string AttendeeEmail { get; set; } = string.Empty;
    public string AttendeeFullName { get; set; } = string.Empty;
    public string SeatNumber { get; set; } = string.Empty;
    public decimal PricePaid { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime PurchasedAt { get; set; }
    
    // Navigation property - many Tickets belong to one Event
    public Event Event { get; set; } = null!;
}

public enum TicketStatus
{
    Reserved = 0,
    Paid = 1,
    Cancelled = 2,
    CheckedIn = 3
}