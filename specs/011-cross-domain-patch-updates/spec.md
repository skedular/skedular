# Feature Specification: Cross-Domain Patch Updates

**Feature Branch**: `011-cross-domain-patch-updates`  
**Created**: 2026-05-21  
**Status**: Draft  
**Input**: User description: "Apply the organisation partial-update and autosave pattern to every remaining domain. Organisation is already done; all other domains with editable update surfaces, including booking and marketplace, need the same feature."

## Clarifications

### Session 2026-05-21

- Q: Should the remaining domains follow the same public update-contract rule as organisation? → A: Replace migrated full-replacement update contracts with one field-masked `Update*` contract per surface.
- Q: Which update surfaces should this feature migrate in each remaining domain? → A: All existing update surfaces for editable domain state; autosave applies only where a user-facing edit screen exists.
- Q: How should callers declare the fields they intend to update on migrated surfaces? → A: Use explicit allowlisted field selection on every migrated update surface.
- Q: How should migrated partial updates handle a detected concurrent change? → A: Reload latest state and retry only the selected fields.
- Q: How should autosave work for edit surfaces where several related fields must stay consistent? → A: Autosave by coherent edit unit: single fields where independent, grouped fields where they must validate together.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Save one edited field across remaining domains (Priority: P1)

An authorised user edits a value on any remaining in-scope domain and expects that change to save without submitting a full edit form or re-sending unrelated data.

**Why this priority**: This is the user-facing outcome already proven for organisation editing. Extending it removes repeated save-button workflows and reduces the risk of overwriting unrelated values across the rest of the product.

**Independent Test**: Can be tested by editing one editable value on an in-scope screen, observing the save complete automatically, and verifying that the changed value persists while unrelated values remain unchanged.

**Acceptance Scenarios**:

1. **Given** an authorised user is editing an independent editable value in a remaining domain, **When** the user changes one field and the autosave trigger is reached, **Then** that value is saved without requiring a page-level update button.
2. **Given** an in-scope record contains multiple editable values, **When** the user changes one supported field, **Then** the saved record reflects that field change and preserves every unrelated value.
3. **Given** a supported field may be intentionally cleared, **When** the user clears that field and autosave completes, **Then** the selected field is cleared and values not included in the edit are preserved.

---

### User Story 2 - Migrate partial callers without data loss (Priority: P2)

A product team updates an in-scope domain from a screen or workflow that only knows the changed values and expects omitted values to be protected from accidental clearing or stale replacement.

**Why this priority**: The rollout is only safe if each migrated update path can accept partial intent while preserving existing validation, permission, and consistency rules.

**Independent Test**: Can be tested on a migrated update path by submitting a change set that only includes selected values and confirming that omitted values remain untouched for successful, invalid, and no-op updates.

**Acceptance Scenarios**:

1. **Given** an in-scope record has values the caller did not load, **When** the caller updates only selected values, **Then** the unknown or omitted values remain unchanged.
2. **Given** one selected value is invalid, **When** a partial update is submitted, **Then** the change is rejected with clear feedback and no selected values are partially saved.
3. **Given** selected values already match the latest record, **When** the update is submitted, **Then** it is treated as a valid no-op and the caller receives the latest saved state.

---

### User Story 3 - Apply one update experience across the rollout (Priority: P3)

An administrator or other authorised user moves between remaining in-scope edit surfaces and sees a consistent editing experience based on local field saves instead of multiple competing update buttons.

**Why this priority**: Consistency lowers user friction and gives delivery teams a clear migration target for all domain update behaviour that remains after organisation.

**Independent Test**: Can be tested by reviewing each migrated in-scope edit surface and confirming it follows the same autosave, saved-state, error-state, omitted-value, and scope rules.

**Acceptance Scenarios**:

1. **Given** an edit surface is migrated by this rollout, **When** a user updates supported independent fields or a grouped edit unit, **Then** the screen uses autosave for that edit unit instead of retaining redundant per-section or page-level update buttons for the same changes.
2. **Given** an autosave succeeds, **When** the user continues editing, **Then** the screen shows the saved state for the completed edit and remains ready for subsequent field changes.
3. **Given** an autosave cannot be completed, **When** feedback is shown to the user, **Then** the user can identify the failed field or edit area and the screen does not present the failed value as saved.
4. **Given** booking, marketplace, or another remaining domain exposes an editable update path, **When** this rollout is assessed, **Then** that path is migrated by this feature.

### Edge Cases

- A user clears a value, enters an empty value, or selects a default-like value that must be distinguished from an omitted field.
- A group of related fields must be validated together before autosave can persist the grouped edit unit.
- A migrated caller includes multiple selected fields and one field fails validation.
- A caller attempts to update a field that is not supported by the migrated partial-update surface.
- A user without permission attempts to update one selected field.
- Two autosaves are triggered close together for the same record or related fields.
- Another update changes the same record while a partial save is in flight; the migrated update reloads the latest state and retries only the selected fields.
- A no-op partial update is submitted because the visible value already matches the latest saved value.
- An autosave fails after the user has moved focus to another field.
- A screen still contains a non-migrated update button for behaviour outside the migrated autosave fields.
- A remaining domain has an update path with no user-facing edit surface; the update path still needs partial-update protection while autosave does not apply.
- Organisation update paths are encountered during migration review and must be treated as the completed reference rather than re-scoped into this feature.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The rollout MUST migrate every existing update surface for editable domain state in each remaining domain after organisation to partial updates where callers identify only the values they intend to change.
- **FR-002**: The rollout MUST include all remaining product domains with editable update surfaces, including booking, core, customer, location, marketplace, Microsoft Teams, Slack, and team.
- **FR-003**: Every migrated partial update MUST preserve fields and related values that the caller did not identify for change.
- **FR-004**: Every migrated partial update MUST distinguish omitted values from selected values that are intentionally empty, cleared, or default-like.
- **FR-005**: Every migrated partial update MUST apply the same permission checks and business rules required for the equivalent existing update behaviour.
- **FR-006**: Every migrated partial update MUST reject unsupported or invalid selected values without saving a partial subset of the rejected change set.
- **FR-007**: Every migrated partial update MUST return clear feedback for invalid selected values and a current saved state after successful saves.
- **FR-008**: Every migrated partial update MUST accept valid no-op changes without altering the saved record.
- **FR-009**: When a migrated partial update detects a concurrent change, it MUST reload the latest state and retry only the selected fields without overwriting values omitted from the submitted change set.
- **FR-010**: Migrated user-facing screens MUST save by coherent edit unit after the user completes or pauses the edit: independent fields save individually, while related fields that must validate together save as a grouped edit unit.
- **FR-011**: Migrated user-facing screens MUST show whether an autosave completed or failed for the affected edit area.
- **FR-012**: Migrated user-facing screens MUST remove redundant update buttons for values covered by autosave.
- **FR-013**: Migrated screens MAY keep explicit actions that perform behaviour other than saving autosaved field edits, provided those actions are not presented as duplicate update controls for the same fields.
- **FR-014**: The rollout MUST document which remaining-domain update surfaces were migrated so completion can be checked across every remaining domain.
- **FR-015**: The rollout MUST keep the organisation partial-update behaviour as the consistency baseline for omitted-value preservation, selected-value validation, no-op handling, concurrency handling, and autosave feedback.
- **FR-016**: The rollout MUST treat organisation as the completed reference implementation and MUST not require re-migration of organisation update surfaces already covered by the earlier organisation feature.
- **FR-017**: Each migrated update surface MUST use one normal public `Update*` contract with field-masked partial-update semantics instead of keeping parallel full-replacement and patch variants.
- **FR-018**: Autosave requirements MUST apply only to migrated update surfaces that have a user-facing edit screen.
- **FR-019**: Every migrated update surface MUST require explicit allowlisted field selection so caller intent is declared independently from empty, default, or omitted values.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Feature MUST emit structured logs for start/completion of core workflows.
- **LOG-002**: Feature MUST emit structured logs for meaningful state transitions and branch decisions.
- **LOG-003**: Feature MUST emit actionable warning/error logs for failure and recovery paths.
- **LOG-004**: Feature logs MUST include correlation context (for example request/workflow identifiers)
  and MUST avoid sensitive data leakage.
- **LOG-005**: Feature logs MUST identify partial-update start, completion, validation rejection, permission rejection, concurrency recovery, no-op handling, and persistence failure paths for migrated update surfaces.

### Key Entities _(include if feature involves data)_

- **In-Scope Editable Record**: A record in any remaining domain after organisation, including booking and marketplace, with values an authorised user or workflow may update.
- **Selected Change Set**: The caller's explicit allowlisted field selection and values for one partial update, declaring which editable values may change and which values must be preserved.
- **Autosave Edit Surface**: A user-facing editing area that saves supported field changes without a duplicate update button for the same values.
- **Update Result**: The saved state or clear failure feedback returned after a migrated partial update attempt.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: During acceptance testing, authorised users can complete an editable single-field edit on every migrated user-facing surface without a full-form submit in at least 95% of attempts.
- **SC-002**: In partial-update acceptance tests for migrated surfaces, 100% of omitted values remain unchanged after successful updates, valid no-op updates, validation failures, and concurrency recovery scenarios.
- **SC-003**: Invalid selected values produce field-specific or edit-area-specific feedback in 100% of covered validation failure scenarios.
- **SC-004**: Migration review identifies no redundant update buttons for values covered by autosave on the migrated screens.
- **SC-005**: Booking and marketplace editable update surfaces meet the same partial-update and autosave outcomes as the other remaining domains before the feature is considered complete.
- **SC-006**: Delivery notes identify the migrated update surfaces for every remaining domain before the feature is considered complete.

## Assumptions

- The organisation rollout is the reference behaviour for this follow-on feature.
- Organisation update surfaces already migrated by the earlier organisation feature are not reimplemented here.
- Every remaining domain is in scope when it has editable update surfaces, including booking and marketplace.
- Remaining-domain update surfaces without user-facing edit screens still require partial-update migration, but do not require autosave work.
- The rollout migrates existing update behaviour rather than introducing new editable fields or new business capabilities for each domain.
- Existing authorisation, validation, audit, and business-rule expectations remain authoritative for each migrated domain.
- Autosave timing may vary by edit control, but users should not need duplicate update buttons for fields covered by autosave.
- A screen may still require explicit actions for non-field workflows such as creation, deletion, submission, or confirmation.
