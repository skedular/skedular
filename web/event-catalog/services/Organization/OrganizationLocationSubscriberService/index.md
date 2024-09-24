---
id: OrganizationLocationSubscriberService
version: 0.0.1
name: Customer location Subscriber Service
summary: |
  Customer Location Subscriber Service that handles all events 
owners:
    - malizadeh
    - full-stack
receives:
  - id: LocationUpserted
    version: 0.0.1
  - id: LocationDeleted
    version: 0.0.1
repository:
  language: C#
  url: 
---

## Overview

The Customer Location Subscriber Service is a component of the system responsible for managing customer's location information. It interacts with other services to maintain accurate booking data.

## Architecture diagram