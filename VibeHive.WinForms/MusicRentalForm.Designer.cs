#nullable disable

namespace VibeHive.WinForms;

partial class MusicRentalForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            api.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        tabControlMain = new TabControl();
        albumsTabPage = new TabPage();
        rentalsTabPage = new TabPage();
        lblTitle = new Label();
        lblArtist = new Label();
        lblGenre = new Label();
        lblYear = new Label();
        txtTitle = new TextBox();
        txtArtist = new TextBox();
        txtGenre = new TextBox();
        numYear = new NumericUpDown();
        btnAddAlbum = new Button();
        btnDeleteAlbum = new Button();
        btnRefreshAlbums = new Button();
        dgvAlbums = new DataGridView();
        albumIdColumn = new DataGridViewTextBoxColumn();
        albumTitleColumn = new DataGridViewTextBoxColumn();
        albumArtistColumn = new DataGridViewTextBoxColumn();
        albumGenreColumn = new DataGridViewTextBoxColumn();
        albumYearColumn = new DataGridViewTextBoxColumn();
        albumAvailableColumn = new DataGridViewCheckBoxColumn();
        lblUserId = new Label();
        lblUserName = new Label();
        lblAlbum = new Label();
        numUserId = new NumericUpDown();
        txtUserName = new TextBox();
        cmbCustomer = new ComboBox();
        cmbAlbum = new ComboBox();
        btnRentAlbum = new Button();
        btnReturnAlbum = new Button();
        btnRefreshRentals = new Button();
        dgvRentals = new DataGridView();
        rentalIdColumn = new DataGridViewTextBoxColumn();
        rentalUserIdColumn = new DataGridViewTextBoxColumn();
        rentalUserNameColumn = new DataGridViewTextBoxColumn();
        rentalAlbumColumn = new DataGridViewTextBoxColumn();
        rentalDateColumn = new DataGridViewTextBoxColumn();
        rentalPendingColumn = new DataGridViewCheckBoxColumn();
        ((System.ComponentModel.ISupportInitialize)numYear).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numUserId).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvAlbums).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvRentals).BeginInit();
        tabControlMain.SuspendLayout();
        albumsTabPage.SuspendLayout();
        rentalsTabPage.SuspendLayout();
        SuspendLayout();
        //
        // tabControlMain
        //
        tabControlMain.Controls.Add(rentalsTabPage);
        tabControlMain.Controls.Add(albumsTabPage);
        tabControlMain.Dock = DockStyle.Fill;
        tabControlMain.Name = "tabControlMain";
        tabControlMain.Size = new Size(1000, 510);
        tabControlMain.SelectedIndex = 0;
        tabControlMain.TabIndex = 0;
        //
        // albumsTabPage
        //
        albumsTabPage.Controls.AddRange(new Control[] { lblTitle, txtTitle, lblArtist, txtArtist,
            lblGenre, txtGenre, lblYear, numYear, btnAddAlbum, btnDeleteAlbum, btnRefreshAlbums, dgvAlbums });
        albumsTabPage.Name = "albumsTabPage";
        albumsTabPage.Size = new Size(992, 482);
        albumsTabPage.Text = "Albums";
        albumsTabPage.UseVisualStyleBackColor = true;
        albumsTabPage.TabIndex = 1;
        //
        // Album entry fields
        //
        lblTitle.AutoSize = true;
        lblTitle.Location = new Point(12, 14);
        lblTitle.Text = "Title";
        txtTitle.Location = new Point(12, 34);
        txtTitle.Name = "txtTitle";
        txtTitle.Size = new Size(215, 23);
        txtTitle.TabIndex = 0;
        lblArtist.AutoSize = true;
        lblArtist.Location = new Point(12, 70);
        lblArtist.Text = "Artist";
        txtArtist.Location = new Point(12, 90);
        txtArtist.Name = "txtArtist";
        txtArtist.Size = new Size(215, 23);
        txtArtist.TabIndex = 1;
        lblGenre.AutoSize = true;
        lblGenre.Location = new Point(12, 126);
        lblGenre.Text = "Genre";
        txtGenre.Location = new Point(12, 146);
        txtGenre.Name = "txtGenre";
        txtGenre.Size = new Size(215, 23);
        txtGenre.TabIndex = 2;
        lblYear.AutoSize = true;
        lblYear.Location = new Point(12, 182);
        lblYear.Text = "Year";
        numYear.Location = new Point(12, 202);
        numYear.Name = "numYear";
        numYear.Minimum = 1;
        numYear.Maximum = 9999;
        numYear.Value = 1;
        numYear.Size = new Size(120, 23);
        numYear.TabIndex = 3;
        //
        // Album buttons
        //
        btnAddAlbum.Location = new Point(12, 246);
        btnAddAlbum.Name = "btnAddAlbum";
        btnAddAlbum.Size = new Size(215, 32);
        btnAddAlbum.Text = "Add Album";
        btnAddAlbum.TabIndex = 4;
        btnAddAlbum.UseVisualStyleBackColor = true;
        btnAddAlbum.Click += btnAddAlbum_Click;
        btnDeleteAlbum.Location = new Point(12, 288);
        btnDeleteAlbum.Name = "btnDeleteAlbum";
        btnDeleteAlbum.Size = new Size(215, 32);
        btnDeleteAlbum.Text = "Delete Selected Album";
        btnDeleteAlbum.TabIndex = 5;
        btnDeleteAlbum.UseVisualStyleBackColor = true;
        btnDeleteAlbum.Click += btnDeleteAlbum_Click;
        btnRefreshAlbums.Location = new Point(12, 330);
        btnRefreshAlbums.Name = "btnRefreshAlbums";
        btnRefreshAlbums.Size = new Size(215, 32);
        btnRefreshAlbums.Text = "Refresh";
        btnRefreshAlbums.TabIndex = 6;
        btnRefreshAlbums.UseVisualStyleBackColor = true;
        btnRefreshAlbums.Click += btnRefresh_Click;
        //
        // dgvAlbums
        //
        dgvAlbums.AllowUserToAddRows = false;
        dgvAlbums.AllowUserToDeleteRows = false;
        dgvAlbums.AllowUserToResizeRows = false;
        dgvAlbums.AutoGenerateColumns = false;
        dgvAlbums.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvAlbums.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvAlbums.Columns.AddRange(albumIdColumn, albumTitleColumn, albumArtistColumn,
            albumGenreColumn, albumYearColumn, albumAvailableColumn);
        dgvAlbums.Location = new Point(245, 14);
        dgvAlbums.Name = "dgvAlbums";
        dgvAlbums.MultiSelect = false;
        dgvAlbums.ReadOnly = true;
        dgvAlbums.RowHeadersVisible = false;
        dgvAlbums.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvAlbums.Size = new Size(735, 456);
        dgvAlbums.TabIndex = 7;
        albumIdColumn.DataPropertyName = "Id";
        albumIdColumn.HeaderText = "ID";
        albumIdColumn.FillWeight = 35;
        albumTitleColumn.DataPropertyName = "Title";
        albumTitleColumn.HeaderText = "Title";
        albumTitleColumn.FillWeight = 160;
        albumArtistColumn.DataPropertyName = "Artist";
        albumArtistColumn.HeaderText = "Artist";
        albumArtistColumn.FillWeight = 130;
        albumGenreColumn.DataPropertyName = "Genre";
        albumGenreColumn.HeaderText = "Genre";
        albumYearColumn.DataPropertyName = "Year";
        albumYearColumn.HeaderText = "Year";
        albumYearColumn.FillWeight = 55;
        albumAvailableColumn.DataPropertyName = "Available";
        albumAvailableColumn.HeaderText = "Available";
        albumAvailableColumn.FillWeight = 65;
        //
        // rentalsTabPage
        //
        rentalsTabPage.Controls.AddRange(new Control[] { lblUserId, numUserId, lblUserName, txtUserName, cmbCustomer,
            lblAlbum, cmbAlbum, btnRentAlbum, btnReturnAlbum, btnRefreshRentals, dgvRentals });
        rentalsTabPage.Name = "rentalsTabPage";
        rentalsTabPage.Size = new Size(992, 482);
        rentalsTabPage.Text = "Active Rentals";
        rentalsTabPage.UseVisualStyleBackColor = true;
        rentalsTabPage.TabIndex = 0;
        //
        // Rental entry fields
        //
        lblUserId.AutoSize = true;
        lblUserId.Location = new Point(12, 14);
        lblUserId.Text = "User ID";
        numUserId.Location = new Point(12, 34);
        numUserId.Name = "numUserId";
        numUserId.Minimum = 1;
        numUserId.Maximum = int.MaxValue;
        numUserId.Value = 1;
        numUserId.Size = new Size(215, 23);
        numUserId.TabIndex = 0;
        lblUserName.AutoSize = true;
        lblUserName.Location = new Point(12, 14);
        lblUserName.Text = "Customer name";
        txtUserName.Location = new Point(12, 34);
        txtUserName.Name = "txtUserName";
        txtUserName.Size = new Size(215, 23);
        txtUserName.TabIndex = 1;
        cmbCustomer.Location = new Point(12, 34);
        cmbCustomer.Name = "cmbCustomer";
        cmbCustomer.Size = new Size(215, 23);
        cmbCustomer.DropDownWidth = 320;
        cmbCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCustomer.TabIndex = 1;
        lblAlbum.AutoSize = true;
        lblAlbum.Location = new Point(12, 70);
        lblAlbum.Text = "Available album";
        cmbAlbum.DisplayMember = "Title";
        cmbAlbum.ValueMember = "Id";
        cmbAlbum.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbAlbum.DropDownWidth = 400;
        cmbAlbum.Location = new Point(12, 90);
        cmbAlbum.Name = "cmbAlbum";
        cmbAlbum.Size = new Size(215, 23);
        cmbAlbum.TabIndex = 2;
        //
        // Rental buttons
        //
        btnRentAlbum.Location = new Point(12, 134);
        btnRentAlbum.Name = "btnRentAlbum";
        btnRentAlbum.Size = new Size(215, 32);
        btnRentAlbum.Text = "Rent Album";
        btnRentAlbum.TabIndex = 3;
        btnRentAlbum.UseVisualStyleBackColor = true;
        btnRentAlbum.Click += btnRentAlbum_Click;
        btnReturnAlbum.Location = new Point(12, 176);
        btnReturnAlbum.Name = "btnReturnAlbum";
        btnReturnAlbum.Size = new Size(215, 32);
        btnReturnAlbum.Text = "Return Selected Rental";
        btnReturnAlbum.TabIndex = 4;
        btnReturnAlbum.UseVisualStyleBackColor = true;
        btnReturnAlbum.Click += btnReturnAlbum_Click;
        btnRefreshRentals.Location = new Point(12, 218);
        btnRefreshRentals.Name = "btnRefreshRentals";
        btnRefreshRentals.Size = new Size(215, 32);
        btnRefreshRentals.Text = "Refresh";
        btnRefreshRentals.TabIndex = 5;
        btnRefreshRentals.UseVisualStyleBackColor = true;
        btnRefreshRentals.Click += btnRefresh_Click;
        //
        // dgvRentals
        //
        dgvRentals.AllowUserToAddRows = false;
        dgvRentals.AllowUserToDeleteRows = false;
        dgvRentals.AllowUserToResizeRows = false;
        dgvRentals.AutoGenerateColumns = false;
        dgvRentals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvRentals.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvRentals.Columns.AddRange(rentalIdColumn, rentalUserIdColumn, rentalUserNameColumn,
            rentalAlbumColumn, rentalDateColumn, rentalPendingColumn);
        dgvRentals.Location = new Point(245, 14);
        dgvRentals.Name = "dgvRentals";
        dgvRentals.MultiSelect = false;
        dgvRentals.ReadOnly = true;
        dgvRentals.RowHeadersVisible = false;
        dgvRentals.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvRentals.Size = new Size(735, 456);
        dgvRentals.TabIndex = 6;
        dgvRentals.CellFormatting += dgvRentals_CellFormatting;
        rentalIdColumn.DataPropertyName = "Id";
        rentalIdColumn.HeaderText = "Rental ID";
        rentalIdColumn.FillWeight = 55;
        rentalUserIdColumn.DataPropertyName = "UserId";
        rentalUserIdColumn.HeaderText = "User ID";
        rentalUserIdColumn.FillWeight = 55;
        rentalUserIdColumn.Visible = false;
        rentalUserNameColumn.DataPropertyName = "UserDisplay";
        rentalUserNameColumn.HeaderText = "Customer";
        rentalUserNameColumn.FillWeight = 120;
        rentalAlbumColumn.DataPropertyName = "AlbumTitle";
        rentalAlbumColumn.HeaderText = "Album";
        rentalAlbumColumn.FillWeight = 150;
        rentalDateColumn.DataPropertyName = "RentalDate";
        rentalDateColumn.HeaderText = "Rental date";
        rentalDateColumn.FillWeight = 130;
        rentalPendingColumn.DataPropertyName = "ReservationPending";
        rentalPendingColumn.HeaderText = "Pending";
        rentalPendingColumn.FillWeight = 55;
        //
        // MusicRentalForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 510);
        Controls.Add(tabControlMain);
        MinimumSize = new Size(900, 500);
        Name = "MusicRentalForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "VibeHive Music Rental Management";
        Shown += MusicRentalForm_Shown;
        ((System.ComponentModel.ISupportInitialize)numYear).EndInit();
        ((System.ComponentModel.ISupportInitialize)numUserId).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvAlbums).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvRentals).EndInit();
        tabControlMain.ResumeLayout(false);
        albumsTabPage.ResumeLayout(false);
        albumsTabPage.PerformLayout();
        rentalsTabPage.ResumeLayout(false);
        rentalsTabPage.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TabControl tabControlMain;
    private TabPage albumsTabPage;
    private TabPage rentalsTabPage;
    private Label lblTitle;
    private Label lblArtist;
    private Label lblGenre;
    private Label lblYear;
    private TextBox txtTitle;
    private TextBox txtArtist;
    private TextBox txtGenre;
    private NumericUpDown numYear;
    private Button btnAddAlbum;
    private Button btnDeleteAlbum;
    private Button btnRefreshAlbums;
    private DataGridView dgvAlbums;
    private DataGridViewTextBoxColumn albumIdColumn;
    private DataGridViewTextBoxColumn albumTitleColumn;
    private DataGridViewTextBoxColumn albumArtistColumn;
    private DataGridViewTextBoxColumn albumGenreColumn;
    private DataGridViewTextBoxColumn albumYearColumn;
    private DataGridViewCheckBoxColumn albumAvailableColumn;
    private Label lblUserId;
    private Label lblUserName;
    private Label lblAlbum;
    private NumericUpDown numUserId;
    private TextBox txtUserName;
    private ComboBox cmbCustomer;
    private ComboBox cmbAlbum;
    private Button btnRentAlbum;
    private Button btnReturnAlbum;
    private Button btnRefreshRentals;
    private DataGridView dgvRentals;
    private DataGridViewTextBoxColumn rentalIdColumn;
    private DataGridViewTextBoxColumn rentalUserIdColumn;
    private DataGridViewTextBoxColumn rentalUserNameColumn;
    private DataGridViewTextBoxColumn rentalAlbumColumn;
    private DataGridViewTextBoxColumn rentalDateColumn;
    private DataGridViewCheckBoxColumn rentalPendingColumn;
}
