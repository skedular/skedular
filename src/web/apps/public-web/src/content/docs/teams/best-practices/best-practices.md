---
id: teams-best-practices
title: "Best Practices"
description: "Practical guidance for setting up, rolling out, and maintaining Skedular Teams effectively."
product: teams
category: best-practices
slug: best-practices
articleKind: best-practice
publicationState: published
evidenceRefs:
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-16
---

A successful Skedular Teams rollout starts with a workplace structure people can understand and Booking information they can trust. These recommendations can help you introduce Skedular gradually, keep the setup accurate, and make everyday Booking easier for your Organization.

## Start with a clear workplace structure

Model the workplace people actually recognize. Define Locations around real places, add only the Resources people need to book, and use names that make sense without extra explanation. Configure Opening Hours before a wider rollout, and keep Location and Resource details accurate as the workplace changes. The [Workplace Setup guides](/docs/teams/workplace-setup) cover the configuration workflows.

## Add structure only when it helps

Start with the smallest structure that accurately represents the workplace. Teams, Zones, Tags, and additional Floor Plans are optional; add them when they solve a real navigation, grouping, or people-management problem. Avoid overlapping labels and agree on naming conventions before the workplace grows. Use [Organize your workplace](/docs/teams/workplace-setup/organize-your-workplace) and [Organize your people](/docs/teams/workplace-setup/organize-your-people) for the detailed setup.

## Make Availability trustworthy

Keep Location Opening Hours accurate, add Resource-specific hours where an override is needed, and make sure each Resource belongs to the correct Location. Manage unavailable Resources and update configuration when the physical workplace changes. If people repeatedly find that a Resource shown as available cannot actually be used, they will stop trusting the Booking system. See [Set up your workplace](/docs/teams/workplace-setup/set-up-your-workplace) for the configuration context.

## Test before a wider rollout

Test the real User journey with an Owner or Administrator before inviting the wider Organization. Find a Location and available Resource, create and view a Booking, change or cancel it, and verify permissions for creating a Booking for another person. If your Organization uses Team associations or recurring Bookings, test those paths as well. Use the [Bookings guide](/docs/teams/bookings) for the complete workflow.

## Introduce Skedular gradually

For a complex workplace, configure the initial setup, test it with Owners and Administrators, then introduce it to a small group of Users. Collect feedback about Resource names, Availability, and Booking workflows, correct confusing setup, and expand when the experience is reliable. Smaller Organizations may be able to launch immediately; use the rollout pace that fits the workplace.

Explain why the Organization is introducing Skedular, which Resources should be booked, when a Booking is expected, and where people should manage Bookings. Give members a clear contact for correcting workplace information.

## Use Teams for meaningful groups

Teams are optional. Users can create Bookings without belonging to a Team, while a Team association can add useful context to a Booking. It does not create a Booking for every Team member, so avoid creating Teams that do not serve a clear organizational or Booking purpose. See [Organize your people](/docs/teams/workplace-setup/organize-your-people).

## Choose integrations intentionally

Use the Skedular Teams web app for the complete workplace management experience. Add Slack when the Organization already works there and the supported Booking, Location, and **Who's in today?** workflows are useful. Connect Slack after the core workplace structure is correct, choose a useful channel for daily updates, make sure members understand **Join**, and confirm Location time zones because the daily schedule depends on them. See the [Slack integration guide](/docs/teams/integrations/slack).

If the Organization needs centrally managed authentication, configure and test [Enterprise sign-in](/docs/teams/integrations/enterprise-sign-in) before a broad rollout. Existing members must be set up correctly; SSO does not automatically add new Organization members. Microsoft Teams documentation is still in progress, so do not depend on that integration until its guide is ready.

## Keep workplace information current

Review Skedular when Resources are added or removed, desks or rooms move, Opening Hours change, layouts change, members or Teams change, or Slack notification needs change. Update or remove obsolete configuration rather than leaving misleading information available to Users.

## Review how the workplace is being used

Periodically review Booking activity, peak Booking hours, busiest days, Occupancy, and Resource availability where those insights are available. These patterns can help you adjust workplace setup and Resource availability without guessing. See [Analytics](/docs/shared/insights/analytics) for the available reporting context.
