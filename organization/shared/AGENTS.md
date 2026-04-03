# Organization Shared Agent Notes

This file covers `organization/shared/`.

## Booking-Derived Architecture

- `organization/shared/` owns local precomputed booking-derived state such as daily booking counts and active-member
  snapshots.
- It does not own replicated booking entities.
- Recompute is driven by booking events that trigger Temporal, then the workflow/activity pulls authoritative booking
  data and rewrites compact local snapshots.

## Replication Boundary

- `organization/shared/` may still contain replicated organization-adjacent auth state consumed by local authorization
  flows.
- Keep organization, organization-member, customer, and customer-identity replicas where they support local access
  checks.
- Do not treat auth-critical replicas as candidates for the workflow-rebuild pattern used for booking-derived analytics.

## Temporal Rule

- Organization booking-derived recompute uses a short-lived signal-with-start workflow pattern.
- Do not use terminate-and-restart for bursty booking events.
- The intended behavior is:
    - first event starts the workflow
    - later events signal the same workflow while it is running
    - signals collapse into a dirty flag/debounced rerun
    - the workflow exits after the burst is quiet instead of staying long-running forever

## Agent Rule

- Treat tax, bank-account, billing-cycle, and connect-account behavior as cross-domain contracts, not local
  implementation details.
- Do not reintroduce a persisted `HasFutureBooking` flag or booking-row replica into organization shared state.

## Xero Shared Rules

- `organization/shared/` owns the org-specific Xero maintenance workflow and token refresh/orchestration services.
- Enterprise-owned Xero primitives such as config binding, SDK factory, and Xero token encryption are registered through
  `Enterprise.Shared.Accounting.AddXeroServices(configuration)`.
- Do not bind `XeroConfiguration` again inside `organization/shared` if the dependency comes from `Enterprise.Shared`.
- Xero token encryption uses `IXeroTokenEncryptionService`, not cookie-specific services.
- Keep `ICookieEncryptionService` reserved for cookie/SSO behavior.
- `MaintainOrganizationXeroConnection` is the long-running org-owned maintenance loop for token refresh, not a booking
  concern.
- Refresh failures that require reconnect should deactivate the org Xero connection and persist a reconnect-required
  error instead of silently continuing.
