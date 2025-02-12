---
id: BillingInternalSubscriberService
version: 0.0.1
name: Billing Internal Subscriber Service
summary: |
  Billing Internal Subscriber Service that handles all events related to billing for an organization.
owners:
    - malizadeh
    - full-stack
receives:
  - id: GenerateOrganizationOfferingInvoice
    version: 0.0.1
repository:
  language: C#
---

## Overview

The Billing Internal Subscriber Service is a component of the system responsible for managing organization billings. It interacts with other services to maintain accurate billing information.

## Architecture diagram