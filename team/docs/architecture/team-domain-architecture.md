# Team Domain Architecture

This document is a high-level architecture view of the Team domain as it exists today.

It is intentionally C4-style rather than implementation-complete. The goal is to explain:

- how teams are created and managed within an organization
- how team membership and invitations are handled
- how the Temporal workflow drives the invitation email lifecycle
- how the domain reacts to events from Customer, Location, and Organization
- how team membership is used by the Booking domain for private booking authorization

## Scope

This document covers the team domain surfaces under:

- `team/apis/Team.Api`
- `team/shared/Team.Shared`
- `team/processors/Team.Processors`
- the team-owned Temporal workflow and activities

It also references:

- the Booking domain, which consumes Team events for authorization checks
- the MS Teams and Slack integrations, which consume Team events for channel/group management

## Core Concepts

- `Team`
    - A named group of members within an organization.
    - Belongs to exactly one organization and optionally one location.
    - Used to scope private booking access.

- `TeamMember`
    - A member of a team, always a subset of the parent organization's members.
    - Has a role within the team (e.g. admin, member).

- `JoinInvitation`
    - A pending or accepted invitation for a customer to join a team.
    - Managed by the `InviteToJoinTeam` Temporal workflow.

- `OrganizationMember` (local read model)
    - A local projection of organization membership state, kept in sync via Kafka events.
    - Used to validate that a team member is still an active org member.

- `Location` (local read model)
    - A local projection of location data, kept in sync via Kafka events.
    - Teams may be scoped to a location; the local record ensures referential integrity.

## System Context

```mermaid
flowchart LR
    Operator["Organization operator"] --> Web["Web / Teams UI"]
    Member["Team member"] --> Web

    Web --> TeamApi["Team API"]
    TeamApi --> TeamShared["Team.Shared"]
    TeamShared --> Temporal["Temporal workflow + activities"]
    TeamShared --> TeamDb["Team database"]

    TeamProc["Team.Processors"] --> Kafka["Kafka"]
    Kafka --> TeamProc
    TeamShared --> Kafka

    TeamShared --> Email["Email provider"]
```

## Component Map

```mermaid
flowchart LR
    subgraph API["Team.Api"]
        GQL["GraphQL subgraph\n(Team, Member, Invitation)"]
        REST["TeamController\n(REST endpoints)"]
        GrpcSrv["gRPC server\n(TeamGrpcService)"]
        Mapper["Mappers"]
    end

    subgraph Shared["Team.Shared"]
        DomainSvc["Domain services"]
        Cache["CachedTeamService"]
        Outbox["TemporalOutboxService\nWorkflowIdService"]
        Repos["Repositories + EF entities\n(Team, TeamMember, JoinInvitation,\nOrganizationMember, Location, Customer)"]
        DB["PostgreSQL\n(Team DB)"]
        Acts["Temporal activities\n(EmailIntegrations,\nInvitationIntegrations)"]
        Wfs["Temporal workflow\n(InviteToJoinTeam)"]
        Pubs["Publishers\n(TeamPublisher,\nTeamOutboxPublisher)"]
        Email["EmailTemplates"]
        Cfg["Configurations\n(EmailConfiguration)"]
    end

    subgraph Processors["Team.Processors"]
        CustomerSub["CustomerSubscriber"]
        OrgSub["OrganizationSubscriber"]
        LocSub["LocationSubscriber"]
    end

    subgraph External["External systems"]
        TemporalSvc["Temporal cluster"]
        KafkaSvc["Kafka"]
        RedisSvc["Redis (cache)"]
        EmailSvc["Email provider"]
    end

    GQL --> DomainSvc
    REST --> DomainSvc
    GrpcSrv --> DomainSvc
    DomainSvc --> Repos
    DomainSvc --> Cache
    DomainSvc --> Outbox
    Repos --> DB
    Cache --> RedisSvc
    Outbox --> TemporalSvc

    Wfs --> Acts
    Acts --> EmailSvc
    Acts --> Repos

    Pubs --> KafkaSvc
    CustomerSub --> KafkaSvc
    OrgSub --> KafkaSvc
    LocSub --> KafkaSvc
    CustomerSub --> Repos
    OrgSub --> Repos
    LocSub --> Repos
```

## InviteToJoinTeam — Temporal Workflow Sequence

The `InviteToJoinTeam` workflow mirrors the organization invitation pattern. It sends
a typed invitation email (new customer vs. existing customer variant), waits up to 7 days
for a response signal, and expires the invitation if none arrives.

```mermaid
sequenceDiagram
    participant Admin as Admin / API
    participant InvSvc as Team invitation service
    participant DB as Team DB
    participant Outbox as TemporalOutboxService
    participant WF as InviteToJoinTeam
    participant EmailAct as EmailIntegrations (activity)
    participant InvAct as InvitationIntegrations (activity)
    participant Email as Email provider

    Admin->>InvSvc: InviteCustomersToJoinTeam
    InvSvc->>DB: persist JoinInvitation (Pending)
    InvSvc->>Outbox: schedule InviteToJoinTeam workflow
    Outbox->>WF: start(joinInvitationId, isNewCustomer)

    alt isNewCustomer = true
        WF->>EmailAct: SendInviteCustomerToJoinTeamNewCustomerAsync
    else existing customer
        WF->>EmailAct: SendInviteCustomerToJoinTeamExistingCustomerAsync
    end
    EmailAct->>Email: send invitation email with accept/reject link

    WF->>WF: WaitConditionAsync(invitationStateChanged, 7 days)

    alt Invitee responds or admin cancels (within 7 days)
        Admin->>InvSvc: AcceptInvitation / RejectInvitation / CancelInvitation
        InvSvc->>DB: update JoinInvitation status
        InvSvc->>WF: signal InvitationStatusChangedAsync
        WF->>WF: condition satisfied → workflow ends
    else No response after 7 days
        WF->>InvAct: ExpireInvitationAsync
        InvAct->>DB: mark JoinInvitation as Expired
    end
```

## Event Publication and Subscriptions

### Events produced by Team domain

| Kafka topic | When published                                                         |
|-------------|------------------------------------------------------------------------|
| `Team`      | Team created, updated, or deleted; member added, changed, or removed  |

### Events consumed by Team.Processors

| Kafka topic    | Event types handled                                                   | Action                                                                                             |
|----------------|-----------------------------------------------------------------------|----------------------------------------------------------------------------------------------------|
| `Customer`     | `CustomerUpserted`, `CustomerDeleted`                                 | Mirrors customer + identity data locally; links pending invitations to newly-seen email addresses  |
| `Organization` | `OrganizationUpserted`, `OrganizationDeleted`, `OrganizationOfferingUpdated` | Mirrors organization record + offering state locally; cascades delete to team members              |
| `Location`     | `LocationUpserted`, `LocationDeleted`                                 | Mirrors location record locally; cascades delete when location is removed                          |

```mermaid
flowchart LR
    subgraph Producers["Event producers → Kafka"]
        CustomerDomain["Customer domain"]
        OrgDomain["Organization domain"]
        LocDomain["Location domain"]
    end

    subgraph KafkaBus["Kafka"]
        CustomerTopic["Customer topic"]
        OrgTopic["Organization topic"]
        LocTopic["Location topic"]
        TeamTopic["Team topic"]
    end

    subgraph TeamProcessors["Team.Processors"]
        CustomerSub["CustomerSubscriber"]
        OrgSub["OrganizationSubscriber"]
        LocSub["LocationSubscriber"]
    end

    subgraph TeamShared["Team.Shared"]
        TeamPub["TeamPublisher\nTeamOutboxPublisher"]
        LocalDB["Team DB\n(local read models)"]
    end

    subgraph Consumers["Downstream consumers of Team events"]
        BookingProc["Booking.Processors"]
        MSTeamsProc["MSTeams.Processors"]
        SlackProc["Slack.Processors"]
    end

    CustomerDomain --> CustomerTopic
    OrgDomain --> OrgTopic
    LocDomain --> LocTopic

    CustomerTopic --> CustomerSub
    OrgTopic --> OrgSub
    LocTopic --> LocSub

    CustomerSub --> LocalDB
    OrgSub --> LocalDB
    LocSub --> LocalDB

    TeamPub --> TeamTopic
    TeamTopic --> BookingProc
    TeamTopic --> MSTeamsProc
    TeamTopic --> SlackProc
```

## Team Membership and Booking Authorization

Team membership acts as a gating mechanism for private booking access. The relationship is:

1. **Organization owns the tenant boundary.** A customer must be an organization member before
   they can be a team member.
2. **Team scopes booking access.** Private bookings on resources that belong to a team-restricted
   location are only accessible to active members of that team.
3. **Booking domain consumes Team events.** The Booking.Processors subscriber listens to the
   `Team` Kafka topic. When team membership changes (member added, removed, role changed), the
   booking domain updates its local projection of team membership used for authorization checks.
4. **No direct cross-domain calls at booking time.** The booking domain does not call the Team
   API at booking time. It uses its own local team-membership projection to evaluate whether the
   caller is authorized to book a team-restricted resource slot.

```mermaid
flowchart TD
    subgraph OrgBoundary["Organization boundary"]
        OrgMember["OrganizationMember\n(must exist)"]
    end

    subgraph TeamBoundary["Team (optional scope)"]
        TeamMember["TeamMember\n(subset of org members)"]
    end

    subgraph BookingBoundary["Booking domain (local projection)"]
        BookingTeamMember["Local TeamMember read model\n(kept in sync via Team Kafka events)"]
        AuthCheck["Private booking authorization\n(checks local read model)"]
    end

    OrgMember -->|prerequisite| TeamMember
    TeamMember -->|Team Kafka event| BookingTeamMember
    BookingTeamMember --> AuthCheck
    AuthCheck -->|authorized| Booking["Booking created / modified"]
    AuthCheck -->|unauthorized| Denied["Request rejected"]
```

## Reading Guide

| You want to understand…                            | Start here                                                                 |
|----------------------------------------------------|----------------------------------------------------------------------------|
| Overall component layout                           | [Component Map](#component-map)                                            |
| Invitation email lifecycle                         | [InviteToJoinTeam workflow](#invitetojointeam--temporal-workflow-sequence) |
| What events the domain produces and consumes       | [Event Publication and Subscriptions](#event-publication-and-subscriptions)|
| How team membership restricts booking access       | [Team Membership and Booking Authorization](#team-membership-and-booking-authorization) |
| Team invitation workflow implementation            | `team/shared/Team.Shared/Workflows/InviteToJoinTeam.cs`                    |
| Kafka subscriber implementations                  | `team/processors/Team.Processors/Subscribers/`                             |
| gRPC team query surface                            | `team/apis/Team.Api/Grpc/TeamGrpcService.cs`                               |
