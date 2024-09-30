---
id: LocationService
version: 0.0.1
name: Location Service
summary: |
  Location Service that handles all commands 
owners:
    - malizadeh
    - full-stack
receives: 
  - id: AddDesk
    version: 0.0.1
  - id: AddBulkDesk
    version: 0.0.1
  - id: UpdateDesk
    version: 0.0.1
  - id: DeleteDesk
    version: 0.0.1
  - id: AddLocation
    version: 0.0.1
  - id: UpdateLocation
    version: 0.0.1
  - id: DeleteLocation
    version: 0.0.1
  - id: AddLocationTag
    version: 0.0.1
  - id: UpdateLocationTag
    version: 0.0.1
  - id: DeleteLocationTag
    version: 0.0.1
  - id: ChangeLocationMemberOwnershipType
    version: 0.0.1
  - id: InviteCustomersToJoinLocation
    version: 0.0.1
  - id: AcceptInvitationToJoinLocation
    version: 0.0.1
  - id: RejectInvitationToJoinLocation
    version: 0.0.1
  - id: CancelInvitationToJoinLocation
    version: 0.0.1
sends:
  - id: LocationUpserted
    version: 0.0.1
  - id: LocationDeleted
    version: 0.0.1
  - id: InvitationToJoinLocationDeleted
    version: 0.0.1
  - id: InvitationToJoinLocationUpserted
    version: 0.0.1
repository:
  language: C#
  url: 
---

## Overview

The Location Service is a component of the system responsible for managing location. It interacts with other services to maintain accurate of locations in an organization.

## Architecture diagram