# Data Model: Resource Availability Dashboard

**Phase**: 1 — Design  
**Date**: 2026-05-10  
**Feature**: [spec.md](spec.md) | [research.md](research.md)

---

## Overview

The dashboard is a **real-time computed view** — no new database tables, no snapshots, and no new gRPC or OpenAPI contracts. The only new external API surface is GraphQL. `ResourceDayView` and its child `BookingWindow` records are assembled at query time by reading directly from the **Booking domain DB**:

1. Booking records for the requested date and resource set — queried directly from the Booking DB
2. Location and resource metadata (name, floor, zone, type, opening hours) — available within the booking domain's existing data model
3. Availability state computed by the new `ResourceAvailabilityDayViewService` in `Booking.Shared`

The `DailyResourceAvailabilitySnapshot` table (from 006) is analytics-only and is **never read** by the dashboard. No cross-domain gRPC call is made.

---

## Extended Classification Enum

**File**: `booking/shared/Booking.Shared/Models/ResourceAvailabilityClassification.cs`  
**Note**: The existing `ResourceAvailabilityClassification` enum in `Location.Shared` (3 states, used for analytics snapshots) remains unchanged. The dashboard uses a new, separate 6-state enum in `Booking.Shared` to avoid a Location → Booking dependency.

```csharp
public enum ResourceAvailabilityClassification
{
    Available,
    Unavailable,
    PartiallyBooked,  // at least one booking but free time remains in opening hours
    FullyBooked,      // all opening-hour time covered by bookings
    Occupied,         // checked-in (current date only, where check-in data available)
    Blocked           // location ClosedDates or resource IsActive == false (v1)
}

public static class ResourceAvailabilityClassificationConstants
{
    public const string Available       = "AVAILABLE";
    public const string Unavailable     = "UNAVAILABLE";
    public const string PartiallyBooked = "PARTIALLY_BOOKED";
    public const string FullyBooked     = "FULLY_BOOKED";
    public const string Occupied        = "OCCUPIED";
    public const string Blocked         = "BLOCKED";
}
```

**Precedence rule** (evaluated in order, first match wins):

```
Blocked > Occupied > FullyBooked > PartiallyBooked > Unavailable > Available
```

---

## Computed Models (no DB tables)

### ResourceDayView

Represents one resource's computed state for a selected date. Returned by the new GraphQL query and subscription. Never persisted.

```csharp
public sealed record ResourceDayView
{
    public required string ResourceId         { get; init; }
    public required string ResourceName       { get; init; }
    public required string ResourceType       { get; init; }   // tag constant string
    public required string LocationId         { get; init; }
    public required string LocationName       { get; init; }
    public required string? FloorId           { get; init; }
    public required string? FloorName         { get; init; }
    public required string? ZoneId            { get; init; }
    public required string? ZoneName          { get; init; }
    public required DateOnly Date             { get; init; }
    public required ResourceAvailabilityClassification Status { get; init; }
    public required TimeOnly? OpeningFrom     { get; init; }   // null = closed all day
    public required TimeOnly? OpeningUntil    { get; init; }   // null = closed all day
    public required int TotalOpeningMinutes   { get; init; }
    public required int BookedMinutes         { get; init; }
    public required IReadOnlyList<BookingWindow> BookingWindows { get; init; }
    // BookingWindows may be empty-detail list for non-admin users in Marketplace/Individual orgs
}
```

### BookingWindow

One booking time window within a resource's day view. Detail fields are redacted (nulled) for non-admin users in Marketplace/Individual organisations.

```csharp
public sealed record BookingWindow
{
    public required string BookingId         { get; init; }
    public required TimeOnly From            { get; init; }
    public required TimeOnly Until           { get; init; }
    public required bool IsRecurring         { get; init; }
    public required bool IsCheckedIn         { get; init; }

    // Detail fields — null when visibility is restricted by org type + role
    public required string? BookedByName     { get; init; }
    public required string? BookedByUserId   { get; init; }
    public required string? Notes            { get; init; }
}
```

### ResourceAvailabilityDayFilter

Input model for the GraphQL filter argument.

```csharp
public sealed record ResourceAvailabilityDayFilter
{
    public required DateOnly Date                                      { get; init; }
    public string? LocationId                                          { get; init; }
    public string? FloorId                                             { get; init; }
    public string? ZoneId                                              { get; init; }
    public string? ResourceType                                        { get; init; }  // tag constant
    public ResourceAvailabilityClassification? Status                  { get; init; }
    public string? OrganisationId                                      { get; init; }  // tenancy scope
}

public enum ResourceAvailabilityOrderByField
{
    ResourceName,
    ResourceType,
    LocationName,
    FloorName,
    ZoneName,
    Status,
}

public sealed record ResourceAvailabilityOrderBy(
    ResourceAvailabilityOrderByField Field,
    bool Descending = false);
```

---

## Service: ResourceAvailabilityDayViewService

**File**: `booking/shared/Booking.Shared/Services/ResourceAvailabilityDayViewService.cs`

### Responsibilities

1. Accept a `ResourceAvailabilityDayFilter` and pagination cursor.
2. Query booking records and associated resource/location data directly from the Booking DB for the requested date, location, and resource filters.
3. Compute effective opening hours from location data available within the Booking domain's existing data model.
4. For each resource, filter out cancelled bookings and compute status using the precedence rule (see Classification Logic below).
5. Compute `ResourceAvailabilityClassification` using the precedence rule.
6. Apply booking detail visibility filter based on org type + user role.
7. Sort results by the requested `ResourceAvailabilityOrderBy` field and direction (default: `ResourceName ASC`).
8. Return a `ResourceDayViewResult` with the full sorted list and the `SubscriptionKey`.

### Method Signature

```csharp
public interface IResourceAvailabilityDayViewService
{
    Task<ResourceDayViewResult> GetAsync(
        ResourceAvailabilityDayFilter filter,
        ResourceAvailabilityOrderBy? orderBy,
        string requestingUserId,
        IReadOnlyList<string> requestingUserRoles,
        CancellationToken cancellationToken = default);
}
```

### Classification Logic (pseudocode)

```
foreach resource in page:
  location = fetch Location with OpeningHours
  effectiveHours = ComputeEffectiveOpeningHours(location.OpeningHours, filter.Date)

  if resource.IsActive == false OR date in location.OpeningHours.ClosedDates:
    status = Blocked/Unavailable  // see precedence
  else if effectiveHours == null OR effectiveHours.Closed:
    status = Unavailable
  else:
    bookings = query Booking DB for (resourceId, date), filter cancelled
    filter out cancelled bookings (deletedByCustomerId != null)
    bookedMinutes = sum intersection of each booking window with opening hours window
    totalMinutes = effectiveHours.Until - effectiveHours.From (minutes)

    if any booking IsCheckedIn and date == today:
      status = Occupied
    else if bookedMinutes >= totalMinutes:
      status = FullyBooked
    else if bookedMinutes > 0:
      status = PartiallyBooked
    else:
      status = Available
```

---

## Existing Entities (accessible within Booking domain)

### Resource (existing)

Relevant fields for the dashboard:

- `Id`, `Name`, `IsActive`
- `OrganizationTags` → determines resource type (Desk, Room, Parking, etc.)
- `LocationId` → FK to Location
- `FloorId` (if floor model exists), `ZoneId` (if zone model exists)

### Location (existing)

Relevant fields:

- `Id`, `Name`
- `OpeningHours` (JSON column) — `WeekOpeningHours`, `DatesWithVariedOpeningHours`, `ClosedDates`
- `OrganisationId` → tenancy scope

### DailyResourceAvailabilitySnapshot (existing, 006)

Unchanged. Used only by analytics/reporting queries, not by the dashboard.

---

## State Transitions

```
[Query time for a given resource + date]

         ┌──────────────────────────────────────────┐
         │         Resource.IsActive == false        │
         │    OR date in Location.ClosedDates        │──► Blocked
         └──────────────────────────────────────────┘
                          │ no
                          ▼
         ┌──────────────────────────────────────────┐
         │  effectiveOpeningHours is null or Closed  │──► Unavailable
         └──────────────────────────────────────────┘
                          │ no
                          ▼
         ┌──────────────────────────────────────────┐
         │  date == today AND any booking IsCheckedIn│──► Occupied
         └──────────────────────────────────────────┘
                          │ no
                          ▼
              query Booking DB, filter cancelled bookings
                          │
              compute bookedMinutes vs totalMinutes
                          │
              ┌───────────┼──────────────┐
         0 mins          partial      ≥ total
              │             │             │
           Available  PartiallyBooked FullyBooked
```

---

## Subscription Event Model

### Design: Backend-Generated Opaque Subscription Key

The subscription key is **generated by the backend** and returned as part of the query result. The client treats it as an opaque string and passes it back to subscribe — the client never constructs or interprets the key format.

**Query → Subscribe flow**:

1. Client sends `resourceDayViews(filter: {...})` query.
2. Backend computes a deterministic `subscriptionKey` from the canonicalised filter fields (`locationId`, `floorId`, `zoneId`, `resourceType`, `organisationId`, `date`) — e.g. a URL-safe base64 of a SHA-256 hash of the canonical JSON.
3. `ResourceDayViewConnection` includes this `subscriptionKey: String!` field in its response.
4. Client immediately opens `onResourceAvailabilityChanged(subscriptionKey: String!)` using the received key.
5. When a booking event fires in `Booking.Api`, the server derives all matching subscription keys from the booking's own attributes (locationId, floorId, zoneId, resourceType, date) — up to 16 permutations for all null-dimension combinations — and calls `GraphQlTopicEventSender.RaiseGraphqlChangeAsync` for each matching key.
6. The subscription resolver re-queries the full filtered `ResourceDayView[]` for that key's filter and pushes it to the subscriber.

**No-filter state**: when all filter fields are null (initial load, all resources shown), the server generates a key encoding the organisation-scoped empty filter. Booking events fire to this key on every mutation for the tenant, so empty-filter subscribers receive updates too.

**Key generation (C#)**:

```csharp
// booking/shared/Booking.Shared/Services/SubscriptionKeyService.cs
public static class SubscriptionKeyService
{
    public static string Compute(ResourceAvailabilityDayFilter filter)
    {
        var canonical = JsonSerializer.Serialize(new
        {
            org   = filter.OrganisationId ?? string.Empty,
            loc   = filter.LocationId     ?? string.Empty,
            floor = filter.FloorId        ?? string.Empty,
            zone  = filter.ZoneId         ?? string.Empty,
            type  = filter.ResourceType   ?? string.Empty,
            date  = filter.Date.ToString("yyyy-MM-dd"),
        });
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return WebEncoders.Base64UrlEncode(hash);
    }

    /// <summary>
    /// Returns all keys that must be notified when a booking changes.
    /// Covers every null-dimension permutation so subscribers with partial
    /// filters (e.g. locationId only, no floorId) still receive the event.
    /// </summary>
    public static IEnumerable<string> AffectedKeys(
        string organisationId, string locationId,
        string? floorId, string? zoneId, string? resourceType,
        DateOnly date)
    {
        // Enumerate all 2^3 = 8 permutations of (floorId, zoneId, resourceType)
        // combined with the fixed (organisationId, locationId, date).
        foreach (var floor in Variants(floorId))
        foreach (var zone  in Variants(zoneId))
        foreach (var type  in Variants(resourceType))
            yield return Compute(new ResourceAvailabilityDayFilter
            {
                OrganisationId = organisationId,
                LocationId     = locationId,
                FloorId        = floor,
                ZoneId         = zone,
                ResourceType   = type,
                Date           = date,
            });
    }

    private static string?[] Variants(string? v) => [v, null];
}
```

**Trigger call** (per booking mutation in `Booking.Api`):

```csharp
foreach (var key in SubscriptionKeyService.AffectedKeys(
    booking.OrganisationId, booking.LocationId,
    booking.FloorId, booking.ZoneId, booking.ResourceType,
    booking.Date))
{
    await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
        key, booking.Date, cancellationToken);
}
```

No Kafka consumer. No cross-process hop. No subscriber state stored on the server.

---

## Validation Rules

| Rule                                                                                                  | Where Enforced                                     |
| ----------------------------------------------------------------------------------------------------- | -------------------------------------------------- |
| `Date` must be a valid calendar date                                                                  | GraphQL input scalar validation                    |
| `PageSize` must be between 1 and 100                                                                  | `ResourceAvailabilityDayViewService` guard         |
| `LocationId` (if provided) must belong to the requesting user's accessible organisations              | `ResourceAvailabilityDayViewService` tenancy check |
| Booking windows must be filtered to remove cancelled bookings (`deletedByCustomerId != null`)         | `ResourceAvailabilityDayViewService`               |
| Booking detail fields nulled when org type is Marketplace/Individual and user role is not owner/admin | `ResourceDayViewBookingVisibilityFilter`           |

---

## v2 Enhancements (out of scope for v1)

- `BlockedPeriod` entity: explicit resource-level blocking with reason and date range (requires new EF migration)
- `MaintenanceWindow` entity: structured maintenance tracking
- Resource-level `OpeningHours` override: stored on `Resource` entity; overrides parent `Location.OpeningHours`
- Occupied state from check-in data for future dates (if check-in prediction is added)
