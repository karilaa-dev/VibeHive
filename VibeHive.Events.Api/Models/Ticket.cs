namespace VibeHive.Events.Api.Models;

public class Ticket
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; } = "Booked"; // "Booked", "Cancelled"
    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
}

public record BookTicketRequest(int EventId);
public record UpdateTicketStatusRequest(string Status);
