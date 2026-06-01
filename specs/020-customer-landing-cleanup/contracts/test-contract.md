# Contract: Verification And Regression Coverage

## Required Verification Areas

- Capability inventory completeness.
- No-subdomain aggregate marketplace discovery.
- Marketplace eligibility filtering.
- Location-level marketplace purchase entry from aggregate discovery.
- Customer booking and subscription hub across organizations.
- Policy-bound customer self-service actions.
- In-place unsupported path handling with no URL redirects.
- Existing custom-subdomain owner-specific marketplace regression.
- Removal or hiding of private/admin navigation from webapp customer surfaces.

## Suggested Web Tests

- Route resolver tests for no-subdomain vs custom-subdomain entry behavior.
- Component tests for aggregate marketplace empty, partial-data, and multi-location states.
- Component tests for location cards and map-selected cards using the same compact anatomy.
- Component tests for customer booking/subscription action eligibility and unavailable-action messaging.
- Tests asserting unsupported paths render in place and do not call browser redirect APIs.
- Regression tests for existing custom-subdomain storefront entry behavior.

## Suggested Backend/Contract Tests If Schema Changes

- Owning-domain unit tests for new eligibility or policy calculations.
- Integration tests for cross-domain marketplace booking/subscription queries through public service or GraphQL boundaries.
- Repository-layer assertions only; no raw `DbContext` assertions in integration tests.
- Generated GraphQL and Relay artifact validation after schema/query updates.

## Manual Validation

- Browse aggregate marketplace without a custom subdomain.
- Browse owner-specific marketplace with a custom subdomain and confirm unchanged behavior.
- Purchase or start purchase for a product from an aggregate-selected location.
- View bookings and subscriptions as a customer with purchases across more than one organization.
- Attempt unavailable cancel/change/refund actions and confirm customer-safe messaging.
- Open removed admin paths and verify in-place no-redirect behavior.
