---
id: NotificationSubscriberService
version: 0.0.1
name: Notification Subscriber Service
summary: |
  Notification Subscriber Service that handles all events 
owners:
    - malizadeh
    - full-stack
receives:
  - id: NotificationUpserted
    version: 0.0.1
repository:
  language: C#
  url: 
---

## Overview

The Notification Subscriber Service is a component of the system responsible for sending email notification raised by other domains.

## Architecture diagram