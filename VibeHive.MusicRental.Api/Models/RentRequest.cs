using System.ComponentModel.DataAnnotations;

namespace VibeHive.MusicRental.Api.Models;

public class RentRequest
{
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
    [Range(1, int.MaxValue)]
    public int AlbumId { get; set; }
    public string UserName { get; set; } = "";
}
