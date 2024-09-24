---
id: CustomerOrganizationSubscriberService
version: 0.0.1
name: Customer Organization Subscriber Service
summary: |
  Customer Organization Subscriber Service that handles all events 
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

The Customer Organization Subscriber Service is a component of the system responsible for managing customer 's organizational information. It interacts with other services to maintain accurate customer data.

## Architecture diagram