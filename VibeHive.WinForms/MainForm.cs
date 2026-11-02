namespace VibeHive.WinForms;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void musicRentalButton_Click(object? sender, EventArgs e)
    {
        using var form = new MusicRentalForm();
        form.ShowDialog(this);
    }

    private void playlistBuilderButton_Click(object? sender, EventArgs e)
    {
        using var form = new PlaylistBuilderForm();
        form.ShowDialog(this);
    }

    private void eventManagementButton_Click(object? sender, EventArgs e)
    {
        using var form = new EventManagementForm();
        form.ShowDialog(this);
    }

    private void albumInventoryButton_Click(object? sender, EventArgs e)
    {
        using var form = new AlbumInventoryForm();
        form.ShowDialog(this);
    }
}
