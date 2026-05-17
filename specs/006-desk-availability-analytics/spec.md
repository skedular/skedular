# Feature Specification: Desk Availability Analytics

**Feature Branch**: `006-desk-availability-analytics`  
**Created**: 2026-04-29  
**Status**: Draft  
**Input**: User description: "Review all existing analytics across the relevant domains, identify what analytics already exist, verify whether they are working correctly, and fix any bugs found. Extend the analytics model where it makes sense, with a focus on desk availability and occupancy reporting. Add support for historical daily desk availability snapshots, so the system can report what desks were available, unavailable, booked, or occupied at the end of each day. Design this as a scheduled job that records daily analytics data. The report should support querying a configurable date range, with six months as a key use case. For each day in the selected period, the analytics should show how many desks were available, how many were unavailable, and which specific desks belonged to each category."

## Clarifications

### Session 2026-04-29

- Q: Should the snapshot capture desk state at UTC midnight, or at the close of the location's local business day? → A: UTC midnight for v1. Timezone-aware snapshot timing is out of scope and should be addressed as a follow-up.
- Q: Should "occupied" be a distinct fourth category (implying physical check-in), or synonymous with "booked" for v1? → A: Synonymous — three categories only (available, unavailable, booked). Distinct "occupied" category deferred to a future check-in feature.
- Q: What should the query return for a calendar day that has no snapshot recorded? → A: Omit the day — only return days where a snapshot exists. The client is responsible for handling gaps.
- Q: When the snapshot job runs again for a location + date that already has records, should it replace or skip? → A: Replace (upsert) — delete existing records for that location + date then insert fresh ones. Jobs are executed via Temporal workflows.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Query Historical Desk Availability Report (Priority: P1)

An administrator or manager wants to understand how desks at a location have been used over time. They open the analytics section, select a date range (up to six months), and view a day-by-day breakdown showing how many desks were available, how many were unavailable, how many had bookings, and which specific desks belonged to each category for each day.

**Why this priority**: This is the core business outcome of the feature. Without the ability to query historical availability per day, the feature delivers no value. All other stories support this one.

**Independent Test**: Can be fully tested by seeding a location with desks, running the daily snapshot job for several days, and querying the GraphQL report for that date range. Delivers actionable desk usage insights to operators.

**Acceptance Scenarios**:

1. **Given** a location has 10 desks (8 active, 2 inactive) and daily snapshots have been recorded for the past 30 days, **When** an administrator queries the desk availability report for that location over the past 30 days, **Then** for each day they see: total desks counted, count of available desks, count of unavailable desks, count of booked desks, and the list of desk names in each category.
2. **Given** the same location, **When** the administrator queries a range of exactly six months, **Then** the system returns results for every calendar day in that range without gaps or errors.
3. **Given** no bookings existed on a particular day, **When** the report is queried for that day, **Then** all active desks appear in the "available" category and the "booked" count is zero.
4. **Given** an inactive desk exists at a location, **When** the report is queried for any day, **Then** that desk consistently appears in the "unavailable" category and never in "available" or "booked".
5. **Given** a desk was deleted after a snapshot was recorded, **When** the report is queried for dates before the deletion, **Then** the historical snapshot still reflects the desk correctly (snapshots are immutable once recorded).
6. **Given** the backend snapshot data is available, **When** an administrator opens the Location Insights section of the analytics page and selects a six-month date range for a location, **Then** the desk availability insight card renders a stacked bar chart with three data series (available, unavailable, booked) showing counts per day, consistent with the existing booking and occupancy insight cards.

---

### User Story 2 - Scheduled Daily Desk Availability Snapshot (Priority: P1)

The system automatically records the desk availability state for every location at the end of each day, without requiring manual intervention. This snapshot captures how many and which desks were available, unavailable, and booked as of that day.

**Why this priority**: Without this job running reliably, the P1 query story has no data to serve. The scheduled snapshot is the foundation of historical analytics.

**Independent Test**: Can be fully tested by triggering the snapshot activity for a single location on a specific date and asserting the correct records were persisted, independently of the query layer.

**Acceptance Scenarios**:

1. **Given** a location with active and inactive desks and several bookings on a given day, **When** the daily snapshot job runs for that location and date, **Then** one availability record is persisted per desk, correctly categorised as available, unavailable, or booked.
2. **Given** the snapshot job has already run for a location on a given date, **When** it runs again for the same location and date (e.g., due to a retry), **Then** the result is idempotent — no duplicate records are created; the existing records are replaced or unchanged.
3. **Given** a location has no desks, **When** the snapshot job runs, **Then** no records are created and no error is raised.
4. **Given** a network or downstream failure occurs during the snapshot, **When** the job retries, **Then** the snapshot eventually completes successfully with consistent results.

---

### User Story 3 - Fix Identified Bugs in Existing Location Analytics (Priority: P2)

The current location analytics have several known correctness issues. These must be identified, confirmed against the live code, and fixed so the existing occupancy percentage and booking count data can be trusted.

**Why this priority**: New analytics built on top of broken existing infrastructure will produce misleading reports. Fixing the most critical bugs is a prerequisite to reliable analytics across the board, though the new desk availability snapshot is independent enough to be built even before all bugs are fixed.

**Independent Test**: Each bug fix can be tested independently via targeted unit or integration tests that reproduce the bug condition and assert the corrected behaviour.

**Acceptance Scenarios**:

1. **Given** a resource has both a desk tag and a room tag (data anomaly), **When** analytics are computed, **Then** the resource is counted only once — in the category matching its primary tag — and not duplicated across both desk and room counts.
2. **Given** a desk has zero capacity bookings for a day, **When** occupancy percentage is computed, **Then** the system returns a value that clearly distinguishes "no capacity" from "zero bookings on a non-zero capacity" (i.e., zero-capacity is excluded or flagged rather than silently returning 0%).
3. **Given** bookings are cancelled, **When** the daily booking count is recomputed, **Then** cancelled bookings are not included in the booked count used for occupancy calculations.
4. **Given** `LocationBookingDerivedState.RecomputeAsync` runs for a location, **When** it is triggered twice with the same booking data, **Then** it produces the same daily recording counts without data loss or duplication — the delete-then-insert pattern ensures idempotency.

---

### User Story 4 - Regenerate or Backfill Historical Snapshots (Priority: P3)

An administrator can trigger regeneration of historical desk availability snapshots for a specific location and date range. This is needed after bug fixes, data corrections, or when a location is newly onboarded and historical data needs to be backfilled.

**Why this priority**: Valuable for data integrity but not required for the core daily snapshot flow. Organizations can function with forward-going snapshots while backfill is treated as a recovery tool.

**Independent Test**: Can be tested by invoking the regenerate endpoint for a location and date range, then asserting the snapshots are re-created from the booking and resource state at that time.

**Acceptance Scenarios**:

1. **Given** a location with existing daily snapshots, **When** an administrator triggers backfill for the past 30 days, **Then** all 30 days of desk availability snapshots are regenerated and replace the previous records.
2. **Given** a location has no existing snapshots for a date range, **When** backfill is triggered, **Then** snapshots are created for each day where resource and booking data allows it.

---

### Edge Cases

- What happens when a location has desks on some days but not others (desk added mid-month)? Snapshots should only reflect desks that existed on the snapshot date.
- What happens when the snapshot job runs at UTC midnight but the location's business day ends at a different local time? The snapshot date must be unambiguous — recording which bookings belong to which calendar day in the location's timezone must be explicitly defined.
- How does the system handle a date range query that spans more than six months? No maximum is enforced for v1 — the system accepts any date range. Six months is the key use case and the performance target (SC-007), but longer ranges are not rejected.
- What happens if a snapshot job fails partway through recording desks? The system should not persist a partial snapshot; either all desks for a day are recorded or none (atomic per-location-per-day).
- What happens if a location is deleted after snapshots were recorded? Historical snapshots for the deleted location should remain queryable by administrators who had access at the time.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST record, once per day per location, the availability state of every desk at that location as of the snapshot moment.
- **FR-002**: Each daily desk availability snapshot MUST classify every desk as one of: available, unavailable, or booked.
- **FR-003**: A desk MUST be classified as "unavailable" when the `Inactive` flag on the resource is `true` at snapshot time.
- **FR-004**: A desk MUST be classified as "booked" when it has at least one active (non-cancelled) booking on the snapshot date.
- **FR-005**: A desk MUST be classified as "available" when it is active and has no active bookings on the snapshot date.
- **FR-006**: The snapshot MUST be recorded as of UTC midnight on the snapshot date. Bookings that fall on the calendar date in UTC are included. Timezone-aware snapshot timing is out of scope for v1 and should be addressed as a follow-up.
- **FR-007**: The daily snapshot job MUST be idempotent — running it multiple times for the same location and date MUST produce the same result. On re-execution, existing snapshot records for that location and date MUST be deleted and replaced with freshly computed records. The snapshot job is implemented as a Temporal workflow activity, consistent with the existing `GenerateLocationDailyAnalytics` workflow.
- **FR-008**: The system MUST expose a query interface that accepts a location, a start date, and an end date, and returns, for each calendar day in the range where a snapshot exists: the count of available desks, the count of unavailable desks, the count of booked desks, and the list of desk names in each category. Days with no recorded snapshot MUST be omitted from the results.
- **FR-009**: The query interface MUST support date ranges of up to six months without performance degradation.
- **FR-010**: Snapshots recorded for a given date MUST be immutable once persisted — they MUST NOT be altered by subsequent resource or booking changes (only explicit regeneration changes them).
- **FR-011**: The system MUST provide an administrative trigger to regenerate desk availability snapshots for a specific location and date range.
- **FR-012**: The existing location analytics (desk count, room count, booking count, occupancy percentage) MUST continue to function correctly after this feature is implemented.
- **FR-013**: Existing analytics MUST exclude cancelled bookings from booking counts and occupancy percentage calculations.
- **FR-014**: A resource tagged as both a desk and a room MUST NOT be double-counted in analytics — it MUST be classified using its primary or sole tag only.
- **FR-015**: The system MUST NOT include days where the aggregated `DailyDeskCountRecording.Count` is zero in occupancy percentage calculations, to avoid misleading 0% readings that are indistinguishable from a fully-booked location. This targets the aggregated count row, not individual `Resource.Capacity` values.
- **FR-016**: For v1, "occupied" and "booked" are synonymous. The snapshot uses three categories only: available, unavailable, and booked. A separate "occupied" category (implying physical check-in or sensor data) is out of scope and may be added in a future feature.
- **FR-017**: Access to the desk availability report query MUST be governed by the existing `CanViewAnalytics` permission on the organization, consistent with the existing `locationsAnalytics` query.
- **FR-018**: The system MUST display the desk availability snapshot data in the organization analytics web application, in the existing "Location Insights" section, as a stacked bar chart showing per-day counts for available, unavailable, and booked desks for the selected date range. The component MUST reuse the existing `AnalyticsInsightCard` and `AnalyticsDaterangeSelector` patterns, with six months as the default date range.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The daily snapshot workflow MUST emit structured logs at the start and completion of each snapshot run, including the location identifier and the snapshot date.
- **LOG-002**: The snapshot activity MUST emit structured logs when it classifies desks, including counts per category and the total desks processed.
- **LOG-003**: The snapshot activity MUST emit a warning log when it encounters a resource with ambiguous classification (e.g., tagged as both desk and room), and MUST log the resource identifier and chosen classification.
- **LOG-004**: All snapshot workflow logs MUST include the workflow run identifier and location identifier as structured correlation context, and MUST NOT log desk names or booking contents as sensitive operational data.

### Key Entities _(include if feature involves data)_

- **DailyDeskAvailabilitySnapshot**: Records the classified state of a single desk on a single calendar day. Key attributes: location identifier, desk identifier, desk name, snapshot date, classification (available / unavailable / booked). One record per desk per day per location.
- **Resource** (existing): Represents a bookable space. Relevant attributes for this feature: `Inactive` flag (drives "unavailable" classification), organization tags (identifies desks vs rooms), `Name` (used in report output).
- **DailyDeskCountRecording** (existing): Aggregated desk capacity count per location per day. Not replaced — the new snapshot entity complements it.
- **DailyBookingCountRecording** (existing): Aggregated booking count per location per day. To be corrected to exclude cancelled bookings.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: For any date range of up to six months, the desk availability report returns a result for every calendar day in the range for which a snapshot was recorded. Days with no snapshot are omitted rather than returned as zeros.
- **SC-002**: The daily snapshot job runs to completion for a location with up to 500 desks within a time that does not block or delay other scheduled workflows for the same location.
- **SC-003**: Re-running the snapshot job for the same location and date zero, one, or multiple times always produces the same final state — verified by automated tests.
- **SC-004**: All existing analytics unit and integration tests continue to pass after bug fixes are applied.
- **SC-005**: The cancellation bug is verified by an automated test that confirms cancelled bookings are excluded from booking counts used in occupancy calculations.
- **SC-006**: The double-counting bug is verified by an automated test confirming a dual-tagged resource appears in only one analytics category.
- **SC-007**: Querying six months of daily desk availability data for a single location completes and returns results without timeout.
- **SC-008**: The desk availability report correctly identifies which specific desks were in each category on any given day, verified by integration tests against known seed data.

## Assumptions

- "Occupied" and "booked" are treated as synonymous for v1. The current system has no physical check-in or occupancy sensor infrastructure. "Occupied" as a distinct category (implying a check-in event) is out of scope unless clarified otherwise.
- The snapshot captures desk state based on bookings that fall on the calendar date in UTC (per FR-006). Timezone-aware snapshot timing is deferred to a follow-up feature.
- A desk's classification for a snapshot is determined at the point the job runs (end of business day / midnight). Bookings that are created or cancelled after the snapshot is taken do not retroactively alter it.
- The six-month query range is a key use case but not a hard system limit. The system should support it efficiently; longer ranges are technically possible but not a primary concern for v1.
- "Inactive" desks are always "unavailable" regardless of whether they have historical bookings. A desk taken out of service mid-month is unavailable for the days after it was deactivated.
- The location domain is the correct home for desk availability analytics. Booking domain provides booking data via the existing cross-domain gRPC call already used by `LocationBookingDerivedState`.
- The existing `GenerateLocationDailyAnalytics` Temporal workflow is extended (not replaced) to also record the new per-desk snapshots, maintaining the existing scheduling contract. The snapshot activity follows the same Temporal activity pattern used by other location analytics activities.
- Backfill / regeneration reuses the same activity logic as the daily snapshot, invoked for a date range rather than a single date.
- Mobile and real-time availability are out of scope for this feature.
- Organization-level cross-location desk availability aggregation (e.g., "across all locations, how many desks were available?") is out of scope for v1.
- The web UI component follows the existing `LocationDeskOccupancyInsight` pattern: a root wrapper component handles data loading via Relay, and an inner component renders the chart. No new design system components are required.
