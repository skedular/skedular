---
id: LocationOrganizationSubscriberService
version: 0.0.1
name: Location Organization Subscriber Service
summary: |
  Location Organization Subscriber Service that handles all events 
owners:
    - malizadeh
    - full-stack
receives: 
  - id: OrganizationUpserted
    version: 0.0.1
  - id: OrganizationDeleted
    version: 0.0.1
repository:
  language: C#
---

## Overview

The Location Organization Subscriber Service is a component of the system responsible for managing location. It interacts with other services to maintain accurate of locations in an organization.

## Architecture diagram