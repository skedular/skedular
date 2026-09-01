---
id: shared-billing-payouts
title: "Billing and Payouts"
description: "Understand when commercial amounts become due, how billing schedules work, how invoices relate to billing, and how funds reach workspace operators."
product: shared
category: commerce
slug: billing-and-payouts
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/billing-and-payouts.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-commerce
  - shared-payments
  - shared-products
  - shared-subscriptions
  - shared-bookings
  - shared-organizations
updatedAt: 2026-07-15
---

<div class="documentation-concept-support"><strong>Available in</strong><span>✅ Skedular Spaces</span></div>

## Overview

Billing determines when a commercial amount becomes due and how recurring charges are scheduled. The commercial terms come from the selected Product and Price, while Organization settings provide payment connections, bank details, invoice terms, and accounting options. Bookings and Subscriptions carry the access being purchased; payment activity records the financial state of the related purchase or billing period.

Skedular does not create one universal Payout record. In this guide, payout describes how funds reach the Organization: Stripe manages settlement for card payments through the Organization's connected account, while a bank transfer moves directly from the Customer to the Organization. Xero supports invoicing and accounting workflows rather than acting as a Customer payment method.

<aside class="documentation-callout" aria-label="Billing core rule"><strong>Core rule</strong><p>Billing determines when amounts become due; payment activity describes their financial state; the selected payment path determines how funds reach the Organization.</p></aside>

## Billing, payments, invoices, and settlement

<div class="documentation-concept-grid"><div><strong>🗓 Billing</strong><small>Sets when a commercial amount becomes due and how recurring periods are scheduled.</small></div><div><strong>💳 Payment activity</strong><small>Stores payment method and status on the related Booking or Subscription billing period.</small></div><div><strong>🧾 Invoice</strong><small>Represents an amount due when the configured billing workflow uses invoicing.</small></div><div><strong>💰 Settlement</strong><small>Describes how funds reach the Organization through Stripe or directly by bank transfer.</small></div></div>

## Billing relationships

<div class="documentation-organization-context" aria-label="Billing relationships"><div class="documentation-context-root">🛒 Product + Price<div class="documentation-context-child"><span>⚙️ Billing rules</span><div class="documentation-context-child"><a href="/docs/shared/core-concepts/bookings">📅 Booking</a><a href="/docs/shared/marketplace/subscriptions">🔁 Subscription</a><div class="documentation-context-child"><span>💳 Payment activity</span><a href="/docs/shared/commerce/payments">🧾 Invoice and payment status</a></div></div></div></div><div class="documentation-context-tree"><span>💳 Card → Stripe → provider-managed settlement</span><span>🏦 Bank transfer → Customer pays Organization directly</span><span>📚 Invoice activity → Xero accounting workflow</span></div></div>

The diagram shows related paths, not one mandatory sequence. A Product and Price define commercial terms, a Booking or Subscription applies those terms to access, and payment or invoice activity appears when an amount becomes due. The payment method and accounting connection determine what happens next.

## Where billing is configured

### Organization

Organization administrators configure shared financial infrastructure, including Stripe Connect, Xero, Organization bank details, invoice payment terms, and the billing cadence used for in-arrears invoice grouping.

### Product and Price

The selected Product Price defines the commercial terms used by a purchase. These terms include whether the Price is Upfront or In Arrears, purchase cadence, accepted payment methods selected from Organization-supported methods, cancellation rules, and tax treatment where configured. Product and Price documentation explains the commercial options in detail.

### Booking and Subscription

The Booking or Subscription carries the selected terms into the access workflow. A Subscription applies them across recurring billing periods; a one-time Booking applies them to its purchase flow.

## Purchase cadence and billing cadence

- **Purchase cadence** describes the access period the Customer buys, such as a week, month, or year.
- **Auto-renewal** determines whether the selected purchase term repeats.
- **Billing cadence** describes how often an in-arrears amount is invoiced or collected.

Booking duration comes from the customer's selected start and end times within the Product Price minimum and maximum. The Organization's billing cadence can split a longer purchase term into invoice and resource-booking slices.

## Upfront and in-arrears billing

### Upfront

With upfront billing, the purchase amount becomes due when the Booking or Subscription purchase is created. Card payments can be processed through the connected Stripe account. Bank-transfer purchases follow the Organization's payment instructions and manual confirmation path. The access record and payment status remain separate concepts.

### In arrears

With in-arrears billing, the billing period is allowed to run before the amount becomes due. Skedular calculates the applicable amount, creates the configured invoice or payment activity, and follows the selected payment and accounting path. In-arrears billing is commonly used for recurring activity where the amount is settled on a regular schedule.

## Billing cadence and due dates

The Organization's billing cadence controls how often in-arrears billing is grouped and how longer purchase terms are sliced for invoices and resource-booking slices. The supported user-facing schedules are weekly, fortnightly, and monthly. Billing cadence does not replace purchase cadence.

Invoice payment terms use an Organization-level due-date setting. The due date applies to invoices and payment paths that use invoice terms; it does not change how Stripe card authorization or provider settlement works.

## One-time Booking billing

A one-time marketplace Booking uses the billing mode and payment methods from its selected Product Price. With an Upfront Price, the amount becomes due when the Booking is created during the purchase. With an In Arrears Price, the amount follows the configured billing period instead. If the flow uses an invoice or bank transfer, the invoice and confirmation steps follow that configuration. See [Bookings](/docs/shared/core-concepts/bookings) for the access and Resource reservation lifecycle.

## Subscription billing and renewal

The first Subscription purchase period carries the selected Product Price terms. That purchase period can contain multiple billing periods when the Organization bills in arrears. Each renewal creates the next purchase period with its applicable billing activity. Auto-renewal proceeds only when an eligible renewable Price is available; the renewed period uses the applicable current Product and Price version while the active period keeps its existing terms.

Disabling renewal stops the creation of future periods. Ending or cancelling access does not automatically erase invoices or financial activity that already exists. See [Subscriptions](/docs/shared/marketplace/subscriptions) for the complete access and renewal lifecycle.

## Invoices and billing

Billing determines when an amount becomes due. When the configured financial path uses invoicing, an Invoice represents that amount and its due date. Not every payment flow is a one-to-one Invoice and payment-status pair. Card and bank-transfer flows can follow different invoice and confirmation steps, and recurring billing can create recurring invoice activity.

## How operators receive funds

### Card payments and Stripe

Card payments are processed through the Organization's connected Stripe account. Stripe manages provider settlement and payout timing according to that connected account's configuration. Skedular tracks the resulting Commerce status and does not hold the Customer's card funds or control Stripe's payout schedule.

### Bank transfers

Bank transfers move directly from the Customer to the Organization using the bank details configured by the Organization. Skedular displays the instructions and tracks the related payment status; the operator confirms or rejects the transfer in the supported workflow. Skedular does not process or transfer the bank funds.

### Xero

Xero is an accounting and invoicing integration. Depending on the Organization's configuration, invoice activity can be exported to Xero, including recurring invoice workflows. Xero can support reconciliation, but it is not a Customer payment method and is not described here as a payout service.

## Taxes

Tax treatment is part of the commercial Product Price configuration. Where configured, prices can be treated as tax-inclusive or tax-exclusive and the resulting tax information can appear in invoice calculations. This page does not provide tax or legal advice.

## Cancellation, refunds, and billing

Cancellation stops future access, future Booking generation, or future Subscription periods according to the applicable Booking, Subscription, and Product Price rules. It does not automatically retract an invoice that has already been generated or payment activity that already exists. Refund eligibility and amount follow the applicable cancellation policy and payment state; eligible card refunds may be processed through the connected payment provider, while bank-transfer refunds require an Organization-managed process.

## Managing Billing

Organization owners and administrators manage shared financial infrastructure: billing cadence, invoice terms, Stripe connection, Xero connection, and bank details. Each Product Price manages its billing mode and selects accepted payment methods from the methods supported by the Organization. Operators can review billing periods, invoices, payment status, and Subscription renewal activity through the workflows available to their role.

## Who can manage billing

Access to billing configuration and payment-management actions is permission-controlled. Organization owners and administrators manage Organization-level financial settings; other operators can only review or act on billing and payment information when their assigned permissions allow it.

## Best practices

- Choose billing cadence independently from purchase cadence when the commercial model requires it.
- Use upfront billing when the amount should be due at purchase; use in-arrears billing when delayed collection fits the workflow.
- Keep Stripe, Xero, and Organization bank details current before publishing paid Products.
- Align invoice due dates with the Organization's payment terms.
- Review upcoming Subscription billing periods before changing Product pricing or renewal settings.
- Review unresolved payment activity before treating a billing period as complete.

## Things to Know

- Billing is part of Commerce and is available in Skedular Spaces.
- Billing and payment activity are different concepts.
- Purchase cadence and billing cadence describe different schedules; booking duration comes from the selected start and end times within the configured limits.
- Upfront and in-arrears billing make amounts due at different points in the access lifecycle.
- Invoices exist when the configured billing workflow uses them.
- Stripe manages card settlement through the connected Organization account.
- Bank transfers move directly between the Customer and Organization.
- Xero supports invoicing and accounting workflows rather than Customer payment collection.
- Skedular does not hold card funds or control Stripe payout timing.
- Existing invoices and payment activity are not automatically erased when access ends.

## Frequently Asked Questions

### What is billing in Skedular?

Billing determines when commercial amounts become due and how recurring charges are scheduled.

### How is billing different from payment?

Billing sets the timing and terms. Payment activity describes the method and current financial status associated with the amount due.

### What is the difference between purchase, booking, and billing cadence?

Purchase cadence defines the access period, auto-renewal defines repetition, and billing cadence defines how longer terms are grouped and collected in arrears.

### What is upfront billing?

The amount becomes due when the Booking or Subscription purchase is created.

### What is billing in arrears?

The access or billing period runs first, then the configured amount becomes due at the billing boundary.

### Which billing cadences are supported?

The Organization billing settings support weekly, fortnightly, and monthly schedules for in-arrears billing.

### When is an Invoice created?

An Invoice is created or synchronized when the configured billing and accounting path uses invoicing. It is not guaranteed for every payment flow.

### How does billing work for Subscriptions?

Each Subscription purchase period carries its commercial terms and can contain one or more billing periods. Each billing period creates the relevant payment or invoice activity.

### How do card payments reach the operator?

Stripe processes the card payment through the Organization's connected account and manages settlement and payout timing.

### How do bank transfers reach the operator?

The Customer transfers funds directly to the Organization using its configured bank details. The operator then confirms the related payment status in Skedular.

### Does Skedular hold Customer funds?

No. Stripe manages card settlement, and bank transfers move directly from the Customer to the Organization.

### Does cancelling access cancel an existing Invoice?

Not automatically. Existing invoices and payment activity follow the applicable cancellation, billing, and accounting rules.

## Example

A Customer purchases a six-month workspace membership. The Subscription represents six months of access, customers choose each booking's start and end times within the offer limits, and the Organization bills monthly in arrears. Each billing period creates the relevant financial activity. If the Customer pays by card, Stripe processes the payment through the connected account. If the Customer pays by bank transfer, the Customer pays the Organization directly and an operator confirms the payment in Skedular.

## Related Documentation

- [Commerce](/docs/shared/commerce)
- [Payments](/docs/shared/commerce/payments)
- [Products](/docs/shared/marketplace/products)
- [Subscriptions](/docs/shared/marketplace/subscriptions)
- [Bookings](/docs/shared/core-concepts/bookings)
- [Organizations](/docs/shared/core-concepts/organizations)
