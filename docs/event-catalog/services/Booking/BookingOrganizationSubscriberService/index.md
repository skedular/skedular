---
id: BookingOrganizationSubscriberService
version: 0.0.1
name: Booking Organization Subscriber Service
summary: |
  Booking Organization Subscriber Service that handles all events 
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

The Booking Organization Subscriber Service is a component of the system responsible for managing an organization information. It interacts with other services to maintain accurate organization data.

## Architecture diagram