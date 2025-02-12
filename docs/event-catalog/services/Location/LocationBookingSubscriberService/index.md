---
id: LocationBookingSubscriberService
version: 0.0.1
name: Location Booking Subscriber Service
summary: |
  Location Service that handles all events 
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
---

## Overview

The Location Booking Subscriber Service is a component of the system responsible for managing location. It interacts with other services to maintain accurate of locations in an organization.

## Architecture diagram