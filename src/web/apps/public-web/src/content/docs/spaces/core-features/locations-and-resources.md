---
id: spaces-locations-resources
title: "Locations and resources"
description: "Prepare the physical Locations and bookable Resources that form the foundation of your workspace offering."
product: spaces
category: workspace-setup
slug: locations-and-resources
articleKind: guide
publicationState: published
evidenceRefs:
  - doc-resources/location.md
  - doc-resources/resource.md
  - doc-resources/availability.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-16
---

Prepare the physical workspace that customers will later discover through your Products. In Spaces, a Location contains the Resources that customers can book, while Location and Resource configuration helps determine when those Resources are available.

## Prepare your Locations

Use **Add Location** to add each physical place your Organization manages. Keep the Location name, address and map details accurate, and set its **Time Zone** and **Opening Hours** before you create customer-facing Products. For the shared definitions, see [Locations](/docs/shared/core-concepts/locations).

## Add the Resources customers can use

Use **Add Resource** from a Location to create the bookable entities in that place. Spaces supports Resource Types such as desks, rooms, parking, and other configured types. Set a clear name, choose the **Resource Type**, and provide **Capacity** where it applies. A Resource can also have its own **Opening Hours** override when it needs a schedule different from the Location.

Location Opening Hours, a Resource-specific Opening Hours override, and existing Bookings can change when a Resource can be offered. Capacity describes how many people or units a Resource can support; keep it accurate separately from the schedules that determine bookable time. See [Resources](/docs/shared/core-concepts/resources) and [Availability](/docs/shared/core-concepts/availability) for the shared model.

## Prepare Resources for Products

After Resources exist, assign **Booking Groups** where the Product workflow uses them to select eligible Resources. Booking Groups are separate from ordinary Tags and are used by Products to determine which Resources can be included in an offering. Products then define what customers can purchase and book; continue with [Products and pricing](/docs/spaces/products-and-marketplace/products-and-pricing) for that workflow.

## Keep the workspace configuration accurate

Keep Locations, Resources, Capacity, and Booking Groups accurate as the physical workspace changes, and review Opening Hours whenever schedules change. Changes to the underlying Resource setup or schedules can affect Availability and what customers can book through a Product.

If you need additional organization, continue to [Zones and floor plans](/docs/spaces/workspace-setup/zones-and-floor-plans). Both are optional. When the physical workspace is ready, continue to [Products and marketplace](/docs/spaces/products-and-marketplace).
