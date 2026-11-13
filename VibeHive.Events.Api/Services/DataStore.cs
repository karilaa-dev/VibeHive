using VibeHive.Events.Api.Models;

namespace VibeHive.Events.Api.Services;

public static class DataStore
{
    public static List<User> Users { get; } = new();

    public static List<Event> Events { get; } = new()
    {
        new Event { Id = 1, Name = "Summer Indie Fest", Date = DateTime.Now.AddDays(14), Venue = "Skyline Amphitheater", Genre = "Indie Rock", AvailableTickets = 150 },
        new Event { Id = 2, Name = "Neon Synthwave Night", Date = DateTime.Now.AddDays(20), Venue = "Warehouse 9", Genre = "Electronic", AvailableTickets = 80 }
    };

    public static List<Ticket> Tickets { get; } = new();
}
