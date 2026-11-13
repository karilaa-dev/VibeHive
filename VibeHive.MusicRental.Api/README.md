# Music Rental API

Part 1 manages rentals through REST. Albums are stored by Album Inventory through gRPC. Rental records use the customer ID and name from the Events JWT, or a regular user selected by an Admin and verified with Events.

Default address: `http://localhost:5254`. Swagger is available at `/swagger` in Development.

1. `POST /api/music` adds an album and returns its generated ID (Admin only).
2. `GET /api/music` lists available albums. Add `?includeUnavailable=true` to include rented albums.
3. `DELETE /api/music/{id}` deletes an album that has no active reservation (Admin only).
4. `POST /api/rentals` rents an album using `userId`, `albumId`, and optional `userName`. For Users and Event Managers, the API replaces customer ID/name with the authenticated identity. Admins can select any registered regular user by `userId`; the API checks that user's role and saves their name from Events. Unknown users and privileged accounts are rejected with 400.
5. `GET /api/rentals` lists the user's active and pending rentals, or all rentals for admins.
6. `POST /api/rentals/{id}/return` returns the user's rental (or any rental for admins) and makes its album available.

Rental endpoints require `Authorization: Bearer <token>` from Events login. `GET /health` is public for the main menu's status checks.

Album additions and deletions also require an Admin JWT. Browsing remains public, and every signed-in role can rent and return albums. The API forwards each caller's token to gRPC for album edits, reservations and returns; it does not use a shared admin account. Missing/invalid tokens return 401 and unauthorized edits return 403.

Invalid input returns 400, missing records return 404, conflicts return 409, and inventory connection failures return 503. A pending rental means the connection failed before confirmation. Reconnect Inventory, refresh Active Rentals, and return the pending record before trying again.

The inventory address is `AlbumInventory:Url` in `appsettings.json`, overridden by `VIBEHIVE_INVENTORY_URL`. Customer verification uses `Events:Url` (default `http://localhost:5273`), overridden by `VIBEHIVE_EVENTS_URL`. Events must be running for Admins to rent for customers; verification failures return 503. All data is in memory. Restart Music Rental and Album Inventory together for a clean reset.
