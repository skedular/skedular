---
id: BillingCustomerSubscriberService
version: 0.0.1
name: Billing Customer Subscriber Service
summary: |
  Billing Service Subscriber that handles all commands 
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

The Billing Service Subscriber is a component of the system responsible for managing customer information. It interacts with other services to maintain accurate customer information.

## Architecture diagram