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
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        musicRentalButton = new Button();
        playlistBuilderButton = new Button();
        eventManagementButton = new Button();
        albumInventoryButton = new Button();
        SuspendLayout();
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
        // MainForm
        //
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(albumInventoryButton);
        Controls.Add(eventManagementButton);
        Controls.Add(playlistBuilderButton);
        Controls.Add(musicRentalButton);
        Name = "MainForm";
        Text = "VibeHive";
        ResumeLayout(false);
    }

    #endregion

    private Button musicRentalButton;
    private Button playlistBuilderButton;
    private Button eventManagementButton;
    private Button albumInventoryButton;
}
