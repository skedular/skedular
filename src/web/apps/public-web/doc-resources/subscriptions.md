# Subscriptions

## Overview

Subscriptions represent long-term or recurring bookings in **Skedular Spaces**.

A subscription is created whenever a customer purchases a recurring offer or an offer that spans more than a single day.

Rather than representing a single booking, a subscription manages an ongoing relationship between the customer and the organization. It controls recurring billing, recurring bookings, renewals, and payment status.

Subscriptions provide a single place for administrators to manage customers who regularly return and continue using the same resources or products.

---

# Availability

Subscriptions are currently available in:

- **Skedular Spaces**

Subscriptions are not used in:

- Skedular Teams
- Skedular Host

---

# Why Subscriptions Exist

Subscriptions simplify the management of long-term customers.

Instead of creating new bookings every day or every month, Skedular creates a subscription that automatically manages:

- Future bookings.
- Billing periods.
- Renewals.
- Payments.
- Invoices.

This allows both customers and workspace operators to manage ongoing bookings with minimal administration.

---

# How Subscriptions Work

When a customer purchases an eligible recurring offer, Skedular creates:

- A subscription.
- One or more bookings.
- A billing schedule.
- Payment information.
- Renewal information.

The subscription becomes the parent record for all future recurring bookings.

---

# What Creates a Subscription?

Subscriptions are typically created when a customer purchases offers such as:

- Weekly memberships.
- Fortnightly memberships.
- Monthly memberships.
- Quarterly memberships.
- Six-month memberships.
- Annual memberships.

Daily and one-time bookings do **not** create subscriptions.

Instead, they create standalone bookings.

---

# Subscription Contents

A subscription contains information such as:

- Customer.
- Product.
- Offer.
- Current status.
- Current billing period.
- Next renewal date.
- Payment method.
- Quantity.
- Auto-renewal settings.
- Invoice history.
- Associated bookings.

This allows administrators to manage the complete customer lifecycle from a single place.

---

# Recurring Bookings

A subscription is linked to a series of bookings.

Instead of storing one long booking, Skedular generates individual bookings for each booking period.

For example, a monthly desk membership may generate a series of daily bookings throughout the membership period.

These bookings remain linked to the same subscription.

This makes it possible to manage long-term reservations while preserving the flexibility of individual bookings.

The initial required booking series is confirmed as a whole. If Skedular cannot allocate capacity for every required occurrence, it does not present a partial series as confirmed. It retains the availability outcome and notifies the customer and authorized organization stakeholders.

After a subscription is active, a later occurrence can fail independently when capacity is no longer available. Skedular retains and communicates that outcome without cancelling the subscription or unrelated occurrences. If payment for a recurring cycle expires or fails before confirmation, Skedular releases the affected unpaid-cycle capacity while preserving the subscription configuration for normal lifecycle and operator review.

---

# Auto Renewal

Subscriptions can optionally renew automatically.

When auto-renewal is enabled:

- The current billing period completes.
- A new billing period begins.
- Future bookings continue to be generated.
- Billing continues according to the offer.

When auto-renewal is disabled:

- The subscription ends after the current billing period.
- No further bookings are generated.
- No additional invoices are created.

---

# Billing

Subscriptions manage recurring billing.

Each subscription records:

- Billing periods.
- Payment status.
- Payment method.
- Renewal dates.
- Invoice history.

Billing follows the configuration defined by the purchased offer.

---

# Invoices

Invoices are associated with subscriptions.

Each billing period can generate its own invoice depending on the organization's billing configuration.

Administrators can review invoice information alongside the subscription.

---

# Subscription Status

Subscriptions progress through different states during their lifecycle.

Typical statuses include:

- Active
- Scheduled to End
- Expired
- Cancelled

These statuses help administrators understand the current state of each customer's recurring booking.

---

# Managing Subscriptions

Organization owners and administrators can:

- View subscriptions.
- Review recurring bookings.
- Monitor payment status.
- Review invoices.
- Stop automatic renewal.
- End subscriptions immediately or at the end of the current billing period.
- Review customer information.

Subscriptions provide a central place for managing long-term customer relationships.

---

# Daily Bookings

Not every booking appears in the subscription list.

One-time bookings, including daily bookings, remain in the Bookings area.

Only bookings that belong to a recurring subscription are displayed in the Subscriptions section.

This keeps one-time bookings and long-term customer relationships clearly separated.

---

# Product Pricing Changes

Subscriptions preserve the pricing that was active when they were created.

If the organization updates the pricing of a product:

- Existing subscriptions continue using their current pricing for the active billing period.
- The current bookings remain unchanged.
- When the subscription renews, the latest version of the pricing is automatically applied.

This ensures pricing changes do not affect customers who have already paid for their current subscription period.

---

# Best Practices

For the best experience:

- Review subscriptions regularly.
- Monitor upcoming renewals.
- Verify payment status before renewal.
- Keep customer payment methods up to date.
- End subscriptions that are no longer required.
- Review invoice history when investigating billing issues.

---

# Things to Know

- Subscriptions are available only in **Skedular Spaces**.
- Subscriptions represent recurring customer relationships.
- Daily bookings do not create subscriptions.
- Subscriptions manage recurring bookings automatically.
- Subscriptions manage billing and invoices.
- Auto-renewal is optional.
- Existing subscriptions are protected from pricing changes during the current billing period.
- Renewals automatically use the latest version of the purchased offer.
- Every recurring booking belongs to a subscription.

---

# Example

A customer purchases a **Premium Desk Monthly Membership**.

Skedular creates:

- A subscription.
- The current month's recurring bookings.
- The first invoice.
- A monthly renewal schedule.

The subscription remains active throughout the month.

Halfway through the month, the coworking operator increases the monthly membership price.

The customer's current subscription continues at the original price until the billing period ends.

When the subscription renews the following month, the latest version of the offer and its updated pricing are automatically applied.

---

# Related Concepts

Operators should use the Marketplace purchases history to review both recurring Subscriptions and one-time marketplace Bookings. The history is retained according to the existing purchase-record policy and is ordered by latest activity; cancellation, deletion, payment, and refund state remain separate concerns.

- Products
- Offers
- Bookings
- Customers
- Payments
- Invoices
- Product Versioning
- Organizations
