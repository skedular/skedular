---
id: NotificationOrganizationSubscriberService
version: 0.0.1
name: Notification Organization Subscriber Service
summary: |
  Notification Organization Subscriber Service that handles all events 
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

The Notification Organization Subscriber Service is a component of the system responsible for managing organization. It interacts with other services to maintain accurate of an organization.

## Architecture diagram