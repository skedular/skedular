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
updatedAt: 2026-08-01
---

## How refunds work

In Skedular Spaces, cancellation and refund are separate decisions. Cancellation changes or ends a Booking or Subscription according to the applicable rules and selected cancellation timing. A refund reverses some or all of a confirmed payment when the applicable policy allows it.

For a Booking, or for an immediate Subscription cancellation, Spaces reviews the Price's cancellation policy after the cancellation is confirmed, checks the payment, and then creates a refund record when the purchase is eligible. Operators do not issue a refund independently of this cancellation workflow. Cancellation and refund remain separate actions: cancellation changes the service or reservation, while refund changes the financial outcome.

## Booking failures and partial recurring series

Availability can change between the public booking form's availability check and the final booking write. Spaces is updating this workflow so availability is checked and claimed at the final point of commitment, failed attempts retain a recorded outcome, and the result remains visible in booking history rather than disappearing into a temporary error message.

If payment has already been captured and the Booking cannot be created, the target workflow records the failure and starts a full refund. Customers and authorized organization stakeholders receive an outcome that explains whether the Booking was created, what happened to the payment, and what action is available next. Notification delivery is separate from the booking response and is designed to remain safe to retry.

For a recurring series where only some requested dates remain available, the customer sees the successful and unavailable occurrences and the proposed unused amount. The confirmed decision window is 24 hours. If the customer accepts the partial series, the unavailable portion is refunded. If the customer rejects it or does not respond, the created occurrences are cancelled and the full payment is refunded. Treat the booking outcome and refund status as separate records while reviewing the case.

See the [booking failure and refund lifecycle overview](/blog/reliable-bookings-honest-refunds) for the customer and operator flow, including the distinction between a cancelled Booking and a completed refund.

## Cancellation policies and eligibility

The cancellation policy is configured with the Product Price. Spaces supports these customer-facing policy shapes:

- **No Refunds:** the purchase cannot be cancelled for a refund after checkout.
- **Full Refund Before Cutoff:** a 100% refund is available until the configured cutoff; after it, no refund is available.
- **Tiered Refunds:** configured cutoffs determine the percentage available at each point before the Booking or renewal.

Refund preview and creation require a confirmed payment and a confirmed, policy-eligible cancellation. A refund cannot exceed the amount paid, less any amount already refunded.

## Operator cancellation overrides

An organization owner or administrator can cancel a Booking or Subscription when the customer-facing cancellation policy would otherwise block the cancellation. The operator must provide a short reason when the policy is overridden. This permission changes only whether cancellation is allowed; it does not change the refund calculation, payment checks, resource cleanup, invoice handling, or provider approval workflow.

Customers remain subject to the published cancellation policy and cannot grant themselves operator permissions by sending an override reason. After an override, Stripe refunds continue through the automatic Stripe flow, while bank-transfer refunds still require the operator's approval and transfer confirmation, and Xero refunds still follow the accounting approval/processing flow.

## How the refund amount is determined

Spaces starts with the cancellation policy and the cancellation time, then considers the confirmed amount paid and any previous refund activity. The operator can enter a lower **Approved amount** than the policy amount, but it cannot exceed the current refundable amount.

## Review and process a refund

Eligible refunds appear with the related Booking or Subscription and on the organization's **Refunds** page. Operators can review the amount, currency, policy percentage, customer request, notes, and status history before choosing **Queue refund**. The approval dialog uses **Approved amount** and **Admin note**.

The normal eligible-refund path does not require a separate approval process. Operators queue the reviewed amount with **Queue refund**. Provider and payment-path actions are described below where they apply.

## Stripe refunds

For a Stripe-paid Booking or Subscription, Spaces submits the approved refund through the connected Stripe payment flow. Refunds can be partial or full. Stripe's result is reflected in the refund record, and a failed refund can be retried with **Retry refund** from the Spaces interface.

## Bank-transfer refunds

Bank transfers are external payments. Spaces does not automatically return money transferred directly between the Customer and the operator. The operator returns the money outside Skedular, then follows the audited workflow: **Approve refund**, **Record transfer sent** with the bank reference, and **Confirm transfer received**. The refund remains visible as **Under review**, **Approved**, or **Processing** until the transfer is confirmed. Allow up to five business days for manual settlement.

## Xero accounting

When Xero processing is available, Spaces creates an authorized credit note in the connected Xero account against the original invoice. If the credit note cannot be created successfully, the refund remains unresolved until the accounting action is completed or an operator resolves the reconciliation outcome with a reason and provider reference when applicable. See [Xero accounting](/docs/spaces/billing-and-payments/xero-accounting) for the integration details.

## Bookings and Subscriptions

For a one-time Booking, cancellation releases the Booking according to the cancellation rules, while any eligible refund is processed separately. For a Subscription, an immediate cancellation may trigger refund evaluation for the current billed period; ending it at period end normally keeps that period active and does not automatically create a refund. A Subscription refund applies to the eligible payment for that billed period, not to each associated Booking separately. Cancelling a Subscription, cancelling an individual Booking, stopping future billing, and refunding an existing payment are separate actions.

## Refund statuses

The refund record and timeline show whether a refund was requested, is under review, approved, processing with a provider, provider-pending, completed, rejected, cancelled, failed, or requires reconciliation. Stripe refunds generally update within minutes of the provider webhook; Xero credit-note processing can take up to one business day. These outcomes are separate from Booking cancellation, Subscription status, payment status, and invoice status.

The important customer-facing distinction is:

- **Approved** means the amount and refund path have been approved; it does not mean the money has returned.
- **Processing** means the provider operation has been submitted and is awaiting confirmation.
- **Provider pending** means the provider response is not final and must be checked before the refund is marked complete.
- **Completed** means the refund outcome is confirmed.
- **Reconciliation required** means local and provider records need an operator to resolve the difference.

Refund operations use a stable idempotency reference and payment allocations so retries do not submit the same financial operation twice or exceed the captured amount. If the provider response is ambiguous, leave the refund in provider-pending or reconciliation handling rather than assuming success.

## What a refund changes

A completed refund records the financial reversal and any provider reference. It does not automatically cancel or restore a Booking or Subscription, change future Subscription billing, release or reserve a Resource, or reverse unrelated invoices. Review the Booking or Subscription workflow separately when you need to change access or recurring service.

## Next step

For the accounting connection and credit-note workflow, continue to [Xero accounting](/docs/spaces/billing-and-payments/xero-accounting).
