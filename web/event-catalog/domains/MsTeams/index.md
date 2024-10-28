---
id: MsTeam
name: MsTeam
version: 0.0.1
owners:
    - malizadeh
services:
    - id: MsTeamsCustomerSubscriberService
      version: 0.0.1
    - id: MsTeamsInternalSubscriberService
      version: 0.0.1
    - id: MsTeamsLocationSubscriberService
      version: 0.0.1
    - id: MsTeamsOrganizationSubscriberService
      version: 0.0.1
    - id: MsTeamsTeamSubscriberService
      version: 0.0.1
      
badges:
    - content: Team Domain
      backgroundColor: blue
      textColor: blue
---

## Overview

The MsTeam Domain encompasses all services and components related to handling Microsoft Teams integration.

## Bounded context

<NodeGraph />

### Team example (sequence diagram)

```mermaid
sequenceDiagram      

```

## Flows

### Add Team flow
<Flow id="AddTeamFlow" version="latest" includeKey={false} />

### Update Team flow
<Flow id="UpdatedTeamFlow" version="latest" includeKey={false} />

### Delete Team flow
<Flow id="DeleteTeamFlow" version="latest" includeKey={false} />