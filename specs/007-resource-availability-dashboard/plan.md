# Implementation Plan: Resource Availability Dashboard

**Branch**: `007-resource-availability-dashboard` | **Date**: 2026-05-10 | **Spec**: [spec.md](spec.md)  
**Input**: Feature specification from `specs/007-resource-availability-dashboard/spec.md`

## Summary

A new Resource Availability Dashboard enabling co-working space owners and organisation administrators to select any date and view the availability status of all accessible resources (desks, rooms, etc.) for that date. Each resource displays its individual bookings alongside a computed day-level status (Available, Partially Booked, Fully Booked, Occupied, Unavailable, Blocked) derived from the resource's effective opening hours and all bookings/blocks for the day. Users can filter by location, floor, zone, resource type, and status. Real-time updates are delivered via GraphQL subscriptions (not polling), following the existing booking-domain subscription pattern. Booking detail visibility is governed by organisation type (Private: all users; Marketplace/Individual: owners and admins only).

The backend extends `Booking.Api` and `Booking.Shared` with a new `ResourceDayView` query and a new GraphQL subscription scoped to location. The booking domain has direct DB access to all booking data and already hosts the subscription infrastructure (`ITopicEventSender`/`ITopicEventReceiver`) — no cross-domain hop is needed. The frontend adds a new Next.js App Router page with Relay fragments and components following the existing webapp patterns.

## Technical Context

**Language/Version**: C# .NET 10 (backend); TypeScript 6 / React 19 / Next.js 16 App Router (frontend)  
**Primary Dependencies (backend)**: HotChocolate 14 (GraphQL + subscriptions — already wired in `Booking.Api`), Entity Framework Core 9, `Enterprise.Shared.Database` (repository pattern), `IRepositoryFactory`. The booking domain has direct access to all booking data and the existing `IGraphQlTopicEventSender` subscription infrastructure. No cross-domain gRPC call is needed from within the dashboard service. Location/resource metadata is available through the booking domain's existing data model.  
**Primary Dependencies (frontend)**: Relay, MUI v9, `@skedular/ui`, `@skedular/shared`, `useSearchParams`/`useRouter` (Next.js)  
**Storage**: PostgreSQL via EF Core — no new migration required. The `DailyResourceAvailabilitySnapshot` table (006) is analytics-only and is **not used** by this dashboard. The dashboard reads live booking data directly from the Booking domain DB.  
**Testing**: xUnit + NSubstitute (backend unit tests); integration tests via `Booking.Domain.IntegrationTests`; Vitest + React Testing Library (frontend)  
**Target Platform**: Linux server (API); web browser desktop/tablet (frontend)  
**Project Type**: Full-stack feature across existing microservice domains + web app  
**Performance Goals**: First page of results ≤ 3 s; filtered queries ≤ 2 s; subscription push ≤ 5 s after server-side change  
**Constraints**: Unbounded queries prohibited (cursor-based pagination enforced); booking detail visibility filtered per organisation type and user role at the API layer; no raw DbContext access in integration tests  
**Scale/Scope**: ≥ 500 resources per location; multi-tenancy (multiple organisations); three web products (webapp, webapp-teams, webapp-spaces)

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — The only new external API surface is GraphQL (new query + subscription in `Booking.Api`). No new OpenAPI or gRPC endpoints are added. The correct generator is `scripts/generate-graphql.sh` after any schema change. Web Relay artefacts regenerated via `web/apps/webapp/scripts/generate.sh`. No hand-editing of generated files.

- [x] **II. Domain Boundaries** — The dashboard is owned by the **Booking domain**, which has direct DB access to all booking data and already hosts the subscription infrastructure (`ITopicEventSender`/`ITopicEventReceiver`, `IGraphQlTopicEventSender`). No cross-domain DB access or new gRPC contract is needed. Location/resource metadata required for the view is available within the booking domain's existing data model. ✅ Clean single-domain implementation.

- [x] **III. Testing** — Unit tests: `ResourceAvailabilityDayViewService` (availability calculation, status precedence, opening hours coverage), `ResourceDayViewBookingVisibilityFilter` (org-type gating). Integration tests: new GraphQL query with filters and pagination in `Booking.Domain.IntegrationTests`; subscription smoke test. Integration tests assert via repository-layer queries, no raw DbContext. Frontend: Vitest + RTL component tests for dashboard page and filter bar.

- [x] **IV. Frontend** — New page uses Next.js App Router + Relay fragments collocated with components. Generated Relay artefacts regenerated via `scripts/generate-graphql.sh` + `web/apps/webapp/scripts/generate.sh`. Typography imported from `@skedular/ui`. British spelling in all user-facing copy. No hand-editing of generated artefacts.

- [x] **V. Pattern Consistency** — Query and subscription live in `Booking.Api`, directly extending the existing `[SubscriptionType]` pattern already established there (`Booking.Api/GraphQL/Booking/RootSubscription.cs`, `GraphQlTopicEventSender.cs`). The classification enum extension (3 → 6 states) is additive and backward-compatible with existing analytics. No new framework or alternative approach introduced.

- [x] **VI. Logging** — Structured logs planned for: dashboard query lifecycle (date, filters, tenant, result count), opening-hours calculation per resource, availability state transitions during calculation, subscription lifecycle (connected, dropped, reconnected), slow-query warning, booking visibility filter decisions. Follows `Microsoft.Extensions.Logging` + structured property conventions used across location and booking domains.

**Constitution check: PASS — all gates met. No violations requiring justification.**

## Project Structure

### Documentation (this feature)

```text
specs/007-resource-availability-dashboard/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/
│   ├── graphql.md       # GraphQL query, subscription, types contract
│   └── relay.md         # Relay fragment and query shapes
└── tasks.md             # Phase 2 output (/speckit.tasks command)
```

### Source Code (repository root)

```text
# Backend — Booking domain (primary owner)
booking/shared/Booking.Shared/
├── Models/
│   ├── ResourceAvailabilityClassification.cs     # NEW: 6-state enum
│   ├── ResourceDayView.cs                        # NEW: computed day view model
│   └── BookingWindow.cs                          # NEW: time window within a day view
└── Services/
    └── ResourceAvailabilityDayViewService.cs     # NEW: queries booking DB directly

booking/shared/Booking.Shared.UnitTests/
└── Services/ResourceAvailabilityDayViewServiceTests/
    ├── ComputeDayStatusShould.cs                 # NEW
    └── FilterBookingDetailsShould.cs             # NEW

booking/apis/Booking.Api/
└── GraphQL/
    ├── ResourceAvailability/
    │   ├── ResourceAvailabilityQuery.cs          # NEW: resourceDayViews query
    │   ├── ResourceAvailabilitySubscription.cs   # NEW: onResourceAvailabilityChanged
    │   ├── ResourceDayViewType.cs                # NEW: GQL type
    │   ├── BookingWindowType.cs                  # NEW: GQL type
    │   └── ResourceAvailabilityFilterInput.cs    # NEW: filter input type
    └── Services/
        └── (reuse existing GraphQlTopicEventSender.cs — no new file needed)

booking/domain/Booking.Domain.IntegrationTests/
└── GraphQL/
    └── ResourceAvailability/
        ├── ResourceDayViewsQueryShould.cs        # NEW
        └── ResourceAvailabilitySubscriptionShould.cs # NEW

# Frontend — Web app
web/apps/webapp/src/
├── rootPages/organizations/organization/
│   └── availabilityDashboard/
│       └── page.tsx                             # NEW: App Router page
└── components/
    └── availabilityDashboard/
        ├── index.ts
        ├── AvailabilityDashboard.tsx            # NEW: root component
        ├── AvailabilityDashboard.graphql        # NEW: Relay query + subscription
        ├── ResourceDayViewList.tsx              # NEW: paginated list
        ├── ResourceDayViewCard.tsx              # NEW: card with status badge + booking list
        ├── AvailabilityFilterBar.tsx            # NEW: filter controls
        ├── AvailabilityStatusBadge.tsx          # NEW: colour-coded status chip
        ├── BookingWindowList.tsx                # NEW: time-window entries per resource
        └── __tests__/
            ├── AvailabilityDashboard.test.tsx   # NEW
            ├── ResourceDayViewCard.test.tsx     # NEW
            └── AvailabilityFilterBar.test.tsx   # NEW
```

**Structure Decision**: The Booking domain is the correct owner — it has direct DB access to all booking data and already hosts the HotChocolate subscription infrastructure. No cross-domain gRPC call is needed. The existing `GraphQlTopicEventSender` in `Booking.Api` is reused for the new subscription topic. The existing `ResourceAvailabilityClassification` enum (Location.Shared) is either referenced as a shared dependency or a new parallel enum is defined in `Booking.Shared` to avoid Location → Booking coupling — to be decided during implementation (prefer keeping it in a shared library if both domains reference it).

## Complexity Tracking

No constitution violations. No complexity exceptions required.
