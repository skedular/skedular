---
id: spaces-faq
title: "FAQs"
description: "Quick answers to common questions about setting up, selling, booking, and managing workspace with Skedular Spaces."
product: spaces
category: faqs
slug: spaces-faq
articleKind: faq
publicationState: published
evidenceRefs:
  - src/web/apps/webapp-spaces/src/components/organization/organizationAnalytics/organization-analytics.tsx
  - doc-resources/booking.md
  - doc-resources/product.md
  - doc-resources/subscriptions.md
  - doc-resources/billing-and-payouts.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-17
---

## Getting started and workspace setup

### What do I set up before I start selling workspace?

Set up the Organization, its Locations, and the bookable Resources in those Locations. Then configure Products and Prices, complete the marketplace profile, and activate the Products customers can purchase. Start with [Getting Started](/docs/spaces/getting-started), then follow [workspace setup](/docs/spaces/workspace-setup) and [Products and pricing](/docs/spaces/products-and-marketplace/products-and-pricing).

### Can I manage more than one Location?

Yes. An Organization can manage multiple Locations. Each Location has its own Resources and location-level Booking and Availability context. [Locations and resources](/docs/spaces/workspace-setup/locations-and-resources) explains the setup.

### What types of Resources can I manage?

Spaces supports Resource types such as desks, rooms, parking, and other bookable Resources. Resource type is separate from Product Tags, which connect Resources to Products.

### Can a Resource belong to more than one Zone?

Yes. Zones are optional Organization-level groups. A Resource can belong to no Zone, one Zone, or multiple Zones. Zones organize Resources; they do not determine Product eligibility. See [Zones and floor plans](/docs/spaces/workspace-setup/zones-and-floor-plans).

### Can I use floor plans?

Yes. A Location can have floor plans that visually represent Resources. Floor plans are optional, so you can configure Locations and Resources and continue to Products without adding one.

## Products and marketplace

### How does a Product decide which Resources customers can book?

Operators assign Product Tags to Resources and select Product Tags on a Product. A Resource with at least one selected Product Tag can enter the Product's eligible pool, and Availability still determines whether it can be booked for the requested time. Selecting multiple Product Tags broadens the matching pool rather than requiring every tag. See [Products and pricing](/docs/spaces/products-and-marketplace/products-and-pricing).

### Can I offer different Prices for the same Product?

Yes. A Product can have multiple Prices with different durations, quantities, payment methods, billing modes, cancellation policies, and purchase cadences. Customers choose the Price that fits the offering.

### Can I change a Price after a customer has purchased it?

Yes. Pricing changes apply to future purchases through a new Product version. Existing Bookings keep the commercial terms used at purchase, and an existing auto-renewing Subscription keeps its current terms until renewal. See [Products and pricing](/docs/spaces/products-and-marketplace/products-and-pricing) for the detailed behavior.

### Can I keep my marketplace private?

Yes. A private Organization is not listed for public marketplace discovery, so Customers cannot find it through the public Skedular marketplace. Operators can still manage the private workspace, while Product activation remains a separate control and does not automatically make the Organization public or activate every Product.

### Do Customers become members of my Organization?

No. A Customer is a registered Skedular user who purchases a Product. That purchase does not make the person an Organization member. Organization membership is a separate relationship for people who belong to and manage the Organization.

### Does a Customer need a Skedular account to purchase?

Yes. Customers must create or use a registered Skedular account to sign in and complete a Product purchase. Having that account does not make them a member of the Organization they purchase from.

## Bookings and Subscriptions

### How long can a single Booking be?

An individual Booking cannot span more than one UTC day. Bookings use 15-minute intervals; a longer customer arrangement is represented through a Subscription and its associated Booking instances.

### What happens when a customer purchases for more than one day?

A recurring purchase creates a Subscription with associated recurring Booking instances for the scheduled workspace use. The Subscription is the commercial arrangement; each Booking instance represents a scheduled reservation.

### What is the difference between a Booking and a Subscription?

A Booking reserves Resources for one defined period. A Subscription groups recurring customer access and the Booking instances generated for its cycles.

### Can Subscriptions renew automatically?

Yes, when the selected Price supports auto-renewal and it is enabled. Spaces continues the arrangement at renewal when the Product still has an available auto-renewable pricing option. If renewal can no longer proceed, the Subscription does not silently switch to an unrelated offering.

### Does cancelling a Subscription automatically refund the customer?

No. Cancellation timing, future billing, and refunding a confirmed payment are separate decisions. An eligible refund depends on the Price's cancellation policy and payment status. See [Subscriptions](/docs/spaces/bookings/subscriptions) and [Refunds](/docs/spaces/billing-and-payments/refunds).

### Are recurring Booking instances counted separately in Analytics?

Yes. Analytics reports the individual Booking instances generated for recurring and Subscription activity. It does not count a Subscription as one Booking. See [Operator analytics](/docs/spaces/analytics).

## Billing and payments

### What payment methods can Customers use?

Customers can use card payments through the operator's connected Stripe account or manual bank transfer when the Organization and selected Price support those methods. Xero is an accounting and invoicing integration, not a Customer payment method. See [Payment methods](/docs/spaces/billing-and-payments/payment-methods).

### When is the first Subscription invoice created?

The first invoice is generated when the Subscription starts for both **Upfront** and **In arrears** billing. Later Upfront invoices follow the Subscription purchase period, while later In-arrears invoices follow the Organization's billing cycle. The invoice due date is separate and determines when each invoice must be paid.

### How are bank-transfer payments confirmed?

The Customer sends the transfer outside Skedular using the Organization's payment instructions. The payment remains **Pending** until an operator uses **Confirm Booking Payment** or **Reject Booking Payment** in Skedular. Skedular does not automatically know that an external transfer has arrived.

### Does Skedular hold Customer payments?

No. Skedular does not hold Customer funds. Card payments are processed through the operator's connected Stripe account, while bank transfers are sent to the Organization's configured bank account.

### How do refunds work?

Cancellation and refund are separate. The Price cancellation policy and confirmed payment determine refund eligibility and amount, while the payment path determines how the refund is processed. See [Refunds](/docs/spaces/billing-and-payments/refunds).

### Can I issue a partial refund?

Yes, when the payment is confirmed and the cancellation policy allows a refund. An operator can approve an amount lower than the policy-derived refundable amount, but not higher than the current refundable amount.

### What does Xero do?

Xero receives connected invoices and supports accounting, payment recording, reconciliation, and authorized credit notes. Skedular remains responsible for the Product, Booking, Subscription, cancellation, and local refund decisions. See [Xero accounting](/docs/spaces/billing-and-payments/xero-accounting).

## Customers and Organization access

### Who can manage a Skedular Spaces Organization?

Organization users with the required roles and permissions manage workspace setup, Products, Bookings, payments, and analytics. A registered Skedular account alone does not grant access to the operator interface.

### Can Customers access the Organization administration area?

No. Marketplace Customers can purchase Products and are associated with the resulting commercial activity, but they are not automatically Organization members and do not gain operator access through a purchase.

### Can operators see who purchased a Product?

Yes. Booking and Subscription workflows expose the registered Customer identity and the customer information available to the operator in that workflow. See [Customers](/docs/shared/marketplace/customers) for the identity distinction.

## Analytics and operations

### Where can I see Booking activity?

Open **Analytics** in the Organization interface when your Organization permissions allow it. Organization views include Booking Insights and Member Attendance Insights; Location views include Booking Insights, Desk Occupancy Insights, and Resource Availability Insights. Continue to [Operator analytics](/docs/spaces/analytics) for metric definitions.

### What does Desk Occupancy Insights measure?

It is a count-based daily percentage comparing recorded desk Booking counts with recorded desk counts. Multiple Booking instances can make it exceed 100%. It is not booked-hours utilization, opening-hours utilization, or physical headcount occupancy.

### Are marketplace Customers included in Member Attendance Insights?

No. Member Attendance Insights measures activity against the Organization's members. Marketplace Customers are registered Skedular users but are not automatically Organization members, so their marketplace activity is not treated as member attendance. This does not exclude their Bookings from other analytics views.

### Can I export analytics?

Not currently. Operator analytics can be viewed in Skedular, but there is no analytics export workflow at this time. See [Operator analytics](/docs/spaces/analytics) for the available views.

## Next step

Continue to [Best Practices](/docs/spaces/best-practices) for practical guidance on operating Skedular Spaces.
