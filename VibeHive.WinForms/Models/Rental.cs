namespace VibeHive.WinForms.Models;

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
}
