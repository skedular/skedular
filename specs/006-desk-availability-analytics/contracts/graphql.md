# GraphQL Contract: Desk Availability Analytics

**Phase**: 1 — Design  
**Date**: 2026-04-29  
**Source file to modify**: `location/apis/Location.Api/GraphQL/Analytics/`  
**Schema regeneration**: run `scripts/generate-graphql.sh` after changes

---

## New GraphQL Type: `DeskAvailabilityDailySnapshot`

Added to `location/apis/Location.Api/GraphQL/Analytics/`.

```graphql
type DeskAvailabilityDailySnapshot {
  date: DateTime!
  availableCount: Int!
  unavailableCount: Int!
  bookedCount: Int!
  availableDeskNames: [String!]!
  unavailableDeskNames: [String!]!
  bookedDeskNames: [String!]!
}
```

C# class (`DeskAvailabilityDailySnapshot.cs`):

```csharp
[GraphQLName("DeskAvailabilityDailySnapshot")]
public class DeskAvailabilityDailySnapshot
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }
    [GraphQLName("availableCount")] public int AvailableCount { get; set; }
    [GraphQLName("unavailableCount")] public int UnavailableCount { get; set; }
    [GraphQLName("bookedCount")] public int BookedCount { get; set; }
    [GraphQLName("availableDeskNames")] public IEnumerable<string> AvailableDeskNames { get; set; } = [];
    [GraphQLName("unavailableDeskNames")] public IEnumerable<string> UnavailableDeskNames { get; set; } = [];
    [GraphQLName("bookedDeskNames")] public IEnumerable<string> BookedDeskNames { get; set; } = [];
}
```

---

## Modified GraphQL Type: `LocationAnalytics`

Add field to existing `LocationAnalytics.cs`:

```graphql
type LocationAnalytics {
  name: String!
  desksOccupancyPercentage: [DesksOccupancyPercentage!]!
  roomsOccupancyPercentage: [RoomsOccupancyPercentage!]!
  dailyBookingsTotals: [LocationDailyBookingsTotal!]!
  resourceAvailabilitySnapshots: [ResourceAvailabilityDailySnapshot!]! # NEW
}
```

---

## Query Interface

The new field is served by the existing `locationsAnalytics` query — no new query field needed.
The `from` / `until` date range filter already on `locationsAnalytics` gates the snapshot results.

```graphql
query LocationsAnalytics($from: DateTime!, $until: DateTime!, $where: LocationWhereInput) {
  locationsAnalytics(from: $from, until: $until, where: $where) {
    name
    resourceAvailabilitySnapshots {
      date
      availableCount
      unavailableCount
      bookedCount
      availableResourceNames
      unavailableResourceNames
      bookedResourceNames
    }
    desksOccupancyPercentage {
      date
      percentage
    }
    dailyBookingsTotals {
      date
      total
    }
  }
}
```

---

## Authorization

Same `CanViewAnalytics` check as existing `locationsAnalytics` query — no new auth surface.
Handled in `LocationAnalyticsService` before data is fetched.

---

## Breaking Changes

None. The new `deskAvailabilitySnapshots` field is additive. Existing clients unaffected.
