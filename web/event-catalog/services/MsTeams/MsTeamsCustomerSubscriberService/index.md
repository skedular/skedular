---
id: MsTeamsCustomerSubscriberService
version: 0.0.1
name: MsTeams Customer Subscriber Service
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
  url: 
---

## Overview

The Customer Service is a component of the system responsible for managing customer information. It interacts with other services to maintain accurate customer data.

## Architecture diagram