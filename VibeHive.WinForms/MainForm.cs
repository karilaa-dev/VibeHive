using System.Net.Http.Json;
using Grpc.Core;
using Grpc.Net.Client;
using VibeHive.Inventory;
using Album = VibeHive.WinForms.Models.Album;
using Rental = VibeHive.WinForms.Models.Rental;

namespace VibeHive.WinForms;

public partial class MainForm : Form
{
    private readonly HttpClient musicRentalApi = new HttpClient
    {
        BaseAddress = new Uri(ClientSettings.MusicRental),
        Timeout = TimeSpan.FromSeconds(3)
    };
    private readonly GrpcChannel inventoryChannel = GrpcChannel.ForAddress(ClientSettings.AlbumInventory);
    private readonly AlbumInventory.AlbumInventoryClient inventoryClient;
    private bool checkingApiStatus;

    public MainForm()
    {
        inventoryClient = new AlbumInventory.AlbumInventoryClient(inventoryChannel);
        InitializeComponent();
    }

    private async void MainForm_Shown(object? sender, EventArgs e)
    {
        if (DesignMode)
            return;

        apiStatusLabel.ToolTipText = $"{ClientSettings.MusicRental} checked every 5 seconds";
        inventoryStatusLabel.ToolTipText = $"{ClientSettings.AlbumInventory} checked every 5 seconds";
        await CheckApiStatusAsync();
        if (!IsDisposed)
            apiStatusTimer.Start();
    }

    private async void apiStatusTimer_Tick(object? sender, EventArgs e)
    {
        await CheckApiStatusAsync();
    }

    private async Task CheckApiStatusAsync()
    {
        if (checkingApiStatus || IsDisposed)
            return;

        checkingApiStatus = true;
        try
        {
            await CheckMusicRentalStatusAsync();
            await CheckInventoryStatusAsync();
        }
        finally
        {
            checkingApiStatus = false;
        }
    }

    private async Task CheckMusicRentalStatusAsync()
    {
        try
        {
            using var response = await musicRentalApi.GetAsync("api/music", HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
                ShowApiStatus(apiStatusLabel, "Music Rental API", "Online", Color.ForestGreen);
            else
                ShowApiStatus(apiStatusLabel, "Music Rental API", $"Error ({(int)response.StatusCode})", Color.DarkOrange);
        }
        catch (HttpRequestException)
        {
            ShowApiStatus(apiStatusLabel, "Music Rental API", "Offline", Color.Firebrick);
        }
        catch (TaskCanceledException)
        {
            ShowApiStatus(apiStatusLabel, "Music Rental API", "Offline", Color.Firebrick);
        }
    }

    private async Task CheckInventoryStatusAsync()
    {
        if (IsDisposed)
            return;

        try
        {
            using var call = inventoryClient.ListAlbums(new ListAlbumsRequest(), deadline: DateTime.UtcNow.AddSeconds(3));
            await foreach (var album in call.ResponseStream.ReadAllAsync())
            {
            }
            ShowApiStatus(inventoryStatusLabel, "Album Inventory", "Online", Color.ForestGreen);
        }
        catch (RpcException)
        {
            ShowApiStatus(inventoryStatusLabel, "Album Inventory", "Offline", Color.Firebrick);
        }
    }

    private void ShowApiStatus(ToolStripStatusLabel label, string service, string status, Color color)
    {
        if (IsDisposed || Disposing)
            return;

        label.Text = $"{service}: {status}";
        label.ForeColor = color;
    }

    private async void loadExampleDataButton_Click(object? sender, EventArgs e)
    {
        menuPanel.Enabled = false;
        try
        {
            string message = await LoadExampleDataAsync();
            if (!IsDisposed)
                MessageBox.Show(this, message, "Example Data");
        }
        catch (Exception error)
        {
            if (!IsDisposed)
            {
                string message = error.Message;
                if (error is HttpRequestException || error is TaskCanceledException)
                    message = "Could not reach Music Rental. Start Music Rental and Album Inventory, then try again.";
                MessageBox.Show(this, message, "Example Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        finally
        {
            if (!IsDisposed)
                menuPanel.Enabled = true;
        }
    }

    private async Task<string> LoadExampleDataAsync()
    {
        using var api = new HttpClient
        {
            BaseAddress = new Uri(ClientSettings.MusicRental),
            Timeout = TimeSpan.FromSeconds(10)
        };
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
        string[] customers = { "Alex Morgan", "Jamie Lee" };
        int addedRentals = 0;
        for (int index = 0; index < customers.Length; index++)
        {
            var album = exampleAlbums[index];
            if (!album.Available || rentals.Any(rental => rental.AlbumId == album.Id))
                continue;

            var request = new { UserId = 1001 + index, UserName = customers[index], AlbumId = album.Id };
            using var response = await api.PostAsJsonAsync("api/rentals", request);
            await CheckExampleResponseAsync(response);
            addedRentals++;
        }

        return $"Added {addedAlbums} example albums and {addedRentals} rentals. "
            + "Existing albums and active rentals were kept. Playlist and Events are still starter APIs.";
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

    private void musicRentalButton_Click(object? sender, EventArgs e)
    {
        using var form = new MusicRentalForm();
        ShowSubMenu(form);
    }

    private void playlistBuilderButton_Click(object? sender, EventArgs e)
    {
        using var form = new PlaylistBuilderForm();
        ShowSubMenu(form);
    }

    private void eventManagementButton_Click(object? sender, EventArgs e)
    {
        using var form = new EventManagementForm();
        ShowSubMenu(form);
    }

    private void albumInventoryButton_Click(object? sender, EventArgs e)
    {
        using var form = new AlbumInventoryForm();
        ShowSubMenu(form);
    }

    private void ShowSubMenu(Form form)
    {
        Hide();
        try
        {
            form.ShowDialog(this);
        }
        finally
        {
            if (!IsDisposed)
            {
                Show();
                Activate();
            }
        }
    }
}
