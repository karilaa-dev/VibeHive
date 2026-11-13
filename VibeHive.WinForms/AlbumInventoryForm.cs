using Grpc.Core;
using Grpc.Net.Client;
using VibeHive.Inventory;

namespace VibeHive.WinForms;

public partial class AlbumInventoryForm : Form
{
    private readonly GrpcChannel channel = GrpcChannel.ForAddress(ClientSettings.AlbumInventory);
    private readonly AlbumInventory.AlbumInventoryClient client;
    private readonly Metadata authHeaders = new() { { "authorization", $"Bearer {UserSession.Token}" } };
    private bool updatingControls;
    private int selectedId;

    public AlbumInventoryForm()
    {
        client = new AlbumInventory.AlbumInventoryClient(channel);
        InitializeComponent();
        ClearEditor();
        btnAdd.Enabled = btnUpdate.Enabled = btnDelete.Enabled = UserSession.IsAdmin;
        txtTitle.Enabled = txtArtist.Enabled = txtGenre.Enabled = numYear.Enabled = chkAvailable.Enabled = UserSession.IsAdmin;
    }

    private async void AlbumInventoryForm_Shown(object? sender, EventArgs e)
    {
        if (!DesignMode)
            await RefreshAlbumsAsync();
    }

    private async Task LoadAlbumsAsync(int? selectId = null)
    {
        if (IsDisposed)
            return;

        var albums = new List<Album>();
        using var call = client.ListAlbums(new ListAlbumsRequest(), deadline: Deadline());
        await foreach (var album in call.ResponseStream.ReadAllAsync())
        {
            albums.Add(album);
        }
        if (IsDisposed)
            return;

        int id = selectId ?? selectedId;
        DataGridViewRow? selectedRow = null;
        updatingControls = true;
        try
        {
            dgvAlbums.DataSource = albums;
            dgvAlbums.ClearSelection();
            dgvAlbums.CurrentCell = null;
            selectedRow = dgvAlbums.Rows.Cast<DataGridViewRow>()
                .FirstOrDefault(row => ((Album)row.DataBoundItem).Id == id);
            if (selectedRow != null)
            {
                dgvAlbums.CurrentCell = selectedRow.Cells[0];
                selectedRow.Selected = true;
            }
        }
        finally
        {
            updatingControls = false;
        }

        if (selectedRow == null)
            ClearEditor();
        else
            ShowAlbum((Album)selectedRow.DataBoundItem);
    }

    private void dgvAlbums_SelectionChanged(object? sender, EventArgs e)
    {
        if (!updatingControls && dgvAlbums.CurrentRow?.DataBoundItem is Album album)
            ShowAlbum(album);
    }

    private void ShowAlbum(Album album)
    {
        if (IsDisposed)
            return;

        selectedId = album.Id;
        lblEditor.Text = $"Selected album #{album.Id}";
        txtTitle.Text = album.Title;
        txtArtist.Text = album.Artist;
        txtGenre.Text = album.Genre;
        numYear.Value = album.Year;
        chkAvailable.Checked = album.Available;
        numLookupId.Value = album.Id;
    }

    private void ClearEditor()
    {
        if (IsDisposed)
            return;

        updatingControls = true;
        selectedId = 0;
        lblEditor.Text = "New album";
        txtTitle.Clear();
        txtArtist.Clear();
        txtGenre.Clear();
        numYear.Value = DateTime.Now.Year;
        chkAvailable.Checked = true;
        dgvAlbums.ClearSelection();
        dgvAlbums.CurrentCell = null;
        updatingControls = false;
    }

    private Album ReadEditor()
    {
        return new Album
        {
            Id = selectedId,
            Title = txtTitle.Text.Trim(),
            Artist = txtArtist.Text.Trim(),
            Genre = txtGenre.Text.Trim(),
            Year = (int)numYear.Value,
            Available = chkAvailable.Checked
        };
    }

    private void btnNew_Click(object? sender, EventArgs e)
    {
        ClearEditor();
    }

    private async void btnAdd_Click(object? sender, EventArgs e)
    {
        workPanel.Enabled = false;
        try
        {
            var album = await client.AddAlbumAsync(ReadEditor(), headers: authHeaders, deadline: Deadline());
            await LoadAlbumsAsync(album.Id);
        }
        catch (Exception error)
        {
            ShowError(error);
        }
        finally
        {
            if (!IsDisposed)
                workPanel.Enabled = true;
        }
    }

    private async void btnUpdate_Click(object? sender, EventArgs e)
    {
        if (selectedId == 0)
        {
            MessageBox.Show(this, "Select an album to update.", "Album Inventory");
            return;
        }

        workPanel.Enabled = false;
        try
        {
            var album = await client.UpdateAlbumAsync(ReadEditor(), headers: authHeaders, deadline: Deadline());
            await LoadAlbumsAsync(album.Id);
        }
        catch (Exception error)
        {
            ShowError(error);
        }
        finally
        {
            if (!IsDisposed)
                workPanel.Enabled = true;
        }
    }

    private async void btnDelete_Click(object? sender, EventArgs e)
    {
        if (selectedId == 0)
        {
            MessageBox.Show(this, "Select an album to delete.", "Album Inventory");
            return;
        }
        int id = selectedId;
        if (MessageBox.Show(this, $"Delete album #{id}: {txtTitle.Text}?", "Album Inventory",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        workPanel.Enabled = false;
        try
        {
            await client.DeleteAlbumAsync(new AlbumRequest { Id = id }, headers: authHeaders, deadline: Deadline());
            ClearEditor();
            await LoadAlbumsAsync();
        }
        catch (Exception error)
        {
            ShowError(error);
        }
        finally
        {
            if (!IsDisposed)
                workPanel.Enabled = true;
        }
    }

    private async void btnFind_Click(object? sender, EventArgs e)
    {
        workPanel.Enabled = false;
        try
        {
            var album = await client.GetAlbumAsync(new AlbumRequest { Id = (int)numLookupId.Value },
                deadline: Deadline());
            ShowAlbum(album);
            await LoadAlbumsAsync(album.Id);
        }
        catch (Exception error)
        {
            ShowError(error);
        }
        finally
        {
            if (!IsDisposed)
                workPanel.Enabled = true;
        }
    }

    private async void btnRefresh_Click(object? sender, EventArgs e)
    {
        await RefreshAlbumsAsync();
    }

    private async Task RefreshAlbumsAsync()
    {
        workPanel.Enabled = false;
        try
        {
            await LoadAlbumsAsync();
        }
        catch (Exception error)
        {
            ShowError(error);
        }
        finally
        {
            if (!IsDisposed)
                workPanel.Enabled = true;
        }
    }

    private void ShowError(Exception error)
    {
        if (IsDisposed)
            return;

        string message = error.Message;
        if (error is RpcException rpcError)
        {
            message = rpcError.Status.Detail;
            if (rpcError.StatusCode is StatusCode.Unavailable or StatusCode.DeadlineExceeded or StatusCode.Cancelled)
                message = $"Could not reach Album Inventory. Start it at {ClientSettings.AlbumInventory}, then click Refresh.";
        }
        MessageBox.Show(this, message, "Album Inventory", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private static DateTime Deadline() => DateTime.UtcNow.AddSeconds(5);
}
