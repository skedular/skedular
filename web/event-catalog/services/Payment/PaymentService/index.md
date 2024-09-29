---
id: PaymentService
version: 0.0.1
name: Payment Service
summary: |
  Payment Service that handles all commands 
owners:
    - malizadeh
    - full-stack
receives:
  - id: AddOrganizationPaymentMethod
    version: 0.0.1
  - id: RemoveOrganizationPaymentMethod
    version: 0.0.1  
sends:
  - id: OrganizationPaymentMethodsUpdated
    version: 0.0.1
repository:
  language: C#
  url: 
---

## Overview

The Payment Service is a component of the system responsible for managing payment details. It interacts with other services to maintain accurate financial matters.

## Architecture diagram