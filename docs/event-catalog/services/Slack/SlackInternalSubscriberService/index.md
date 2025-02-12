---
id: SlackInternalSubscriberService
version: 0.0.1
name: Slack Internal Subscriber Service
summary: |
  Notification Team Subscriber Service that handles all events 
owners:
    - malizadeh
    - full-stack
receives:
  - id: DeactivateOrganizationMembersNotFoundOnSlack
    version: 0.0.1
  - id: RefreshWorkspaceChannels
    version: 0.0.1
  - id: RefreshWorkspaceMembers
    version: 0.0.1
  - id: SendWorkspaceLocationDailyUpdateMessage
    version: 0.0.1
  - id: SendWorkspaceTeamDailyUpdateMessage
    version: 0.0.1
  - id: UpdateWorkspaceMemberProfileStatus
    version: 0.0.1
repository:
  language: C#
---

## Overview

The Slack Internal Subscriber Service is a component of the system responsible for managing slack internal information. It interacts with other services to maintain accurate slack data.

## Architecture diagram