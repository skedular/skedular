# Feature Specification: Organization Patch Updates

**Feature Branch**: `010-organization-patch-updates`  
**Created**: 2026-05-20  
**Status**: Draft  
**Input**: User description: "Support patch-style updates instead of requiring the entire object. Focus first on the organisation domain so callers can submit only the fields provided by inline editing, then later apply the pattern elsewhere."

## Clarifications

### Session 2026-05-20

- Q: Which organisation fields should be patchable in the initial setup slice? → A: Only explicitly allowlisted organisation profile/settings fields are patchable in that slice.
- Q: How should concurrent inline saves be handled? → A: Rely on existing entity concurrency, reload after a concurrency failure, and retry the selected patch fields against the latest organisation.
- Q: How should no-op partial updates be handled? → A: Accept no-op partial updates and return the latest organisation details.
- Q: Which update surfaces should support organisation patch updates in the initial setup slice? → A: The initial setup slice supports organisation partial updates through GraphQL before specialised GraphQL and selected gRPC update surfaces are migrated.
- Q: Which fields are in the organisation patch allowlist? → A: All editable organisation setup fields previously sent by the GraphQL full update path.
- Q: Should patch support extend the existing full-update API or use a new API? → A: Avoid keeping full-replacement and patch variants side by side after migration; use one field-masked update contract for the migrated surface.
- Q: How should callers declare which values to patch? → A: The patch API uses an enum list named fieldsToUpdate to declare the selected fields.

### Session 2026-05-21

- Q: What is the broader organisation-domain direction after the setup patch slice? → A: Migrate the remaining organisation update surfaces to field-mask semantics and remove duplicate full-update or `*Patch` public variants once each surface is migrated.
- Q: Which specialised organisation update surface was the first follow-on patch migration candidate? → A: Organisation SSO settings, because it is organisation-owned, exists in all three admin web apps, and its values are related enough to patch as one aggregate `SSO_SETTINGS` field.
- Q: Once organisation update contracts all use field-mask patch semantics, should their public names still include `Patch`? → A: No. Remove dead full-update code paths and keep the public GraphQL and gRPC names as normal `Update*` contracts; `fieldsToUpdate` and patch-field enums define the partial-update semantics.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Save a single organisation field inline (Priority: P1)

An organisation administrator edits one organisation setting or text value inline and expects only that changed value to be saved without submitting the full organisation form.

**Why this priority**: Inline editing is the primary user value. It reduces form friction and prevents callers from needing to send unrelated organisation data.

**Independent Test**: Can be fully tested by changing one editable organisation field, saving it through `updateOrganization` with `fieldsToUpdate`, and confirming that only that field changes while every omitted organisation field remains unchanged.

**Acceptance Scenarios**:

1. **Given** an organisation has setup values, **When** an administrator submits an update containing one changed allowlisted value, **Then** the organisation is saved with that value changed and every omitted value preserved.
2. **Given** an administrator changes a text field and then leaves the field, **When** the inline save completes successfully, **Then** the updated value remains visible without requiring a full-page or full-form save.
3. **Given** an administrator submits a partial update with a valid explicit empty value for a nullable or clearable field, **When** the update completes, **Then** the selected field is cleared and omitted fields are preserved.

---

### User Story 2 - Preserve omitted values from partial callers (Priority: P2)

A product team updates organisation data from a partial client without risking accidental clearing or overwriting of fields that the client did not load or did not intend to change.

**Why this priority**: The feature must protect existing organisation records from incomplete client payloads before the pattern can be reused in other domains.

**Independent Test**: Can be tested by submitting an update from a client that only knows a subset of organisation values and verifying that unknown or omitted values are not changed.

**Acceptance Scenarios**:

1. **Given** an organisation has an existing billing, profile, and settings configuration, **When** a caller updates only the organisation name, **Then** billing, profile, and settings values not included in the update remain unchanged.
2. **Given** a caller submits a partial update that identifies a field as provided but the provided value is invalid, **When** the update is processed, **Then** the update is rejected with a clear validation result and no partial changes are applied.
3. **Given** a caller submits unchanged values for selected fields, **When** the update is processed, **Then** the system accepts the request, returns the latest organisation details, and leaves the organisation unchanged.

---

### User Story 3 - Reuse a consistent patch contract across organisation updates (Priority: P3)

A developer extending update behaviour can follow the organisation-domain patch rules as the reference pattern for the remaining organisation update entry points and future domains.

**Why this priority**: The organisation domain needs one repeatable product contract for GraphQL and the migrated gRPC update callers.

**Independent Test**: Can be tested by reviewing the organisation update behaviour and confirming the same rules cover submitted fields, omitted fields, validation, permission checks, and audit visibility.

**Acceptance Scenarios**:

1. **Given** the organisation-domain patch behaviour is documented, **When** another organisation update surface is planned later, **Then** the team can identify the required caller intent, validation, and unchanged-field rules from this feature.
2. **Given** the GraphQL full-object organisation update implementation is removed, **When** the web apps update organisation setup details, **Then** they use `updateOrganization` with an explicit field mask for inline and full-form saves.
3. **Given** a migrated organisation gRPC update caller submits billing details, tag, custom tag, product tag, or zone changes, **When** it omits fields, **Then** the server preserves those omitted values.
4. **Given** an existing GraphQL caller still uses a removed `*Patch` mutation or input name, **When** schemas and Relay artefacts are regenerated, **Then** that caller fails generation and must migrate to the normal `Update*` contract with `fieldsToUpdate`.
5. **Given** a specialised organisation GraphQL update mutation is exposed, **When** it updates its aggregate, **Then** it uses field-mask patch semantics without a second public `*Patch` mutation.
6. **Given** organisation SSO settings are edited in the admin UI, **When** the administrator saves the SSO settings, **Then** the UI uses `updateOrganizationSsoSettings` with `fieldsToUpdate: [SSO_SETTINGS]`.

### Edge Cases

- A submitted field is explicitly set to `null`, empty, or a default-like value; the system must distinguish this from the field being omitted.
- A caller submits a field that the current user is not allowed to update.
- A caller submits multiple fields where one value is invalid.
- Two inline saves are triggered close together from the same screen.
- A caller submits a partial update while another save changes the organisation; if the existing entity concurrency mechanism detects a conflict, the system reloads the latest organisation and retries only the selected patch fields.
- A caller submits fields that are unknown, deprecated, not on the first-release allowlist, or not patchable in the organisation domain.
- An organisation changes or is deleted before a pending inline save completes.
- Existing GraphQL full-object update implementations and removed `*Patch` public aliases must not be retained.
- Organisation GraphQL update-style mutations use field-mask patch semantics after migration.
- Organisation gRPC billing details, tag, custom tag, product tag, and zone update callers use field-mask patch semantics after migration.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST support partial organisation updates where callers can identify exactly which allowlisted organisation profile/settings fields they intend to change.
- **FR-002**: The system MUST preserve every organisation field that is not identified as part of a partial update.
- **FR-003**: The system MUST distinguish between an omitted field and a provided field whose intended value is empty, default, or cleared.
- **FR-004**: The system MUST validate only the submitted changes and any existing values required to confirm that the resulting organisation remains valid.
- **FR-005**: The system MUST apply a partial organisation update atomically, so either all submitted valid changes are saved together or none are saved.
- **FR-006**: The system MUST reject partial updates that try to change organisation fields that are not on the first-release patch allowlist or are not patchable through the supported update contract.
- **FR-007**: The system MUST enforce the same authorisation and business rules for patched fields as for equivalent full organisation updates.
- **FR-008**: The system MUST return clear field-level validation feedback when a submitted patched value cannot be accepted.
- **FR-009**: The system MUST remove old full-replacement organisation update implementations and require migrated clients to use field-masked update contracts.
- **FR-010**: The system MUST make inline organisation editing possible without requiring users to open or submit a full organisation edit form.
- **FR-011**: The system MUST return the latest organisation details after every successful patch so Relay and other GraphQL clients can reconcile the saved organisation state after inline or full-form updates.
- **FR-012**: The system MUST define organisation-domain patch rules as the reference pattern for later rollout to other domains and update entry points.
- **FR-013**: The system MUST rely on existing entity concurrency protection for concurrent saves; when a concurrency failure occurs, it MUST reload the latest organisation and retry only the selected patch fields without overwriting omitted fields.
- **FR-014**: The system MUST accept valid no-op partial updates, leave the organisation unchanged, and return the latest organisation details.
- **FR-015**: Public organisation GraphQL update contracts MUST keep normal `Update*` mutation and input names while using `fieldsToUpdate` to carry partial-update intent.
- **FR-016**: The organisation patch allowlist MUST include all editable organisation setup fields previously sent by the GraphQL full update path.
- **FR-017**: The organisation-domain patch pattern MUST apply to migrated GraphQL organisation update-style mutations while keeping a single public update mutation per update surface.
- **FR-018**: Organisation SSO settings MUST use `updateOrganizationSsoSettings` with an explicit `OrganizationSsoSettingsPatchField` enum list and an aggregate `SSO_SETTINGS` patch field.
- **FR-019**: Organisation gRPC billing details, tag, custom tag, product tag, and zone updates MUST use field-masked update inputs with normal `Update*` RPC names.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Feature MUST emit structured logs for start/completion of core workflows.
- **LOG-002**: Feature MUST emit structured logs for meaningful state transitions and branch decisions.
- **LOG-003**: Feature MUST emit actionable warning/error logs for failure and recovery paths.
- **LOG-004**: Feature logs MUST include correlation context (for example request/workflow identifiers) and MUST avoid sensitive data leakage.
- **LOG-005**: Feature logs MUST identify organisation patch processing, including selected fields, completion, validation failure, authorisation failure, concurrency retry, and persistence failure branches.

### Key Entities _(include if feature involves data)_

- **Organisation**: The customer-owned organisation record whose editable profile, settings, billing, and administrative values may be updated.
- **Patch Field Selection**: The caller's enum-list declaration, named `fieldsToUpdate` in the GraphQL patch API, of which allowlisted organisation profile/settings fields are intentionally included in a partial update.
- **Patch Update Request**: A user-initiated update containing the target organisation, the intended changed values, and the field selection needed to preserve omitted values.
- **Update Result**: The mutation response returned to the caller, including validation failures, authorisation failures, or the latest saved organisation details.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: Organisation administrators can complete a single-field inline edit with no full-form submit in at least 95% of supported edit attempts during acceptance testing.
- **SC-002**: In partial-update test coverage, 100% of omitted organisation fields remain unchanged after successful patched updates.
- **SC-003**: Invalid patched values produce field-specific feedback in 100% of validation failure scenarios covered by acceptance tests.
- **SC-004**: Existing organisation setup journeys continue to pass after being migrated from full-replacement update behaviour to field-masked update behaviour.
- **SC-005**: Product teams can identify the organisation patch rules needed for a later domain rollout within one planning session using this specification and resulting delivery notes.
- **SC-006**: Concurrent partial-update acceptance tests preserve 100% of omitted fields when retrying after entity concurrency conflicts.
- **SC-007**: No-op partial-update acceptance tests leave the organisation unchanged and return the latest organisation details in 100% of valid no-op scenarios.

## Assumptions

- The rollout is limited to the organisation domain and does not require applying patch update support to every domain.
- The patch rollout covers the organisation setup fields previously submitted by the GraphQL full update path.
- Old full-replacement organisation update implementations are removed when the update surface is migrated.
- The target end state for this specification is one field-masked public update contract per migrated organisation update surface, not parallel full-update and patch-update contracts.
- Organisation SSO settings are patched as a single aggregate because entity id, login URL, federation metadata URL, and active state are validated together.
- Field-masked update contracts should stay extendable so later phases can add more fields and domains deliberately.
- Organisation administrators and authorised operators are the initial users of inline organisation editing.
- Existing GraphQL full-object organisation update callers must migrate to the field-masked update contract.
- Partial updates should not bypass existing validation, permission checks, audit expectations, or business rules.
- Existing entity-layer concurrency protection is sufficient for this rollout; the GraphQL field-masked update contract does not require callers to submit a version token.
- GraphQL and migrated gRPC update contracts use a `fieldsToUpdate` enum list to make the update mask explicit and extendable.
- Inline editing may save on field exit or after a short idle period, but the exact client timing is outside this specification.
