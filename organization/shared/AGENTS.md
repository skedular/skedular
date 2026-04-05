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
- `organization/shared/` should not decide how booking exports repeating invoices. It only persists and maintains the
  org-level Xero connection/settings state, including the selected billing mode.
- `organization/shared/` also owns the persisted org-level invoice payment-terms settings, including default invoice
  due days.
- `organization/shared/` also should not decide whether recurring repeating schedules use organization billing cycle or
  recurring purchase cadence. That split belongs to booking-owned recurring invoice behavior.
- `organization/shared/` should not treat invoice due days as a substitute for billing cycle or recurring cadence.
  Booking consumes the org setting and applies it to generated invoices as payment terms.
- `organization/shared/` should not try to auto-migrate existing recurring Xero exports when org billing mode or org
  billing cycle changes. Booking owns the transition policy for live versus pending recurring exports.
- Refresh failures that require reconnect should deactivate the org Xero connection and persist a reconnect-required
  error instead of silently continuing.

## Workflow ID Rule

- Organization Temporal workflow IDs belong in `organization/shared/Organization.Shared/Services/WorkflowIdService.cs`.
- Do not rebuild organization workflow IDs inline in Temporal services, outbox services, or tests.

## Workflow ID Test Shape

- Keep organization workflow ID unit tests split one class/file per `WorkflowIdService` method under
  `Organization.Shared.UnitTests/Services/WorkflowIdServiceTests`.
- In organization unit tests, keep frozen/injected constructor dependencies before `sut`, and keep random inputs after
  `sut`.
