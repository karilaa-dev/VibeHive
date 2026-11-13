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
        sessionLabel = new Label();
        logoutButton = new Button();
        apiStatusStrip = new StatusStrip();
        secondaryApiStatusStrip = new StatusStrip();
        apiStatusLabel = new ToolStripStatusLabel();
        inventoryStatusLabel = new ToolStripStatusLabel();
        playlistStatusLabel = new ToolStripStatusLabel();
        eventsStatusLabel = new ToolStripStatusLabel();
        apiStatusTimer = new System.Windows.Forms.Timer(components);
        apiStatusStrip.SuspendLayout();
        secondaryApiStatusStrip.SuspendLayout();
        SuspendLayout();
        //
        // menuPanel
        //
        menuPanel.Dock = DockStyle.Fill;
        menuPanel.Name = "menuPanel";
        menuPanel.Controls.AddRange(new Control[] { musicRentalButton, playlistBuilderButton,
            eventManagementButton, albumInventoryButton, loadExampleDataButton, sessionLabel, logoutButton });
        sessionLabel.Location = new Point(12, 9);
        sessionLabel.Size = new Size(284, 30);
        sessionLabel.AutoEllipsis = true;
        sessionLabel.TextAlign = ContentAlignment.MiddleLeft;
        sessionLabel.Name = "sessionLabel";
        logoutButton.Location = new Point(308, 9);
        logoutButton.Size = new Size(80, 30);
        logoutButton.TabIndex = 5;
        logoutButton.Name = "logoutButton";
        logoutButton.Text = "Log Out";
        logoutButton.Click += logoutButton_Click;
        //
        // musicRentalButton
        //
        musicRentalButton.Location = new Point(12, 54);
        musicRentalButton.Name = "musicRentalButton";
        musicRentalButton.Size = new Size(376, 30);
        musicRentalButton.TabIndex = 0;
        musicRentalButton.Text = "Music Rental Management";
        musicRentalButton.UseVisualStyleBackColor = true;
        musicRentalButton.Click += musicRentalButton_Click;
        //
        // playlistBuilderButton
        //
        playlistBuilderButton.Location = new Point(12, 90);
        playlistBuilderButton.Name = "playlistBuilderButton";
        playlistBuilderButton.Size = new Size(376, 30);
        playlistBuilderButton.TabIndex = 1;
        playlistBuilderButton.Text = "Collaborative Playlist Builder";
        playlistBuilderButton.UseVisualStyleBackColor = true;
        playlistBuilderButton.Click += playlistBuilderButton_Click;
        //
        // eventManagementButton
        //
        eventManagementButton.Location = new Point(12, 126);
        eventManagementButton.Name = "eventManagementButton";
        eventManagementButton.Size = new Size(376, 30);
        eventManagementButton.TabIndex = 2;
        eventManagementButton.Text = "Event Management and Ticketing";
        eventManagementButton.UseVisualStyleBackColor = true;
        eventManagementButton.Click += eventManagementButton_Click;
        //
        // albumInventoryButton
        //
        albumInventoryButton.Location = new Point(12, 162);
        albumInventoryButton.Name = "albumInventoryButton";
        albumInventoryButton.Size = new Size(376, 30);
        albumInventoryButton.TabIndex = 3;
        albumInventoryButton.Text = "Album Inventory";
        albumInventoryButton.UseVisualStyleBackColor = true;
        albumInventoryButton.Click += albumInventoryButton_Click;
        //
        // loadExampleDataButton
        //
        loadExampleDataButton.Location = new Point(12, 210);
        loadExampleDataButton.Name = "loadExampleDataButton";
        loadExampleDataButton.Size = new Size(376, 30);
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
        apiStatusStrip.SizingGrip = false;
        apiStatusStrip.TabIndex = 6;
        secondaryApiStatusStrip.Items.AddRange(new ToolStripItem[] { playlistStatusLabel, eventsStatusLabel });
        secondaryApiStatusStrip.Name = "secondaryApiStatusStrip";
        secondaryApiStatusStrip.ShowItemToolTips = true;
        secondaryApiStatusStrip.TabIndex = 7;
        //
        // apiStatusLabel
        //
        apiStatusLabel.Name = "apiStatusLabel";
        apiStatusLabel.AutoSize = false;
        apiStatusLabel.Size = new Size(170, 17);
        apiStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
        apiStatusLabel.ForeColor = Color.Gray;
        apiStatusLabel.Text = "Music Rental: Checking...";
        apiStatusLabel.ToolTipText = "Checked every 5 seconds";
        //
        // inventoryStatusLabel
        //
        inventoryStatusLabel.Name = "inventoryStatusLabel";
        inventoryStatusLabel.Margin = new Padding(12, 3, 0, 2);
        inventoryStatusLabel.ForeColor = Color.Gray;
        inventoryStatusLabel.Text = "Album Inventory: Checking...";
        inventoryStatusLabel.ToolTipText = "Checked every 5 seconds";
        playlistStatusLabel.Name = "playlistStatusLabel";
        playlistStatusLabel.AutoSize = false;
        playlistStatusLabel.Size = new Size(170, 17);
        playlistStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
        playlistStatusLabel.ForeColor = Color.Gray;
        playlistStatusLabel.Text = "Playlists: Checking...";
        eventsStatusLabel.Name = "eventsStatusLabel";
        eventsStatusLabel.Margin = new Padding(12, 3, 0, 2);
        eventsStatusLabel.ForeColor = Color.Gray;
        eventsStatusLabel.Text = "Events: Checking...";
        //
        // apiStatusTimer
        //
        apiStatusTimer.Interval = 5000;
        apiStatusTimer.Tick += apiStatusTimer_Tick;
        //
        // MainForm
        //
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(400, 300);
        MinimumSize = new Size(416, 339);
        StartPosition = FormStartPosition.CenterScreen;
        Controls.Add(menuPanel);
        Controls.Add(apiStatusStrip);
        Controls.Add(secondaryApiStatusStrip);
        Name = "MainForm";
        Text = "VibeHive";
        Shown += MainForm_Shown;
        apiStatusStrip.ResumeLayout(false);
        apiStatusStrip.PerformLayout();
        secondaryApiStatusStrip.ResumeLayout(false);
        secondaryApiStatusStrip.PerformLayout();
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
    private Label sessionLabel;
    private Button logoutButton;
    private StatusStrip apiStatusStrip;
    private StatusStrip secondaryApiStatusStrip;
    private ToolStripStatusLabel apiStatusLabel;
    private ToolStripStatusLabel inventoryStatusLabel;
    private ToolStripStatusLabel playlistStatusLabel;
    private ToolStripStatusLabel eventsStatusLabel;
    private System.Windows.Forms.Timer apiStatusTimer;
}
