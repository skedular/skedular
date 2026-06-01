# Implementation Notes: Customer Landing Cleanup

## GraphQL Gaps

Initial audit uses existing marketplace Relay surfaces first:

- `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-locations.tsx` already queries `marketplaceLocations` with search boundary and resource-type inputs.
- `src/web/apps/webapp/src/components/location/marketplaceLocation/marketplace-location.tsx` is the preferred location-level marketplace detail surface.
- `src/web/apps/webapp/src/components/marketplaceProductBooking/` and `src/web/apps/webapp/src/components/marketplaceProductSubscription/` are the preferred customer purchase/self-service surfaces.

Potential gaps to verify during implementation:

- Explicit marketplace-enabled and customer-bookable filters for aggregate discovery. The first implementation honors explicit `marketplaceEnabled` and `customerBookable` flags when they are present in Relay data, and keeps existing results when the current schema does not expose those flags yet.
- Customer-facing organization context and selected-location URL key for aggregate marketplace cards/details.
- Policy-derived eligible customer self-service actions and customer-safe unavailable reason codes.
- Failure/recovery logging context for failed discovery, purchase hub load, and self-service action paths.

No generated Relay artifact should be edited by hand. If query selections or schema fields change, regenerate through the established web generation path.

## No-Redirect Decisions

- The feature rule is no URL redirects from webapp during this phase.
- Removed, relocated, unsupported, and owner-specific paths opened in no-subdomain webapp must resolve in place with customer-safe content.
- Existing account/auth behavior must be reviewed carefully. Any current sign-out return behavior that uses browser navigation needs an explicit shared-account decision before it can remain.
- Unsupported path states must avoid private administration controls and avoid automatic redirection to `webapp-teams`, `webapp-spaces`, or custom-subdomain URLs.

## Owner-Specific Marketplace Regression

- Existing custom-subdomain marketplace browse and purchase behavior is protected.
- No-subdomain aggregate discovery must not inject cross-organization discovery into owner-specific custom-subdomain pages.
- Regression validation should cover owner storefront entry, location/product browse, booking entry, subscription entry, and purchase-detail links.

## Customer-Owned Data Preservation

Customer-owned data must not become unreachable while private/admin routes are removed or hidden from customer navigation. Review these data classes before route changes:

- Marketplace bookings and booking detail links.
- Marketplace subscriptions and subscription detail links.
- Invoice/payment/refund state surfaced through marketplace purchase flows.
- Account settings, authentication, notifications, and profile state.
- Historical purchase links that may include organization-specific marketplace paths.

## Marketplace Link Helper Audit

`src/web/apps/webapp/src/components/links/index.ts` currently separates:

- Aggregate/no-subdomain marketplace location links: `getMarketplaceLocationLink(undefined, locationId)` returns `/marketplace/locations/{locationId}`.
- Owner/organization-scoped marketplace product and purchase links: helpers use `organizationCustomDomain` when `isCustomDomain` is false, and short owner-specific paths when `isCustomDomain` is true.
- Customer booking/subscription hub links: `getMarketplaceBookingsLink` and `getMarketplaceSubscriptionsLink` can produce no-subdomain organization-scoped paths or custom-domain short paths.
- Private organization/admin links: `getOrganization*` helpers generate `/organizations/{organizationCustomDomain}/...` routes that should be absent from primary customer navigation after cleanup.

Aggregate URL assumption: selected aggregate locations should prefer `/marketplace/locations/{locationId}` or another explicit aggregate path that remains shareable and does not depend on private organization administration routes. Product purchase links still need validation because several existing helpers depend on `organizationCustomDomain` for no-subdomain product paths.

## Marketplace GraphQL/Relay Audit

- Existing `MarketplaceLocations` is the aggregate discovery starting point and should be extended only if current fields cannot express eligibility, organization context, or insights.
- Existing `MarketplaceLocation` detail should stay the location-level marketplace surface for aggregate-selected locations.
- Booking/subscription detail components should expose customer self-service actions only through existing policy/status fields where available.
- If missing fields are found, update source GraphQL/domain contracts first, then regenerate GraphQL/Relay artifacts.

## US3 Customer Self-Service Notes

- Existing customer booking and subscription surfaces already use owned marketplace booking/subscription GraphQL queries and cancellation mutations. This slice adds explicit UI eligibility helpers plus telemetry around cancellation start/rejection; no new GraphQL action eligibility fields were available or required for this pass.
- Cross-organization aggregate hub behavior remains constrained by the current organization-scoped marketplace query shape. Hub count/sign-in guardrails are isolated in code, and broader backend/query support should be added through the owning GraphQL/domain source before Relay regeneration.

## US4 Navigation Cleanup Notes

- The no-organization customer navigation currently exposes only Home, Notifications, and Settings. Private organization, resource, booking-admin, subscription-admin, and team administration entries remain absent from this shell.
- Unsupported marketplace paths can render the in-place unsupported path state at `/marketplace/unsupported/...` without URL redirection. Owner-specific marketplace paths remain protected by route ownership classification rather than being rewritten.

## US5 Product Direction Notes

- Aggregate discovery keeps the actual browsing surface as the first screen. Desktop uses split map/list comparison, while mobile keeps a map-first interaction with compact selected-location cards.
- Marketplace cards avoid placeholder text for missing address/capacity/area data and keep the location detail link as the customer booking entry point.

## Final Before-And-After Summary

- Before: no-subdomain webapp mixed customer marketplace entry with private organization and owner-administration assumptions. After: no-subdomain ownership is documented as aggregate customer marketplace discovery plus customer purchase/self-service surfaces.
- Preserved: custom-subdomain owner-specific marketplace browse, product, booking, subscription, and purchase-detail paths remain protected and unchanged.
- Removed from webapp routing: private `/organizations/**` admin routes, `/msteams/**` routes, organization rootPages, MS Teams install/start rootPages, MS Teams OpenAPI web clients, the Teams SDK dependency, Teams token/provider wiring, Teams proxy public-path handling, and orphaned organization-admin shell/navigation components.
- Hidden/removed from customer navigation: private organization, resource, team, booking-admin, subscription-admin, spaces, and MS Teams administration entries are classified out of webapp customer navigation; marketplace unsupported paths remain customer-safe and in place.
- Remaining webapp responsibilities: aggregate marketplace discovery, marketplace location/product purchase entry, signed-in customer bookings/subscriptions, account/auth/shared settings, notifications, and customer-safe unsupported states.
