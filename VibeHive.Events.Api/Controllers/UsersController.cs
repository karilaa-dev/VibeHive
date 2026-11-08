using Microsoft.AspNetCore.Mvc;
using VibeHive.Events.Api.Models;
using VibeHive.Events.Api.Services;

namespace VibeHive.Events.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (DataStore.Users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
            return BadRequest(new { Message = "Email already registered." });

        var validRoles = new[] { "User", "Admin", "Event Manager" };
        if (!validRoles.Contains(request.Role))
            return BadRequest(new { message = "Invalid role. Choose from 'User', 'Admin', or 'Event Manager'." });

        var user = new User
        {
            Id = DataStore.Users.Count + 1,
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            Role = request.Role
        };

        DataStore.Users.Add(user);
        return Ok(new { Message = "Registration successful", userId = user.Id });
    }
}