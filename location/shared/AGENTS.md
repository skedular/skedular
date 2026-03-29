# Location Shared Agent Notes

This file covers `location/shared/`.

## Booking-Derived Architecture

- `location/shared/` owns local precomputed booking-derived analytics tables such as:
  - daily booking counts
  - daily desk booking counts
  - daily room booking counts
- It does not own replicated booking entities.
- Recompute is driven by booking events that trigger Temporal, then the workflow/activity pulls authoritative booking data and rewrites compact local snapshots.

## Replication Boundary

- `location/shared/` may still keep replicated organization, organization-member, customer, and customer-identity state when that state is required for local authorization.
- Those auth-critical replicas are separate from booking-derived analytics and should not be removed by applying the analytics pattern too broadly.

## Temporal Rule

- Location booking-derived recompute uses a short-lived signal-with-start workflow pattern.
- Do not use terminate-and-restart for bursty booking events.
- The intended behavior is:
  - first event starts the workflow
  - later events signal the same workflow while it is running
  - signals collapse into a dirty flag/debounced rerun
  - the workflow exits after the burst is quiet instead of staying long-running forever

## Agent Rule

- Be careful with date/time boundaries and generated availability assumptions.
- Do not reintroduce a persisted `HasFutureBooking` flag or booking-row replica into location shared state.
