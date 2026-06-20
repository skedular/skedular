---
id: shared-zones
title: "Zones"
description: "Organization-level groupings used to organize related Resources by area, function, or purpose."
product: shared
category: core-concepts
slug: zones
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/zone.md
  - doc-resources/resource.md
  - doc-resources/tag.md
  - doc-resources/location.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-organizations
  - shared-locations
  - shared-resources
  - shared-tags
updatedAt: 2026-07-15
---

## Overview

A Zone is an Organization-level grouping used to organize related Resources by area, function, or purpose. Zones help teams describe how Resources are grouped without changing the Resource's Location or ownership. Unlike [Tags](/docs/shared/core-concepts/tags), which describe what a Resource is or has, Zones describe a defined grouping.

<div class="documentation-concept-support"><strong>Supported in</strong><span>✅ Skedular Teams</span><span>✅ Skedular Spaces</span><span>✅ Skedular Host</span></div>

<aside class="documentation-callout" aria-label="Core rule"><strong>Core Rule</strong><p>Zones belong to an Organization, and a Resource can belong to zero, one, or multiple Zones.</p></aside>

## Why Zones Exist

Zones let organizations group Resources using their own physical or operational vocabulary. Examples include Level 1, North Wing, Engineering Area, Quiet Zone, and Collaboration Area. These are organization-defined Zones, not a fixed set of predefined values.

## How Zones Work

<div class="documentation-concept-workflow"><span><b>1</b>Create Zone</span><span><b>2</b>Assign Resources</span><span><b>3</b>Group Resources</span><span><b>4</b>Filter Resources by Zone</span><span><b>5</b>Update Membership</span></div>

Zones are available across the Organization. A Zone can be assigned to many Resources, and a Resource can belong to multiple Zones or none. Zone membership does not move a Resource or change its Location.

## Where This Concept Fits

<div class="documentation-organization-context" aria-label="Zone relationships"><div class="documentation-context-root">🏢 <a href="/docs/shared/core-concepts/organizations">Organization</a><div class="documentation-context-child"><span>🔲 Zones ↔ Resources</span><a href="/docs/shared/core-concepts/locations">📍 Locations</a></div></div><div class="documentation-context-tree"><a href="/docs/shared/core-concepts/resources">🪑 Resources belong to Locations independently</a></div></div>

## Zone Ownership

A Zone belongs to one Organization and is available only within that Organization. It is not owned by a Location. Resources from different Locations in the same Organization can use the same Zone when that grouping is useful.

## Zone Membership

A Resource can belong to zero, one, or multiple Zones. A Zone can be assigned to many Resources. This many-to-many relationship lets one Zone group a set of Resources while each Resource participates in other groupings as needed.

## Zones and Locations

Locations own Resources, while the Organization owns Zones. A Zone does not change a Resource's Location, and the Zone itself is not restricted to one Location. Organizations can use Zones for a floor, wing, department, or operating area that may span or relate to more than one Location. A Zone name such as “Level 1” describes a grouping; it does not create a Location hierarchy.

## Creating and Managing Zones

Authorized users manage Zones from the Organization administration experience. They can create Zones, name or rename them, assign Resources, remove assignments, and delete Zones. Renaming a Zone updates its displayed name wherever it is assigned. Deleting a Zone removes its assignments from Resources; it does not delete the Resources.

## Assigning Resources to Zones

Zone membership is assigned from Resource management and can also be selected while creating or editing Resources. Assigning a Resource to a Zone changes its grouping metadata only. The Resource remains in its existing Location and keeps its existing booking and availability behavior.

## Zones vs Tags

Use [Tags](/docs/shared/core-concepts/tags) to describe what a Resource is or has. Use Zones to describe where Resources are grouped or how an area is organized.

Examples of Tags include Standing Desk, Dual Monitor, Accessible, and Near Window. Examples of Zones include Level 1, North Wing, Engineering Area, and Collaboration Area. A Resource can have both because Tags describe characteristics and Zones define group membership. A phrase such as Quiet Zone can be either a Tag or a Zone; choose a Zone when it represents a named grouping that Resources participate in.

## What Zones Do Not Do

- Zones do not own Resources.
- Zones do not change a Resource's Location.
- Zones do not directly change Resource Availability or Booking rules.
- Zones do not create Bookings.
- Zones are not the same as Tags.

## Product Differences

<div class="documentation-concept-grid"><div><strong>🧑‍💼 Skedular Teams</strong><small>Administrators use Zones to organize workplace Resources by floor, department, or area.</small></div><div><strong>🛒 Skedular Spaces</strong><small>Operators use Zones to organize related Resources across marketplace Locations.</small></div><div><strong>🏠 Skedular Host</strong><small>Hosts can manage Zones in administration and assign them while managing Resources.</small></div></div>

## How Zones Connect

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/organizations"><strong>🏢 Organization</strong><small>Owns the Zones available within its scope.</small></a><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>Can belong to zero, one, or multiple Zones.</small></a><a href="/docs/shared/core-concepts/locations"><strong>📍 Locations</strong><small>Own Resources independently of Zone membership.</small></a><a href="/docs/shared/core-concepts/tags"><strong>🏷 Tags</strong><small>Provide a separate characteristic-based classification model.</small></a></div>

## Best Practices

- Use short, descriptive Zone names.
- Use consistent naming across the Organization.
- Avoid duplicate Zones with overlapping meanings.
- Use Zones for groups or areas rather than individual Resources.
- Review unused Zones periodically and remove them when no longer needed.

## Things to Know

- Zones are created at the Organization level.
- Resources from multiple Locations can use the same Zone.
- Zones are not owned by Locations.
- A Resource may belong to multiple Zones or none.
- The same Zone can be assigned to multiple Resources.
- Zone membership does not change a Resource's Location.
- Zone names do not create a Location hierarchy.
- Renaming a Zone updates its displayed name wherever it is assigned.
- Deleting a Zone removes its assignments without deleting Resources.
- Zones do not affect Resource Availability or Booking rules.
- Zones and Tags are different concepts.

## Frequently Asked Questions

### What is a Zone?

A Zone is an Organization-defined grouping for related Resources.

### Can a Resource belong to multiple Zones?

Yes. A Resource can belong to zero, one, or multiple Zones.

### Can one Zone group Resources from multiple Locations?

Yes. Zones are Organization-level, so Resources from different Locations in the same Organization can share a Zone.

### Does changing a Resource's Zone change its Location?

No. Zone membership changes only how the Resource is grouped. The Resource remains in its existing Location.

### Are Zones and Tags the same?

No. Zones group Resources, while Tags describe Resource characteristics.

### Do Zones affect Availability?

No. Zones organize Resources but do not change Availability or Booking rules.

### What happens when I rename or delete a Zone?

Renaming updates the displayed name wherever the Zone is assigned. Deleting a Zone removes its assignments, while Resources remain intact in their Locations.

## Continue Learning

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/organizations"><strong>🏢 Organizations</strong><small>Where Zones are owned.</small></a><a href="/docs/shared/core-concepts/locations"><strong>📍 Locations</strong><small>Where Resources belong.</small></a><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>The assets grouped by Zones.</small></a><a href="/docs/shared/core-concepts/tags"><strong>🏷 Tags</strong><small>A separate classification model.</small></a></div>

## Related Concepts

- [Organizations](/docs/shared/core-concepts/organizations)
- [Locations](/docs/shared/core-concepts/locations)
- [Resources](/docs/shared/core-concepts/resources)
- [Tags](/docs/shared/core-concepts/tags)
