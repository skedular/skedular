---
id: PaymentCustomerSubscriberService
version: 0.0.1
name: Payment Customer Subscriber Service
summary: |
  Payment Customer Service Subscriber that handles all events 
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

The Payment Customer Subscriber Service is a component of the system responsible for managing payment details. It interacts with other services to maintain accurate financial matters.

## Architecture diagram