---
id: shared-organizations
title: "Organizations"
description: "Organizations are the ownership and access boundary for Skedular operations."
product: shared
category: core-concepts
slug: organizations
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/organization.md
  - doc-resources/user.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-locations
  - shared-resources
  - shared-users
  - shared-bookings
updatedAt: 2026-07-15
---

## Overview

An organization is the root of the Skedular domain model. It owns the people, locations, resources, bookings, settings, and reporting for one operation. In commercial workflows, products, subscriptions, payments, and analytics also belong to the organization that operates them.

<div class="documentation-concept-support"><strong>Supported in</strong><span>✅ Skedular Teams</span><span>✅ Skedular Spaces</span><span>✅ Skedular Host</span></div>

<aside class="documentation-callout" aria-label="Core rule"><strong>Core Rule</strong><p>Every location, resource, booking, product, payment, subscription, report, and setting belongs to exactly one organization.</p></aside>

## Where This Concept Fits

<div class="documentation-organization-context" aria-label="Organization relationships"><div class="documentation-context-root">🏢 <a href="/docs/shared/core-concepts/organizations">Organization</a></div><div class="documentation-context-tree"><div><a href="/docs/shared/core-concepts/locations">📍 Locations</a><div class="documentation-context-child"><a href="/docs/shared/core-concepts/resources">🪑 Resources</a></div></div><a href="/docs/shared/core-concepts/users">👤 Users</a><a href="/docs/shared/core-concepts/teams">👥 Teams</a><a href="/docs/shared/core-concepts/bookings">📅 Bookings</a><a href="/docs/shared/marketplace/products">🛒 Products</a><a href="/docs/shared/commerce/payments">💳 Payments</a><a href="/docs/shared/insights/analytics">📊 Analytics</a></div></div>

## Why Organizations Exist

Organizations give Skedular a clear ownership and access boundary. They determine which people can manage a setup, which locations and resources appear together, and where booking and operational data is reported.

## What Belongs to an Organization

<div class="documentation-concept-grid">
  <a href="/docs/shared/core-concepts/locations"><strong>📍 Locations</strong><small>Physical places operated by the organization.</small></a>
  <a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>Bookable inventory inside a location.</small></a>
  <a href="/docs/shared/core-concepts/users"><strong>👤 Users</strong><small>People with access or booking participation.</small></a>
  <a href="/docs/shared/core-concepts/teams"><strong>👥 Teams</strong><small>Private workplace groups in Skedular Teams.</small></a>
  <a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>Reservations made against resources.</small></a>
  <a href="/docs/shared/marketplace/products"><strong>🛒 Products</strong><small>Commercial offers in Skedular Spaces.</small></a>
  <a href="/docs/shared/commerce/payments"><strong>💳 Payments</strong><small>Commercial payment activity for the operation.</small></a>
  <a href="/docs/shared/insights/analytics"><strong>📊 Analytics</strong><small>Reporting on usage and activity.</small></a>
</div>

## Organization Types

<div class="documentation-product-cards"><div class="documentation-product-card"><h3>👥 <a href="/docs/teams">Skedular Teams</a></h3><p>Private workplace management</p><ul><li>Employees</li><li>Teams</li><li>Internal bookings</li></ul></div><div class="documentation-product-card"><h3>🛒 <a href="/docs/spaces">Skedular Spaces</a></h3><p>Commercial workspace operations</p><ul><li>Marketplace</li><li>Products and customers</li><li>Payments</li></ul></div><div class="documentation-product-card"><h3>🏠 <a href="/docs/host">Skedular Host</a></h3><p>Simple place-first rentals</p><ul><li>Listings</li><li>Availability</li><li>Renters</li></ul></div></div>

## Roles and Permissions

Users belong to an organization and receive access based on their role. Keep product-specific rules in the relevant guide.

| Role          | Typical responsibility              |
| ------------- | ----------------------------------- |
| Owner         | Full organization management        |
| Administrator | Day-to-day setup and administration |
| Member        | Booking and standard access         |

Read [Organize your people in Skedular Teams](/docs/teams/workplace-setup/organize-your-people), [Customers in the Skedular domain model](/docs/shared/marketplace/customers), and [Host bookings and renters](/docs/host/bookings/bookings-and-renters) for product-specific behavior.

## Typical Setup Workflow

<div class="documentation-concept-workflow"><span><b>1</b>Create the organization</span><span><b>2</b>Invite users</span><span><b>3</b>Add locations</span><span><b>4</b>Create resources</span><span><b>5</b>Start booking</span></div>

Begin with the organization that should own the operation. Add only the people and locations you need for the first rollout, then verify resource availability and a test booking before expanding the setup.

## Best Practices

- Use one organization for one clear operational boundary.
- Create a separate organization when ownership, billing, or reporting must remain independent.
- Avoid combining unrelated operations in one organization.
- Align permissions with each user's responsibilities.
- Confirm the organization before changing locations, resources, or booking rules.
- Give users only the access they need for their role.
- Keep commercial configuration in the organization that owns the marketplace operation.
- Use the product-specific documentation when a workflow differs between Teams, Spaces, and Host.

## Things to Know

- An organization can own multiple locations.
- Every location belongs to exactly one organization.
- A user can belong to multiple organizations.
- Resources cannot be shared across organizations.
- Billing, payments, analytics, and reporting are isolated per organization.
- Organizations are the primary security boundary within Skedular.

## Frequently Asked Questions

### Can I belong to multiple organizations?

Yes. A user can participate in multiple organizations, with access determined separately in each one.

### Can one organization have multiple locations?

Yes. Each location still belongs to exactly one organization.

### Can organizations share resources?

No. A resource belongs to one location, and that location belongs to one organization.

## Continue Learning

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/locations"><strong>📍 Locations</strong><small>The physical places an organization operates.</small></a><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>The bookable inventory inside a location.</small></a><a href="/docs/shared/core-concepts/users"><strong>👤 Users</strong><small>The people who access and use the organization.</small></a><a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>The reservations made against resources.</small></a></div>

## Related Concepts

- [Locations](/docs/shared/core-concepts/locations)
- [Resources](/docs/shared/core-concepts/resources)
- [Users](/docs/shared/core-concepts/users)
- [Bookings](/docs/shared/core-concepts/bookings)
