---
id: TeamOrganizationSubscriberService
version: 0.0.1
name: Team Organization Subscriber Service
summary: |
  Team Service that handles all commands 
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
  url: 
---

## Overview

The Team Service is a component of the system responsible for managing team structure and roles. It interacts with other services to maintain accurate team members format.

## Architecture diagram

<NodeGraph title="Hello world" />