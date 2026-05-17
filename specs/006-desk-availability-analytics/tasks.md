# Tasks: Desk Availability Analytics

**Input**: Design documents from `specs/006-desk-availability-analytics/`  
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅, quickstart.md ✅

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: User story label (US1–US4)
- All paths are repository-root-relative

---

## Phase 1: Setup

**Purpose**: Migration, entity registration, and foundational persistence layer. No user story work can begin until the new entity is in the database and accessible via the repository factory.

- [x] T001 Add `DeskAvailabilityClassification` enum in `location/shared/Location.Shared/Database/Entities/DailyDeskAvailabilitySnapshot.cs`
- [x] T002 Add `DailyDeskAvailabilitySnapshot` entity class and `DailyDeskAvailabilitySnapshotConfiguration` in `location/shared/Location.Shared/Database/Entities/DailyDeskAvailabilitySnapshot.cs`
- [x] T003 Add `DailyDeskAvailabilitySnapshots` navigation collection to `Location` entity in `location/shared/Location.Shared/Database/Entities/Location.cs`
- [x] T004 Add `DbSet<DailyDeskAvailabilitySnapshot> DailyDeskAvailabilitySnapshot` to `LocationDbContext` in `location/shared/Location.Shared/Database/LocationDbContext.cs`
- [x] T005 Create `IDailyDeskAvailabilitySnapshotRepository` interface and `DailyDeskAvailabilitySnapshotRepository` implementation in `location/shared/Location.Shared/Repositories/DailyDeskAvailabilitySnapshotRepository.cs`
- [x] T006 Register `IDailyDeskAvailabilitySnapshotRepository` in `IRepositoryFactory` interface and `RepositoryFactory` class in `location/shared/Location.Shared/Repositories/RepositoryFactory.cs`
- [x] T007 Generate EF Core migration `AddDailyDeskAvailabilitySnapshot` in `location/shared/Location.Shared/Database/Migrations/`

**Checkpoint**: Database schema is in place, repository is wired, migration exists. Implementation of all user stories can now begin.

---

## Phase 2: Foundational — Booking Cancellation Investigation

**Purpose**: Confirm the correct gRPC field to use for filtering out cancelled bookings. This finding gates the bug-fix story (US3) and the snapshot activity (US2), both of which must exclude cancelled bookings.

**⚠️ BLOCKS**: US2 (snapshot classification accuracy) and US3 (Bug 1 fix)

- [x] T008 Read `booking/apis/` implementation of `Admin_GetPaginatedBookings` to confirm whether it already excludes soft-deleted bookings and which field on `Booking` proto distinguishes active from cancelled bookings. Document the finding as a comment in `location/shared/Location.Shared/Activities/LocationBookingDerivedState.cs` before making any changes.

**Checkpoint**: Cancellation filter field confirmed. US2 and US3 can proceed.

---

## Phase 3: User Story 2 — Scheduled Daily Desk Availability Snapshot (Priority: P1) 🎯 MVP

**Goal**: The system records per-desk availability state once per UTC day per location via the existing `GenerateLocationDailyAnalytics` Temporal workflow. This is the data producer for US1.

**Independent Test**: Trigger the snapshot activity for a test location with known desks and bookings, then query `DailyDeskAvailabilitySnapshotRepository.GetByLocationIdAndDateRangeAsync` and assert one record per desk with correct classification.

### Unit Tests — User Story 2

- [x] T009 [P] [US2] Add `RecordDeskAvailabilitySnapshotShould.cs` unit tests in `location/shared/Location.Shared.UnitTests/Activities/LocationDailyAnalyticsTests/` covering: active desk with no bookings → Available; inactive desk → Unavailable; desk with active booking on date → Booked; dual-tagged resource → Unavailable (desk wins) with warning log; location not found returns false; idempotent replace deletes existing then inserts

### Implementation — User Story 2

- [x] T010 [US2] Add `RecordDeskAvailabilitySnapshotAsync(string locationId)` activity method to `LocationDailyAnalytics` class in `location/shared/Location.Shared/Activities/LocationDailyAnalytics.cs`. Method must: (1) load location + resources with org tags, (2) fetch bookings for today via gRPC filtered to the snapshot date UTC range, (3) apply cancellation filter per T008 finding, (4) classify each desk resource using Inactive → Unavailable, booked → Booked, else → Available, (5) warn on dual-tagged resources, (6) call `DeleteByLocationAndDateAsync` then insert fresh records via repository, (7) emit LOG-001 start/completion, LOG-002 per-category counts, LOG-003 dual-tag warnings, LOG-004 correlation context
- [x] T011 [US2] Extend `GenerateLocationDailyAnalytics` workflow in `location/shared/Location.Shared/Workflows/GenerateLocationDailyAnalytics.cs` to call `RecordDeskAvailabilitySnapshotAsync` as a third activity step after `RecordLocationRoomsCountAsync`, with the same `StartToCloseTimeout = 1 min`, `MaximumAttempts = 3` retry policy
- [x] T012 [P] [US2] Add integration test `RecordDeskAvailabilitySnapshotShould.cs` in `location/shared/Location.Shared.IntegrationTests/` (or existing integration test project) asserting that after the activity runs, `GetByLocationIdAndDateRangeAsync` returns exactly one row per desk with correct classification; second invocation replaces records idempotently. Seed with at least 20 desks to provide confidence that per-batch processing does not breach the 1-minute `StartToCloseTimeout` (SC-002) at integration-test scale.

**Checkpoint**: Daily snapshot runs automatically, records persist correctly, tests pass. US1 can now read the data.

---

## Phase 4: User Story 1 — Query Historical Desk Availability Report (Priority: P1) 🎯 MVP

**Goal**: Operators can query the GraphQL `locationsAnalytics` field and retrieve `deskAvailabilitySnapshots` — per-day counts and desk name lists for each classification category — for any date range up to six months.

**Independent Test**: Seed known snapshots, query `locationsAnalytics(from:..., until:...) { deskAvailabilitySnapshots { date availableCount ... } }`, assert correct counts and desk names returned. Verify days with no snapshot are omitted.

### Unit Tests — User Story 1

- [x] T013 [P] [US1] Add unit tests for `LocationAnalyticsService.GetAnalyticsAsync` (with snapshots) in `location/apis/Location.Api.UnitTests/Services/LocationAnalyticsServiceTests/` covering: correct grouping of snapshots by date; days with no snapshot omitted from results; zero-capacity occupancy days omitted (bug fix integrated); auth check returns empty when `CanViewAnalytics` is false

### Implementation — User Story 1

- [x] T014 [P] [US1] Create `DeskAvailabilityDailySnapshot.cs` GraphQL type in `location/apis/Location.Api/GraphQL/Analytics/DeskAvailabilityDailySnapshot.cs` with fields: `date`, `availableCount`, `unavailableCount`, `bookedCount`, `availableDeskNames`, `unavailableDeskNames`, `bookedDeskNames`
- [x] T015 [US1] Add `deskAvailabilitySnapshots` field (type `IEnumerable<DeskAvailabilityDailySnapshot>`) to `LocationAnalytics.cs` in `location/apis/Location.Api/GraphQL/Analytics/LocationAnalytics.cs`
- [x] T016 [US1] Add `DeskAvailabilitySnapshotReport` read model record to `location/apis/Location.Api/Models/LocationAnalytics.cs` (or equivalent models file)
- [x] T017 [US1] Extend `ILocationAnalyticsService` and `LocationAnalyticsService` in `location/apis/Location.Api/Services/LocationAnalyticsService.cs` to fetch `DailyDeskAvailabilitySnapshot` rows for the date range and group them into `DeskAvailabilitySnapshotReport` records
- [x] T018 [US1] Update `Mapper` in `location/apis/Location.Api/Mappers/Mapper.cs` to map `DeskAvailabilitySnapshotReport` list to `IEnumerable<DeskAvailabilityDailySnapshot>`
- [x] T019 [US1] Update `RootQuery.LocationsAnalyticsAsync` in `location/apis/Location.Api/GraphQL/Analytics/RootQuery.cs` to pass snapshot data through to `LocationAnalytics` result object
- [x] T020 [US1] Run `scripts/generate-graphql.sh` to regenerate schema files and composed gateway schema; commit generated outputs
- [x] T021 [P] [US1] Add integration test `LocationDeskAvailabilityAnalyticsShould.cs` in `location/apis/Location.Api.IntegrationTests/GraphQL/Analytics/` asserting the GraphQL query returns correct per-day snapshot data; days with no snapshot omitted; auth check respected

**Checkpoint**: `deskAvailabilitySnapshots` field is live in GraphQL. Six-month range query returns correct results.

---

## Phase 5: User Story 3 — Fix Existing Location Analytics Bugs (Priority: P2)

**Goal**: Three confirmed bugs in existing analytics are fixed and covered by regression tests: cancelled bookings counted in booking totals, dual-tagged resource double-counting, zero-capacity misleading occupancy percentage.

**Independent Test**: Each bug has its own unit test that reproduces the before/after condition. Existing unit and integration tests must all still pass.

### Unit Tests — User Story 3

- [x] T022 [P] [US3] Add `CancelledBookingsExcludedShould.cs` in `location/shared/Location.Shared.UnitTests/Activities/LocationBookingDerivedStateTests/` asserting that a booking with the confirmed cancellation field set is not counted in any daily recording
- [x] T023 [P] [US3] Add `DualTaggedResourceNotDoubleCountedShould.cs` in `location/shared/Location.Shared.UnitTests/Activities/LocationDailyAnalyticsTests/` asserting a resource with both desk and room tags appears in desk count only, and a warning log is emitted
- [x] T024 [P] [US3] Add `ZeroCapacityOccupancyOmittedShould.cs` in `location/apis/Location.Api.UnitTests/Services/LocationAnalyticsServiceTests/` asserting that a day with zero-count desk recording does not appear in `desksOccupancyPercentage` result

### Implementation — User Story 3

- [x] T025 [US3] Fix Bug 1 in `LocationBookingDerivedState.GetBookingsAsync` in `location/shared/Location.Shared/Activities/LocationBookingDerivedState.cs`: apply client-side cancellation filter per T008 finding; add a structured warning log when cancelled bookings are filtered out with a count
- [x] T026 [US3] Fix Bug 2 in `RecordLocationDesksCountAsync` in `location/shared/Location.Shared/Activities/LocationDailyAnalytics.cs`: exclude resources that also have a `ResourceRoom` tag from desk count; emit warning log per dual-tagged resource with its ID
- [x] T027 [US3] Fix Bug 2 mirror in `RecordLocationRoomsCountAsync` in `location/shared/Location.Shared/Activities/LocationDailyAnalytics.cs`: exclude resources that also have a `ResourceDesk` tag from room count
- [x] T028 [US3] Fix Bug 3 in `LocationAnalyticsService` private `GetAnalyticsAsync` in `location/apis/Location.Api/Services/LocationAnalyticsService.cs`: change zero-count desk/room guard to omit the day entirely from the occupancy list instead of returning `Percentage = 0`
- [x] T038 [P] [US3] Add `RecomputeIdempotencyShould.cs` unit test in `location/shared/Location.Shared.UnitTests/Activities/LocationBookingDerivedStateTests/` asserting that calling `RecomputeAsync` twice with the same input produces identical `DailyBookingCountRecording` output without data loss or duplication — covering US3 Acceptance Scenario 4

**Checkpoint**: All three bugs fixed with regression tests. Explicitly run the full `Location.Shared.UnitTests`, `Location.Api.UnitTests`, `Location.Shared.IntegrationTests`, and `Location.Api.IntegrationTests` test suites and confirm all pass before marking US3 complete (SC-004).

---

## Phase 6: User Story 4 — Regenerate or Backfill Historical Snapshots (Priority: P3)

**Goal**: Administrators can trigger regeneration of desk availability snapshots for a specific location and date range via a REST endpoint. The backfill iterates each day and calls the same activity logic.

**Independent Test**: Call `PUT /v1/location/analytics/{locationId}/regenerate-desk-availability-snapshots` with a 7-day range; assert all 7 days of snapshot records are present in the repository after the call.

### Implementation — User Story 4

- [x] T029 [US4] Add `RegenerateDeskAvailabilitySnapshotsInput` to `location_analytics_v1.yaml` in `api-definitions/openapi/skedular/location/location_analytics_v1.yaml` per `contracts/openapi.md`
- [x] T030 [US4] Run `api-definitions/openapi/generate.sh` to regenerate `Location.Api` controller base and client; commit generated outputs
- [x] T031 [US4] Implement `RegenerateDeskAvailabilitySnapshotsAsync(string locationId, RegenerateDeskAvailabilitySnapshotsInput input)` on `IWorkaroundService` and `WorkaroundService` in `location/apis/Location.Api/Services/WorkaroundService.cs`: iterate each UTC day in `[input.From, input.Until]`, trigger `RecordDeskAvailabilitySnapshotAsync` for each day via the Temporal workflow or directly as an activity invocation, consistent with the existing `RegenerateDailyAnalyticsAsync` pattern
- [x] T032 [US4] Wire the new method into `LocationAnalyticsController` in `location/apis/Location.Api/Controllers/LocationAnalyticsController.cs` using the generated base class action
- [x] T033 [P] [US4] Add integration test `RegenerateDeskAvailabilitySnapshotsShould.cs` in `location/apis/Location.Api.IntegrationTests/` asserting: calling the endpoint for a 7-day range creates/replaces snapshots for all 7 days; calling it twice is idempotent

**Checkpoint**: Backfill endpoint live. Historical data recovery is possible for newly onboarded locations or after bug fixes.

---

## Phase 8: Web UI — Desk Availability Insight

**Goal**: Display per-day desk availability data in the organization analytics web application, in the existing Location Insights section, as a stacked bar chart with three series (available, unavailable, booked). Follows the identical component pattern used by `LocationDeskOccupancyInsight`.

**Independent Test**: After the backend schema is exported and Relay artifacts are regenerated, the component renders correctly for a location with seeded snapshot data: the stacked bar chart shows three coloured series per day and the date range selector defaults to six months.

**Prerequisites**: T020 (`scripts/generate-graphql.sh`) must be complete so the GraphQL schema includes `deskAvailabilitySnapshots` before Relay can compile the new fragment.

### Implementation — Web UI

- [x] T039 [P] Extend `AnalyticsDaterangeSelector` in `web/apps/webapp/src/components/analytics/analytics-daterange-selector.tsx`: add `'6months'` to the `Period` type, add a `6 Months` `ToggleButton`, and add the matching `case '6months'` to `handlePeriodChange` (subtract 6 months from today)
- [x] T040 Create `location-desk-availability-insight.tsx` in `web/apps/webapp/src/components/location/locationDeskAvailabilityInsight/`: Relay `useRefetchableFragment` reading `location.analytics.deskAvailabilitySnapshots { date availableCount unavailableCount bookedCount }`, stacked `BarChart` from `@mui/x-charts` with three series (Available / Unavailable / Booked), `AnalyticsDaterangeSelector` with `defaultPeriod="6months"`, wrapped in `AnalyticsInsightCard` with title `"Desk Availability Insights"`. Must use `@skedular/ui` typography wrappers — never `@mui/material/Typography` directly.
- [x] T041 Create `location-desk-availability-insight-root.tsx` in the same directory matching the `locationDeskOccupancyInsight-root.tsx` pattern: `useQueryLoader`, `useEffect` initial load with a 6-month default range (`from = today.subtract(6, 'months')`, `to = today`), `Skeleton` fallback inside `AnalyticsInsightCard`, `ErrorBoundary` with `RelayError`
- [x] T042 [P] Create `index.ts` barrel export: `export { default as LocationDeskAvailabilityInsightRoot } from './location-desk-availability-insight-root'`
- [x] T043 Integrate `LocationDeskAvailabilityInsightRoot` into `organization-analytics.tsx` in `web/apps/webapp/src/components/organization/organizationAnalytics/`: import from `@/components/location/locationDeskAvailabilityInsight` and add `<Grid><LocationDeskAvailabilityInsightRoot onReloadRequired={onReloadRequired} locationId={location.id} /></Grid>` alongside `LocationDeskOccupancyInsightRoot` in the `GridContainer`
- [x] T044 Run Relay compiler from `web/apps/webapp/` (`pnpm relay` or equivalent script) to regenerate TypeScript artifacts for the new Relay fragments introduced in T040–T041; commit generated outputs under `src/queries/__generated__/`

**Checkpoint**: `LocationDeskAvailabilityInsightRoot` renders in the Location Insights section. Relay types are generated and type-check passes. Stacked bar chart shows three series. Date range defaults to 6 months.

---

## Phase 7: Polish & Cross-Cutting Concerns

- [x] T034 [P] Run `scripts/generate-graphql.sh` final pass to verify schema is fully in sync with implementation; fix any drift
- [ ] T035 [P] Run `quickstart.md` validation end-to-end: trigger snapshot, query GraphQL, verify results match expected shape
- [x] T036 [P] Review all new structured log statements against LOG-001–LOG-004: confirm no desk names are logged at ERROR/WARN level (only counts and IDs); confirm workflow run ID and location ID are present in all activity logs
- [x] T037 Review and update `specs/006-desk-availability-analytics/quickstart.md` if any implementation details changed during development

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: No dependencies. Start immediately.
- **Phase 2 (Foundational)**: Depends on Phase 1. Blocks US2 and US3 (cancellation filter confirmation).
- **Phase 3 (US2)**: Depends on Phase 1 + Phase 2. Produces snapshot data consumed by US1.
- **Phase 4 (US1)**: Depends on Phase 1 + Phase 3 (needs snapshot records to exist). Can start in parallel with US2 on the read/GraphQL layer, but integration tests require US2 data.
- **Phase 5 (US3)**: Depends on Phase 2 (cancellation field confirmed). Independent of US1 and US2.
- **Phase 6 (US4)**: Depends on Phase 3 (US2 snapshot activity must exist to be called by backfill). Independent of US1 and US3.
- **Phase 7 (Polish)**: Depends on all desired stories complete.
- **Phase 8 (Web UI)**: Depends on Phase 4 T020 (`scripts/generate-graphql.sh` complete so `deskAvailabilitySnapshots` is in the exported schema). T039 (`AnalyticsDaterangeSelector`) is independent and can be done any time.

### User Story Dependencies

- **US2 (P1)**: Requires Phase 1 (entity/repo) + Phase 2 (cancellation filter) → no US story dependency
- **US1 (P1)**: Requires Phase 1 + US2 activity exists → depends on US2
- **US3 (P2)**: Requires Phase 2 (cancellation field) → no US story dependency
- **US4 (P3)**: Requires US2 snapshot activity to exist → depends on US2

### Parallel Opportunities

**Within Phase 1** (all can be done in one sitting, mostly independent files):

- T001–T004 can be done in parallel (different files)
- T005–T006 sequentially (repository then factory)
- T007 last (migration after entity + DbContext)

**After Phase 2**:

- US2 (T009–T012) and US3 (T022–T028) can run in parallel — different files, no cross-dependency

**Within US1 (Phase 4)**:

- T013 (unit tests), T014 (GraphQL type), T016 (read model) — can all be done in parallel
- T015 and T017 depend on T014 and T016 respectively

---

## Parallel Example: User Story 2 (Snapshot Activity)

```
Day 1:
  [A] T009 — Write unit tests for RecordDeskAvailabilitySnapshotAsync
  [B] T010 — Implement activity method (LocationDailyAnalytics.cs)
  → T010 needs T008 (Phase 2 finding) to be done first

Day 2:
  [A] T011 — Extend GenerateLocationDailyAnalytics workflow
  [B] T012 — Write integration test
  → Both depend on T010 being complete
```

---

## Implementation Strategy

**MVP scope (recommended)**: Complete Phases 1–4 + Phase 8 (US2 + US1 + Web UI) to deliver the end-to-end desk availability analytics feature. US3 (bug fixes) and US4 (backfill) add quality and operational value but are not required for the core analytics flow to work.

**US2 before US1**: Build the data producer (snapshot activity) before the data consumer (GraphQL query). This ensures integration tests for US1 have real data to assert against.

**Bug fixes (US3) are independent**: They can be done in a separate PR or after the main feature is merged, as long as they are included before the feature is considered production-ready.
