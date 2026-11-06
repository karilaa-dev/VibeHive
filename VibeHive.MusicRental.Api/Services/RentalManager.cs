using Grpc.Core;
using VibeHive.MusicRental.Api.Models;

namespace VibeHive.MusicRental.Api.Services;

public class RentalManager
{
    private readonly MusicService music;
    private readonly List<Rental> rentals = new();
    // Async requests need a semaphore because a lock cannot be held across await.
    private readonly SemaphoreSlim gate = new(1, 1);
    private int nextId = 1;

    public RentalManager(MusicService music)
    {
        this.music = music;
    }

    public async Task<List<Rental>> GetActive()
    {
        await gate.WaitAsync();
        try
        {
            return rentals.Where(rental => rental.ReturnDate == null).ToList();
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<Rental> Rent(RentRequest request)
    {
        await gate.WaitAsync();
        try
        {
            if (rentals.Any(rental => rental.AlbumId == request.AlbumId && rental.ReturnDate == null))
                throw new RpcException(new Status(StatusCode.FailedPrecondition,
                    "This album already has an active or pending rental. Return it before renting again."));

            var rental = new Rental
            {
                Id = nextId++,
                UserId = request.UserId,
                AlbumId = request.AlbumId,
                UserName = string.IsNullOrWhiteSpace(request.UserName)
                    ? $"User {request.UserId}" : request.UserName.Trim(),
                AlbumTitle = $"Album {request.AlbumId}",
                RentalDate = DateTime.UtcNow
            };
            try
            {
                var album = await music.Reserve(request.AlbumId, rental.ReservationId);
                rental.AlbumTitle = album.Title;
            }
            catch (RpcException error) when (IsConnectionError(error))
            {
                // The server may have committed before the connection broke. Keep its token
                // so the customer can safely return this pending rental after reconnecting.
                rental.ReservationPending = true;
                rentals.Add(rental);
                throw;
            }
            rentals.Add(rental);
            return rental;
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<Rental> Return(int id)
    {
        await gate.WaitAsync();
        try
        {
            var rental = rentals.Find(item => item.Id == id);
            if (rental == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Rental not found."));
            if (rental.ReturnDate != null)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "This rental has already been returned."));

            try
            {
                await music.Release(rental.AlbumId, rental.ReservationId);
            }
            catch (RpcException error) when (rental.ReservationPending &&
                error.StatusCode is StatusCode.NotFound or StatusCode.FailedPrecondition)
            {
                // A pending reservation that never reached inventory can be canceled safely.
            }
            rental.ReturnDate = DateTime.UtcNow;
            rental.ReservationPending = false;
            return rental;
        }
        finally
        {
            gate.Release();
        }
    }

    private static bool IsConnectionError(RpcException error)
    {
        return error.StatusCode is StatusCode.Unavailable or StatusCode.DeadlineExceeded or StatusCode.Cancelled;
    }
}
