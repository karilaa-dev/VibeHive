namespace VibeHive.MusicRental.Api.Models;

public class Rental
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AlbumId { get; set; }
    public string UserName { get; set; } = "";
    public string AlbumTitle { get; set; } = "";
    public DateTime RentalDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool ReservationPending { get; set; }

    // Used only for inventory calls. Internal properties are not included in JSON responses.
    internal string ReservationId { get; } = Guid.NewGuid().ToString();
}
