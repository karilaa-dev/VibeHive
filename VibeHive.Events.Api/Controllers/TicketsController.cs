using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VibeHive.Events.Api.Models;
using VibeHive.Events.Api.Services;

namespace VibeHive.Events.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    [HttpPost]
    public IActionResult BookTicket([FromBody] BookTicketRequest request)
    {
        lock (DataStore.Events)
        {
            var evt = DataStore.Events.FirstOrDefault(e => e.Id == request.EventId);
            if (evt == null) return NotFound(new { Message = "Event not found." });
            if (evt.AvailableTickets <= 0) return BadRequest(new { Message = "No tickets available." });

            evt.AvailableTickets--;
            var ticket = new Ticket
            {
                Id = DataStore.Tickets.Count + 1,
                EventId = request.EventId,
                UserId = request.UserId,
                Status = "Booked",
            };
            DataStore.Tickets.Add(ticket);
            return Ok(ticket);
        }
    }

    [HttpGet("{userId:int}")]
    public IActionResult GetUserTickets(int userId)
    {
        var tickets = (from t in DataStore.Tickets
                       join e in DataStore.Events on t.EventId equals e.Id
                       where t.UserId == userId
                       select new
                       {
                           t.Id,
                           t.EventId,
                           EventName = e.Name,
                           e.Date,
                           e.Venue,
                           t.Status,
                           t.BookedAt
                       }).ToList();

        return Ok(tickets);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateTicketStatus(int id, [FromBody] UpdateTicketStatusRequest request)
    {
        var ticket = DataStore.Tickets.FirstOrDefault(t => t.Id == id);
        if (ticket == null) return NotFound(new { Message = "Ticket not found" });

        if (ticket.Status != "Cancelled" && request.Status == "Cancelled")
        {
            var evt = DataStore.Events.FirstOrDefault(e => e.Id == ticket.EventId);
            if (evt != null) evt.AvailableTickets++;
        }

        ticket.Status = request.Status;
        return Ok(ticket);
    }
}