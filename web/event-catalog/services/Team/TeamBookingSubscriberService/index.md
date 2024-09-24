---
id: TeamBookingSubscriberService
version: 0.0.1
name: Team Booking Subscriber Service
summary: |
  Team Service that handles all events related to booking in Team
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

The Team Booking Subscriber Service is a component of the system responsible for managing team structure and members bookings. It interacts with other services to maintain accurate team member information.

## Architecture diagram