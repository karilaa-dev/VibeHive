# Playlist API

Part 2 provides playlist creation, songs, invitations, voting and rankings with the existing OOP models and Command Pattern.

Default address: `http://localhost:5133`. Swagger is available at `/swagger` in Development.

All `/api/playlists` endpoints require an Events JWT with the User or Admin role in `Authorization: Bearer <token>`. Event Managers are denied access. Creator, editor, inviter and voter IDs come from the token. Regular users see playlists they own or collaborate on. Admins can see and manage all playlists without being collaborators; voting still requires membership.

Invitations verify the selected user and role with Events at `VIBEHIVE_EVENTS_URL`, falling back to `Events:Url` or `http://localhost:5273`. Only regular Users can receive invitations. Unknown users and privileged roles return 400; unavailable Events returns 503. The GUI loads registered collaborators from Events.

Songs use the existing `PUT /api/playlists/{id}/add` operation and manual entry fields. The example loader creates a collaborative **VibeHive Community** playlist owned by a regular user, invites all other regular users, and adds ten sample songs directly to it.

`GET /health` is public for service status checks. Data remains in memory.
