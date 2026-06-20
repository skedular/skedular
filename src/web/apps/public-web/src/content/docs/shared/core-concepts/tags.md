---
id: shared-tags
title: "Tags"
description: "Organization-level labels used to classify and organize Resources by shared characteristics."
product: shared
category: core-concepts
slug: tags
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/tag.md
  - doc-resources/resource.md
  - doc-resources/zone.md
  - doc-resources/product-tag.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-organizations
  - shared-resources
  - shared-zones
updatedAt: 2026-07-15
---

## Overview

A Tag is an organization-level label used to classify and organize Resources. Tags describe what a Resource is or has without changing the Resource's Location, type, or identity.

<div class="documentation-concept-support"><strong>Supported in</strong><span>✅ Skedular Teams</span><span>✅ Skedular Spaces</span><span>✅ Skedular Host</span></div>

<aside class="documentation-callout" aria-label="Core rule"><strong>Core Rule</strong><p>Tags belong to an Organization, and a Resource can have zero, one, or multiple Tags.</p></aside>

## Why Tags Exist

Tags let an organization describe Resource characteristics using its own vocabulary. Examples include Standing Desk, Near Window, Quiet, Accessible, or Dual Monitor. These are organization-defined labels, not a fixed list of system Tags.

## How Tags Work

<div class="documentation-concept-workflow"><span><b>1</b>Create Tag</span><span><b>2</b>Assign to Resources</span><span><b>3</b>Classify Resources</span><span><b>4</b>Organize Resources</span><span><b>5</b>Update Assignments</span></div>

Tags are scoped to their Organization. A Tag can be assigned to multiple Resources, and a Resource can have multiple Tags or none. Adding or removing a Tag changes classification metadata, not the Resource itself.

## Where This Concept Fits

<div class="documentation-organization-context" aria-label="Tag relationships"><div class="documentation-context-root">🏢 <a href="/docs/shared/core-concepts/organizations">Organization</a><div class="documentation-context-child"><span>🏷 Tags ↔ Resources</span><a href="/docs/shared/core-concepts/resources">🪑 Resources</a></div></div><div class="documentation-context-tree"><a href="/docs/shared/core-concepts/zones">🔲 Zones ↔ Resources, as a separate grouping model</a></div></div>

## Tag Ownership

Tags belong to one Organization and are available to that Organization's Resources across its Locations. They are not shared across unrelated Organizations.

## Creating and Managing Tags

Authorized users manage Tags within the Organization. They can create Tags, rename them, assign them to Resources, remove assignments, and delete Tags. Renaming a Tag updates its displayed name wherever it is assigned. Deleting a Tag removes the Tag assignment from Resources; it does not delete the Resources.

## Assigning Tags to Resources

Resources can have zero, one, or multiple Tags. A Tag can be assigned to many Resources. Assignments are independent of Resource identity, so changing Tags does not move a Resource or change its booking behavior.

## Finding Resources with Tags

Tags are exposed as classification metadata in the Resource management workflows. Administrators can use the labels while managing Resources, but Tags do not provide a universal Resource search or Availability filter. Tags do not replace Availability or Booking rules.

## Tags vs Zones

Use Tags to describe what a Resource is or has. Use [Zones](/docs/shared/core-concepts/zones) to describe where Resources are grouped or how an area is organized.

Examples of Tags include Standing Desk, Dual Monitor, Accessible, and Near Window. Examples of Zones include Level 1, North Wing, Engineering Area, and Quiet Zone. A Resource can have both Tags and Zones because they answer different questions.

## Tags vs Product Tags

Resource Tags classify Resources for organization. Product Tags are a separate Skedular Spaces concept used to determine which Resources are eligible for allocation to marketplace Products. Do not use Resource Tags as a replacement for Product Tags.

<div class="documentation-concept-grid"><div><strong>🏷 Resource Tags</strong><small>Organization labels that describe Resource characteristics.</small></div><div><strong>🛒 Product Tags</strong><small>Marketplace allocation rules in Skedular Spaces. Dedicated documentation is coming soon.</small></div></div>

## Product Differences

<div class="documentation-concept-grid"><div><strong>🧑‍💼 Skedular Teams</strong><small>Tags help classify and organize workplace Resources for private scheduling.</small></div><div><strong>🛒 Skedular Spaces</strong><small>Resource Tags remain descriptive; Product Tags separately control marketplace allocation.</small></div><div><strong>🏠 Skedular Host</strong><small>Hosts can manage organization Tags and assign them to Resources from the Host administration experience.</small></div></div>

## How Tags Connect

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/organizations"><strong>🏢 Organization</strong><small>Owns the Tags available to its Resources.</small></a><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>Can be assigned zero, one, or multiple Tags.</small></a><a href="/docs/shared/core-concepts/zones"><strong>🔲 Zones</strong><small>Provide a different way to group Resources.</small></a><div><strong>🛒 Product Tags</strong><small>Control Resource eligibility for Products in Skedular Spaces. Dedicated documentation is coming soon.</small></div></div>

## Best Practices

- Use clear and descriptive Tag names.
- Create Tags around characteristics that are meaningful when organizing Resources.
- Keep naming conventions consistent.
- Avoid multiple Tags with the same meaning.
- Review unused Tags periodically.
- Use Zones for physical or organizational areas.
- Keep Resource Tags separate from Product Tags in Skedular Spaces.

## Things to Know

- Tags are created at the Organization level.
- All Locations in an Organization can use its Tags.
- Resources can have zero, one, or many Tags.
- The same Tag can be assigned to multiple Resources.
- Tags have no parent-child hierarchy.
- Tags do not affect Resource Availability or Booking rules.
- Renaming a Tag updates its displayed name wherever it is used.
- Deleting a Tag removes its assignments without deleting Resources.
- Resource Tags and Product Tags are different concepts.

## Frequently Asked Questions

### What is a Tag?

A Tag is an Organization-defined label used to classify a Resource.

### Can a Resource have multiple Tags?

Yes. A Resource can have zero, one, or multiple Tags.

### Can one Tag be assigned to multiple Resources?

Yes. Tags can be reused across the Resources in their Organization.

### Are Tags and Zones the same?

No. Tags describe Resource characteristics, while [Zones](/docs/shared/core-concepts/zones) describe physical or logical grouping.

### Are Resource Tags and Product Tags the same?

No. Product Tags are used by Skedular Spaces to control marketplace Resource allocation.

### What happens when I rename a Tag?

The new name appears everywhere that Tag is assigned.

### What happens when I delete a Tag?

The Tag and its assignments are removed. Resources remain intact.

## Continue Learning

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/organizations"><strong>🏢 Organizations</strong><small>Where Tags are owned.</small></a><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>The assets Tags describe.</small></a><a href="/docs/shared/core-concepts/zones"><strong>🔲 Zones</strong><small>A separate grouping concept.</small></a></div>

## Related Concepts

- [Organizations](/docs/shared/core-concepts/organizations)
- [Resources](/docs/shared/core-concepts/resources)
- [Zones](/docs/shared/core-concepts/zones)
