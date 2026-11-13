# VibeHive

Music rental, collaborative playlists, events and album inventory for DEV422. The Windows Forms application uses one Events login for all modules.

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
3. [Playlist API](VibeHive.Playlist.Api/README.md) manages playlists, collaborators and voting on port 5133.
4. [Events API](VibeHive.Events.Api/README.md) provides registration, JWT login, events and tickets on port 5273.
5. [WinForms](VibeHive.WinForms/README.md) provides startup login, all four modules, service status and example loading.

Start Events before signing in. Rentals, playlists and inventory accept the JWT issued by Events. When overriding JWT configuration, use the same `Jwt__Key`, `Jwt__Issuer` and `Jwt__Audience` values for all four backends. Music Rental and Playlist use `VIBEHIVE_EVENTS_URL` (or `Events:Url`) to verify rental customers and registered collaborators.

Album creation, editing and deletion are Admin-only in the GUI and services. All roles can browse albums and rent/return them through Music Rental.

Data is in memory. Restart Music Rental and Album Inventory together for a clean reset.

## Test users and login

No users are preloaded. Register your own Admin account on the startup screen, sign in, and click **Load Example Data** to create the regular test accounts below. Event Manager accounts must also be registered manually.

| Name | Role | Login email | Password | Available |
| --- | --- | --- | --- | --- |
| Alex Morgan | User | `alex.morgan@example.com` | `Example@123` | After Load Example Data |
| Jamie Lee | User | `jamie.lee@example.com` | `Example@123` | After Load Example Data |

To create the example accounts, register/sign in with your Admin account and click **Load Example Data**. This also creates **VibeHive Community**, owned by Alex Morgan, with ten songs and every other regular user invited. Then log out and sign in with an example account. Restarting the Events API removes registered accounts; reopen the GUI to load examples again after registering an Admin.

## Role access

| App part | User | Event Manager | Admin |
| --- | --- | --- | --- |
| Music Rental | Browse albums; rent, view and return own rentals. | Same as User. | Browse albums; select any regular user to rent for; view and return anyone's rentals. |
| Album Inventory | Browse through Music Rental; no album editing. | Same as User. | Open Inventory; add, edit and delete albums. |
| Playlist Builder | Create playlists; edit owned/shared playlists; invite regular users; vote in collaborative playlists. | No access. | View and manage all playlists without invitations. |
| Events & Tickets | Browse events; book, view and cancel own tickets. | Same as User, plus create events. | Create events; book own tickets; view and cancel anyone's bookings. |
| Load Example Data | No access. | No access. | Run once per GUI session; creates regular demo users, albums, rentals and a shared ten-song playlist. |

- Only regular Users can receive playlist invitations. The owner and existing collaborators are excluded from the invite choices. Admins and Event Managers cannot be invited. Admins may vote only in collaborative playlists they own.
- Past/cancelled bookings follow the same visibility rules: users see their own; Admins see everyone's.
- Users and Event Managers rent for themselves. Admins choose a registered regular user for each rental. Ticket bookings always belong to the signed-in account, including for Admins.
