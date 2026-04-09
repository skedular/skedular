# Location Domain Architecture

This document is a high-level architecture view of the Location domain as it exists today.

It is intentionally C4-style rather than implementation-complete. The goal is to explain:

- how locations, resources, and floor plans are managed
- how Temporal workflows drive analytics generation, booking-derived state recomputation, and location–product
  relationship computation
- how the location domain integrates with the booking, marketplace, and organization domains through Kafka events
- how location data feeds booking resource allocation

## Scope

This document covers the location domain surfaces under:

- `location/apis/Location.Api`
- `location/shared/Location.Shared`
- the location-owned Temporal workflows and activities

It also references the external systems and sibling domains that the location domain coordinates with:

- Booking API (gRPC — for booking snapshot queries)
- Kafka (event publication and consumption)
- Temporal (durable workflow execution)
- PostgreSQL (location database)
- Redis (cache)

---

## Purpose & Scope

The Location domain is the authoritative source of truth for:

- **Physical locations** — workspaces an organization owns or operates.
- **Resources** — individual bookable units (desks, rooms, amenities) inside a location.
- **Floor plans** — spatial layout metadata that maps resources to positions.
- **Location memberships and invitations** — joining a location triggers an email flow.
- **Booking-derived state** — rolling analytics computed from live booking data pulled from the Booking API.
- **Location–product relationships** — precomputed mapping of which marketplace products apply to which locations,
  derived from matching organization tags.

---

## Core Concepts

- `Location`  
  A physical workspace owned by an organization. Contains resources, floor plans, and accumulated analytics.

- `Resource`  
  A bookable unit inside a location (desk or room). Tagged with organization tags that control product eligibility.

- `Floor plan`  
  A spatial layout document that positions resources on a map inside a location.

- `DailyDeskCountRecording` / `DailyRoomCountRecording`  
  Daily capacity snapshot — count of active desks or rooms at the time of recording. Created by the
  `GenerateLocationDailyAnalytics` workflow.

- `DailyBookingCountRecording` / `DailyDeskBookingCountRecording` / `DailyRoomBookingCountRecording`  
  Daily booking totals derived from actual booking data. Created by the `RecomputeLocationBookingDerivedState`
  workflow.

- `PrecomputedLocationProduct`  
  A materialized join row that records which marketplace product applies to which location, based on matching
  organization tags. Created by the `ComputeOrganizationLocationsAndProductsRelationships` workflow.

- `Offering`  
  An organization-level subscription tier from `Api.Shared.Services.Offering` that controls the maximum number of
  locations and members allowed.

---

## System Context

```mermaid
flowchart LR
    Operator["Organization operator"] --> Web["Web UI"]
    Web --> LocationApi["Location API"]

    LocationApi --> LocationDomain["Location Shared Domain"]
    LocationDomain --> Temporal["Temporal workflows + activities"]
    LocationDomain --> Db["Location database (PostgreSQL)"]
    LocationDomain --> Cache["Redis cache"]

    Temporal --> BookingGrpc["Booking API (gRPC)"]
    Temporal --> Email["Email service"]

    LocationDomain --> Kafka["Kafka (Location events)"]
    Kafka --> BookingProc["Booking.Processors"]
    Kafka --> TeamProc["Team.Processors"]
    Kafka --> MsTeamsProc["MsTeams.Processors"]
    Kafka --> SlackProc["Slack.Processors"]
    Kafka --> CustomerProc["Customer.Processors"]

    KafkaIn["Kafka (Booking / Marketplace / Organization / Customer events)"] --> LocationProc["Location.Processors"]
    LocationProc --> LocationDomain
```

---

## Container View

```mermaid
flowchart TB
    subgraph API["Location API (location/apis/Location.Api)"]
        GQL["GraphQL subgraph\n(HotChocolate)"]
        REST["REST controller\n(LocationController)"]
        GRPC["gRPC service\n(LocationGrpcService)"]
        ApiSvc["API services\n(LocationService, ResourceService,\nFloorPlanService, LocationAnalyticsService,\nLocationOpeningHoursService,\nOrganizationPhysicalAddressService)"]
        OfferingSvc["OrganizationOfferingService\n(quota enforcement)"]
    end

    subgraph Shared["Location Shared (location/shared/Location.Shared)"]
        Models["Domain models\n(Location, Resource, FloorPlan,\nDailyRecordings, PrecomputedLocationProduct)"]
        Repos["Repositories\n(LocationRepository, ResourceRepository,\nFloorPlanRepository, PrecomputedLocationProductRepository,\nDailyCountRecordingRepositories)"]
        DB["LocationDbContext\n(PostgreSQL via EF Core)"]
        CacheSvc["Cache services\n(CachedLocationService,\nCachedOrganizationService,\nCachedCustomerService)"]
        WFService["TemporalService / TemporalOutboxService\nWorkflowIdService"]
        Publishers["LocationPublisher\nLocationOutboxPublisher"]
    end

    subgraph Workflows["Temporal workflows"]
        WF1["NewLocationJoined"]
        WF2["GenerateLocationDailyAnalytics"]
        WF3["RecomputeLocationBookingDerivedState"]
        WF4["ComputeOrganizationLocationsAndProductsRelationships"]
    end

    subgraph Activities["Temporal activities"]
        A1["EmailIntegrations\n(SendNewLocationJoinedEmailAsync)"]
        A2["LocationDailyAnalytics\n(RecordLocationDesksCountAsync,\nRecordLocationRoomsCountAsync)"]
        A3["LocationBookingDerivedState\n(RecomputeAsync)"]
        A4["LocationsProductsRelationships\n(ComputeLocationAndProductsRelationshipsAsync)"]
    end

    subgraph Processors["Location.Processors"]
        PB["BookingSubscriber"]
        PC["CustomerSubscriber"]
        PM["MarketplaceSubscriber"]
        PO["OrganizationSubscriber"]
    end

    API --> Shared
    Shared --> Workflows
    Workflows --> Activities
    Activities --> Repos
    Activities --> CacheSvc
    Processors --> Shared
```

---

## Directory Layout

```
location/
├── apis/
│   └── Location.Api/
│       ├── Controllers/LocationController.cs       # REST endpoints
│       ├── GraphQL/                                # HotChocolate subgraph
│       │   ├── Location/                           # Location queries/mutations
│       │   ├── Resource/                           # Resource queries/mutations
│       │   ├── FloorPlan/                          # Floor plan queries/mutations
│       │   ├── Analytics/                          # Analytics queries
│       │   ├── ContactedVia/                       # Contacted-via mutations
│       │   ├── PhysicalAddress/                    # Physical address mutations
│       │   └── Ownership/                          # Ownership mutations
│       ├── Grpc/LocationGrpcService.cs             # gRPC service
│       ├── Mappers/Mapper.cs
│       └── Services/
│           ├── Authorization/
│           │   ├── OrganizationAuthorizationService.cs
│           │   ├── OrganizationOfferingService.cs  # Quota enforcement
│           │   └── OrganizationSsoAuthorizationService.cs
│           ├── LocationService.cs
│           ├── ResourceService.cs
│           ├── FloorPlanService.cs
│           ├── LocationAnalyticsService.cs
│           └── LocationOpeningHoursService.cs
├── shared/
│   ├── Location.Shared/
│   │   ├── Activities/
│   │   │   ├── EmailIntegrations.cs
│   │   │   ├── LocationBookingDerivedState.cs
│   │   │   ├── LocationDailyAnalytics.cs
│   │   │   └── LocationsProductsRelationships.cs
│   │   ├── Database/
│   │   │   ├── Entities/                           # EF Core entities
│   │   │   ├── Migrations/
│   │   │   └── LocationDbContext.cs
│   │   ├── EmailTemplates/
│   │   │   └── NewLocationJoined.template.html/.txt
│   │   ├── Models/                                 # Domain models
│   │   ├── Publishers/
│   │   │   ├── LocationPublisher.cs
│   │   │   └── LocationOutboxPublisher.cs
│   │   ├── Repositories/
│   │   ├── Services/
│   │   │   ├── Cache/
│   │   │   │   ├── CachedLocationService.cs
│   │   │   │   ├── CachedOrganizationService.cs
│   │   │   │   └── CachedCustomerService.cs
│   │   │   ├── TemporalOutboxService.cs
│   │   │   ├── TemporalService.cs
│   │   │   └── WorkflowIdService.cs
│   │   └── Workflows/
│   │       ├── NewLocationJoined.cs
│   │       ├── GenerateLocationDailyAnalytics.cs
│   │       ├── RecomputeLocationBookingDerivedState.cs
│   │       ├── ComputeOrganizationLocationsAndProductsRelationships.cs
│   │       └── Constants.cs
│   └── Location.Infrastructure/
│       └── Services/MigrationService.cs
├── domain/
│   ├── Location.Domain.AppHost/AppHost.cs          # Aspire app host
│   ├── Location.Domain.FakeDependencies/
│   └── Location.Domain.IntegrationTests/
├── jobs/
│   └── Location.Jobs/                              # Scheduled jobs
└── processors/
    └── Location.Processors/
        └── Subscribers/
            ├── BookingSubscriber.cs
            ├── CustomerSubscriber.cs
            ├── MarketplaceSubscriber.cs
            └── OrganizationSubscriber.cs
```

---

## Temporal Workflow Details

### Workflow ID ownership

All workflow IDs are constructed by `WorkflowIdService` in `Location.Shared.Services`. This centralises prefix
definitions and ensures that production code and tests share the same ID-generation contract.

```
GenerateLocationDailyAnalytics-{locationId}
RecomputeLocationBookingDerivedState-{locationId}
ComputeLocationProductRelationships-{organizationId}
NewLocationJoined-{locationId}
```

---

### `NewLocationJoined`

Fires when a new location is created. Sends a welcome/notification email via the `EmailIntegrations` activity.

```mermaid
sequenceDiagram
    participant API as Location API
    participant Temporal as Temporal
    participant EA as EmailIntegrations activity
    participant Email as Email service

    API->>Temporal: StartWorkflow(NewLocationJoined, locationId)
    Temporal->>EA: SendNewLocationJoinedEmailAsync(locationId)
    EA->>EA: Load location from repository
    EA->>Email: SendRawEmailAsync(subject, text, html)
    Note over EA: MaxAttempts=3, StartToClose=1 min
    Temporal-->>API: Workflow complete
```

**Key behaviour:**
- Guards on `EmailConfiguration.EnableNewLocationJoinedEmail` — email can be disabled in configuration.
- Renders `NewLocationJoined.template.html` and `.txt` with `{{LOCATION_ID}}` and `{{LOCATION_NAME}}` substitution.
- Retries up to 3 times with a 1-minute maximum interval.

---

### `GenerateLocationDailyAnalytics`

A long-running, perpetual workflow that wakes daily to snapshot the current desk and room counts for a location.

```mermaid
flowchart TD
    Start["Workflow starts\nGenerateLocationDailyAnalyticsInput(locationId, generationTime?)"]
    Delay1["Optional initial delay\nuntil generationTime"]
    RecordDesks["Activity: RecordLocationDesksCountAsync\nCount active desk resources → DailyDeskCountRecording"]
    CheckDesks{Returns true?}
    RecordRooms["Activity: RecordLocationRoomsCountAsync\nCount active room resources → DailyRoomCountRecording"]
    CheckRooms{Returns true?}
    DailyDelay["Workflow.DelayAsync(24 hours)"]
    Exit["Workflow exits\n(location deleted)"]

    Start --> Delay1
    Delay1 --> RecordDesks
    RecordDesks --> CheckDesks
    CheckDesks -- "false (location deleted)" --> Exit
    CheckDesks -- "true" --> RecordRooms
    RecordRooms --> CheckRooms
    CheckRooms -- "false (location deleted)" --> Exit
    CheckRooms -- "true" --> DailyDelay
    DailyDelay --> RecordDesks
```

**Key behaviour:**
- Self-perpetuating: after each 24-hour delay it re-runs the recording activities in a `do/while(true)` loop.
- Activities return `false` when the location no longer exists, causing the workflow to exit cleanly.
- Each activity has `StartToCloseTimeout = 1 min` and retries up to 3 times.

---

### `RecomputeLocationBookingDerivedState`

A long-lived, signal-driven workflow that maintains up-to-date booking analytics for a location. The
`BookingSubscriber` processor signals it whenever a booking involving this location is created, updated, or deleted.

```mermaid
sequenceDiagram
    participant BookingProc as Booking.Processors\n(BookingSubscriber)
    participant Temporal as Temporal
    participant WF as RecomputeLocationBookingDerivedState
    participant Activity as LocationBookingDerivedState activity
    participant BookingGrpc as Booking API (gRPC)
    participant DB as Location database

    BookingProc->>Temporal: StartOrSignalWorkflow(RecomputeLocationBookingDerivedStateInput)
    Note over WF: Sets _recomputeRequested = true
    WF->>WF: Wait 10 seconds (debounce)
    WF->>Activity: RecomputeAsync(locationId)
    Activity->>BookingGrpc: Admin_GetPaginatedBookings(locationId, pageSize=1000)
    BookingGrpc-->>Activity: BookingSnapshot[] (paginated)
    Activity->>DB: Replace DailyBookingCountRecording\nDailyDeskBookingCountRecording\nDailyRoomBookingCountRecording
    Activity->>DB: Update location entity
    Activity-->>WF: Done
    WF->>WF: WaitConditionAsync(_recomputeRequested, 30 s timeout)
    Note over WF: If no new signal within 30 s → workflow exits\nIf new signal arrives → loop again
```

**Key behaviour:**
- Uses a `_recomputeRequested` flag and a `BookingChanged` signal to coalesce rapid successive booking changes into a
  single recompute.
- 10-second debounce delay prevents thundering-herd recompute on bulk booking operations.
- 30-second idle timeout causes the workflow to self-terminate when no further signals arrive, keeping Temporal state
  clean.
- Activity queries the Booking API over gRPC in pages of 1,000 bookings.
- `StartToCloseTimeout = 10 min` to allow large locations with many bookings to complete.

---

### `ComputeOrganizationLocationsAndProductsRelationships`

Triggered whenever a marketplace product is upserted (via `MarketplaceSubscriber`). Recomputes which locations in
an organization are eligible for each product, based on matching organization tags across location resources.

```mermaid
sequenceDiagram
    participant MktProc as Location.Processors\n(MarketplaceSubscriber)
    participant Temporal as Temporal
    participant Activity as LocationsProductsRelationships activity
    participant DB as Location database
    participant Cache as Redis cache

    MktProc->>Temporal: StartComputeOrganizationLocationsAndProductsRelationshipsAsync(organizationId)
    Temporal->>Activity: ComputeLocationAndProductsRelationshipsAsync(organizationId)
    Activity->>DB: GetProducts(organizationId)
    Activity->>DB: GetLocations(organizationId)
    Activity->>DB: GetExistingPrecomputedLocationProducts(organizationId)
    Note over Activity: For each product × location pair:\nIntersect product tag IDs with resource tag IDs.\nIf match exists → create PrecomputedLocationProduct.
    Activity->>DB: RemoveRange(existingPrecomputedLocationProducts)
    Activity->>DB: AddRange(newPrecomputedLocationProducts)
    Activity->>Cache: RemoveByIdAsync(affected locationIds)
    Activity-->>Temporal: Done
```

**Key behaviour:**
- Uses the first `ProductVersion`'s organization tags of type `Product` as the filter set.
- A location matches if any of its resources carries at least one matching product tag.
- Replaces all existing `PrecomputedLocationProduct` rows for the organization atomically.
- Invalidates the Redis cache for all affected locations so subsequent reads pick up fresh relationship data.
- `StartToCloseTimeout = 1 min`, retries up to 3 times with a 30-second maximum interval.

---

## Event Publication

The location domain publishes two Kafka event types via `LocationPublisher` (direct) and `LocationOutboxPublisher`
(transactional outbox):

| Event type        | Condition                        | Key field    |
|-------------------|----------------------------------|--------------|
| `LocationUpserted` | Location created or updated     | `locationId` |
| `LocationDeleted`  | Location soft-deleted           | `locationId` |

Both publishers emit to the `location.v1.event` Kafka topic.  
`LocationOutboxPublisher` writes event rows inside the same database transaction as the domain mutation, guaranteeing
at-least-once delivery.

### Consumers of location events

```mermaid
flowchart LR
    Kafka["Kafka\nlocation.v1.event"]

    Kafka --> BookingProc["Booking.Processors\nLocationSubscriber"]
    Kafka --> TeamProc["Team.Processors\nLocationSubscriber"]
    Kafka --> MsTeamsProc["MsTeams.Processors\nLocationSubscriber"]
    Kafka --> SlackProc["Slack.Processors\nLocationSubscriber"]
    Kafka --> CustomerProc["Customer.Processors\nLocationSubscriber"]
```

---

## Processor Subscriptions

`Location.Processors` consumes events from four Kafka topics:

```mermaid
flowchart LR
    subgraph KafkaTopics["Kafka topics consumed"]
        BK["booking.v1.event\n(BookingUpserted, BookingDeleted)"]
        CU["customer.v1.event\n(CustomerUpserted, CustomerDeleted)"]
        MK["marketplace.v1.event\n(ProductUpserted, ProductDeleted)"]
        ORG["organization.v1.event\n(OrganizationUpserted, OrganizationDeleted,\nOrganizationOfferingUpdated)"]
    end

    subgraph Processors["Location.Processors"]
        BS["BookingSubscriber"]
        CS["CustomerSubscriber"]
        MS["MarketplaceSubscriber"]
        OS["OrganizationSubscriber"]
    end

    BK --> BS
    CU --> CS
    MK --> MS
    ORG --> OS

    BS --> TS["TemporalService\n→ StartOrSignalWorkflow\n  RecomputeLocationBookingDerivedState"]
    CS --> Repo["LocationDbContext\nCustomer upsert/delete\nCache invalidation"]
    MS --> Repo2["LocationDbContext\nProduct + ProductVersion upsert/delete\n→ StartComputeOrganizationLocationsAndProductsRelationships"]
    OS --> Repo3["LocationDbContext\nOrganization + Members + Tags upsert/delete\nCache invalidation"]
```

### Subscriber responsibilities

| Subscriber | Events handled | Effect |
|---|---|---|
| `BookingSubscriber` | `BookingUpserted`, `BookingDeleted` | Signals `RecomputeLocationBookingDerivedState` for each involved location |
| `CustomerSubscriber` | `CustomerUpserted`, `CustomerDeleted` | Upserts/removes customer + identities in local DB; invalidates customer cache |
| `MarketplaceSubscriber` | `ProductUpserted`, `ProductDeleted` | Upserts/removes product + product versions in local DB; triggers `ComputeOrganizationLocationsAndProductsRelationships` |
| `OrganizationSubscriber` | `OrganizationUpserted`, `OrganizationDeleted`, `OrganizationOfferingUpdated` | Upserts/removes organization, members, tags, SSO settings in local DB; invalidates organization cache |

---

## Relationship to the Booking Domain

The location domain and the booking domain share a bidirectional data dependency:

```mermaid
flowchart LR
    subgraph Location["Location domain"]
        LocDB["Location database\n(locations, resources, floor plans)"]
        DerivedState["BookingDerivedState activity\n(daily booking counts per desk/room)"]
        PrecompLP["PrecomputedLocationProduct\n(location ↔ product join)"]
    end

    subgraph Booking["Booking domain"]
        BookingDB["Booking database\n(bookings, resources, schedules)"]
        BookingGrpc["Booking gRPC service\nAdmin_GetPaginatedBookings"]
    end

    subgraph Marketplace["Marketplace domain"]
        MktProduct["Product + ProductVersion\n(tags, pricing options)"]
    end

    BookingDB -- "BookingUpserted /\nBookingDeleted events via Kafka" --> Location
    DerivedState -- "gRPC paginated query" --> BookingGrpc
    BookingGrpc -- "BookingSnapshot[]" --> DerivedState
    DerivedState -- "stores daily counts" --> LocDB

    MktProduct -- "ProductUpserted events via Kafka" --> Location
    Location -- "recomputes tag intersection" --> PrecompLP
    PrecompLP -- "read by Location API\n(which products apply\nto this location)" --> LocDB
```

**Data flow summary:**

1. Booking domain publishes `BookingUpserted`/`BookingDeleted` events.
2. `Location.Processors.BookingSubscriber` receives these events and signals
   `RecomputeLocationBookingDerivedState` for each affected location.
3. The workflow's `LocationBookingDerivedState` activity calls the Booking gRPC API to retrieve a full
   paginated snapshot of all bookings for the location.
4. It then recomputes and replaces the daily booking count rows (`DailyBookingCountRecording`,
   `DailyDeskBookingCountRecording`, `DailyRoomBookingCountRecording`) in the Location database.
5. Booking resources are tagged with organization tags that also appear on marketplace products; the
   `ComputeOrganizationLocationsAndProductsRelationships` workflow builds the `PrecomputedLocationProduct` table from
   this intersection so the Location API can surface which products are available at each location without a live join.

---

## Reading Guide

| You want to understand… | Start here |
|---|---|
| How a location is created and what triggers the welcome email | `Location.Api/Services/LocationService.cs` → `NewLocationJoined` workflow |
| How daily desk/room capacity snapshots are generated | `GenerateLocationDailyAnalytics` workflow + `LocationDailyAnalytics` activity |
| How booking analytics are maintained per location | `RecomputeLocationBookingDerivedState` workflow + `LocationBookingDerivedState` activity |
| How marketplace products map to locations | `ComputeOrganizationLocationsAndProductsRelationships` workflow + `LocationsProductsRelationships` activity |
| How the domain reacts to booking events | `Location.Processors/Subscribers/BookingSubscriber.cs` |
| How the domain reacts to marketplace product changes | `Location.Processors/Subscribers/MarketplaceSubscriber.cs` |
| How organization quota limits (offering) are enforced | `Location.Api/Services/Authorization/OrganizationOfferingService.cs` |
| How location events are published to other domains | `Location.Shared/Publishers/LocationPublisher.cs` + `LocationOutboxPublisher.cs` |
| Workflow ID naming conventions | `Location.Shared/Services/WorkflowIdService.cs` + `Workflows/Constants.cs` |
