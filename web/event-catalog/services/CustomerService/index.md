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
  - id: OrganizationUpserted
    version: 0.0.1
  - id: OrganizationDeleted
    version: 0.0.1
  - id: TeamUpserted
    version: 0.0.1
  - id: TeamDeleted
    version: 0.0.1
repository:
  language: C#
  url: 
---

## Overview

The Customer Service is a component of the system responsible for managing customer information. It interacts with other services to maintain accurate customer data.

## Architecture diagram

<NodeGraph title="Hello world" />