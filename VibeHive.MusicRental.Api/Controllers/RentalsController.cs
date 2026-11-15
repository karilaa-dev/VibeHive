using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Http.Json;
using System.Text.Json;
using VibeHive.MusicRental.Api.Models;
using VibeHive.MusicRental.Api.Services;

namespace VibeHive.MusicRental.Api.Controllers;

[ApiController]
[Route("api/rentals")]
[Authorize]
public class RentalsController : ControllerBase
{
    private readonly RentalManager rentals;
    private readonly IHttpClientFactory clients;

    public RentalsController(RentalManager rentals, IHttpClientFactory clients)
    {
        this.rentals = rentals;
        this.clients = clients;
    }

    [HttpGet]
    public async Task<ActionResult<List<Rental>>> GetRentals()
    {
        var active = await rentals.GetActive();
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(User.IsInRole("Admin") ? active : active.Where(rental => rental.UserId == userId).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<Rental>> RentAlbum(RentRequest request)
    {
        if (User.IsInRole("Admin"))
        {
            try
            {
                using var lookup = new HttpRequestMessage(HttpMethod.Get, $"api/users/{request.UserId}");
                lookup.Headers.Add("Authorization", Request.Headers.Authorization.ToString());
                using var response = await clients.CreateClient("Events").SendAsync(lookup);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return BadRequest("Select an existing regular customer.");
                if (!response.IsSuccessStatusCode)
                    return StatusCode(503, "Could not verify the selected customer with Events.");
                var customer = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (customer.GetProperty("role").GetString() != "User")
                    return BadRequest("Admins can rent albums only for regular users.");
                request.UserId = customer.GetProperty("id").GetInt32();
                request.UserName = customer.GetProperty("name").GetString() ?? $"User {request.UserId}";
            }
            catch (Exception error) when (error is HttpRequestException or TaskCanceledException)
            {
                return StatusCode(503, "Events is unavailable. Start it, then select a customer and try again.");
            }
        }
        else
        {
            request.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            request.UserName = User.FindFirstValue(ClaimTypes.Name) ?? $"User {request.UserId}";
        }
        return Ok(await rentals.Rent(request, Request.Headers.Authorization.ToString()));
    }

    [HttpPost("{id}/return")]
    public async Task<ActionResult<Rental>> ReturnAlbum(int id)
    {
        return Ok(await rentals.Return(id, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            User.IsInRole("Admin"), Request.Headers.Authorization.ToString()));
    }
}
