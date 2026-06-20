---
id: shared-analytics
title: "Analytics"
description: "Understand Booking activity, desk and room occupancy, and Resource availability across your Locations."
product: shared
category: insights
slug: analytics
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/analytics.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-bookings
  - shared-resources
  - shared-locations
  - shared-availability
updatedAt: 2026-07-15
---

<div class="documentation-concept-support"><strong>Available in</strong><span>✅ Skedular Teams</span><span>✅ Skedular Spaces</span><span>✅ Skedular Host</span></div>

## Overview

Analytics is the current capability within Insights. It reads Booking activity and presents daily information for Resources in their Location context. The current experience includes Booking Insights, desk and room occupancy percentages, and Resource Availability Insights over a selected date range.

Analytics is read-oriented. It does not create, change, or remove Bookings, Resources, Locations, or Availability rules.

<aside class="documentation-callout" aria-label="Analytics core rule"><strong>Core rule</strong><p>Bookings provide the activity; Analytics turns that activity into Booking, occupancy, and Resource availability insights.</p></aside>

## How Analytics works

<div class="documentation-organization-context" aria-label="Analytics relationships"><div class="documentation-context-root"><div class="documentation-context-tree"><span>📅 Bookings provide operational activity</span><span>🪑 Resources provide the measured entities</span><span>📍 Locations provide context for grouping Resources</span></div><div class="documentation-context-child"><span>📊 Analytics aggregates activity by day and Location</span><div class="documentation-context-child"><span>Booking, occupancy, and Resource availability insights</span></div></div></div></div>

Bookings, Resources, and Locations are independent inputs to Analytics. A selected date range determines which daily activity totals are shown; Analytics then presents the resulting counts and percentages for the selected Location within the current Organization.

## What Analytics measures

### Booking count

Booking Insights shows the daily total of Booking activity for a Location over the selected range.

### Occupancy percentage

Desk and room occupancy are daily count-based indicators calculated by comparing applicable Booking instances with the number of desks or rooms in the Location. The calculation is:

**Occupancy percentage = applicable desk or room Booking instances ÷ desk or room count × 100**

Days with zero desks or rooms are omitted from the percentage series rather than reported as 0%.

### Resource Availability Insights

Resource Availability Insights shows daily counts grouped by Resource type: available, unavailable, and booked. These are counts of Resource states, not booked minutes.

## How occupancy is calculated

Occupancy is a daily count-based measure. It does not calculate booked duration divided by opening minutes. For example, if a Location has 10 desks and 4 applicable desk Booking instances for the day, the desk occupancy percentage is 40%. Because the numerator counts Booking instances, multiple Bookings for the same desk can make the value exceed 100%; it is a count-based occupancy indicator rather than a physical-capacity percentage.

Desk and room percentages are calculated separately. A Location can therefore show different occupancy values for desks and rooms on the same day.

## Analytics and Availability

Availability answers whether a Resource can be booked at a particular time. Analytics summarizes Booking and Resource-state activity over a selected date range. A Resource can be available without being booked; that state appears in Resource Availability Insights and does not become a duration-based occupancy percentage.

## Which activity is included

Booking Insights and occupancy use the daily activity generated from non-deleted Bookings for the Location; cancelled Bookings are excluded when they are soft-deleted. Analytics does not currently provide a Booking-status filter, and occupancy is not based on Booking duration. Other Booking states are not broken out separately in the Analytics views.

Recurring bookings and Subscription-generated reservations contribute through their individual Booking instances. Analytics does not count a Subscription itself as one Booking.

## Analytics scope and date ranges

Analytics provides Location-level views within the current Organization. Users can select a date range; the Booking Insights and occupancy views default to a month, while Resource Availability Insights defaults to six months. Charts display daily values for the selected Location and range.

The interface does not expose a separate Resource-level analytics route or an export/report workflow. Location access and Analytics visibility are permission-controlled by the Organization.

## Reading Analytics

Booking count and occupancy are related count-based measures. Booking count reports daily Booking activity; occupancy divides applicable desk or room Booking instances by the corresponding desk or room count. Multiple Bookings for the same Resource can affect both values. Compare the relevant Resource type and date range before drawing conclusions.

## Filtering Analytics

The current user-facing controls are the selected date range and, for Resource Availability Insights, an optional Resource type view. Changing the date range updates the Analytics views for that range. Analytics does not provide filters for tags, zones, Customers, payment status, or Booking status.

## Who can view Analytics

Analytics access is checked against the Organization's Analytics permission for the current user. Users can view Analytics only when their Organization permissions allow access.

## Example

A Location has 10 desks and 4 applicable desk Booking instances for the day, so Desk Occupancy Insights shows 40%. This does not mean four hours were booked. If two instances were for the same desk, the numerator would still be 4 because the metric counts Booking instances, not distinct desks or booked hours.

## Best practices

- Read Booking totals and occupancy together; they present Booking activity in different contexts.
- Keep the selected date range consistent when comparing Locations.
- Compare desks and rooms separately because each has its own occupancy series.
- Use Resource Availability Insights for available, unavailable, and booked Resource counts.
- Use Availability when the question is whether a Resource can be booked now or later.
- Treat a partial current date range as incomplete when interpreting daily patterns.

## Things to Know

- Analytics is part of Insights and reads operational metrics.
- Booking Insights reports daily Booking totals.
- Desk and room occupancy are count-based percentages.
- Occupancy is not booked time divided by opening time.
- Occupancy uses applicable Booking instances divided by the desk or room count and can exceed 100% when a Resource has multiple instances.
- Zero-capacity desk or room days are omitted from occupancy percentages.
- Resource Availability Insights reports daily available, unavailable, and booked Resource counts by Resource type.
- Analytics charts display daily values for the selected date range.
- Booking, occupancy, and Resource availability views have different default ranges.
- Analytics does not modify Bookings, Resources, Locations, or Availability.
- Access is scoped by Organization Analytics permissions.

## Frequently Asked Questions

### What is Analytics in Skedular?

Analytics turns daily Booking and Resource activity into Booking totals, occupancy percentages, and Resource availability views for accessible Locations.

### How is Analytics different from Availability?

Availability answers whether a Resource can be booked. Analytics summarizes Booking activity and Resource states over a selected date range.

### How is occupancy calculated?

Desk or room occupancy is the applicable daily Booking-instance count divided by the daily desk or room count, multiplied by 100.

### Does occupancy measure booked minutes?

No. The current occupancy series is count-based and does not use booked duration divided by opening minutes.

### What does Booking Insights show?

It shows the daily total of Booking activity for the selected Location and date range.

### What does Resource Availability Insights show?

It shows daily available, unavailable, and booked Resource counts grouped by Resource type.

### Do recurring Bookings count?

Recurring bookings and Subscription-generated reservations contribute through their individual Booking instances. Analytics does not count a Subscription itself as one Booking.

### Can occupancy exceed 100%?

Yes. Occupancy uses Booking instances rather than distinct occupied Resources, so multiple Bookings for the same desk or room can produce a value above 100%.

### Which date range does Analytics use?

Users select the range. Booking and occupancy views default to one month; Resource Availability Insights defaults to six months.

### Can I export Analytics reports?

The current interface does not expose an Analytics export or report workflow.

### Who can view Analytics?

Users need the Organization permission to view Analytics for the relevant Location data.

## Related Documentation

- [Insights](/docs/shared/insights)
- [Bookings](/docs/shared/core-concepts/bookings)
- [Resources](/docs/shared/core-concepts/resources)
- [Locations](/docs/shared/core-concepts/locations)
- [Availability](/docs/shared/core-concepts/availability)
