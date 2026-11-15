using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using VibeHive.WinForms.Services;
using Album = VibeHive.WinForms.Models.Album;
using Rental = VibeHive.WinForms.Models.Rental;

namespace VibeHive.WinForms;

internal static class ExampleData
{
    public static async Task<string> LoadAsync()
    {
        using var api = new HttpClient
        {
            BaseAddress = new Uri(ClientSettings.MusicRental),
            Timeout = TimeSpan.FromSeconds(10)
        };
        api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserSession.Token);
        using var libraryResponse = await api.GetAsync("api/music?includeUnavailable=true");
        await CheckExampleResponseAsync(libraryResponse);
        var library = await libraryResponse.Content.ReadFromJsonAsync<List<Album>>() ?? new List<Album>();

        var examples = new List<Album>
        {
            new Album { Title = "Discovery", Artist = "Daft Punk", Genre = "Electronic", Year = 2001 },
            new Album { Title = "Kind of Blue", Artist = "Miles Davis", Genre = "Jazz", Year = 1959 },
            new Album { Title = "Abbey Road", Artist = "The Beatles", Genre = "Rock", Year = 1969 },
            new Album { Title = "Thriller", Artist = "Michael Jackson", Genre = "Pop", Year = 1982 },
            new Album { Title = "Rumours", Artist = "Fleetwood Mac", Genre = "Rock", Year = 1977 }
        };
        var exampleAlbums = new List<Album>();
        int addedAlbums = 0;
        foreach (var example in examples)
        {
            var album = library.FirstOrDefault(item =>
                string.Equals(item.Title, example.Title, StringComparison.OrdinalIgnoreCase)
                && string.Equals(item.Artist, example.Artist, StringComparison.OrdinalIgnoreCase)
                && item.Year == example.Year);
            if (album == null)
            {
                using var response = await api.PostAsJsonAsync("api/music", example);
                await CheckExampleResponseAsync(response);
                album = await response.Content.ReadFromJsonAsync<Album>()
                    ?? throw new InvalidOperationException("The API did not return the added album.");
                library.Add(album);
                addedAlbums++;
            }
            exampleAlbums.Add(album);
        }

        using var rentalsResponse = await api.GetAsync("api/rentals");
        await CheckExampleResponseAsync(rentalsResponse);
        var rentals = await rentalsResponse.Content.ReadFromJsonAsync<List<Rental>>() ?? new List<Rental>();
        using var directory = new EventsApiClient(ClientSettings.EventsApiBaseUrl);
        var users = await directory.GetUsersAsync();
        using var eventsApi = new HttpClient { BaseAddress = new Uri(ClientSettings.EventsApiBaseUrl), Timeout = TimeSpan.FromSeconds(10) };
        var customers = new[]
        {
            (Name: "Alex Morgan", Email: "alex.morgan@example.com"),
            (Name: "Jamie Lee", Email: "jamie.lee@example.com")
        };
        const string examplePassword = "Example@123";
        int addedUsers = 0;
        int addedRentals = 0;
        string? playlistOwnerToken = null;
        int playlistOwnerId = 0;
        for (int index = 0; index < customers.Length; index++)
        {
            var customer = customers[index];
            if (!users.Any(user => user.Name == customer.Name && user.Role == "User"))
            {
                if (!await directory.RegisterAsync(customer.Name, customer.Email, examplePassword, "User"))
                    throw new InvalidOperationException($"Could not create example user {customer.Name}.");
                addedUsers++;
            }
            using var loginResponse = await eventsApi.PostAsJsonAsync("api/auth/login", new { email = customer.Email, password = examplePassword });
            await CheckExampleResponseAsync(loginResponse);
            var login = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
            if (login.GetProperty("role").GetString() != "User")
                throw new InvalidOperationException("Example rentals must belong to regular users.");
            if (index == 0)
            {
                playlistOwnerToken = login.GetProperty("token").GetString();
                playlistOwnerId = login.GetProperty("userId").GetInt32();
            }
            var album = exampleAlbums[index];
            if (!album.Available || rentals.Any(rental => rental.AlbumId == album.Id))
                continue;
            var request = new { UserId = login.GetProperty("userId").GetInt32(), UserName = customer.Name, AlbumId = album.Id };
            using var rentalRequest = new HttpRequestMessage(HttpMethod.Post, "api/rentals") { Content = JsonContent.Create(request) };
            rentalRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", login.GetProperty("token").GetString());
            using var response = await api.SendAsync(rentalRequest);
            await CheckExampleResponseAsync(response);
            addedRentals++;
        }

        using var playlistApi = new HttpClient { BaseAddress = new Uri(ClientSettings.Playlist), Timeout = TimeSpan.FromSeconds(10) };
        playlistApi.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", playlistOwnerToken);
        var playlists = await playlistApi.GetFromJsonAsync<List<PlaylistModel>>("api/playlists") ?? new();
        var demoPlaylist = playlists.FirstOrDefault(item => item.Name == "VibeHive Community"
            && item.CreatedBy == playlistOwnerId && item.IsCollaborative);
        int addedPlaylists = 0;
        if (demoPlaylist == null)
        {
            using var response = await playlistApi.PostAsJsonAsync("api/playlists", new
            {
                Name = "VibeHive Community", CreatedBy = playlistOwnerId, IsCollaborative = true
            });
            await CheckExampleResponseAsync(response);
            demoPlaylist = await response.Content.ReadFromJsonAsync<PlaylistModel>()
                ?? throw new InvalidOperationException("The API did not return the example playlist.");
            addedPlaylists++;
        }
        users = await directory.GetUsersAsync();
        int addedInvitations = 0;
        foreach (var user in users.Where(user => user.Role == "User" && user.Id != playlistOwnerId
            && !demoPlaylist.Collaborators.Contains(user.Id)))
        {
            using var response = await playlistApi.PutAsJsonAsync($"api/playlists/{demoPlaylist.Id}/invite",
                new { InvitedBy = playlistOwnerId, InvitedUserId = user.Id });
            await CheckExampleResponseAsync(response);
            addedInvitations++;
        }
        var exampleSongs = new[]
        {
            new SongModel(0, "One More Time", "Daft Punk", "Electronic", 320, 0),
            new SongModel(0, "Digital Love", "Daft Punk", "Electronic", 301, 0),
            new SongModel(0, "So What", "Miles Davis", "Jazz", 562, 0),
            new SongModel(0, "Freddie Freeloader", "Miles Davis", "Jazz", 587, 0),
            new SongModel(0, "Come Together", "The Beatles", "Rock", 259, 0),
            new SongModel(0, "Here Comes the Sun", "The Beatles", "Rock", 185, 0),
            new SongModel(0, "Billie Jean", "Michael Jackson", "Pop", 294, 0),
            new SongModel(0, "Beat It", "Michael Jackson", "Pop", 258, 0),
            new SongModel(0, "Dreams", "Fleetwood Mac", "Rock", 257, 0),
            new SongModel(0, "Go Your Own Way", "Fleetwood Mac", "Rock", 223, 0)
        };
        int addedSongs = 0;
        foreach (var song in exampleSongs)
        {
            if (demoPlaylist.SongDetails.Any(item => string.Equals(item.Title, song.Title, StringComparison.OrdinalIgnoreCase)
                && string.Equals(item.Artist, song.Artist, StringComparison.OrdinalIgnoreCase))) continue;
            using var response = await playlistApi.PutAsJsonAsync($"api/playlists/{demoPlaylist.Id}/add", new
            {
                UserId = playlistOwnerId, song.Title, song.Artist, song.Genre, DurationSeconds = song.Duration
            });
            await CheckExampleResponseAsync(response);
            addedSongs++;
        }

        return $"Added {addedAlbums} albums, {addedUsers} regular users, {addedRentals} rentals, "
            + $"{addedPlaylists} playlists, {addedInvitations} invitations and {addedSongs} songs. Existing data was kept.";
    }

    private static async Task CheckExampleResponseAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(message)
                ? response.ReasonPhrase : message.Trim('"'));
        }
    }

}
