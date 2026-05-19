# Route Retirement Register: Split Web Products

No route may be deleted until its backend-originated return URL risk is checked, its action is recorded here, and the user has reviewed the moved route in the target app.

## Rules

- Backend services, APIs, and contracts remain unchanged for this feature.
- Route deletion is blocked when payment, authentication, notification, external callback, or backend-generated redirect flows still target the old URL.
- If a backend-originated return path exists, the route must remain as `keep` or `transition` until an app-specific target URL strategy exists.
- For every moved journey, the original WebApp route remains available during first review. Route removal, redirect, or blocking happens only after manual approval.
- Any app-specific base URL configuration required later must be documented without changing backend ownership in this feature.

## Register

| route | current_owner | target_owner | action | backend_originated_return_url_audit | configuration_source | removal_condition | manual_review_path | notes |
|-------|---------------|--------------|--------|-------------------------------------|----------------------|-------------------|--------------------|-------|
| / | WebApp | WebApp | keep | not applicable | existing WebApp base URL | none | http://localhost:15000 | Customer-facing public discovery root remains in WebApp. Custom-domain storefront resolution is now documented in WebApp code, but existing behaviour is preserved. |
| /auth/** and /api/auth/** | WebApp | WebApp | keep | blocked | WorkOS/AuthKit and frontend auth URLs | none | http://localhost:15000/auth/signin | Auth entry points remain shared WebApp entry points. |
| /callback | WebApp | WebApp | keep | blocked | External callback assumptions unknown | none | http://localhost:15000/callback | Do not delete until callback source ownership is audited. |
| /api/v1/graphql | WebApp | WebApp | keep | blocked | Existing WebApp GraphQL endpoint/base URL | none | http://localhost:15000/api/v1/graphql | Gateway route remains with WebApp unless app-specific GraphQL endpoints are designed. |
| /api/v1/core/uploadPrivateAccessFile and /api/v1/core/uploadPublicAccessFile | WebApp | WebApp | keep | blocked | Existing WebApp upload proxy URLs | none | http://localhost:15000 | Upload return/consumer usage must be audited before moving. |
| /api/v1/organization/xero/oauth/start | WebApp | transition | transition | blocked | Xero OAuth redirect configuration | App-specific organisation app URL strategy exists and is verified. | http://localhost:15000/api/v1/organization/xero/oauth/start | External provider return flow; keep adapter until Spaces/Teams ownership is explicit. |
| /billing-and-payment and /msteams/billing-and-payment | WebApp | transition | keep | blocked | Payment return URLs and account billing links | Payment flows are classified by customer/private/marketplace ownership. | http://localhost:15000/billing-and-payment | High risk. Do not delete during early slices. |
| /notifications and /msteams/notifications | WebApp | transition | keep | blocked | Notification deep links | Notification target URLs are app-specific and verified. | http://localhost:15000/notifications | Deep links may point to multiple product journeys. |
| /settings and /msteams/settings | WebApp | WebApp | keep | blocked | Account settings links | none | http://localhost:15000/settings | Shared account surface stays in WebApp for now. |
| /organizations/add-private | WebApp | WebApp Teams | keep | blocked | Unknown onboarding/auth return usage | User approves the Teams route and backend return URL audit passes. | http://localhost:15000/organizations/add-private and http://localhost:15002/organizations/add-private | Dual-run first. Teams has a review shell; the data-backed WebApp form remains in place until Relay/import migration is handled. Do not redirect or delete before manual review. |
| /organizations/add-marketplace | WebApp | WebApp Spaces | keep | blocked | Unknown onboarding/auth return usage | User approves the Spaces route and backend return URL audit passes. | http://localhost:15000/organizations/add-marketplace | Dual-run first. Do not redirect or delete before manual review. |
| /organizations/add-individual | WebApp | transition | keep | blocked | Unknown onboarding/account return usage | Product ownership for individual organisations is decided. | http://localhost:15000/organizations/add-individual | Needs product decision before migration. |
| /organizations/setup | WebApp | transition | keep | blocked | Organisation onboarding links | Setup flow is split by organisation type. | http://localhost:15000/organizations/setup | Cross-product setup route. |
| /organizations/[organizationCustomDomain]/teams/** | WebApp | WebApp Teams | keep | blocked | Unknown notification/auth/deep-link usage | User approves a data-backed Teams route and backend return URL audit passes. | http://localhost:15000/organizations/example/teams and http://localhost:15002/teams | Dual-run first. Private organisation team management. |
| /organizations/[organizationCustomDomain]/users/** | WebApp | WebApp Teams | transition | blocked | Unknown notification/auth/deep-link usage | Teams route exists and old route redirects or blocks after audit. | http://localhost:15000/organizations/example/users | Private organisation user management. |
| /organizations/[organizationCustomDomain]/sso-signin | WebApp | WebApp Teams | transition | blocked | SSO/auth return URLs | Teams SSO route exists and auth return path is verified. | http://localhost:15000/organizations/example/sso-signin | High auth risk. |
| /organizations/[organizationCustomDomain]/products/** | WebApp | WebApp Spaces | keep | blocked | Unknown notification/payment/deep-link usage | User approves the Spaces product list/add/edit routes and backend return URL audit passes. | http://localhost:15000/organizations/example/products, http://localhost:15004/organizations/example/products, http://localhost:15004/organizations/example/products/add, and http://localhost:15004/organizations/example/products/example-product-id | Dual-run first. Product list/add/edit UI is copied into Spaces; original WebApp product routes stay available for side-by-side review. Marketplace/co-working operator product management; no Teams exposure. |
| /organizations/[organizationCustomDomain]/setup-marketplace | WebApp | WebApp Spaces | transition | blocked | Unknown onboarding/deep-link usage | Spaces marketplace setup route exists and old route redirects or blocks after audit. | http://localhost:15000/organizations/example/setup-marketplace | Spaces-owned. |
| /organizations/[organizationCustomDomain]/stripe-connect-accounts/** | WebApp | WebApp Spaces | transition | blocked | Stripe Connect return URLs | Spaces Stripe Connect route exists and Stripe return URL strategy is verified. | http://localhost:15000/organizations/example/stripe-connect-accounts/add | High payment return risk. |
| /organizations/[organizationCustomDomain]/subscriptions/** | WebApp | WebApp Spaces | transition | blocked | Subscription/payment links | Spaces subscription operator route exists and payment/subscription returns are verified. | http://localhost:15000/organizations/example/subscriptions | High payment/subscription risk. |
| /organizations/[organizationCustomDomain]/bookings/** | WebApp | transition | keep | blocked | Booking/payment/notification links | Booking admin ownership is split between Teams and Spaces. | http://localhost:15000/organizations/example/bookings | Split by private vs marketplace booking flow. |
| /organizations/[organizationCustomDomain]/locations/** | WebApp | transition | keep | blocked | Location deep links | Location ownership is split by private vs marketplace/co-working organisation type. | http://localhost:15000/organizations/example/locations | Split add-private to Teams and add-marketplace/operator location setup to Spaces. |
| custom-domain root | WebApp | WebApp | keep | blocked | Existing WebApp custom-domain storefront URL strategy | none | custom domain mapped to WebApp | Co-working custom domains keep the existing storefront. Private organisation custom-domain support has a shell/resolver path but still needs data-backed organisation-type detection before activation. |
| /marketplace/** | WebApp | WebApp | keep | blocked | Customer payment, booking, subscription, and notification links | none | http://localhost:15000/marketplace/products/example | Customer-facing marketplace stays in WebApp. |
| /msteams/** | WebApp | transition | keep | blocked | Microsoft Teams integration URLs | Microsoft Teams hosted app strategy is defined. | http://localhost:15000/msteams | Do not move/delete as part of first browser app slices. |

## Known High-Risk Areas To Audit Per Slice

- Payment success, cancellation, failed payment, and invoice return paths.
- Authentication sign-in, callback, sign-out, and account settings return paths.
- Notification links that deep-link into web routes.
- External provider callbacks or redirects that build frontend URLs from environment configuration.

## Completed Slice Route Safety Summary

| slice | old_route | action | return_url_status | transition_path |
|-------|-----------|--------|-------------------|-----------------|
| foundation | none | keep | not applicable | No routes moved or removed. |
| teams-organisation-selection-foundation | none | keep | not applicable | New Teams route only. |
| teams-team-management-shell | /organizations/[organizationCustomDomain]/teams/** | keep | blocked | Old WebApp team route remains available while `http://localhost:15002/teams` is reviewed. |
| spaces-organisation-selection-foundation | none | keep | not applicable | New Spaces route only. |
| spaces-products-operator-shell | /organizations/[organizationCustomDomain]/products/** | keep | blocked | Old WebApp product route remains available while `http://localhost:15004/products` is reviewed. |
| webapp-customer-facing-entry-foundation | / and custom-domain root | keep | blocked | WebApp behaviour is preserved; private organisation custom-domain activation waits for data-backed organisation-type detection. |
| shared-neutral-foundations | none | keep | not applicable | Shared extraction only; no route changes. |

## Unresolved Return URL Blockers

- `/organizations/[organizationCustomDomain]/teams/**`: notification, authentication, and deep-link usage is still unknown. Keep the old route until a data-backed Teams route exists and the user approves route transition.
- `/organizations/[organizationCustomDomain]/products/**`: payment, notification, and deep-link usage is still unknown. Keep the old route until a data-backed Spaces route exists and the user approves route transition.
- `custom-domain root`: private organisation customer-facing behaviour needs data-backed organisation-type detection before it can diverge from the existing co-working storefront.
- Payment, authentication, notification, Xero OAuth, Stripe Connect, subscription, and Microsoft Teams-hosted routes remain blocked for deletion or redirect until app-specific target URL configuration is designed and verified.

## Deletion Guard

No completed slice deletes or redirects an existing WebApp route. Every completed route movement uses `keep`, and every risky route keeps `backend_originated_return_url_audit` as `blocked` until a passed audit exists.
