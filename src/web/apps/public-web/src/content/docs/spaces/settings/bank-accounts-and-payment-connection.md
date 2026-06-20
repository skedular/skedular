---
id: spaces-bank-payments
title: "Payment methods"
description: "Configure how customers pay for Products and how operators manage each payment flow in Skedular Spaces."
product: spaces
category: settings
slug: bank-accounts-and-payment-connection
articleKind: guide
publicationState: published
evidenceRefs:
  - doc-resources/billing-and-payouts.md
  - doc-resources/xero-integration.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - spaces-refunds
  - spaces-xero
updatedAt: 2026-07-17
---

## Understand payment methods

Skedular Spaces supports card payments through a connected Stripe account and manual bank transfers using the Organization's configured bank details. Xero supports invoicing and accounting workflows; it is not a Customer payment method. At checkout, the available methods are the payment types supported by the Organization and accepted by the selected Product Price. An empty accepted-method list allows all supported Organization methods, while a Price can restrict the choices; customers can choose among the methods that remain available.

## Configure payment methods

Operators manage the Organization's payment connections and bank details in the payment settings used by Spaces. Stripe requires an external connection. Bank transfer requires the bank details and payment instructions customers should receive. Xero requires an Organization accounting connection. The detailed setup and connection steps belong in [Xero accounting](/docs/spaces/billing-and-payments/xero-accounting); this page explains how each path behaves.

## Stripe card payments

When Stripe is connected and enabled for the purchase, the Customer completes online card checkout through Stripe. A successful provider result updates the related payment status automatically, and operators can see that status on the Booking or Subscription payment activity. Stripe processes the card payment through the Organization's connected account; Skedular tracks the resulting status and does not hold the card funds.

## Bank transfers

The Customer receives the Organization's configured bank-transfer instructions and sends the payment outside Skedular, directly to the Organization's bank account. The related payment remains **Pending** until an operator confirms or rejects it in Skedular. The exact actions are **Confirm Booking Payment** and **Reject Booking Payment**; where supported, **Make Booking Payment Not Required** records that payment is not required. Skedular does not automatically know that an external transfer has arrived.

## Xero payment workflows

When the Organization's Xero connection is configured, Xero can receive invoices and support payment recording and reconciliation. Skedular keeps the related invoice and payment status available to the operator while Xero remains the accounting system for the connected workflow. See [Xero accounting](/docs/spaces/billing-and-payments/xero-accounting) for configuration and reconciliation details.

## Payment status and confirmation

Payment status is separate from the Booking or Subscription lifecycle and from invoice status. Operators may see **Pending**, **Confirmed**, or **Rejected** payment states. Stripe can confirm a successful card payment automatically; bank transfers require operator confirmation. Xero synchronization can update accounting and payment information when the connected workflow supports it.

Manually confirming a payment records the payment state in Skedular. It does not move money between bank accounts. Payment actions apply to the financial activity and should not be confused with cancelling or completing a Booking.

## How payments reach the operator

Stripe processes card payments through the Organization's connected Stripe account. Bank transfers move directly from the Customer to the Organization's configured bank account. Xero supports invoicing, payment recording, and reconciliation rather than collecting Customer payments. Skedular does not hold Customer card or bank-transfer funds.

## Invoices and payment methods

Invoice behavior depends on the configured path. When a bank-transfer purchase uses an invoice-supported billing path, the Customer receives the generated invoice together with the Organization's payment instructions and the payment remains pending for operator confirmation. Stripe checkout records the provider payment result, while invoice activity depends on the configured billing and accounting workflow. Skedular prepares invoice data and sends it to Xero, where connected invoices are managed and their accounting status is synchronized. Not every payment method follows the same invoice lifecycle.

## Next step

Continue to [Refunds](/docs/spaces/billing-and-payments/refunds) to understand the financial outcome when an eligible payment needs to be returned after a cancellation or other supported scenario.
