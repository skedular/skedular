---
id: shared-floor-plans
title: "Floor Plans"
description: "Floor Plans provide a visual representation of a location so users can find and book resources from an interactive layout."
product: shared
category: core-concepts
slug: floor-plans
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/floor-paln.md
  - doc-resources/location.md
  - doc-resources/resource.md
  - doc-resources/booking.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-locations
  - shared-resources
  - shared-bookings
  - shared-availability
updatedAt: 2026-07-15
---

## Overview

A Floor Plan is a visual layout managed inside a [Location](/docs/shared/core-concepts/locations). It shows where [Resources](/docs/shared/core-concepts/resources) are positioned, helping people understand the physical space, check availability, and choose a resource before creating a [Booking](/docs/shared/core-concepts/bookings).

<div class="documentation-concept-support"><strong>Supported in</strong><span>✅ Skedular Teams</span><span>✅ Skedular Spaces</span><span>✅ Skedular Host</span></div>

<aside class="documentation-callout" aria-label="Core rule"><strong>Core Rule</strong><p>Every Floor Plan belongs to one Location and visually represents the Resources within that Location.</p></aside>

## How Floor Plans Work

<div class="documentation-concept-workflow"><span><b>1</b>Create Location</span><span><b>2</b>Upload Floor Plan</span><span><b>3</b>Place Resources</span><span><b>4</b>Save Layout</span><span><b>5</b>View Floor Plan</span><span><b>6</b>Book Resources</span></div>

The uploaded layout provides the visual canvas. Resource positions are stored separately, so moving a resource on the plan changes its visual position without changing the resource or its existing bookings.

## Where This Concept Fits

<div class="documentation-organization-context" aria-label="Floor Plan relationships"><div class="documentation-context-root">🏢 <a href="/docs/shared/core-concepts/organizations">Organization</a><div class="documentation-context-child"><a href="/docs/shared/core-concepts/locations">📍 Location</a><div class="documentation-context-child"><span>🗺 Floor Plan</span><div class="documentation-context-child"><a href="/docs/shared/core-concepts/resources">🪑 Resource positions</a><div class="documentation-context-child"><a href="/docs/shared/core-concepts/bookings">📅 Bookings</a></div></div></div></div></div><div class="documentation-context-tree"><a href="/docs/shared/core-concepts/availability">⏱ Availability</a></div></div>

## Floor Plan Ownership

Floor Plans belong to a single Location and cannot exist independently. A Location can contain multiple Floor Plans, such as separate levels, buildings, or areas. Floor Plans are optional; a Location can operate without one.

## Resource Placement

Floor Plans display Resources but do not own them. Administrators position, move, or remove resource markers on the layout while the underlying resource remains part of its Location. A resource can be placed on one Floor Plan at a time, and its position can be updated independently of its booking history.

## Booking from a Floor Plan

Users can use the layout to find a resource and see its availability for a selected date. Selecting a resource leads into the normal booking workflow. The resulting booking reserves the [Resource](/docs/shared/core-concepts/resources), not the Floor Plan itself.

## Editing Floor Plans

Authorized users can create Floor Plans, upload their images, rename them, position resources, move resource markers, remove resources from a layout, and delete Floor Plans. Updating a layout changes the visual reference for the Location; it does not rewrite existing bookings.

## Product Differences

<div class="documentation-concept-grid"><div><strong>🧑‍💼 Skedular Teams</strong><small>Supports internal workplace navigation, desk selection, and team booking workflows.</small></div><div><strong>🛒 Skedular Spaces</strong><small>Supports visual resource browsing for marketplace locations and customer bookings.</small></div><div><strong>🏠 Skedular Host</strong><small>Supports Floor Plans for host locations and the simplified place-first booking workflow.</small></div></div>

## What Belongs to a Floor Plan

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/locations"><strong>📍 Location</strong><small>The parent that owns the Floor Plan.</small></a><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>Assets displayed at visual positions.</small></a><a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>Reservations made against displayed resources.</small></a><a href="/docs/shared/core-concepts/availability"><strong>⏱ Availability</strong><small>The state shown for each resource.</small></a></div>

## Best Practices

- Use clear, meaningful names for each Floor Plan.
- Upload a high-resolution layout that matches the physical space.
- Position Resources accurately.
- Keep layouts synchronized with physical changes.
- Remove outdated Floor Plans and resource positions.

## Things to Know

- Every Floor Plan belongs to one Location.
- Locations can contain multiple Floor Plans.
- Floor Plans are optional.
- Floor Plans display Resources but do not own them.
- Bookings reserve Resources, not Floor Plans.
- Resource positions can be updated independently of bookings.
- Availability can be shown for resources on a selected Floor Plan.

## Frequently Asked Questions

### What is a Floor Plan?

A Floor Plan is an interactive visual layout that places a Location's Resources in their physical context.

### Can a Location have multiple Floor Plans?

Yes. Locations can use multiple Floor Plans for levels, buildings, or distinct areas.

### Can one Resource appear on multiple Floor Plans?

No. A resource is assigned to one Floor Plan at a time. Remove the existing placement before assigning it to another.

### Can I replace a Floor Plan image?

Authorized users can update the uploaded layout used by a Floor Plan.

### Can users book directly from a Floor Plan?

The Floor Plan helps users select a resource. The reservation then follows the normal Booking workflow.

### What happens if I move a Resource?

Moving a resource updates its visual position on the Floor Plan. Existing bookings remain associated with the resource.

## Continue Learning

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/locations"><strong>📍 Locations</strong><small>The places that own Floor Plans.</small></a><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>The assets placed on a layout.</small></a><a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>Reservations made against resources.</small></a><a href="/docs/shared/core-concepts/availability"><strong>⏱ Availability</strong><small>When displayed resources can be booked.</small></a></div>

## Related Concepts

- [Locations](/docs/shared/core-concepts/locations)
- [Resources](/docs/shared/core-concepts/resources)
- [Bookings](/docs/shared/core-concepts/bookings)
- [Availability](/docs/shared/core-concepts/availability)
