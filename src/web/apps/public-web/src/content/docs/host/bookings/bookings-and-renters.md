---
id: host-bookings
title: "Bookings and renters"
description: "View bookings for your place, understand who booked, and manage the actions available for each booking."
product: host
category: bookings-and-renters
slug: bookings-and-renters
articleKind: guide
publicationState: published
evidenceRefs:
  - doc-resources/booking.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-24
---

When a renter books your place, the Booking appears in the Host bookings area. Use it to review the time reserved, see who booked, check payment status when payment is required, and manage the actions available for that Booking.

## View your bookings

The bookings view shows the selected week. Use the week picker to move through dates, the **User** selector to filter by renter, and the **Location** selector to narrow the list to a place. Each Booking card shows the place, date and time range, renter name, and a **Recurring** indicator when it belongs to a recurring series. A card can also show the payment status and a **View Invoice** link when an invoice is available.

## View Booking details

Open a card with **View details** to review the Booking. The details include the date and time, the place reserved, the renter's name, notes when they were provided, payment status, and invoice links when available. The Booking represents the entire place being reserved, rather than individual desks, rooms, or other bookable resources.

## Understand the renter information

The Booking card and details view show the renter's name. Use that name to identify who is arriving and which Booking it belongs to. No email address, phone number, or in-product messaging control is shown in this Booking workflow.

## Understand payment status

When a Booking requires payment, its payment status is shown on the card and in the details view. The status identifies the current payment state, such as Pending, Confirmed, or Rejected. Detailed payment setup, provider handling, and refund processing belong in [Payments and refunds](/docs/host/payments-and-refunds).

## Manage a Booking

From a Booking card, you can:

- Open **View details**.
- Depending on the Booking type and state, use the available **Cancel booking** or **Remove booking** action.
- Review or manage a recurring series when the Booking belongs to one.
- Use the available payment actions when payment requires operator attention: **Confirm Payment**, **Reject Payment**, or **Make Payment Not Required**.

Cancellation changes the Booking. It does not by itself decide or complete a refund. The cancellation policy that applies to the pricing option determines refund eligibility, while the operational refund workflow is documented in [Payments and refunds](/docs/host/payments-and-refunds). Pricing, opening hours, duration limits, and cancellation-policy configuration are covered in [Pricing and availability](/docs/host/pricing-and-availability).

## Review a new Booking

When a new Booking appears, check the renter name, date and time, duration, payment status when shown, and any notes. Confirm that the details match what you expect for the place before the renter arrives.

## Availability when a renter books

Host checks availability again while creating a renter's Booking. If another Booking takes the final compatible capacity after the renter has selected a time, Host does not create a partial Booking. The renter receives an availability outcome and can choose another time or pricing option.

## Next step

Use **Marketplace purchases** to review retained one-time Bookings and Subscriptions together. The page keeps canceled and deleted purchase evidence, shows payment and renewal state, and supports list/grid views with pagination.

After you understand how to review and manage Bookings, continue to [Payments and refunds](/docs/host/payments-and-refunds) to learn how payment and refund workflows related to those Bookings are handled.
