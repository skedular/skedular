---
id: shared-subscriptions
title: "Subscriptions"
description: "Manage recurring customer access created from marketplace Products and fulfilled through a series of Bookings."
product: shared
category: marketplace
slug: subscriptions
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/marketplace.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-products
  - shared-customers
  - shared-bookings
  - shared-payments
updatedAt: 2026-08-02
---

<div class="documentation-concept-support"><strong>Managed in</strong><span>✅ Skedular Spaces</span></div>

## Overview

A Subscription represents recurring customer access purchased through a Skedular Spaces Product. The selected Price supplies the purchase cadence and commercial terms. The Subscription keeps the access cycle active and maintains the recurring Bookings that reserve eligible Resources. Commerce handles the payment, invoice, billing, payout, and accounting records created for that access.

<aside class="documentation-callout" aria-label="Subscription core rule"><strong>Core rule</strong><p>A Subscription manages ongoing customer access; its Bookings reserve the individual Resource periods that fulfil that access.</p></aside>

## Subscriptions vs Bookings

<div class="documentation-concept-grid"><div><strong>📅 Booking</strong><small>Reserves one or more eligible Resources for a specific date and time or booking period.</small></div><div><strong>🔁 Subscription</strong><small>Represents the longer-running customer access that maintains a series of related Bookings.</small></div><div><strong>🪑 Resource</strong><small>The actual desk, room, parking space, or other entity reserved by each Booking.</small></div></div>

One Subscription can have many recurring Bookings. A Booking can exist without a Subscription, such as a one-time marketplace purchase or a private booking. A Subscription is created for a recurring purchase cadence and can temporarily have no future Booking instances while its next cycle is being prepared.

## Subscription relationships

<div class="documentation-organization-context" aria-label="Subscription relationships"><div class="documentation-context-root">👤 <a href="/docs/shared/marketplace/customers">Customer</a><div class="documentation-context-child"><span>🛒 Chooses Product + Price</span><div class="documentation-context-child"><span>🔁 Subscription</span><a href="/docs/shared/core-concepts/bookings">📅 Maintains Bookings</a><a href="/docs/shared/core-concepts/resources">🪑 Reserves Resources</a></div></div></div><div class="documentation-context-tree"><span>🔄 Reaches renewal date</span><span>💳 Creates financial activity handled by <a href="/docs/shared/commerce">Commerce</a></span></div></div>

The Subscription keeps the Product Version and Price used for its current access period. Its recurring Bookings are the reservations that fulfil that period. At renewal, Skedular checks the current Product Version for a matching Price before creating the next cycle.

## When a Subscription is created

The purchase cadence on the selected Product Price determines whether access is one-time or recurring. One-time, per-minute, 15-minute, 30-minute, hourly, and half-day purchase cadences create standalone Bookings. Daily, weekly, fortnightly, monthly, two-month, quarterly, four-month, five-month, six-month, and yearly purchase cadences use the Subscription workflow. Event Products cannot use subscription auto-renewal.

This is the canonical model: the Customer chooses a Product and Price; recurring pricing creates a Subscription; the Subscription maintains the Booking series.

Purchase cadence and Booking cadence describe different things. Purchase cadence sets the commercial access cycle. Booking cadence sets how often individual Resource reservations are created within that cycle. A monthly Subscription can therefore be fulfilled through daily Bookings.

## Booking generation and resource allocation

Skedular creates the current recurring Booking cycle and keeps future Bookings aligned with the Subscription schedule. It uses the Product's Product Tags and requested resource count to find eligible Resources. When the schedule changes, Skedular repairs or removes future Bookings and creates missing Booking days for the current cycle.

When the purchased Price has selected **Available days**, recurring generation creates Booking instances only on those calendar days. Sunday through Saturday are equal choices; an empty selection means every day. The Price rule is evaluated before normal opening-hours and Resource checks, which can still prevent an instance on an otherwise permitted day.

### Weekly customer-selected days

A weekly Price can additionally require a Customer to choose an exact number of days per week. This is separate from **Available days**: available days are the Price’s permitted pool, while the Customer’s chosen days become that Subscription’s fixed weekly pattern. This weekly field applies only to weekly Prices; fortnightly and monthly Prices use their own future settings rather than this field.

Recurring generation evaluates only the Customer’s chosen days. It never moves a Tuesday or Wednesday selection to another available weekday because capacity exists there. Calendar-day matching uses UTC because Skedular does not currently store booking time zones.

The initial Booking series is confirmed only when every required occurrence can be allocated. If capacity is unavailable for any required occurrence, Skedular does not present a partial series as confirmed. It retains the availability outcome and notifies the Customer and authorized organization stakeholders.

After a Subscription is active, a later occurrence can fail independently when capacity changes. Skedular retains that outcome and communicates it without cancelling the Subscription or unrelated occurrences. An operator can edit an individual Booking, which prevents further automatic changes to it without changing the Subscription’s weekly pattern. Refund decisions remain separate from availability and follow the Product Price cancellation policy.

Existing Booking instances are not the Subscription itself. They retain their own dates, payment state, and Resource reservations while the Subscription coordinates the recurring access that produced them.

## Renewal

Renewal is triggered when the Subscription reaches its next renewal date, not by the cadence of each individual Booking. Skedular keeps the current cycle aligned and prepares the next cycle when its renewal time arrives.

At renewal, Skedular checks the current Product Version for a Price matching the Subscription's recurring configuration. If no compatible auto-renewable Price remains, the Subscription moves to Renewal Failed instead of silently renewing with different terms. Auto-renewal must be enabled on both the Subscription and the selected Price.

## Product Version and Price changes

The Subscription keeps the Product Version and Price used for its current access period, including its available-day rule. Existing Booking instances keep the version and pricing used when they were created. At renewal, Skedular checks the current Product Version for a matching Price and uses that Price's current available-day rule for the new period. A changed or removed Price can therefore cause renewal to fail rather than silently changing the customer's commercial terms.

## Cancellation and expiry

Immediate cancellation ends the Subscription now, stops future Booking generation, and asynchronously cancels Subscription-generated Bookings from the current day onward. Past Bookings remain as historical records. Cancel at period end keeps the current cycle active, disables renewal, and changes the Subscription to Cancelled at the cycle boundary. A Subscription that reaches its renewal date without auto-renewal becomes Expired.

The customer-facing experience shows cancellation actions only when the current actor can use them. If cancellation is unavailable, the Subscription details show the reason rather than offering an action that cannot succeed. An authorized owner or administrator may be able to override a cancellation restriction; an override requires a short reason that is retained in the organization's audit history.

Cancellation stops future access and Booking generation. Immediate cancellation cancels generated current and future Bookings asynchronously, while past Bookings remain. It does not automatically retract invoices that have already been created or sent. Refund decisions are separate from cancellation and follow the Product Price cancellation policy through Booking and Commerce.

## Subscription Status

The Spaces Subscription experience exposes these lifecycle states:

<div class="documentation-concept-grid"><div><strong>Active</strong><small>The Subscription is providing access.</small></div><div><strong>Paused</strong><small>Access is temporarily paused while the Subscription remains available to resume.</small></div><div><strong>Renewal Failed</strong><small>The next cycle could not be created with a compatible auto-renewable Price.</small></div><div><strong>Cancelled</strong><small>The Subscription ended immediately or at the end of its current period.</small></div><div><strong>Expired</strong><small>The access period ended without renewal.</small></div></div>

## Billing and payments

Products and Prices define accepted payment methods, billing mode, and purchase cadence. Commerce owns payment collection, invoices, billing schedules, payouts, Stripe, Xero, and bank-account workflows. Subscription status and payment status are related but separate: a Subscription can remain a commercial access record while its current billing window is pending, paid, failed, or cancelled.

## Who Can Manage Subscriptions

Subscription management is available to operators who have access to the organization's Subscription administration workflow. Customer and member access is contextual: customers can view their own subscription experience, while members only see subscription information exposed by the workflows they are permitted to view.

## Marketplace purchase history

Operators review marketplace purchases from the organization's **Marketplace purchases** area. This unified history includes both standalone marketplace Bookings and recurring Subscriptions, so a customer purchase can be followed from its commercial details through its related Booking activity in one place.

The history includes canceled and deleted purchases when they have historical activity. These entries remain available for lifecycle, payment, and refund review; deleting or canceling a purchase does not erase its historical record. Access is limited to authorized organization operators.

## Managing Subscriptions in Skedular Spaces

Operators can use the organization's Subscriptions area to review the Customer, Product, Price, status, next renewal date, and current Booking cycle. Where cancellation is allowed, they can end access immediately or choose cancel at period end. Use Commerce workflows for invoices, payment status, refunds, payouts, and accounting follow-up.

## Example

A Customer chooses a monthly desk Product and Price. Skedular creates a Subscription for monthly access and maintains daily Bookings that reserve an eligible desk. At the renewal date, an enabled auto-renewal with a compatible Price starts the next access period. If auto-renewal is disabled, the Subscription ends after the current period. If no compatible Price exists, renewal fails.

## Best Practices

- Review Product and Price changes before an upcoming renewal.
- Use cancel at period end when the Customer should keep access through the current period.
- Check Subscription-generated future Bookings when ending access immediately.
- Keep Product Tags accurate so future cycles can find eligible Resources.
- Investigate Renewal Failed status before the next access period is expected to begin.

## Things to Know

- A Subscription is recurring customer access, not a Booking.
- One Subscription can maintain many recurring Bookings.
- Purchase cadence determines the Subscription cycle; Booking cadence determines reservation intervals.
- Auto-renewal requires a Price that supports subscription auto-renewal.
- Renewal rechecks the current Product Version and matching Price.
- Product Tags determine which Resources can fulfil generated Bookings.
- Immediate cancellation cancels generated current and future Bookings asynchronously; past Bookings remain historical records.
- Immediate cancellation and cancel at period end have different effects on the current access period.
- Commerce owns billing, payment, invoice, payout, and accounting workflows.

## Frequently Asked Questions

### What is a Subscription in Skedular?

A Subscription is the ongoing customer access created from recurring marketplace pricing. Its recurring Bookings reserve the Resources needed to fulfil that access.

### Is a Subscription the same as a Booking?

No. A Booking reserves Resources for a specific period. A Subscription coordinates the recurring access and the series of Bookings that fulfil it.

### What creates a Subscription?

Daily, weekly, fortnightly, monthly, two-month, quarterly, four-month, five-month, six-month, and yearly purchase cadences use the Subscription process. One-time, per-minute, 15-minute, 30-minute, hourly, and half-day purchase cadences use standalone Bookings.

### Can a Subscription contain daily Bookings?

Yes. Purchase cadence controls the access cycle, while Booking cadence controls individual reservations. A monthly Subscription can contain daily Bookings.

### What happens when a Subscription renews?

When the Subscription reaches its next renewal date, Skedular checks the current Product Version for a matching auto-renewable Price. If none exists, Renewal Failed is shown instead of changing the terms silently.

### What is the difference between immediate cancellation and cancellation at period end?

Immediate cancellation ends access now and cancels Subscription-generated Bookings from the current day onward. Cancel at period end keeps the current cycle active, disables renewal, and ends the Subscription at the cycle boundary.

### What happens to existing Bookings when a Subscription is cancelled?

Future generation stops and Subscription-generated current and future Bookings are cancelled asynchronously. Past Bookings remain historical records. Already-created invoices are not automatically retracted.

### Where are Subscription payments and invoices managed?

Payment collection, invoices, refunds, payouts, and accounting are handled through [Commerce](/docs/shared/commerce), using the Subscription and its recurring Bookings as the commercial context.

## Related Documentation

- [Marketplace](/docs/shared/marketplace)
- [Products](/docs/shared/marketplace/products)
- [Customers](/docs/shared/marketplace/customers)
- [Bookings](/docs/shared/core-concepts/bookings)
- [Resources](/docs/shared/core-concepts/resources)
- [Commerce](/docs/shared/commerce)
