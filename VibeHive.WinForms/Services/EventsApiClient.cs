using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace VibeHive.WinForms.Services;

public class EventsApiClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public EventsApiClient(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl), Timeout = TimeSpan.FromSeconds(10) };
    }

    private void SetAuthHeader()
    {
        _http.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(UserSession.Token)
            ? new AuthenticationHeaderValue("Bearer", UserSession.Token)
            : null;
    }

    public async Task<bool> RegisterAsync(string name, string email, string password, string role)
    {
        var body = JsonSerializer.Serialize(new { name, email, password, role });
        var res = await _http.PostAsync("/api/users/register", new StringContent(body, Encoding.UTF8, "application/json"));
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var body = JsonSerializer.Serialize(new { email, password });
        var res = await _http.PostAsync("/api/auth/login", new StringContent(body, Encoding.UTF8, "application/json"));
        if (!res.IsSuccessStatusCode) return false;

        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        UserSession.Token = doc.RootElement.GetProperty("token").GetString();
        UserSession.UserId = doc.RootElement.GetProperty("userId").GetInt32();
        UserSession.UserName = doc.RootElement.GetProperty("name").GetString();
        UserSession.Role = doc.RootElement.GetProperty("role").GetString();
        return true;
    }

    public async Task<string> GetEventsJsonAsync()
    {
        SetAuthHeader();
        return await _http.GetStringAsync("/api/events");
    }

    public async Task<List<RegisteredUser>> GetUsersAsync()
    {
        SetAuthHeader();
        var users = await _http.GetFromJsonAsync<List<RegisteredUser>>("api/users") ?? new();
        UserSession.UserNames.Clear();
        foreach (var user in users) UserSession.UserNames[user.Id] = user.Name;
        return users;
    }

    public void Dispose() => _http.Dispose();

    public async Task<bool> CreateEventAsync(string name, DateTime date, string venue, string genre, int tickets)
    {
        SetAuthHeader();
        var body = JsonSerializer.Serialize(new { name, date, venue, genre, availableTickets = tickets });
        var res = await _http.PostAsync("/api/events", new StringContent(body, Encoding.UTF8, "application/json"));
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> BookTicketAsync(int eventId)
    {
        SetAuthHeader();
        var body = JsonSerializer.Serialize(new { eventId });
        var res = await _http.PostAsync("/api/tickets", new StringContent(body, Encoding.UTF8, "application/json"));
        return res.IsSuccessStatusCode;
    }

    public async Task<string> GetUserTicketsJsonAsync()
    {
        SetAuthHeader();
        return await _http.GetStringAsync("/api/tickets");
    }

    public async Task<bool> CancelTicketAsync(int ticketId)
    {
        SetAuthHeader();
        var body = JsonSerializer.Serialize(new { status = "Cancelled" });
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var res = await _http.PatchAsync($"/api/tickets/{ticketId}/status", content);
        return res.IsSuccessStatusCode;
    }
}

public record RegisteredUser(int Id, string Name, string Role)
{
    public override string ToString() => UserSession.DisplayUser(Id, Name);
}
