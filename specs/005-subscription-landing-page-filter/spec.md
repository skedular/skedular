# Feature Specification: Subscription Landing Page Filtering

**Feature Branch**: `005-subscription-landing-page-filter`  
**Created**: 2026-04-27  
**Status**: Draft  
**Input**: User description: "Create a SpecKit prompt for enhancing the subscription landing page filtering in the coworking space owner Management Portal. The page must support filtering subscriptions by subscription status and payment status. The available values for both filters must come from the backend through the GraphQL API, following the same pattern already used by other backend driven combo boxes in the application. Both filters must be implemented as multi select combo boxes, so inspect the existing multi select combo box implementation and reuse the same UX and technical pattern where possible. If no values are selected for a filter, that filter must not restrict the results and all values should be included. If one or more values are selected, the subscription list must only return records matching the selected criteria. Filtering must happen on the backend, not by loading all subscriptions into the page and filtering client side. If the GraphQL API does not already support filtering subscriptions by status and payment status, extend the schema, query inputs, resolvers, services, and tests to support it. Whenever the filter selection changes, the UI must issue a fresh GraphQL query and refresh the subscription list from the backend. Update or add the required unit tests, integration tests, GraphQL tests, and UI tests to cover the new backend driven filter options and server side filtering behaviour."

---

## User Scenarios & Testing _(mandatory)_

### User Story 1 — Filter Subscriptions by Subscription Status (Priority: P1)

A coworking space owner opens the subscription management page and needs to review only active subscriptions (or only cancelled ones, or any other specific statuses). They select one or more subscription statuses from the multi-select filter control and immediately see the list refresh to show only matching subscriptions.

**Why this priority**: Finding subscriptions in a specific status is the most common day-to-day management task for space owners and has the highest direct operational value.

**Independent Test**: Can be fully tested by selecting a single subscription status in the filter and confirming the returned list matches only that status. Delivers immediate value without the payment status filter.

**Acceptance Scenarios**:

1. **Given** the subscription list is open with no filters applied, **When** the owner selects one subscription status in the status filter, **Then** the list refreshes and shows only subscriptions in that status, with the filter control reflecting the selection.
2. **Given** one subscription status is selected, **When** the owner adds a second status to the filter, **Then** the list refreshes to include subscriptions matching either selected status.
3. **Given** one or more statuses are selected, **When** the owner removes all selections from the status filter, **Then** the filter is cleared and the full unfiltered subscription list is restored.
4. **Given** valid status filters are applied, **When** no subscriptions match the selected statuses, **Then** an empty state is shown rather than a loading spinner or error.

---

### User Story 2 — Filter Subscriptions by Payment Status (Priority: P2)

A coworking space owner needs to identify subscriptions with outstanding or failed payments. They select one or more payment statuses from the multi-select payment status filter and the list refreshes to show only matching subscriptions.

**Why this priority**: Payment status filtering is critical for revenue management but typically used less frequently than subscription status filtering.

**Independent Test**: Can be fully tested by selecting a single payment status and verifying returned records all carry that payment status. Delivers value independently of the subscription status filter.

**Acceptance Scenarios**:

1. **Given** the subscription list is open with no filters applied, **When** the owner selects one payment status in the payment status filter, **Then** the list refreshes and shows only subscriptions with that payment status.
2. **Given** one payment status is selected, **When** the owner adds a second payment status, **Then** the list refreshes to include subscriptions matching either selected payment status.
3. **Given** one or more payment statuses are selected, **When** the owner removes all selections from the payment status filter, **Then** the filter is cleared and the full subscription list is restored.

---

### User Story 3 — Combined Subscription Status and Payment Status Filtering (Priority: P3)

A space owner needs to find active subscriptions that also have a payment failure. They apply both a subscription status filter and a payment status filter simultaneously and the list shows only subscriptions satisfying both criteria at once.

**Why this priority**: Combined filtering is a powerful but less frequent use case; it depends on both individual filters being in place first.

**Independent Test**: Can be tested by selecting one status from each filter and confirming every item in the result matches both selected values.

**Acceptance Scenarios**:

1. **Given** both filter controls are visible, **When** the owner selects at least one subscription status and at least one payment status, **Then** the list refreshes and shows only subscriptions matching the selected subscription status AND the selected payment status.
2. **Given** combined filters are active, **When** the owner clears just one filter, **Then** only the remaining active filter continues to restrict the results.

---

### User Story 4 — Backend-Driven Filter Option Values (Priority: P2)

A space owner opens the subscription list page and sees that the available filter options for both filters are populated automatically from the backend, reflecting the actual values defined in the system, without requiring any client-side hardcoding.

**Why this priority**: Backend-driven options ensure the filter choices stay in sync with any future additions to status or payment status values without frontend code changes.

**Independent Test**: Can be verified by querying the GraphQL API for filter option values and confirming the combo box displays the same set.

**Acceptance Scenarios**:

1. **Given** the subscription list page loads, **When** the filter combo boxes render, **Then** each combo box displays the set of available values returned by the backend GraphQL query.
2. **Given** the backend adds a new subscription status value, **When** the page is reloaded, **Then** the subscription status filter includes the new value without any frontend code change.

---

### Edge Cases

- What happens when no subscriptions match the combined selected filter criteria? An empty-state message is shown; no error is raised.
- What happens if the GraphQL query for filter option values fails to load? The filter combo boxes should show an appropriate error or be disabled; the subscription list itself should still load unfiltered.
- What happens when filters are changed rapidly in quick succession? Because queries fire immediately on each selection change, in-flight queries from earlier selections MUST be superseded by the latest query; the UI MUST display only the result of the most recent query and discard stale responses.
- What happens when the subscription list has many pages and a filter is applied? The results reset to page one so the owner does not land on a page that no longer exists within the filtered result set.
- What happens when the URL contains an unrecognised or invalid filter value? The backend logs a warning (LOG-002) and treats the invalid value as absent; the UI renders whatever valid values remain and does not show an error page.
- What happens when a filter query fails (network error or backend error)? The loading overlay is dismissed, the previous results (or empty state) are shown, and an error notification informs the owner that the filter could not be applied.

---

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The subscription management page in the Management Portal MUST display a multi-select combo box for filtering subscriptions by subscription status.
- **FR-002**: The subscription management page MUST display a multi-select combo box for filtering subscriptions by payment status.
- **FR-003**: The available option values for the subscription status filter MUST be sourced from the backend via GraphQL, following the same backend-driven combo box pattern used elsewhere in the application.
- **FR-004**: The available option values for the payment status filter MUST be sourced from the backend via GraphQL, following the same backend-driven combo box pattern used elsewhere in the application.
- **FR-005**: Both filter controls MUST be implemented as multi-select combo boxes, reusing the same UX component and technical pattern as existing multi-select combo boxes in the application.
- **FR-006**: When no values are selected in a filter, that filter MUST NOT restrict the subscription list results; all subscriptions MUST be included as if the filter were absent.
- **FR-007**: When one or more values are selected in the subscription status filter, the subscription list MUST return only subscriptions whose status matches one of the selected values.
- **FR-008**: When one or more values are selected in the payment status filter, the subscription list MUST return only subscriptions whose payment status matches one of the selected values.
- **FR-009**: When both filters have selections, the subscription list MUST return only subscriptions satisfying both the subscription status filter AND the payment status filter simultaneously.
- **FR-010**: Filtering MUST be performed on the backend; the client MUST NOT load the full subscription list and apply filtering in the browser.
- **FR-011**: If the booking domain GraphQL API does not already support filtering the subscription list by subscription status and/or payment status, the GraphQL schema, query input types, resolvers, services, and their tests in the booking domain MUST be extended to support it before the UI integration is built.
- **FR-012**: Whenever any filter selection changes (a value is added to or removed from either filter), the UI MUST immediately issue a fresh GraphQL query without debouncing and refresh the displayed subscription list from the backend response. In-flight queries superseded by a newer selection change MUST be cancelled or their responses discarded so only the latest result is rendered.
- **FR-013**: When paginated results are active and a filter changes, the subscription list MUST reset to the first page of results.
- **FR-014**: Active filter selections MUST be reflected in the browser URL as query string parameters so that the filtered view is deep-linkable and bookmarkable. When the page loads with filter parameters already present in the URL, the filter controls MUST be pre-populated and the subscription list MUST be fetched with those filters applied immediately.
- **FR-015**: While a filter-triggered backend query is in-flight, the subscription list MUST display a skeleton or loading overlay. The filter controls MUST remain fully interactive during loading so that the owner can adjust their selection without waiting for the current query to complete.
- **FR-016**: The available option values for both filter controls MUST be fetched from the backend once when the subscription list page loads. They MUST NOT be re-fetched each time a filter dropdown is opened.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The backend subscription list query resolver MUST emit a structured log entry when filter inputs are received, including the filter values (but not personally identifiable information) and the result count.
- **LOG-002**: The backend MUST emit a structured log entry at warning level if an unrecognised filter value is submitted by the client.
- **LOG-003**: Any failure to load filter option values from the backend MUST be logged at error level with sufficient context to diagnose the root cause.
- **LOG-004**: Logs MUST include the request or operation identifier for correlation with other backend activity.

### Key Entities _(include if feature involves data)_

- **Subscription**: A record representing a coworking space customer's recurring membership. Has a subscription status (e.g. active, cancelled, paused) and a payment status (e.g. paid, pending, failed, overdue).
- **SubscriptionStatusOption**: A backend-defined value representing a valid subscription status choice, used to populate the subscription status filter control.
- **PaymentStatusOption**: A backend-defined value representing a valid payment status choice, used to populate the payment status filter control.
- **SubscriptionFilterInput**: The GraphQL input type (in the booking domain API) carrying one or more selected subscription statuses and/or payment statuses to be applied server-side when querying the subscription list.

---

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: A space owner can apply a subscription status filter and see the refreshed list within the normal page response time, without any client-side filtering delay.
- **SC-002**: A space owner can apply a payment status filter and see the refreshed list within the normal page response time.
- **SC-003**: A space owner combining both filters sees only subscriptions satisfying all selected criteria; no subscriptions outside those criteria appear in the results.
- **SC-004**: Filter option values displayed in the combo boxes exactly match the set returned by the backend at the time the page was loaded, with no hardcoded values on the frontend.
- **SC-005**: Clearing all filter selections restores the full unfiltered subscription list in a single fresh backend query.
- **SC-006**: All new filtering behaviour is covered by unit tests, integration tests, GraphQL schema/query tests, and UI tests; no regression in existing subscription list behaviour is introduced.
- **SC-007**: The backend correctly rejects or ignores unrecognised filter values and logs a warning rather than returning an error to the user.

---

## Clarifications

### Session 2026-04-27

- Q: Which backend domain GraphQL API hosts the subscription list that this filter will be added to? → A: booking
- Q: Does a coworking space owner see subscriptions for all their spaces or only the currently selected space? → A: All spaces the owner manages
- Q: Should the UI debounce filter changes or fire a query immediately on each confirmed selection change? → A: Immediate — query fires on every confirmed selection change (add or remove)
- Q: Should active filter selections be reflected in the browser URL (deep-linkable / bookmarkable)? → A: Yes — reflect filters in the URL query string

### Session 2026-04-27 (continued)

- Q: What should the UI show while a filter-triggered backend query is in-flight? → A: Skeleton or loading overlay on the subscription list; filter controls remain interactive
- Q: When should filter option values be loaded from the backend? → A: Once when the subscription list page loads
- Q: Should a dedicated "Clear all filters" affordance be provided? → A: No — owner clears each filter individually by deselecting its values
- Q: What accessibility standard must the filter controls meet? → A: Follow the same accessibility baseline as existing controls in the portal

---

## Assumptions

- The Management Portal refers to the coworking space owner-facing portal, not the end customer portal.
- The subscription list page already exists in the Management Portal and currently shows subscriptions without any status or payment status filters.
- Subscription status and payment status are already defined as enum types (or equivalent) in the domain; the feature adds filterability to the existing list query rather than introducing new domain concepts.
- Filter selections are reflected in the URL query string and therefore persist across browser refreshes and can be shared as deep links. When no filter parameters are present in the URL, the filters default to "no selection" (all results).
- Pagination is already implemented for the subscription list and server-side filtering is expected to operate alongside it correctly.
- The existing multi-select combo box pattern in the application can be reused without modifications to the core component.
- Only space owner users in the Management Portal are in scope; customer-facing subscription views are out of scope.
- The subscription list shows subscriptions across all spaces managed by the authenticated owner; the status and payment status filters apply across that full multi-space scope.
- The subscription list query and its filter option value queries are owned by the booking domain GraphQL API.
- No dedicated "Clear all filters" button is provided. Each filter control includes its own deselect mechanism; the owner clears a filter by removing its selected values individually.
- The filter controls are expected to meet the same accessibility baseline already established for other controls in the Management Portal; no elevated accessibility standard is introduced by this feature.
