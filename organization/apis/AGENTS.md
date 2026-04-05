# Organization API Agent Notes

This file covers `organization/apis/`.

## Agent Rule

- Keep transport code thin.
- If the issue is about billing settings or bank-account behavior, the real fix is usually in shared logic or
  persistence.
- `organization/apis/` should serve precomputed analytics/state from local organization storage.
- Do not reintroduce request-time organization-to-booking calls for analytics or booking-derived state.
- Do not reintroduce `hasFutureBooking` to GraphQL or REST surfaces in this domain.

## Xero API Boundary

- Xero OAuth routes are contract-first OpenAPI surfaces, not ad hoc controller-only endpoints.
- Add or change Xero REST routes in `api-definitions/openapi/skedular/organization_v1.yaml`, regenerate, then implement
  the generated controller surface.
- `organization/apis/` owns:
    - Xero authorize URL generation
    - OAuth callback handling
    - tenant-selection completion/update rules
    - org-facing connection settings mutations/queries
- `organization/apis/` also owns the allowed Xero billing-mode list exposed to the UI/API surface.
- `organization/apis/` owns the org-facing invoice payment-terms settings too, such as default invoice due days.
- Keep the billing-mode surface small and explicit. Booking should own the downstream invoice-behavior differences.
- `RepeatingInvoices` is a valid org-facing Xero billing mode. It enables booking-owned recurring repeating-invoice
  export, not a new org-owned invoice engine.
- `organization/apis/` should not encode recurring cadence rules beyond exposing the billing mode itself. The booking
  layer owns the billing-cycle versus purchase-cadence split for repeating invoices.
- `organization/apis/` also should not encode booking invoice-cadence rules into invoice payment terms. Invoice due
  days are org-level payment terms, not billing-cycle or recurring-cadence rules.
- `organization/apis/` also should not assume that changing billing mode or billing cycle performs an immediate live
  migration of existing recurring Xero exports. Booking owns the freeze-versus-migrate transition policy.
- If the org-facing billing-mode list changes, run `scripts/generate-graphql.sh` instead of hand-editing
  `schema.graphql`
  files, then regenerate the web Relay artifacts that use `organizationXeroBillingModes`.
- Token expiry timestamps are integration-managed state and should not be writable from GraphQL settings mutations.
- The OAuth callback is intentionally not customer-JWT-authenticated. Trust comes from:
    - our encrypted `state`
    - successful Xero code exchange using our client credentials
- Keep callback route building tied to generated controller metadata instead of hardcoding route strings.
- Xero connection writes must invalidate `ICachedOrganizationService`; otherwise the setup UI can keep serving stale org
  Xero state after callback/update/remove.
