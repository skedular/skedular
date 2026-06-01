# Customer Help Source Inventory

## Existing Help Shell

The existing Customer help shell used generic Skedular marketing copy. It did not explain the public Customer app, the marketplace route structure, customer bookings, subscriptions, or self-service boundaries. Replace it with app-specific help.

Baseline files already existed:

- `src/web/apps/webapp-help/src/app/page.mdx`
- `src/web/apps/webapp-help/src/content/index.mdx`
- `src/web/apps/webapp-help/src/content/_meta.ts`

## Reviewed Product Surfaces

| Product surface | Route or source area | Help mapping |
| --- | --- | --- |
| Customer landing and app entry | `/`, `src/rootPages/page.tsx` | Overview, Discovery |
| Customer-facing subdomain resolution | `customer-facing-subdomain/*` | Discovery, Content gaps for domain behavior detail |
| Marketplace organization page | `/marketplace/organizations/[organizationCustomDomain]` | Discovery |
| Customer organization bookings | `/marketplace/organizations/[organizationCustomDomain]/bookings` | Bookings and subscriptions |
| Customer organization subscriptions | `/marketplace/organizations/[organizationCustomDomain]/subscriptions` | Bookings and subscriptions |
| Location detail | `/marketplace/locations/[locationId]` | Discovery, Customer guides |
| Location floor plans | `/marketplace/locations/[locationId]/floorPlans` | Discovery, Customer guides |
| Product detail | `/marketplace/products/[productId]` | Products |
| Product booking form | `/marketplace/products/[productId]/book` | Products, Customer guides |
| Product subscription form | `/marketplace/products/[productId]/subscribe` | Products, Customer guides |
| Personal booking list | `/marketplace/bookings` | Bookings and subscriptions |
| Personal booking detail | `/marketplace/bookings/[bookingId]` | Bookings and subscriptions, Customer guides |
| Personal subscription list | `/marketplace/subscriptions` | Bookings and subscriptions |
| Personal subscription detail | `/marketplace/subscriptions/[subscriptionId]` | Bookings and subscriptions, Customer guides |
| Unsupported marketplace path | `/marketplace/unsupported/[[...path]]` | Account and support |
| Notifications | `/notifications` | Account and support |
| Settings | `/settings` | Account and support |
| Sign in and sign up | `/signin`, `/signup`, `/auth/signin`, `/auth/signup`, `/callback` | Account and support |
| Welcome | `/welcome` | Account and support |
| Slack install/success pages | `/install-slack`, `/slack-success-install` | Account and support |

## Important States To Explain

- A location or product may not be available to every customer.
- Booking and subscription actions may depend on product rules, availability, policy, and payment state.
- Cancellation or refund options may not appear for every booking.
- Unsupported routes should send the user back to the right marketplace or support path.

## Coverage Table

| Help page | Covers | Remaining gap |
| --- | --- | --- |
| `index.mdx` | Purpose, audience, boundaries | None |
| `discovery.mdx` | Landing, marketplace organization pages, locations, floor plans | Exact subdomain fallback behavior |
| `products.mdx` | Product details, booking form, subscription form | Product policy wording varies by operator |
| `bookings-and-subscriptions.mdx` | Booking list/detail, subscription list/detail, cancellation/refund guidance | Exact status labels need screenshot review |
| `account-and-support.mdx` | Notifications, settings, auth, welcome, Slack, unsupported paths | Provider-specific auth failure copy |
| `page-reference.mdx` | Page-by-page customer reference for all major customer surfaces | None |
| `booking-examples.mdx` | Practical customer examples for day pass, meeting room, subscription, cancellation, renewal failure, missing booking, and refund question | Exact UI labels need screenshot review |
| `states-and-policies.mdx` | Booking, subscription, payment, cancellation, and refund state explanations | Exact UI labels need screenshot review |
| `actions-reference.mdx` | Customer action reference for browse, book, subscribe, cancel, refund help, payment retry, and support | None |
| `troubleshooting.mdx` | Missing bookings/subscriptions, payment issues, blocked booking, cancellation/refund next steps | Provider-specific payment/auth errors |
| `support-handoff.mdx` | Customer support templates and support triage boundaries | None |
| `screenshot-plan.mdx` | Screenshot capture rules and required customer screenshot list | Screenshots not captured in this slice |
| `review-qa.mdx` | Product/support/engineering/copy review matrix | None |
| `glossary.mdx` | Customer-facing definitions for shared terms | None |
| `review-checklists.mdx` | Pre-booking, pre-subscription, pre-cancellation, support, and post-change checks | None |
| `faq.mdx` | Common customer questions | None |
| `customer-guides.mdx` | Step-by-step public customer workflows | Screenshots needed |
| `content-gaps.mdx` | Known unclear flows | Tracked in gap register |
