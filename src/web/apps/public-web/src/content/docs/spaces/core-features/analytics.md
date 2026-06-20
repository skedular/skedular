---
id: spaces-analytics
title: "Operator analytics"
description: "Understand Booking activity, occupancy, and Resource availability across your Organization and Locations."
product: spaces
category: analytics
slug: analytics
articleKind: guide
publicationState: published
evidenceRefs:
  - src/web/apps/webapp-spaces/src/components/organization/organizationAnalytics/organization-analytics.tsx
  - src/web/apps/webapp-spaces/src/components/organization/organizationBookingInsight/organization-booking-insight.tsx
  - src/web/apps/webapp-spaces/src/components/organization/organizationMemberAttendancyInsight/organization-member-attendancy-insight.tsx
  - src/web/apps/webapp-spaces/src/components/location/locationBookingInsight/location-booking-insight.tsx
  - src/web/apps/webapp-spaces/src/components/location/locationDeskOccupancyInsight/location-desk-occupancy-insight.tsx
  - src/web/apps/webapp-spaces/src/components/location/locationResourceAvailabilityInsight/location-resource-availability-insight.tsx
  - src/web/apps/webapp-spaces/src/components/analytics/analytics-daterange-selector.tsx
  - src/web/apps/webapp-spaces/src/components/navigationMenu/left-side-navigation-menu-content.tsx
  - docs-resources/analytics.md
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - spaces-faq
updatedAt: 2026-07-17
---

## Understand Operator analytics

Operator analytics gives Spaces Organization users a read-only view of Booking activity and Resource activity over time. Use it to see how busy the Organization is, review activity at individual Locations, and understand desk occupancy and Resource availability.

Analytics does not create or change Bookings, Resources, Locations, Availability rules, Products, or Customers. It reports the activity already recorded by Skedular.

## Choose the scope

The Analytics area has two views:

- **Organization** shows Organization-level Booking totals and Member Attendance Insights over the selected range.
- **Locations** lets you select one Location or view the available Locations. Each Location view shows Booking Insights, Desk Occupancy Insights, and Resource Availability Insights.

There is no separate Resource analytics page. Resources appear in the Location-level availability view rather than as an independent reporting scope.

## Understand the key metrics

### Booking Insights

Booking Insights shows daily Booking totals for the Organization or selected Location. It counts the Booking activity returned for the selected date range; it does not report booked hours.

### Member Attendance Insights

Organization Insights also includes Member Attendance Insights. It compares the daily Booking count with the Organization's daily member count and presents that result as a percentage. The metric describes Organization members; it does not measure marketplace Customers, who are registered Skedular users and are not automatically Organization members. Because it is based on Booking and member counts, the percentage can exceed 100% when a day contains more Bookings than the recorded member count.

### Desk Occupancy Insights

Location views show Desk Occupancy Insights. The daily percentage compares the recorded desk Booking count with the recorded desk count for that day. Multiple Booking instances can make the percentage exceed 100%; it is not a measure of uniquely occupied Resources, booked duration, opening-hours usage, or physical headcount.

### Resource Availability Insights

Resource Availability Insights shows daily Resource counts grouped by Resource type: **Available**, **Unavailable**, and **Booked**. These are daily state counts produced for the selected Location and range, not a real-time booking decision or a calculation of booked minutes. Use Availability when you need to check whether a Resource can be booked for a particular time.

## Explore activity over time

Each insight has a date-range control with these options:

- **1 Week**
- **1 Month**
- **3 Months**
- **6 Months**
- **Custom**

The Booking and occupancy views start with **1 Month**. Resource Availability Insights starts with **6 Months**. Choosing **Custom** opens a From and To date range. The charts display daily values for the selected range, including current dates when they fall within it.

## Location and Resource activity

Use the Locations view to select a Location and read its Booking, occupancy, and Resource Availability Insights together. Resource type is an optional view in the Resource Availability chart. The current UI does not offer Customer, Product, Tag, Zone, payment-status, or Booking-status filters, rankings, exports, or a separate Resource analytics page.

## Analytics and Availability

Availability answers whether a Resource can be booked for a particular date or time. Analytics summarizes Booking activity and Resource patterns across a reporting period, which can include the current date.

## Use analytics to improve operations

Operators can use the views to identify busy days, review activity across Locations, understand desk occupancy, and spot Resource types that are often Available or Booked. Use those observations to guide operational decisions such as reviewing workspace configuration or opening hours; Analytics does not make those changes automatically.

## Data boundaries

Analytics is based on daily Booking and Resource activity for the selected Organization or Location. Cancelled Bookings are excluded from the analytics recordings. Recurring and Subscription activity contributes through its individual Booking instances. Analytics does not treat a Subscription as one Booking, and it does not provide a separate Booking-status breakdown.

## Who can view Operator analytics

The Organization's **Analytics** permission controls access. Organization users can open Analytics only when that permission allows it. Marketplace Customers are registered Skedular users, not Organization users, and do not gain access to the operator's Analytics area through a Product purchase.

## Things to know

- Analytics is read-only and scoped to the current Organization.
- Member Attendance Insights describes Organization members, not marketplace Customers.
- Occupancy is a count-based percentage and can exceed 100% when daily desk Booking counts are higher than the recorded desk count.
- There is no separate Resource-level analytics route or export workflow.

## Next step

Continue to [FAQs](/docs/spaces/faqs) for common questions about using and operating Skedular Spaces.
