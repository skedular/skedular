---
id: shared-locations
title: "Locations"
description: "Locations are the physical places where Skedular resources exist and bookings take place."
product: shared
category: core-concepts
slug: locations
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/location.md
  - doc-resources/resource.md
  - doc-resources/availability.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-organizations
  - shared-resources
  - shared-floor-plans
  - shared-availability
  - shared-bookings
updatedAt: 2026-07-15
---

## Overview

A location represents a physical place where bookable resources exist. Locations are available in [Skedular Teams](/docs/teams), [Skedular Spaces](/docs/spaces), and [Skedular Host](/docs/host).

<div class="documentation-concept-support"><strong>Supported in</strong><span>✅ Skedular Teams</span><span>✅ Skedular Spaces</span><span>✅ Skedular Host</span></div>

<aside class="documentation-callout" aria-label="Core rule"><strong>Core Rule</strong><p>Every resource belongs to exactly one location. Resources cannot exist without a location.</p></aside>

## Where This Concept Fits

<div class="documentation-organization-context" aria-label="Location relationships"><div class="documentation-context-root">🏢 <a href="/docs/shared/core-concepts/organizations">Organization</a><div class="documentation-context-child"><span>📍 Location</span></div></div><div class="documentation-context-tree"><div><a href="/docs/shared/core-concepts/resources">🪑 Resources</a><div class="documentation-context-child"><a href="/docs/shared/core-concepts/bookings">📅 Bookings</a></div></div><a href="/docs/shared/core-concepts/floor-plans">🗺 Floor Plans</a><a href="/docs/shared/administration/organization-settings">🕒 Opening Hours</a><a href="/docs/shared/core-concepts/locations">📍 Address and Time Zone</a><a href="/docs/shared/core-concepts/tags">🏷 Tags and Zones</a><a href="/docs/shared/core-concepts/locations">📶 Amenities</a></div></div>

## Why Locations Exist

Locations provide the physical context for resources. They let an organization manage offices, coworking spaces, buildings, floors, studios, meeting venues, warehouses, event spaces, and other places that contain bookable inventory.

## What Belongs to a Location

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>Desks, rooms, equipment, and other bookable inventory.</small></a><a href="/docs/shared/core-concepts/floor-plans"><strong>🗺 Floor Plans</strong><small>Visual layouts of the physical space.</small></a><a href="/docs/shared/administration/organization-settings"><strong>🕒 Opening Hours</strong><small>When resources at the location can be booked.</small></a><div><strong>📍 Address</strong><small>Physical details that help people find the venue.</small></div><div><strong>📶 Amenities</strong><small>Useful details that describe the place.</small></div><div><strong>🔒 Private Notes</strong><small>Operational information for authorized users.</small></div></div>

## How Locations Work

Each location belongs to one organization and operates with its own address, time zone, opening hours, floor plans, and resource configuration. An organization can contain multiple locations, and locations can have different operating hours and local time zones.

## Typical Setup Workflow

<div class="documentation-concept-workflow"><span><b>1</b>Create Location</span><span><b>2</b>Configure Address</span><span><b>3</b>Set Time Zone and Opening Hours</span><span><b>4</b>Upload Floor Plans</span><span><b>5</b>Add Resources</span><span><b>6</b>Start Accepting Bookings</span></div>

## Best Practices

- Create one location for each physical site.
- Configure the correct local time zone.
- Keep opening hours accurate and current.
- Add floor plans for larger or multi-level buildings.
- Maintain an accurate address and location description.
- Keep private operational information current and separate from public details.

## Things to Know

- A location belongs to exactly one organization.
- Every resource belongs to exactly one location.
- An organization can have multiple locations.
- Locations can have different opening hours and time zones.
- Resources may override location opening hours where the product supports it.
- A location can contain multiple floor plans.

## Frequently Asked Questions

### Can an organization have multiple locations?

Yes. Each location operates independently while remaining part of the same organization.

### Can locations have different opening hours?

Yes. Opening hours are configured independently for each location.

### Can I move resources between locations?

Resources belong to one location at a time. Move them only when the resulting booking and availability rules remain correct.

### Can a location have multiple floor plans?

Yes. A location can use multiple floor plans for levels, buildings, or separate areas.

### Can locations have different time zones?

Yes. Each location uses its own local time zone for bookings, opening hours, and availability.

## Continue Learning

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>Bookable inventory inside a location.</small></a><a href="/docs/shared/core-concepts/floor-plans"><strong>🗺 Floor Plans</strong><small>Visual layouts that help people find resources.</small></a><a href="/docs/shared/core-concepts/availability"><strong>⏱ Availability</strong><small>When resources can be used.</small></a><a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>Reservations made against resources.</small></a></div>

## Related Concepts

- [Organizations](/docs/shared/core-concepts/organizations)
- [Resources](/docs/shared/core-concepts/resources)
- [Floor Plans](/docs/shared/core-concepts/floor-plans)
- [Availability](/docs/shared/core-concepts/availability)
- [Bookings](/docs/shared/core-concepts/bookings)
