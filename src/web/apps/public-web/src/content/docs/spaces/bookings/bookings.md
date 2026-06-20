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
updatedAt: 2026-07-24
---

## Understand Bookings in Spaces

A Booking reserves one or more Resources for a defined period. In the Spaces marketplace, the customer chooses a Product and Price; the resulting Booking reserves eligible workspace Resources for the requested time.

Products define the commercial offering. Product Tags guide which Resources are eligible, while Availability determines whether a matching Resource can be reserved.

### Price calendar-day rules

A Price can also be limited to selected calendar days. Sunday through Saturday are equal choices; this is not a workweek-only rule. An empty selection means the Price is available every day. The selected start date must satisfy the Price rule, then normal opening hours, Resource matching, and booking conflicts still decide whether the Booking can be created.

## How Bookings are created

Customer-facing Bookings start when a registered Skedular user purchases a Product through the public Spaces experience. Operators can also create Bookings from the organization management interface using **Add Booking**. The form's **User** selector lists Organization members, not marketplace Customers. It also selects the date and time, Location, and available Resources; the form can be opened with a Location, date, or Resource already selected.

Recurring customer arrangements are represented by Subscriptions and their associated recurring Booking instances. See [Subscriptions](/docs/spaces/bookings/subscriptions) for that workflow.

## View and manage Bookings

Operators find Bookings in the organization **Bookings** area. The list is shown for a selected date range and can be filtered by Locations, customers, and other available booking criteria. Each Booking card exposes the customer, time, Location, assigned Resources, and any marketplace or Subscription context available to the operator.

Open a Booking to edit its supported details. Depending on the Booking type and permissions, operators can edit a private Booking, update its customer or Resources, remove a Booking, remove a recurring series, or manage marketplace payment actions such as **Confirm Booking Payment**, **Reject Booking Payment**, and **Make Booking Payment Not Required**.

## Customers and Bookings

For customer-facing Bookings, the person who purchased or created the Booking is a registered Skedular user acting as the Organization's Customer. That Customer does not need to belong to the Organization. Operators can see the customer identity exposed by the Booking workflow, such as the person's name and available contact details. See [Customers](/docs/shared/marketplace/customers) for the full distinction between Customers and Organization members.

## Resource assignment and availability

Product Tags identify the Resource pool eligible for a Product. In marketplace Bookings, Spaces matches that pool and reserves the Resource or Resources required by the Price. In **Add Booking**, the operator selects from the **Available resources** for the selected Location and time. If no eligible Resource is available for a marketplace request, the Booking cannot be completed.

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
