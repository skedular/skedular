---
id: teams-private-bookings
title: "Bookings"
description: "Create, view, and manage Bookings in a private Skedular Teams workplace."
product: teams
category: bookings
slug: bookings
articleKind: guide
publicationState: published
evidenceRefs:
  - doc-resources/booking.md
  - doc-resources/availability.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-16
---

Use this guide when your Location and Resources are already set up. The shared [Bookings](/docs/shared/core-concepts/bookings) page explains the universal Booking model; this page explains how people create and manage private Bookings in Skedular Teams.

## Create a Booking

Open **Bookings** in your Organization and choose **Add Booking**. Select the person, date and time, Location, and Resource in the order that suits the form. The Resource choices reflect the selected context and the time that can be booked, subject to the Location's Opening Hours, Resource-specific hours, and existing Bookings. See [Availability](/docs/shared/core-concepts/availability) for the rules behind those results.

You can create a Booking for yourself or, if your account has permission to manage Bookings for others, select another Organization member. For a one-time Booking, finish with **Create booking**; for a recurring Booking, choose the recurring option and finish with **Create recurring booking**.

## Associate a Booking with a Team

After selecting the person, the **Team** field can show Teams that person belongs to. Team selection is optional. It associates the individual Booking with that person's Team; it does not create a separate Booking for every Team member.

## View and manage Bookings

Use **Bookings** to review the private Bookings available in the Organization view. **My Bookings** shows private Bookings where the signed-in User is one of the people involved. Open a Booking and use **Edit Booking** or **Remove Booking**. For recurring Bookings, the interface also exposes **Edit this occurrence**, **Remove this occurrence**, **Edit recurring booking**, and **Remove recurring series** where those actions apply.

## Understand private workplace visibility

Private Bookings stay within the private Organization. Users who can access the Organization's **Bookings** view can see the private Booking information exposed there; **My Bookings** filters the list to Bookings involving the signed-in User. Team association does not grant a separate permission boundary or change the Booking into a group of individual Bookings.

## Ready to use Bookings

Your workplace is ready when an Organization member can select an available Resource and time, create a private Booking, find it in the appropriate Booking view, and use the available management actions. If no Resources appear, return to [Set up your workplace](/docs/teams/workplace-setup/set-up-your-workplace) and check the Location, Resource, and Opening Hours configuration.
