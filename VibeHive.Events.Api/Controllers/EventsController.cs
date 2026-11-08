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
        var newEvent = new Event
        {
            Id = DataStore.Events.Count + 1,
            Name = request.Name,
            Date = request.Date,
            Venue = request.Venue,
            Genre = request.Genre,
            AvailableTickets = request.AvailableTickets
        };

        DataStore.Events.Add(newEvent);
        return CreatedAtAction(nameof(GetEvents), new { id = newEvent.Id }, newEvent);
    }
}