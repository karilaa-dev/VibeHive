namespace VibeHive.WinForms;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed, otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            musicRentalApi.Dispose();
            inventoryChannel.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support. Do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        menuPanel = new Panel();
        musicRentalButton = new Button();
        playlistBuilderButton = new Button();
        eventManagementButton = new Button();
        albumInventoryButton = new Button();
        loadExampleDataButton = new Button();
        apiStatusStrip = new StatusStrip();
        apiStatusLabel = new ToolStripStatusLabel();
        inventoryStatusLabel = new ToolStripStatusLabel();
        apiStatusTimer = new System.Windows.Forms.Timer(components);
        apiStatusStrip.SuspendLayout();
        SuspendLayout();
        //
        // menuPanel
        //
        menuPanel.Dock = DockStyle.Fill;
        menuPanel.Name = "menuPanel";
        menuPanel.Controls.AddRange(new Control[] { musicRentalButton, playlistBuilderButton,
            eventManagementButton, albumInventoryButton, loadExampleDataButton });
        //
        // musicRentalButton
        //
        musicRentalButton.Location = new Point(12, 12);
        musicRentalButton.Name = "musicRentalButton";
        musicRentalButton.Size = new Size(260, 30);
        musicRentalButton.TabIndex = 0;
        musicRentalButton.Text = "Music Rental Management";
        musicRentalButton.UseVisualStyleBackColor = true;
        musicRentalButton.Click += musicRentalButton_Click;
        //
        // playlistBuilderButton
        //
        playlistBuilderButton.Location = new Point(12, 48);
        playlistBuilderButton.Name = "playlistBuilderButton";
        playlistBuilderButton.Size = new Size(260, 30);
        playlistBuilderButton.TabIndex = 1;
        playlistBuilderButton.Text = "Collaborative Playlist Builder";
        playlistBuilderButton.UseVisualStyleBackColor = true;
        playlistBuilderButton.Click += playlistBuilderButton_Click;
        //
        // eventManagementButton
        //
        eventManagementButton.Location = new Point(12, 84);
        eventManagementButton.Name = "eventManagementButton";
        eventManagementButton.Size = new Size(260, 30);
        eventManagementButton.TabIndex = 2;
        eventManagementButton.Text = "Event Management and Ticketing";
        eventManagementButton.UseVisualStyleBackColor = true;
        eventManagementButton.Click += eventManagementButton_Click;
        //
        // albumInventoryButton
        //
        albumInventoryButton.Location = new Point(12, 120);
        albumInventoryButton.Name = "albumInventoryButton";
        albumInventoryButton.Size = new Size(260, 30);
        albumInventoryButton.TabIndex = 3;
        albumInventoryButton.Text = "Album Inventory";
        albumInventoryButton.UseVisualStyleBackColor = true;
        albumInventoryButton.Click += albumInventoryButton_Click;
        //
        // loadExampleDataButton
        //
        loadExampleDataButton.Location = new Point(12, 174);
        loadExampleDataButton.Name = "loadExampleDataButton";
        loadExampleDataButton.Size = new Size(260, 30);
        loadExampleDataButton.TabIndex = 4;
        loadExampleDataButton.Text = "Load Example Data";
        loadExampleDataButton.UseVisualStyleBackColor = true;
        loadExampleDataButton.Click += loadExampleDataButton_Click;
        //
        // apiStatusStrip
        //
        apiStatusStrip.Items.AddRange(new ToolStripItem[] { apiStatusLabel, inventoryStatusLabel });
        apiStatusStrip.Name = "apiStatusStrip";
        apiStatusStrip.ShowItemToolTips = true;
        apiStatusStrip.TabIndex = 5;
        //
        // apiStatusLabel
        //
        apiStatusLabel.Name = "apiStatusLabel";
        apiStatusLabel.ForeColor = Color.Gray;
        apiStatusLabel.Text = "Music Rental API: Checking...";
        apiStatusLabel.ToolTipText = $"{ClientSettings.MusicRental} checked every 5 seconds";
        //
        // inventoryStatusLabel
        //
        inventoryStatusLabel.Name = "inventoryStatusLabel";
        inventoryStatusLabel.Margin = new Padding(24, 3, 0, 2);
        inventoryStatusLabel.ForeColor = Color.Gray;
        inventoryStatusLabel.Text = "Album Inventory: Checking...";
        inventoryStatusLabel.ToolTipText = $"{ClientSettings.AlbumInventory} checked every 5 seconds";
        //
        // apiStatusTimer
        //
        apiStatusTimer.Interval = 5000;
        apiStatusTimer.Tick += apiStatusTimer_Tick;
        //
        // MainForm
        //
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(menuPanel);
        Controls.Add(apiStatusStrip);
        Name = "MainForm";
        Text = "VibeHive";
        Shown += MainForm_Shown;
        apiStatusStrip.ResumeLayout(false);
        apiStatusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button musicRentalButton;
    private Panel menuPanel;
    private Button playlistBuilderButton;
    private Button eventManagementButton;
    private Button albumInventoryButton;
    private Button loadExampleDataButton;
    private StatusStrip apiStatusStrip;
    private ToolStripStatusLabel apiStatusLabel;
    private ToolStripStatusLabel inventoryStatusLabel;
    private System.Windows.Forms.Timer apiStatusTimer;
}
