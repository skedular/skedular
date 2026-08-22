---
id: shared-resources
title: "Resources"
description: "Resources are the bookable assets that people reserve through Skedular."
product: shared
category: core-concepts
slug: resources
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/resource.md
  - doc-resources/location.md
  - doc-resources/booking.md
  - doc-resources/availability.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-locations
  - shared-bookings
  - shared-availability
  - shared-floor-plans
  - shared-tags
  - shared-zones
  - shared-products
updatedAt: 2026-08-07
---

## Overview

A resource is anything that can be booked in Skedular. Resources are the physical assets that users or customers reserve, such as desks, meeting rooms, offices, parking spaces, studios, event spaces, equipment, or other bookable assets.

<div class="documentation-concept-support"><strong>Supported in</strong><span>✅ Skedular Teams</span><span>✅ Skedular Spaces</span><span>✅ Skedular Host</span></div>

<aside class="documentation-callout" aria-label="Core rule"><strong>Core Rule</strong><p>Every resource belongs to exactly one location and represents a bookable asset within that location.</p></aside>

## How Resources Work

Resources are created inside a [Location](/docs/shared/core-concepts/locations). They become available according to the location's opening hours and any resource-specific schedule. [Bookings](/docs/shared/core-concepts/bookings) reserve resources, and the availability view updates as bookings are confirmed. In Skedular Spaces, products can expose resources through Booking Groups. Skedular Host manages the underlying resource automatically behind its place-first workflow.

## Why Resources Exist

Resources represent the things an organization makes available for booking. Once a resource is created, Skedular prepares its availability schedule so users can reserve it according to opening hours and booking rules.

## Where This Concept Fits

<div class="documentation-organization-context" aria-label="Resource relationships"><div class="documentation-context-root">🏢 <a href="/docs/shared/core-concepts/organizations">Organization</a><div class="documentation-context-child"><a href="/docs/shared/core-concepts/locations">📍 Location</a><div class="documentation-context-child"><span>🪑 Resource</span><div class="documentation-context-child"><a href="/docs/shared/core-concepts/bookings">📅 Bookings</a><div class="documentation-context-child"><a href="/docs/shared/core-concepts/availability">⏱ Availability</a></div></div><a href="/docs/shared/administration/organization-settings">🕒 Opening Hours</a><a href="/docs/shared/core-concepts/tags">🏷 Tags</a><a href="/docs/shared/core-concepts/zones">🔲 Zones</a><a href="/docs/shared/core-concepts/floor-plans">🗺 Floor Plans</a></div></div></div></div>

## Product Support

- **Skedular Teams:** Resources represent private workplace assets such as desks, meeting rooms, and parking spaces.
- **Skedular Spaces:** Resources are connected to marketplace products using Booking Groups. Customers browse products, and Skedular allocates suitable resources based on availability.
- **Skedular Host:** A resource is created automatically when a host creates a place. Hosts manage a simplified place workflow while the booking engine uses the resource behind the scenes.

## Resource Types and Properties

A resource represents any physical asset that people can reserve. Common examples include desks, meeting rooms, offices, parking spaces, studios, event spaces, and equipment. Skedular currently supports the types Desk, Room, Parking, and Other. Each resource also has a name, capacity, location, opening hours, tags, zones, and optional color.

Capacity describes how many people or units a resource can accommodate. Set it accurately so users can choose a resource that fits their needs.

## What Belongs to a Resource

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>Reservations made against one or more resources.</small></a><a href="/docs/shared/core-concepts/availability"><strong>⏱ Availability</strong><small>The times when the resource can be reserved.</small></a><a href="/docs/shared/administration/organization-settings"><strong>🕒 Opening Hours</strong><small>The default schedule used to calculate availability.</small></a><a href="/docs/shared/core-concepts/tags"><strong>🏷 Tags</strong><small>Characteristics used for search and filtering.</small></a><a href="/docs/shared/core-concepts/zones"><strong>🔲 Zones</strong><small>Physical or logical areas that group resources.</small></a><a href="/docs/shared/core-concepts/floor-plans"><strong>🗺 Floor Plan Placement</strong><small>Where the resource appears in a visual layout.</small></a><div><strong>🏷 Booking Groups</strong><small>Commercial allocation rules in Skedular Spaces. Dedicated documentation is coming soon.</small></div></div>

## Typical Workflow

<div class="documentation-concept-workflow"><span><b>1</b>Create Location</span><span><b>2</b>Create Resource</span><span><b>3</b>Assign Tags</span><span><b>4</b>Assign Zones</span><span><b>5</b>Place on Floor Plan</span><span><b>6</b>Configure Availability</span><span><b>7</b>Accept Bookings</span></div>

## Booking Behavior

Resources are booked through the [Skedular booking engine](/docs/shared/core-concepts/bookings). A booking can reserve one or more resources. A confirmed booking makes each selected resource unavailable for the selected period, prevents overlapping bookings, and updates availability. Bookings use 15-minute increments and can be made up to one year in advance.

For an eligible Skedular Spaces marketplace Booking, a customer or authorized operator can select replacement Resources while changing the Booking. Every replacement must be currently available and eligible for the originally purchased Product, and the selection cannot exceed the Resource quantity included in that purchase. Skedular reserves the complete replacement set together, so it never leaves a Booking with only part of its required Resources. A replacement cannot switch the Product, Price, or purchased quantity. Skedular Host does not show this picker because it manages the underlying Resource for the whole place automatically.

## Opening Hours and Availability

By default, a resource inherits the opening hours of its location. A resource can have its own opening hours when it needs a different schedule, such as a meeting room with a separate entrance or equipment available only during staffed hours.

## Managing Resources

Resources are always managed within a location. Open the organization, select a location, and open Resources to create, edit, or delete them. Resources can be created individually or in bulk. Removing a resource removes it from future availability; historical booking information may be retained for reporting and auditing.

## Best Practices

- Use descriptive names that match the physical workspace.
- Choose the correct resource type and capacity.
- Apply tags and zones consistently.
- Keep floor plans synchronized with the physical layout.
- Configure availability before accepting bookings.
- Use resource-specific opening hours only when necessary.

## Things to Know

- Every resource belongs to exactly one location.
- Every resource can receive bookings.
- Resources support 15-minute booking intervals.
- Resources can be booked up to one year in advance.
- Resources inherit location opening hours by default.
- Resources can override opening hours with their own schedule.
- Resources may have zero, one, or many tags and zones.
- Skedular Host creates a resource automatically for a place.

## Frequently Asked Questions

### What is a resource?

A resource is bookable inventory such as a desk, room, parking space, office, or piece of equipment.

### Can a resource belong to multiple locations?

No. Every resource belongs to exactly one location.

### Can I move a resource between locations?

Resources are managed within a location. Move one only after checking its existing bookings and availability rules.

### What happens if I delete a resource?

Deleting a resource removes it from future availability. Existing booking information may remain available for reporting and auditing.

### Can I temporarily disable a resource?

Use the resource's availability or opening-hours settings to stop accepting bookings while keeping the resource configuration available.

### Can multiple people book the same resource?

No. Confirmed bookings prevent overlapping reservations for the same resource.

### Can one booking include multiple resources?

Yes. A booking can be made against one or more resources.

### Why do I not see resources in Skedular Host?

Host creates and manages the resource behind the place-first workflow, so hosts may work with the place rather than the underlying resource directly.

## Continue Learning

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>Reservations made against resources.</small></a><a href="/docs/shared/core-concepts/availability"><strong>⏱ Availability</strong><small>When resources can be used.</small></a><a href="/docs/shared/core-concepts/floor-plans"><strong>🗺 Floor Plans</strong><small>Visual layouts for finding resources.</small></a><a href="/docs/shared/core-concepts/tags"><strong>🏷 Tags and Zones</strong><small>Ways to organize and find inventory.</small></a></div>

## Related Concepts

- [Locations](/docs/shared/core-concepts/locations)
- [Bookings](/docs/shared/core-concepts/bookings)
- [Availability](/docs/shared/core-concepts/availability)
- [Floor Plans](/docs/shared/core-concepts/floor-plans)
- [Tags](/docs/shared/core-concepts/tags)
- [Zones](/docs/shared/core-concepts/zones)
- [Products](/docs/shared/marketplace/products)
