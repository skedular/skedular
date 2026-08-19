# Spaces Help Source Inventory

## Existing Help Shell

The existing Spaces help shell used generic Skedular copy. It did not explain marketplace setup, product publishing, operator bookings, subscriptions, refunds, payment setup, or the difference between Spaces and Teams. Replace it with Spaces-specific help.

Baseline files already existed:

- `src/web/apps/webapp-spaces-help/src/app/page.mdx`
- `src/web/apps/webapp-spaces-help/src/content/index.mdx`
- `src/web/apps/webapp-spaces-help/src/content/_meta.ts`

## Reviewed Product Surfaces

| Product surface               | Route or source area                                                                            | Help mapping                                          |
| ----------------------------- | ----------------------------------------------------------------------------------------------- | ----------------------------------------------------- |
| Spaces entry                  | `/`, `/organizations/[organizationCustomDomain]`                                                | Overview, Marketplace setup                           |
| Add marketplace organization  | `/organizations/add-marketplace`                                                                | Marketplace setup, Spaces guides                      |
| Admin                         | `/organizations/[organizationCustomDomain]/admin`                                               | Marketplace setup                                     |
| Marketplace setup             | `/setup-marketplace`, `/msteams/.../marketplace-setup`, `/marketplace-public`                   | Marketplace setup                                     |
| Locations list/add/detail     | `/locations`, `/locations/add-marketplace`, `/locations/[locationId]`                           | Locations and resources                               |
| Resources add                 | `/resources/add`                                                                                | Locations and resources                               |
| Products list/add/detail      | `/products`, `/products/add`, `/products/[productId]`                                           | Commerce operations                                   |
| Bookings list/add/detail      | `/bookings`, `/bookings/add`, `/bookings/[bookingId]`                                           | Commerce operations                                   |
| Subscriptions list/detail     | `/subscriptions`, `/subscriptions/[subscriptionId]`                                             | Commerce operations                                   |
| Refunds                       | `rootPages/organizations/organization/refunds/page.tsx`                                         | Commerce operations, content gap for direct app route |
| Bank accounts list/add/detail | `/bank-accounts/add`, `/bank-accounts/[organizationBankAccountId]`                              | Analytics, payments, integrations                     |
| Stripe Connect accounts       | `/stripe-connect-accounts/add`, `/stripe-connect-accounts/[organizationStripeConnectAccountId]` | Analytics, payments, integrations                     |
| Availability dashboard        | `/availability`                                                                                 | Analytics, payments, integrations                     |
| Analytics                     | `/analytics`                                                                                    | Analytics, payments, integrations                     |
| Users list/detail             | `/users`, `/users/[customerId]`                                                                 | Marketplace setup, Commerce operations                |
| SSO sign-in                   | `/sso-signin`                                                                                   | Analytics, payments, integrations                     |
| Slack and Microsoft Teams     | `/install-slack`, `/msteams/*`                                                                  | Analytics, payments, integrations                     |
| Auth and welcome              | `/signin`, `/signup`, `/auth/*`, `/callback`, `/welcome`                                        | Marketplace setup                                     |

## Important States To Explain

- Spaces is for operators who publish and run marketplace/co-working spaces.
- Product availability, booking state, subscription state, refund state, and payment setup can affect what customers can buy.
- Payment and integration help must stay public-safe and avoid secrets or internal settlement details.
- Some refund surfaces are visible in root page code but need direct route confirmation.

## Coverage Table

| Help page                             | Covers                                                                                                                                | Remaining gap                               |
| ------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------- |
| `index.mdx`                           | Purpose, audience, boundaries                                                                                                         | None                                        |
| `marketplace-setup.mdx`               | Entry, add marketplace org, admin, setup, auth/welcome                                                                                | Marketplace-public route state detail       |
| `locations-resources.mdx`             | Locations, resources, zones, floor plans                                                                                              | Detail of every resource field              |
| `commerce-operations.mdx`             | Products, bookings, subscriptions, refunds                                                                                            | Exact refund status labels need review      |
| `analytics-payments-integrations.mdx` | Analytics, availability, payments, bank accounts, Stripe, SSO, Slack, Microsoft Teams                                                 | Provider-specific setup failures            |
| `page-reference.mdx`                  | Page-by-page Spaces reference for all major operator surfaces                                                                         | None                                        |
| `operator-examples.mdx`               | Practical operator examples for locations, products, bookings, subscriptions, refunds, payment setup, and empty marketplace diagnosis | None                                        |
| `payment-and-refund-safety.mdx`       | Public-safe payment setup and refund handling guidance                                                                                | Provider-specific/accounting process detail |
| `actions-reference.mdx`               | Spaces action reference for organization, location, resource, product, booking, subscription, refund, and payment actions             | None                                        |
| `troubleshooting.mdx`                 | Booking, product, subscription, refund, payment, Microsoft Teams, analytics issue checks                                              | Provider-specific failures                  |
| `support-handoff.mdx`                 | Spaces support templates and triage notes                                                                                             | None                                        |
| `screenshot-plan.mdx`                 | Screenshot capture rules and required Spaces screenshot list                                                                          | Screenshots not captured in this slice      |
| `review-qa.mdx`                       | Product/support/engineering/copy review matrix                                                                                        | None                                        |
| `glossary.mdx`                        | Marketplace operator definitions for shared terms                                                                                     | None                                        |
| `review-checklists.mdx`               | Marketplace organization, location, product, booking, subscription, refund, and payment checks                                        | None                                        |
| `faq.mdx`                             | Common Spaces questions                                                                                                               | None                                        |
| `spaces-guides.mdx`                   | Step-by-step operator workflows                                                                                                       | Screenshots needed                          |
| `content-gaps.mdx`                    | Known unclear flows                                                                                                                   | Tracked in gap register                     |
