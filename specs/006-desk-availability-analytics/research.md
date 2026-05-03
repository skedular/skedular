# Research: Desk Availability Analytics

**Phase**: 0 — Pre-design research  
**Date**: 2026-04-29  
**Branch**: `006-desk-availability-analytics`

## Existing Analytics Infrastructure

### Decision: Extend `GenerateLocationDailyAnalytics` workflow, do not create a new workflow

- **Decision**: The new `RecordDeskAvailabilitySnapshotAsync` activity is added as a third step inside the existing `GenerateLocationDailyAnalytics` 24-hour loop, after `RecordLocationDesksCountAsync` and `RecordLocationRoomsCountAsync`.
- **Rationale**: The existing workflow already handles per-location daily scheduling, retry policy, and workflow ID management. Creating a parallel workflow adds operational overhead with no benefit. The new activity is independent and idempotent, so it fits naturally in the same loop.
- **Alternatives considered**: Separate `GenerateLocationDeskAvailabilitySnapshot` workflow — rejected because it would duplicate the scheduling contract and complicate the `IWorkflowIdService` without adding value.

### Decision: New entity `DailyDeskAvailabilitySnapshot` — one row per desk per day

- **Decision**: A new `DailyDeskAvailabilitySnapshot` entity stores: `Id`, `LocationId` (FK), `ResourceId` (FK to Resource), `ResourceName` (denormalised snapshot of name at recording time), `Date`, `Classification` (enum: Available / Unavailable / Booked), `CreatedAt`, `DeletedAt`.
- **Rationale**: Per-desk granularity is required by FR-008 (specific desk names per category per day). A single aggregated count row cannot serve that requirement. Denormalising `ResourceName` preserves the historical desk name even if the resource is later renamed or deleted.
- **Alternatives considered**:
  - Extend `DailyDeskCountRecording` with a JSON list of desk states — rejected because it cannot be indexed or queried efficiently per-desk.
  - Three count columns on a single row — rejected because it cannot return desk names (FR-008).
- **Index plan**: Composite index on `(LocationId, Date)` for the primary query pattern; index on `Date` and on `Classification` individually.

### Decision: Idempotency via delete-then-insert per location per day

- **Decision**: On each execution of `RecordDeskAvailabilitySnapshotAsync` for a given `locationId` + `date`, all existing `DailyDeskAvailabilitySnapshot` rows for that location and date are deleted, then fresh rows are inserted.
- **Rationale**: Consistent with the existing `LocationBookingDerivedState.ReplaceDailyRecordingsAsync` pattern which uses the same delete-all-then-insert approach. Ensures correctness on retries and backfill.
- **Alternatives considered**: Upsert — rejected because the per-desk row identity (resource may change) makes upsert key definition ambiguous.

### Decision: Booking classification uses the same gRPC call as `LocationBookingDerivedState`

- **Decision**: The new activity fetches bookings via `BookingService.BookingServiceClient.Admin_GetPaginatedBookingsAsync` (same call used by `LocationBookingDerivedState`). It filters to the snapshot date UTC range.
- **Rationale**: This is the established cross-domain boundary for location→booking data. No new interfaces needed.
- **Known gap addressed**: The existing `LocationBookingDerivedState.GetBookingsAsync` does not filter by booking status, so cancelled bookings are currently included. The new activity MUST add a status filter. The same fix will be applied to `LocationBookingDerivedState` as the P2 bug fix.

### Decision: Resource classification — `Inactive` flag takes precedence

- **Decision**: Classification order: (1) if `Inactive == true` → Unavailable; (2) else if desk has ≥1 active booking on date → Booked; (3) else → Available. "Active booking" means not cancelled.
- **Rationale**: An inactive desk cannot be booked regardless of whether a booking record exists (data anomaly). Inactive takes precedence.
- **Dual-tag handling**: If a resource has both `ResourceDesk` and `ResourceRoom` tags, it is treated as a desk (first tag match wins, `ResourceDesk` checked first). A warning log is emitted. This is consistent with how the P2 bug fix will handle double-counting.

---

## Bug Analysis: `LocationBookingDerivedState`

### Bug 1 — Cancelled bookings included in booking counts

- **Evidence**: `GetBookingsAsync` calls `Admin_GetPaginatedBookingsAsync` with only `LocationIds` in the `Where` clause. No status filter is applied. Cancelled bookings are fetched and counted.
- **Fix**: Add a status/state filter to exclude cancelled bookings when fetching via gRPC. The exact gRPC `BookingWhereInput` field needs verification against the booking proto — check `BookingStatus` enum and `Status` filter field in `BookingWhereInput`.
- **Impact**: Existing `DailyBookingCountRecording`, `DailyDeskBookingCountRecording`, `DailyRoomBookingCountRecording`, and occupancy percentage calculations are all affected.

### Bug 2 — Dual-tagged resource double-counted in desk and room counts

- **Evidence**: `RecordLocationDesksCountAsync` counts resources with any `ResourceDesk` tag. `RecordLocationRoomsCountAsync` counts resources with any `ResourceRoom` tag. A resource with both tags is counted in both.
- **Fix**: For `RecordLocationDesksCountAsync`, exclude resources that also have a `ResourceRoom` tag (or vice versa — pick one canonical resolution). Emit a warning log for any dual-tagged resource encountered.
- **Impact**: `DailyDeskCountRecording` and `DailyRoomCountRecording` may be inflated for locations with dual-tagged resources.

### Bug 3 — Zero-capacity desks produce misleading 0% occupancy

- **Evidence**: In `LocationAnalyticsService`, `if (item.Count == 0) return new LocationDesksOccupancyPercentage { Date = item.Date, Percentage = 0 }`. This silently returns 0% for a location that has no capacity desks — indistinguishable from a location with capacity where no bookings occurred.
- **Fix**: Exclude zero-count desk/room recording days from the occupancy percentage result set entirely, or return `null` for percentage to signal "no capacity data". The GraphQL type will need to be updated to allow a nullable percentage.
- **Impact**: `desksOccupancyPercentage` and `roomsOccupancyPercentage` fields on `LocationAnalytics`.

---

## GraphQL Booking Status Filter — Verified Finding

**Finding**: `BookingWhereInput` in `booking_v1.proto` has NO cancellation/status filter field. The `paymentStatuses` field exists but does not filter by booking cancellation state. The `Booking` proto message exposes `deletedByCustomerId` (non-empty = soft-deleted/cancelled by someone) but no explicit boolean `deleted` or `cancelled` field.

**Implication for bug fix**: The fix for Bug 1 cannot be applied at the gRPC query level. It must be applied client-side in the activity: after fetching bookings, filter out any booking where `deletedByCustomerId` is non-empty. This must be verified against actual booking cancellation semantics — specifically whether `deletedByCustomerId` is the correct field to use as a cancellation signal, or whether `Admin_GetPaginatedBookings` already excludes soft-deleted records server-side (in which case the "bug" might be about a different booking state such as approval-rejected bookings).

**Action required during implementation**: Read the `BookingService` implementation in `booking/apis/` or the generated booking gRPC server to confirm what states `Admin_GetPaginatedBookings` returns and which field(s) distinguish active from cancelled bookings. Only then apply the client-side filter.

---

## Technology Choices Confirmed

| Concern           | Decision                                                                             | Source                                              |
| ----------------- | ------------------------------------------------------------------------------------ | --------------------------------------------------- |
| Workflow          | Extend `GenerateLocationDailyAnalytics`                                              | Existing pattern                                    |
| Activity          | New `RecordDeskAvailabilitySnapshotAsync` in `LocationDailyAnalytics` class          | Existing sibling activities                         |
| Persistence       | New EF entity + migration + repository                                               | All existing analytics entities follow this pattern |
| Repository access | `IRepositoryFactory`                                                                 | All existing activities use this                    |
| Cross-domain data | gRPC `BookingService.BookingServiceClient`                                           | Used by `LocationBookingDerivedState`               |
| GraphQL           | New field on `LocationAnalytics` type, new GraphQL type `DeskAvailabilitySnapshot`   | Existing analytics query pattern                    |
| REST (backfill)   | New endpoint in `location_analytics_v1.yaml`                                         | Existing regeneration endpoint pattern              |
| Workflow ID       | New method on `IWorkflowIdService` only if a new workflow is added (not needed here) | Existing `WorkflowIdService`                        |
| Auth              | `CanViewAnalytics` permission (existing)                                             | Confirmed in clarifications                         |
