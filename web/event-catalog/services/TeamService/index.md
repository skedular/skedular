---
id: TeamService
version: 0.0.1
name: Team Service
summary: |
  Team Service that handles all commands 
owners:
    - malizadeh
    - full-stack
receives:
  - id: AddTeam
    version: 0.0.1
  - id: UpdateTeam
    version: 0.0.1
  - id: DeleteTeam
    version: 0.0.1
sends:
  - id: TeamUpserted
    version: 0.0.1
  - id: TeamDeleted
    version: 0.0.1
repository:
  language: C#
  url: 
---

## Overview

The Team Service is a component of the system responsible for managing team structure and roles. It interacts with other services to maintain accurate team members format.

## Architecture diagram

<NodeGraph title="Hello world" />