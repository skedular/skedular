---
id: Slack
name: Slack
version: 0.0.1
owners:
    - malizadeh
services:
    - id: SlackInternalSubscriberService
      version: 0.0.1
    - id: SlackCustomerSubscriberService
      version: 0.0.1
    - id: SlackLocationSubscriberService
      version: 0.0.1
    - id: SlackOrganizationSubscriberService
      version: 0.0.1
    - id: SlackTeamSubscriberService
badges:
    - content: Team Domain
      backgroundColor: blue
      textColor: blue
---

## Overview

The Slack Domain encompasses all services and components related to handling Slack integration within the platform.

## Bounded context

<NodeGraph />

### Team example (sequence diagram)

```mermaid
sequenceDiagram
  
```

## Flows