# Organization Domain Architecture

This document is a high-level architecture view of the Organization domain as it exists today.

It is intentionally C4-style rather than implementation-complete. The goal is to explain:

- how the organization domain anchors all other domains as the top-level tenant boundary
- how members, invitations, and offerings are managed
- how Temporal workflows drive onboarding, token refresh, offering renewal, Azure sync, and analytics
- how Stripe Connect accounts and Xero OAuth connections fit into the lifecycle
- how the domain reacts to events from Booking, Customer, and its own internal event stream

## Scope

This document covers the organization domain surfaces under:

- `organization/apis/Organization.Api`
- `organization/shared/Organization.Shared`
- `organization/processors/Organization.Processors`
- the organization-owned Temporal workflows and activities

It also references the external systems that the organization domain coordinates with:

- Stripe
- Xero
- Azure AD / Entra (via Microsoft Graph)
- Temporal
- Kafka

## Core Concepts

- `Organization`
    - The top-level tenant boundary in Skedular.
    - All locations, products, teams, and bookings belong to an organization.
    - Identified by either its internal ID or a human-readable custom domain.

- `OrganizationMember`
    - A customer who has joined an organization.
    - Members have roles (e.g. admin, member) and can complete onboarding steps.

- `JoinInvitation`
    - A pending or accepted invitation for a customer to join the organization.
    - Managed by the `InviteToJoinOrganization` Temporal workflow.

- `OrganizationOffering`
    - A feature license / subscription tier that the organization holds.
    - Renewals are scheduled via `ScheduleRenewOrganizationOffering`.

- `OrganizationStripeConnectAccount`
    - An organization's own Stripe account connected via OAuth.
    - Enables marketplace payments to flow into the organization's Stripe account.
    - Authorization state is maintained through Stripe webhook events.

- `OrganizationXeroConnection`
    - Stores the encrypted Xero OAuth refresh token for an organization.
    - Maintained by `MaintainOrganizationXeroConnection`, a long-lived Temporal workflow.

- `AzureTenant`
    - An Azure AD / Entra tenant linked to the organization for SSO and member sync.
    - Group membership is reconciled by `ReSyncAzureTenant`.

- `DailyBookingCountRecording` / `DailyMemberCountRecording`
    - Persisted analytics snapshots produced by `GenerateOrganizationDailyAnalytics`.

## Organization Field-Masked Updates

Organisation update contracts keep their normal `Update*` names. Their current organisation-domain implementation uses
field-masked patch semantics instead of full-object replacement: GraphQL setup editing calls `updateOrganization`, and
the specialised organisation GraphQL update mutations follow the same pattern for billing details, tax details, bank
accounts, offering selection, tags, Stripe Connect account metadata, Xero connection settings, and SSO settings.

Patch requests carry an explicit `fieldsToUpdate` enum list. The enum is the update mask: only fields named in that
list may change, and every omitted organisation value is preserved. This is required because nullable GraphQL input
values cannot distinguish "caller did not send this field" from "caller intentionally cleared this field".

The patch mapper owns applying selected setup fields to the organisation entity so the behaviour is reusable outside the
GraphQL resolver. It supports the editable setup fields previously sent by the GraphQL setup update path, including
name, description, title, subtitle, custom domain, public URLs, billing cycle, invoice due days, contact details, refund
notification recipients, industry subcategories, feature images, and marketplace listing metadata.

Successful field-masked update mutations return the existing payload shapes with the latest saved details. The setup UI
uses that payload to let Relay update other displayed fields without a success toast. While an inline text save is in
flight, the field shows an inline saving indicator; failures are surfaced through the existing toast path.

Concurrency remains owned by the entity layer. If persistence reports a concurrency conflict, the API reloads the latest
organisation and retries the same selected patch fields against that latest entity. The API does not expose an
`expectedVersion` argument.

Organisation gRPC billing details, tag, custom tag, product tag, and zone update RPCs use the same field-mask model.
Their RPCs keep their normal `Update*` names, but their inputs carry `fieldsToUpdate` and patch field enums so callers
cannot accidentally replace omitted values.

Organisation SSO settings use `updateOrganizationSsoSettings` with `fieldsToUpdate: [SSO_SETTINGS]` and still return
`OrganizationPayload`. SSO is patched as one aggregate field because entity id, login URL, federation metadata URL, and
active state are validated together against metadata and certificate checks.

## System Context

```mermaid
flowchart LR
    Operator["Organization operator"] --> Web["Web / Teams UI"]
    Member["Organization member"] --> Web

    Web --> OrgApi["Organization API"]
    OrgApi --> OrgShared["Organization.Shared"]
    OrgShared --> Temporal["Temporal workflows + activities"]
    OrgShared --> OrgDb["Organization database"]

    Temporal --> Stripe["Stripe"]
    Temporal --> Xero["Xero"]
    Temporal --> AzureAD["Azure AD / Entra (Graph API)"]

    OrgProc["Organization.Processors"] --> Kafka["Kafka"]
    Kafka --> OrgProc
    OrgShared --> Kafka
```

## Component Map

```mermaid
flowchart LR
    subgraph API["Organization.Api"]
        GQL["GraphQL subgraph\n(Organization, Member, Invitation,\nOffering, BankAccount, Billing,\nXero, Azure, Analytics)"]
        REST["OrganizationController\n(REST endpoints)"]
        ApiSvc["API-layer services\n(XeroConnectionService, mappers)"]
        GrpcSrv["gRPC server"]
    end

    subgraph Shared["Organization.Shared"]
        DomainSvc["Domain services\n(OrganizationMemberService,\nOrganizationDefaultValuesProvider,\nStripeConnectAccountLinkService,\nXeroTokenRefreshService)"]
        Cache["CachedOrganizationService\nCachedCustomerService"]
        Outbox["TemporalOutboxService\nWorkflowIdService"]
        Repos["Repositories + EF entities"]
        DB["PostgreSQL\n(Organization DB)"]
        Acts["Temporal activities\n(AzureTenantIntegrations,\nEmailIntegrations,\nInvitationIntegrations,\nOrganizationBookingDerivedState,\nOrganizationDailyAnalytics,\nOrganizationOfferings,\nStripeIntegrations,\nXeroIntegrations)"]
        Wfs["Temporal workflows\n(NewOrganizationJoined,\nInviteToJoinOrganization,\nAddOrganizationStripePaymentMethod,\nMaintainOrganizationXeroConnection,\nScheduleRenewOrganizationOffering,\nReSyncAzureTenant,\nGenerateOrganizationDailyAnalytics,\nRecomputeOrganizationBookingDerivedState)"]
        Pubs["Publishers\n(OrganizationPublisher,\nOrganizationOutboxPublisher,\nOrganizationInternalPublisher)"]
        Email["EmailTemplates"]
    end

    subgraph Processors["Organization.Processors"]
        BookingSub["BookingSubscriber"]
        CustomerSub["CustomerSubscriber"]
        OrgInternalSub["OrganizationInternalSubscriber"]
    end

    subgraph External["External systems"]
        TemporalSvc["Temporal cluster"]
        StripeSvc["Stripe"]
        XeroSvc["Xero"]
        GraphSvc["Azure AD Graph API"]
        KafkaSvc["Kafka"]
        RedisSvc["Redis (cache)"]
    end

    GQL --> ApiSvc
    REST --> ApiSvc
    GrpcSrv --> DomainSvc
    ApiSvc --> DomainSvc

    DomainSvc --> Repos
    DomainSvc --> Cache
    DomainSvc --> Outbox
    Repos --> DB
    Cache --> RedisSvc
    Outbox --> TemporalSvc
    Wfs --> Acts

    Acts --> StripeSvc
    Acts --> XeroSvc
    Acts --> GraphSvc
    Acts --> Repos
    Acts --> DomainSvc

    Pubs --> KafkaSvc
    BookingSub --> KafkaSvc
    CustomerSub --> KafkaSvc
    OrgInternalSub --> KafkaSvc
    OrgInternalSub --> Pubs
    CustomerSub --> Repos
```

## Temporal Workflow Overview

```mermaid
flowchart TB
    subgraph Onboarding["Onboarding"]
        NOW["NewOrganizationJoined\n▸ sends welcome email"]
    end

    subgraph Membership["Membership"]
        INV["InviteToJoinOrganization\n▸ sends invitation email\n▸ waits up to 7 days for response\n▸ expires invitation if no response"]
    end

    subgraph Payments["Payments"]
        SPM["AddOrganizationStripePaymentMethod\n▸ resolves Stripe SetupIntent\n▸ persists payment method\n▸ returns redirect URL"]
    end

    subgraph Accounting["Accounting"]
        XWF["MaintainOrganizationXeroConnection\n▸ long-lived ContinueAsNew loop\n▸ refreshes Xero OAuth token\n▸ schedules next refresh"]
    end

    subgraph Offerings["Offerings"]
        OWF["ScheduleRenewOrganizationOffering\n▸ waits until renewal date\n▸ cancellable via signal\n▸ charges Stripe then renews offering"]
    end

    subgraph AzureSync["Azure AD"]
        AWF["ReSyncAzureTenant\n▸ daily loop\n▸ syncs group membership\n▸ provisions/removes members"]
    end

    subgraph Analytics["Analytics"]
        GDA["GenerateOrganizationDailyAnalytics\n▸ daily loop\n▸ records member + booking counts"]
        RDS["RecomputeOrganizationBookingDerivedState\n▸ debounce 10 s\n▸ recomputes org-level stats\n▸ drains additional signals"]
    end
```

## Key Workflow Sequence Diagrams

### MaintainOrganizationXeroConnection — Long-lived token refresh loop

This workflow runs indefinitely for any organization that has connected Xero. It uses
`ContinueAsNew` to avoid unbounded history growth.

```mermaid
sequenceDiagram
    participant Org as Organization API
    participant Outbox as TemporalOutboxService
    participant WF as MaintainOrganizationXeroConnection
    participant XeroAct as XeroIntegrations (activity)
    participant DB as Organization DB
    participant Xero as Xero OAuth

    Org->>Outbox: schedule workflow on Xero connect
    Outbox->>WF: start(organizationId, notBefore?)

    loop ContinueAsNew loop
        WF->>WF: delay until notBefore (if set)
        WF->>XeroAct: RefreshOrganizationXeroConnectionAsync
        XeroAct->>DB: load OrganizationXeroConnection
        XeroAct->>Xero: POST /token (refresh_token grant)
        Xero-->>XeroAct: new access_token + refresh_token
        XeroAct->>DB: update encrypted tokens + expiry
        XeroAct-->>WF: RefreshOrganizationXeroConnectionResult\n(shouldContinue, nextRefreshAt)

        alt shouldContinue = true
            WF->>WF: ContinueAsNew(organizationId, nextRefreshAt)
        else token expired or disconnected
            WF->>DB: mark connection inactive + lastError
            WF->>WF: workflow ends
        end
    end
```

### InviteToJoinOrganization — Email invitation flow

```mermaid
sequenceDiagram
    participant Admin as Admin / API
    participant InvSvc as InvitationService
    participant Outbox as TemporalOutboxService
    participant WF as InviteToJoinOrganization
    participant DB as Organization DB
    participant EmailAct as EmailIntegrations (activity)
    participant InvAct as InvitationIntegrations (activity)
    participant Email as Email provider

    Admin->>InvSvc: InviteCustomersToJoinOrganization
    InvSvc->>DB: persist JoinInvitation (Pending)
    InvSvc->>Outbox: schedule InviteToJoinOrganization workflow

    Outbox->>WF: start(joinInvitationId, isNewCustomer)

    alt isNewCustomer = true
        WF->>EmailAct: SendInviteCustomerToJoinOrganizationNewCustomerAsync
    else existing customer
        WF->>EmailAct: SendInviteCustomerToJoinOrganizationExistingCustomerAsync
    end
    EmailAct->>Email: send invitation email with accept/reject link

    WF->>WF: WaitConditionAsync(invitationStateChanged, 7 days)

    alt Invitee accepts / rejects / admin cancels (within 7 days)
        Admin->>InvSvc: AcceptInvitation / RejectInvitation / CancelInvitation
        InvSvc->>DB: update JoinInvitation status
        InvSvc->>WF: signal InvitationStatusChangedAsync
        WF->>WF: condition satisfied → workflow ends
    else No response after 7 days
        WF->>InvAct: ExpireInvitationAsync
        InvAct->>DB: mark JoinInvitation as Expired
    end
```

## Stripe Connect Account Flow

Organizations can connect their own Stripe accounts so that marketplace payments flow
directly into their Stripe destination account.

```mermaid
sequenceDiagram
    participant Admin as Org Admin
    participant OrgApi as Organization API
    participant StripeSvc as Stripe
    participant WebhookController as OrganizationController\n(Stripe webhook)
    participant IntPub as OrganizationInternalPublisher
    participant Kafka as Kafka
    participant OrgIntSub as OrganizationInternalSubscriber
    participant DB as Organization DB
    participant OrgPub as OrganizationPublisher

    Admin->>OrgApi: initiate Stripe Connect OAuth
    OrgApi->>StripeSvc: create account link (OAuth)
    StripeSvc-->>Admin: redirect to Stripe onboarding

    Admin->>StripeSvc: complete Stripe Connect OAuth flow
    StripeSvc->>WebhookController: account.application.authorized webhook
    WebhookController->>IntPub: PublishStripeConnectAccountWebhookEventReceived
    IntPub->>Kafka: OrganizationInternal event

    Kafka->>OrgIntSub: StripeConnectAccountWebhookEventReceived
    OrgIntSub->>DB: upsert OrganizationStripeConnectAccountAuthorization (isAuthorized=true)
    OrgIntSub->>OrgPub: PublishOrganizationsAsync (updated org state)

    Note over StripeSvc,OrgIntSub: Deauthorize follows same path with isAuthorized=false\nand removes the connect account record
```

### Stripe Payment Method Setup

The `AddOrganizationStripePaymentMethod` workflow handles the async redirect-back flow from
Stripe's SetupIntent for storing a payment method on the organization.

```mermaid
sequenceDiagram
    participant Admin as Org Admin
    participant OrgApi as Organization API
    participant Outbox as TemporalOutboxService
    participant WF as AddOrganizationStripePaymentMethod
    participant StripeAct as StripeIntegrations (activity)
    participant Stripe as Stripe
    participant DB as Organization DB
    OrgApi->>Stripe: create SetupIntent
    Stripe-->>Admin: redirect to Stripe hosted form

    Admin->>Stripe: complete card entry
    Stripe-->>OrgApi: redirect back with setupIntentId + status

    OrgApi->>Outbox: schedule AddOrganizationStripePaymentMethod
    Outbox->>WF: start(organizationId, setupIntentId, redirectStatus)

    WF->>StripeAct: SetOrganizationPaymentMethodAsync
    StripeAct->>Stripe: retrieve SetupIntent → PaymentMethod
    StripeAct->>DB: persist OrganizationStripePaymentMethod
    StripeAct-->>WF: redirect URL

    WF-->>Admin: return redirect URL with status
```

## Azure AD Sync Flow

The `ReSyncAzureTenant` workflow runs daily to reconcile Azure AD group membership with
Skedular organization members. It provisions new members and removes members that have left
the group.

```mermaid
sequenceDiagram
    participant OrgApi as Organization API
    participant Outbox as TemporalOutboxService
    participant WF as ReSyncAzureTenant
    participant AzureAct as AzureTenantIntegrations (activity)
    participant Graph as Azure AD Graph API
    participant CustomerGrpc as Customer gRPC
    participant LocationGrpc as Location gRPC
    participant DB as Organization DB

    OrgApi->>Outbox: schedule ReSyncAzureTenant on tenant link
    Outbox->>WF: start(tenantId, reSyncTime?)

    loop Daily sync loop
        WF->>WF: delay until reSyncTime (first run only)
        WF->>AzureAct: ReSyncTenantAsync(tenantId)
        AzureAct->>DB: load AzureTenant + AzureTenantMembers
        AzureAct->>Graph: GET /groups/{groupId}/members
        Graph-->>AzureAct: current Azure AD member list

        AzureAct->>AzureAct: diff: additions, removals, updates
        AzureAct->>CustomerGrpc: provision / look up customers for new members
        AzureAct->>LocationGrpc: sync location memberships
        AzureAct->>DB: upsert / remove AzureTenantMember records
        AzureAct-->>WF: bool shouldContinue

        alt shouldContinue = true
            WF->>WF: delay 24 hours → next iteration
        else tenant deleted or disabled
            WF->>WF: workflow ends
        end
    end
```

## Event Publication and Processor Subscriptions

### Events produced by Organization domain

| Kafka topic              | When published                                                                 |
|--------------------------|--------------------------------------------------------------------------------|
| `Organization`           | Organization created, updated, or deleted; Stripe connect state changes        |
| `OrganizationMember`     | Member joined, role changed, status changed, removed, onboarding completed     |
| `OrganizationInternal`   | Stripe Connect webhook payload forwarded for async processing                  |

### Events consumed by Organization.Processors

| Kafka topic              | Event types handled                          | Action                                                                        |
|--------------------------|----------------------------------------------|-------------------------------------------------------------------------------|
| `Booking`                | `BookingUpserted`, `BookingDeleted`          | Signals `RecomputeOrganizationBookingDerivedState` workflow for each involved org |
| `Customer`               | `CustomerUpserted`, `CustomerDeleted`        | Mirrors customer + identity data locally; links pending invitations to newly-seen customers |
| `OrganizationInternal`   | `StripeConnectAccountWebhookEventReceived`   | Parses Stripe event; updates connect account authorization state; re-publishes org event |

```mermaid
flowchart LR
    subgraph Producers["Event producers → Kafka"]
        BookingDomain["Booking domain"]
        CustomerDomain["Customer domain"]
        StripeWebhook["Stripe webhook\n(via OrganizationController)"]
    end

    subgraph KafkaBus["Kafka"]
        BookingTopic["Booking topic"]
        CustomerTopic["Customer topic"]
        OrgInternalTopic["OrganizationInternal topic"]
        OrgTopic["Organization topic"]
        OrgMemberTopic["OrganizationMember topic"]
    end

    subgraph OrgProcessors["Organization.Processors"]
        BookingSub["BookingSubscriber"]
        CustomerSub["CustomerSubscriber"]
        OrgInternalSub["OrganizationInternalSubscriber"]
    end

    subgraph OrgShared["Organization.Shared"]
        TemporalOutbox["TemporalOutboxService\n(signals RecomputeBookingDerivedState)"]
        OrgPub["OrganizationPublisher"]
        OrgMemberPub["OrganizationMember publishers"]
    end

    BookingDomain --> BookingTopic
    CustomerDomain --> CustomerTopic
    StripeWebhook --> OrgInternalTopic

    BookingTopic --> BookingSub
    CustomerTopic --> CustomerSub
    OrgInternalTopic --> OrgInternalSub

    BookingSub --> TemporalOutbox
    CustomerSub --> OrgShared
    OrgInternalSub --> OrgPub

    OrgPub --> OrgTopic
    OrgMemberPub --> OrgMemberTopic
```

### Downstream consumers of Organization events

| Topic                  | Known consumers                                     |
|------------------------|-----------------------------------------------------|
| `Organization`         | Booking, Team, Location, Marketplace processors     |
| `OrganizationMember`   | Booking, Team processors                            |

## Reading Guide

| You want to understand…                              | Start here                                                                             |
|------------------------------------------------------|----------------------------------------------------------------------------------------|
| Overall component layout                             | [Component Map](#component-map)                                                        |
| How all workflows relate to each other               | [Temporal Workflow Overview](#temporal-workflow-overview)                              |
| Xero OAuth token lifecycle                           | [MaintainOrganizationXeroConnection](#maintainorganizationxeroconnection--long-lived-token-refresh-loop) |
| How invitations are sent and expire                  | [InviteToJoinOrganization](#invitetojoinorganization--email-invitation-flow)           |
| Stripe Connect account authorization                 | [Stripe Connect Account Flow](#stripe-connect-account-flow)                           |
| Payment method setup via SetupIntent                 | [Stripe Payment Method Setup](#stripe-payment-method-setup)                           |
| Azure AD group sync                                  | [Azure AD Sync Flow](#azure-ad-sync-flow)                                              |
| What events the domain produces and consumes         | [Event Publication and Processor Subscriptions](#event-publication-and-processor-subscriptions) |
| Xero token encryption boundary                       | `shared/Enterprise.Shared/Accounting` — `IXeroTokenEncryptionService`                 |
| Offering renewal / license billing                   | `Organization.Shared/Workflows/ScheduleRenewOrganizationOffering.cs`                  |
| Daily analytics recording                            | `Organization.Shared/Workflows/GenerateOrganizationDailyAnalytics.cs`                 |
| Booking stat recomputation (debounced)               | `Organization.Shared/Workflows/RecomputeOrganizationBookingDerivedState.cs`            |
