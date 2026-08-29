---
id: spaces-bookings
title: "Bookings in Spaces"
description: "Create and manage workspace reservations made through Skedular Spaces Products."
product: spaces
category: bookings
slug: bookings
articleKind: guide
publicationState: published
evidenceRefs:
  - doc-resources/booking.md
  - doc-resources/resource.md
  - doc-resources/subscriptions.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - spaces-products-pricing
  - spaces-subscriptions
updatedAt: 2026-08-07
---

## Understand Bookings in Spaces

A Booking reserves one or more Resources for a defined period. In the Spaces marketplace, the customer chooses a Product and Price; the resulting Booking reserves eligible workspace Resources for the requested time.

Products define the commercial offering. Booking Groups guide which Resources are eligible, while Availability determines whether a matching Resource can be reserved.

### Price calendar-day rules

A Price can also be limited to selected calendar days. Sunday through Saturday are equal choices; this is not a workweek-only rule. An empty selection means the Price is available every day. The selected start date must satisfy the Price rule, then normal opening hours, Resource matching, and booking conflicts still decide whether the Booking can be created.

## How Bookings are created

Customer-facing Bookings start when a registered Skedular user purchases a Product through the public Spaces experience. Operators can also create Bookings from the organization management interface using **Add Booking**. The form's **User** selector lists Organization members, not marketplace Customers. It also selects the date and time, Location, and available Resources; the form can be opened with a Location, date, or Resource already selected.

Recurring customer arrangements are represented by Subscriptions and their associated recurring Booking instances. See [Subscriptions](/docs/spaces/bookings/subscriptions) for that workflow.

## View and manage Bookings

Operators find Bookings in the organization **Bookings** area. The list is shown for a selected date range and can be filtered by Locations, customers, and other available booking criteria. Each Booking card exposes the customer, time, Location, assigned Resources, and any marketplace or Subscription context available to the operator.

Open a Booking to edit its supported details. Depending on the Booking type and permissions, operators can edit a private Booking, update its customer or Resources, remove a Booking, remove a recurring series, or manage marketplace payment actions such as **Confirm Booking Payment**, **Reject Booking Payment**, and **Make Booking Payment Not Required**.

## Marketplace purchase history

The organization's **Marketplace purchases** page is the unified history for marketplace activity. It lists standalone Bookings alongside Subscriptions and credit entitlements and shows the customer, Product, booking window, amount, payment state, and lifecycle state where available. Canceled and deleted purchases remain visible when they have historical activity, allowing authorized users to review what happened without treating a removed purchase as erased history.

Use the purchase history for the commercial overview and the Booking details for individual Resource reservations and scheduling changes. Subscription and credit-entitlement detail pages render the backend-provided lifecycle history; the frontend does not reconstruct events from aggregate dates. Standalone one-time Booking details remain unchanged and do not receive a history tab. Customers can access their own purchases, while organization operators require the relevant authorization.

## Modify a marketplace Booking

Customers can open their own marketplace Booking details to change an eligible future Booking. Authorized organization owners and administrators can open the Booking details or Marketplace purchases history to make the change for the customer. The Booking must be **Confirmed** or **Payment not required**, and the change must be completed before the Booking starts. This change is separate from cancellation, so the Product cancellation cutoff does not control it.

Choose the new date and time, then select any available Resources that the originally purchased Product can fulfill. The selection can use a different eligible Resource type or individual Resource, but cannot exceed the purchased Resource quantity or switch the Product or Price. If no replacement Resources are selected, Spaces may retain the current Resources or use its normal automatic allocation. Spaces checks the full date, time, and Resource set together. If capacity or eligibility has changed, it leaves the original Booking unchanged and shows the reason.

The completed change keeps the purchase, payment, invoice, refund, and cancellation terms unchanged. Customers can review the resulting Booking and change history. An operator must enter a reason; Spaces records it in the audit history and notifies the customer. If notification delivery needs follow-up, the operator can see that recovery state with the change record.

## Customers and Bookings

For customer-facing Bookings, the person who purchased or created the Booking is a registered Skedular user acting as the Organization's Customer. That Customer does not need to belong to the Organization. Operators can see the customer identity exposed by the Booking workflow, such as the person's name and available contact details. See [Customers](/docs/shared/marketplace/customers) for the full distinction between Customers and Organization members.

## Resource assignment and availability

Booking Groups identify the Resource pool eligible for a Product. In marketplace Bookings, Spaces matches that pool and reserves the Resource or Resources required by the Price. In **Add Booking**, the operator selects from the **Available resources** for the selected Location and time. If no eligible Resource is available for a marketplace request, the Booking cannot be completed.

Availability is checked again when Spaces creates the Booking. If another customer takes the final available capacity after the customer has selected a time, Spaces does not create a partial Booking. The customer sees an availability outcome and can choose another time or Product. Operators can review the retained outcome in the relevant customer-facing Booking history when it is available.

Operator-created private Bookings use the **Available resources** results for the selected Location and time. Existing Bookings, opening-hour rules, and other availability rules determine which Resources are offered. Bookings use 15-minute intervals, and one Booking stays within a single calendar day. When a Product purchase creates a Subscription, its scheduled workspace usage is represented by associated recurring Booking instances.

## Booking status and payment

Booking and payment state are separate. Marketplace Bookings expose payment statuses such as **Pending**, **Confirmed**, or **Rejected** where payment is required; these describe payment state, not the Booking itself. Card checkout can complete payment through the customer-facing checkout, while other payment paths may leave payment pending until an operator acts. Operators can see the payment state on the Booking and use the available payment actions there.

Where the Booking requires operator payment handling, the available actions include **Confirm Booking Payment**, **Reject Booking Payment**, and **Make Booking Payment Not Required**.

If a required payment expires or fails before confirmation, Spaces releases the affected Resource capacity and retains the payment outcome. The customer can start a new Booking request, which checks current availability again.

Payment-provider setup, invoices, refunds, payouts, and accounting configuration belong to [Billing and payments](/docs/spaces/billing-and-payments), not this Booking guide.

## Cancel a Booking

Customers can cancel eligible future marketplace Bookings from the customer-facing Booking details. Operators can remove Bookings from the organization Booking workflow, subject to the Booking type and permissions. The applicable Product cancellation policy determines whether a confirmed payment is refundable; cancellation and refund are separate outcomes. See [Refunds](/docs/spaces/billing-and-payments/refunds) for refund handling.

When a Booking is cancelled, the reserved Resource is released for the affected booking period. A cancellation does not automatically mean that a refund is created, especially when payment was never confirmed.

## Bookings created by Subscriptions

A Subscription is the longer-running commercial arrangement. Its recurring Booking instances represent the scheduled workspace usage for each cycle. Auto-renewal controls whether the Subscription continues into another cycle; it is not a separate way of creating the Subscription itself. Continue to [Subscriptions](/docs/spaces/bookings/subscriptions) for subscription management.

## Next step

Continue to [Subscriptions](/docs/spaces/bookings/subscriptions) to manage longer-running arrangements and the recurring Booking activity they generate.

## Booking with credits

Customers can use an active entitlement for eligible bookings. The customer view shows the remaining balance and expiry date; cancellation restores or forfeits the consumed credit according to the product policy.
