---
id: OrganizationBookingSubscriberService
version: 0.0.1
name: Organization Booking Subscriber Service
summary: |
  Organization Service that handles all events related to booking in an organization
owners:
    - malizadeh
    - full-stack
receives:
  - id: BookingUpserted
    version: 0.0.1
  - id: BookingDeleted
    version: 0.0.1
repository:
  language: C#
  url: 
---

## Overview

The Organization Booking Subscriber Service is a component of the system responsible for managing team structure and members bookings. It interacts with other services to maintain accurate an organization information.

## Architecture diagram