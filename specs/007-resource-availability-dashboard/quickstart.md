# Quickstart: Resource Availability Dashboard

**Feature**: [spec.md](spec.md)  
**Date**: 2026-05-10

---

## Prerequisites

- Booking domain running with existing database seeded (bookings, resources, location opening hours)
- Web app dev server running

> **Scope reminder**: The only new external API surface is GraphQL, added to `Booking.Api`. No new gRPC or OpenAPI endpoints. No cross-domain calls. The snapshot table is not used.

---

## 1. Backend — Run Existing Booking Tests

Verify the baseline before adding any code:

```bash
dotnet test booking/shared/Booking.Shared.UnitTests/Booking.Shared.UnitTests.csproj
dotnet test booking/domain/Booking.Domain.IntegrationTests/Booking.Domain.IntegrationTests.csproj
```

---

## 2. Backend — Define the Classification Enum and Computed Models

Create in `booking/shared/Booking.Shared/Models/`:

- `ResourceAvailabilityClassification.cs` — new 6-state enum (`Available`, `PartiallyBooked`, `FullyBooked`, `Occupied`, `Unavailable`, `Blocked`) with string constants
- `ResourceDayView.cs` — computed record (see [data-model.md](data-model.md))
- `BookingWindow.cs` — time-window record

The existing `ResourceAvailabilityClassification` in `Location.Shared` (3 states, analytics-only) is left unchanged.

---

## 3. Backend — Implement the Day View Service

Create `booking/shared/Booking.Shared/Services/ResourceAvailabilityDayViewService.cs`:

1. Query booking records directly from the Booking DB for the requested date, location, and resource filters.
2. Compute effective opening hours from location data available in the booking domain.
3. For each resource, filter out cancelled bookings and compute status using the precedence rule (see [data-model.md](data-model.md)).
4. Apply the `ResourceDayViewBookingVisibilityFilter` to strip booking detail for restricted users.

Run unit tests after each method to validate the status precedence logic.

---

## 4. Backend — Add GraphQL Types and Resolvers

In `booking/apis/Booking.Api/GraphQL/ResourceAvailability/`:

1. Create `ResourceDayViewType.cs` — HotChocolate object type for `ResourceDayView`.
2. Create `BookingWindowType.cs` — HotChocolate object type for `BookingWindow`.
3. Create `ResourceAvailabilityFilterInput.cs` — HotChocolate input type.
4. Create `ResourceAvailabilityQuery.cs` — extends `Query` with `resourceDayViews(...)`.
5. Create `ResourceAvailabilitySubscription.cs` — `[SubscriptionType]` with `OnResourceAvailabilityChanged(locationId, date)`, following the exact pattern of the existing `Booking.Api/GraphQL/Booking/RootSubscription.cs`.
6. Register all types in `Booking.Api`'s existing schema builder. **Reuse the existing `GraphQlTopicEventSender`** — no new sender class needed.

---

## 5. Backend — Wire the Subscription Trigger

In `Booking.Api`, wherever a booking is created, modified, or cancelled (mutation handlers / domain event handlers), add a call to the existing `GraphQlTopicEventSender`:

```csharp
await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
    ResourceAvailabilityConstants.TopicName(booking.LocationId),
    booking.Date,
    cancellationToken);
```

This is identical to how the existing booking subscription topic is triggered today. No Kafka consumer, no cross-process hop.

---

## 6. Backend — Regenerate GraphQL Schema

After all types and resolvers are registered:

```bash
scripts/generate-graphql.sh
```

Verify:

- `booking/domain/Booking.Domain.IntegrationTests/schema.graphql` contains the new `resourceDayViews` query and `onResourceAvailabilityChanged` subscription.
- `api-definitions/graphql/skedular/v1/schema.graphql` (composed gateway) is updated.

---

## 7. Backend — Integration Tests

Add integration tests to `booking/domain/Booking.Domain.IntegrationTests/GraphQL/ResourceAvailability/`:

- `ResourceDayViewsQueryShould.cs`: seed bookings, query `resourceDayViews`, assert correct status and booking windows.
- `ResourceAvailabilitySubscriptionShould.cs`: subscribe, create/cancel a booking, assert subscription push delivers updated view.

Assert persistence state through repository methods, not raw DbContext.

---

## 8. Frontend — Regenerate Relay Artefacts

```bash
web/apps/webapp/scripts/generate.sh
```

Verify that `__generated__/` files for the new query and subscription are created.

---

## 9. Frontend — Implement the Dashboard Page

1. Create `web/apps/webapp/src/rootPages/organizations/organization/availabilityDashboard/page.tsx`:
   - Use `loadQuery` with `AvailabilityDashboardQuery`.
   - Read `date`, `locationId`, and other filter params from `searchParams`.
   - Default `date` to today's date if not provided.

2. Create components (see [contracts/relay.md](contracts/relay.md) for fragment shapes):
   - `AvailabilityDashboard.tsx` — root, wires query + subscription.
   - `AvailabilityFilterBar.tsx` — date picker + filter selects synced to URL params.
   - `ResourceDayViewList.tsx` — paginated list with load-more.
   - `ResourceDayViewCard.tsx` — individual resource row with status badge + booking list.
   - `AvailabilityStatusBadge.tsx` — MUI Chip with colour per status.
   - `BookingWindowList.tsx` — time window entries.

3. Import typography from `@skedular/ui`. Use British spelling in all user-facing copy.

---

## 10. Frontend — Component Tests

```bash
cd web/apps/webapp
pnpm test -- --testPathPattern=availabilityDashboard
```

Tests should cover:

- Status badge renders correct colour and label for each of the 6 states.
- Filter bar updates URL params correctly.
- Booking windows are hidden for restricted users (empty `bookedByName`, `notes`).
- Empty state displays when no resources match the filter.
- Subscription reconnect warning appears when connection drops.

---

## Validation Checklist

- [ ] All 6 status values displayed with correct labels and colours.
- [ ] Filters narrow results; clearing a filter restores resources.
- [ ] Pagination loads next page on "Load more" click.
- [ ] Subscription pushes status change within 5 s of a booking being made.
- [ ] Restricted user (Marketplace/Individual org, non-admin) sees booking windows as opaque blocks only.
- [ ] Admin of the same org sees full booking detail.
- [ ] No resources from other organizations appear regardless of filter manipulation.
- [ ] GraphQL schema files regenerated and committed (no hand-edits).
- [ ] All unit and integration tests pass.
- [ ] British spelling used in all user-facing copy.
