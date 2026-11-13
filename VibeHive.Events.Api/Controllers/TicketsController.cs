using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId) || userId <= 0)
            return Unauthorized();

        lock (DataStore.Events)
        {
            var evt = DataStore.Events.FirstOrDefault(e => e.Id == request.EventId);
            if (evt == null) return NotFound(new { Message = "Event not found." });
            if (evt.Date <= DateTime.Now) return BadRequest(new { Message = "Cannot book tickets for a past event." });
            if (evt.AvailableTickets <= 0) return BadRequest(new { Message = "No tickets available." });

            evt.AvailableTickets--;
            var ticket = new Ticket
            {
                Id = DataStore.Tickets.Count + 1,
                EventId = request.EventId,
                UserId = userId,
                Status = "Booked",
            };
            DataStore.Tickets.Add(ticket);
            return Ok(ticket);
        }
    }

    [HttpGet]
    [HttpGet("{userId:int}")]
    public IActionResult GetUserTickets([FromRoute] int? userId = null)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int currentUserId) || currentUserId <= 0)
            return Unauthorized();
        bool isAdmin = User.IsInRole("Admin");
        if (userId.HasValue && userId.Value != currentUserId && !isAdmin)
            return Forbid();

        int? targetUserId = userId ?? (isAdmin ? null : currentUserId);
        lock (DataStore.Events)
        {
            var tickets = (from t in DataStore.Tickets
                           join e in DataStore.Events on t.EventId equals e.Id
                           where !targetUserId.HasValue || t.UserId == targetUserId.Value
                           select new
                           {
                               t.Id,
                               t.EventId,
                               t.UserId,
                               UserName = DataStore.Users.FirstOrDefault(user => user.Id == t.UserId)?.Name ?? "User",
                               EventName = e.Name,
                               e.Date,
                               e.Venue,
                               t.Status,
                               t.BookedAt
                           }).ToList();

            return Ok(tickets);
        }
    }

    [HttpPatch("{id:int}/status")]
    public IActionResult UpdateTicketStatus(int id, [FromBody] UpdateTicketStatusRequest request)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId) || userId <= 0)
            return Unauthorized();

        lock (DataStore.Events)
        {
            var ticket = DataStore.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null) return NotFound(new { Message = "Ticket not found." });
            bool isAdmin = User.IsInRole("Admin");
            if (!isAdmin && ticket.UserId != userId) return Forbid();
            if (request.Status is not ("Booked" or "Cancelled"))
                return BadRequest(new { Message = "Status must be Booked or Cancelled." });
            if (!isAdmin && request.Status != "Cancelled") return Forbid();
            if (ticket.Status == request.Status) return Ok(ticket);

            var evt = DataStore.Events.FirstOrDefault(e => e.Id == ticket.EventId);
            if (evt == null) return NotFound(new { Message = "Event not found." });
            if (request.Status == "Booked")
            {
                if (evt.Date <= DateTime.Now) return BadRequest(new { Message = "Cannot book tickets for a past event." });
                if (evt.AvailableTickets <= 0) return BadRequest(new { Message = "No tickets available." });
                evt.AvailableTickets--;
            }
            else
            {
                evt.AvailableTickets++;
            }

            ticket.Status = request.Status;
            return Ok(ticket);
        }
    }
}
