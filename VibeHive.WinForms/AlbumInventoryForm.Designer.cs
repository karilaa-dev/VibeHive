#nullable disable

namespace VibeHive.WinForms;

partial class AlbumInventoryForm
{
    private System.ComponentModel.IContainer components;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            channel.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        workPanel = new Panel();
        lblEditor = new Label();
        lblTitle = new Label();
        lblArtist = new Label();
        lblGenre = new Label();
        lblYear = new Label();
        txtTitle = new TextBox();
        txtArtist = new TextBox();
        txtGenre = new TextBox();
        numYear = new NumericUpDown();
        chkAvailable = new CheckBox();
        btnNew = new Button();
        btnAdd = new Button();
        btnUpdate = new Button();
        btnDelete = new Button();
        btnRefresh = new Button();
        lblLookup = new Label();
        numLookupId = new NumericUpDown();
        btnFind = new Button();
        dgvAlbums = new DataGridView();
        idColumn = new DataGridViewTextBoxColumn();
        titleColumn = new DataGridViewTextBoxColumn();
        artistColumn = new DataGridViewTextBoxColumn();
        genreColumn = new DataGridViewTextBoxColumn();
        yearColumn = new DataGridViewTextBoxColumn();
        availableColumn = new DataGridViewCheckBoxColumn();
        ((System.ComponentModel.ISupportInitialize)numYear).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numLookupId).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvAlbums).BeginInit();
        workPanel.SuspendLayout();
        SuspendLayout();

        workPanel.Dock = DockStyle.Fill;
        workPanel.Location = new Point(0, 0);
        workPanel.Name = "workPanel";
        workPanel.Size = new Size(1000, 510);
        workPanel.Controls.AddRange(new Control[] { lblEditor, lblTitle, txtTitle, lblArtist, txtArtist,
            lblGenre, txtGenre, lblYear, numYear, chkAvailable, btnNew, btnAdd, btnUpdate, btnDelete,
            lblLookup, numLookupId, btnFind, btnRefresh, dgvAlbums });

        lblEditor.AutoSize = true;
        lblEditor.Name = "lblEditor";
        lblEditor.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblEditor.Location = new Point(16, 14);
        lblEditor.Text = "New album";
        lblTitle.AutoSize = true;
        lblTitle.Location = new Point(16, 49);
        lblTitle.Text = "Title";
        txtTitle.Location = new Point(16, 69);
        txtTitle.Name = "txtTitle";
        txtTitle.Size = new Size(220, 23);
        txtTitle.TabIndex = 0;
        lblArtist.AutoSize = true;
        lblArtist.Location = new Point(16, 104);
        lblArtist.Text = "Artist";
        txtArtist.Location = new Point(16, 124);
        txtArtist.Name = "txtArtist";
        txtArtist.Size = new Size(220, 23);
        txtArtist.TabIndex = 1;
        lblGenre.AutoSize = true;
        lblGenre.Location = new Point(16, 159);
        lblGenre.Text = "Genre";
        txtGenre.Location = new Point(16, 179);
        txtGenre.Name = "txtGenre";
        txtGenre.Size = new Size(220, 23);
        txtGenre.TabIndex = 2;
        lblYear.AutoSize = true;
        lblYear.Location = new Point(16, 214);
        lblYear.Text = "Year";
        numYear.Location = new Point(16, 234);
        numYear.Name = "numYear";
        numYear.Minimum = 1;
        numYear.Maximum = 9999;
        numYear.Value = 1;
        numYear.Size = new Size(100, 23);
        numYear.TabIndex = 3;
        chkAvailable.AutoSize = true;
        chkAvailable.Location = new Point(133, 236);
        chkAvailable.Name = "chkAvailable";
        chkAvailable.Text = "Available";
        chkAvailable.TabIndex = 4;

        btnNew.Location = new Point(16, 278);
        btnNew.Name = "btnNew";
        btnNew.Size = new Size(220, 32);
        btnNew.Text = "New / Clear";
        btnNew.TabIndex = 5;
        btnNew.Click += btnNew_Click;
        btnAdd.Location = new Point(16, 320);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(220, 32);
        btnAdd.Text = "Add Album";
        btnAdd.TabIndex = 6;
        btnAdd.Click += btnAdd_Click;
        btnUpdate.Location = new Point(16, 362);
        btnUpdate.Name = "btnUpdate";
        btnUpdate.Size = new Size(220, 32);
        btnUpdate.Text = "Update Selected Album";
        btnUpdate.TabIndex = 7;
        btnUpdate.Click += btnUpdate_Click;
        btnDelete.Location = new Point(16, 404);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(220, 32);
        btnDelete.Text = "Delete Selected Album";
        btnDelete.TabIndex = 8;
        btnDelete.Click += btnDelete_Click;

        lblLookup.AutoSize = true;
        lblLookup.Location = new Point(258, 23);
        lblLookup.Text = "Album ID";
        numLookupId.Location = new Point(326, 19);
        numLookupId.Name = "numLookupId";
        numLookupId.Minimum = 1;
        numLookupId.Maximum = int.MaxValue;
        numLookupId.Value = 1;
        numLookupId.Size = new Size(100, 23);
        numLookupId.TabIndex = 9;
        btnFind.Location = new Point(438, 14);
        btnFind.Name = "btnFind";
        btnFind.Size = new Size(112, 32);
        btnFind.Text = "Find by ID";
        btnFind.TabIndex = 10;
        btnFind.Click += btnFind_Click;
        btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnRefresh.Location = new Point(854, 14);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(130, 32);
        btnRefresh.Text = "Refresh";
        btnRefresh.TabIndex = 11;
        btnRefresh.Click += btnRefresh_Click;

        dgvAlbums.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvAlbums.Location = new Point(258, 62);
        dgvAlbums.Name = "dgvAlbums";
        dgvAlbums.Size = new Size(726, 432);
        dgvAlbums.AllowUserToAddRows = false;
        dgvAlbums.AllowUserToDeleteRows = false;
        dgvAlbums.AllowUserToResizeRows = false;
        dgvAlbums.AutoGenerateColumns = false;
        dgvAlbums.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvAlbums.MultiSelect = false;
        dgvAlbums.ReadOnly = true;
        dgvAlbums.RowHeadersVisible = false;
        dgvAlbums.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvAlbums.TabIndex = 12;
        dgvAlbums.Columns.AddRange(idColumn, titleColumn, artistColumn, genreColumn, yearColumn, availableColumn);
        dgvAlbums.SelectionChanged += dgvAlbums_SelectionChanged;
        idColumn.DataPropertyName = "Id";
        idColumn.HeaderText = "ID";
        idColumn.FillWeight = 35;
        titleColumn.DataPropertyName = "Title";
        titleColumn.HeaderText = "Title";
        titleColumn.FillWeight = 160;
        artistColumn.DataPropertyName = "Artist";
        artistColumn.HeaderText = "Artist";
        artistColumn.FillWeight = 130;
        genreColumn.DataPropertyName = "Genre";
        genreColumn.HeaderText = "Genre";
        genreColumn.FillWeight = 100;
        yearColumn.DataPropertyName = "Year";
        yearColumn.HeaderText = "Year";
        yearColumn.FillWeight = 55;
        availableColumn.DataPropertyName = "Available";
        availableColumn.HeaderText = "Available";
        availableColumn.FillWeight = 70;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 510);
        MinimumSize = new Size(900, 500);
        Controls.Add(workPanel);
        Name = "AlbumInventoryForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "VibeHive Album Inventory";
        Shown += AlbumInventoryForm_Shown;
        ((System.ComponentModel.ISupportInitialize)numYear).EndInit();
        ((System.ComponentModel.ISupportInitialize)numLookupId).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvAlbums).EndInit();
        workPanel.ResumeLayout(false);
        workPanel.PerformLayout();
        ResumeLayout(false);
    }

    private Panel workPanel;
    private Label lblEditor;
    private Label lblTitle;
    private Label lblArtist;
    private Label lblGenre;
    private Label lblYear;
    private TextBox txtTitle;
    private TextBox txtArtist;
    private TextBox txtGenre;
    private NumericUpDown numYear;
    private CheckBox chkAvailable;
    private Button btnNew;
    private Button btnAdd;
    private Button btnUpdate;
    private Button btnDelete;
    private Button btnRefresh;
    private Label lblLookup;
    private NumericUpDown numLookupId;
    private Button btnFind;
    private DataGridView dgvAlbums;
    private DataGridViewTextBoxColumn idColumn;
    private DataGridViewTextBoxColumn titleColumn;
    private DataGridViewTextBoxColumn artistColumn;
    private DataGridViewTextBoxColumn genreColumn;
    private DataGridViewTextBoxColumn yearColumn;
    private DataGridViewCheckBoxColumn availableColumn;
}
