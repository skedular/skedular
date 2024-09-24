---
id: CustomerService
version: 0.0.1
name: Customer Service
summary: |
  Customer Service that handles all commands 
owners:
    - malizadeh
    - full-stack
receives:
  - id: AddCustomerDefaultLocation
    version: 0.0.1
  - id: AddCustomerDefaultLocationTag
    version: 0.0.1
  - id: AddCustomerDefaultTeam
    version: 0.0.1
  - id: ClearCustomerDefaultOrganization
    version: 0.0.1
  - id: CompleteDefaultLocationOnboarding
    version: 0.0.1
  - id: CompleteDefaultOrganizationOnboarding
    version: 0.0.1
  - id: CompleteLocationOnboarding
    version: 0.0.1
  - id: CompleteOrganizationOnboarding
    version: 0.0.1
  - id: CompletePreferredDeskOnboarding
    version: 0.0.1
  - id: CompletePreferredZoneOnboarding
    version: 0.0.1
  - id: RemoveCustomerDefaultLocation
    version: 0.0.1
  - id: RemoveCustomerDefaultLocationTag
    version: 0.0.1
  - id: RemoveCustomerDefaultTeam
    version: 0.0.1
  - id: SetCustomerDefaultOrganization
    version: 0.0.1
sends:
  - id: CustomerUpserted
    version: 0.0.1
  - id: CustomerDeleted
    version: 0.0.1    
repository:
  language: C#
  url: 
---

## Overview

The Customer Service is a component of the system responsible for managing customer information. It interacts with other services to maintain accurate customer data.

## Architecture diagram

<NodeGraph title="Hello world" />