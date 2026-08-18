---
id: host-credit-entitlements
title: "Credit-based booking entitlements"
description: "Offer prepaid credits for eligible bookings and control unused-credit refunds."
product: host
category: bookings
slug: credit-entitlements
articleKind: guide
publicationState: published
evidenceRefs:
  - doc-resources/credit-entitlements.md
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - host-bookings
  - host-pricing
updatedAt: 2026-08-11
---

# Credit-based booking entitlements

Hosts can offer a pricing option that grants prepaid booking credits. Buying the option creates an entitlement, not a Booking or Resource allocation. Customers return later to use the credits for eligible resource bookings during the configured validity period.

## Product-level policy

Configure the pricing option as **Entitlement**, define the credit quantity, set its validity period, and choose the allowed calendar days for spending the credits. Each credit pays for one eligible booking. Leave **Available days** empty to allow every day, or select any combination of Sunday through Saturday. A credit-funded booking or date modification must use an allowed day before the credit can be consumed. Normal opening-hours, resource-availability, and conflict checks still apply.

The pricing option also explicitly determines whether unused credits may be refunded. This policy is stored with the purchase, so future catalog changes do not silently change existing customer rights. Credit quantity is the number of bookings included; no separate credit unit is required.

Customers can create, change, or cancel credit-funded Bookings after purchase. Authorized Host owners and administrators can perform the same actions on a customer's behalf. Date, time, available-day, resource, and normal availability rules still apply.

## Payment and credit grant

Stripe card payment follows the automatic checkout path. Bank-transfer payment remains pending until an authorized Host owner or administrator confirms it. Credits are granted only after payment is confirmed, so an unconfirmed purchase does not create usable credit balance or booking state.

## Payment and settlement

When payment is confirmed, unused credits can be prorated at expiry or eligible cancellation if the product pricing allows refunds. Stripe can complete supported refunds automatically. Bank-transfer refunds are manual. Xero records the accounting adjustment by creating a credit note, but does not currently settle the customer payment automatically.

If payment is not confirmed, the entitlement expires or is forfeited without creating a refund. Customers can review their balance, expiry date, credit ledger, and refund or settlement status.

If auto-renew is enabled, the entitlement is renewed at the end of its cycle using the current compatible pricing and the same payment process. A renewal does not grant credits until payment is confirmed. If renewal payment fails or no compatible pricing exists, the current cycle ends without granting a new cycle.

Reservation-based and recurring booking behavior remains unchanged. Credit entitlements are an additional fulfillment path: the purchase grants future usage, and the renter or an authorized Host operator creates the Booking later.
