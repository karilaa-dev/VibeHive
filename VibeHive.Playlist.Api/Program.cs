// =====================================================================
// VibeHive - Part 2: Collaborative Playlist Builder (REST API)
// ASP.NET Core minimal API, in-memory storage.
//
// OOP concepts : abstraction, inheritance, encapsulation, polymorphism
// Pattern      : Command Pattern (add / remove / vote) with history
// =====================================================================

using System.Security.Claims;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Playlist module services
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddSingleton<PlaylistService>();
builder.Services.AddHttpClient("Events", client =>
{
    client.BaseAddress = new Uri((Environment.GetEnvironmentVariable("VIBEHIVE_EVENTS_URL")
        ?? builder.Configuration["Events:Url"] ?? "http://localhost:5273").TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(3);
});
// Reuse the Events issuer, audience and signing key for playlist identity.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "VibeHive.Events.Api",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "VibeHive.Clients",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["Jwt:Key"] ?? "VibeHiveSecretKeyForAuthentication2026!"))
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok());
var api = app.MapGroup("/api/playlists").RequireAuthorization(policy => policy.RequireRole("User", "Admin"));

static int CurrentUserId(ClaimsPrincipal user) => int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

// POST /api/playlists : create a new playlist
api.MapPost("/", (CreatePlaylistRequest req, ClaimsPrincipal user, PlaylistService svc) =>
{
    req = req with { CreatedBy = CurrentUserId(user) };
    if (string.IsNullOrWhiteSpace(req.Name))
        return Results.BadRequest(new { error = "Playlist name is required." });
    if (req.CreatedBy <= 0)
        return Results.BadRequest(new { error = "A valid CreatedBy user ID is required." });

    var playlist = svc.Create(req.Name.Trim(), req.CreatedBy, req.IsCollaborative);
    return Results.Created($"/api/playlists/{playlist.Id}", playlist.ToDto());
});

// GET /api/playlists : playlists owned by or shared with the signed-in user
api.MapGet("/", (ClaimsPrincipal user, PlaylistService svc) =>
    Results.Ok(svc.GetAll(user.IsInRole("Admin") ? null : CurrentUserId(user)).Select(p => p.ToDto())));

// GET /api/playlists/{id} : single playlist (handy for the GUI refresh timer)
api.MapGet("/{id:int}", (int id, ClaimsPrincipal user, PlaylistService svc) =>
{
    var p = svc.Get(id);
    if (p != null && !user.IsInRole("Admin") && !p.CanEdit(CurrentUserId(user))) return Results.Forbid();
    return p is null ? Results.NotFound(new { error = "Playlist not found." }) : Results.Ok(p.ToDto());
});

// PUT /api/playlists/{id}/add : add a song to a playlist
api.MapPut("/{id:int}/add", (int id, AddSongRequest req, ClaimsPrincipal user, PlaylistService svc) =>
{
    req = req with { UserId = CurrentUserId(user) };
    if (string.IsNullOrWhiteSpace(req.Title) || string.IsNullOrWhiteSpace(req.Artist))
        return Results.BadRequest(new { error = "Song title and artist are required." });
    if (req.DurationSeconds < 0)
        return Results.BadRequest(new { error = "Duration cannot be negative." });

    return svc.AddSong(id, req, user.IsInRole("Admin")).ToHttp();
});

// DELETE /api/playlists/{id}/songs/{songId} : remove a song (extra, supports "get removed")
api.MapDelete("/{id:int}/songs/{songId:int}", (int id, int songId, ClaimsPrincipal user, PlaylistService svc) =>
    svc.RemoveSong(id, songId, CurrentUserId(user), user.IsInRole("Admin")).ToHttp());

// PUT /api/playlists/{id}/invite : invite another user to collaborate
api.MapPut("/{id:int}/invite", async (int id, InviteRequest req, HttpContext context, PlaylistService svc, IHttpClientFactory clients) =>
{
    req = req with { InvitedBy = CurrentUserId(context.User) };
    if (req.InvitedUserId <= 0)
        return Results.BadRequest(new { error = "A valid InvitedUserId is required." });

    try
    {
        using var lookup = new HttpRequestMessage(HttpMethod.Get, $"api/users/{req.InvitedUserId}");
        lookup.Headers.Add("Authorization", context.Request.Headers.Authorization.ToString());
        using var response = await clients.CreateClient("Events").SendAsync(lookup);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.BadRequest(new { error = "Select a registered user to invite." });
        if (!response.IsSuccessStatusCode)
            return Results.Json(new { error = "Could not verify the user with Events. Please try again." }, statusCode: 503);
        var invitedUser = await response.Content.ReadFromJsonAsync<JsonElement>();
        if (invitedUser.GetProperty("role").GetString() != "User")
            return Results.BadRequest(new { error = "Only regular users can be invited to playlists." });
    }
    catch (Exception error) when (error is HttpRequestException or TaskCanceledException)
    {
        return Results.Json(new { error = "Events is unavailable. Start it, then try inviting again." }, statusCode: 503);
    }

    return svc.Invite(id, req, context.User.IsInRole("Admin")).ToHttp();
});

// POST /api/playlists/{id}/vote : vote on a song
api.MapPost("/{id:int}/vote", (int id, VoteRequest req, ClaimsPrincipal user, PlaylistService svc) =>
    svc.Vote(id, req with { UserId = CurrentUserId(user) }).ToHttp());

// GET /api/playlists/{id}/rankings : songs ranked by votes
api.MapGet("/{id:int}/rankings", (int id, ClaimsPrincipal user, PlaylistService svc) =>
{
    var playlist = svc.Get(id);
    if (playlist != null && !user.IsInRole("Admin") && !playlist.CanEdit(CurrentUserId(user))) return Results.Forbid();
    var rankings = svc.GetRankings(id);
    return rankings is null
        ? Results.NotFound(new { error = "Playlist not found." })
        : Results.Ok(rankings);
});

// GET /api/playlists/history : log of executed commands (shows the Command Pattern)
api.MapGet("/history", (PlaylistService svc) => Results.Ok(svc.GetHistory()));

app.MapControllers();

app.Run();


// =====================================================================
// REQUEST DTOs
// =====================================================================
public record CreatePlaylistRequest(string Name, int CreatedBy, bool IsCollaborative);
public record AddSongRequest(int UserId, string Title, string Artist, string Genre, int DurationSeconds);
public record InviteRequest(int InvitedBy, int InvitedUserId);
public record VoteRequest(int UserId, int SongId);

// =====================================================================
// RESPONSE DTOs
// =====================================================================
public record SongDto(int Id, string Title, string Artist, string Genre, int Duration, int Votes);
public record PlaylistDto(
    int Id, string Name, int CreatedBy, bool IsCollaborative,
    List<int> Songs, List<SongDto> SongDetails, List<int> Collaborators);
public record RankingDto(int Rank, int SongId, string Title, string Artist, string Genre, int Votes);

// =====================================================================
// DOMAIN MODEL  (abstraction + inheritance + encapsulation)
// =====================================================================

/// <summary>Abstract base for anything playable. Shared state is encapsulated.</summary>
public abstract class MediaItem
{
    public int Id { get; }
    public string Title { get; }
    public string Artist { get; }

    protected MediaItem(int id, string title, string artist)
    {
        Id = id;
        Title = title;
        Artist = artist;
    }

    public abstract string Describe();
}

/// <summary>A song. Votes are private; the only way to change them is Vote()/RemoveVote().</summary>
public class Song : MediaItem
{
    private readonly HashSet<int> _voters = new();

    public string Genre { get; }
    public int Duration { get; }          // seconds
    public int Votes => _voters.Count;

    public Song(int id, string title, string artist, string genre, int duration)
        : base(id, title, artist)
    {
        Genre = genre;
        Duration = duration;
    }

    public bool HasVoted(int userId) => _voters.Contains(userId);
    public bool Vote(int userId) => _voters.Add(userId);          // false if already voted
    public bool RemoveVote(int userId) => _voters.Remove(userId); // used by Undo

    public override string Describe() => $"{Title} by {Artist} ({Genre})";

    public SongDto ToDto() => new(Id, Title, Artist, Genre, Duration, Votes);
}

/// <summary>A standard playlist owned by one user.</summary>
public class Playlist
{
    private readonly List<Song> _songs = new();

    public int Id { get; }
    public string Name { get; }
    public int CreatedBy { get; }
    public virtual bool IsCollaborative => false;

    public IReadOnlyList<Song> Songs => _songs;

    public Playlist(int id, string name, int createdBy)
    {
        Id = id;
        Name = name;
        CreatedBy = createdBy;
    }

    // Polymorphism: collaborative playlists override this rule.
    public virtual bool CanEdit(int userId) => userId == CreatedBy;
    public virtual bool CanVote(int userId) => false;
    public virtual IReadOnlyList<int> Collaborators => Array.Empty<int>();

    public void AddSong(Song song) => _songs.Add(song);
    public bool RemoveSong(Song song) => _songs.Remove(song);
    public Song? FindSong(int songId) => _songs.FirstOrDefault(s => s.Id == songId);

    public PlaylistDto ToDto() => new(
        Id, Name, CreatedBy, IsCollaborative,
        _songs.Select(s => s.Id).ToList(),
        _songs.Select(s => s.ToDto()).ToList(),
        Collaborators.ToList());
}

/// <summary>Collaborative playlist: inherits Playlist, adds collaborators and voting.</summary>
public class CollaborativePlaylist : Playlist
{
    private readonly List<int> _collaborators = new();

    public CollaborativePlaylist(int id, string name, int createdBy) : base(id, name, createdBy) { }

    public override bool IsCollaborative => true;
    public override IReadOnlyList<int> Collaborators => _collaborators;

    public bool IsMember(int userId) => userId == CreatedBy || _collaborators.Contains(userId);

    public override bool CanEdit(int userId) => IsMember(userId);
    public override bool CanVote(int userId) => IsMember(userId);

    public bool AddCollaborator(int userId)
    {
        if (IsMember(userId)) return false;
        _collaborators.Add(userId);
        return true;
    }
}

// =====================================================================
// COMMAND PATTERN
// =====================================================================
public record CommandResult(bool Success, int Status, string Message, object? Data = null)
{
    public static CommandResult Ok(string msg, object? data = null) => new(true, 200, msg, data);
    public static CommandResult Created(string msg, object? data = null) => new(true, 201, msg, data);
    public static CommandResult Fail(int status, string msg) => new(false, status, msg);

    public IResult ToHttp() => Success
        ? Results.Json(new { message = Message, data = Data }, statusCode: Status)
        : Results.Json(new { error = Message }, statusCode: Status);
}

public interface IPlaylistCommand
{
    string Name { get; }
    CommandResult Execute();
    void Undo();
}

public class AddSongCommand : IPlaylistCommand
{
    private readonly Playlist _playlist;
    private readonly Song _song;
    private readonly int _userId;
    private readonly bool _isAdmin;

    public string Name => $"AddSong '{_song.Title}' to playlist {_playlist.Id}";

    public AddSongCommand(Playlist playlist, Song song, int userId, bool isAdmin = false)
    {
        _playlist = playlist; _song = song; _userId = userId; _isAdmin = isAdmin;
    }

    public CommandResult Execute()
    {
        if (!_isAdmin && !_playlist.CanEdit(_userId))
            return CommandResult.Fail(403, "You do not have permission to add songs to this playlist.");

        _playlist.AddSong(_song);
        return CommandResult.Created("Song added.", _song.ToDto());
    }

    public void Undo() => _playlist.RemoveSong(_song);
}

public class RemoveSongCommand : IPlaylistCommand
{
    private readonly Playlist _playlist;
    private readonly int _songId;
    private readonly int _userId;
    private readonly bool _isAdmin;
    private Song? _removed;

    public string Name => $"RemoveSong {_songId} from playlist {_playlist.Id}";

    public RemoveSongCommand(Playlist playlist, int songId, int userId, bool isAdmin = false)
    {
        _playlist = playlist; _songId = songId; _userId = userId; _isAdmin = isAdmin;
    }

    public CommandResult Execute()
    {
        if (!_isAdmin && !_playlist.CanEdit(_userId))
            return CommandResult.Fail(403, "You do not have permission to remove songs from this playlist.");

        _removed = _playlist.FindSong(_songId);
        if (_removed is null)
            return CommandResult.Fail(404, "Song not found in this playlist.");

        _playlist.RemoveSong(_removed);
        return CommandResult.Ok("Song removed.");
    }

    public void Undo()
    {
        if (_removed is not null) _playlist.AddSong(_removed);
    }
}

public class VoteSongCommand : IPlaylistCommand
{
    private readonly Playlist _playlist;
    private readonly int _songId;
    private readonly int _userId;
    private Song? _song;

    public string Name => $"Vote on song {_songId} in playlist {_playlist.Id} by user {_userId}";

    public VoteSongCommand(Playlist playlist, int songId, int userId)
    {
        _playlist = playlist; _songId = songId; _userId = userId;
    }

    public CommandResult Execute()
    {
        if (!_playlist.IsCollaborative)
            return CommandResult.Fail(400, "Voting is only available on collaborative playlists.");
        if (!_playlist.CanVote(_userId))
            return CommandResult.Fail(403, "Only the owner or invited collaborators can vote.");

        _song = _playlist.FindSong(_songId);
        if (_song is null)
            return CommandResult.Fail(404, "Song not found in this playlist.");

        if (!_song.Vote(_userId))
            return CommandResult.Fail(409, "You have already voted for this song.");

        return CommandResult.Ok("Vote recorded.", _song.ToDto());
    }

    public void Undo() => _song?.RemoveVote(_userId);
}

/// <summary>Invoker: runs commands and keeps a history log.</summary>
public class CommandInvoker
{
    private readonly List<(DateTime Time, string Command, bool Success)> _history = new();
    private readonly Stack<IPlaylistCommand> _undoStack = new();

    public CommandResult Run(IPlaylistCommand command)
    {
        var result = command.Execute();
        _history.Add((DateTime.UtcNow, command.Name, result.Success));
        if (result.Success) _undoStack.Push(command);
        return result;
    }

    public bool UndoLast()
    {
        if (!_undoStack.TryPop(out var cmd)) return false;
        cmd.Undo();
        _history.Add((DateTime.UtcNow, "UNDO: " + cmd.Name, true));
        return true;
    }

    public IEnumerable<object> History() =>
        _history.Select(h => new { time = h.Time, command = h.Command, success = h.Success });
}

// =====================================================================
// SERVICE (in-memory store, thread-safe)
// =====================================================================
public class PlaylistService
{
    private readonly Dictionary<int, Playlist> _playlists = new();
    private readonly CommandInvoker _invoker = new();
    private readonly object _lock = new();
    private int _nextPlaylistId = 1;
    private int _nextSongId = 1;

    public Playlist Create(string name, int createdBy, bool isCollaborative)
    {
        lock (_lock)
        {
            var id = _nextPlaylistId++;
            Playlist playlist = isCollaborative
                ? new CollaborativePlaylist(id, name, createdBy)
                : new Playlist(id, name, createdBy);
            _playlists[id] = playlist;
            return playlist;
        }
    }

    public List<Playlist> GetAll(int? userId = null)
    {
        lock (_lock)
        {
            var all = _playlists.Values.AsEnumerable();
            if (userId.HasValue)
                all = all.Where(p => p.CanEdit(userId.Value));
            return all.ToList();
        }
    }

    public Playlist? Get(int id)
    {
        lock (_lock) return _playlists.GetValueOrDefault(id);
    }

    public CommandResult AddSong(int playlistId, AddSongRequest req, bool isAdmin = false)
    {
        lock (_lock)
        {
            if (!_playlists.TryGetValue(playlistId, out var playlist))
                return CommandResult.Fail(404, "Playlist not found.");

            var song = new Song(_nextSongId, req.Title.Trim(), req.Artist.Trim(),
                                req.Genre?.Trim() ?? "", req.DurationSeconds);
            var result = _invoker.Run(new AddSongCommand(playlist, song, req.UserId, isAdmin));
            if (result.Success) _nextSongId++;
            return result;
        }
    }

    public CommandResult RemoveSong(int playlistId, int songId, int userId, bool isAdmin = false)
    {
        lock (_lock)
        {
            if (!_playlists.TryGetValue(playlistId, out var playlist))
                return CommandResult.Fail(404, "Playlist not found.");
            return _invoker.Run(new RemoveSongCommand(playlist, songId, userId, isAdmin));
        }
    }

    public CommandResult Vote(int playlistId, VoteRequest req)
    {
        lock (_lock)
        {
            if (!_playlists.TryGetValue(playlistId, out var playlist))
                return CommandResult.Fail(404, "Playlist not found.");
            return _invoker.Run(new VoteSongCommand(playlist, req.SongId, req.UserId));
        }
    }

    public CommandResult Invite(int playlistId, InviteRequest req, bool isAdmin = false)
    {
        lock (_lock)
        {
            if (!_playlists.TryGetValue(playlistId, out var playlist))
                return CommandResult.Fail(404, "Playlist not found.");
            if (playlist is not CollaborativePlaylist collab)
                return CommandResult.Fail(400, "Only collaborative playlists accept invitations.");
            if (!isAdmin && !collab.IsMember(req.InvitedBy))
                return CommandResult.Fail(403, "Only members can invite others.");
            if (!collab.AddCollaborator(req.InvitedUserId))
                return CommandResult.Fail(409, "User is already part of this playlist.");

            return CommandResult.Ok("User invited.", playlist.ToDto());
        }
    }

    public List<RankingDto>? GetRankings(int playlistId)
    {
        lock (_lock)
        {
            if (!_playlists.TryGetValue(playlistId, out var playlist)) return null;

            var ordered = playlist.Songs
                .OrderByDescending(s => s.Votes)
                .ThenBy(s => s.Title)
                .ToList();

            return ordered
                .Select((s, i) => new RankingDto(i + 1, s.Id, s.Title, s.Artist, s.Genre, s.Votes))
                .ToList();
        }
    }

    public IEnumerable<object> GetHistory()
    {
        lock (_lock) return _invoker.History().ToList();
    }
}
