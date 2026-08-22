# Feature Specification: Persisted Duration Display Units

**Feature Branch**: `043-duration-display-units`  
**Created**: 2026-08-21  
**Status**: Draft  
**Input**: Persist the selected minutes/hours presentation for editable Marketplace pricing durations while retaining canonical minute values.

## Clarifications

### Session 2026-08-21

- Q: Should the feature remain limited to Marketplace pricing, or cover every domain with this editable minute/hour concept? → A: Cover every domain with persisted, user-editable minute-based values that can be displayed as minutes or hours; perform research first and implement the applicable cases.
- Q: Should the feature change the existing hours conversion or rounding behavior? → A: No; preserve the existing conversion and rounding, including displaying 5 minutes as 0.08 hours, and persist only the selected display unit.
- Q: Should the repository-wide audit block implementation? → A: Yes; complete the audit before starting user-story implementation.
- Q: Should implementation tasks use exact test paths? → A: Yes; resolve exact test project/file paths during the initial audit before development starts.
- Q: Should display-unit metadata be replicated through other domains? → A: No; keep it in the owning domain/subgraph. Cross-domain replication is only for canonical data when independently required for sorting, pagination, caching, or another domain need.
- Q: Should focused logging tests be included? → A: Yes; test invalid display-unit values and persistence/contract failure logging.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Restore Marketplace Duration Preferences (Priority: P1)

As a user editing any supported persisted configuration, I want each editable duration field to remember whether I used minutes or hours, so reopening the editor shows the value in my chosen unit without changing its actual duration.

**Why this priority**: This prevents confusing value changes in the primary pricing workflow while preserving the existing business value.

**Independent Test**: Create or edit a supported persisted configuration with a duration, save it, reopen it, and verify both the chosen unit and exact minute value are restored.

**Acceptance Scenarios**:

1. **Given** a pricing record with no display-unit metadata, **when** an editor loads it, **then** the field displays in hours and canonical minutes are unchanged.
2. **Given** a user enters 5 minutes and selects minutes, **when** the record is saved and reopened, **then** it displays 5 minutes and retains 5 canonical minutes.
3. **Given** a user selects hours, **when** the record is saved and reopened, **then** it displays the equivalent hour value while retaining exact canonical minutes.

### User Story 2 - Consistent Cross-Domain Editor Conversion (Priority: P2)

As a user of any supported domain editor, I want all applicable editable duration fields to use the same unit selection and conversion behavior, so switching units never changes the configured duration.

**Why this priority**: The same concept may exist in multiple domains and editors; inconsistent conversions would reintroduce data loss and make the preference unreliable.

**Independent Test**: In each identified supported editor, switch a field between hours and minutes and verify the submitted minute value and selected unit.

**Acceptance Scenarios**:

1. **Given** a displayed duration, **when** the user switches units, **then** the visible number changes to the equivalent value while canonical minutes remain constant.
2. **Given** a user submits a duration, lock window, or cancellation timing, **when** the editor saves, **then** it submits canonical minutes plus the selected optional display unit.

### User Story 3 - Preserve Non-Editor Duration Semantics (Priority: P3)

As an operator, I want operational durations and derived read-only values to remain unchanged, so display preferences do not leak into calculations, workflows, logs, or customer-facing output.

**Why this priority**: Display metadata is presentation-only and must not alter booking, refund, availability, or lock-window behavior.

**Independent Test**: Compare calculations and non-editor duration outputs before and after the feature for identical canonical minute inputs.

**Acceptance Scenarios**:

1. **Given** identical canonical minute values, **when** booking, refund, availability, or lock-window logic runs, **then** results are identical regardless of display unit.

### Edge Cases

- Missing metadata defaults to hours without rewriting stored minutes.
- Existing conversion and rounding behavior remains unchanged; for example, 5 minutes may display as 0.08 hours.
- Changing or saving a display unit must not migrate, normalize, or otherwise change any existing canonical minute value.
- Existing JSON records and clients omitting optional fields remain valid.
- Unsupported units follow the existing validation/error policy and never silently change canonical minutes.
- Operational TimeSpan values, workflow timeouts, logs, backend calculations, and read-only derived values are excluded.
- A discovered occurrence must be classified during research before implementation; only persisted user-editable configuration is in scope.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: Every owning domain with in-scope persisted, user-editable duration configuration MUST retain business durations canonically in minutes; Marketplace is the first confirmed domain.
- **FR-002**: Every applicable persisted, user-editable minute-based configuration identified by the repository-wide audit MUST support optional display metadata with values `MINUTES` or `HOURS`, including Marketplace pricing and cancellation/refund timing fields.
- **FR-003**: Existing records without display metadata MUST deserialize and display as hours by default.
- **FR-004**: New display metadata MUST be persisted within the existing persistence boundary of its owning domain and returned through applicable same-domain read and write contracts as optional fields; it MUST NOT be replicated to other domains unless a concrete independent domain need is identified.
- **FR-005**: The duration input MUST accept canonical minutes, restore an initial unit, support controlled unit changes, and use the existing conversion and rounding behavior for the visible value.
- **FR-006**: Saving or autosaving MUST persist the selected display unit without changing, migrating, normalizing, or replacing the existing canonical minute value.
- **FR-007**: Spaces editors, Host pricing editors, and every other identified editor for in-scope configuration MUST use the same conversion and persistence behavior.
- **FR-008**: A repository-wide research audit MUST identify minute-based occurrences, document their domain, persistence boundary, editor/read-only status, and classification as editable configuration, operational/internal duration, or read-only derived presentation.
- **FR-009**: The implementation MUST cover every occurrence classified by the audit as persisted user-editable configuration, and MUST document why operational or read-only occurrences are excluded.
- **FR-010**: Booking, validation, refund, availability, and other domain calculations MUST use canonical minutes only and remain behaviorally unchanged.
- **FR-011**: Contract source definitions MUST be updated before generated schemas or client artifacts, and affected generated artifacts MUST be regenerated through required scripts.
- **FR-012**: Tests MUST cover backward compatibility, restoration and switching, canonical preservation, every in-scope domain/editor, and unchanged backend calculations.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: New persistence or contract failures involving display metadata MUST produce an actionable structured warning or error with correlation context.
- **LOG-002**: Conversion failures or rejected display-unit values MUST be observable without logging sensitive pricing or customer data.
- **LOG-003**: Routine unit conversion and successful per-keystroke editor changes MUST not create noisy logs.

### Key Entities

- **Persisted duration configuration**: Any user-editable business value stored canonically in minutes with optional presentation metadata, regardless of owning domain.
- **Duration display unit**: An optional user-facing preference limited to minutes or hours and used only when restoring an editor.
- **Cancellation/refund timing rule**: A pricing rule containing a canonical minute threshold and optional display metadata.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of in-scope editable duration fields identified by the audit restore their persisted unit when metadata exists.
- **SC-002**: 100% of existing records without metadata remain readable and display in hours without changing canonical minutes.
- **SC-003**: 100% of unit-switching and save/reload acceptance tests preserve the existing canonical minute values and existing conversion/rounding behavior.
- **SC-004**: Booking, refund, availability, and lock-window regression tests produce identical results for unchanged canonical inputs.
- **SC-005**: All identified minute-based occurrences are classified and documented, with every persisted user-editable occurrence either implemented or explicitly tracked as blocked.
- **SC-006**: Pricing editor users can change a duration unit and save it without re-entering the duration or correcting a changed value.

## Assumptions

- Allowed display units are limited to `MINUTES` and `HOURS`; hours are the fallback when metadata is absent.
- Existing canonical minute values and validation rules remain the source of business truth.
- Display-unit metadata is optional for backward-compatible inputs and records.
- Research must precede implementation and produce an inventory of all repository occurrences, their owning domains, persistence boundaries, editors, and scope classification.
- The feature covers persisted, user-editable configuration across all domains; operational values and read-only derived values remain out of scope.
- Existing Marketplace product-version JSON is the persistence boundary; no separate display-preference store is needed.
- Public documentation needs no behavior change unless implementation discovery identifies new customer/operator-visible semantics.
