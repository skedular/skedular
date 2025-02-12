---
id: OrganizationTeamSubscriberService
version: 0.0.1
name: Organization  Team Subscriber Service
summary: |
  Organization Team Subscriber Service that handles all events 
owners:
    - malizadeh
    - full-stack
receives:
  - id: TeamUpserted
    version: 0.0.1
  - id: TeamDeleted
    version: 0.0.1
repository:
  language: C#
---

## Overview

The Organization Team Subscriber Service is a component of the system responsible for managing customer's team information. It interacts with other services to maintain accurate customer data.

## Architecture diagram