---
id: shared-users
title: "Users"
description: "Users are the people who access Skedular organizations, manage work, and make bookings."
product: shared
category: core-concepts
slug: users
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/user.md
  - doc-resources/organization.md
  - doc-resources/booking.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-organizations
  - shared-teams
  - shared-bookings
  - shared-resources
updatedAt: 2026-07-15
---

## Overview

Users are the people who sign in to Skedular, belong to one or more organizations, and perform actions such as booking resources, managing locations, or administering workspaces. Their organization role determines which actions and information are available to them.

<div class="documentation-concept-support"><strong>Supported in</strong><span>✅ Skedular Teams</span><span>✅ Skedular Spaces</span><span>✅ Skedular Host</span></div>

<aside class="documentation-callout" aria-label="Core rule"><strong>Core Rule</strong><p>Every action performed in Skedular is associated with a user who belongs to an organization with an assigned role.</p></aside>

## How Users Work

A user creates or receives an account, accepts an organization invitation when required, and joins the organization as a member. The organization assigns a role, and the user then signs in to perform the actions allowed by that role. A user can belong to multiple organizations, with a different role in each. Removing the organization membership ends that user's access to the organization.

## Where This Concept Fits

<div class="documentation-organization-context" aria-label="User relationships"><div class="documentation-context-root">👤 <span>User</span><div class="documentation-context-child"><a href="/docs/shared/core-concepts/organizations">🏢 Organizations</a><div class="documentation-context-child"><span>🔐 Roles and permissions</span><a href="/docs/shared/core-concepts/teams">👥 Teams</a><a href="/docs/shared/core-concepts/bookings">📅 Bookings</a></div></div></div><div class="documentation-context-tree"><span>🔑 Authentication</span><span>🪪 Profile</span></div></div>

## Product Support

- **Skedular Teams:** Users are private organization members. Owners, Administrators, and Members can have different responsibilities, and members can join teams.
- **Skedular Spaces:** Users operate marketplace organizations as Owners, Administrators, or Members. Customers have a separate customer-facing booking relationship and do not manage the organization.
- **Skedular Host:** The current model has one owner for the host organization. Additional organization users are not supported in this simplified workflow.

## User Identity

A user is identified by their account and organization memberships. The same person can belong to several organizations. Each membership stores its own role and status, so access is evaluated in the context of the organization being used.

## Organizations and Roles

Organizations are the ownership boundary for users, locations, resources, settings, and bookings. A user can belong to multiple organizations, but a role in one organization does not automatically grant access to another.

### Owner

Owners have full control of an organization, including settings, billing, locations, resources, users, teams, bookings, and role assignments.

### Administrator

Administrators handle day-to-day operations such as locations, resources, bookings, teams, users, and operational settings. Their access may not include billing or ownership controls.

### Member

Members use the organization. Depending on its settings, they can create and manage bookings, view availability, and join teams, without access to administrative functions.

## Authentication and Permissions

Users sign in through Skedular's configured identity provider. WorkOS is supported as an identity-provider integration where it is configured. Authentication confirms who the user is; the organization's assigned role determines what that user can do. This page describes the relationship only. See the product guides for the available sign-in workflow.

## User Lifecycle

Users move through a simple organization membership lifecycle:

- **Invited:** An organization asks a person to join.
- **Joined:** The person accepts and becomes an organization member.
- **Active:** The member signs in and uses the permissions granted by their role.
- **Removed:** The membership is removed, ending access to that organization.

## Product Differences

<div class="documentation-concept-grid"><div><strong>👥 Skedular Teams</strong><small>Private workplace members can join teams and coordinate workplace bookings.</small></div><div><strong>🛒 Skedular Spaces</strong><small>Organization users operate locations, products, customers, bookings, and marketplace settings.</small></div><div><strong>🏠 Skedular Host</strong><small>A single owner manages the host organization and its places through a simplified workflow.</small></div></div>

## What Belongs to a User

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/organizations"><strong>🏢 Organizations</strong><small>Ownership boundaries that grant a user access.</small></a><div><strong>🔐 Roles</strong><small>Organization-specific responsibilities and permissions.</small></div><a href="/docs/shared/core-concepts/teams"><strong>👥 Teams</strong><small>Groups a user can join in Skedular Teams.</small></a><a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>Reservations a user can make or manage.</small></a><div><strong>🛡 Permissions</strong><small>Access derived from the assigned organization role.</small></div><div><strong>🪪 Profile</strong><small>Account information used to identify the user.</small></div></div>

## Typical Workflow

<div class="documentation-concept-workflow"><span><b>1</b>Invite User</span><span><b>2</b>Accept Invitation</span><span><b>3</b>Join Organization</span><span><b>4</b>Assign Role</span><span><b>5</b>Book Resources</span><span><b>6</b>Use Skedular</span></div>

## Best Practices

- Assign the least privileged role needed for the work.
- Review administrator access regularly.
- Remove users who no longer need organization access.
- Keep team membership current in Skedular Teams.
- Keep user account details accurate.

## Things to Know

- A user can belong to multiple organizations.
- Roles are assigned per organization.
- Permissions come from the assigned role.
- Users can create bookings when their role allows it.
- Teams are available only in Skedular Teams.
- Marketplace customers have a customer-facing relationship, separate from organization administration.
- Skedular Host currently uses a single-owner organization model.

## Frequently Asked Questions

### What is a user?

A user is a person with an account who belongs to an organization or participates in a booking workflow.

### Can a user belong to multiple organizations?

Yes. The same person can belong to multiple organizations, with a different role in each.

### Can I change a user's role?

An owner or authorized administrator can manage organization membership and assign roles when permitted.

### What happens when I remove a user?

The user's membership is removed and access to that organization ends. Existing booking records may remain part of the organization's history.

### Do marketplace customers count as organization users?

Customers can make marketplace bookings, but they are separate from the organization members who administer the marketplace organization.

### How do users sign in?

Users sign in through the identity provider configured for their Skedular environment. WorkOS can provide that identity-provider integration when configured.

## Continue Learning

<div class="documentation-concept-grid"><a href="/docs/shared/core-concepts/organizations"><strong>🏢 Organizations</strong><small>How ownership and membership are structured.</small></a><a href="/docs/shared/core-concepts/resources"><strong>🪑 Resources</strong><small>The assets users reserve.</small></a><a href="/docs/shared/core-concepts/teams"><strong>👥 Teams</strong><small>Groups for private workplace coordination.</small></a><a href="/docs/shared/core-concepts/bookings"><strong>📅 Bookings</strong><small>How users reserve resources.</small></a></div>

## Related Concepts

- [Organizations](/docs/shared/core-concepts/organizations)
- [Resources](/docs/shared/core-concepts/resources)
- [Teams](/docs/shared/core-concepts/teams)
- [Bookings](/docs/shared/core-concepts/bookings)
- Roles and Permissions (covered on this page)
- Authentication (covered on this page)
