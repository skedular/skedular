---
id: NotificationLocationSubscriberService
version: 0.0.1
name: Notification location Subscriber Service
summary: |
  Notification Location Subscriber Service that handles all events 
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
  url: 
---

## Overview

The Notification Location Subscriber Service is a component of the system responsible for managing customer's location information. It interacts with other services to maintain accurate locations.

## Architecture diagram