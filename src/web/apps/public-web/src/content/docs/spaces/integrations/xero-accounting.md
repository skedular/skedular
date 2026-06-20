---
id: spaces-xero
title: "Xero accounting"
description: "Connect Skedular Spaces with Xero to manage invoices, accounting records, payments, and refund-related credit notes."
product: spaces
category: settings
slug: xero-accounting
articleKind: guide
publicationState: published
evidenceRefs:
  - src/web/apps/webapp-spaces/src/components/organization/organizationMarketplaceSetup/organization-marketplace-setup.tsx
  - src/organization/apis/Organization.Api/Services/OrganizationXeroConnectionService.cs
  - src/booking/shared/Booking.Shared/Services/XeroInvoiceService.cs
  - src/booking/shared/Booking.Shared/Services/XeroRepeatingInvoiceScheduleService.cs
  - src/booking/shared/Booking.Shared/Services/RecurringInvoiceBillingScheduleService.cs
  - src/booking/shared/Booking.Shared/Services/XeroRefundService.cs
  - docs-resources/xero-integration.md
  - docs-resources/billing-and-payouts.md
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - spaces-bank-payments
  - spaces-refunds
updatedAt: 2026-07-17
---

## Understand the Xero integration

Xero is Skedular Spaces' accounting and invoicing integration. Skedular remains responsible for Products, Prices, Bookings, Subscriptions, cancellation rules, and billing decisions. When the Organization is configured to use Xero, Skedular prepares the invoice data and sends it to the connected Xero tenant. Xero then manages the connected accounting record, while relevant payment and invoice status can return to Skedular.

Xero is not a customer payment method. Customers pay through the payment methods enabled for the Product Price. See [Payment methods](/docs/spaces/billing-and-payments/payment-methods) for the distinction between Stripe, bank transfer, and accounting integrations.

## Connect Xero

Only Organization owners and administrators can configure the connection. In the Organization admin area, open the **Xero** section and choose **Connect Xero**. Skedular redirects you to Xero to authorize access. If your Xero login can access multiple tenants, return to Skedular and choose the tenant shown under **Available Xero Tenants**.

After a tenant is selected, the Xero section shows **Connected to** followed by the tenant name and **Connection active**. The connection normally remains active without operator action. If Xero authorization expires or is revoked, choose **Reconnect Xero**. To stop the connection, choose **Disconnect**.

## Configure Organization-level accounting settings

The Xero settings apply to the Organization's exported invoices and recurring invoice behavior:

- **Billing Mode** is the Xero integration's invoice/export mode. It controls whether Xero is **Disabled**, **Enabled** for standard invoices, or set to **Repeating Invoices**. It is separate from the Product Price billing mode, such as Upfront or In arrears.
- **Default Sales Account Code** supplies the sales account used when Skedular creates an invoice in Xero.
- **Tracking Category 1** and **Tracking Category 2** add optional Organization-level reporting categories.
- **Branding Theme ID** selects the Xero branding theme applied to exported invoices.
- **Reference Prefix** adds a prefix to exported invoice references.

The Product Price still controls the commercial amount and tax-inclusive or tax-exclusive treatment. Xero settings provide the accounting context; they do not change Product pricing, Availability, Resource allocation, or cancellation policy.

## How invoices flow to Xero

When a supported Booking or recurring billing event requires an invoice, Skedular creates the local invoice and prepares an accounting export link. It then creates the corresponding Xero invoice with the configured Customer contact, Product and Booking description, amount, currency, due date, sales account, reference, and tax treatment from the Product Price.

With **Billing Mode** set to **Disabled**, Skedular does not export invoices to Xero. With **Enabled**, it exports standard Xero invoices. With **Repeating Invoices**, it creates repeating templates when the cadence is supported and uses standard invoices as a fallback when it is not. For exported invoices and templates, the implementation's invoice-sending setting determines whether Xero creates them as **Authorized** or **Draft**. Skedular stores the relationship between the local invoice and the Xero invoice, including the Xero invoice number when available.

## Booking and Subscription invoices

One-time Booking invoices use the Customer who paid for the Booking and include the Product and Booking context needed for accounting. The first Subscription invoice is generated when the Subscription starts for both upfront and in-arrears billing. The invoice due date is calculated separately from the Subscription purchase cadence and the Organization billing cycle.

For later Subscription invoices, upfront billing follows the Subscription purchase period. In-arrears billing follows the Organization's configured billing cycle. Xero participates after Skedular has determined that an invoice is due; Xero does not determine the Subscription schedule.

## Repeating invoices

When the Organization selects **Repeating Invoices**, Skedular can create a Xero repeating invoice for supported recurring cadences. The schedule follows the Product purchase cadence unless a longer cadence is split into the Organization billing cycle for in-arrears billing. The supported cadences are:

- **Weekly**
- **Fortnightly**
- **Monthly**
- **Two months**
- **Quarterly**
- **Four months**
- **Five months**
- **Six months**
- **Yearly**

If a cadence cannot be represented as a Xero repeating schedule, Skedular uses a standard Xero invoice instead. When a Subscription ends, its active repeating invoice is stopped. Existing repeating exports are not silently rewritten when billing settings change; Skedular keeps the existing export stable and requires an explicit transition where needed.

## Customer and Xero Contacts

Before exporting an invoice, Skedular reuses an existing linked Xero Contact where possible and creates or updates one when needed. The Customer remains a registered Skedular user and is not made a member of the operator's Organization by being represented as a Xero Contact.

## Payment reconciliation

When **Auto Reconcile Payments** is enabled, Skedular checks the connected Xero invoice and updates the local payment status for the related marketplace Booking or recurring Booking when Xero reports that invoice as paid. Operators do not need to copy each reconciled payment manually. Payment status remains separate from the Booking lifecycle, Subscription lifecycle, and invoice status; an unpaid or overdue Xero invoice does not by itself cancel the Booking or Subscription.

## Refunds and credit notes

For a refund with a linked Xero invoice, an operator can choose **Send to Xero** from the refund workflow. Skedular creates an **Authorized** credit note in Xero against the original invoice and records the Xero credit-note reference on the refund. The credit note represents the accounting adjustment; it does not itself move money back to the Customer.

If the credit note cannot be created or the original invoice cannot be resolved, the refund remains unresolved until the accounting action is completed or the operator records another outcome. See [Refunds](/docs/spaces/billing-and-payments/refunds) for refund eligibility and payment-path handling.

## Manage connection issues

The Xero section shows whether the connection is active, the selected tenant, the last successful sync, and the last sync error when one is available. If authorization expires or is revoked, use **Reconnect Xero**. If an invoice export is blocked by a missing mapping, unavailable tenant, or unresolved Xero invoice, correct the Organization configuration or Xero connection before continuing the affected workflow.

## What Xero does not control

Xero does not determine Product pricing, Booking Availability, Subscription purchase cadence, cancellation policy, Resource allocation, or Organization membership. Skedular owns those operational and commercial rules; Xero manages the connected accounting records.

## Next step

Continue to [Operator analytics](/docs/spaces/analytics) to understand workspace usage, Booking patterns, and operational performance.
