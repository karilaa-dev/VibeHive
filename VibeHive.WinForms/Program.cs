namespace VibeHive.WinForms;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        while (true)
        {
            UserSession.Clear();
            using (var login = new EventManagementForm(loginOnly: true))
            {
                if (login.ShowDialog() != DialogResult.OK)
                    break;
            }

            using var main = new MainForm();
            Application.Run(main);
            if (!main.SignOutRequested)
                break;
        }
        UserSession.Clear();
    }    
}
