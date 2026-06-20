---
id: teams-set-up-workplace
title: "Set up your workplace"
description: "Create a Location, add bookable Resources, and verify that your private workplace is ready for Bookings."
product: teams
category: workplace-setup
slug: set-up-your-workplace
articleKind: guide
publicationState: published
evidenceRefs:
  - doc-resources/location.md
  - doc-resources/resource.md
  - doc-resources/availability.md
  - doc-resources/booking.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-15
---

Use this guide to set up the Locations and Resources people will use in your private workplace.

## What you are setting up

A Location is the workplace you are configuring. Resources are the individual desks, rooms, parking spaces, or other bookable things inside that Location. This guide focuses on putting them together in Skedular Teams; the shared [Location](/docs/shared/core-concepts/locations) and [Resource](/docs/shared/core-concepts/resources) pages provide the full definitions.

## 1. Create or select the workplace Location

In Skedular Teams, open your Organization and choose **Locations**. Select **Add Location** to create a private Location, or open an existing Location to update it. Complete the Location details and set its **Opening Hours**, which define the normal schedule for its Resources.

## 2. Add the Resources people can book

From the Location, choose **Add Resource**. Choose the type of Resource you are adding, then enter its name and any relevant details, such as capacity. Keep Resource names specific enough that people can choose the right desk, room, or space from a Booking workflow.

## 3. Configure when Resources can be booked

Resources normally follow the Location's Opening Hours. These hours define the normal schedule used when determining Resource Availability and creating Bookings. If one Resource follows a different schedule, open that Resource's settings and choose **Opening Hours** to enable custom hours. This override is optional and should only be used when the Resource is available at different times from the Location.

## 4. Check Availability

Open the Organization's **Availability** view and select the Location and date you want to test. Confirm that the expected Resources appear available during their Location schedule or custom Resource hours. See [Availability](/docs/shared/core-concepts/availability) for the rules behind the result.

## 5. Create a test Booking

Open **Bookings**, choose **Add Booking**, and select the Location, Resource, person, and time you want to test. A successful private Booking confirms that the Location, Resource, and schedule are ready for use. See [Bookings](/docs/shared/core-concepts/bookings) for the shared Booking model.

## Workplace readiness check

Your basic setup is ready when the Location appears with the correct details, its expected Resources are listed, the right booking times appear available, and you can create a test Booking successfully.

Floor Plans, Tags, and Zones are optional follow-up organization. Continue with [Organize your workplace](/docs/teams/workplace-setup/organize-your-workplace) when you want to make Resources easier to find.
