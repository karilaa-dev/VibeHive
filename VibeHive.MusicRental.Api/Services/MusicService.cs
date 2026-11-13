using Grpc.Core;
using Grpc.Net.Client;
using VibeHive.Inventory;
using Album = VibeHive.MusicRental.Api.Models.Album;
using InventoryAlbum = VibeHive.Inventory.Album;

namespace VibeHive.MusicRental.Api.Services;

// Album Inventory is the only owner of albums. This service is a gRPC client.
public class MusicService : IDisposable
{
    private readonly GrpcChannel channel;
    private readonly AlbumInventory.AlbumInventoryClient client;

    public MusicService(IConfiguration configuration)
    {
        string address = Environment.GetEnvironmentVariable("VIBEHIVE_INVENTORY_URL")
            ?? configuration["AlbumInventory:Url"] ?? "http://localhost:5056";
        channel = GrpcChannel.ForAddress(address);
        client = new AlbumInventory.AlbumInventoryClient(channel);
    }

    public async Task<List<Album>> GetAll(bool availableOnly = false)
    {
        using var call = client.ListAlbums(new ListAlbumsRequest { AvailableOnly = availableOnly }, deadline: Deadline());
        var albums = new List<Album>();
        await foreach (var album in call.ResponseStream.ReadAllAsync())
        {
            albums.Add(ToModel(album));
        }
        return albums;
    }

    public async Task<Album> Add(Album album, string authorization)
    {
        var request = new InventoryAlbum
        {
            Title = album.Title,
            Artist = album.Artist,
            Genre = album.Genre,
            Year = album.Year,
            Available = album.Available
        };
        var addedAlbum = await client.AddAlbumAsync(request, headers: AuthHeaders(authorization), deadline: Deadline());
        return ToModel(addedAlbum);
    }

    public async Task Delete(int id, string authorization)
    {
        await client.DeleteAlbumAsync(new AlbumRequest { Id = id }, headers: AuthHeaders(authorization), deadline: Deadline());
    }

    public async Task<Album> Reserve(int albumId, string reservationId, string authorization)
    {
        var request = new ReservationRequest
        {
            AlbumId = albumId,
            ReservationId = reservationId
        };
        var album = await client.ReserveAlbumAsync(request, headers: AuthHeaders(authorization), deadline: Deadline());
        return ToModel(album);
    }

    public async Task Release(int albumId, string reservationId, string authorization)
    {
        var request = new ReservationRequest
        {
            AlbumId = albumId,
            ReservationId = reservationId
        };
        await client.ReleaseAlbumAsync(request, headers: AuthHeaders(authorization), deadline: Deadline());
    }

    private static DateTime Deadline() => DateTime.UtcNow.AddSeconds(5);

    private static Metadata AuthHeaders(string authorization) => new() { { "authorization", authorization } };

    private static Album ToModel(InventoryAlbum album)
    {
        return new Album
        {
            Id = album.Id,
            Title = album.Title,
            Artist = album.Artist,
            Genre = album.Genre,
            Year = album.Year,
            Available = album.Available
        };
    }

    public void Dispose()
    {
        channel.Dispose();
    }
}
