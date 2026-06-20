---
id: spaces-refunds
title: "Refunds"
description: "Understand when refunds apply and how to manage them across supported payment workflows."
product: spaces
category: bookings
slug: refunds
articleKind: guide
publicationState: published
evidenceRefs:
  - src/booking/shared/AGENTS.md
  - src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-refund-status-card.tsx
  - src/web/apps/webapp-spaces/src/components/marketplaceRefund/marketplace-refund-admin-panel.tsx
  - src/web/apps/webapp-spaces/src/components/marketplaceProduct/cancellation-policy-details.tsx
  - docs-resources/billing-and-payouts.md
  - docs-resources/xero-integration.md
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - spaces-bank-payments
  - spaces-xero
updatedAt: 2026-07-17
---

## How refunds work

In Skedular Spaces, cancellation and refund are separate decisions. Cancellation changes or ends a Booking or Subscription according to the applicable rules and selected cancellation timing. A refund reverses some or all of a confirmed payment when the applicable policy allows it.

For a Booking, or for an immediate Subscription cancellation, Spaces reviews the Price's cancellation policy after the cancellation is confirmed, checks the payment, and then creates a refund record when the purchase is eligible. Operators do not issue a refund independently of this cancellation workflow. Cancellation and refund remain separate actions: cancellation changes the service or reservation, while refund changes the financial outcome.

## Cancellation policies and eligibility

The cancellation policy is configured with the Product Price. Spaces supports these customer-facing policy shapes:

- **No Refunds:** the purchase cannot be cancelled for a refund after checkout.
- **Full Refund Before Cutoff:** a 100% refund is available until the configured cutoff; after it, no refund is available.
- **Tiered Refunds:** configured cutoffs determine the percentage available at each point before the Booking or renewal.

Refund preview and creation require a confirmed payment and a confirmed, policy-eligible cancellation. A refund cannot exceed the amount paid, less any amount already refunded.

## How the refund amount is determined

Spaces starts with the cancellation policy and the cancellation time, then considers the confirmed amount paid and any previous refund activity. The operator can enter a lower **Approved amount** than the policy amount, but it cannot exceed the current refundable amount.

## Review and process a refund

Eligible refunds appear with the related Booking or Subscription and on the organization's **Refunds** page. Operators can review the amount, currency, policy percentage, customer request, notes, and status history before choosing **Queue refund**. The approval dialog uses **Approved amount** and **Admin note**.

The normal eligible-refund path does not require a separate approval process. Operators queue the reviewed amount with **Queue refund**. Provider and payment-path actions are described below where they apply.

## Stripe refunds

For a Stripe-paid Booking or Subscription, Spaces submits the approved refund through the connected Stripe payment flow. Refunds can be partial or full. Stripe's result is reflected in the refund record, and a failed refund can be retried with **Retry refund** from the Spaces interface.

## Bank-transfer refunds

Bank transfers are external payments. Spaces does not automatically return money transferred directly between the Customer and the operator. The operator returns the money outside Skedular, then uses the **Needs manual follow-up** action to move the refund into manual handling and **Mark manual payout complete** to record that the external payout is finished.

## Xero accounting

When Xero processing is available, Spaces creates an authorized credit note in the connected Xero account against the original invoice. If the credit note cannot be created successfully, the refund remains unresolved until the accounting action is completed or the operator records another outcome with **Mark complete** or **Mark failed**. See [Xero accounting](/docs/spaces/billing-and-payments/xero-accounting) for the integration details.

## Bookings and Subscriptions

For a one-time Booking, cancellation releases the Booking according to the cancellation rules, while any eligible refund is processed separately. For a Subscription, an immediate cancellation may trigger refund evaluation for the current billed period; ending it at period end normally keeps that period active and does not automatically create a refund. A Subscription refund applies to the eligible payment for that billed period, not to each associated Booking separately. Cancelling a Subscription, cancelling an individual Booking, stopping future billing, and refunding an existing payment are separate actions.

## Refund statuses

The refund record and timeline show whether a refund was requested, is waiting for accounting or provider processing, completed, failed, or requires manual follow-up. These outcomes are separate from Booking cancellation, Subscription status, payment status, and invoice status.

## What a refund changes

A completed refund records the financial reversal and any provider reference. It does not automatically cancel or restore a Booking or Subscription, change future Subscription billing, release or reserve a Resource, or reverse unrelated invoices. Review the Booking or Subscription workflow separately when you need to change access or recurring service.

## Next step

For the accounting connection and credit-note workflow, continue to [Xero accounting](/docs/spaces/billing-and-payments/xero-accounting).
