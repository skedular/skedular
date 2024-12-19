---
id: BillingService
version: 0.0.1
name: Billing Service
summary: |
  Billing Service that handles all commands 
owners:
    - malizadeh
    - full-stack  
sends:
  - id: OrganizationBillingInfoUpdated
    version: 0.0.1
  - id: BillingOrganizationOfferingUpserted
    version: 0.0.1  
repository:
  language: C#
---

## Overview

The Billing Service is a component of the system responsible for managing billings. It interacts with other services to maintain accurate organization's billing information.

## Architecture diagram