---
id: host-pricing
title: "Pricing and availability"
description: "Set how much your place costs, when renters can book it, and the conditions that apply to their booking."
product: host
category: pricing-and-availability
slug: pricing-and-availability
articleKind: guide
publicationState: published
evidenceRefs:
  - doc-resources/location.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-17
---

Pricing and availability work together to shape what renters can book. Pricing determines what a renter pays, availability determines when the place can be booked, booking rules set duration limits, and a cancellation policy explains the financial outcome if a renter cancels.

## Set your pricing

Host lets you create one or more pricing options for the place. For each option, set a renter-facing label, a **Purchase term**, and a **Price**. The available terms are **Daily**, **Weekly**, **Fortnightly**, **Monthly**, **Two months**, **Quarterly**, **Four months**, **Five months**, **Six months**, and **Yearly**.

You can also choose the place's **Currency** from the currencies shown in the Host editor and set whether the price is **Tax inclusive** or **Tax exclusive**. Keep each option easy for renters to compare. Payment connection and payment workflow guidance belongs in [Payments and refunds](/docs/host/payments-and-refunds).

### Limit a Price to calendar days

Use **Available days** on a Price when that Price should be purchasable only on particular calendar days. You can choose any combination of **Sunday** through **Saturday**; all seven days have the same meaning, so this is not a workweek-only setting. Leave **Available days** empty for **Every day**, which preserves the normal behavior for that Price.

Available days apply to the renter's booking start date. They are an additional Price rule: opening hours, existing Bookings, and matching Resource availability can still prevent a booking on an allowed day. Renters see the applicable days while choosing a Price and cannot select disallowed dates.

## Set when your place can be booked

Configure the place's opening hours to define when it can normally be booked. Each day can be closed, open all day, or use a specific opening period. The place's **Timezone** determines how those opening hours and booking times are interpreted.

Opening hours describe when the place is normally available. An existing Booking can still make a particular time unavailable to a renter, so review the calendar after changing the schedule.

## Set booking rules

Each pricing option has **Booking Rules** for **Minimum duration (minutes)** and **Maximum duration (minutes)**. Use these limits to control how long a renter can book under that option. Leave a limit empty when you do not need that boundary.

## Choose a cancellation policy

Cancellation is configured for each pricing option. Host provides three choices:

- **No Refunds**: Cancelling under this policy does not make the renter eligible for a refund.
- **Full Refund Before Cutoff**: The renter is eligible for a full refund when they cancel at least the configured number of minutes before the Booking starts.
- **Tiered Refunds**: The refundable percentage depends on how long before the Booking starts the renter cancels.

For **Full Refund Before Cutoff**, enter the **Minutes before booking** value. For **Tiered Refunds**, add rules using **Minutes before** and **Refund %**. Renters see the applicable cancellation terms before checkout. The cancellation policy determines refund eligibility; handling an approved refund is a separate workflow covered in [Payments and refunds](/docs/host/payments-and-refunds).

## Review the renter experience

Before sharing the listing, review the renter-facing flow. Confirm that the available pricing options are understandable, the place can be booked during the intended hours, the duration limits are correct, and the applicable cancellation terms are visible.

## Next step

Once pricing, availability, booking rules, and cancellation terms are configured, continue to [Bookings and renters](/docs/host/bookings-and-renters) to learn how to view and manage the bookings renters make.

## Credit entitlements

Hosts can offer prepaid booking credits with a product-level validity period and explicit unused-credit refund choice. The policy is captured when the customer purchases the pricing option.
