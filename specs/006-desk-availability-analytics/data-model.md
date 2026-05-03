# Data Model: Desk Availability Analytics

**Phase**: 1 — Design  
**Date**: 2026-04-29  
**Branch**: `006-desk-availability-analytics`

## New Entity: `DailyDeskAvailabilitySnapshot`

Stores the classified availability state of a single desk on a single calendar day for a location.

### Fields

| Field            | Type                                    | Constraints                  | Notes                                                                   |
| ---------------- | --------------------------------------- | ---------------------------- | ----------------------------------------------------------------------- |
| `Id`             | `string`                                | PK, non-null                 | Follows `EntityBaseWithDeleted` pattern (random ID via `IRandomHelper`) |
| `LocationId`     | `string`                                | FK → `Location.Id`, non-null | Navigated via `Location` nav property                                   |
| `Location`       | `Location` (nav)                        | Required                     | EF navigation; `WithMany(l => l.DailyDeskAvailabilitySnapshots)`        |
| `ResourceId`     | `string`                                | non-null                     | Denormalised snapshot — resource may be deleted later                   |
| `ResourceName`   | `string`                                | max 255, non-null            | Denormalised snapshot of desk name at recording time                    |
| `Date`           | `DateTimeOffset`                        | non-null, indexed            | UTC midnight of the snapshot date                                       |
| `Classification` | `DeskAvailabilityClassification` (enum) | non-null                     | `Available` / `Unavailable` / `Booked`                                  |
| `CreatedAt`      | `DateTimeOffset`                        | non-null                     | Set by repository on insert                                             |
| `DeletedAt`      | `DateTimeOffset?`                       | nullable                     | Soft-delete; set on replace                                             |

### Enum: `DeskAvailabilityClassification`

```
Available   = 0
Unavailable = 1
Booked      = 2
```

Stored as `int` in PostgreSQL (existing pattern for enums in this domain).

### Indexes

- Composite: `(LocationId, Date)` — primary query pattern (all desks for a location on a date range)
- Single: `Date` — for date-range queries without location filter (admin/backfill use)
- Single: `Classification` — for filtering by category in reporting queries

### EF Configuration

Follows `DailyDeskCountRecordingConfiguration` pattern:

- `builder.ConfigureEntityBaseWithDeleted()`
- `builder.HasOne(item => item.Location).WithMany(item => item.DailyDeskAvailabilitySnapshots)`
- `builder.Property(item => item.ResourceName).HasMaxLength(255)`
- `builder.Property(item => item.Classification).HasConversion<int>()`

### Migration

New EF Core migration: `AddDailyDeskAvailabilitySnapshot`  
Location: `location/shared/Location.Shared/Database/Migrations/`

---

## Modified Entity: `Location`

Add navigation collection:

```csharp
public virtual ICollection<DailyDeskAvailabilitySnapshot> DailyDeskAvailabilitySnapshots { get; set; } = [];
```

---

## Modified Entity: `LocationDbContext`

Add `DbSet`:

```csharp
public DbSet<DailyDeskAvailabilitySnapshot> DailyDeskAvailabilitySnapshot { get; set; }
```

---

## New Repository: `DailyDeskAvailabilitySnapshotRepository`

Extends `RepositoryBase<LocationDbContext, DailyDeskAvailabilitySnapshot>`.

### Interface methods

```csharp
// Add a single snapshot record (sets CreatedAt)
DailyDeskAvailabilitySnapshot Add(DailyDeskAvailabilitySnapshot snapshot);

// Delete all snapshots for a location on a specific date (for idempotent replace)
Task DeleteByLocationAndDateAsync(string locationId, DateTimeOffset date, CancellationToken cancellationToken);

// Query all snapshots for a location over a date range (returns only non-deleted)
Task<ICollection<DailyDeskAvailabilitySnapshot>> GetByLocationIdAndDateRangeAsync(
    string locationId,
    DateTimeOffset from,
    DateTimeOffset until,
    CancellationToken cancellationToken);
```

---

## Existing Entities: Bug-Fix Changes (no migration needed)

### `DailyBookingCountRecording` / `DailyDeskBookingCountRecording` / `DailyRoomBookingCountRecording`

No schema changes. Bug fixes are in the `LocationBookingDerivedState` activity:

1. **Bug 1 (cancelled bookings)**: Apply client-side filter after gRPC fetch. Exact filter field (`deletedByCustomerId` or other) to be confirmed during implementation (see `research.md`). The `GetBookingsAsync` private method must be updated to accept an optional date filter (`fromGte` / `fromLt`) so the snapshot activity can scope to a single day.

2. **Bug 2 (dual-tagged resource)**: In `RecordLocationDesksCountAsync`, exclude resources that also carry a `ResourceRoom` tag. In `RecordLocationRoomsCountAsync`, exclude resources that also carry a `ResourceDesk` tag. Log a warning with the resource ID when encountered.

3. **Bug 3 (zero-capacity occupancy)**: In `LocationAnalyticsService`, change the zero-count guard to skip the day entirely instead of returning 0%:
   ```csharp
   // Before:
   if (item.Count == 0) return new LocationDesksOccupancyPercentage { Date = item.Date, Percentage = 0 };
   // After:
   if (item.Count == 0) continue; // or filter via .Where(item => item.Count > 0) before the .Select
   ```
   The `desksOccupancyPercentage` and `roomsOccupancyPercentage` GraphQL fields remain non-nullable in their list element type (the list just omits zero-capacity days).

---

## New Model: `DeskAvailabilitySnapshotReport` (read model for analytics service)

A read-model aggregate used by `ILocationAnalyticsService` to return desk availability data:

```csharp
public record DeskAvailabilitySnapshotReport(
    DateTimeOffset Date,
    int AvailableCount,
    int UnavailableCount,
    int BookedCount,
    IReadOnlyList<string> AvailableDeskNames,
    IReadOnlyList<string> UnavailableDeskNames,
    IReadOnlyList<string> BookedDeskNames
);
```

---

## State Transitions

```
Daily snapshot lifecycle per (LocationId, Date):
  Not yet recorded
    → [RecordDeskAvailabilitySnapshotAsync runs]
    → Recorded (one row per desk, Classification set)

  Recorded
    → [RecordDeskAvailabilitySnapshotAsync runs again (retry/backfill)]
    → All existing rows soft-deleted (DeletedAt set)
    → New rows inserted with fresh classification
    → Recorded (replaced)
```

Classification decision logic per desk:

```
Resource.Inactive == true           → Unavailable
Resource has active booking on date → Booked
else                                → Available
```
