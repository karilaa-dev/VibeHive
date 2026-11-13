using Grpc.Core;
using Grpc.Net.Client;
using VibeHive.Inventory;

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
    private static bool exampleDataLoaded;
    private bool checkingApiStatus;
    public bool SignOutRequested { get; private set; }

    public MainForm()
    {
        inventoryClient = new AlbumInventory.AlbumInventoryClient(inventoryChannel);
        InitializeComponent();
        sessionLabel.Text = $"Signed in as {UserSession.DisplayUser(UserSession.UserId)} [{UserSession.Role}]";
        albumInventoryButton.Enabled = UserSession.IsAdmin;
        playlistBuilderButton.Enabled = UserSession.CanUsePlaylists;
        loadExampleDataButton.Enabled = UserSession.IsAdmin && !exampleDataLoaded;
    }

    private async void MainForm_Shown(object? sender, EventArgs e)
    {
        if (DesignMode)
            return;

        apiStatusLabel.ToolTipText = $"{ClientSettings.MusicRental} checked every 5 seconds";
        inventoryStatusLabel.ToolTipText = $"{ClientSettings.AlbumInventory} checked every 5 seconds";
        playlistStatusLabel.ToolTipText = $"{ClientSettings.Playlist} checked every 5 seconds";
        eventsStatusLabel.ToolTipText = $"{ClientSettings.EventsApiBaseUrl} checked every 5 seconds";
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
            await Task.WhenAll(
                CheckRestStatusAsync(ClientSettings.MusicRental, apiStatusLabel, "Music Rental"),
                CheckRestStatusAsync(ClientSettings.Playlist, playlistStatusLabel, "Playlists"),
                CheckRestStatusAsync(ClientSettings.EventsApiBaseUrl, eventsStatusLabel, "Events"),
                CheckInventoryStatusAsync());
        }
        finally
        {
            checkingApiStatus = false;
        }
    }

    private async Task CheckRestStatusAsync(string address, ToolStripStatusLabel label, string service)
    {
        try
        {
            using var response = await musicRentalApi.GetAsync(address + "health", HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
                ShowApiStatus(label, service, "Online", Color.ForestGreen);
            else
                ShowApiStatus(label, service, $"Error ({(int)response.StatusCode})", Color.DarkOrange);
        }
        catch (HttpRequestException)
        {
            ShowApiStatus(label, service, "Offline", Color.Firebrick);
        }
        catch (TaskCanceledException)
        {
            ShowApiStatus(label, service, "Offline", Color.Firebrick);
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

    private void musicRentalButton_Click(object? sender, EventArgs e)
    {
        using var form = new MusicRentalForm();
        ShowSubMenu(form);
    }

    private void logoutButton_Click(object? sender, EventArgs e)
    {
        SignOutRequested = true;
        UserSession.Clear();
        Close();
    }

    private void playlistBuilderButton_Click(object? sender, EventArgs e)
    {
        if (!UserSession.CanUsePlaylists) return;
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
        if (!UserSession.IsAdmin) return;
        using var form = new AlbumInventoryForm();
        ShowSubMenu(form);
    }

    private async void loadExampleDataButton_Click(object? sender, EventArgs e)
    {
        if (!UserSession.IsAdmin || exampleDataLoaded) return;
        menuPanel.Enabled = false;
        try
        {
            string message = await ExampleData.LoadAsync();
            exampleDataLoaded = true;
            if (!IsDisposed)
                MessageBox.Show(this, message, "Example Data");
        }
        catch (Exception error)
        {
            if (!IsDisposed)
            {
                string message = error.Message;
                if (error is HttpRequestException || error is TaskCanceledException)
                    message = "Could not reach a required service. Start all four services, then try again.";
                MessageBox.Show(this, message, "Example Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        finally
        {
            if (!IsDisposed)
            {
                menuPanel.Enabled = true;
                loadExampleDataButton.Enabled = UserSession.IsAdmin && !exampleDataLoaded;
            }
        }
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
