using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using VibeHive.WinForms.Models;
using VibeHive.WinForms.Services;

namespace VibeHive.WinForms;

public partial class MusicRentalForm : Form
{
    private readonly HttpClient api = new HttpClient
    {
        BaseAddress = new Uri(ClientSettings.MusicRental),
        Timeout = TimeSpan.FromSeconds(10)
    };

    public MusicRentalForm()
    {
        InitializeComponent();
        numYear.Value = DateTime.Now.Year;
        api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserSession.Token);
        numUserId.Maximum = int.MaxValue;
        numUserId.Value = Math.Max(1, UserSession.UserId);
        numUserId.Enabled = false;
        lblUserId.Visible = numUserId.Visible = false;
        txtUserName.Text = UserSession.DisplayUser(UserSession.UserId);
        txtUserName.ReadOnly = true;
        txtUserName.Visible = !UserSession.IsAdmin;
        cmbCustomer.Visible = UserSession.IsAdmin;
        btnAddAlbum.Enabled = UserSession.IsAdmin;
        btnDeleteAlbum.Enabled = UserSession.IsAdmin;
        txtTitle.Enabled = txtArtist.Enabled = txtGenre.Enabled = numYear.Enabled = UserSession.IsAdmin;
    }

    private async void MusicRentalForm_Shown(object? sender, EventArgs e)
    {
        if (DesignMode)
            return;

        tabControlMain.Enabled = false;
        try
        {
            await LoadDataAsync();
        }
        catch (Exception error)
        {
            ShowError(error);
        }
        finally
        {
            if (!IsDisposed)
                tabControlMain.Enabled = true;
        }
    }

    private async Task LoadDataAsync()
    {
        var rentals = await GetDataAsync<List<Rental>>("api/rentals");
        if (IsDisposed)
            return;
        dgvRentals.DataSource = rentals;
        try
        {
            var albums = await GetDataAsync<List<Album>>("api/music?includeUnavailable=true");
            if (IsDisposed)
                return;
            dgvAlbums.DataSource = albums;
            cmbAlbum.DataSource = albums?.Where(album => album.Available).ToList();
        }
        catch
        {
            if (!IsDisposed)
                cmbAlbum.DataSource = null;
            throw;
        }

        if (UserSession.IsAdmin)
        {
            int? selectedUserId = (cmbCustomer.SelectedItem as RegisteredUser)?.Id;
            cmbCustomer.DataSource = null;
            using var eventsApi = new EventsApiClient(ClientSettings.EventsApiBaseUrl);
            try
            {
                var users = await eventsApi.GetUsersAsync();
                if (IsDisposed) return;
                var customers = users.Where(user => user.Role == "User").ToList();
                cmbCustomer.DataSource = customers;
                if (selectedUserId.HasValue && customers.Any(user => user.Id == selectedUserId.Value))
                    cmbCustomer.SelectedItem = customers.First(user => user.Id == selectedUserId.Value);
            }
            catch (Exception error) when (error is HttpRequestException or TaskCanceledException)
            {
                throw new InvalidOperationException("Could not load customers from Events. Start Events, then click Refresh.", error);
            }
        }
    }

    private async void btnAddAlbum_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtArtist.Text)
            || string.IsNullOrWhiteSpace(txtGenre.Text))
        {
            MessageBox.Show(this, "Enter a title, artist, and genre.", "Music Rental");
            return;
        }

        tabControlMain.Enabled = false;
        try
        {
            var album = new Album
            {
                Title = txtTitle.Text.Trim(),
                Artist = txtArtist.Text.Trim(),
                Genre = txtGenre.Text.Trim(),
                Year = (int)numYear.Value
            };
            using var response = await api.PostAsJsonAsync("api/music", album);
            await CheckResponseAsync(response);
            if (IsDisposed)
                return;

            txtTitle.Clear();
            txtArtist.Clear();
            txtGenre.Clear();
            await LoadDataAsync();
        }
        catch (Exception error)
        {
            ShowError(error);
        }
        finally
        {
            if (!IsDisposed)
                tabControlMain.Enabled = true;
        }
    }

    private async void btnDeleteAlbum_Click(object? sender, EventArgs e)
    {
        var album = dgvAlbums.CurrentRow?.DataBoundItem as Album;
        if (album == null)
        {
            MessageBox.Show(this, "Select an album to delete.", "Music Rental");
            return;
        }

        if (MessageBox.Show(this, $"Delete {album.Title}?", "Music Rental",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        tabControlMain.Enabled = false;
        try
        {
            using var response = await api.DeleteAsync($"api/music/{album.Id}");
            await CheckResponseAsync(response);
            await LoadDataAsync();
        }
        catch (Exception error)
        {
            ShowError(error);
        }
        finally
        {
            if (!IsDisposed)
                tabControlMain.Enabled = true;
        }
    }

    private async void btnRentAlbum_Click(object? sender, EventArgs e)
    {
        var album = cmbAlbum.SelectedItem as Album;
        var customer = UserSession.IsAdmin ? cmbCustomer.SelectedItem as RegisteredUser : null;
        if (album == null || (UserSession.IsAdmin && customer == null))
        {
            MessageBox.Show(this, "Select an available album and, for Admins, a regular customer.", "Music Rental");
            return;
        }

        tabControlMain.Enabled = false;
        try
        {
            var rental = new
            {
                UserId = customer?.Id ?? UserSession.UserId,
                UserName = customer?.Name ?? UserSession.UserName,
                AlbumId = album.Id
            };
            using var response = await api.PostAsJsonAsync("api/rentals", rental);
            await CheckResponseAsync(response);
            await LoadDataAsync();
        }
        catch (Exception error)
        {
            ShowError(error);
        }
        finally
        {
            if (!IsDisposed)
                tabControlMain.Enabled = true;
        }
    }

    private async void btnReturnAlbum_Click(object? sender, EventArgs e)
    {
        var rental = dgvRentals.CurrentRow?.DataBoundItem as Rental;
        if (rental == null)
        {
            MessageBox.Show(this, "Select an active rental to return.", "Music Rental");
            return;
        }

        tabControlMain.Enabled = false;
        try
        {
            using var response = await api.PostAsync($"api/rentals/{rental.Id}/return", null);
            await CheckResponseAsync(response);
            await LoadDataAsync();
        }
        catch (Exception error)
        {
            ShowError(error);
        }
        finally
        {
            if (!IsDisposed)
                tabControlMain.Enabled = true;
        }
    }

    private async void btnRefresh_Click(object? sender, EventArgs e)
    {
        tabControlMain.Enabled = false;
        try
        {
            await LoadDataAsync();
        }
        catch (Exception error)
        {
            ShowError(error);
        }
        finally
        {
            if (!IsDisposed)
                tabControlMain.Enabled = true;
        }
    }

    private async Task<T?> GetDataAsync<T>(string path)
    {
        using var response = await api.GetAsync(path);
        await CheckResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    private static async Task CheckResponseAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            if (response.Content.Headers.ContentType?.MediaType == "application/problem+json")
            {
                using var problem = JsonDocument.Parse(message);
                if (problem.RootElement.TryGetProperty("errors", out var errors))
                    message = string.Join(Environment.NewLine, errors.EnumerateObject()
                        .SelectMany(error => error.Value.EnumerateArray().Select(value => value.GetString())));
                else if (problem.RootElement.TryGetProperty("title", out var title))
                    message = title.GetString() ?? message;
            }
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(message)
                ? response.ReasonPhrase : message.Trim('"'));
        }
    }

    private void ShowError(Exception error)
    {
        if (IsDisposed)
            return;

        string message = error.Message;
        if (error is HttpRequestException || error is TaskCanceledException)
            message = $"Could not reach the Music Rental API. Start it at {ClientSettings.MusicRental}, then click Refresh.";

        MessageBox.Show(this, message, "Music Rental", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void dgvRentals_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (dgvRentals.Columns[e.ColumnIndex].DataPropertyName == "RentalDate" && e.Value is DateTime date)
        {
            e.Value = date.ToLocalTime().ToString("g");
            e.FormattingApplied = true;
        }
    }
}
