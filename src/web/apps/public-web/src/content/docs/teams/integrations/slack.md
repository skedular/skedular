---
id: teams-slack
title: "Slack integration"
description: "Use the Skedular Teams Slack app to manage workplace Bookings and Locations and receive daily updates about who's coming in."
product: teams
category: integrations
slug: slack
articleKind: guide
publicationState: published
evidenceRefs:
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-14
---

## Overview

The Skedular Teams Slack app brings everyday workplace workflows into Slack. From the app home, members can view and create Bookings, review a Location's Bookings and Resources, and join an available Booking without opening another tab. Location cards also expose administrative actions when the member has the required permission.

## Before you connect Slack

You need a Skedular Teams Organization and a Slack workspace where you are allowed to install apps. The Slack app uses the Locations, Resources, and members available in the connected Skedular Teams Organization. A member must already be recognized in the Organization before they can use Organization data in Slack.

## Connect Slack to Skedular Teams

1. Select the **Add to Slack** button above this guide.
2. Choose the Slack workspace and approve the permissions shown by Slack.
3. After installation, select **Start using Skedular** on the confirmation page to open the app in Slack.

The installation requests access to workspace and member details, channel discovery and management, app mentions, and posting messages. Slack workspace administrators may need to approve the installation according to their workspace policy.

## Create and manage Bookings from Slack

Open **Bookings** from the app home, then select **Make a booking**. The Booking form uses the Organization's available Locations and Resources and checks the selected time before it is saved. A Booking card shows the date, Location, Resources, members, and Teams attached to the Booking when those values exist.

From a Booking card, available actions include:

- **Edit** to change a Booking.
- **Cancel** to cancel the Booking.
- **Join** to create your own private Booking for the same time as a Booking shared in Slack.

You can filter the list by date, move through pages, and turn on **Only show my bookings**. When a Location or Team update lists a Booking, **Join** creates a separate private Booking for you using that Booking's time and Team association; it does not add you to the original Booking or assign you its Resources. For the complete Skedular Teams Booking model, see [Bookings](/docs/teams/bookings).

## Work with Locations

Open **Locations** from the app home to browse the Locations available to your Organization. A Location card shows its name, description, time zone, the number of Resources at the Location, and its configured **Slack update channel**. From a Location card, members can open its **Bookings**, **Resources**, **Zones**, or **Tags**.

Members with the relevant Location permissions can use **Add Location**, **Edit**, or **Remove**. The edit form can update the Location name, description, time zone, and Slack update channel. Members can also use **Add as preferred location** or **Remove preferred location** to control the Location used by their Slack workflow.

## Who's in today? updates

Skedular Teams can post a daily **Who's in today?** update for a Location. The update is based on that day's Bookings in the Location's time zone. It lists the members attached to those Bookings, includes the associated Resources, and includes a **Join** button so another member can create their own Booking for the same time.

The update is sent to the Location's configured Slack update channel at 7:00 a.m. in that Location's time zone. If no Bookings are found, the message says **No one has joined yet, be the first**. The message shows up to five Bookings; when more exist, it points members to the Skedular app in Slack for the rest.

## Configure the update channel

Members who can edit a Location can set its **Slack update channel** from the Location's **Edit** form. The channel is optional. If no channel is configured, Skedular does not send the daily update. The send time is currently fixed at 7:00 a.m. in the Location's time zone and is not configurable in Slack.

## Permissions and access

The Slack app respects the same permissions that apply to the corresponding actions in Skedular Teams. Members need the relevant Booking permission to access or manage Bookings. Location changes, including adding, editing, and removing a Location, require the corresponding Location permission. Actions can be hidden or blocked when a member lacks access; ask an Organization administrator to grant permission or complete the action in the Skedular Teams web app.

## When to use the Skedular Teams web app

Slack covers the day-to-day Booking and Location actions described above. Use the Skedular Teams web app for workflows that are not exposed in the Slack app, such as broader Organization administration and detailed configuration of the workplace model.

## Troubleshooting

- **The app cannot find you:** confirm that your Slack account is connected to a member of the Skedular Teams Organization.
- **No Locations or Resources appear:** ask an Organization administrator to confirm that they exist and that you have access to them.
- **The daily update is missing:** check that the Location has a **Slack update channel**, that the app can post there, and that the Location time zone is correct.
- **You cannot edit a Location or Booking:** the action requires the matching Organization or Location permission.
