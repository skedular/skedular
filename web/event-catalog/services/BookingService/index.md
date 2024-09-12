---
id: BookingService
version: 0.0.1
name: Booking Service
summary: |
  Booking Service that handles all commands 
owners:
    - malizadeh
    - full-stack
receives:
  - id: AddBooking
    version: 0.0.1
  - id: UpdateBooking
    version: 0.0.1
  - id: DeleteBooking
    version: 0.0.1
  - id: CustomerUpserted
    version: 0.0.1
  - id: CustomerDeleted
    version: 0.0.1   
  - id: TeamUpserted
    version: 0.0.1
  - id: TeamDeleted
    version: 0.0.1
sends:
  - id: BookingUpserted
    version: 0.0.1
  - id: BookingDeleted
    version: 0.0.1
repository:
  language: C#
  url: 
---

## Overview

The Team Service is a component of the system responsible for managing team structure and roles. It interacts with other services to maintain accurate team members format.

## Architecture diagram

<NodeGraph title="Hello world" />