---
id: NotificationCustomerSubscriberService
version: 0.0.1
name: Notification Customer Subscriber Service
summary: |
  Notification Customer Subscriber Service that handles all events 
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

The Notification Customer Subscriber Service is a component of the system responsible for managing notifications. It interacts with other services to maintain accurate notifications.

## Architecture diagram