---
id: TeamCustomerSubscriberService
version: 0.0.1
name: Team Customer Subscriber Service
summary: |
  Team Service that handles all commands 
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

The Team Service is a component of the system responsible for managing team structure and roles. It interacts with other services to maintain accurate team members format.

## Architecture diagram