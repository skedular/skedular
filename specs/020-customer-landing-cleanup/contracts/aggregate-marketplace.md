# Contract: Aggregate Marketplace Experience

## Purpose

Defines the customer-facing behavior for webapp when no coworking-space owner custom subdomain is present.

## Entry Point Rules

- No-subdomain webapp is the aggregate marketplace surface.
- Coworking-space owner custom subdomains continue to serve the existing owner-specific marketplace and remain unchanged.
- Private organization custom-domain behavior is not redesigned by this feature.
- Webapp MUST NOT perform URL redirects in this phase.

## Aggregate Discovery Contract

The aggregate discovery page MUST:

- Show only marketplace-enabled customer-bookable locations.
- Support browsing through a map-and-list experience.
- Present customer-relevant location details, including name, organization context, address or map context, imagery when available, availability cues when available, and practical insights when available.
- Exclude private, non-marketplace, and non-customer-bookable locations.
- Provide useful empty states when no eligible locations are available.
- Avoid private organization, coworking-owner, resource-management, and admin controls.

## Location-Level Marketplace Contract

When a customer selects a location from aggregate discovery, webapp MUST:

- Keep the customer within webapp without redirecting.
- Provide a location-level marketplace experience comparable to the existing owner-specific customer-facing marketplace model.
- Show customer-facing products available for purchase at that selected location.
- Allow booking or subscription purchase through marketplace-style flows when policy and product configuration allow.
- Preserve selected location context in the URL or equivalent shareable path shape.

## Customer Bookings And Subscriptions Contract

Signed-in customers MUST be able to:

- See marketplace bookings across organizations.
- See marketplace subscriptions across organizations.
- Distinguish organization, location, product, schedule or renewal context, payment state, and current status.
- Use eligible self-service actions for cancel, change, and refund only when the relevant policy allows.
- Understand unavailable actions without seeing internal policy mechanics or private administration controls.

Unauthenticated users MUST be prompted to sign in before customer-specific booking or subscription data is shown.

## Owner-Specific Marketplace Regression Contract

Existing custom-subdomain owner-specific marketplace pages MUST:

- Continue to browse the owner-specific customer marketplace as before.
- Continue to show existing products and customer purchase paths as before.
- Avoid aggregate cross-organization discovery unless the user is on the no-subdomain webapp surface.
- Pass current browse and purchase validation after aggregate marketplace changes.

## No-Redirect Unsupported Path Contract

When webapp receives a removed, unsupported, or owner-specific path during this phase, it MUST:

- Stay on the current URL.
- Show customer-safe explanatory or unavailable content.
- Avoid exposing private administration controls.
- Avoid linking users through an automatic redirect.

## Observability Contract

Implementation MUST log meaningful behavior at these boundaries:

- Aggregate marketplace discovery load, including whether eligible locations were available.
- Location selection and transition into location-level marketplace details.
- Customer booking/subscription hub load and self-service action attempts.
- In-place unsupported path handling.
- Owner-specific custom-subdomain marketplace regression-sensitive entry resolution.

Logs must use structured fields, preserve correlation context, and avoid customer-sensitive payloads.
