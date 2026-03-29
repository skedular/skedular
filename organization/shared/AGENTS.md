# Organization Shared Agent Notes

This file covers `organization/shared/`.

## Booking-Derived Architecture

- `organization/shared/` owns local precomputed booking-derived state such as daily booking counts and active-member snapshots.
- It does not own replicated booking entities.
- Recompute is driven by booking events that trigger Temporal, then the workflow/activity pulls authoritative booking data and rewrites compact local snapshots.

## Temporal Rule

- Organization booking-derived recompute uses a short-lived signal-with-start workflow pattern.
- Do not use terminate-and-restart for bursty booking events.
- The intended behavior is:
  - first event starts the workflow
  - later events signal the same workflow while it is running
  - signals collapse into a dirty flag/debounced rerun
  - the workflow exits after the burst is quiet instead of staying long-running forever

## Agent Rule

- Treat tax, bank-account, billing-cycle, and connect-account behavior as cross-domain contracts, not local implementation details.
- Do not reintroduce a persisted `HasFutureBooking` flag or booking-row replica into organization shared state.
