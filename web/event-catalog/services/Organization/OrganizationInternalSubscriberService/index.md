---
id: OrganizationInternalSubscriberService
version: 0.0.1
name: Organization Internal Subscriber Service
summary: |
  Organization Internal Subscriber Service that handles all events 
owners:
    - malizadeh
    - full-stack
receives:
  - id: RefreshAzureTenantMembers
    version: 0.0.1
  - id: RecordDailyMemberCount
    version: 0.0.1
  - id: RenewOrganizationOffering
    version: 0.0.1
repository:
  language: C#
---

## Overview

The Organization Internal Subscriber Service is a component of the system responsible for internal jobs in Organization domains.

## Architecture diagram