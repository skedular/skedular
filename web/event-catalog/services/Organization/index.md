---
id: OrganizationService
version: 0.0.1
name: Organization Service
summary: |
  Organization Service that handles all commands 
owners:
    - malizadeh
    - full-stack
receives:
  - id: AddOrganization
    version: 0.0.1
  - id: UpdateOrganization
    version: 0.0.1
  - id: DeleteOrganization
    version: 0.0.1
  - id: OrganizationPaymentMethodsUpdated
    version: 0.0.1
sends:
  - id: OrganizationDeleted
    version: 0.0.1
  - id: OrganizationUpserted
    version: 0.0.1
  - id: OrganizationOfferingUpdated
    version: 0.0.1    
repository:
  language: C#
  url: 
---

## Overview

The organization Service is a component of the system responsible for managing organization details.

## Architecture diagram

<NodeGraph title="Hello world" />