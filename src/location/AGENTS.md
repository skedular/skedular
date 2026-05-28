# Location Domain Agent Notes

This file is the entry point for AI agents working in `location/`.

## Purpose

- `location/` owns locations, resources, and availability-related domain state.
- `location/` also owns precomputed location analytics derived from booking data.

## Where To Read Next

- `location/apis/AGENTS.md`
- `location/domain/AGENTS.md`
- `location/shared/AGENTS.md`

## Booking-Derived State

- `location/` no longer stores replicated booking rows.
- `location/` no longer exposes or persists a local `HasFutureBooking` concept.
- If some future feature needs to know whether a location has future bookings, ask the booking domain directly instead
  of rebuilding the old local flag.
- Location analytics should come from local precomputed snapshot tables, not request-time cross-domain API calls.

## Replication Boundary

- `location/` is still allowed to keep replicated organization, organization-member, customer, and customer-identity
  state when that state is needed for local authorization and access checks.
- Do not remove auth-critical replicas just because they originate in another domain.
- Only booking-derived and other non-auth passive replicas are candidates for replacement with workflow-driven
  background fetches.

## Agent Rule

- Availability and slot generation logic tends to have time-boundary edge cases. Check generated side effects when
  editing this domain.
