using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using VibeHive.Inventory;

namespace VibeHive.AlbumInventory.Grpc.Services;

public class AlbumInventoryService : global::VibeHive.Inventory.AlbumInventory.AlbumInventoryBase
{
    // Shared across service instances, like the Canvas UserAPI and previous BookService.
    private static readonly Dictionary<int, Album> albums = new();
    private static readonly Dictionary<int, string> reservations = new();
    private static readonly Dictionary<int, HashSet<string>> returnedReservations = new();
    private static readonly object syncRoot = new();
    private static int nextId = 1;

    public override Task<Album> AddAlbum(Album request, ServerCallContext context)
    {
        ValidateDetails(request);
        lock (syncRoot)
        {
            var album = Clean(request);
            album.Id = nextId++;
            albums.Add(album.Id, album);
            return Task.FromResult(album.Clone());
        }
    }

    public override Task<Album> GetAlbum(AlbumRequest request, ServerCallContext context)
    {
        lock (syncRoot)
        {
            return Task.FromResult(Find(request.Id).Clone());
        }
    }

    public override Task<Album> UpdateAlbum(Album request, ServerCallContext context)
    {
        ValidateDetails(request);
        lock (syncRoot)
        {
            Find(request.Id);
            if (reservations.ContainsKey(request.Id) && request.Available)
                throw Error(StatusCode.FailedPrecondition, "Return this album before making it available.");

            albums[request.Id] = Clean(request);
            return Task.FromResult(albums[request.Id].Clone());
        }
    }

    public override Task<Empty> DeleteAlbum(AlbumRequest request, ServerCallContext context)
    {
        lock (syncRoot)
        {
            Find(request.Id);
            if (reservations.ContainsKey(request.Id))
                throw Error(StatusCode.FailedPrecondition, "Return this album before deleting it.");

            albums.Remove(request.Id);
            returnedReservations.Remove(request.Id);
            return Task.FromResult(new Empty());
        }
    }

    public override async Task ListAlbums(ListAlbumsRequest request, IServerStreamWriter<Album> responseStream,
        ServerCallContext context)
    {
        List<Album> snapshot;
        lock (syncRoot)
        {
            snapshot = albums.Values.Where(album => !request.AvailableOnly || album.Available)
                .OrderBy(album => album.Id).Select(album => album.Clone()).ToList();
        }

        // Send a snapshot so another request can edit inventory while this stream is read.
        foreach (var album in snapshot)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            await responseStream.WriteAsync(album);
        }
    }

    public override Task<Album> ReserveAlbum(ReservationRequest request, ServerCallContext context)
    {
        ValidateReservation(request);
        lock (syncRoot)
        {
            var album = Find(request.AlbumId);
            if (reservations.TryGetValue(album.Id, out var owner) && owner == request.ReservationId)
                return Task.FromResult(album.Clone());
            if (returnedReservations.TryGetValue(album.Id, out var returned) && returned.Contains(request.ReservationId))
                throw Error(StatusCode.FailedPrecondition, "This reservation has already been returned.");
            if (!album.Available || reservations.ContainsKey(album.Id))
                throw Error(StatusCode.FailedPrecondition, "This album is not available.");

            reservations.Add(album.Id, request.ReservationId);
            album.Available = false;
            return Task.FromResult(album.Clone());
        }
    }

    public override Task<Album> ReleaseAlbum(ReservationRequest request, ServerCallContext context)
    {
        ValidateReservation(request);
        lock (syncRoot)
        {
            var album = Find(request.AlbumId);
            // Keep returned tokens so retrying a return cannot release a newer rental.
            if (returnedReservations.TryGetValue(album.Id, out var returned) && returned.Contains(request.ReservationId))
                return Task.FromResult(album.Clone());
            if (!reservations.TryGetValue(album.Id, out var owner) || owner != request.ReservationId)
                throw Error(StatusCode.FailedPrecondition, "This reservation does not own the album.");

            reservations.Remove(album.Id);
            if (!returnedReservations.ContainsKey(album.Id))
                returnedReservations.Add(album.Id, new HashSet<string>());
            returnedReservations[album.Id].Add(request.ReservationId);
            album.Available = true;
            return Task.FromResult(album.Clone());
        }
    }

    private static Album Find(int id)
    {
        if (id < 1)
            throw Error(StatusCode.InvalidArgument, "Album ID must be positive.");
        if (!albums.TryGetValue(id, out var album))
            throw Error(StatusCode.NotFound, "Album not found.");
        return album;
    }

    private static Album Clean(Album request)
    {
        var album = request.Clone();
        album.Title = album.Title.Trim();
        album.Artist = album.Artist.Trim();
        album.Genre = album.Genre.Trim();
        return album;
    }

    private static void ValidateDetails(Album album)
    {
        if (string.IsNullOrWhiteSpace(album.Title) || string.IsNullOrWhiteSpace(album.Artist)
            || string.IsNullOrWhiteSpace(album.Genre))
            throw Error(StatusCode.InvalidArgument, "Title, artist, and genre are required.");
        if (album.Year < 1 || album.Year > 9999)
            throw Error(StatusCode.InvalidArgument, "Year must be from 1 to 9999.");
    }

    private static void ValidateReservation(ReservationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ReservationId))
            throw Error(StatusCode.InvalidArgument, "A reservation ID is required.");
    }

    private static RpcException Error(StatusCode code, string message)
    {
        return new RpcException(new Status(code, message));
    }
}
