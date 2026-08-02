---
id: shared-bookings
title: "Bookings"
description: "Bookings reserve one or more resources for a defined period and power scheduling across Skedular."
product: shared
category: core-concepts
slug: bookings
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/booking.md
  - doc-resources/resource.md
  - doc-resources/team.md
  - doc-resources/subscriptions.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-resources
  - shared-availability
  - shared-users
  - shared-teams
  - shared-products
  - shared-subscriptions
updatedAt: 2026-08-02
---

## Overview

A booking reserves one or more [Resources](/docs/shared/core-concepts/resources) for a specific period. Bookings connect users and customers to the resources they need, while [Availability](/docs/shared/core-concepts/availability) determines when those resources can be reserved.

<div class="documentation-concept-support"><strong>Supported in</strong><span>✅ Skedular Teams</span><span>✅ Skedular Spaces</span><span>✅ Skedular Host</span></div>

<aside class="documentation-callout" aria-label="Core rule"><strong>Core Rule</strong><p>Every booking reserves one or more resources for a defined period of time.</p></aside>

## How Bookings Work

The booking lifecycle is straightforward:

<div class="documentation-concept-workflow"><span><b>1</b>Choose Resource or Product</span><span><b>2</b>Check Availability</span><span><b>3</b>Create Booking</span><span><b>4</b>Confirm Booking</span><span><b>5</b>Manage Booking</span><span><b>6</b>Cancel or Complete</span></div>

When a booking is confirmed, the selected resources become unavailable for the reserved period. Changes and cancellation follow the permissions and product workflow that created the booking.

## Where This Concept Fits

<div class="documentation-organization-context" aria-label="Booking relationships"><div class="documentation-context-root">👤 <a href="/docs/shared/core-concepts/users">User</a><div class="documentation-context-child"><span>📅 Booking</span><div class="documentation-context-child"><a href="/docs/shared/core-concepts/resources">🪑 Resources</a><a href="/docs/shared/core-concepts/availability">⏱ Availability</a><a href="/docs/shared/core-concepts/teams">👥 Teams</a><a href="/docs/shared/marketplace/products">🛒 Products</a><a href="/docs/shared/marketplace/subscriptions">🔁 Subscriptions</a></div></div></div><div class="documentation-context-tree"><a href="/docs/shared/core-concepts/organizations">🏢 Organization</a></div></div>

## Booking Ownership

Every booking belongs to one organization and is created by, or on behalf of, users or customers who are allowed to use the selected resources. The organization controls the locations, resources, rules, and visibility that apply to its bookings.

## Resources and Duration

A booking can reserve one or more resources, such as desks, rooms, parking spaces, or equipment. Bookings use 15-minute intervals and can be made up to one year in advance. A single booking exists within one calendar day; a reservation that repeats across days is represented by a recurring booking group instead of one booking spanning multiple days.

## Recurring Bookings

Recurring bookings group repeated daily bookings under one schedule. The group stores recurrence details such as frequency, interval, dates, and skipped dates. Marketplace subscriptions can create recurring booking cycles, but a subscription is the commercial agreement and the recurring bookings are the reservations it produces. See [Subscriptions](/docs/shared/marketplace/subscriptions) for the commercial model.

## Team Bookings

In [Skedular Teams](/docs/shared/core-concepts/teams), an authorized user can select a team when creating a booking. Skedular creates the required bookings for the team's members, subject to resource availability and booking rules. Team membership changes affect future bookings and do not rewrite bookings that already exist.

## Product Bookings

- **Skedular Teams:** Users generally select resources directly for private workplace bookings.
- **Skedular Spaces:** Customers usually select marketplace Products. The product and its Product Tags guide resource allocation behind the booking.
- **Skedular Host:** Guests book Places through a simplified workflow. The underlying resource is managed automatically.

## Availability and Conflicts

Before a booking is confirmed, Skedular checks the selected resources and their availability. A confirmed booking makes those resources unavailable for the reserved period and prevents overlapping reservations for the same resource. See [Availability](/docs/shared/core-concepts/availability) for the scheduling model.

## Cancellation

Cancellation ends the booking's future resource entitlement according to the applicable product and organization rules. It is separate from any refund or accounting decision. Use the relevant product documentation for cancellation windows and refund behavior.

The customer-facing experience shows **Cancel** only when the booking can be cancelled by the current actor. When cancellation is unavailable, the booking details show the reason instead of presenting an action that cannot succeed. An owner or administrator with the required permission may be able to override a product cancellation restriction; an override requires a short cancellation reason for the organization's audit history.

## Product Differences

<div class="documentation-concept-grid"><div><strong>📅 Skedular Teams</strong><small>Resource-first booking for private workplace scheduling, including team bookings.</small></div><div><strong>🛒 Skedular Spaces</strong><small>Product-first marketplace bookings where products allocate suitable resources.</small></div><div><strong>🏠 Skedular Host</strong><small>Place-first bookings with the underlying resource managed behind the scenes.</small></div></div>

## What Belongs to a Booking

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>The assets reserved by the booking.</small></a><a href="/docs/shared/core-concepts/availability"><strong>⏱ Availability</strong><small>The schedule checked before confirmation.</small></a><a href="/docs/shared/core-concepts/users"><strong>👤 Users</strong><small>People who create or use bookings.</small></a><a href="/docs/shared/core-concepts/teams"><strong>👥 Teams</strong><small>Groups that can create member bookings in Skedular Teams.</small></a><a href="/docs/shared/marketplace/products"><strong>🛒 Products</strong><small>Marketplace offers that can lead to resource bookings.</small></a><a href="/docs/shared/marketplace/subscriptions"><strong>🔁 Subscriptions</strong><small>Commercial agreements that can generate recurring bookings.</small></a><a href="/docs/shared/commerce/payments"><strong>💳 Payments</strong><small>Payment state connected to commercial bookings.</small></a></div>

## Best Practices

- Reserve resources only for the time they are needed.
- Check availability before confirming a booking.
- Cancel bookings that are no longer required.
- Use recurring bookings for repeated reservations.
- Keep the people and resource details on a booking accurate.

## Things to Know

- Every booking belongs to one organization.
- Bookings reserve one or more resources.
- Bookings use 15-minute intervals.
- Bookings can be made up to one year in advance.
- A single booking cannot span multiple calendar days.
- Multi-day reservations are represented by recurring booking groups.
- Products ultimately lead to resource bookings in marketplace workflows.
- Team bookings create bookings for team members.

## Frequently Asked Questions

### What is a booking?

A booking is a reservation of one or more resources for a defined period.

### Can one booking reserve multiple resources?

Yes. A booking can include one or more resources when the workflow requires them together.

### Can a booking span multiple days?

No. A single booking stays within one calendar day. Multi-day reservations use a recurring booking group.

### What happens when I cancel a booking?

The booking's future resource entitlement ends according to the applicable product and organization rules. Refunds, when relevant, are handled separately.

### Why can I not cancel a booking?

Cancellation may be unavailable because the product's cancellation window has passed, the booking is in a state that cannot be cancelled, or the current actor does not have permission. Skedular shows the applicable reason in the booking details. An authorized owner or administrator may be able to override the restriction and must provide a reason when doing so.

### What is the difference between a booking and a subscription?

A booking reserves resources for a period. A subscription is a commercial agreement that can generate recurring bookings over multiple cycles.

### How do recurring bookings work?

A recurring schedule groups repeated bookings and stores the recurrence dates, frequency, and exceptions together.

### How do team bookings work?

An authorized user selects a team and resources. Skedular creates the required bookings for the team members, subject to availability and booking rules.

## Continue Learning

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>The assets bookings reserve.</small></a><a href="/docs/shared/core-concepts/availability"><strong>⏱ Availability</strong><small>When resources can be booked.</small></a><a href="/docs/shared/core-concepts/teams"><strong>👥 Teams</strong><small>Group bookings for workplace coordination.</small></a><a href="/docs/shared/core-concepts/users"><strong>👤 Users</strong><small>The people who create and use bookings.</small></a></div>

## Related Concepts

- [Organizations](/docs/shared/core-concepts/organizations)
- [Resources](/docs/shared/core-concepts/resources)
- [Availability](/docs/shared/core-concepts/availability)
- [Users](/docs/shared/core-concepts/users)
- [Teams](/docs/shared/core-concepts/teams)
- [Products](/docs/shared/marketplace/products)
- [Subscriptions](/docs/shared/marketplace/subscriptions)
