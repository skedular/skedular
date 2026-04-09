# Skedular Platform — Scheduler Overview

This document gives a cross-domain view of the entire Skedular platform: how the ten
microservice domains relate to each other, how Kafka events flow between them, how
Temporal workflows orchestrate long-running operations, and how a customer booking
request travels end-to-end from the browser to Stripe/Xero and back.

Read it alongside the per-domain architecture documents under each domain's `docs/`
folder and the existing C4 diagrams under `docs/architecture/`.

---

## 1. System Context

All external actors, domains, shared infrastructure, and external systems in one view.

```mermaid
flowchart LR
    subgraph Clients["Client layer"]
        Web["Web app\n(React / Relay)"]
        MsTeamsApp["MS Teams\ntab app"]
        SlackApp["Slack app"]
    end

    subgraph Gateway["Gateway layer"]
        GW["gateway\nHotChocolate Fusion\n+ YARP reverse proxy"]
    end

    subgraph Domains["Domain microservices"]
        BookingApi["booking API"]
        CoreApi["core API"]
        CustomerApi["customer API"]
        LocationApi["location API"]
        MarketplaceApi["marketplace API"]
        OrgApi["organization API"]
        TeamApi["team API"]
        MsTeamsApi["msteams API"]
        SlackApi["slack API"]
    end

    subgraph Processors["Event processors"]
        BookingProc["booking\nprocessors"]
        CustomerProc["customer\nprocessors"]
        LocationProc["location\nprocessors"]
        MarketplaceProc["marketplace\nprocessors"]
        OrgProc["organization\nprocessors"]
        TeamProc["team\nprocessors"]
        MsTeamsProc["msteams\nprocessors"]
        SlackProc["slack\nprocessors"]
    end

    subgraph Infra["Shared infrastructure"]
        Kafka["Kafka\nmessage bus"]
        Temporal["Temporal\nworkflow engine"]
        Redis["Redis\ncache"]
        PG["PostgreSQL\n(per-domain DB)"]
    end

    subgraph External["External services"]
        Stripe["Stripe\npayments"]
        Xero["Xero\naccounting"]
        WorkOS["WorkOS\nSSO / directory"]
        AzureAD["Azure Entra ID\n/ Graph API"]
    end

    Web --> GW
    MsTeamsApp --> GW
    SlackApp --> GW

    GW --> BookingApi
    GW --> CoreApi
    GW --> CustomerApi
    GW --> LocationApi
    GW --> MarketplaceApi
    GW --> OrgApi
    GW --> TeamApi
    GW --> MsTeamsApi
    GW --> SlackApi

    BookingApi --> Temporal
    CustomerApi --> Temporal
    LocationApi --> Temporal
    OrgApi --> Temporal
    TeamApi --> Temporal
    MsTeamsApi --> Temporal
    SlackApi --> Temporal

    Domains --> Kafka
    Kafka --> Processors

    Temporal --> Stripe
    Temporal --> Xero
    Temporal --> AzureAD

    OrgApi --> WorkOS
    CoreApi --> WorkOS

    Domains --> PG
    Domains --> Redis
    Processors --> PG
```

---

## 2. Domain Interaction Map — Kafka Event Flow

Each domain publishes events on its topic(s); other domains' processor services subscribe
to the topics they care about.  Arrows point from **publisher → subscriber**.

```mermaid
flowchart TB
    subgraph Publishers["Event publishers (domains)"]
        BK["booking\n(Booking / BookingInternal)"]
        CU["customer\n(Customer)"]
        LO["location\n(Location)"]
        MK["marketplace\n(Marketplace)"]
        OR["organization\n(Organization / OrganizationInternal\n/ OrganizationMember)"]
        TM["team\n(Team)"]
    end

    subgraph Subscribers["Event subscribers (processors)"]
        BKP["booking\nprocessors"]
        CUP["customer\nprocessors"]
        LOP["location\nprocessors"]
        MKP["marketplace\nprocessors"]
        ORP["organization\nprocessors"]
        TMP["team\nprocessors"]
        MSP["msteams\nprocessors"]
        SLP["slack\nprocessors"]
    end

    BK -->|"BookingInternal"| BKP
    BK -->|"Booking"| LOP

    CU -->|"Customer"| BKP
    CU -->|"Customer"| CUP
    CU -->|"Customer"| LOP
    CU -->|"Customer"| MKP
    CU -->|"Customer"| ORP
    CU -->|"Customer"| TMP
    CU -->|"Customer"| MSP
    CU -->|"Customer"| SLP

    LO -->|"Location"| BKP
    LO -->|"Location"| CUP
    LO -->|"Location"| LOP
    LO -->|"Location"| TMP
    LO -->|"Location"| MSP
    LO -->|"Location"| SLP

    MK -->|"Marketplace"| BKP
    MK -->|"Marketplace"| LOP
    MK -->|"Marketplace"| MKP

    OR -->|"Organization"| BKP
    OR -->|"Organization"| CUP
    OR -->|"Organization"| LOP
    OR -->|"Organization"| MKP
    OR -->|"Organization"| ORP
    OR -->|"Organization"| TMP
    OR -->|"Organization"| MSP
    OR -->|"Organization"| SLP
    OR -->|"OrganizationInternal"| ORP

    TM -->|"Team"| BKP
    TM -->|"Team"| TMP
    TM -->|"Team"| MSP
    TM -->|"Team"| SLP
```

### Topic subscription summary

| Processor domain | Subscribed topics |
|---|---|
| booking | BookingInternal, Location, Organization, Customer, Marketplace, Team |
| customer | Location, Organization |
| location | Booking, Customer, Marketplace, Organization |
| marketplace | Customer, Organization |
| organization | Booking, Customer, OrganizationInternal |
| team | Customer, Location, Organization |
| msteams | Customer, Location, Organization, Team |
| slack | Customer, Location, Organization, Team |

---

## 3. Temporal Workflow Map

All Temporal workflows grouped by owning domain.

```mermaid
flowchart LR
    subgraph booking["booking domain"]
        BW1["ProcessMarketplaceBookingPayment"]
        BW2["BookMarketplaceBookingSubscriptionResources"]
        BW3["ProcessMarketplaceBookingArrears"]
        BW4["CancelMarketplaceBookingSubscription"]
        BW5["RefundMarketplaceBooking"]
        BW6["ProcessXeroRepeatingInvoiceWebhook"]
    end

    subgraph customer["customer domain"]
        CW1["NewCustomerJoined"]
        CW2["AddCustomerStripePaymentMethod"]
        CW3["SubmitCustomerFeedback"]
    end

    subgraph location["location domain"]
        LW1["NewLocationJoined"]
        LW2["GenerateLocationDailyAnalytics"]
        LW3["RecomputeLocationBookingDerivedState"]
        LW4["ComputeOrganizationLocationsAndProductsRelationships"]
    end

    subgraph organization["organization domain"]
        OW1["NewOrganizationJoined"]
        OW2["InviteToJoinOrganization"]
        OW3["AddOrganizationStripePaymentMethod"]
        OW4["MaintainOrganizationXeroConnection"]
        OW5["ScheduleRenewOrganizationOffering"]
        OW6["ReSyncAzureTenant"]
        OW7["GenerateOrganizationDailyAnalytics"]
        OW8["RecomputeOrganizationBookingDerivedState"]
    end

    subgraph team["team domain"]
        TW1["InviteToJoinTeam"]
    end

    subgraph msteams["msteams domain"]
        MW1["ReSyncMsTeams"]
    end

    subgraph slack["slack domain"]
        SW1["NewSlackWorkspaceJoined"]
        SW2["ReSyncSlackWorkspace"]
    end

    subgraph ext["External systems called by workflows"]
        Stripe["Stripe"]
        Xero["Xero"]
        AzureAD["Azure Entra ID\n/ Graph API"]
        Email["Email service"]
    end

    BW1 --> Stripe
    BW1 --> Xero
    BW3 --> Xero
    BW5 --> Xero
    BW6 --> Xero

    CW2 --> Stripe
    OW3 --> Stripe
    OW4 --> Xero
    OW6 --> AzureAD
    MW1 --> AzureAD

    OW2 --> Email
    TW1 --> Email
    CW1 --> Email
```

---

## 4. GraphQL Federation Diagram

The gateway fuses subgraph schemas from all domain APIs using HotChocolate Fusion.

```mermaid
flowchart TB
    subgraph GW["gateway — HotChocolate Fusion + YARP"]
        Schema["Composed\nfederated schema"]
        Router["YARP reverse\nproxy routes"]
    end

    subgraph Subgraphs["Domain GraphQL subgraphs"]
        S1["booking subgraph\n/v1/graphql"]
        S2["core subgraph\n/v1/graphql"]
        S3["customer subgraph\n/v1/graphql"]
        S4["location subgraph\n/v1/graphql"]
        S5["marketplace subgraph\n/v1/graphql"]
        S6["organization subgraph\n/v1/graphql"]
        S7["team subgraph\n/v1/graphql"]
        S8["msteams subgraph\n/v1/graphql"]
        S9["slack subgraph\n/v1/graphql"]
    end

    Client["Web / mobile\nclient"] -->|"GraphQL over HTTP"| Schema
    Schema -->|"Fusion plan execution"| S1
    Schema -->|"Fusion plan execution"| S2
    Schema -->|"Fusion plan execution"| S3
    Schema -->|"Fusion plan execution"| S4
    Schema -->|"Fusion plan execution"| S5
    Schema -->|"Fusion plan execution"| S6
    Schema -->|"Fusion plan execution"| S7
    Schema -->|"Fusion plan execution"| S8
    Schema -->|"Fusion plan execution"| S9

    Router -->|"REST / gRPC passthrough"| S1
    Router -->|"REST / gRPC passthrough"| S2
    Router -->|"REST / gRPC passthrough"| S3
    Router -->|"REST / gRPC passthrough"| S4
    Router -->|"REST / gRPC passthrough"| S5
    Router -->|"REST / gRPC passthrough"| S6
    Router -->|"REST / gRPC passthrough"| S7
    Router -->|"REST / gRPC passthrough"| S8
    Router -->|"REST / gRPC passthrough"| S9
```

---

## 5. Request Lifecycle — Booking a Marketplace Resource

End-to-end sequence for a customer booking a marketplace product (card payment path).

```mermaid
sequenceDiagram
    autonumber
    actor Customer
    participant Web as Web App
    participant GW as Gateway
    participant BookingApi as Booking API
    participant BookingShared as Booking.Shared
    participant OrgApi as Organization API
    participant DB as Booking DB
    participant Outbox as Temporal Outbox
    participant Temporal as Temporal cluster
    participant Stripe as Stripe
    participant Xero as Xero
    participant Kafka as Kafka
    participant LocationProc as Location Processor
    participant OrgProc as Organization Processor

    Customer->>Web: Select product & confirm booking
    Web->>GW: GraphQL mutation createMarketplaceBooking
    GW->>BookingApi: Route to Booking subgraph
    BookingApi->>BookingShared: CreateMarketplaceBookingAsync(command)
    BookingShared->>OrgApi: Fetch org billing settings & invoice days
    BookingShared->>DB: Persist MarketplaceBooking (PaymentPending)
    BookingShared->>Outbox: Enqueue ProcessMarketplaceBookingPayment workflow
    BookingShared-->>BookingApi: Booking ID + pending state
    BookingApi-->>GW: Booking created (pending payment)
    GW-->>Web: Booking ID + status
    Web-->>Customer: "Processing payment…"

    Note over Temporal: Temporal picks up outbox entry
    Temporal->>Stripe: Create/confirm PaymentIntent
    Stripe-->>Temporal: Payment succeeded
    Temporal->>BookingShared: ConfirmPaymentActivity → update PaymentStatus=Confirmed
    BookingShared->>DB: Persist payment confirmation
    Temporal->>Xero: CreateInvoiceActivity → export invoice
    Xero-->>Temporal: Invoice ID
    Temporal->>DB: Persist AccountingInvoiceExportLink

    Temporal->>Kafka: Publish BookingUpserted event (booking.v1.event, PaymentStatus=Confirmed)
    Kafka->>LocationProc: BookingUpserted → update location occupancy derived state
    Kafka->>OrgProc: BookingUpserted → update org booking derived state

    Note over Web: GraphQL subscription / polling
    Web->>GW: Query booking status
    GW->>BookingApi: Fetch booking
    BookingApi-->>GW: PaymentStatus=Confirmed
    GW-->>Web: Payment confirmed
    Web-->>Customer: Booking confirmed ✓
```

---

## 6. Reading Guide

| Document | Location | What it covers |
|---|---|---|
| Scheduler overview (this file) | `docs/architecture/scheduler-overview.md` | Cross-domain Kafka flows, Temporal workflows, request lifecycle |
| Booking domain architecture | `booking/docs/architecture/booking-domain-architecture.md` | Payment, Xero invoicing, refunds, subscriptions |
| Booking runtime flows | `booking/docs/architecture/booking-runtime-flows.md` | Temporal workflow state machines |
| System context (C4 L1) | `docs/architecture/level-1-system-context/` | Actor ↔ platform context |
| Container diagram (C4 L2) | `docs/architecture/level-2-container-diagram/` | Domain containers |
| Domain component diagrams (C4 L3) | `docs/architecture/level-3-component-diagram-*/` | Per-domain component breakdowns |
| Shared libraries architecture | `shared/docs/architecture/shared-libraries-architecture.md` | All shared libraries and their relationships |
| Xero bank transfer integration | `docs/architecture/xero-bank-transfer-integration.md` | Xero bank transfer specifics |
| Event catalog | `docs/event-catalog/` | Per-topic event schemas |
| ADR index | `docs/adr-index.md` | Architecture decision records |
