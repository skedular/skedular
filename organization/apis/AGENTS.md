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
- Token expiry timestamps are integration-managed state and should not be writable from GraphQL settings mutations.
- The OAuth callback is intentionally not customer-JWT-authenticated. Trust comes from:
    - our encrypted `state`
    - successful Xero code exchange using our client credentials
- Keep callback route building tied to generated controller metadata instead of hardcoding route strings.
- Xero connection writes must invalidate `ICachedOrganizationService`; otherwise the setup UI can keep serving stale org
  Xero state after callback/update/remove.
