namespace VibeHive.WinForms;

partial class EventManagementForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            _apiClient?.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.tabControlMain = new System.Windows.Forms.TabControl();

        // Tab 1: Auth
        this.tabAuth = new System.Windows.Forms.TabPage();
        this.grpLogin = new System.Windows.Forms.GroupBox();
        this.lblLoginEmail = new System.Windows.Forms.Label();
        this.txtLoginEmail = new System.Windows.Forms.TextBox();
        this.lblLoginPassword = new System.Windows.Forms.Label();
        this.txtLoginPassword = new System.Windows.Forms.TextBox();
        this.btnLogin = new System.Windows.Forms.Button();
        this.grpRegister = new System.Windows.Forms.GroupBox();
        this.lblRegName = new System.Windows.Forms.Label();
        this.txtRegName = new System.Windows.Forms.TextBox();
        this.lblRegEmail = new System.Windows.Forms.Label();
        this.txtRegEmail = new System.Windows.Forms.TextBox();
        this.lblRegPassword = new System.Windows.Forms.Label();
        this.txtRegPassword = new System.Windows.Forms.TextBox();
        this.lblRegRole = new System.Windows.Forms.Label();
        this.cmbRegRole = new System.Windows.Forms.ComboBox();
        this.btnRegister = new System.Windows.Forms.Button();
        this.lblSessionStatus = new System.Windows.Forms.Label();

        // Tab 2: Upcoming Events
        this.tabEvents = new System.Windows.Forms.TabPage();
        this.dgvEvents = new System.Windows.Forms.DataGridView();
        this.btnRefreshEvents = new System.Windows.Forms.Button();
        this.btnBookTicket = new System.Windows.Forms.Button();

        // Tab 3: Create Event
        this.tabCreateEvent = new System.Windows.Forms.TabPage();
        this.pnlCreateEvent = new System.Windows.Forms.Panel();
        this.lblEvtName = new System.Windows.Forms.Label();
        this.txtEvtName = new System.Windows.Forms.TextBox();
        this.lblEvtDate = new System.Windows.Forms.Label();
        this.dtpEvtDate = new System.Windows.Forms.DateTimePicker();
        this.lblEvtVenue = new System.Windows.Forms.Label();
        this.txtEvtVenue = new System.Windows.Forms.TextBox();
        this.lblEvtGenre = new System.Windows.Forms.Label();
        this.txtEvtGenre = new System.Windows.Forms.TextBox();
        this.lblEvtTickets = new System.Windows.Forms.Label();
        this.numEvtTickets = new System.Windows.Forms.NumericUpDown();
        this.btnSubmitCreateEvent = new System.Windows.Forms.Button();
        this.lblCreateRestricted = new System.Windows.Forms.Label();

        // Tab 4: My Bookings
        this.tabBookings = new System.Windows.Forms.TabPage();
        this.dgvBookings = new System.Windows.Forms.DataGridView();
        this.btnRefreshBookings = new System.Windows.Forms.Button();
        this.btnCancelBooking = new System.Windows.Forms.Button();
        this.tabPastBookings = new System.Windows.Forms.TabPage();
        this.dgvPastBookings = new System.Windows.Forms.DataGridView();
        this.btnRefreshPastBookings = new System.Windows.Forms.Button();
        this.btnCancelPastBooking = new System.Windows.Forms.Button();

        this.tabControlMain.SuspendLayout();
        this.tabAuth.SuspendLayout();
        this.grpLogin.SuspendLayout();
        this.grpRegister.SuspendLayout();
        this.tabEvents.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.dgvEvents)).BeginInit();
        this.tabCreateEvent.SuspendLayout();
        this.pnlCreateEvent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.numEvtTickets)).BeginInit();
        this.tabBookings.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.dgvBookings)).BeginInit();
        this.tabPastBookings.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.dgvPastBookings)).BeginInit();
        this.SuspendLayout();

        // tabControlMain
        this.tabControlMain.Controls.Add(this.tabAuth);
        this.tabControlMain.Controls.Add(this.tabEvents);
        this.tabControlMain.Controls.Add(this.tabCreateEvent);
        this.tabControlMain.Controls.Add(this.tabBookings);
        this.tabControlMain.Controls.Add(this.tabPastBookings);
        this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tabControlMain.Location = new System.Drawing.Point(0, 0);
        this.tabControlMain.Name = "tabControlMain";
        this.tabControlMain.SelectedIndex = 0;
        this.tabControlMain.Size = new System.Drawing.Size(860, 520);
        this.tabControlMain.TabIndex = 0;

        // tabAuth
        this.tabAuth.Controls.Add(this.lblSessionStatus);
        this.tabAuth.Controls.Add(this.grpRegister);
        this.tabAuth.Controls.Add(this.grpLogin);
        this.tabAuth.Location = new System.Drawing.Point(4, 29);
        this.tabAuth.Name = "tabAuth";
        this.tabAuth.Padding = new System.Windows.Forms.Padding(12);
        this.tabAuth.Size = new System.Drawing.Size(852, 487);
        this.tabAuth.Text = "Auth & Profile";

        // grpLogin
        this.grpLogin.Controls.Add(this.btnLogin);
        this.grpLogin.Controls.Add(this.txtLoginPassword);
        this.grpLogin.Controls.Add(this.lblLoginPassword);
        this.grpLogin.Controls.Add(this.txtLoginEmail);
        this.grpLogin.Controls.Add(this.lblLoginEmail);
        this.grpLogin.Location = new System.Drawing.Point(20, 20);
        this.grpLogin.Name = "grpLogin";
        this.grpLogin.Size = new System.Drawing.Size(380, 260);
        this.grpLogin.TabIndex = 0;
        this.grpLogin.TabStop = false;
        this.grpLogin.Text = "Login";

        this.lblLoginEmail.Location = new System.Drawing.Point(15, 35);
        this.lblLoginEmail.Text = "Email:";
        this.txtLoginEmail.Location = new System.Drawing.Point(15, 60);
        this.txtLoginEmail.Size = new System.Drawing.Size(340, 27);

        this.lblLoginPassword.Location = new System.Drawing.Point(15, 105);
        this.lblLoginPassword.Text = "Password:";
        this.txtLoginPassword.Location = new System.Drawing.Point(15, 130);
        this.txtLoginPassword.Size = new System.Drawing.Size(340, 27);
        this.txtLoginPassword.UseSystemPasswordChar = true;

        this.btnLogin.Location = new System.Drawing.Point(15, 185);
        this.btnLogin.Size = new System.Drawing.Size(120, 35);
        this.btnLogin.Text = "Log In";
        this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

        // grpRegister
        this.grpRegister.Controls.Add(this.btnRegister);
        this.grpRegister.Controls.Add(this.cmbRegRole);
        this.grpRegister.Controls.Add(this.lblRegRole);
        this.grpRegister.Controls.Add(this.txtRegPassword);
        this.grpRegister.Controls.Add(this.lblRegPassword);
        this.grpRegister.Controls.Add(this.txtRegEmail);
        this.grpRegister.Controls.Add(this.lblRegEmail);
        this.grpRegister.Controls.Add(this.txtRegName);
        this.grpRegister.Controls.Add(this.lblRegName);
        this.grpRegister.Location = new System.Drawing.Point(420, 20);
        this.grpRegister.Name = "grpRegister";
        this.grpRegister.Size = new System.Drawing.Size(400, 360);
        this.grpRegister.TabIndex = 1;
        this.grpRegister.TabStop = false;
        this.grpRegister.Text = "Register User";

        this.lblRegName.Location = new System.Drawing.Point(15, 30);
        this.lblRegName.Text = "Full Name:";
        this.txtRegName.Location = new System.Drawing.Point(15, 55);
        this.txtRegName.Size = new System.Drawing.Size(360, 27);

        this.lblRegEmail.Location = new System.Drawing.Point(15, 95);
        this.lblRegEmail.Text = "Email Address:";
        this.txtRegEmail.Location = new System.Drawing.Point(15, 120);
        this.txtRegEmail.Size = new System.Drawing.Size(360, 27);

        this.lblRegPassword.Location = new System.Drawing.Point(15, 160);
        this.lblRegPassword.Text = "Password:";
        this.txtRegPassword.Location = new System.Drawing.Point(15, 185);
        this.txtRegPassword.Size = new System.Drawing.Size(360, 27);
        this.txtRegPassword.UseSystemPasswordChar = true;

        this.lblRegRole.Location = new System.Drawing.Point(15, 225);
        this.lblRegRole.Text = "Role:";
        this.cmbRegRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbRegRole.Items.AddRange(new object[] { "User", "Admin", "Event Manager" });
        this.cmbRegRole.Location = new System.Drawing.Point(15, 250);
        this.cmbRegRole.Size = new System.Drawing.Size(360, 28);

        this.btnRegister.Location = new System.Drawing.Point(15, 300);
        this.btnRegister.Size = new System.Drawing.Size(120, 35);
        this.btnRegister.Text = "Register";
        this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);

        // lblSessionStatus
        this.lblSessionStatus.AutoSize = true;
        this.lblSessionStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.lblSessionStatus.Location = new System.Drawing.Point(20, 305);
        this.lblSessionStatus.Size = new System.Drawing.Size(200, 23);
        this.lblSessionStatus.Text = "Status: Not logged in";

        // tabEvents
        this.tabEvents.Controls.Add(this.btnBookTicket);
        this.tabEvents.Controls.Add(this.btnRefreshEvents);
        this.tabEvents.Controls.Add(this.dgvEvents);
        this.tabEvents.Location = new System.Drawing.Point(4, 29);
        this.tabEvents.Name = "tabEvents";
        this.tabEvents.Padding = new System.Windows.Forms.Padding(12);
        this.tabEvents.Size = new System.Drawing.Size(852, 487);
        this.tabEvents.Text = "Upcoming Events";

        this.dgvEvents.AllowUserToAddRows = false;
        this.dgvEvents.AllowUserToDeleteRows = false;
        this.dgvEvents.AllowUserToResizeRows = false;
        this.dgvEvents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvEvents.Location = new System.Drawing.Point(12, 12);
        this.dgvEvents.MultiSelect = false;
        this.dgvEvents.ReadOnly = true;
        this.dgvEvents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvEvents.Size = new System.Drawing.Size(828, 380);

        this.btnRefreshEvents.Location = new System.Drawing.Point(12, 410);
        this.btnRefreshEvents.Size = new System.Drawing.Size(130, 36);
        this.btnRefreshEvents.Text = "Refresh Events";
        this.btnRefreshEvents.Click += new System.EventHandler(this.btnRefreshEvents_Click);

        this.btnBookTicket.Location = new System.Drawing.Point(660, 410);
        this.btnBookTicket.Size = new System.Drawing.Size(180, 36);
        this.btnBookTicket.Text = "Book Selected Event";
        this.btnBookTicket.Click += new System.EventHandler(this.btnBookTicket_Click);

        // tabCreateEvent
        this.tabCreateEvent.Controls.Add(this.lblCreateRestricted);
        this.tabCreateEvent.Controls.Add(this.pnlCreateEvent);
        this.tabCreateEvent.Location = new System.Drawing.Point(4, 29);
        this.tabCreateEvent.Name = "tabCreateEvent";
        this.tabCreateEvent.Size = new System.Drawing.Size(852, 487);
        this.tabCreateEvent.Text = "Create Event";

        this.lblCreateRestricted.AutoSize = true;
        this.lblCreateRestricted.ForeColor = System.Drawing.Color.Firebrick;
        this.lblCreateRestricted.Location = new System.Drawing.Point(30, 20);
        this.lblCreateRestricted.Text = "Access restricted: Only Admins and Event Managers can create events.";
        this.lblCreateRestricted.Visible = false;

        this.pnlCreateEvent.Controls.Add(this.btnSubmitCreateEvent);
        this.pnlCreateEvent.Controls.Add(this.numEvtTickets);
        this.pnlCreateEvent.Controls.Add(this.lblEvtTickets);
        this.pnlCreateEvent.Controls.Add(this.txtEvtGenre);
        this.pnlCreateEvent.Controls.Add(this.lblEvtGenre);
        this.pnlCreateEvent.Controls.Add(this.txtEvtVenue);
        this.pnlCreateEvent.Controls.Add(this.lblEvtVenue);
        this.pnlCreateEvent.Controls.Add(this.dtpEvtDate);
        this.pnlCreateEvent.Controls.Add(this.lblEvtDate);
        this.pnlCreateEvent.Controls.Add(this.txtEvtName);
        this.pnlCreateEvent.Controls.Add(this.lblEvtName);
        this.pnlCreateEvent.Location = new System.Drawing.Point(25, 45);
        this.pnlCreateEvent.Size = new System.Drawing.Size(550, 400);

        this.lblEvtName.Location = new System.Drawing.Point(10, 10);
        this.lblEvtName.Text = "Event Name:";
        this.txtEvtName.Location = new System.Drawing.Point(10, 35);
        this.txtEvtName.Size = new System.Drawing.Size(480, 27);

        this.lblEvtDate.Location = new System.Drawing.Point(10, 75);
        this.lblEvtDate.Text = "Event Date:";
        this.dtpEvtDate.Location = new System.Drawing.Point(10, 100);
        this.dtpEvtDate.Size = new System.Drawing.Size(480, 27);

        this.lblEvtVenue.Location = new System.Drawing.Point(10, 140);
        this.lblEvtVenue.Text = "Venue:";
        this.txtEvtVenue.Location = new System.Drawing.Point(10, 165);
        this.txtEvtVenue.Size = new System.Drawing.Size(480, 27);

        this.lblEvtGenre.Location = new System.Drawing.Point(10, 205);
        this.lblEvtGenre.Text = "Genre:";
        this.txtEvtGenre.Location = new System.Drawing.Point(10, 230);
        this.txtEvtGenre.Size = new System.Drawing.Size(480, 27);

        this.lblEvtTickets.Location = new System.Drawing.Point(10, 270);
        this.lblEvtTickets.Text = "Available Tickets:";
        this.numEvtTickets.Location = new System.Drawing.Point(10, 295);
        this.numEvtTickets.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        this.numEvtTickets.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        this.numEvtTickets.Value = new decimal(new int[] { 100, 0, 0, 0 });

        this.btnSubmitCreateEvent.Location = new System.Drawing.Point(10, 345);
        this.btnSubmitCreateEvent.Size = new System.Drawing.Size(180, 36);
        this.btnSubmitCreateEvent.Text = "Create Event";
        this.btnSubmitCreateEvent.Click += new System.EventHandler(this.btnSubmitCreateEvent_Click);

        // tabBookings
        this.tabBookings.Controls.Add(this.btnCancelBooking);
        this.tabBookings.Controls.Add(this.btnRefreshBookings);
        this.tabBookings.Controls.Add(this.dgvBookings);
        this.tabBookings.Location = new System.Drawing.Point(4, 29);
        this.tabBookings.Name = "tabBookings";
        this.tabBookings.Padding = new System.Windows.Forms.Padding(12);
        this.tabBookings.Size = new System.Drawing.Size(852, 487);
        this.tabBookings.Text = "My Bookings";

        this.dgvBookings.AllowUserToAddRows = false;
        this.dgvBookings.AllowUserToDeleteRows = false;
        this.dgvBookings.AllowUserToResizeRows = false;
        this.dgvBookings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
        this.dgvBookings.Location = new System.Drawing.Point(12, 12);
        this.dgvBookings.MultiSelect = false;
        this.dgvBookings.ReadOnly = true;
        this.dgvBookings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvBookings.Size = new System.Drawing.Size(828, 380);

        this.btnRefreshBookings.Location = new System.Drawing.Point(12, 410);
        this.btnRefreshBookings.Size = new System.Drawing.Size(150, 36);
        this.btnRefreshBookings.Text = "Refresh Bookings";
        this.btnRefreshBookings.Click += new System.EventHandler(this.btnRefreshBookings_Click);

        this.btnCancelBooking.Location = new System.Drawing.Point(680, 410);
        this.btnCancelBooking.Size = new System.Drawing.Size(160, 36);
        this.btnCancelBooking.Text = "Cancel Booking";
        this.btnCancelBooking.Click += new System.EventHandler(this.btnCancelBooking_Click);

        // tabPastBookings
        this.tabPastBookings.Controls.Add(this.btnCancelPastBooking);
        this.tabPastBookings.Controls.Add(this.btnRefreshPastBookings);
        this.tabPastBookings.Controls.Add(this.dgvPastBookings);
        this.tabPastBookings.Location = new System.Drawing.Point(4, 29);
        this.tabPastBookings.Name = "tabPastBookings";
        this.tabPastBookings.Padding = new System.Windows.Forms.Padding(12);
        this.tabPastBookings.Size = new System.Drawing.Size(852, 487);
        this.tabPastBookings.Text = "Past / Cancelled Bookings";

        this.dgvPastBookings.AllowUserToAddRows = false;
        this.dgvPastBookings.AllowUserToDeleteRows = false;
        this.dgvPastBookings.AllowUserToResizeRows = false;
        this.dgvPastBookings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
        this.dgvPastBookings.Location = new System.Drawing.Point(12, 12);
        this.dgvPastBookings.MultiSelect = false;
        this.dgvPastBookings.ReadOnly = true;
        this.dgvPastBookings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvPastBookings.Size = new System.Drawing.Size(828, 380);

        this.btnRefreshPastBookings.Location = new System.Drawing.Point(12, 410);
        this.btnRefreshPastBookings.Size = new System.Drawing.Size(150, 36);
        this.btnRefreshPastBookings.Text = "Refresh Bookings";
        this.btnRefreshPastBookings.Click += new System.EventHandler(this.btnRefreshBookings_Click);

        this.btnCancelPastBooking.Location = new System.Drawing.Point(680, 410);
        this.btnCancelPastBooking.Size = new System.Drawing.Size(160, 36);
        this.btnCancelPastBooking.Text = "Cancel Booking";
        this.btnCancelPastBooking.Click += new System.EventHandler(this.btnCancelBooking_Click);

        // EventManagementForm
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(860, 520);
        this.Controls.Add(this.tabControlMain);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.Name = "EventManagementForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "VibeHive - Event Management & Ticketing";
        this.Load += new System.EventHandler(this.EventManagementForm_Load);

        this.tabControlMain.ResumeLayout(false);
        this.tabAuth.ResumeLayout(false);
        this.tabAuth.PerformLayout();
        this.grpLogin.ResumeLayout(false);
        this.grpLogin.PerformLayout();
        this.grpRegister.ResumeLayout(false);
        this.grpRegister.PerformLayout();
        this.tabEvents.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.dgvEvents)).EndInit();
        this.tabCreateEvent.ResumeLayout(false);
        this.tabCreateEvent.PerformLayout();
        this.pnlCreateEvent.ResumeLayout(false);
        this.pnlCreateEvent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.numEvtTickets)).EndInit();
        this.tabBookings.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.dgvBookings)).EndInit();
        this.tabPastBookings.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.dgvPastBookings)).EndInit();
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TabControl tabControlMain;
    private System.Windows.Forms.TabPage tabAuth;
    private System.Windows.Forms.GroupBox grpLogin;
    private System.Windows.Forms.Label lblLoginEmail;
    private System.Windows.Forms.TextBox txtLoginEmail;
    private System.Windows.Forms.Label lblLoginPassword;
    private System.Windows.Forms.TextBox txtLoginPassword;
    private System.Windows.Forms.Button btnLogin;
    private System.Windows.Forms.GroupBox grpRegister;
    private System.Windows.Forms.Label lblRegName;
    private System.Windows.Forms.TextBox txtRegName;
    private System.Windows.Forms.Label lblRegEmail;
    private System.Windows.Forms.TextBox txtRegEmail;
    private System.Windows.Forms.Label lblRegPassword;
    private System.Windows.Forms.TextBox txtRegPassword;
    private System.Windows.Forms.Label lblRegRole;
    private System.Windows.Forms.ComboBox cmbRegRole;
    private System.Windows.Forms.Button btnRegister;
    private System.Windows.Forms.Label lblSessionStatus;
    private System.Windows.Forms.TabPage tabEvents;
    private System.Windows.Forms.DataGridView dgvEvents;
    private System.Windows.Forms.Button btnRefreshEvents;
    private System.Windows.Forms.Button btnBookTicket;
    private System.Windows.Forms.TabPage tabCreateEvent;
    private System.Windows.Forms.Panel pnlCreateEvent;
    private System.Windows.Forms.Label lblEvtName;
    private System.Windows.Forms.TextBox txtEvtName;
    private System.Windows.Forms.Label lblEvtDate;
    private System.Windows.Forms.DateTimePicker dtpEvtDate;
    private System.Windows.Forms.Label lblEvtVenue;
    private System.Windows.Forms.TextBox txtEvtVenue;
    private System.Windows.Forms.Label lblEvtGenre;
    private System.Windows.Forms.TextBox txtEvtGenre;
    private System.Windows.Forms.Label lblEvtTickets;
    private System.Windows.Forms.NumericUpDown numEvtTickets;
    private System.Windows.Forms.Button btnSubmitCreateEvent;
    private System.Windows.Forms.Label lblCreateRestricted;
    private System.Windows.Forms.TabPage tabBookings;
    private System.Windows.Forms.DataGridView dgvBookings;
    private System.Windows.Forms.Button btnRefreshBookings;
    private System.Windows.Forms.Button btnCancelBooking;
    private System.Windows.Forms.TabPage tabPastBookings;
    private System.Windows.Forms.DataGridView dgvPastBookings;
    private System.Windows.Forms.Button btnRefreshPastBookings;
    private System.Windows.Forms.Button btnCancelPastBooking;
}
