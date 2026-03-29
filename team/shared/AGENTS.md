# Team Shared Agent Notes

This file covers `team/shared/`.

## Agent Rule

- Preserve team identity and membership consistency because other domains may rely on these assumptions.
- Do not reintroduce booking replication, booking-derived snapshot tables, or a persisted `HasFutureBooking` flag into team shared state.
