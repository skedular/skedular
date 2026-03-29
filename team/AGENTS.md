# Team Domain Agent Notes

This file is the entry point for AI agents working in `team/`.

## Purpose

- `team/` owns team-related domain state and APIs used by other parts of the platform.

## Where To Read Next

- `team/apis/AGENTS.md`
- `team/domain/AGENTS.md`
- `team/shared/AGENTS.md`

## Booking Boundary

- `team/` no longer owns any booking-derived state.
- `team/` does not store replicated booking rows.
- `team/` does not expose or persist a local `HasFutureBooking` concept.
- If a future requirement needs team booking existence, that query should go to the booking domain instead of rebuilding team-local state.

## Agent Rule

- Treat team identity and membership semantics as shared contracts with other domains.
