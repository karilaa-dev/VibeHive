using System.Text.Json;
using System.ComponentModel;
using VibeHive.WinForms.Services;

namespace VibeHive.WinForms;

public partial class EventManagementForm : Form
{
    private readonly EventsApiClient _apiClient;
    private readonly bool loginOnly;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public EventManagementForm() : this(false) { }

    public EventManagementForm(bool loginOnly)
    {
        this.loginOnly = loginOnly;
        InitializeComponent();
        _apiClient = new EventsApiClient(ClientSettings.EventsApiBaseUrl);
        if (loginOnly)
        {
            Text = "VibeHive - Login";
            tabControlMain.TabPages.Clear();
            tabControlMain.TabPages.Add(tabAuth);
            tabAuth.Text = "Login & Registration";
            AcceptButton = btnLogin;
        }
        else
        {
            tabControlMain.TabPages.Remove(tabAuth);
        }
    }

    private async void EventManagementForm_Load(object sender, EventArgs e)
    {
        cmbRegRole.SelectedIndex = 0; // Default to "User"
        ApplyRolePermissions();

        if (!loginOnly && UserSession.IsLoggedIn)
        {
            await LoadUpcomingEventsAsync();
            await LoadUserBookingsAsync();
        }
    }

    private void ApplyRolePermissions()
    {
        bool isLoggedIn = UserSession.IsLoggedIn;

        // Ensure this control name matches the label in EventManagementForm.Designer.cs
        lblSessionStatus.Text = isLoggedIn
            ? $"Status: Logged in as {UserSession.DisplayUser(UserSession.UserId)} [{UserSession.Role}]"
            : "Status: Not logged in";

        btnLogin.Enabled = !isLoggedIn;

        // Booking and cancellation permissions
        btnBookTicket.Enabled = isLoggedIn;
        btnCancelBooking.Enabled = btnCancelPastBooking.Enabled = isLoggedIn;
        tabBookings.Text = UserSession.IsAdmin ? "All Bookings" : "My Bookings";

        // Show or hide the Create Event tab page based on role
        if (!loginOnly && UserSession.CanManageEvents)
        {
            if (!tabControlMain.TabPages.Contains(tabCreateEvent))
            {
                tabControlMain.TabPages.Add(tabCreateEvent);
            }
            btnSubmitCreateEvent.Enabled = true;
        }
        else
        {
            if (tabControlMain.TabPages.Contains(tabCreateEvent))
            {
                tabControlMain.TabPages.Remove(tabCreateEvent);
            }
        }
    }

    // --- Tab 1: Auth & Profile ---

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        var email = txtLoginEmail.Text.Trim();
        var password = txtLoginPassword.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Please enter both email and password.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            btnLogin.Enabled = false;
            btnRegister.Enabled = false;
            var success = await _apiClient.LoginAsync(email, password);
            if (IsDisposed) return;
            if (success)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid email or password.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            if (!IsDisposed)
                MessageBox.Show(this, $"Could not log in. Check that the Events API is running.\n{ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed)
            {
                btnLogin.Enabled = true;
                btnRegister.Enabled = true;
            }
        }
    }

    private async void btnRegister_Click(object sender, EventArgs e)
    {
        var name = txtRegName.Text.Trim();
        var email = txtRegEmail.Text.Trim();
        var password = txtRegPassword.Text;
        var role = cmbRegRole.SelectedItem?.ToString() ?? "User";

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("All registration fields are required.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var success = await _apiClient.RegisterAsync(name, email, password, role);
            if (success)
            {
                MessageBox.Show("Registration successful! You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtLoginEmail.Text = email;
                txtLoginPassword.Clear();
                txtLoginPassword.Focus();
                txtRegName.Clear();
                txtRegEmail.Clear();
                txtRegPassword.Clear();
            }
            else
            {
                MessageBox.Show("Registration failed. Email may already be in use.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error registering user: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // --- Tab 2: Upcoming Events ---

    private async void btnRefreshEvents_Click(object sender, EventArgs e)
    {
        await LoadUpcomingEventsAsync();
    }

    private async Task LoadUpcomingEventsAsync()
    {
        try
        {
            var json = await _apiClient.GetEventsJsonAsync();
            var events = JsonSerializer.Deserialize<List<EventViewModel>>(json, JsonOpts);
            dgvEvents.DataSource = events;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load events: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnBookTicket_Click(object sender, EventArgs e)
    {
        if (!UserSession.IsLoggedIn)
        {
            MessageBox.Show("Please log in first to book a ticket.", "Authentication Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (dgvEvents.CurrentRow?.DataBoundItem is not EventViewModel selectedEvent)
        {
            MessageBox.Show("Please select an event to book.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (selectedEvent.AvailableTickets <= 0)
        {
            MessageBox.Show("No tickets available for this event.", "Sold Out", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var booked = await _apiClient.BookTicketAsync(selectedEvent.Id);
            if (booked)
            {
                MessageBox.Show($"Ticket booked successfully for {selectedEvent.Name}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadUpcomingEventsAsync();
                await LoadUserBookingsAsync();
            }
            else
            {
                MessageBox.Show("Failed to book ticket.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Booking error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // --- Tab 3: Create Event ---

    private async void btnSubmitCreateEvent_Click(object sender, EventArgs e)
    {
        if (!UserSession.CanManageEvents)
        {
            MessageBox.Show("Only Admins and Event Managers can create events.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var name = txtEvtName.Text.Trim();
        var venue = txtEvtVenue.Text.Trim();
        var genre = txtEvtGenre.Text.Trim();
        var date = dtpEvtDate.Value;
        var tickets = (int)numEvtTickets.Value;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(venue) || string.IsNullOrWhiteSpace(genre))
        {
            MessageBox.Show("Please fill out all event fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var success = await _apiClient.CreateEventAsync(name, date, venue, genre, tickets);
            if (success)
            {
                MessageBox.Show("Event created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtEvtName.Clear();
                txtEvtVenue.Clear();
                txtEvtGenre.Clear();
                await LoadUpcomingEventsAsync();
                tabControlMain.SelectedTab = tabEvents;
            }
            else
            {
                MessageBox.Show("Failed to create event.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating event: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // --- Tab 4: My Bookings ---

    private async void btnRefreshBookings_Click(object sender, EventArgs e)
    {
        await LoadUserBookingsAsync();
    }

    private async Task LoadUserBookingsAsync()
    {
        if (!UserSession.IsLoggedIn) return;

        try
        {
            var json = await _apiClient.GetUserTicketsJsonAsync();
            var tickets = JsonSerializer.Deserialize<List<TicketViewModel>>(json, JsonOpts) ?? new();
            if (IsDisposed) return;
            var now = DateTime.Now;
            dgvBookings.DataSource = tickets.Where(ticket => ticket.Status == "Booked" && ticket.Date.ToLocalTime() > now).ToList();
            dgvPastBookings.DataSource = tickets.Where(ticket => ticket.Status != "Booked" || ticket.Date.ToLocalTime() <= now)
                .OrderByDescending(ticket => ticket.Date).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load bookings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnCancelBooking_Click(object sender, EventArgs e)
    {
        if (!UserSession.IsLoggedIn)
        {
            MessageBox.Show("Please log in first to cancel a booking.", "Authentication Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var bookingsGrid = sender == btnCancelPastBooking ? dgvPastBookings : dgvBookings;
        if (bookingsGrid.CurrentRow?.DataBoundItem is not TicketViewModel selectedTicket)
        {
            MessageBox.Show("Please select a booking to cancel.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!UserSession.IsAdmin && selectedTicket.UserId != UserSession.UserId)
        {
            MessageBox.Show("You can only cancel your own bookings.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (selectedTicket.Status == "Cancelled")
        {
            MessageBox.Show("This booking is already cancelled.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show($"Are you sure you want to cancel Ticket #{selectedTicket.Id} for '{selectedTicket.EventName}'?",
            "Confirm Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        try
        {
            var success = await _apiClient.CancelTicketAsync(selectedTicket.Id);
            if (success)
            {
                MessageBox.Show("Booking cancelled successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadUserBookingsAsync();
                await LoadUpcomingEventsAsync();
            }
            else
            {
                MessageBox.Show("Failed to cancel booking.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error cancelling ticket: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

// Supporting view models for DataGridView binding
public class EventViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int AvailableTickets { get; set; }
}

public class TicketViewModel
{
    public int Id { get; set; }
    public int EventId { get; set; }
    [Browsable(false)]
    public int UserId { get; set; }
    [Browsable(false)]
    public string UserName { get; set; } = "";
    [DisplayName("User")]
    public string UserDisplay => UserSession.DisplayUser(UserId, UserName);
    public string EventName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime BookedAt { get; set; }
}
