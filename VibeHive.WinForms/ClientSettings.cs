namespace VibeHive.WinForms;

// Client addresses. Solution launch profiles select processes, not service addresses.
internal static class ClientSettings
{
    public static string MusicRental => Read("VIBEHIVE_MUSIC_RENTAL_URL", "http://localhost:5254/");
    public static string AlbumInventory => Read("VIBEHIVE_INVENTORY_URL", "http://localhost:5056/");
    public static string Playlist => Read("VIBEHIVE_PLAYLIST_URL", "http://localhost:5133/");
    public static string EventsApiBaseUrl => Read("VIBEHIVE_EVENTS_URL", "http://localhost:5273/");

    private static string Read(string name, string fallback) =>
        (Environment.GetEnvironmentVariable(name) ?? fallback).TrimEnd('/') + "/";
}
