# VibeHive

Music rental and album inventory for DEV422. Parts 1 and 4 are implemented. Playlist and Events are starter projects for teammates.

Open `VibeHive.sln` with .NET 8 and Windows Forms support. Select **Start APIs and GUI**, **Start APIs**, or **Start GUI** in Visual Studio's Start dropdown. The shared profiles require Visual Studio 2022 version 17.11 or later. Enable Multi Project Launch Profiles in Preview Features if the choices are hidden.

On older Visual Studio versions, use Configure Startup Projects to start the four backends and optionally WinForms. From terminals, run these commands in separate windows. Omit the final command to start only the APIs.

```powershell
dotnet run --project VibeHive.AlbumInventory.Grpc --launch-profile http
dotnet run --project VibeHive.MusicRental.Api --launch-profile http
dotnet run --project VibeHive.Playlist.Api --launch-profile http
dotnet run --project VibeHive.Events.Api --launch-profile http
dotnet run --project VibeHive.WinForms
```

1. [Music Rental API](VibeHive.MusicRental.Api/README.md) manages album rentals through REST on port 5254.
2. [Album Inventory](VibeHive.AlbumInventory.Grpc/README.md) owns albums and reservations through gRPC on port 5056.
3. [Playlist API](VibeHive.Playlist.Api/README.md) is the Part 2 starter on port 5133.
4. [Events API](VibeHive.Events.Api/README.md) is the Part 3 starter on port 5273.
5. [WinForms](VibeHive.WinForms/README.md) provides the main menu, example loading, rental screen, and inventory screen.

Data is in memory. Restart Music Rental and Album Inventory together for a clean reset.
