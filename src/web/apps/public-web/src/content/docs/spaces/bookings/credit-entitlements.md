---
id: spaces-credit-entitlements
title: "Credit-based booking entitlements"
description: "Configure and use prepaid booking credits with expiry and refund rules."
product: spaces
category: bookings
slug: credit-entitlements
articleKind: guide
publicationState: published
evidenceRefs:
  - doc-resources/credit-entitlements.md
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - spaces-bookings
  - spaces-products-pricing
updatedAt: 2026-08-11
---

# Credit-based booking entitlements

Spaces administrators can configure a pricing option as an entitlement. An entitlement grants a fixed number of booking credits for a defined validity period. Purchasing it creates no Booking, Resource allocation, or quota usage; each credit can later pay for one eligible resource booking.

## Configure a pricing option

In the product pricing editor, choose **Entitlement**, then set:

- the number of included bookings/credits;
- the number of validity days from purchase or activation; and
- the allowed calendar days on which customers may spend the credits; and
- whether unused credits are refundable when the entitlement ends.

The refund choice belongs to the pricing option. Changing a product later does not rewrite the policy captured by an existing purchase.

Leave **Available days** empty to allow credit spending on every calendar day. Otherwise select any combination of Sunday through Saturday. The booking date must be allowed before a credit is consumed; the same rule applies when a credit-funded booking is modified. Opening hours, resource availability, and booking conflicts still apply.

Customers can create, modify, and cancel credit-funded Bookings after purchase. Authorized Spaces owners and administrators can create or manage those Bookings on behalf of a customer, including changing the date, time, or resource.

## Payment and credit grant

Stripe card payment follows the automatic checkout path. Bank-transfer payment remains pending until an authorized operator confirms it. Credits are granted only after payment is confirmed, so an unconfirmed purchase does not create usable credit balance or booking state.

## Expiry and refunds

Unused credits expire at the entitlement end date. If the pricing option allows refunds and payment was confirmed, Skedular calculates a prorated refund from the unused credits and creates the refund in the normal refund workflow. Used credits are never refundable as unused entitlement credits.

Stripe refunds can be submitted automatically when the payment path supports it. Bank-transfer refunds require manual settlement. Xero currently creates a credit note for the accounting adjustment; payment settlement remains manual until the Xero integration supports automatic settlement.

## Customer visibility

Customers can see their remaining balance, expiry date, credit history, and any refund or settlement status from their booking and entitlement views.

When auto-renew is enabled, the entitlement renews at the cycle boundary through the configured Stripe or bank-transfer payment path. The next cycle is granted only after payment confirmation. A failed payment or missing compatible pricing ends the current cycle without granting new credits.

Reservation-based and recurring booking behavior remains unchanged. Credit entitlements are an additional fulfillment path: the purchase grants future usage, and the customer or an authorized operator creates the Booking later.
