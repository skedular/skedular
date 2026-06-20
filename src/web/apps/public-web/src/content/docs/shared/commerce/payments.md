---
id: shared-payments
title: "Payments"
description: "Understand how Skedular tracks payment methods, payment status, confirmation, and financial activity for commercial access."
product: shared
category: commerce
slug: payments
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/payments.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-products
  - shared-bookings
  - shared-subscriptions
  - shared-billing-payouts
updatedAt: 2026-07-15
---

<div class="documentation-concept-support"><strong>Available in</strong><span>✅ Skedular Spaces</span></div>

## Overview

Payments describe the financial activity associated with commercial access. In Skedular Spaces, payment status and payment method are stored on marketplace Bookings and on the billing windows that support Subscriptions; Skedular does not create one universal standalone Payment record for every flow. The payment information is not the access itself: Bookings reserve Resources, and Subscriptions manage recurring access. Invoices and billing schedules are documented under [Billing and Payouts](/docs/shared/commerce/billing-and-payouts).

<aside class="documentation-callout" aria-label="Payment core rule"><strong>Core rule</strong><p>Payment status tracks financial activity; Bookings and Subscriptions represent the access that activity relates to.</p></aside>

## Payment and access

<div class="documentation-concept-grid"><div><strong>💳 Payment activity</strong><small>Payment method and status attached to a commercial Booking or Subscription billing period.</small></div><div><strong>📅 Booking</strong><small>Reserves one or more Resources for a specific period.</small></div><div><strong>🔁 Subscription</strong><small>Maintains recurring customer access and its Booking series.</small></div></div>

Payment status and access status are related but separate. A Booking or Subscription can continue to exist while its associated financial activity is being processed or requires further action. Payment status does not itself create or represent workspace access.

## Payment relationships

<div class="documentation-organization-context" aria-label="Payment relationships"><div class="documentation-context-root">👤 Customer<div class="documentation-context-child"><span>🛒 Chooses Product + Price</span><div class="documentation-context-child"><a href="/docs/shared/core-concepts/bookings">📅 Booking</a><a href="/docs/shared/marketplace/subscriptions">🔁 Subscription</a><div class="documentation-context-child"><span>💳 Payment activity</span><a href="/docs/shared/commerce/billing-and-payouts">🧾 Invoice and Billing</a></div></div></div></div><div class="documentation-context-tree"><span>💳 Card payment → Stripe</span><span>🏦 Bank transfer → manual confirmation</span><span>📚 Accounting → Xero</span></div></div>

The Customer chooses a Product and Price. That choice creates a Booking or Subscription. When the resulting purchase or billing period requires collection, Skedular starts tracking payment activity on that commercial record. Payment methods, payment providers, invoices, and accounting integrations are different parts of the Commerce model and are not interchangeable.

## When Payments are created

Payment tracking begins when a commercial Booking or Subscription purchase or billing period enters a payment flow. A Product Price supplies the accepted payment methods and billing mode used by that flow. Upfront pricing is tracked for the purchase period; in-arrears pricing is tracked as each configured billing period becomes due. The exact invoice and payment timing belongs to the selected billing configuration.

## Payment methods

Skedular Spaces supports these customer-facing payment methods where they are enabled by the Product Price:

- **Card payment**: online card processing through the Organization's connected Stripe payment setup.
- **Bank transfer**: a manual payment path using the Organization's configured bank details and confirmation process.

Stripe is the payment provider for card processing, not a customer-facing payment method. Xero is an accounting and invoice integration, not a customer payment method.

## Payment status

The Spaces interface exposes these payment states:

<div class="documentation-concept-grid"><div><strong>Pending</strong><small>Payment is awaiting completion or confirmation.</small></div><div><strong>Confirmed</strong><small>Skedular has received confirmation that the required payment was completed or accepted.</small></div><div><strong>Rejected</strong><small>The payment attempt or submitted confirmation was not accepted and may need follow-up.</small></div><div><strong>Expired</strong><small>The allowed payment window ended without confirmation.</small></div><div><strong>No payment required</strong><small>No payment collection is required for the Booking or Subscription activity, so no payment confirmation is expected.</small></div></div>

These states describe financial processing and do not replace the status of the Booking or Subscription.

## Card Payments

Card payment is processed automatically through the Organization's connected Stripe payment setup. Skedular receives the provider result and updates the associated payment status without manual operator confirmation. Stripe is the provider; Skedular tracks the resulting Commerce status.

## Bank Transfers

Bank transfer is a manual payment path. The Customer uses the Organization's configured bank details and the operator confirms or rejects the payment in Skedular. While confirmation is pending, the associated Booking or Subscription remains a separate record with its own access state. The transfer itself is made to the Organization rather than processed by Skedular.

## Payments and Invoices

An Invoice records an amount due when the configured billing flow requires one. Payment activity tracks the status used to satisfy or reconcile that obligation. They are related Commerce records, not interchangeable records, and they do not have to be created as a one-to-one pair.

## Payment confirmation and access

Card payments receive confirmation from the connected payment provider. Bank transfers follow the configured manual confirmation path. A payment can remain pending or rejected while the related Booking or Subscription record remains available for the operator to review. Cancellation, expiry, and refund decisions follow the Booking, Subscription, Product Price, and Commerce rules that apply to the transaction.

## Ownership and scope

Payment activity is scoped to the Organization's commercial activity. The Customer is the person associated with the transaction, the Product and Price define what was purchased, the Booking or Subscription represents the access, and the Invoice or billing record represents the financial obligation when the configured billing flow requires one. Operators can only review or change payment details when their organization role grants the relevant permissions.

## Managing Payments in Skedular Spaces

Operators can review payment method, payment status, invoice references, and related Booking or Subscription activity through the workflows they can access. Where the workflow permits, operators can confirm or reject bank-transfer payments and open related Booking, Subscription, or invoice information. Use [Billing and Payouts](/docs/shared/commerce/billing-and-payouts) for billing schedules, invoices, payouts, and accounting follow-up.

## Example

A Customer books a meeting room through a Product. With card payment, Stripe processes the payment and Skedular updates the associated payment status automatically. With bank transfer, the Customer pays using the Organization's bank details and the operator confirms or rejects the payment in Skedular. In both cases, the Booking remains the access record and its payment status describes the financial state.

## Things to Know

- Payment status tracks financial activity; it is not a Booking or Subscription.
- Product Prices determine accepted payment methods and billing mode.
- Card payment is processed through Stripe; Stripe is not itself a payment method.
- Bank transfer is a manual payment method using Organization bank details.
- Xero is an accounting and invoice integration.
- Payment status and access status are separate.
- A Booking or Subscription can remain visible while payment activity needs further action.
- Invoices are created or tracked when the configured billing flow requires them.
- Recurring Subscription activity can carry payment status for each billing period.

## Best Practices

- Review pending bank-transfer payments that require confirmation.
- Check the related Booking or Subscription before investigating a payment status.
- Do not assume a rejected payment status automatically removes access.
- Use Billing and Payouts when investigating invoice timing or amounts due.
- Verify the configured payment method before troubleshooting a payment.
- Keep Stripe and Organization bank details up to date.

## Frequently Asked Questions

### What is a Payment in Skedular?

A payment status describes the financial activity associated with commercial access in Skedular Spaces. It is stored on the related marketplace Booking or Subscription billing window rather than as one universal standalone Payment record.

### Is a Payment the same as a Booking?

No. A Booking reserves Resources. Payment activity tracks the financial state associated with that access.

### Is a Payment the same as a Subscription?

No. A Subscription manages recurring access and its Booking series. Its billing windows can have separate payment status.

### Which payment methods are supported?

Product Prices can accept card payment or bank transfer where configured.

### How do card payments work?

Stripe processes the card payment through the Organization's connected payment setup, and Skedular updates the associated payment status automatically.

### How do bank transfers work?

The Customer pays using the Organization's configured bank details. The operator confirms or rejects the payment in Skedular, while the transfer itself occurs outside Skedular.

### What does Pending mean?

Payment is awaiting completion or confirmation. The related Booking or Subscription remains a separate record with its own access state.

### What happens when a payment is rejected or expires?

The payment attempt or confirmation was not accepted, or its payment window ended. Follow the applicable Booking, Subscription, billing, and cancellation rules; payment status does not by itself describe the full access outcome.

### Does every Payment have an Invoice?

No. An Invoice is created or tracked when the configured billing flow requires one; payment activity and Invoices are related but are not one-to-one in every flow.

### Where are invoices and payouts documented?

See [Billing and Payouts](/docs/shared/commerce/billing-and-payouts) for invoice timing, billing schedules, payouts, and accounting workflows.

## Related Documentation

- [Commerce](/docs/shared/commerce)
- [Products](/docs/shared/marketplace/products)
- [Bookings](/docs/shared/core-concepts/bookings)
- [Subscriptions](/docs/shared/marketplace/subscriptions)
- [Billing and Payouts](/docs/shared/commerce/billing-and-payouts)
