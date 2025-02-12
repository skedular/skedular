---
id: BookingCustomerSubscriberService
version: 0.0.1
name: Booking Customer Subscriber Service
summary: |
  Booking Customer Subscriber Service that handles all events 
owners:
    - malizadeh
    - full-stack
receives:
  - id: CustomerUpserted
    version: 0.0.1
  - id: CustomerDeleted
    version: 0.0.1
repository:
  language: C#
---

## Overview

The Booking Customer Subscriber Service is a component of the system responsible for managing customer information. It interacts with other services to maintain accurate customer data.

## Architecture diagram