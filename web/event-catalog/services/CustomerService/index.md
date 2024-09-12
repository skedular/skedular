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
  - id: CustomerUpserted
    version: 0.0.1
  - id: CustomerDeleted
    version: 0.0.1
repository:
  language: C#
  url: 
---

## Overview

The Team Service is a component of the system responsible for managing team structure and roles. It interacts with other services to maintain accurate team members format.

## Architecture diagram

<NodeGraph title="Hello world" />