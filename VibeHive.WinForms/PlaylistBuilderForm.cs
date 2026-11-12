using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace VibeHive.WinForms;

public partial class PlaylistBuilderForm : Form
{
    private static readonly HttpClient Http = new()
    {
        BaseAddress = new Uri(ClientSettings.Playlist)
    };
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    // ---- Controls ----
    private readonly NumericUpDown numUser = new() { Minimum = 1, Maximum = 100000, Value = 1, Width = 70 };

    private readonly TextBox txtName = new() { Width = 265 };
    private readonly CheckBox chkCollab = new() { Text = "Collaborative", AutoSize = true };
    private readonly Button btnCreate = new() { Text = "Create", Width = 90, Height = 26 };

    private readonly ListBox lstPlaylists = new() { Dock = DockStyle.Fill, IntegralHeight = false };
    private readonly Button btnRefresh = new() { Text = "Refresh list", Dock = DockStyle.Fill };

    private readonly TextBox txtTitle = new() { Width = 200 };
    private readonly TextBox txtArtist = new() { Width = 170 };
    private readonly TextBox txtGenre = new() { Width = 120 };
    private readonly NumericUpDown numDuration = new() { Minimum = 0, Maximum = 36000, Value = 180, Width = 70 };
    private readonly Button btnAddSong = new() { Text = "Add song", Width = 100, Height = 26 };

    private readonly NumericUpDown numInvite = new() { Minimum = 1, Maximum = 100000, Value = 2, Width = 70 };
    private readonly Button btnInvite = new() { Text = "Invite", Width = 90, Height = 26 };
    private readonly Label lblMembers = new() { AutoSize = true, Text = "No playlist selected" };

    private readonly TextBox txtSearch = new() { Width = 200 };
    private readonly Button btnVote = new() { Text = "Vote for selected song", AutoSize = true };
    private readonly Button btnRemove = new() { Text = "Remove selected song", AutoSize = true };
    private readonly DataGridView gridSongs = new();

    private readonly CheckBox chkAuto = new() { Text = "Auto-refresh every 4 seconds", AutoSize = true, Checked = true };
    private readonly DataGridView gridRankings = new();

    private readonly Label lblStatus = new()
    {
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(6, 0, 0, 0),
        Text = "Ready."
    };

    private readonly System.Windows.Forms.Timer tmr = new() { Interval = 4000 };

    // ---- State ----
    private PlaylistModel? _detail;     // latest data for the selected playlist
    private bool _suppress;             // blocks SelectedIndexChanged while we rebind the list
    private bool _busy;                 // stops overlapping timer refreshes

    private PlaylistModel? Current => lstPlaylists.SelectedItem as PlaylistModel;
    private int UserId => (int)numUser.Value;

    public PlaylistBuilderForm()
    {
        InitializeComponent();
        BuildUi();
        WireEvents();
    }

    // =====================================================================
    // UI
    // =====================================================================
    private void BuildUi()
    {
        Text = "VibeHive - Collaborative Playlist Builder";
        ClientSize = new Size(1150, 720);
        MinimumSize = new Size(1000, 640);

        // ----- Left column -----
        var gbUser = Group("Current user");
        gbUser.Controls.Add(Lbl("User ID:", 10, 28));
        numUser.Location = new Point(75, 24);
        gbUser.Controls.Add(numUser);

        var gbCreate = Group("Create playlist");
        gbCreate.Controls.Add(Lbl("Name:", 10, 22));
        txtName.Location = new Point(10, 40);
        chkCollab.Location = new Point(10, 72);
        btnCreate.Location = new Point(185, 68);
        gbCreate.Controls.AddRange(new Control[] { txtName, chkCollab, btnCreate });

        var gbPlaylists = Group("Playlists");
        var plTable = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        plTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        plTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        plTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        plTable.Controls.Add(lstPlaylists, 0, 0);
        plTable.Controls.Add(btnRefresh, 0, 1);
        gbPlaylists.Controls.Add(plTable);

        var left = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        left.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        left.Controls.Add(gbUser, 0, 0);
        left.Controls.Add(gbCreate, 0, 1);
        left.Controls.Add(gbPlaylists, 0, 2);

        // ----- Right column -----
        var gbAdd = Group("Add song to selected playlist");
        gbAdd.Controls.Add(Lbl("Title:", 10, 18));
        gbAdd.Controls.Add(Lbl("Artist:", 220, 18));
        gbAdd.Controls.Add(Lbl("Genre:", 400, 18));
        gbAdd.Controls.Add(Lbl("Duration (sec):", 530, 18));
        txtTitle.Location = new Point(10, 38);
        txtArtist.Location = new Point(220, 38);
        txtGenre.Location = new Point(400, 38);
        numDuration.Location = new Point(530, 38);
        btnAddSong.Location = new Point(615, 36);
        gbAdd.Controls.AddRange(new Control[] { txtTitle, txtArtist, txtGenre, numDuration, btnAddSong });

        var gbCollab = Group("Collaboration");
        gbCollab.Controls.Add(Lbl("Invite user ID:", 10, 28));
        numInvite.Location = new Point(105, 24);
        btnInvite.Location = new Point(185, 22);
        lblMembers.Location = new Point(290, 28);
        gbCollab.Controls.AddRange(new Control[] { numInvite, btnInvite, lblMembers });

        var gbSongs = Group("Songs in playlist");
        ConfigureGrid(gridSongs);
        var songBar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
        songBar.Controls.Add(new Label { Text = "Search:", AutoSize = true, Margin = new Padding(3, 8, 3, 0) });
        songBar.Controls.Add(txtSearch);
        songBar.Controls.Add(btnVote);
        songBar.Controls.Add(btnRemove);
        gbSongs.Controls.Add(TwoRows(songBar, gridSongs));

        var gbRank = Group("Rankings - Top Playlist");
        ConfigureGrid(gridRankings);
        var rankBar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
        rankBar.Controls.Add(chkAuto);
        gbRank.Controls.Add(TwoRows(rankBar, gridRankings));

        var right = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
        right.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
        right.Controls.Add(gbAdd, 0, 0);
        right.Controls.Add(gbCollab, 0, 1);
        right.Controls.Add(gbSongs, 0, 2);
        right.Controls.Add(gbRank, 0, 3);

        // ----- Root + status bar -----
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.Controls.Add(left, 0, 0);
        root.Controls.Add(right, 1, 0);

        var outer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        outer.Controls.Add(root, 0, 0);
        outer.Controls.Add(lblStatus, 0, 1);

        Controls.Add(outer);
    }

    private static GroupBox Group(string text) => new() { Text = text, Dock = DockStyle.Fill };

    private static Label Lbl(string text, int x, int y) =>
        new() { Text = text, Left = x, Top = y, AutoSize = true };

    private static TableLayoutPanel TwoRows(Control top, Control fill)
    {
        var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        t.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        t.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        t.Controls.Add(top, 0, 0);
        t.Controls.Add(fill, 0, 1);
        return t;
    }

    private static void ConfigureGrid(DataGridView g)
    {
        g.Dock = DockStyle.Fill;
        g.ReadOnly = true;
        g.AllowUserToAddRows = false;
        g.AllowUserToDeleteRows = false;
        g.MultiSelect = false;
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        g.RowHeadersVisible = false;
        g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        g.BackgroundColor = SystemColors.Window;
    }

    private void WireEvents()
    {
        Load += async (_, _) =>
        {
            await Safe(() => LoadPlaylistsAsync());
            tmr.Start();
        };
        FormClosing += (_, _) => tmr.Stop();

        btnCreate.Click += async (_, _) => await Safe(CreateAsync);
        btnRefresh.Click += async (_, _) => await Safe(() => LoadPlaylistsAsync());
        btnAddSong.Click += async (_, _) => await Safe(AddSongAsync);
        btnInvite.Click += async (_, _) => await Safe(InviteAsync);
        btnVote.Click += async (_, _) => await Safe(VoteAsync);
        btnRemove.Click += async (_, _) => await Safe(RemoveAsync);

        lstPlaylists.SelectedIndexChanged += async (_, _) =>
        {
            if (_suppress) return;
            await Safe(RefreshSelectedAsync);
        };

        txtSearch.TextChanged += (_, _) => BindSongs();

        chkAuto.CheckedChanged += (_, _) => tmr.Enabled = chkAuto.Checked;

        // Timer: refresh votes/songs/rankings every few seconds (real-time feel)
        tmr.Tick += async (_, _) =>
        {
            if (_busy || Current is null) return;
            _busy = true;
            try { await Safe(RefreshSelectedAsync); }
            finally { _busy = false; }
        };
    }

    // =====================================================================
    // ACTIONS
    // =====================================================================
    private async Task LoadPlaylistsAsync(bool selectNewest = false)
    {
        var list = await Http.GetFromJsonAsync<List<PlaylistModel>>("api/playlists", JsonOpts)
                   ?? new List<PlaylistModel>();

        int? keepId = selectNewest && list.Count > 0 ? list.Max(p => p.Id) : Current?.Id;

        _suppress = true;
        lstPlaylists.DataSource = list;
        int idx = keepId.HasValue ? list.FindIndex(p => p.Id == keepId.Value) : -1;
        lstPlaylists.SelectedIndex = idx >= 0 ? idx : (list.Count > 0 ? 0 : -1);
        _suppress = false;

        await RefreshSelectedAsync();
        SetStatus($"Loaded {list.Count} playlist(s).", false);
    }

    private async Task RefreshSelectedAsync()
    {
        var sel = Current;
        if (sel is null)
        {
            _detail = null;
            lblMembers.Text = "No playlist selected";
            BindSongs();
            BindRankings(new List<RankingModel>());
            return;
        }

        var detail = await Http.GetFromJsonAsync<PlaylistModel>($"api/playlists/{sel.Id}", JsonOpts);
        var ranks = await Http.GetFromJsonAsync<List<RankingModel>>($"api/playlists/{sel.Id}/rankings", JsonOpts)
                    ?? new List<RankingModel>();

        if (Current?.Id != sel.Id) return; // user picked another playlist meanwhile

        _detail = detail;
        lblMembers.Text = detail is null ? "" :
            detail.IsCollaborative
                ? $"Owner: #{detail.CreatedBy}   Collaborators: " +
                  (detail.Collaborators.Count == 0 ? "none yet" : string.Join(", ", detail.Collaborators.Select(c => "#" + c)))
                : $"Private playlist (owner #{detail.CreatedBy})";

        BindSongs();
        BindRankings(ranks);
    }

    private async Task CreateAsync()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            SetStatus("Enter a playlist name.", true);
            return;
        }

        var r = await SendAsync(HttpMethod.Post, "api/playlists",
            new { name = txtName.Text.Trim(), createdBy = UserId, isCollaborative = chkCollab.Checked });

        if (r.Ok)
        {
            txtName.Clear();
            await LoadPlaylistsAsync(selectNewest: true);
            SetStatus("Playlist created.", false);
        }
        else SetStatus(r.Message, true);
    }

    private async Task AddSongAsync()
    {
        if (Current is null) { SetStatus("Select a playlist first.", true); return; }
        if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtArtist.Text))
        {
            SetStatus("Song title and artist are required.", true);
            return;
        }

        var r = await SendAsync(HttpMethod.Put, $"api/playlists/{Current.Id}/add", new
        {
            userId = UserId,
            title = txtTitle.Text.Trim(),
            artist = txtArtist.Text.Trim(),
            genre = txtGenre.Text.Trim(),
            durationSeconds = (int)numDuration.Value
        });

        SetStatus(r.Message, !r.Ok);
        if (r.Ok)
        {
            txtTitle.Clear(); txtArtist.Clear(); txtGenre.Clear();
            await RefreshSelectedAsync();
        }
    }

    private async Task InviteAsync()
    {
        if (Current is null) { SetStatus("Select a playlist first.", true); return; }

        var r = await SendAsync(HttpMethod.Put, $"api/playlists/{Current.Id}/invite",
            new { invitedBy = UserId, invitedUserId = (int)numInvite.Value });

        SetStatus(r.Message, !r.Ok);
        if (r.Ok) await RefreshSelectedAsync();
    }

    private async Task VoteAsync()
    {
        if (Current is null) { SetStatus("Select a playlist first.", true); return; }
        var songId = SelectedId(gridSongs, "Id");
        if (songId is null) { SetStatus("Select a song to vote for.", true); return; }

        var r = await SendAsync(HttpMethod.Post, $"api/playlists/{Current.Id}/vote",
            new { userId = UserId, songId = songId.Value });

        SetStatus(r.Message, !r.Ok);
        if (r.Ok) await RefreshSelectedAsync();
    }

    private async Task RemoveAsync()
    {
        if (Current is null) { SetStatus("Select a playlist first.", true); return; }
        var songId = SelectedId(gridSongs, "Id");
        if (songId is null) { SetStatus("Select a song to remove.", true); return; }

        if (MessageBox.Show("Remove the selected song from this playlist?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        var r = await SendAsync(HttpMethod.Delete,
            $"api/playlists/{Current.Id}/songs/{songId.Value}?userId={UserId}", null);

        SetStatus(r.Message, !r.Ok);
        if (r.Ok) await RefreshSelectedAsync();
    }

    // =====================================================================
    // BINDING
    // =====================================================================
    private void BindSongs()
    {
        var q = txtSearch.Text.Trim();
        var songs = (_detail?.SongDetails ?? new List<SongModel>())
            .Where(s => q.Length == 0
                     || s.Title.Contains(q, StringComparison.OrdinalIgnoreCase)
                     || s.Artist.Contains(q, StringComparison.OrdinalIgnoreCase)
                     || s.Genre.Contains(q, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var keep = SelectedId(gridSongs, "Id");
        gridSongs.DataSource = songs;
        HideColumn(gridSongs, "Id");
        RestoreSelection(gridSongs, "Id", keep);
    }

    private void BindRankings(List<RankingModel> ranks)
    {
        var keep = SelectedId(gridRankings, "SongId");
        gridRankings.DataSource = ranks;
        HideColumn(gridRankings, "SongId");
        RestoreSelection(gridRankings, "SongId", keep);
    }

    private static int? SelectedId(DataGridView g, string column)
    {
        if (g.CurrentRow is null || !g.Columns.Contains(column)) return null;
        return g.CurrentRow.Cells[column].Value is int id ? id : null;
    }

    private static void HideColumn(DataGridView g, string name)
    {
        if (g.Columns.Contains(name)) g.Columns[name].Visible = false;
    }

    private static void RestoreSelection(DataGridView g, string column, int? id)
    {
        if (id is null || !g.Columns.Contains(column)) return;
        foreach (DataGridViewRow row in g.Rows)
        {
            if (row.Cells[column].Value is int v && v == id)
            {
                var cell = row.Cells.Cast<DataGridViewCell>().FirstOrDefault(c => c.Visible);
                if (cell is not null) g.CurrentCell = cell;
                row.Selected = true;
                return;
            }
        }
    }

    // =====================================================================
    // HTTP HELPERS
    // =====================================================================
    private record ApiResult(bool Ok, string Message);

    private static async Task<ApiResult> SendAsync(HttpMethod method, string url, object? body)
    {
        using var req = new HttpRequestMessage(method, url);
        if (body is not null)
            req.Content = new StringContent(JsonSerializer.Serialize(body, JsonOpts), Encoding.UTF8, "application/json");

        using var resp = await Http.SendAsync(req);
        var text = await resp.Content.ReadAsStringAsync();
        bool ok = resp.IsSuccessStatusCode;

        return new ApiResult(ok, ParseMessage(text, ok, (int)resp.StatusCode));
    }

    private static string ParseMessage(string body, bool ok, int status)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty(ok ? "message" : "error", out var p) &&
                p.ValueKind == JsonValueKind.String)
                return p.GetString() ?? "";
        }
        catch (JsonException) { /* fall through */ }

        return ok ? "Done." : $"Request failed ({status}).";
    }

    private async Task Safe(Func<Task> action)
    {
        try { await action(); }
        catch (HttpRequestException)
        {
            SetStatus("Cannot reach the API. Is VibeHive.Playlist.Api running, and is ClientSettings.Playlist correct?", true);
        }
        catch (TaskCanceledException) { SetStatus("The API request timed out.", true); }
        catch (JsonException) { SetStatus("Unexpected response from the API.", true); }
    }

    private void SetStatus(string message, bool isError)
    {
        lblStatus.ForeColor = isError ? Color.Firebrick : Color.DarkGreen;
        lblStatus.Text = message;
    }
}

// =====================================================================
// Client-side models (match the API's JSON)
// =====================================================================
public record SongModel(int Id, string Title, string Artist, string Genre, int Duration, int Votes);

public record PlaylistModel(
    int Id, string Name, int CreatedBy, bool IsCollaborative,
    List<int> Songs, List<SongModel> SongDetails, List<int> Collaborators)
{
    public override string ToString() =>
        $"{Name}{(IsCollaborative ? " (collab)" : "")}  -  owner #{CreatedBy}";
}

public record RankingModel(int Rank, int SongId, string Title, string Artist, string Genre, int Votes);