namespace VibeHive.WinForms;

public static class UserSession
{
    public static string? Token { get; set; }
    public static int UserId { get; set; }
    public static string? UserName { get; set; }
    public static string? Role { get; set; }

    public static bool IsLoggedIn => !string.IsNullOrEmpty(Token);
    public static bool IsAdmin => Role == "Admin";
    public static bool CanManageEvents => Role is "Admin" or "EventManager";

    public static void Clear()
    {
        Token = null;
        UserId = 0;
        UserName = null;
        Role = null;
    }
}