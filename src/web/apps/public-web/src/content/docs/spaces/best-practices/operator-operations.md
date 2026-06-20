---
id: spaces-operations
title: "Best Practices"
description: "Practical guidance for setting up, launching, and operating your workspace effectively with Skedular Spaces."
product: spaces
category: best-practices
slug: operator-operations
articleKind: best-practice
publicationState: published
evidenceRefs:
  - src/web/apps/webapp-spaces/src/components/organization/organizationAnalytics/organization-analytics.tsx
  - doc-resources/location.md
  - doc-resources/resource.md
  - doc-resources/product.md
  - doc-resources/subscriptions.md
  - doc-resources/billing-and-payouts.md
  - doc-resources/analytics.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-17
---

This guide brings together practical recommendations from across Skedular Spaces. Use it to prepare a clear customer experience before launch and to keep the workspace, commercial offering, and operations aligned over time.

## Start with a simple workspace structure

We recommend building around the relationship `Organization → Locations → Resources`. Create Locations around real physical places and Resources around the things Customers can actually book. Use clear names, the correct Resource type, accurate addresses, opening hours, and the correct Location timezone. Configure Resource-specific opening-hour overrides carefully, because these settings and existing Bookings affect when an eligible Resource can be booked.

See [Locations and resources](/docs/spaces/workspace-setup/locations-and-resources) for the setup workflow.

## Use Zones and Floor Plans intentionally

Zones are optional Organization-level groups for organizing Resources operationally or spatially. A Resource can belong to no Zone, one Zone, or multiple Zones. Keep Zone names simple and meaningful; Zones do not determine Product eligibility.

Floor Plans are also optional. When the physical position of Resources matters, use a clear image, place Resources accurately, and update the layout when the workspace changes. Avoid adding visual detail that does not help operators or Customers.

See [Zones and floor plans](/docs/spaces/workspace-setup/zones-and-floor-plans) and [Products and pricing](/docs/spaces/products-and-marketplace/products-and-pricing) for the separate organization and Product workflows.

## Design Product Tags and Products deliberately

Product Tags connect Products with eligible Resources; Zones solve a different organizational problem. A Resource matching at least one Product Tag selected on a Product can enter that Product's eligible pool. Selecting several Product Tags therefore broadens the pool, so avoid unrelated tags and review eligibility before activating a Product.

Keep Products understandable to Customers. Use clear titles, useful descriptions, included features, relevant images, meaningful Prices, and an intentional cancellation policy. Offer only Prices that represent real customer choices, avoid near-duplicates, and review payment methods, billing mode, and auto-renewal before activation. Product activation controls whether a Product can be purchased; marketplace visibility is a separate Organization setting.

Continue to [Products and pricing](/docs/spaces/products-and-marketplace/products-and-pricing) for the detailed Product workflow.

## Choose billing and payment settings intentionally

Consider whether **Upfront** or **In arrears** billing fits the way you want to charge Customers. The first Subscription invoice is generated when the Subscription starts in either mode. Subsequent Upfront invoices follow the Subscription purchase period, while subsequent In-arrears invoices follow the Organization billing cycle. The invoice due date is separate from billing timing.

Before activating Products, verify that Stripe is connected when card payments are offered, bank details are correct when bank transfer is offered, and each Price exposes the intended payment methods. Xero supports accounting and invoicing; it is not a Customer payment method. Review [Billing and payments](/docs/spaces/billing-and-payments), [Payment methods](/docs/spaces/billing-and-payments/payment-methods), and [Xero accounting](/docs/spaces/billing-and-payments/xero-accounting).

If you use Xero, periodically review the connected Organization, accounting mappings, sales account, and any tracking or branding theme settings you use. Check that invoice synchronization is behaving as expected, and return to [Xero accounting](/docs/spaces/billing-and-payments/xero-accounting) for the detailed workflow.

## Test the complete Customer journey

Before launch, test the intended public or private marketplace path from a Customer's perspective: sign in, select a Product and Price, confirm Resource eligibility and Availability, create a Booking, complete the payment flow, and check the resulting invoice and Booking. Where applicable, test recurring purchase and Subscription creation, cancellation timing, and the refund workflow. Test the whole journey rather than relying on individual settings in isolation.

## Keep Availability and operations accurate

Product eligibility does not guarantee that a Customer can book. Product Tags determine which Resources may satisfy a Product; Availability determines whether an eligible Resource can be booked at the requested time. Regularly review Location opening hours, Resource overrides, existing Bookings, and the Resources selected by each Product.

A Booking represents scheduled Resource use. A Subscription represents an ongoing commercial arrangement with associated Booking instances. Review Subscription status separately from individual Bookings, and keep cancellation, stopping future billing, and refunding an existing payment as separate decisions. See [Bookings](/docs/spaces/bookings), [Subscriptions](/docs/spaces/bookings/subscriptions), and [Refunds](/docs/spaces/billing-and-payments/refunds).

For bank transfers, review pending payments regularly. Confirm a payment only after verifying receipt, reject or follow up on unresolved payments as appropriate, and keep the configured bank details current.

## Use Analytics as a signal

Use [Operator analytics](/docs/spaces/analytics) to look for patterns in Booking activity, Desk Occupancy Insights, Resource Availability Insights, and Location activity. Member Attendance Insights concerns Organization members, not marketplace Customers. Desk Occupancy Insights is a count-based daily percentage and can exceed 100%; it is not booked-hours utilization or physical headcount occupancy. Treat unusual results as prompts to investigate workspace configuration and Availability, not as automatic decisions.

## Review your setup as the workspace changes

Review your configuration whenever the real operation changes. Review the related configuration when adding or removing Resources, changing Location or Resource-specific opening hours, updating Floor Plans or Zones, changing Product Tags or Product eligibility, introducing or changing Products or Prices, or changing payment, billing, or accounting configuration. Keeping these details current helps avoid incorrect Availability, unintended Product eligibility, confusing Customer information, and inconsistent commercial workflows.

## Before you go live

- Organization profile is complete.
- Locations have accurate addresses and timezones.
- Opening hours and Resource overrides are correct.
- Resources have the intended names and types.
- Product Tags connect Products to the intended Resources.
- Zones and Floor Plans are accurate where used.
- Products have complete customer-facing information.
- Prices are correct.
- Cancellation policies are intentional.
- Intended payment methods are configured.
- Stripe is connected when card payments are offered.
- Bank details are correct when bank transfer is offered.
- Tax and invoice information is correct.
- Xero is configured when the integration is used.
- Products intended for sale are activated.
- Marketplace visibility is configured correctly.
- The complete Customer purchase journey has been tested.

## Ongoing operating checklist

- Review pending bank-transfer payments.
- Review unresolved billing activity.
- Review active Subscriptions and relevant future Bookings.
- Keep Location and Resource configuration current.
- Review opening hours when operating schedules change.
- Review Product eligibility when Resources or Product Tags change.
- Review cancellation and refund requests consistently.
- Verify Xero integration health when Xero is used.
- Review Analytics for meaningful operational patterns.

## Need more detail?

These recommendations bring together the completed Spaces guides. Return to the [Skedular Spaces documentation hub](/docs/spaces) for detailed setup and workflow instructions.
