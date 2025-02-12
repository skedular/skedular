---
id: Team
name: Team
version: 0.0.1
owners:
    - malizadeh
services:
    - id: TeamService
      version: 0.0.1
    - id: TeamBookingSubscriberService
      version: 0.0.1
    - id: TeamCustomerSubscriberService
      version: 0.0.1
    - id: TeamOrganizationSubscriberService
      version: 0.0.1
    - id: CustomerTeamSubscriberService
      version: 0.0.1
    - id: BookingTeamSubscriberService
      version: 0.0.1
    - id: OrganizationTeamSubscriberService
      version: 0.0.1
    - id: NotificationTeamSubscriberService
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
    participant User
    participant TeamService
    participant CustomerService
    participant OrganizationService
    participant BookingService

    User->>TeamService: AddTeam
    TeamService-->>CustomerService: TeamUpersted
    TeamService-->>OrganizationService: TeamUpersted
    TeamService-->>BookingService: TeamUpersted
    TeamService->>User: Team Added
    
    User->>TeamService: UpdateTeam
    TeamService-->>CustomerService: TeamUpersted
    TeamService-->>OrganizationService: TeamUpersted
    TeamService-->>BookingService: TeamUpersted
    TeamService->>User: Team Updated   
    
    CustomerService-->>TeamService: CustomerUpserted
    CustomerService-->>TeamService: CustomerDeleted
    OrganizationService-->>TeamService: OrganizationUpserted
    OrganizationService-->>TeamService: OrganizationDeleted
    BookingService-->>TeamService: BookingUpserted
    BookingService-->>TeamService: BookingDeleted
    
    

```

## Flows

### Add Team flow
<Flow id="AddTeamFlow" version="latest" includeKey={false} />

### Update Team flow
<Flow id="UpdatedTeamFlow" version="latest" includeKey={false} />

### Delete Team flow
<Flow id="DeleteTeamFlow" version="latest" includeKey={false} />