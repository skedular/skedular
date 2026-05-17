# Tasks: Resource Availability Dashboard

**Feature**: `007-resource-availability-dashboard`  
**Spec**: [spec.md](spec.md) | **Plan**: [plan.md](plan.md)  
**Domain**: Booking (`Booking.Api` + `Booking.Shared`)  
**Total Tasks**: 55 | **MVP Scope**: Phase 3 (US1)

---

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel (different files, no blocking dependencies)
- **[Story]**: User story label — US1…US5. Foundational / Setup phases have no label.
- File paths are relative to the repository root.

---

## Phase 1: Setup (Baseline Verification)

**Purpose**: Confirm the existing Booking domain compiles and all tests pass before any new code is added. No new files.

- [x] T001 Run `dotnet test booking/shared/Booking.Shared.UnitTests/Booking.Shared.UnitTests.csproj` and confirm all tests pass
- [x] T002 Run `dotnet test booking/domain/Booking.Domain.IntegrationTests/Booking.Domain.IntegrationTests.csproj` and confirm all tests pass

**Checkpoint**: Booking domain is green — safe to add new code.

---

## Phase 2: Foundational (Blocking Prerequisites for All User Stories)

**Purpose**: Define all new models, enums, filter inputs, and the service interface. These are pure model/interface definitions with no external dependencies — all subsequent phases depend on them.

**⚠️ CRITICAL**: Phases 3–7 cannot begin until this phase is complete.

- [x] T003 [P] Create `ResourceAvailabilityClassification.cs` (new 6-state enum: `Available`, `PartiallyBooked`, `FullyBooked`, `Occupied`, `Unavailable`, `Blocked` plus string constants) in `booking/shared/Booking.Shared/Models/ResourceAvailabilityClassification.cs` — do NOT modify `Location.Shared` enum
- [x] T004 [P] Create `BookingWindow.cs` (computed record: `BookingId`, `From`, `Until`, `IsRecurring`, `IsCheckedIn`, `BookedByName?`, `BookedByUserId?`, `Notes?`) in `booking/shared/Booking.Shared/Models/BookingWindow.cs`
- [x] T005 [P] Create `ResourceDayView.cs` (computed record: resource metadata, `Date`, `Status`, `OpeningFrom?`, `OpeningUntil?`, `TotalOpeningMinutes`, `BookedMinutes`, `IReadOnlyList<BookingWindow>`) in `booking/shared/Booking.Shared/Models/ResourceDayView.cs`
- [x] T006 [P] Create `ResourceAvailabilityDayFilter.cs` (filter record: `Date`, `OrganizationId?`, `LocationId?`, `FloorId?`, `ZoneId?`, `ResourceType?`, `Status?`) in `booking/shared/Booking.Shared/Models/ResourceAvailabilityDayFilter.cs`
- [x] T007 [P] Create `ResourceDayViewResult.cs` (result wrapper record: `IReadOnlyList<ResourceDayView> Items`, `string SubscriptionKey`) in `booking/shared/Booking.Shared/Models/ResourceDayViewResult.cs`
- [x] T007b [P] Create `ResourceAvailabilityOrderBy.cs` (sort record: `ResourceAvailabilityOrderByField Field`, `bool Descending = false`; enum values: `ResourceName`, `ResourceType`, `LocationName`, `FloorName`, `ZoneName`, `Status`) in `booking/shared/Booking.Shared/Models/ResourceAvailabilityOrderBy.cs`
- [x] T008 Define `IResourceAvailabilityDayViewService` interface (method: `GetAsync(filter, orderBy, requestingUserId, requestingUserRoles, ct) → ResourceDayViewResult`) in `booking/shared/Booking.Shared/Services/IResourceAvailabilityDayViewService.cs`
- [x] T008b [P] [FR-017] Add XML `<summary>` doc-comments to all public API surfaces in `Booking.Shared`: `IResourceAvailabilityDayViewService`, `SubscriptionKeyService` (`Compute`, `AffectedKeys`), `ResourceDayView`, `BookingWindow`, `ResourceDayViewResult`, `ResourceAvailabilityDayFilter`, `ResourceAvailabilityOrderBy`, `ResourceAvailabilityClassification` enum — comments must describe intent and any non-obvious invariants (e.g. opaque key contract, precedence rule); no PII
- [x] T009 [P] Create `SubscriptionKeyService.cs` (deterministic opaque key generator: SHA-256 of canonical filter JSON, URL-safe base64; `Compute(filter)` and `AffectedKeys(organizationId, locationId, floorId, zoneId, resourceType, date)` returning all null-dimension permutations) in `booking/shared/Booking.Shared/Services/SubscriptionKeyService.cs`

**Checkpoint**: All model definitions compile — US1–US5 implementation can begin in parallel.

---

## Phase 3: User Story 1 – View Resource Availability for a Selected Date (Priority: P1) 🎯 MVP

**Goal**: Given a date, return a list of `ResourceDayView` records, each showing the resource's computed day-level status, effective opening hours, and individual booking windows for that date.

**Independent Test**: Query `resourceDayViews(filter: { date: "2026-05-10" })` and confirm each resource in the Booking DB is returned with a correct `status` that matches its bookings and location opening hours.

- [x] T010 [US1] Investigate existing floor plan / availability calculation logic
- [x] T011 [US1] Implement `ComputeEffectiveOpeningHours` in `ResourceAvailabilityDayViewService.cs`
- [x] T012 [US1] Implement availability classification logic in `ResourceAvailabilityDayViewService.cs`
- [x] T013 [US1] Implement `GetAsync` in `ResourceAvailabilityDayViewService.cs`
- [x] T014 [P] [US1] Create `BookingWindowDetails.cs` in `Booking.Api/GraphQL/ResourceAvailability/`
- [x] T015 [P] [US1] Create `ResourceDayViewDetails.cs` in `Booking.Api/GraphQL/ResourceAvailability/`
- [x] T016 [P] [US1] Create `ResourceAvailabilityFilterInput.cs` in `Booking.Api/GraphQL/ResourceAvailability/`
- [x] T016b [P] [US1] Create `ResourceAvailabilityOrderByInput.cs` in `Booking.Api/GraphQL/ResourceAvailability/`
- [x] T017 [US1] Create `ResourceAvailabilityQuery.cs` in `Booking.Api/GraphQL/ResourceAvailability/`
- [x] T018 [US1] Register `IResourceAvailabilityDayViewService` in `Booking.Shared` DI
- [x] T019 [P] [US1] Unit test: `ComputeDayStatusShould.cs`

**Checkpoint**: `resourceDayViews` query returns correctly classified resources for a given date. US1 is independently testable.

---

## Phase 4: User Story 5 – Authorised Access and Booking Visibility (Priority: P1)

**Goal**: Only resources in the requesting user's authorised organizations/locations are returned. Booking detail visibility is governed by org type + user role (Private: full detail; Marketplace/Individual: owners/admins see full detail, regular users see redacted windows).

**Independent Test**: Log in as a regular user of a Marketplace organization, query `resourceDayViews`, and confirm `bookedByName`, `bookedByUserId`, and `notes` are null for all booking windows. Log in as an admin and confirm all fields are populated.

- [x] T020 [US5] Implement tenancy guard in `ResourceAvailabilityDayViewService.GetPageAsync`
- [x] T021 [US5] Create `ResourceDayViewBookingVisibilityFilter.cs`
- [x] T022 [US5] Wire `ResourceDayViewBookingVisibilityFilter` into `GetPageAsync`
- [x] T023 [P] [US5] Unit test: `FilterBookingDetailsShould.cs`
- [x] T024 [US5] Register `ResourceDayViewBookingVisibilityFilter` in `Booking.Shared` DI

**Checkpoint**: Tenancy scoping and booking-detail visibility filtering verified. US5 is independently testable alongside US1.

---

## Phase 5: User Story 2 – Filter Resources by Location, Floor, Zone, Type, and Status (Priority: P1)

**Goal**: The `resourceDayViews` query accepts and correctly applies locationId, floorId, zoneId, resourceType, and status filters. Filters are combined with AND logic. All filters empty → all accessible resources returned.

**Independent Test**: Apply a single `locationId` filter and confirm only resources at that location are returned. Apply `status: AVAILABLE` and confirm only available resources are returned. Clear all filters and confirm the full accessible resource list is restored.

- [x] T025 [US2] Wire `locationId`, `floorId`, `zoneId`, `resourceType` filter params into the resource DB query
- [x] T026 [US2] Wire `status` (post-classification) filter in `GetPageAsync`
- [x] T027 [P] [US2] Verified filter contract completeness in `ResourceAvailabilityFilterInput.cs`

**Checkpoint**: All five filter dimensions work independently and in combination. US2 is independently testable.

---

## Phase 6: ~~User Story 4 – Pagination~~ (Removed)

**Decision**: The query always returns the full filtered result set in a single response. Pagination is not implemented. Performance at scale (500+ resources) is achieved through DB-level filtering and efficient query construction rather than client-side pagination.

---

## Phase 7: User Story 3 – Real-Time Availability Updates via Subscription (Priority: P2)

**Goal**: When a booking is created, modified, or cancelled, the server pushes updated `ResourceDayView` records to all subscribed clients for the affected locationId and date. No Kafka consumer; the trigger is a direct call to the existing `GraphQlTopicEventSender` inside Booking.Api.

**Independent Test**: Open a subscription for `onResourceAvailabilityChanged(locationId, date)`, create a booking for a resource at that location and date, and confirm a subscription push is received with the updated resource status — without a full page reload.

- [x] T031 [US3] Create `ResourceAvailabilitySubscription.cs` in `Booking.Api/GraphQL/ResourceAvailability/`
- [x] T032 [US3] Subscription type auto-discovered via `AddSourceSchemaDefaults()`
- [x] T033 [US3] Wire subscription trigger in `GraphQlTopicEventSender.RaiseGraphqlChangeAsync`
- [x] T034 [US3] Run `scripts/generate-graphql.sh` and verify: `booking/domain/Booking.Domain.IntegrationTests/schema.graphql` contains `resourceDayViews` and `onResourceAvailabilityChanged`; `api-definitions/graphql/skedular/v1/schema.graphql` is updated; no hand-edited schema files
- [x] T035 [P] [US3] Integration test: `ResourceAvailabilitySubscriptionShould.cs` — subscribe, create a booking, assert subscription push delivers updated ResourceDayView with correct status in `booking/domain/Booking.Domain.IntegrationTests/GraphQL/ResourceAvailability/ResourceAvailabilitySubscriptionShould.cs`
- [x] T036 [P] [US3] Integration test: `ResourceDayViewsQueryShould.cs` — seed bookings for multiple statuses (available, partially booked, fully booked, blocked); query `resourceDayViews`; assert correct status, booking windows, and tenant scoping in `booking/domain/Booking.Domain.IntegrationTests/GraphQL/ResourceAvailability/ResourceDayViewsQueryShould.cs`

**Checkpoint**: Real-time subscription push verified end-to-end. US3 is independently testable.

---

## Phase 8: Frontend – Availability Dashboard Page and Components

**Purpose**: New Next.js App Router page and Relay-based component tree. Depends on Phase 7 (schema must be generated before `generate.sh` can produce Relay artefacts).

**Independent Test**: Open the dashboard page in a browser, confirm all resources for today's date are shown with correct status badges, apply a filter, confirm results narrow, scroll to load more, and confirm subscription badge updates when a booking is created in another session.

- [x] T037 Create `AvailabilityDashboard.graphql` (Relay root query `AvailabilityDashboardQuery` + subscription `OnResourceAvailabilityChangedSubscription` per relay.md — **no fragments here; each fragment lives collocated with its component**) in `web/apps/webapp/src/components/availabilityDashboard/AvailabilityDashboard.graphql`
- [x] T038 Run `web/apps/webapp/scripts/generate.sh` and confirm Relay artefacts are generated in `__generated__/` directories — do not hand-edit generated files
- [x] T039 [P] Create `AvailabilityStatusBadge.tsx` (MUI Chip, colour-coded by `ResourceAvailabilityStatus`, no Relay) in `web/apps/webapp/src/components/availabilityDashboard/AvailabilityStatusBadge.tsx` — use typography from `@skedular/ui`, not `@mui/material/Typography` directly
- [x] T040 [P] Create `BookingWindowList.tsx` (`useFragment` on `BookingWindowList_bookingWindow`; fragment `BookingWindowList_bookingWindow on BookingWindow` defined inline via `graphql` tagged template literal in the same `.tsx` file) in `web/apps/webapp/src/components/availabilityDashboard/BookingWindowList.tsx`
- [x] T041 Create `ResourceDayViewCard.tsx` (`useFragment` on `ResourceDayViewCard_resourceDayView`, renders `AvailabilityStatusBadge` + `BookingWindowList`; fragment `ResourceDayViewCard_resourceDayView on ResourceDayView` defined inline via `graphql` tagged template literal in the same `.tsx` file) in `web/apps/webapp/src/components/availabilityDashboard/ResourceDayViewCard.tsx`
- [x] T042 Create `ResourceDayViewList.tsx` (`useFragment` on `ResourceDayViewList_result`, renders `ResourceDayViewCard` per item; fragment `ResourceDayViewList_result on ResourceDayViewResult` with `items { ...ResourceDayViewCard_resourceDayView }` + `subscriptionKey` defined inline via `graphql` tagged template literal in the same `.tsx` file) in `web/apps/webapp/src/components/availabilityDashboard/ResourceDayViewList.tsx`
- [x] T043 Create `AvailabilityFilterBar.tsx` (date picker + location/floor/zone/type/status filter selects + sort field/direction selects; filter and sort state synced to URL via `useSearchParams`/`useRouter`; all controls empty on initial load with default sort `RESOURCE_NAME ASC`) in `web/apps/webapp/src/components/availabilityDashboard/AvailabilityFilterBar.tsx`
- [x] T044 Create `AvailabilityDashboard.tsx` (`usePreloadedQuery` on `AvailabilityDashboardQuery`; `useSubscription` on `OnResourceAvailabilityChangedSubscription` passing `subscriptionKey` from the query result — the key is opaque and must not be constructed on the client; re-subscribes automatically when filter changes by re-running the query; composes `AvailabilityFilterBar` + `ResourceDayViewList`) in `web/apps/webapp/src/components/availabilityDashboard/AvailabilityDashboard.tsx`
- [x] T045 Create `page.tsx` (App Router page: calls `loadQuery` with default filter `{ date: today }`, renders `AvailabilityDashboard` inside `Suspense`) in `web/apps/webapp/src/rootPages/organizations/organization/availabilityDashboard/page.tsx`
- [x] T046 [P] Create `index.ts` (barrel export of public components) in `web/apps/webapp/src/components/availabilityDashboard/index.ts`
- [x] T047 [P] Component test: `AvailabilityFilterBar.test.tsx` — render filter bar; simulate date change; confirm URL param `date` updates; apply location filter; confirm `locationId` param updates in `web/apps/webapp/src/components/availabilityDashboard/__tests__/AvailabilityFilterBar.test.tsx`
- [x] T048 [P] Component test: `ResourceDayViewCard.test.tsx` — render card with mock fragment data for each status value; confirm correct `AvailabilityStatusBadge` label and colour; confirm booking windows render; confirm detail fields absent for restricted mock data in `web/apps/webapp/src/components/availabilityDashboard/__tests__/ResourceDayViewCard.test.tsx`
- [x] T049 Component test: `AvailabilityDashboard.test.tsx` — render dashboard with mock Relay environment; confirm resource list renders; confirm filter bar is empty on load; confirm subscription update mutates store and card re-renders in `web/apps/webapp/src/components/availabilityDashboard/__tests__/AvailabilityDashboard.test.tsx`

**Checkpoint**: Dashboard page is browsable; filters, pagination, and subscription update are verified in component tests.

---

## Phase 9: Polish and Cross-Cutting Concerns

**Purpose**: Structured logging, disconnection handling, floor plan navigation, and final test sweep.

- [x] T050 Add structured logging to `ResourceAvailabilityDayViewService` per LOG-001–LOG-005: query lifecycle log (date, filters, tenant, result count); state-transition log per resource; slow-query warning (configurable threshold via `IOptions`); no PII in log properties
- [x] T050b [LOG-003] Add server-side subscription lifecycle logging to `ResourceAvailabilitySubscription.cs`: log when a subscription is established (subscriptionKey, tenantId at DEBUG) and when it is torn down / cancelled (subscriptionKey, reason at DEBUG); no PII; satisfies LOG-003 ("subscription established / torn-down" lifecycle events)
- [x] T051 Handle subscription disconnection in `AvailabilityDashboard.tsx`: on `onError` / connection loss show a non-intrusive `Alert` ("Live updates paused — reconnecting…"); on reconnect call `refetch()` to refresh state (FR-008, spec edge case)
- [x] T052 Add navigation deep-link from the Availability Dashboard to the floor plan view and from the floor plan view to the dashboard for the same date/location (FR-015) — wire in `page.tsx` and the relevant floor plan page
- [x] T053 Run `scripts/generate-graphql.sh` once more as a final verification that the composed gateway schema is in sync with all new Booking.Api types
- [x] T054 Final sweep: run all Booking unit tests, integration tests, and webapp component tests; confirm no regressions and that accessibility checks pass (keyboard navigation, ARIA labels on filter controls and status badges, per SC-008)

**Checkpoint**: Feature is complete, all tests pass, schema is generated, observability is in place.

---

## Dependencies (User Story Completion Order)

```
Phase 1 (Setup)
  └── Phase 2 (Foundational: models + interfaces)
        ├── Phase 3 (US1 – Core query)         ← MVP: deliver here
        │     └── Phase 4 (US5 – Tenancy)
        │           └── Phase 5 (US2 – Filtering)
        │                 └── Phase 6 (US4 – Pagination)
        │                       └── Phase 7 (US3 – Subscription + schema gen)
        │                             └── Phase 8 (Frontend)
        │                                   └── Phase 9 (Polish)
        └── (T009 – constants — parallel with Phase 3+)
```

US1 and US5 are tightly coupled (tenancy enforcement is part of every query). US2 and US4 extend the same query. US3 introduces the subscription and must precede frontend work (schema must be generated first).

---

## Parallel Execution Opportunities

**Within Phase 2** (after T001–T002): T003, T004, T005, T006, T007, T007b, T008, T008b, T009 can all run in parallel.

**Within Phase 3**: T014, T015, T016 (GraphQL types) can run in parallel with T010–T013 (service implementation) because they depend only on the model types from Phase 2, not on each other.

**Within Phase 7**: T035 and T036 (integration tests) can run in parallel once the subscription and query are registered (T031–T034).

**Within Phase 8**: T039, T040 (`AvailabilityStatusBadge`, `BookingWindowList`) can be built in parallel immediately after T038 (Relay generation). T047, T048 (component tests for filter bar and card) can run in parallel.

---

## Implementation Strategy (Increment Order)

1. **MVP (Phases 1–3)**: Passing baseline + models + basic `resourceDayViews` query with status classification. Deliverable: query the API and get real availability data.
2. **Secured MVP (Phase 4)**: Add tenancy + booking-detail visibility. Deliverable: safe to demo to real users.
3. **Full P1 (Phase 5)**: Add filter dimensions. Deliverable: all P1 user stories complete.
4. **P2 (Phases 6–7)**: Pagination + real-time subscription + schema generation. Deliverable: production-ready backend.
5. **Frontend (Phase 8)**: Complete dashboard UI. Deliverable: end-to-end feature.
6. **Complete (Phase 9)**: Logging, reconnection, floor plan nav, final sweep.
