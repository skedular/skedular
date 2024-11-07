---
id: SlackCustomerSubscriberService
version: 0.0.1
name: Slack Customer Subscriber Service
summary: |
  Slack Customer Subscriber Service that handles all events 
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

The Slack Customer Subscriber Service is a component of the system responsible for managing customers. It interacts with other services to maintain accurate slack customers.

## Architecture diagram