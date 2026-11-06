using Microsoft.AspNetCore.Mvc;
using VibeHive.MusicRental.Api.Models;
using VibeHive.MusicRental.Api.Services;

namespace VibeHive.MusicRental.Api.Controllers;

[ApiController]
[Route("api/music")]
public class MusicController : ControllerBase
{
    private readonly MusicService music;

    public MusicController(MusicService music)
    {
        this.music = music;
    }

    [HttpGet]
    public async Task<ActionResult<List<Album>>> GetAlbums(bool includeUnavailable = false)
    {
        return Ok(await music.GetAll(availableOnly: !includeUnavailable));
    }

    [HttpPost]
    public async Task<ActionResult<Album>> AddAlbum(Album album)
    {
        return Ok(await music.Add(album));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAlbum(int id)
    {
        await music.Delete(id);
        return Ok("Album removed successfully!");
    }
}
