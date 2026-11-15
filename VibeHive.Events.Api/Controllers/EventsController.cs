using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VibeHive.Events.Api.Models;
using VibeHive.Events.Api.Services;

namespace VibeHive.Events.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetEvents()
    {
        var upcoming = DataStore.Events.Where(e => e.Date >= DateTime.Now).OrderBy(e => e.Date).ToList();
        return Ok(upcoming);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Event Manager")]
    public IActionResult CreateEvent([FromBody] CreateEventRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Venue)
            || string.IsNullOrWhiteSpace(request.Genre))
            return BadRequest(new { Message = "Name, venue, and genre are required." });

        var date = request.Date.ToUniversalTime();
        if (date <= DateTime.UtcNow)
            return BadRequest(new { Message = "Event date must be in the future." });

        if (request.AvailableTickets < 1 || request.AvailableTickets > 100000)
            return BadRequest(new { Message = "Ticket count must be between 1 and 100000." });

        var newEvent = new Event
        {
            Id = DataStore.Events.Count + 1,
            Name = request.Name.Trim(),
            Date = date.ToLocalTime(),
            Venue = request.Venue.Trim(),
            Genre = request.Genre.Trim(),
            AvailableTickets = request.AvailableTickets
        };

        DataStore.Events.Add(newEvent);
        return CreatedAtAction(nameof(GetEvents), new { id = newEvent.Id }, newEvent);
    }
}
