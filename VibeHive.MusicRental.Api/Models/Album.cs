using System.ComponentModel.DataAnnotations;

namespace VibeHive.MusicRental.Api.Models;

public class Album
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = "";
    [Required]
    public string Artist { get; set; } = "";
    [Required]
    public string Genre { get; set; } = "";
    [Range(1, 9999)]
    public int Year { get; set; }
    public bool Available { get; set; } = true;
}
