---
id: spaces-subscriptions
title: "Subscriptions"
description: "Manage recurring customer arrangements, scheduled Bookings, renewals, and billing in Skedular Spaces."
product: spaces
category: bookings
slug: subscriptions
articleKind: guide
publicationState: published
evidenceRefs:
  - doc-resources/subscriptions.md
  - doc-resources/booking.md
  - doc-resources/product.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - spaces-bookings
  - spaces-products-pricing
updatedAt: 2026-08-07
---

## Understand Subscriptions in Spaces

A Subscription is a longer-running commercial arrangement created when a customer purchases a Product Price with a subscription purchase cadence. Its scheduled workspace use is represented by associated Booking instances. A Booking is one reservation for a defined period; a Subscription groups the commercial arrangement and its recurring reservations.

The customer is a registered Skedular user who purchased the Product. Purchasing does not make the Customer an Organization member. See [Customers](/docs/shared/marketplace/customers) for the full distinction.

## When a purchase creates a Subscription

Spaces supports these purchase terms: **Daily**, **Weekly**, **Fortnightly**, **Monthly**, **Two months**, **Quarterly**, **Four months**, **Five months**, **Six months**, and **Yearly**. Auto-renewal is separate: disabled means one term, while enabled repeats the selected term. Booking duration comes only from the customer's selected start/end times and the offer's minimum/maximum limits.

The **purchase cadence** sets the length of the Subscription cycle. Recurring Booking instances are materialized for workspace use within that cycle, while the organization's billing mode and billing cycle determine when invoices are issued. These are related but separate settings. See [Products and pricing](/docs/spaces/products-and-marketplace/products-and-pricing) for configuring Prices.

## View and manage Subscriptions

Operators find Subscriptions in the organization **Subscriptions** area. The list can be filtered by Subscription status and payment status, and is ordered by next renewal. Each row or card can show the Product, Customer, start date, next renewal, renewal setting, quantity, payment method, payment status, and current lifecycle status.

Open a Subscription to review its recurring Booking instances, customer, Product, payment context, renewal details, and cancellation or refund information. Operators can manage eligible payment actions, cancel a Subscription immediately or at the end of the current period where available, and review the associated recurring Bookings.

The organization's **Marketplace purchases** page provides the unified history view for these Subscriptions and standalone marketplace Bookings. It retains canceled and deleted purchases with historical activity, so operators can review lifecycle, payment, and refund outcomes after the active access or Booking has ended.

## Subscription lifecycle and payment state

Subscription status and payment status are separate. Operator filters include **Active**, **Cancelled**, **Expired**, **Renewal failed**, and **Paused** where those states are present. The interface can also show **Ends at period end** when cancellation is scheduled.

Payment statuses belong to the recurring Booking or marketplace payment, not to the Subscription lifecycle itself. Operators may see **Pending**, **Confirmed**, or **Rejected** payment states and can use the available payment actions for a recurring Booking. See [Billing and payments](/docs/spaces/billing-and-payments) for payment and invoice workflows.

## How recurring Bookings are scheduled

When a Subscription starts, Spaces creates and maintains Booking instances for the current cycle. Instances are added progressively as the cycle is maintained, so operators see current and upcoming instances in the Bookings area and from Subscription details. Each instance keeps its own dates, times, Resources, and payment state while remaining associated with the Subscription.

If the selected Price has **Available days**, only those calendar days are materialized for the current Subscription period. Sunday through Saturday are equal choices, and an empty selection means every day. A Resource still has to be open and available on an allowed day. The current period keeps the Price rule it was purchased with; a renewed period uses the current matching Price rule.

For a weekly Price that requires Customer day selection, the Customer’s selected days are the fixed schedule for that Subscription. Skedular considers only those selected UTC calendar days; it does not substitute another available weekday when a selected day has no compatible Resource. The same selected pattern is retained on auto-renewal.

Booking recurrence and billing cadence are separate. The Subscription purchase cadence determines the cycle, while the recurring flow materializes day-level Booking instances for day-or-longer plans. Skedular generates the first invoice when the Subscription starts for both **Upfront** and **In arrears** billing. After that, Upfront invoices follow the configured upfront billing behavior, while In arrears invoices follow the Organization's billing cycle. The invoice due date is a separate setting that determines when each generated invoice must be paid.

## Resource assignment and availability

Subscription Booking instances use the Product's eligible Resource pool and requested Resource quantity. Resources are assigned to individual Booking instances rather than permanently reserved for the entire Subscription.

The first series is confirmed as a whole. If Spaces cannot allocate capacity for every required occurrence in that initial series, it does not present any occurrence as confirmed. The customer and authorized organization stakeholders receive the retained availability outcome and can review the Subscription before trying another arrangement.

After a Subscription is active, a later occurrence can fail independently if capacity is no longer available. Spaces retains that occurrence outcome and communicates it without cancelling the Subscription or unrelated occurrences. An operator edit still makes that individual Booking an override, so the recurring workflow does not rewrite the Subscription’s weekly pattern.

When payment has been captured and only part of a requested recurring series can be created, the target customer workflow shows the available and unavailable occurrences and gives the customer 24 hours to decide whether the partial series works. Acceptance keeps the created occurrences and refunds the unavailable portion. Rejection or no response cancels the created occurrences and refunds the full payment. This decision is recorded separately from the Subscription status and from the refund's provider status; see [Refunds](/docs/spaces/billing-and-payments/refunds) for the financial workflow.

If payment for a recurring cycle expires or fails before confirmation, Spaces releases the affected unpaid-cycle capacity and retains the payment outcome. The Subscription configuration remains available for its normal lifecycle and operator review.

## Modify one recurring Booking occurrence

Customers can change an eligible occurrence from their Booking details, and authorized organization owners and administrators can make the change from the occurrence or Subscription workflow. The occurrence must be **Confirmed** or **Payment not required**, must not have started, and can move only within its current Subscription cycle. An operator must provide a reason, which is recorded with the change; Spaces notifies the customer and retains a delivery-recovery record when follow-up is needed.

The actor chooses a new date and time and can select currently available Resources that the purchased Product can fulfill, up to the purchased Resource quantity. Spaces validates the complete proposed result against the original entitlement and live availability. If it cannot fulfill the proposal, it keeps the original occurrence unchanged and explains why.

This change creates an occurrence override. It does not change the parent Subscription, Product, Price, payment or billing cadence, renewal, selected weekly days, or other occurrences. Subscription maintenance preserves the override rather than rewriting it to the recurring pattern.

## Auto-renewal

Auto-renewal is separate from Subscription creation. The customer can choose it when the selected Price supports Subscription auto-renewal, and the Subscription stores that choice. When renewal runs, Spaces reloads the current Product version and looks for compatible pricing for the next cycle. If auto-renewal is off, the current period ends without another renewal; cancellation at period end keeps the current period active before stopping future renewal.

## Product and Price changes

Product and Price edits that create a new version apply to new purchases and do not rewrite the active cycle of an existing Subscription. At renewal, Spaces loads the current Product version and matches a compatible auto-renewable Price. If no matching Price remains, renewal fails rather than silently switching to an unrelated Price.

## End or cancel a Subscription

For an active Subscription, the interface supports **Cancel now** and **Cancel at period end** when those cancellation modes are available. Cancel now prevents new future Subscription billing activity, but does not automatically cancel generated invoices, remove outstanding balances, reverse completed payments, or create a refund. Cancel at period end keeps the current period active and then stops renewal. Disabling auto-renewal is a separate choice that lets the current period finish without starting another one. Cancellation stops future recurring Booking activity according to the selected mode, while past Booking instances remain historical records. Cancellation and refunds are separate decisions; see [Refunds](/docs/spaces/billing-and-payments/refunds) for financial outcomes.

## Next step

The **Marketplace purchases** page is the retained history view for both one-time marketplace Bookings and Subscriptions. It is sorted by newest activity and supports list/grid views and page navigation. A deleted or canceled purchase remains visible so operators can review its lifecycle and payment/refund outcome.

Continue to [Billing and payments](/docs/spaces/billing-and-payments) to configure and manage the payment, invoicing, and accounting workflows associated with commercial Bookings and Subscriptions.
