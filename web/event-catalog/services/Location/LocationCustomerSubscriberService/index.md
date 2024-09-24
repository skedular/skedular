---
id: LocationCustomerSubscriberService
version: 0.0.1
name: Location Customer Subscriber Service
summary: |
  Location Customer Subscriber Service that handles all events 
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
  url: 
---

## Overview

The Location Customer Subscriber Service is a component of the system responsible for managing location. It interacts with other services to maintain accurate of locations in an organization.

## Architecture diagram