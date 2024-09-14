---
id: Team
name: Team
version: 0.0.1
owners:
    - malizadeh
services:
    - id: TeamService
      version: 0.0.1
badges:
    - content: Team Domain
      backgroundColor: blue
      textColor: blue
---

## Overview

The Team Domain encompasses all services and components related to handling teams within the organization.

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