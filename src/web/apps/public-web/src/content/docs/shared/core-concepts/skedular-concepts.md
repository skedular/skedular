---
id: shared-concepts
title: "Understanding Skedular"
description: "Understand the Skedular domain model: organizations, locations, resources, bookings, availability, products, subscriptions, and commerce."
product: shared
category: core-concepts
slug: skedular-concepts
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/organization.md
  - doc-resources/location.md
  - doc-resources/resource.md
  - doc-resources/booking.md
  - doc-resources/availability.md
  - doc-resources/floor-paln.md
  - doc-resources/tag.md
  - doc-resources/zone.md
  - doc-resources/analytics.md
  - doc-resources/user.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-14
---

## The Skedular domain model

<div class="documentation-domain-layers" aria-label="Skedular domain model">
  <section class="documentation-domain-layer"><div class="documentation-domain-layer-heading"><span>01</span><div><h3>Organization</h3><p>The ownership boundary for the operation.</p></div></div><div class="documentation-domain-layer-cards"><a href="/docs/shared/core-concepts/organizations">🏢 <strong>Organization</strong><small>Owns configuration and data</small></a><a href="/docs/shared/core-concepts/users">👤 <strong>Users</strong><small>People with access</small></a><a href="/docs/shared/core-concepts/teams">👥 <strong>Teams</strong><small>Private workplace groups</small></a></div></section>
  <section class="documentation-domain-layer"><div class="documentation-domain-layer-heading"><span>02</span><div><h3>Inventory</h3><p>The places and resources people can use.</p></div></div><div class="documentation-domain-layer-cards"><a href="/docs/shared/core-concepts/locations">📍 <strong>Locations</strong><small>Physical places</small></a><a href="/docs/shared/core-concepts/floor-plans">🗺 <strong>Floor plans</strong><small>Visual layouts</small></a><a href="/docs/shared/core-concepts/resources">🪑 <strong>Resources</strong><small>Bookable inventory</small></a><a href="/docs/shared/core-concepts/tags">🏷 <strong>Tags and zones</strong><small>Ways to organize inventory</small></a></div></section>
  <section class="documentation-domain-layer"><div class="documentation-domain-layer-heading"><span>03</span><div><h3>Activity</h3><p>How people use resources and how operators understand usage.</p></div></div><div class="documentation-domain-layer-cards"><a href="/docs/shared/core-concepts/bookings">📅 <strong>Bookings</strong><small>Reservations of resources</small></a><a href="/docs/shared/core-concepts/availability">⏱ <strong>Availability</strong><small>When resources can be used</small></a><a href="/docs/shared/insights/analytics">📊 <strong>Analytics</strong><small>What happened over time</small></a></div></section>
  <section class="documentation-domain-layer documentation-domain-layer-commerce"><div class="documentation-domain-layer-heading"><span>04</span><div><h3>Commerce in Skedular Spaces</h3><p>The commercial layer for selling access to resources.</p></div></div><div class="documentation-domain-layer-cards"><a href="/docs/shared/marketplace/products">🛒 <strong>Products and offers</strong><small>What customers purchase</small></a><a href="/docs/shared/marketplace/subscriptions">📦 <strong>Subscriptions</strong><small>Recurring access</small></a><a href="/docs/shared/commerce/payments">💳 <strong>Payments and invoices</strong><small>Collect and record payment</small></a><a href="/docs/shared/commerce/billing-and-payouts">💰 <strong>Billing and integrations</strong><small>Billing, payouts, and accounting</small></a></div></section>
</div>

This is the practical way to understand Skedular: an organization connects people, places, inventory, and usage. Skedular Spaces adds commerce when that inventory is sold.

<p class="documentation-badge-legend"><strong>Support legend:</strong> ✅ Supported &nbsp; ❌ Not available &nbsp; ⚙ Automatic &nbsp; 👤 Owner or renter access</p>

| Concept        | Skedular Teams   | Skedular Spaces  | Skedular Host           |
| -------------- | ---------------- | ---------------- | ----------------------- |
| Organization   | ✅ Supported     | ✅ Supported     | ✅ Supported            |
| Location       | ✅ Supported     | ✅ Supported     | ✅ Supported            |
| Resource       | ✅ Supported     | ✅ Supported     | ✅ Supported            |
| Users          | ✅ Supported     | ✅ Supported     | 👤 Owner and renters    |
| Teams          | ✅ Supported     | ❌ Not available | ❌ Not available        |
| Product        | ❌ Not available | ✅ Supported     | ⚙ Listing configuration |
| Booking Groups | ❌ Not available | ✅ Supported     | ❌ Not available        |
| Subscriptions  | ❌ Not available | ✅ Supported     | ❌ Not available        |
| Floor Plans    | ✅ Supported     | ✅ Supported     | ✅ Supported            |
| Availability   | ✅ Supported     | ✅ Supported     | ✅ Supported            |
| Analytics      | ✅ Supported     | ✅ Supported     | ✅ Supported            |

## Organization

An organization is the root of the Skedular domain model. Almost everything belongs to exactly one organization: [locations](/docs/shared/core-concepts/locations), [resources](/docs/shared/core-concepts/resources), [users](/docs/shared/core-concepts/users), teams, [bookings](/docs/shared/core-concepts/bookings), [products](/docs/shared/marketplace/products), subscriptions, payments, and [analytics](/docs/shared/insights/analytics).

> **Core rule:** An organization is the ownership and access boundary for the operation.

## Location

A [location](/docs/shared/core-concepts/locations) is a physical place managed by one organization. Its address, time zone, [opening hours](/docs/shared/administration/organization-settings), and layout provide the context in which resources can be booked. One organization can operate more than one location, and each location can have its own availability rules.

## Floor plan

A [floor plan](/docs/shared/core-concepts/floor-plans) is the visual layout of a location. It places resources in context so people can understand where a desk, room, or other bookable space sits.

## Resource

A [resource](/docs/shared/core-concepts/resources) is bookable inventory that belongs to exactly one location. A resource has a type and capacity, may have [tags](/docs/shared/core-concepts/tags), may belong to [zones](/docs/shared/core-concepts/zones), and may have Booking Groups in [Skedular Spaces](/docs/spaces). It inherits opening hours from its location unless an override applies. Resources are the things users and customers actually book.

> **Core rule:** Every resource belongs to exactly one location.

## User

A user is a person who has access to an organization or takes part in a booking workflow. Users may manage settings, configure resources, make bookings, or simply use a resource. Their role determines which actions and information are available to them.

### Users in Skedular Teams

[Skedular Teams](/docs/teams) uses users and teams to organize private workplace access. A team groups people who share a workplace context; individual roles determine what each person can manage or book. Read [Organize your people](/docs/teams/workplace-setup/organize-your-people) for the product workflow.

### Users in Skedular Spaces

[Skedular Spaces](/docs/spaces) separates the operator managing the Organization from registered Customers making commercial Bookings. Operator access controls workspace setup, while customer information belongs to the Booking or Subscription relationship. Read [Customers in the Skedular domain model](/docs/shared/marketplace/customers) for the distinction.

### Users in Skedular Host

[Skedular Host](/docs/host) keeps the setup focused on the person managing a place and the renters who book it. The host prepares the listing; renters interact with the published place through its booking flow. Read [Host bookings and renters](/docs/host/bookings/bookings-and-renters) for that workflow.

## Booking

A [booking](/docs/shared/core-concepts/bookings) belongs to exactly one organization and reserves one or more resources for a defined period. Bookings cannot span multiple calendar days. Visibility, approval, payment, and cancellation behavior depend on the product: [Skedular Teams](/docs/teams) centers private workplace bookings, [Skedular Spaces](/docs/spaces) supports marketplace bookings, and [Skedular Host](/docs/host) presents bookings for a published place. Recurring access is managed separately through [subscriptions](/docs/shared/marketplace/subscriptions).

## Availability

[Availability](/docs/shared/core-concepts/availability) answers whether a resource can be booked for a selected date. It can be used for historical dates, today, and future dates. It is calculated from opening hours, existing bookings, configured rules, and, where relevant, an offer's availability. Use it for planning, reviewing historical bookings, and understanding resource utilization.

## Tag

A tag is an organization-level label used to classify and find resources. For example, a resource might be tagged by equipment, room type, or accessibility. Tags describe the resource itself and support filtering and discovery.

## Zone

A zone is a reusable grouping for an area or purpose within an organization’s locations. Resources can be associated with more than one zone, allowing the same inventory to be grouped in different useful ways. Zones describe location structure; tags describe resource characteristics.

## Analytics

Analytics explains what has happened across bookings and resource activity. It helps an operator understand usage, demand, and operational patterns over time. Analytics is different from availability: analytics reports on history, while availability supports the next booking decision.

## Product and offer context

[Products](/docs/shared/marketplace/products) are the commercial layer of [Skedular Spaces](/docs/spaces). They define what customers purchase. Booking Groups determine which resources fulfil a purchase, and a product may contain multiple offers. Each offer can define pricing, billing rules, payment methods, and cancellation policies. [Skedular Host](/docs/host) exposes a simpler place-first listing configuration instead.

> **Spaces only:** Products and Booking Groups belong to the Skedular Spaces marketplace model.

## Booking Groups

[Booking Groups](/docs/spaces/core-features/products-and-pricing) connect a Spaces product or offer to the resources that can fulfil it. They are different from ordinary resource tags: resource tags describe inventory, while Booking Groups support commercial allocation.

## Subscriptions and commerce

[Subscriptions](/docs/shared/marketplace/subscriptions) manage recurring customer access separately from one-time bookings. The associated payments, invoices, billing and payouts, Stripe Connect, bank accounts, and [Xero integration](/docs/spaces/settings/xero-accounting) belong to the Skedular Spaces commerce workflow.

## How the concepts work together

The normal flow is: an **organization** owns a **location**; the location contains **resources**; resources have **tags**, belong to **zones**, and appear on a **floor plan**; **availability** determines when they can be used; a **user** or customer creates a **booking**; and **analytics** records the resulting activity. In [Skedular Spaces](/docs/spaces), a **product** adds the commercial rules around that booking.

## Product boundaries

<div class="documentation-product-cards">
  <div class="documentation-product-card"><h3>👥 <a href="/docs/teams">Skedular Teams</a></h3><ul><li>Private workplaces</li><li>Employees and teams</li><li>Internal bookings</li></ul><a href="/docs/teams">Read Teams documentation →</a></div>
  <div class="documentation-product-card"><h3>🛒 <a href="/docs/spaces">Skedular Spaces</a></h3><ul><li>Marketplace operations</li><li>Products and customers</li><li>Payments and commerce</li></ul><a href="/docs/spaces">Read Spaces documentation →</a></div>
  <div class="documentation-product-card"><h3>🏠 <a href="/docs/host">Skedular Host</a></h3><ul><li>Place-first listings</li><li>Availability and pricing</li><li>Simple rentals</li></ul><a href="/docs/host">Read Host documentation →</a></div>
</div>

For a guided setup, choose [Skedular Teams](/docs/teams), [Skedular Spaces](/docs/spaces), or [Skedular Host](/docs/host).

## Related concepts

- [Organizations](/docs/shared/core-concepts/organizations)
- [Locations](/docs/shared/core-concepts/locations)
- [Resources](/docs/shared/core-concepts/resources)
- [Bookings](/docs/shared/core-concepts/bookings)
- [Availability](/docs/shared/core-concepts/availability)
- [Products](/docs/shared/marketplace/products)
- [Subscriptions](/docs/shared/marketplace/subscriptions)
- [Analytics](/docs/shared/insights/analytics)
