---
id: shared-availability
title: "Availability"
description: "See when resources can be booked and understand their booking status for a selected date."
product: shared
category: core-concepts
slug: availability
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/availability.md
  - doc-resources/resource.md
  - doc-resources/booking.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-resources
  - shared-bookings
  - shared-locations
  - shared-floor-plans
updatedAt: 2026-08-07
---

## Overview

Availability is the calculated state that determines when a [Resource](/docs/shared/core-concepts/resources) can be booked. The Availability view presents that state for a selected date, using the Resource's opening schedule and existing [Bookings](/docs/shared/core-concepts/bookings) to show what can be reserved.

<div class="documentation-concept-support"><strong>Supported in</strong><span>✅ Skedular Teams</span><span>✅ Skedular Spaces</span><span>✅ Skedular Host</span></div>

<aside class="documentation-callout" aria-label="Core rule"><strong>Core Rule</strong><p>A resource is available only when its scheduling rules allow a booking and no conflicting booking reserves it for the requested time.</p></aside>

## Why Availability Exists

Availability answers a practical question: what can I book on this date? Instead of opening every resource individually, users and administrators can review resource status, booked time, and booking windows in one date-based view. This supports finding resources, planning future bookings, and reviewing past booking activity without replacing dedicated Analytics reporting.

## How Availability Works

<div class="documentation-concept-workflow"><span><b>1</b>Location Opening Hours</span><span><b>2</b>Resource Schedule</span><span><b>3</b>Existing Bookings</span><span><b>4</b>Availability Calculated</span><span><b>5</b>Status Displayed</span><span><b>6</b>New Booking Recalculates</span></div>

Location Opening Hours establish the bookable schedule. A Resource-specific schedule can provide a different window. Existing Bookings remove reserved periods from that schedule, and the resulting state is displayed in the Availability view.

Product Prices can add a separate **Available days** purchase rule. It accepts any combination of Sunday through Saturday, with no workweek distinction; an empty rule means every day. This rule filters which booking dates a Price can be purchased on. It does not make a Resource available by itself: opening hours, Resource schedules, and existing Bookings still determine Resource Availability.

## Where This Concept Fits

<div class="documentation-organization-context" aria-label="Availability relationships"><div class="documentation-context-root">🕒 <span>Location Opening Hours</span><div class="documentation-context-child"><span>⚙️ Resource Schedule</span><div class="documentation-context-child"><span>⏱ Availability</span><div class="documentation-context-child"><a href="/docs/shared/core-concepts/bookings">📅 New Bookings</a></div></div></div></div><div class="documentation-context-tree"><a href="/docs/shared/core-concepts/bookings">📅 Existing Bookings remove reserved time</a><a href="/docs/shared/core-concepts/floor-plans">🗺 Floor Plans display Availability</a></div></div>

## Viewing Availability

Availability is viewed one date at a time. The date picker supports past, current, and future dates, so users can review previous Booking activity, check today's Resource state, or plan future Bookings. The view can also be filtered by Location and Resource status.

## Resource Status

The Availability view uses these status classifications:

- **Available:** The Resource has bookable time remaining in its schedule.
- **Unavailable:** The Resource cannot be booked for the selected date.
- **Partially Booked:** Some bookable time is reserved, but time remains available.
- **Fully Booked:** All bookable time is reserved for the Resource.
- **Occupied:** The resource has a checked-in booking for the current date where check-in data is available.
- **Blocked:** The resource is inactive or otherwise blocked by the booking workflow. A closed Location or day is reported as Unavailable.

A partially booked resource is not treated as unavailable for the entire day. Its status reflects that some time is reserved while other bookable periods remain.

## Resource Information

Each resource entry can show its name, resource type, Location, Floor Plan and Zone context, status, booked minutes, opening minutes, and individual booking windows. The booked-time summary is shown as `booked minutes / total opening minutes`; the denominator is the resource's calculated opening time for the selected date, not a fixed 24-hour day.

## Historical and Future Availability

Past dates show the resource state and booking windows recorded for that date. Future dates show the bookings already scheduled and the remaining capacity available for planning. The date picker can inspect dates, while creating a Booking is limited to the supported horizon of up to one year in advance.

## Floor Plan Integration

When a resource has a Floor Plan position, the Availability view links to that date's visual layout. Users can locate the resource, see its state in physical context, and continue into the normal booking flow. [Floor Plans](/docs/shared/core-concepts/floor-plans) display Availability; they do not calculate it.

## How Availability Is Calculated

Location Opening Hours provide the default bookable schedule. A Resource can use its own opening-hours override where configured. Existing bookings then remove reserved intervals, and overlapping bookings are rejected. The remaining schedule is the resource's available time for the selected date.

## Managing Availability

Availability is maintained by the booking and scheduling system. Creating, changing, or cancelling a booking changes the affected resource state. Changing Location or Resource opening hours also changes the calculated schedule. Administrators do not manually set a resource to Available or Booked in this view.

When a customer or authorized operator modifies an eligible marketplace Booking, Skedular validates the new date, time, and any replacement Resources against the purchased Product rules, price calendar-day rules, opening hours, and current booking conflicts. Availability is checked again when the change is confirmed. If another Booking takes capacity first, Skedular does not apply a partial change; the original Booking and its Resources stay in place.

## Product Differences

<div class="documentation-concept-grid"><div><strong>🧑‍💼 Skedular Teams</strong><small>Helps internal users understand and book workplace Resources, including Team booking workflows.</small></div><div><strong>🛒 Skedular Spaces</strong><small>Shows the Availability of Resources that can be allocated through marketplace Products.</small></div><div><strong>🏠 Skedular Host</strong><small>Calculates Availability against the underlying Resource while guests interact with the Place.</small></div></div>

## What Affects Availability

<div class="documentation-concept-grid"><div><strong>🕒 Opening Hours</strong><small>Define the default bookable schedule. Dedicated documentation is coming soon.</small></div><div><strong>⚙️ Resource Schedule</strong><small>Overrides the Location schedule where a different resource window is configured.</small></div><a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>Remove reserved periods from the bookable schedule.</small></a><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>The assets whose Availability is calculated.</small></a></div>

## Example

<aside class="documentation-callout" aria-label="Availability example"><p>An employee needs a meeting room on Thursday. They select Thursday in Availability, choose their Location, review which rooms still have bookable time, open the Floor Plan to find a suitable room, and continue into the normal Booking workflow.</p></aside>

## Best Practices

- Keep Location opening hours accurate.
- Configure Resource-specific hours only when the schedule differs.
- Review existing bookings before planning new reservations.
- Cancel bookings that are no longer required.
- Use the status and booking-window details together when choosing a resource.

## Things to Know

- Availability is calculated for Resources.
- The Availability view is date-based.
- Partially Booked means some opening time remains available.
- Booked minutes are measured against the Resource's opening minutes for that date.
- Location and Resource opening hours affect the calculated schedule.
- Existing bookings prevent overlapping reservations.
- Floor Plans display availability but do not calculate it.
- Availability changes automatically when relevant Bookings or schedules change.
- The view can show past, current, and future dates; Booking creation is supported up to one year in advance.

## Frequently Asked Questions

### What is Availability?

Availability is the calculated time during which a Resource can be booked. The Availability view presents that state for a selected date.

### How is Availability calculated?

Location Opening Hours establish the default schedule. A Resource schedule can override it, and existing Bookings remove reserved periods. The remaining schedule is the Resource's available time.

### Why is a Resource unavailable?

It may have no opening time for the selected date, be fully booked, or be marked Unavailable or Blocked by the scheduling workflow.

### What does Partially Booked mean?

Some of the resource's opening time is reserved, but other bookable periods remain available.

### What does the booked-minutes value mean?

It shows booked minutes compared with the resource's total calculated opening minutes for the selected date.

### How far into the future can I view Availability?

The date picker supports past, current, and future dates. Creating a Booking is supported up to one year in advance.

### Does cancelling a Booking make the Resource available again?

Cancelling a Booking removes its reserved period, so the affected Resource can become available again if other scheduling rules do not prevent booking.

## Continue Learning

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/locations"><strong>📍 Locations</strong><small>Where opening schedules are defined.</small></a><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>The assets whose state is calculated.</small></a><a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>Reservations that affect available time.</small></a><a href="/docs/shared/core-concepts/floor-plans"><strong>🗺 Floor Plans</strong><small>Visual resource availability.</small></a></div>

## Related Concepts

- [Resources](/docs/shared/core-concepts/resources)
- [Bookings](/docs/shared/core-concepts/bookings)
- [Locations](/docs/shared/core-concepts/locations)
- [Floor Plans](/docs/shared/core-concepts/floor-plans)
