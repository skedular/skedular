# Implementation Plan: Desk Availability Analytics

**Branch**: `006-desk-availability-analytics` | **Date**: 2026-04-29 | **Spec**: [spec.md](spec.md)  
**Input**: Feature specification from `specs/006-desk-availability-analytics/spec.md`

## Summary

Extend the location domain's existing daily analytics infrastructure to capture per-desk
availability snapshots (available / unavailable / booked) once per UTC day per location. Add
a new GraphQL query that returns these per-day per-desk results for a configurable date range
(key use case: six months). Fix three confirmed bugs in the existing `LocationBookingDerivedState`
activity: cancelled bookings not excluded, dual-tagged resource double-counting, and zero-capacity
occupancy producing misleading 0% values. New snapshot logic runs inside the existing
`GenerateLocationDailyAnalytics` Temporal workflow via a new activity; backfill uses the same
activity triggered for a date range via the existing REST regeneration endpoints.

## Technical Context

**Language/Version**: C# on .NET 10  
**Primary Dependencies**: Temporal (workflows/activities), HotChocolate (GraphQL), Entity Framework Core, gRPC (booking data via `BookingService.BookingServiceClient`), `Enterprise.Shared.Database` repository pattern, `IRepositoryFactory`, `IWorkflowIdService`  
**Storage**: PostgreSQL via EF Core — new `DailyDeskAvailabilitySnapshot` table; new migration required in `location/shared/Location.Shared/Database/Migrations/`  
**Testing**: xUnit (C# backend) — unit tests in `Location.Shared.UnitTests` and `Location.Api.UnitTests`; integration tests in `Location.Api.IntegrationTests` and `Location.Processors.IntegrationTests`  
**Target Platform**: Linux server (Aspire-hosted `Location.Api` and `Location.Jobs`)  
**Project Type**: Backend microservice domain extension + web UI component  
**Performance Goals**: Snapshot activity for up to 500 desks per location must complete within the 1-minute `StartToCloseTimeout` already used by sibling activities; six-month date-range query must not timeout  
**Constraints**: No new cross-domain DB access — booking data continues to be fetched via gRPC. No direct `DbContext` access in integration tests. Snapshots are immutable once written (replace-on-regeneration only).  
**Frontend**: TypeScript 6, React 19, Next.js 16 (App Router), Relay, MUI v9, `@mui/x-charts` — new `LocationDeskAvailabilityInsight` component pair added to the existing Location Insights analytics section following the `LocationDeskOccupancyInsight` pattern.  
**Scale/Scope**: Per-location, per-day granularity; six-month query range (~180 rows × desk count per location per query)

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — This feature touches two contract surfaces: 1. **GraphQL**: A new `deskAvailabilitySnapshots` field (type `[DeskAvailabilityDailySnapshot!]!`) is added to the existing `LocationAnalytics` type in `Location.Api`. This requires running `scripts/generate-graphql.sh` after the resolver is wired. The schema export and composed gateway schema must be regenerated. No hand-edits to generated schema files. 2. **OpenAPI (REST)**: The existing `location_analytics_v1.yaml` needs a new endpoint for backfill by date range. After adding the YAML, `api-definitions/openapi/generate.sh` must be run to regenerate the controller base and client. The existing two endpoints are unchanged.
      The correct generator scripts are identified. No generated outputs will be hand-edited.

- [x] **II. Domain Boundaries** — The new snapshot activity needs booking data (to classify desks as booked). This follows the existing cross-domain path: `LocationBookingDerivedState` already calls `BookingService.BookingServiceClient` via gRPC. The new snapshot activity will reuse the same gRPC call pattern. No direct booking DB access.

- [x] **III. Testing** — Unit tests required for: new `RecordDeskAvailabilitySnapshotAsync` activity logic (desk classification), bug fixes in `LocationBookingDerivedState` (cancelled bookings, dual-tag), `LocationAnalyticsService` (zero-capacity). Integration tests required for: the new snapshot repository query, the new GraphQL query, and the bug fixes. All integration test assertions use repository-layer methods, not raw `DbContext`.

- [x] **IV. Frontend** — Frontend changes are in scope. A new `LocationDeskAvailabilityInsight` component pair is added to `web/apps/webapp/src/components/location/locationDeskAvailabilityInsight/` following the identical pattern used by `LocationDeskOccupancyInsight`. The new component reads `location.analytics.deskAvailabilitySnapshots` via a Relay `useRefetchableFragment`, renders a stacked `BarChart` from `@mui/x-charts`, and integrates into the existing `organization-analytics.tsx` Location Insights `GridContainer`. The `AnalyticsDaterangeSelector` is extended with a `6months` period option. Relay artifacts are regenerated via `pnpm relay` (from `web/apps/webapp/`) after the backend GraphQL schema export is updated.

- [x] **V. Pattern Consistency** — All new code follows established patterns: Temporal activity class with `[Activity]` methods, `IRepositoryFactory` for persistence, `IWorkflowIdService` for workflow IDs, `RepositoryBase` for the new repository, `EntityBaseWithDeleted` for the new entity, `IEntityTypeConfiguration<T>` for EF configuration. No new patterns introduced.

- [x] **VI. Logging** — `LOG-001` through `LOG-004` are specified in the spec. The new activity must log: start/completion, per-category desk counts, warnings for ambiguous resource tags, and correlation context (workflow run ID + location ID). Bug-fix paths must log the corrective classification decision.

## Project Structure

### Documentation (this feature)

```text
specs/006-desk-availability-analytics/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   └── graphql.md
│   └── openapi.md
└── tasks.md             # Phase 2 output (/speckit.tasks command)
```

### Source Code (repository root)

```text
location/
├── shared/
│   └── Location.Shared/
│       ├── Activities/
│       │   ├── LocationDailyAnalytics.cs           # MODIFIED — new RecordDeskAvailabilitySnapshotAsync activity
│       │   └── LocationBookingDerivedState.cs       # MODIFIED — bug fixes (cancelled bookings, dual-tag, zero-cap)
│       ├── Database/
│       │   ├── Entities/
│       │   │   └── DailyDeskAvailabilitySnapshot.cs # NEW — entity + EF config
│       │   ├── LocationDbContext.cs                 # MODIFIED — add DbSet<DailyDeskAvailabilitySnapshot>
│       │   └── Migrations/
│       │       └── [timestamp]_AddDailyDeskAvailabilitySnapshot.cs  # NEW — EF migration
│       ├── Repositories/
│       │   ├── DailyDeskAvailabilitySnapshotRepository.cs  # NEW
│       │   └── RepositoryFactory.cs                 # MODIFIED — register new repository
│       └── Workflows/
│           └── GenerateLocationDailyAnalytics.cs    # MODIFIED — call new snapshot activity
├── shared/
│   └── Location.Shared.UnitTests/
│       └── Activities/
│           ├── LocationDailyAnalyticsTests/
│           │   └── RecordDeskAvailabilitySnapshotShould.cs  # NEW
│           └── LocationBookingDerivedStateTests/
│               ├── CancelledBookingsExcludedShould.cs        # NEW
│               ├── DualTaggedResourceNotDoubleCountedShould.cs # NEW
│               └── ZeroCapacityOccupancyShould.cs            # NEW
├── apis/
│   └── Location.Api/
│       ├── GraphQL/
│       │   └── Analytics/
│       │       ├── LocationAnalytics.cs             # MODIFIED — add deskAvailabilitySnapshots field
│       │       ├── DeskAvailabilitySnapshot.cs      # NEW — GraphQL type
│       │       └── RootQuery.cs                     # MODIFIED — wire new query field
│       └── Services/
│           └── LocationAnalyticsService.cs          # MODIFIED — fetch and map snapshot data
└── apis/
    └── Location.Api.IntegrationTests/
        └── GraphQL/
            └── Analytics/
                └── LocationDeskAvailabilityAnalyticsShould.cs  # NEW

api-definitions/
└── openapi/
    └── skedular/
        └── location/
            └── location_analytics_v1.yaml           # MODIFIED — add backfill-by-date-range endpoint

web/
└── apps/
    └── webapp/
        └── src/
            ├── components/
            │   ├── analytics/
            │   │   └── analytics-daterange-selector.tsx     # MODIFIED — add 6months period option
            │   └── location/
            │       ├── locationDeskAvailabilityInsight/
            │       │   ├── location-desk-availability-insight.tsx       # NEW — Relay fragment + stacked BarChart
            │       │   ├── location-desk-availability-insight-root.tsx  # NEW — root wrapper, 6-month default, Skeleton, ErrorBoundary
            │       │   └── index.ts                                     # NEW — barrel export
            │       └── (organizationAnalytics/organization-analytics.tsx — see below)
            │   └── organization/
            │       └── organizationAnalytics/
            │           └── organization-analytics.tsx  # MODIFIED — add LocationDeskAvailabilityInsightRoot to GridContainer
            └── queries/
                └── __generated__/
                    └── (Relay artifacts regenerated by pnpm relay)
```

**Structure Decision**: Location domain, shared + api layers. No new top-level domain. Web app: new insight component pair in existing analytics shell.
