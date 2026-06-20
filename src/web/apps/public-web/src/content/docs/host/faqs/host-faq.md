---
id: host-faq
title: "FAQs"
description: "Answers to common questions about setting up, renting out, and managing your place with Skedular Host."
product: host
category: faqs
slug: host-faq
articleKind: faq
publicationState: published
evidenceRefs:
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-17
---

## Your place

### Do renters book my entire place?

Yes. A Host Booking represents the entire place being reserved, rather than individual desks, rooms, or other bookable resources. See [Bookings and renters](/docs/host/bookings-and-renters) for the details view and available Booking actions.

### When does my place become available online?

The place becomes available online when it is created. No additional step is required before renters can view it.

### Can I change my place details after creating it?

Yes. Update the place details, address and map location, Space Type, images, Amenities, and other renter-facing information through [Your place](/docs/host/your-place). For ongoing maintenance guidance, see [Managing your listing](/docs/host/managing-your-listing).

## Pricing and availability

### What happens when I change my price?

Price changes apply to future Bookings. They do not rewrite the amount already agreed for an existing Booking. Review [Pricing and availability](/docs/host/pricing-and-availability) for configuration details.

### Do availability changes affect existing Bookings?

Availability changes affect when new Bookings can be made; they do not automatically cancel or rewrite existing Bookings. Review [Managing your listing](/docs/host/managing-your-listing) before making a major schedule change.

### How do I temporarily stop accepting new Bookings?

Adjust the place's **Opening Hours** in [Pricing and availability](/docs/host/pricing-and-availability) so new Booking times are not offered while the place is unavailable. If you change the regular schedule temporarily, restore the normal **Opening Hours** when the closure ends.

## Bookings

### What information can I see about a renter?

Host shows the renter's name on the Booking card and in Booking details. The Host Booking workflow does not provide an in-product view of the renter's email, phone number, or messaging history.

### Can I change an existing Booking by editing my listing?

No. Changing pricing, availability, or listing details does not automatically rewrite an existing Booking. Review and manage the Booking separately in [Bookings and renters](/docs/host/bookings-and-renters).

### How do I cancel a Booking?

Depending on the Booking type and state, the available action is **Cancel booking** or **Remove booking**. See [Bookings and renters](/docs/host/bookings-and-renters) for the surrounding Booking actions.

### Can a Booking be part of a recurring series?

Yes. Host can show a **Recurring** indicator and provide series-related actions when a Booking belongs to a recurring series. The available actions depend on the Booking and its state.

## Payments and refunds

### Can my place be online before I connect Stripe?

Yes. A place can be online without a completed Stripe connection. Stripe readiness controls whether renters can complete paid checkout; it does not control whether the listing can be viewed. See [Payments and refunds](/docs/host/payments-and-refunds).

### Why can't renters complete a paid Booking?

Check the Host payment setup first. Paid checkout requires the connected Stripe account to be ready to accept charges, so incomplete onboarding or disabled charges can prevent checkout. See [Payments and refunds](/docs/host/payments-and-refunds).

### Do I need to confirm Stripe payments manually?

No. Successful Stripe payments are confirmed automatically. Use **Confirm Payment** only when a pending payment requires Host confirmation under the supported workflow.

### What does Make Payment Not Required do?

**Make Payment Not Required** removes the payment requirement so the Booking can proceed without a Stripe payment. It does not charge the renter, and it does not mean Skedular collects the money elsewhere.

### Does cancelling a Booking automatically mean the renter has been refunded?

No. Cancellation changes the Booking, the applicable cancellation policy determines refund eligibility, and the refund workflow processes the financial reversal. See [Payments and refunds](/docs/host/payments-and-refunds) for the full workflow.

### How is the refund amount calculated?

The Booking's cancellation policy determines the maximum refundable amount. You can reduce the amount before queuing the refund, but you cannot exceed the amount calculated by the policy. See [Payments and refunds](/docs/host/payments-and-refunds).

## Next step

For practical recommendations on operating your listing, continue to [Best Practices](/docs/host/best-practices).
