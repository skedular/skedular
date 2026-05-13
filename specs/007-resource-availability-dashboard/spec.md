# Feature Specification: Resource Availability Dashboard

**Feature Branch**: `007-resource-availability-dashboard`  
**Created**: 2026-05-10  
**Status**: Draft

## User Scenarios & Testing _(mandatory)_

### User Story 1 – View Resource Availability for a Selected Date (Priority: P1)

A co-working space owner or organisation administrator opens the Resource Availability Dashboard, selects a date (today, a past date, or a future date), and immediately sees the availability state of every resource and desk for that date. Each resource is clearly labelled as available, booked, occupied, unavailable, or blocked.

**Why this priority**: This is the core value of the dashboard. Without date-based availability visibility, the feature does not exist.

**Independent Test**: Navigate to the dashboard, pick any date, and confirm that all resources are listed with a correct availability state that matches the underlying booking and schedule data.

**Acceptance Scenarios**:

1. **Given** the user is on the dashboard with no filters applied, **When** they select today's date, **Then** each accessible resource is displayed with its day-level status and all individual bookings for that date listed against it.
2. **Given** the user selects a future date, **When** the data loads, **Then** resources show their projected day-level status and advance bookings based on the resource's effective opening hours for that date.
3. **Given** the user selects a past date, **When** the data loads, **Then** resources show their historical bookings and the derived day-level status for that date.
4. **Given** a resource has one or more bookings that together cover all of its effective opening hours for the selected date, **When** the user views the dashboard, **Then** the resource shows "Fully Booked".
5. **Given** a resource has one or more bookings but free opening-hour time remains, **When** the user views the dashboard, **Then** the resource shows "Partially Booked" and the free windows are visible.
6. **Given** a resource is under a maintenance window or blocked period for the selected date, **When** the user views the dashboard, **Then** the resource appears in the "Blocked" state regardless of any bookings.

---

### User Story 2 – Filter Resources by Location, Floor, Zone, Type, and Status (Priority: P1)

The user opens the dashboard and sees all accessible resources immediately, with all filter controls visible but empty. They then apply one or more filters to narrow the list, reducing noise for large co-working spaces.

**Why this priority**: Large organisations and co-working spaces can have hundreds or thousands of resources. Filtering is essential for usability at scale, but the full unfiltered view must remain the starting point so users can explore without being forced to know the location hierarchy upfront.

**Independent Test**: Open the dashboard and confirm all accessible resources are shown; then apply a single filter (e.g., a specific location) and confirm the dashboard shows only resources belonging to that location with correct statuses.

**Acceptance Scenarios**:

1. **Given** the user opens the dashboard, **When** the page loads, **Then** all filter controls are visible and empty, and all accessible resources are displayed with their day-level availability status.
2. **Given** all resources are shown, **When** the user selects a specific location filter, **Then** only resources belonging to that location are displayed.
3. **Given** a location is selected, **When** the user further selects a floor within that location, **Then** only resources on that floor are displayed.
4. **Given** the user selects the "Available" status filter, **When** the dashboard refreshes, **Then** only resources in the available state for the selected date are shown.
5. **Given** multiple filters are applied simultaneously, **When** the data loads, **Then** only resources matching all applied filters are displayed.
6. **Given** a filter is cleared, **When** the dashboard refreshes, **Then** resources that were previously hidden by that filter reappear. Clearing all filters restores the full unfiltered resource list.

---

### User Story 3 – Real-Time Availability Updates via Subscription (Priority: P2)

While the user has the dashboard open, availability changes (new bookings, cancellations, check-ins, blocks) are pushed to the dashboard in real time via a GraphQL subscription, without requiring a full page reload or periodic polling.

**Why this priority**: A stale dashboard undermines confidence in the availability data, particularly for busy co-working spaces during peak hours. Subscriptions are the established real-time pattern in this platform and provide lower latency than polling.

**Independent Test**: Open the dashboard, establish the GraphQL subscription, make a booking for a visible resource from another session, and confirm the affected resource's day-level status updates immediately on the dashboard without a reload.

**Acceptance Scenarios**:

1. **Given** the dashboard is open and the GraphQL subscription is active, **When** a booking is created for a visible resource, **Then** the server pushes the updated resource status to the client and the dashboard reflects the change without a full page reload.
2. **Given** the dashboard is open and the subscription is active, **When** a booking is cancelled, **Then** the server pushes the updated resource status and the dashboard updates accordingly.
3. **Given** the dashboard is open and the subscription is active, **When** a resource is blocked or a maintenance window is added, **Then** the resource's status updates to "Blocked" on the dashboard immediately.
4. **Given** the subscription connection is lost (network error), **When** the connection drops, **Then** the dashboard shows a non-intrusive warning that live updates are paused and attempts to reconnect. On reconnect, the dashboard refreshes to the current state.
5. **Given** the user is viewing any date (past, present, or future), **When** a booking change occurs for a resource in the current view, **Then** the subscription pushes the update regardless of the selected date.

---

### User Story 4 – Navigate Large Resource Lists with Pagination (Priority: P2)

When a location or space contains a large number of resources, the dashboard loads them incrementally (via pagination or infinite scroll) rather than attempting to load all data at once.

**Why this priority**: Scalability for large co-working spaces requires the backend and frontend to avoid loading unbounded data sets.

**Independent Test**: With a space containing more than 50 resources, open the dashboard and confirm that the initial load is fast, and additional resources load on demand.

**Acceptance Scenarios**:

1. **Given** a location has more than 50 resources, **When** the dashboard loads, **Then** the first page of results renders quickly and remaining pages are loaded on demand.
2. **Given** the user scrolls to the bottom of the resource list, **When** more resources exist, **Then** the next page loads automatically or a "Load more" control is shown.
3. **Given** all resources for the current filter set have been loaded, **When** the user is at the bottom, **Then** no further pagination requests are made.

---

### User Story 5 – Authorised Access and Booking Visibility (Tenancy and Role Enforcement) (Priority: P1)

A user can only view resources that belong to their organisation or the locations they are authorised to access. Additionally, the level of booking detail visible depends on the organisation type and the user's role.

**Why this priority**: Multi-tenancy data isolation and role-scoped booking visibility are non-negotiable security requirements.

**Independent Test**: Log in as a regular user of a co-working space organisation, open the dashboard, and confirm that booked windows appear as unavailable but no other user's booking details are shown. Log in as an administrator of the same organisation and confirm all booking details are visible.

**Acceptance Scenarios**:

1. **Given** a user belongs to Organisation A, **When** they open the dashboard, **Then** only resources within Organisation A's accessible locations are visible.
2. **Given** a user does not have access to a specific location, **When** they attempt to filter by that location, **Then** no resources are returned and no data from that location leaks.
3. **Given** a read-only user role, **When** they open the dashboard, **Then** they can view resource availability but cannot initiate bookings or changes from the dashboard.
4. **Given** the organisation is of type **Private**, **When** any user opens the dashboard, **Then** all booking details for all resources are visible to all users within that organisation.
5. **Given** the organisation is of type **Co-working Space**, **Marketplace**, or **Individual**, **When** a regular (non-admin, non-owner) user opens the dashboard, **Then** booked time windows appear as occupied/unavailable but the identity and details of those bookings are not shown.
6. **Given** the organisation is of type **Co-working Space**, **Marketplace**, or **Individual**, **When** an owner or administrator opens the dashboard, **Then** full booking details across all users are visible.

---

### Edge Cases

- What happens when no resources exist for the selected filters and date? The dashboard displays an empty state with a clear message.
- What happens when the selected date is more than a configured horizon in the future and no schedule data exists? Resources without advance booking data show as "Available" by default unless explicitly blocked.
- What happens if availability calculation produces an ambiguous state (e.g., a resource is both booked and blocked)? The most restrictive state takes precedence: Blocked > Occupied > Fully Booked > Partially Booked > Unavailable > Available.
- What happens when a resource's opening hours are not configured? The resource falls back to the location's opening hours. If neither is set, the resource is treated as available for the full 24-hour day for booking-coverage calculations.
- What happens when a resource has no opening hours on the selected date (e.g., closed on weekends)? The resource shows as "Unavailable" regardless of any bookings.
- What happens when a user applies filters that result in zero resources? An empty state is shown with guidance to adjust filters.
- What happens if the GraphQL subscription connection drops (network error)? The dashboard remains usable with the last-known data and shows a non-intrusive warning that live updates are paused. The client automatically attempts to reconnect and refreshes the full state on reconnection.

---

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The dashboard MUST allow users to select any date (past, present, or future) and display each accessible resource alongside all of its individual bookings for that date. Bookings may be of any duration (e.g., 30 minutes, half-day, full day). The resource row MUST make it clear whether any free time remains within the resource's effective opening hours for that day.
- **FR-002**: Each resource MUST display a computed day-level status derived from the relationship between its bookings and its effective opening hours for the selected date. Status values are: **Available** (no bookings within opening hours), **Partially Booked** (at least one booking exists but free opening-hour time remains), **Fully Booked** (all opening-hour time is covered by bookings), **Occupied** (checked-in on the current date), **Unavailable** (closed or outside opening hours for that date), or **Blocked** (explicit block or maintenance window). The precedence for conflicting states is: Blocked > Occupied > Fully Booked > Partially Booked > Unavailable > Available.
- **FR-003**: The availability state MUST be calculated using: the resource's effective opening hours (inherited from the location or overridden at the resource level), one-off bookings, recurring booking instances, explicit blocked periods, and maintenance windows for the selected date.
- **FR-004**: The dashboard MUST support filtering by: location, floor, zone, resource type, and resource status. All filter controls MUST be visible and empty on initial load, with all accessible resources displayed immediately.
- **FR-005**: Filters MUST be combinable (AND logic) and individually clearable without resetting other active filters. Clearing all filters returns the view to the full unfiltered resource list.
- **FR-006**: The backend MUST expose the dashboard data via GraphQL, including filtering and sorting. The query returns all matching resources in a single response; no pagination is applied.
- **FR-007**: ~~Removed~~ — pagination is not implemented; the query always returns the full filtered result set. Performance is managed via efficient DB queries and the filter dimensions (location, floor, zone, type, status) that naturally scope the result set.
- **FR-008**: The dashboard MUST support real-time availability updates via GraphQL subscriptions. When a booking is created, modified, or cancelled, or when a resource's blocked/maintenance state changes, the server MUST push the updated day-level status of the affected resource(s) to all subscribed clients. The subscription MUST be scoped to an organisation or location so clients receive only updates relevant to the resources currently displayed. Existing GraphQL subscription implementations in the booking domain MUST be reviewed and reused or followed as the pattern.
- **FR-009**: Tenancy boundaries MUST be enforced at the API layer so a user can only query resources belonging to their authorised organisations and locations.
- **FR-009a**: Booking visibility on the dashboard MUST be governed by the organisation type of the resource's owning organisation:
  - **Private organisation**: all users within the organisation can see all bookings for resources.
  - **Co-working Space, Marketplace, or Individual organisation**: only owners and administrators can see the full booking details for all users. Regular users can see that a resource is booked (i.e., unavailable during that window) but MUST NOT see booking details belonging to other users.
- **FR-010**: Role-based access control MUST be applied; read-only users can view the dashboard but cannot perform booking actions from it.
- **FR-011**: The backend availability logic MUST be consolidated with or reused from existing floor plan and analytics availability logic to avoid duplicate implementations.
- **FR-012**: The dashboard UI MUST follow the design system and frontend architecture patterns currently used in the project (typography wrappers, MUI v9, Relay, Next.js App Router).
- **FR-013**: The solution MUST include unit tests for availability calculation logic, integration tests for GraphQL queries, and component tests for the dashboard UI.
- **FR-014**: All GraphQL schema changes, DTOs, API shared clients, and TypeScript generated types MUST be updated and kept in sync.
- **FR-015**: The dashboard MUST complement (not replace) the existing floor plan view; deep-linking or navigation between the two views MUST be supported.
- **FR-016**: When viewing the current date, the dashboard MUST distinguish between "Occupied" (checked in) and "Booked" (reserved but not yet checked in) states where check-in data is available.
- **FR-017**: The implementation MUST document API usage, architectural decisions, and integration points in the feature spec and inline docs.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The availability query workflow MUST emit structured logs for each query including the selected date, applied filters, tenant context, and result count.
- **LOG-002**: The availability state calculation MUST log when a resource transitions between states (e.g., available → booked) during calculation for diagnostic traceability.
- **LOG-003**: GraphQL subscription connection events (established, dropped, reconnecting, reconnected) MUST emit structured logs with correlation identifiers. Subscription errors that cause the client to lose real-time updates MUST emit actionable warning logs.
- **LOG-004**: All logs MUST include a correlation/request identifier and MUST NOT include personally identifiable information or booking content.
- **LOG-005**: Slow queries (exceeding a configurable threshold) MUST emit a warning log with query metadata to support performance monitoring.

### Key Entities

- **Resource**: A bookable asset (desk, meeting room, hot desk, locker, etc.) belonging to a location. Has a type, floor, zone, active/inactive state, and optionally its own opening hours that override the location's opening hours.
- **OpeningHours**: The time window during which a resource (or its parent location) is operational on a given day or day-of-week. A resource's effective opening hours are its own override if set, otherwise the location's opening hours. Availability and "Fully Booked" status are evaluated against effective opening hours only.
- **ResourceDayView**: A computed representation of a resource for a selected date, containing: the day-level status, the effective opening hours, and the list of all individual booking windows and blocks for that date. The booking detail content within this view is filtered according to the requesting user's role and the organisation type.
- **Organisation**: The owning tenant for a set of locations and resources. Has a type that governs booking visibility rules: **Private** (all users see all bookings), **Co-working Space** (only owners/admins see full booking details), **Marketplace** (only owners/admins see full booking details), or **Individual** (only owners/admins see full booking details).
- **Location**: A physical space (building, floor, zone) that organises resources under a tenant/organisation. Defines default opening hours for all resources within it.
- **Booking**: A confirmed reservation of a resource for a specific time window. May be of any duration (minutes to full day). May be one-off or recurring.
- **RecurringBooking**: A repeating schedule that generates booking instances on matching dates. Affects availability on those dates.
- **BlockedPeriod**: An explicit block applied to a resource (e.g., maintenance, internal hold) that renders the resource unavailable regardless of bookings. Takes precedence over all booking states.
- **AvailabilityFilter**: The set of user-selected criteria (date, location, floor, zone, type, status) used to scope the dashboard query.

---

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: Users can open the dashboard and see availability for all resources at their accessible locations for any selected date within 3 seconds under normal load conditions.
- **SC-002**: Applying one or more filters narrows results and the filtered view loads within 2 seconds.
- **SC-003**: The dashboard supports spaces with at least 500 resources per location without performance degradation, returning the full filtered result set within 3 seconds.
- **SC-004**: Real-time availability changes are pushed to the dashboard via GraphQL subscription and reflected within 5 seconds of the change occurring on the server, for any selected date.
- **SC-005**: 100% of resources a user is authorised to see appear on the dashboard for the selected date; 0% of resources from other tenants or unauthorised locations appear.
- **SC-006**: Unit test coverage for availability calculation logic is sufficient to verify all six states and their precedence rules.
- **SC-007**: Integration tests confirm that GraphQL queries with filters and sorting return correctly scoped results.
- **SC-008**: The dashboard passes accessibility checks for keyboard navigation and screen reader support.

---

## Assumptions

- The existing floor plan view already performs some availability state computation; the new dashboard will reuse or consolidate that logic rather than reimplementing it from scratch.
- Resources are already modelled with location, floor, and zone associations in the existing data model.
- Check-in data (to distinguish "Occupied" from "Booked") is available in the current booking domain for the current date; it may not be available for future dates.
- Real-time updates are delivered via GraphQL subscriptions following the existing subscription pattern established in the booking domain. The subscription transport (WebSocket) is already supported by the platform infrastructure.
- The dashboard is a new dedicated page in the web application and does not replace the floor plan view.
- Users accessing the dashboard already have authenticated sessions; no new authentication mechanism is required.
- The authorisation and tenancy model is already established in the platform; the dashboard consumes the existing rules.
- Mobile-specific optimisation is out of scope for v1; the dashboard is designed for desktop/tablet usage.
- The GraphQL gateway composes existing domain schemas; new fields added to domain schemas will be automatically available through the gateway after schema regeneration.
- Historical availability data (past dates) is computed from historical booking records, not from a pre-computed snapshot store, unless an existing snapshot mechanism is already in place.
- The `006-desk-availability-analytics` feature may have introduced related backend analytics logic; this feature should review and reuse or extend that work rather than duplicating it.

---

## Clarifications

### Session 2026-05-10

- Q: Should the dashboard show availability at day granularity only, or at time-slot granularity within the day? → A: Day granularity primary view; individual time-slot detail revealed on resource selection or hover.
- Q: When a user arrives at the dashboard, should the active location be pre-selected from navigation context, or should all filters start empty? → A: All filters start empty and all accessible resources are displayed immediately; applying a filter narrows the results.
- Q: When a resource is only partially booked during the selected day, what should the day-level status badge display? → A: The dashboard shows each resource with all its individual bookings listed against it for the day. A resource is considered "Fully Booked" only when all its effective opening hours are covered by bookings. Opening hours are inherited from the location or overridden at the resource level. Status must be evaluated against opening hours, not the full 24-hour day.
- Q: When a co-working space owner views the dashboard, should they see all bookings from all member organisations on each resource? → A: Booking visibility is governed by organisation type. In a **Private** organisation, all users can see all bookings for resources. In a **Co-working Space**, **Marketplace**, or **Individual** organisation, only owners and administrators can see all bookings; regular users cannot see other users' bookings.
- Q: Should real-time refresh use polling or another mechanism? → A: Use GraphQL subscriptions, not polling. When a booking or resource status changes, the server pushes the updated resource status back to the client. The subscription scope is at the organisation or location level (subscribe to all resources under an organisation or location). Existing subscription implementations in the booking domain should be reviewed and reused or followed as the pattern.
