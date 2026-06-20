---
id: shared-teams
title: "Teams"
description: "Teams group organization users for workplace coordination and team bookings in Skedular Teams."
product: shared
category: core-concepts
slug: teams
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/team.md
  - doc-resources/user.md
  - doc-resources/booking.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-organizations
  - shared-users
  - shared-resources
  - shared-bookings
updatedAt: 2026-07-15
---

## Overview

Teams are groups of users within an organization that simplify workplace coordination, team bookings, and collaboration in Skedular Teams. Organizations create them when people regularly work together, attend the office together, or need to reserve resources as a group.

<div class="documentation-concept-support"><strong>Availability</strong><span>✅ Skedular Teams</span><span>❌ Skedular Spaces</span><span>❌ Skedular Host</span></div>

<aside class="documentation-callout" aria-label="Core rule"><strong>Core Rule</strong><p>Every team belongs to exactly one organization and contains one or more users from that organization.</p></aside>

## How Teams Work

An authorized organization user creates a team, gives it a name, and adds members. A user can belong to multiple teams. When a team booking is created, Skedular creates the required bookings for the selected members subject to resource availability and the organization's booking rules. Membership changes affect future bookings and do not change bookings that already exist.

## Where This Concept Fits

<div class="documentation-organization-context" aria-label="Team relationships"><div class="documentation-context-root">🏢 <a href="/docs/shared/core-concepts/organizations">Organization</a><div class="documentation-context-child"><a href="/docs/shared/core-concepts/users">👤 Users</a><div class="documentation-context-child"><span>👥 Team</span><div class="documentation-context-child"><a href="/docs/shared/core-concepts/bookings">📅 Bookings</a><div class="documentation-context-child"><a href="/docs/shared/core-concepts/resources">🪑 Resources</a></div></div></div></div></div><div class="documentation-context-tree"><span>📊 Analytics (coming soon)</span></div></div>

## Team Ownership

A team cannot exist independently. It is owned by one organization, and every team member must belong to that same organization. Organization owners and authorized administrators manage teams according to their permissions.

## Team Membership

Teams contain organization users with active team memberships. Users can belong to no teams, one team, or multiple teams. Authorized users can add members, remove members, and update the team as the organization changes.

## Team Bookings

Team bookings let an authorized user select a team instead of adding each person one by one. The booking still reserves [Resources](/docs/shared/core-concepts/resources), and the resulting reservations are recorded as [Bookings](/docs/shared/core-concepts/bookings). See the Bookings guide for complete booking rules and availability behavior.

## Team Visibility

Teams are managed inside the organization. Owners and authorized administrators can manage team details and membership. Members may be able to view teams, view members, or create team bookings depending on the organization's permission settings.

## Team Lifecycle

Teams can be created, renamed, updated, and deleted by authorized users. Deleting a team removes the grouping for future coordination. Existing bookings created for its members are not rewritten by a membership change.

## Product Differences

<div class="documentation-concept-grid"><div><strong>✅ Skedular Teams</strong><small>Supports workplace teams, membership management, and team bookings for private organizations.</small></div><div><strong>❌ Skedular Spaces</strong><small>Does not use workplace teams. Marketplace organizations manage customers, products, and bookings instead.</small></div><div><strong>❌ Skedular Host</strong><small>Does not use teams. The current host workflow uses a simplified single-owner organization model.</small></div></div>

## What Belongs to a Team

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/users"><strong>👤 Members</strong><small>Users from the owning organization.</small></a><a href="/docs/shared/core-concepts/organizations"><strong>🏢 Organization</strong><small>The ownership boundary for the team.</small></a><a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>Reservations created for team members.</small></a><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>Bookable assets used by team bookings.</small></a><div><strong>📊 Analytics</strong><small>Team analytics documentation is coming soon.</small></div></div>

## Typical Workflow

<div class="documentation-concept-workflow"><span><b>1</b>Create Team</span><span><b>2</b>Invite Members</span><span><b>3</b>Assign Members</span><span><b>4</b>Book Resources</span><span><b>5</b>Coordinate Workspace</span><span><b>6</b>Manage Membership</span></div>

## Best Practices

- Create teams that reflect how people actually work together.
- Use clear, descriptive team names.
- Keep membership current as responsibilities change.
- Remove inactive members from teams.
- Review team membership regularly.
- Avoid duplicate teams with overlapping purposes.

## Things to Know

- Every team belongs to one organization.
- Teams are available only in Skedular Teams.
- Users can belong to multiple teams.
- Team bookings reserve resources for the selected members.
- Membership changes affect future bookings only.
- Team membership is organization-specific.

## Frequently Asked Questions

### What is a team?

A team is a group of organization users who need to coordinate workplace activity or bookings together.

### Can users belong to multiple teams?

Yes. A user can belong to no teams, one team, or multiple teams.

### Can I rename a team?

Yes. Authorized users can edit team details, including the team name.

### Can I delete a team?

Yes. Authorized users can delete teams that are no longer required. Existing bookings are not rewritten by membership changes.

### Are teams available in Skedular Spaces?

No. Teams are currently available only in Skedular Teams.

### Are teams available in Skedular Host?

No. Skedular Host currently uses a simplified single-owner organization model.

### How do team bookings work?

An authorized user selects a team and resources. Skedular creates the required bookings for the team's members, subject to availability and booking rules.

## Continue Learning

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/organizations"><strong>🏢 Organizations</strong><small>The ownership boundary for teams.</small></a><a href="/docs/shared/core-concepts/users"><strong>👤 Users</strong><small>The people who belong to teams.</small></a><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>The assets teams reserve.</small></a><a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>How team reservations are created.</small></a></div>

## Related Concepts

- [Organizations](/docs/shared/core-concepts/organizations)
- [Users](/docs/shared/core-concepts/users)
- [Resources](/docs/shared/core-concepts/resources)
- [Bookings](/docs/shared/core-concepts/bookings)
