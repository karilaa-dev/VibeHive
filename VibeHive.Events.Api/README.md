# Events API

Part 3 provides registration, JWT login, role-based event creation and ticket booking.

Default address: `http://localhost:5273`. Swagger is available at `/swagger` in Development.

`POST /api/users/register` and `POST /api/auth/login` support the application's startup login screen. Music Rental and Playlist validate the same JWT; keep signing key, issuer and audience consistent across the three REST APIs.

Authenticated `GET /api/users` and `GET /api/users/{id}` return registered user IDs, names and roles for playlist invitations, without exposing passwords or email addresses. Booking rows also include the owner's name for display. `GET /health` is public for the main menu's status checks.

Users, events and tickets remain in memory. No users are seeded at startup. Register an Admin manually, then use the GUI's Load Example Data button to create regular example accounts. Restarting this API resets registered users and bookings.

`POST /api/tickets` takes `eventId` and always books for the JWT user ID, including when the caller is an Admin. A supplied `userId` field is ignored. `GET /api/tickets` returns the caller's tickets for User/Event Manager accounts and all bookings for Admins. The existing `GET /api/tickets/{userId}` route allows owners or Admins; other users receive 403. Booking rows include `userId` so Admins can identify their owners. All ticket endpoints require a valid JWT.

`PATCH /api/tickets/{id}/status` accepts `Cancelled` from the booking's owner or an Admin. Only Admins may set `Booked` to restore a cancelled booking. Status changes and availability use the same lock as ticket booking: cancelling restores one ticket, restoring consumes one available ticket, and repeating the same status does not change availability. Invalid statuses return 400, unauthorized changes return 403, and missing tickets return 404. New or restored bookings for past events are rejected. Existing past bookings can still be viewed and cancelled; cancelled records are retained.
