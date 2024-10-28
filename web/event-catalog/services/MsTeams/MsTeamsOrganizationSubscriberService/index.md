---
id: MsTeamsOrganizationSubscriberService
version: 0.0.1
name: MsTeams Organization Subscriber Service
summary: |
  MsTeams Organization Subscriber Service that handles all events 
owners:
    - malizadeh
    - full-stack
receives: 
  - id: OrganizationUpserted
    version: 0.0.1
  - id: OrganizationDeleted
    version: 0.0.1
  - id: InvitationToJoinOrganizationUpserted
    version: 0.0.1
  - id: InvitationToJoinOrganizationDeleted
    version: 0.0.1
repository:
  language: C#
  url: 
---

## Overview

The Notification Organization Subscriber Service is a component of the system responsible for managing organization. It interacts with other services to maintain accurate of an organization.

## Architecture diagram