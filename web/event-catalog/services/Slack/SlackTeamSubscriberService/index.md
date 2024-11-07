---
id: SlackTeamSubscriberService
version: 0.0.1
name: Slack  Team Subscriber Service
summary: |
  Slack Team Subscriber Service that handles all events 
owners:
    - malizadeh
    - full-stack
receives:
  - id: TeamUpserted
    version: 0.0.1
  - id: TeamDeleted
    version: 0.0.1
  - id: InvitationToJoinTeamDeleted
    version: 0.0.1
  - id: InvitationToJoinTeamUpserted
    version: 0.0.1
repository:
  language: C#
  url: 
---

## Overview

The Slack Team Subscriber Service is a component of the system responsible for managing Slack's team information. It interacts with other services to maintain accurate team data.

## Architecture diagram