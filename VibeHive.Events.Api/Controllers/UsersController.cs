using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VibeHive.Events.Api.Models;
using VibeHive.Events.Api.Services;

namespace VibeHive.Events.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetUsers() => Ok(DataStore.Users.Select(user => new { user.Id, user.Name, user.Role }).ToList());

    [HttpGet("{id:int}")]
    [Authorize]
    public IActionResult GetUser(int id)
    {
        var user = DataStore.Users.FirstOrDefault(user => user.Id == id);
        return user == null ? NotFound() : Ok(new { user.Id, user.Name, user.Role });
    }

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
            Id = DataStore.Users.Select(user => user.Id).DefaultIfEmpty(0).Max() + 1,
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            Role = request.Role
        };

        DataStore.Users.Add(user);
        return Ok(new { Message = "Registration successful", userId = user.Id });
    }
}
