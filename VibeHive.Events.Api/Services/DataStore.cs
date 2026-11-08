using VibeHive.Events.Api.Models;

namespace VibeHive.Events.Api.Services;

public static class DataStore
{
    public static List<User> Users { get; } = new()
    {
        new User { Id = 1, Name = "Admin User", Email = "admin@vibehive.com", Password = "Admin@123", Role = "Admin" },
        new User { Id = 2, Name = "Stage Manager", Email = "manager@vibehive.com", Password = "Manager@123", Role = "Manager" },
        new User { Id = 3, Name = "Regular User", Email = "user@vibehive.com", Password = "User@123", Role = "User" }
    };

    public static List<Event> Events { get; } = new()
    {
        new Event { Id = 1, Name = "Summer Indie Fest", Date = DateTime.Now.AddDays(14), Venue = "Skyline Amphitheater", Genre = "Indie Rock", AvailableTickets = 150 },
        new Event { Id = 2, Name = "Neon Synthwave Night", Date = DateTime.Now.AddDays(20), Venue = "Warehouse 9", Genre = "Electronic", AvailableTickets = 80 }
    };

    public static List<Ticket> Tickets { get; } = new();
}