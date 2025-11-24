namespace EventTicketing.API.DTOs;

public class CreateTicketDto
{
    public int EventId { get; set; }
    public string AttendeeEmail { get; set; } = string.Empty;
    public string AttendeeFullName { get; set; } = string.Empty;
    public string SeatNumber { get; set; } = string.Empty;
    public decimal PricePaid { get; set; }
    public int Status { get; set; }
    public DateTime PurchasedAt { get; set; }
}
