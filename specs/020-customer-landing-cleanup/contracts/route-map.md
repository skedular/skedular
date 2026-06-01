# Contract: Route Ownership Map

## Purpose

Defines the planned route ownership categories for webapp cleanup. Exact route lists will be generated during task execution from the current route tree.

## Keep In Webapp

- `/` as no-subdomain aggregate marketplace landing/discovery.
- `/marketplace/locations/[locationId]` and related customer-facing location detail paths when used by aggregate marketplace.
- `/marketplace/products/[productId]`, `/book`, `/subscribe`, and booking-detail paths when they represent customer marketplace purchase flows.
- `/marketplace/bookings` and `/marketplace/subscriptions` as signed-in customer self-service surfaces.
- Authentication, callback, account, notification, and other shared entry points required for customer access.

## Protect Unchanged

- Existing owner-specific custom-subdomain marketplace behavior.
- Existing owner-specific customer-facing product browse and purchase behavior.

## Move Or Remove From Customer Navigation

- Private organization booking creation and editing.
- Coworking-space owner booking management.
- Coworking-space owner subscription management.
- Resource, zone, tag, product setup, payment setup, user management, and admin workflows.
- MS Teams organization administration routes that are now product-owned by `webapp-teams`.
- Co-working administration workflows owned by `webapp-spaces`.

## In-Place Unsupported Handling

Routes removed from webapp or no longer supported in customer navigation MUST resolve without URL redirects in this phase. They should show a customer-safe unavailable state or remain available only when classified as a shared entry point.

## Route Inventory Output

Implementation tasks should produce a reviewable route map with:

- Route pattern.
- Owner app.
- Disposition.
- Customer impact.
- Admin impact.
- URL handling.
- Approval status.
- Verification notes.
