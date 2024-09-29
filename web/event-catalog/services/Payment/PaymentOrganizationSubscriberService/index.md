---
id: PaymentOrganizationSubscriberService
version: 0.0.1
name: Payment Organization Subscriber Service
summary: |
  Payment Service that handles all commands 
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

The Payment Organization Subscriber Service is a component of the system responsible for managing payment details. It interacts with other services to maintain accurate financial matters.

## Architecture diagram