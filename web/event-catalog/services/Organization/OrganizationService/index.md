---
id: OrganizationService
version: 0.0.1
name: Organization Service
summary: |
  Organization Service that handles all commands 
owners:
    - malizadeh
    - full-stack
receives:
  - id: AddOrganization
    version: 0.0.1
  - id: UpdateOrganization
    version: 0.0.1
  - id: DeleteOrganization
    version: 0.0.1
  - id: AcceptInvitationToJoinOrganization
    version: 0.0.1
  - id: CancelInvitationToJoinOrganization
    version: 0.0.1
  - id: CancelOrganizationOffering
    version: 0.0.1
  - id: ChangeOrganizationMemberOwnershipType
    version: 0.0.1
  - id: InviteCustomersToJoinOrganization
    version: 0.0.1
  - id: RejectInvitationToJoinOrganization
    version: 0.0.1
  - id: UpdateOrganizationOffering
    version: 0.0.1
sends:
  - id: OrganizationDeleted
    version: 0.0.1
  - id: OrganizationUpserted
    version: 0.0.1
  - id: OrganizationOfferingUpdated
    version: 0.0.1
  - id: OrganizationMemberUpserted
    version: 0.0.1
  - id: OrganizationMemberDeleted
    version: 0.0.1
  - id: OrganizationMemberStatusUpdated
    service: 0.0.1
repository:
  language: C#
  url: 
---

## Overview

The organization Service is a component of the system responsible for managing organization details.

## Architecture diagram

<NodeGraph title="Hello world" />