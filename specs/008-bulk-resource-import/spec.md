# Feature Specification: Bulk Resource Import

**Feature Branch**: `008-bulk-resource-import`
**Created**: 2026-05-13
**Status**: Draft
**Input**: User description: "I need to give users the ability to add bulk resources (including desks and other kinds of resources). The resources will be added to a location with a set of tags assigned to them."

## Clarifications

### Session 2026-05-13

- Q: Which domain service should own the bulk resource import GraphQL mutation? → A: `location` domain
- Q: What is the maximum number of resources allowed in a single bulk import batch? → A: 100
- Q: When two rows in the same submitted batch share the same resource name, how should the system handle it? → A: The API auto-resolves naming conflicts via a naming convention (e.g., appending a numeric suffix) rather than rejecting duplicates.
- Q: Should the UI show auto-resolved names before or after submission? → A: No UI visibility needed — the API accepts a base name (which may be empty) and silently generates all resource names; the admin sees the final names only in the location resource list.
- Q: Should tags apply uniformly to all resources in a row, or per individual generated resource? → A: Tags are uniform per row — all resources generated from the same row share the same set of tags.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Bulk Add Resources to a Location (Priority: P1)

A location administrator needs to add many resources (desks, meeting rooms, phone booths, etc.) to a location at once rather than one by one. They navigate to the location's resource management screen, specify the resource type, an optional base name, quantity, and tags, and submit the batch in one action. The system auto-generates individual resource names from the base name (or a default when the base name is empty) and creates all resources. It reports back which ones were created successfully and which failed, showing a clear reason for each failure.

**Why this priority**: Core value proposition. Without this story there is no bulk import capability at all.

**Independent Test**: Can be fully tested by submitting a list of mixed valid/invalid resource rows for a known location and verifying that valid rows are persisted and invalid rows are reported with actionable error messages.

**Acceptance Scenarios**:

1. **Given** an admin is on the bulk resource import screen for a location, **When** they submit a batch of 10 resources with a type, base name "Desk", and at least one tag, **Then** all 10 resources are created under that location with auto-generated names (e.g., "Desk-1" through "Desk-10") and a success summary is displayed.
2. **Given** an admin submits a batch with an empty base name, **When** the submission is processed, **Then** the system generates names from a default naming scheme based on the resource type (e.g., "Desk-1", "Desk-2") and creates all resources successfully.
3. **Given** an admin submits a batch that contains rows with invalid data (e.g., quantity of zero, unrecognised type), **When** the submission is processed, **Then** the invalid rows are rejected and each rejected row shows a specific reason; valid rows are still created.
4. **Given** an admin submits an empty batch (zero rows), **When** the submission is attempted, **Then** the system prevents submission and shows a validation message before the request is sent.
5. **Given** generated resource names would conflict with existing names at that location, **When** processed, **Then** the system increments the numeric suffix until each generated name is unique (e.g., existing "Desk-1" → new resource becomes "Desk-2").

---

### User Story 2 - Compose Batch in the UI Before Submitting (Priority: P1)

An admin needs to freely compose the list of resource batches before committing. Each row represents a batch entry with a resource type, an optional base name, a quantity, and a set of tags. They can add rows, remove rows, and edit values. Tags are selected from the available tag pool for that location. The admin can review the full list and make corrections before submitting.

**Why this priority**: Without the ability to build and edit the batch list, the bulk import UX degrades to a single-entry form repeated manually.

**Independent Test**: Can be fully tested by adding rows, removing a row, editing a field, and verifying the final batch state matches what was submitted.

**Acceptance Scenarios**:

1. **Given** an admin is on the bulk import screen, **When** they click "Add row", **Then** a new empty batch row is appended to the list.
2. **Given** the admin has one or more rows, **When** they click "Remove" on a row, **Then** that row is removed from the list without affecting the others.
3. **Given** the admin edits the type or base name field on a row, **When** they change the value, **Then** only that row is updated.
4. **Given** the admin opens the tag selector on a row, **When** they select or deselect tags, **Then** the selected tags are shown on that row only.
5. **Given** the admin leaves the base name field empty on a row, **When** they submit, **Then** the API generates default names from the resource type without requiring a base name.

---

### User Story 3 - Review Import Results (Priority: P2)

After submission, the admin sees a results summary distinguishing which resources were created and which were rejected, with rejection reasons displayed inline per row. The admin can dismiss the results and choose to retry or correct any failed rows.

**Why this priority**: Without clear per-row feedback, the admin cannot know whether their import was complete or partially failed, leading to manual reconciliation effort.

**Independent Test**: Can be fully tested by submitting a batch with deliberate errors and verifying the results UI shows correct success/failure counts and per-row reasons.

**Acceptance Scenarios**:

1. **Given** a batch was submitted with mixed results, **When** the results screen is displayed, **Then** the total created count and total failed count are both visible.
2. **Given** a resource row failed validation, **When** the results are shown, **Then** the failed row displays the specific reason returned by the API.
3. **Given** the admin wants to retry failed rows, **When** they choose to retry, **Then** only the failed rows are pre-populated for re-submission.

---

### Edge Cases

- What happens when the batch contains more resources than the location's resource capacity limit?
- How does the suffix increment when generated names collide with a non-contiguous set of existing names (e.g., "Desk-1" and "Desk-3" exist — does "Desk-2" fill the gap or does the system always append at the end as "Desk-4")?
- What if a selected tag is deleted between the user composing the batch and submitting it? — The deleted tag ID is silently ignored; the resource is created without it.
- What happens when all rows in a batch fail validation?
- How does the system behave if the network drops mid-submission? — The server runs all resource creation inside a single database transaction and only publishes the location domain event after a successful commit. If the HTTP request is cancelled (network drop), the transaction rolls back and no resources are persisted. If the request completes server-side before the client receives the response, the transaction has already committed; the client should treat a network error as ambiguous and advise the admin to verify the location's resource list.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The `location` domain GraphQL API MUST expose a mutation that accepts a list of resource inputs (optional base name, resource type, quantity, tags, location) and returns a per-resource result list indicating success or failure with reason.
- **FR-002**: The GraphQL mutation MUST support partial success — valid resources in the batch are persisted even when other resources in the same batch fail validation.
- **FR-003**: Each batch row input MUST include: resource type, quantity (minimum 1), target location identifier, and zero or more tag identifiers. A base name is optional; if omitted or empty, the system derives a default name from the resource type. All resources generated from a single row MUST share the same set of tags.
- **FR-004**: Resource types available for selection MUST be driven by the existing resource type definitions in the system (e.g., desk, meeting room, phone booth).
- **FR-005**: The mutation MUST validate each resource individually before any writes and return a structured error reason for each invalid resource in the response.
- **FR-006**: The system MUST auto-generate individual resource names from the provided base name and a numeric suffix (e.g., base name "Desk" with quantity 3 → "Desk-1", "Desk-2", "Desk-3").
- **FR-007**: When the base name is empty or omitted, the system MUST derive a default base name from the resource type tag's `Name` property for name generation.
- **FR-008**: When any generated name conflicts with an existing resource name at that location, the system MUST increment the numeric suffix until a unique name is found; no generated resource name MUST be rejected solely due to a naming conflict.
- **FR-009**: The UI MUST provide a dynamic row-based form allowing the admin to add, remove, and edit rows before submission.
- **FR-010**: The UI MUST allow the admin to select tags from the available tags for the organisation on a per-row basis, using organisation-wide tag queries consistent with existing tag selectors in the application.
- **FR-011**: The UI MUST display per-row success and failure feedback after submission, including the rejection reason for each failed row.
- **FR-012**: The UI MUST prevent form submission when the batch has no rows.
- **FR-013**: The UI MUST allow the admin to retry only the failed rows from a previous submission, pre-populated with their last entered values.
- **FR-014**: After a successful bulk write transaction, the service MUST publish the location domain event via the location outbox publisher and trigger `ITemporalOutboxService.StartComputeOrganizationLocationsAndProductsRelationships` exactly once per location (not once per created resource).

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The GraphQL mutation handler MUST emit a structured log entry when a bulk import batch is received, including the location identifier and batch size.
- **LOG-002**: The system MUST log a structured entry for each resource that fails validation, including the failure reason (without logging sensitive user data).
- **LOG-003**: The system MUST log a summary on completion indicating how many resources were created and how many were rejected.
- **LOG-004**: All log entries MUST include the request correlation identifier and MUST NOT include raw tag content that could leak sensitive data.

### Key Entities _(include if feature involves data)_

- **Resource**: A bookable unit within a location. Has a name, a type, and belongs to one location. Can have zero or more tags assigned.
- **ResourceType**: A predefined classification for a resource (e.g., desk, meeting room, phone booth). Determines how the resource appears and is booked.
- **Location**: An existing organisational venue to which resources belong. Identified by its identifier in the bulk import input.
- **Tag**: A label that can be assigned to a resource. Tags belong to the system and are referenced by identifier in the bulk import input.
- **BulkImportResourcesInput** / **BulkImportResourceRowInput**: The per-row input payload: optional base name, resource type, quantity, location identifier, list of tag identifiers. Tags are applied uniformly to every resource generated from this row.
- **BulkImportResourceRowResult**: The per-row outcome (GraphQL type); **BulkImportRowResult** (service model): resource identifier (if created), success flag, and optional failure reason.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: An admin can add 50 resources to a location in a single operation in under 3 minutes from opening the import screen to seeing results.
- **SC-002**: All valid resources in a batch are persisted regardless of how many invalid resources exist in the same batch (partial success rate = 100% of valid rows).
- **SC-003**: Per-row failure reasons are displayed within 2 seconds of the mutation response being received.
- **SC-004**: 90% of admins can complete their first bulk import without requiring assistance, as measured by task-completion rate in usability testing.
- **SC-005**: Batch submissions of up to 100 resources complete within 5 seconds under normal load.

## Assumptions

- Tags already exist in the system before the bulk import is performed; inline tag creation is out of scope for this feature.
- The target location must already exist; bulk resource import does not create new locations.
- Resource types are a predefined, finite set managed elsewhere in the system; this feature does not introduce new resource types.
- This is an end-to-end feature: the `location` domain GraphQL API and the webapp UI are both in scope. No REST/OpenAPI endpoints are added or modified.
- Authentication and authorisation use the existing admin-level permission model; no new permission types are introduced.
- Mobile support is not required for v1; the bulk import UI targets desktop/tablet form factors.
- The API enforces a maximum batch size of 100 resources per import request to prevent abuse and ensure acceptable response times.
