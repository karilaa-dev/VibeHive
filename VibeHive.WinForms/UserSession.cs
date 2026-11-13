namespace VibeHive.WinForms;

public static class UserSession
{
    public static string? Token { get; set; }
    public static int UserId { get; set; }
    public static string? UserName { get; set; }
    public static string? Role { get; set; }
    public static Dictionary<int, string> UserNames { get; } = new();

    public static bool IsLoggedIn => !string.IsNullOrEmpty(Token);
    public static bool IsAdmin => Role == "Admin";
    public static bool CanManageEvents => Role is "Admin" or "Event Manager";
    public static bool CanUsePlaylists => Role is "User" or "Admin";

    public static string DisplayUser(int id, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = UserNames.GetValueOrDefault(id) ?? (id == UserId ? UserName : null);
        return $"{(string.IsNullOrWhiteSpace(name) ? "User" : name)} ({id})";
    }

    public static void Clear()
    {
        Token = null;
        UserId = 0;
        UserName = null;
        Role = null;
        UserNames.Clear();
    }
}
