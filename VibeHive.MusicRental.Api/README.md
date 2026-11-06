# Music Rental API

Part 1 manages rentals through REST. Albums are stored by Album Inventory through gRPC. Rental records contain the entered customer ID and name.

Default address: `http://localhost:5254`. Swagger is available at `/swagger` in Development.

1. `POST /api/music` adds an album and returns its generated ID.
2. `GET /api/music` lists available albums. Add `?includeUnavailable=true` to include rented albums.
3. `DELETE /api/music/{id}` deletes an album that has no active reservation.
4. `POST /api/rentals` rents an album using `userId`, `albumId`, and optional `userName`.
5. `GET /api/rentals` lists active and pending rentals.
6. `POST /api/rentals/{id}/return` returns a rental and makes its album available.

Invalid input returns 400, missing records return 404, conflicts return 409, and inventory connection failures return 503. A pending rental means the connection failed before confirmation. Reconnect Inventory, refresh Active Rentals, and return the pending record before trying again.

The inventory address is `AlbumInventory:Url` in `appsettings.json`, overridden by `VIBEHIVE_INVENTORY_URL`. All data is in memory. Restart Music Rental and Album Inventory together for a clean reset.
