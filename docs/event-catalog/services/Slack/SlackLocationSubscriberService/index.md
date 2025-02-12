---
id: SlackLocationSubscriberService
version: 0.0.1
name: Slack location Subscriber Service
summary: |
  Slack Location Subscriber Service that handles all events 
owners:
    - malizadeh
    - full-stack
receives:
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
---

## Overview

The Slack Location Subscriber Service is a component of the system responsible for managing slack's location information. It interacts with other services to maintain accurate locations.

## Architecture diagram