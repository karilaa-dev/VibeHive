using Microsoft.AspNetCore.Mvc;
using VibeHive.MusicRental.Api.Models;
using VibeHive.MusicRental.Api.Services;

namespace VibeHive.MusicRental.Api.Controllers;

[ApiController]
[Route("api/rentals")]
public class RentalsController : ControllerBase
{
    private readonly RentalManager rentals;

    public RentalsController(RentalManager rentals)
    {
        this.rentals = rentals;
    }

    [HttpGet]
    public async Task<ActionResult<List<Rental>>> GetRentals()
    {
        return Ok(await rentals.GetActive());
    }

    [HttpPost]
    public async Task<ActionResult<Rental>> RentAlbum(RentRequest request)
    {
        return Ok(await rentals.Rent(request));
    }

    [HttpPost("{id}/return")]
    public async Task<ActionResult<Rental>> ReturnAlbum(int id)
    {
        return Ok(await rentals.Return(id));
    }
}
