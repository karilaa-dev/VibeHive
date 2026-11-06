# Album Inventory

Part 4 stores albums and rental reservations in memory. It serves gRPC over HTTP/2 at `http://localhost:5056`.

The contract is `Protos/album_inventory.proto`. Clients link this file to generate their client types. Album fields are ID, title, artist, genre, year, and availability.

1. `AddAlbum` validates an album and generates its ID. Set availability explicitly.
2. `GetAlbum` finds one album by ID.
3. `UpdateAlbum` changes album details and availability.
4. `DeleteAlbum` removes an album with no active rental reservation.
5. `ListAlbums` sends a finite server stream. Its request can filter available albums.
6. `ReserveAlbum` atomically reserves an available album for Music Rental.
7. `ReleaseAlbum` returns the matching reservation. Retrying a return cannot release another customer's reservation.

Invalid data returns `InvalidArgument`, missing albums return `NotFound`, and protected changes return `FailedPrecondition`. Rented albums cannot be deleted or forced available. Metadata can still be edited.

Restart this service and Music Rental together to reset the demo. Playlist songs have their own IDs. A song may optionally reference an album ID if the team adds that relationship.
