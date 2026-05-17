# Research: Resource Availability Dashboard

**Phase**: 0 — Pre-design research  
**Date**: 2026-05-10  
**Feature**: [spec.md](spec.md)

---

## 1. Existing Availability Infrastructure (from 006-desk-availability-analytics)

### Decision

Extend `ResourceAvailabilityClassification` from the location domain (additive states only). The `DailyResourceAvailabilitySnapshot` table and all snapshot infrastructure are analytics-only and are **not used** by the dashboard. The dashboard is a real-time availability check: it reads resource data from the Location DB and live booking data from the Booking domain using the **existing internal** `BookingServiceClient` gRPC client already in use within `Location.Shared`. No new gRPC or OpenAPI contracts are added.

### What Already Exists

| Artefact                                                     | Location                                                               | Status                                                           |
| ------------------------------------------------------------ | ---------------------------------------------------------------------- | ---------------------------------------------------------------- |
| `DailyResourceAvailabilitySnapshot` entity                   | `location/shared/Location.Shared/Database/Entities/`                   | ✅ Implemented (006)                                             |
| `IDailyResourceAvailabilitySnapshotRepository`               | `location/shared/Location.Shared/Repositories/`                        | ✅ Implemented (006)                                             |
| `ResourceAvailabilityClassification` enum                    | `location/shared/Location.Shared/Models/`                              | ✅ Implemented — 3 states: Available, Unavailable, Booked        |
| `LocationAnalyticsService`                                   | `location/apis/Location.Api/Services/`                                 | ✅ Implemented (006) — aggregates snapshots for charts           |
| `RecordResourceAvailabilitySnapshotAsync` activity           | `location/shared/Location.Shared/Activities/LocationDailyAnalytics.cs` | ✅ Implemented (006) — daily Temporal activity                   |
| `BookingService.BookingServiceClient` (internal gRPC client) | consumed in `Location.Shared` activities                               | ✅ In use — existing internal mechanism, no new contract         |
| `ResourceAvailabilityDailySnapshot` GraphQL type             | `location/apis/Location.Api/`                                          | ✅ Exposed (006) — analytics surface only, not used by dashboard |

### What the Dashboard Needs That Doesn't Exist Yet

| Need                                                                                               | Gap                                                                                                    |
| -------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------ |
| 6-state classification (Available, Partially Booked, Fully Booked, Occupied, Unavailable, Blocked) | Current enum has only 3 states; extension needed                                                       |
| Per-resource day view with individual booking windows                                              | No existing query returns booking time windows alongside resources                                     |
| Opening-hours-relative coverage calculation                                                        | Opening hours are modelled on `Location` and inherited by resources, but no coverage calculator exists |
| GraphQL query: `resourceDayViews(date, filters, pagination)`                                       | Not implemented                                                                                        |
| GraphQL subscription: `onResourceAvailabilityChanged(locationId/organizationId)`                   | Not implemented in Location domain                                                                     |
| Booking detail visibility filtering by org type + user role                                        | Not implemented at the dashboard query layer                                                           |
| Frontend dashboard page + components                                                               | Not implemented                                                                                        |

### Rationale

The snapshot table is pre-computed nightly for analytics aggregation and stores only a coarse three-state classification per resource per day — it has no booking windows and is stale by design. The dashboard is a real-time view: it must reflect the current state of bookings for any selected date, including booking windows, partial-vs-full coverage, and check-in state. The existing `BookingServiceClient` internal gRPC call is the correct mechanism; it is already wired into `Location.Shared` and is an implementation detail, not a new API surface.

---

## 2. Resource Availability Classification — Extension Decision

### Decision

Extend `ResourceAvailabilityClassification` from 3 → 6 states. The new states are additive to the enum and string constant map. Existing analytics code uses only `Available`, `Unavailable`, and `Booked` and is unaffected by adding new values.

### New State Model

```
Blocked         — explicit BlockedPeriod or closed date takes precedence over all
Occupied        — checked-in on current date (check-in data available today only)
Fully Booked    — all effective opening hours covered by bookings
Partially Booked — at least one booking window exists but free opening-hour time remains
Unavailable     — no opening hours for this date (e.g., closed on weekends)
Available       — no bookings within opening hours
```

**Precedence** (most to least restrictive): `Blocked > Occupied > Fully Booked > Partially Booked > Unavailable > Available`

### Opening Hours Coverage Calculation

A resource's **effective opening hours** for a given date = resource-level override if present, otherwise the parent location's opening hours for that day-of-week, minus any `ClosedDates` entries.

Coverage algorithm:

1. Compute the total opening-hour minutes for the date from effective opening hours.
2. If total = 0 (closed) → `Unavailable`.
3. Intersect all booking windows with the opening-hour window to get booked minutes.
4. If booked minutes = 0 → `Available`.
5. If booked minutes ≥ total minutes → `Fully Booked`.
6. Else → `Partially Booked`.

**Key finding**: Resources do not currently have their own opening-hours override field in the DB. The `OpeningHours` model is on `Location`. For v1, resources inherit location opening hours. If resource-level override is needed, it is a future enhancement (document as assumption).

---

## 3. Existing Opening Hours Model

### Decision

Use the existing `OpeningHours` model on `Location`. Resources inherit location opening hours for v1.

### Structure (existing)

```csharp
// On Location entity
OpeningHours {
    WeekOpeningHours: Dictionary<DayOfWeek, OpeningHoursDetails>
    DatesWithVariedOpeningHours: Dictionary<DateOnly, OpeningHoursDetails>
    ClosedDates: List<DateOnly>
}

OpeningHoursDetails {
    Closed: bool
    From: TimeOnly
    Until: TimeOnly
}
```

**Effective opening hours for a date**:

1. Check `ClosedDates` → if date in list, resource is closed (Unavailable).
2. Check `DatesWithVariedOpeningHours` → if date has an override, use it.
3. Else use `WeekOpeningHours[date.DayOfWeek]`.
4. If `OpeningHoursDetails.Closed == true` → Unavailable.
5. Else window is `[From, Until)`.

---

## 4. Domain Ownership — Booking, Not Location

### Decision

The dashboard implementation lives entirely in the **Booking domain** (`Booking.Shared` + `Booking.Api`). This is the right choice because:

- The Booking domain has **direct DB access** to all booking data (no cross-domain call needed).
- `Booking.Api` **already hosts** the HotChocolate subscription infrastructure (`ITopicEventReceiver`, `ITopicEventSender`, `IGraphQlTopicEventSender`, `[SubscriptionType]`) following the exact pattern to reuse.
- When a booking is created, modified, or cancelled, the trigger point is already inside `Booking.Api` — the sender call is a natural extension of the existing `GraphQlTopicEventSender.RaiseGraphqlChangeAsync` calls already made there.
- No Kafka consumer needs to be added to a separate domain just to forward events. The booking domain raises the topic event directly as part of its own booking mutation flow.

### Revised Subscription Trigger Path

```
Booking mutation / workflow in Booking domain
  → GraphQlTopicEventSender.RaiseGraphqlChangeAsync(
        Constants.ResourceAvailabilityTopicName(locationId), date)
  → Booking.Api HotChocolate ITopicEventSender
  → All subscribers for that locationId receive updated ResourceDayView[]
```

No Kafka. No cross-process hop. No extra consumer. The sender is called in the same process, the same way the existing booking and marketplace-subscription topic senders work today.

---

## 5. Organization Type — Booking Visibility Rule

### Decision

The `OrganizationType` enum in `shared/Api.Shared.Services/Models/OrganizationType.cs` has three values: `Private`, `Marketplace`, `Individual`. The spec's "Co-working Space" org type maps to `Marketplace` in the codebase. Update the spec assumption accordingly.

### Visibility Matrix

| Org Type      | Regular User                                                            | Owner / Admin                |
| ------------- | ----------------------------------------------------------------------- | ---------------------------- |
| `Private`     | Full booking details visible                                            | Full booking details visible |
| `Marketplace` | Booking windows visible as "unavailable" blocks only; no booking detail | Full booking details visible |
| `Individual`  | Booking windows visible as "unavailable" blocks only; no booking detail | Full booking details visible |

### Implementation Location

A new `ResourceDayViewBookingVisibilityFilter` service in `Booking.Shared` will apply this rule. It receives the `OrganizationType` and the current user's role/claims from the HotChocolate context and strips booking detail from the `ResourceDayView` before returning it to the client.

---

## 6. Floor Plan UI — Reuse Analysis

### Decision

The new Availability Dashboard is a **separate page** from the existing floor plan. The floor plan (`/floorPlans/`) renders a visual spatial layout; the dashboard renders a data table/list with status badges and booking windows. The two complement each other. Navigation links between them will be added.

### Reusable Patterns Identified

| Pattern                                    | Source                   | Reuse in Dashboard                |
| ------------------------------------------ | ------------------------ | --------------------------------- |
| `usePreloadedQuery` + Relay root query     | `floorPlans/page.tsx`    | Same pattern for dashboard page   |
| Filter bar with `useSearchParams`          | existing filter pages    | `AvailabilityFilterBar` component |
| `@skedular/ui` typography wrappers         | all feature components   | Dashboard cards and labels        |
| Relay pagination (`usePaginationFragment`) | existing list components | `ResourceDayViewList`             |
| MUI v9 Chip for status display             | existing components      | `AvailabilityStatusBadge`         |

### Existing Insights Components

`locationResourceAvailabilityInsight/` and `locationDeskOccupancyInsight/` exist but are chart/analytics components for the 006 analytics feature, not interactive dashboards. They will not be reused directly; however, the GraphQL fragments they use can inform the new dashboard's data shape.

---

## 7. Blocked Periods / Maintenance Windows

### Decision

For v1, "Blocked" state maps to:

1. The location's `ClosedDates` list (date is a closed day).
2. A resource's `IsActive == false` (inactive resources → Unavailable, not Blocked).

There is **no dedicated BlockedPeriod or MaintenanceWindow entity** in the current data model. The spec introduced these as domain concepts; they will be modelled as a future enhancement. For v1, resources show as Blocked only when the date is in the location's `ClosedDates`. Resources that are inactive show as Unavailable.

**Rationale**: Introducing a new `BlockedPeriod` entity requires a new EF Core migration and is a significant scope addition. The v1 implementation delivers high value with the existing data (bookings + opening hours + closed dates + active/inactive state) and documents BlockedPeriod as a v2 enhancement.

---

## 8. No New Contracts, No Migration

### Decision

No new database migrations are required. No new gRPC or OpenAPI endpoints. The only new external API surface is GraphQL. The dashboard reads live data directly from the Booking domain DB. The `DailyResourceAvailabilitySnapshot` table (006) is never consulted.

---

## 9. Resolved Unknowns Summary

| Unknown                                                                        | Resolution                                                                                                                                                                           |
| ------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Does a coverage calculator exist?                                              | No — new `ResourceAvailabilityDayViewService` required                                                                                                                               |
| Are resource-level opening hours supported?                                    | Not in DB for v1 — resources inherit location hours                                                                                                                                  |
| Is a subscription provider (Redis/InMemory) already configured in Booking.Api? | Already configured — existing subscriptions (`OnBookingUpdated`, `OnMarketplaceBookingSubscriptionUpdated`) are live in `Booking.Api` and use the same `ITopicEventReceiver` pattern |
| Does "Co-working Space" org type exist in code?                                | No — maps to `Marketplace` in `OrganizationType` enum                                                                                                                                |
| Is there a BlockedPeriod entity?                                               | No — v1 uses ClosedDates + IsActive; BlockedPeriod deferred to v2                                                                                                                    |
| Are cancelled bookings filtered at gRPC level?                                 | Partially (006 research found cancelled bookings may leak through) — dashboard must filter `deletedByCustomerId` client-side, same fix as 006                                        |
| Will new EF migration be needed?                                               | No — query path reads existing Booking DB tables; no new tables for v1                                                                                                               |
