---
id: OrganizationCustomerSubscriberService
version: 0.0.1
name: Organization Customer Subscriber Service
summary: |
  Organization Customer Subscriber Service that handles all events 
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

The Organization Customer Subscriber Service is a component of the system responsible for managing organization. It interacts with other services to maintain accurate organization information.

## Architecture diagram