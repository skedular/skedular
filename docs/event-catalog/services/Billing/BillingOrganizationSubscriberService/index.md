---
id: BillingOrganizationSubscriberService
version: 0.0.1
name: Billing Organization Subscriber Service
summary: |
  Team Service that handles all commands 
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

The Billing Organization Subscriber Service is a component of the system responsible for managing organization information. It interacts with other services to maintain accurate organization information.

## Architecture diagram