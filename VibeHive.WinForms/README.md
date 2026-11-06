# WinForms

The main menu opens the component forms and shows Music Rental and Album Inventory status. It hides while a component form is open.

1. **Load Example Data** creates five albums and two rentals through Music Rental. Album IDs come from the API. Repeated clicks reuse matching albums and existing active rentals. Existing data is kept. Playlist and Events are still starter APIs.
2. **Music Rental Management** adds and deletes albums, rents available albums, and returns active rentals. It uses `/api/music` and `/api/rentals`.
3. **Album Inventory** adds, finds, updates, and deletes albums through the gRPC methods in the Inventory project. Click Refresh to load changes made elsewhere. Errors use message boxes.
4. **Collaborative Playlist Builder** and **Event Management and Ticketing** are placeholder forms for teammates.

Start Music Rental and Album Inventory before loading examples or using their forms. `ClientSettings.cs` contains default client addresses. `VIBEHIVE_MUSIC_RENTAL_URL`, `VIBEHIVE_INVENTORY_URL`, `VIBEHIVE_PLAYLIST_URL`, and `VIBEHIVE_EVENTS_URL` can override them before launch.
