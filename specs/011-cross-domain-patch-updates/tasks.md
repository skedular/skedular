# Tasks: Cross-Domain Patch Updates

**Input**: Design documents from `specs/011-cross-domain-patch-updates/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/cross-domain-patch-contracts.md`, `quickstart.md`

**Tests**: Required. The spec and constitution require backend unit coverage, GraphQL/gRPC integration coverage, web UI tests, and logging verification for changed workflows.

**Organisation**: Tasks are grouped by user story so each story can be implemented and tested as an increment after the shared foundation is ready.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel with other incomplete tasks because it owns different files.
- **[Story]**: User story mapped from `spec.md`.
- Every task names the primary files or directories it changes.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Lock the repository inventory and generated-surface workflow before domain implementation starts.

- [x] T001 Re-scan owned editable update surfaces for booking, customer, location, marketplace, team, core, Slack, and Microsoft Teams and update the inventory table in `specs/011-cross-domain-patch-updates/contracts/cross-domain-patch-contracts.md`
- [x] T002 [P] Record domain coverage and any no-surface findings for core, Slack, and Microsoft Teams in `specs/011-cross-domain-patch-updates/quickstart.md`
- [x] T003 [P] Inspect changed GraphQL and gRPC generation entry points against repository rules in `scripts/generate-graphql.sh`, `api-definitions/grpc/skedular/`, and `web/apps/webapp/scripts/generate.sh`
- [x] T003A Add any owned editable update surfaces found during the core, Slack, or Microsoft Teams re-scan to the migration inventory and create matching contract, implementation, and verification tasks in `specs/011-cross-domain-patch-updates/contracts/cross-domain-patch-contracts.md`, `specs/011-cross-domain-patch-updates/quickstart.md`, and `specs/011-cross-domain-patch-updates/tasks.md` before closing the feature scope

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Define the cross-domain contract shape and contract tests that every story reuses.

**CRITICAL**: Do not migrate user-facing autosave screens or backend update services until field-selection contracts and generated-surface expectations are established.

- [x] T004 [P] Add allowlisted patch-field enums and `fieldsToUpdate` inputs for booking private, marketplace, and recurring booking GraphQL updates in `booking/apis/Booking.Api/GraphQL/Booking/UpdatePrivateBookingInput.cs`, `booking/apis/Booking.Api/GraphQL/Booking/UpdateMarketplaceBookingInput.cs`, and `booking/apis/Booking.Api/GraphQL/RecurringBooking/UpdatePrivateRecurringBookingInput.cs`
- [x] T005 [P] Add allowlisted patch-field enums and `fieldsToUpdate` inputs for customer details and billing GraphQL updates in `customer/apis/Customer.Api/GraphQL/Customer/UpdateCustomerDetailsInput.cs`, `customer/apis/Customer.Api/GraphQL/Customer/UpdateMyCustomerDetailsInput.cs`, and `customer/apis/Customer.Api/GraphQL/Billing/UpdateMyBillingDetailsInput.cs`
- [x] T006 [P] Add allowlisted patch-field enums and `fieldsToUpdate` inputs for location details, restricted information, physical address, floor plan, and resource GraphQL updates in `location/apis/Location.Api/GraphQL/Location/`, `location/apis/Location.Api/GraphQL/PhysicalAddress/`, `location/apis/Location.Api/GraphQL/FloorPlan/`, and `location/apis/Location.Api/GraphQL/Resource/`
- [x] T007 [P] Add allowlisted patch-field enums and `fieldsToUpdate` inputs for marketplace product GraphQL updates in `marketplace/apis/Marketplace.Api/GraphQL/Product/UpdateProductInput.cs`
- [x] T008 [P] Add allowlisted patch-field enums and `fieldsToUpdate` inputs for team and team-member GraphQL updates in `team/apis/Team.Api/GraphQL/Team/` and `team/apis/Team.Api/GraphQL/Member/UpdateTeamMembersInput.cs`
- [x] T009 [P] Add typed field-selection members to internal update protobufs in `api-definitions/grpc/skedular/booking/booking_core_v1.proto`, `api-definitions/grpc/skedular/customer/customer_admin_v1.proto`, `api-definitions/grpc/skedular/location/location_core_v1.proto`, `api-definitions/grpc/skedular/location/location_resources_v1.proto`, and `api-definitions/grpc/skedular/team/team_core_v1.proto`
- [x] T010 Add source-contract expectations for required field selection and removed full-replacement semantics in owning GraphQL inputs and contract tests under `booking/apis/Booking.Api/GraphQL/`, `customer/apis/Customer.Api/GraphQL/`, `location/apis/Location.Api/GraphQL/`, `marketplace/apis/Marketplace.Api/GraphQL/`, `team/apis/Team.Api/GraphQL/`, and the matching domain GraphQL integration-test directories; leave exported `schema.graphql` files to the regeneration flow
- [x] T011 Define structured logging branch expectations for selected fields, no-op, validation rejection, permission rejection, retry, and persistence failure in `specs/011-cross-domain-patch-updates/quickstart.md`

**Checkpoint**: Every owning update family has a field-selection contract target and story implementation can proceed.

---

## Phase 3: User Story 1 - Save One Edited Field Across Remaining Domains (Priority: P1) MVP

**Goal**: Users can save migrated edit units without sending unrelated record state or relying on redundant update buttons.

**Independent Test**: Change one independent field or one coherent grouped edit unit on a migrated booking, customer, location, marketplace product, or team edit surface and verify the save completes while unrelated values remain unchanged.

### Tests for User Story 1

- [x] T012 [P] [US1] Add booking autosave UI tests for private, marketplace, and recurring booking editors in `web/apps/webapp/src/components/booking/`, `web/apps/webapp-teams/src/components/booking/`, and `web/apps/webapp-spaces/src/components/booking/`
- [x] T013 [P] [US1] Add customer autosave UI tests for profile and billing edit units in `web/apps/webapp/src/components/mySettings/`, `web/apps/webapp/src/components/myBillingAndPayment/`, `web/apps/webapp-spaces/src/components/mySettings/`, and `web/apps/webapp-spaces/src/components/myBillingAndPayment/`
- [x] T014 [P] [US1] Add location and resource autosave UI tests for location, physical-address, restricted-information, floor-plan, resource, and available-hours edit units in `web/apps/webapp/src/components/organization/organizationLocation/`, `web/apps/webapp-teams/src/components/organization/organizationLocation/`, `web/apps/webapp-spaces/src/components/organization/organizationLocation/`, `web/apps/webapp/src/components/floorPlan/`, `web/apps/webapp-teams/src/components/floorPlan/`, `web/apps/webapp-spaces/src/components/floorPlan/`, and `web/apps/webapp-spaces/src/components/resource/`
- [x] T015 [P] [US1] Add marketplace product autosave UI tests for coherent product edit units in `web/apps/webapp-spaces/src/components/product/editProduct/` and `web/apps/webapp-spaces/src/components/product/product-editor-form.tsx`
- [x] T015A [P] [US1] Add team autosave UI tests for team and team-member edit units in affected team components under `web/apps/webapp/src/components/`, `web/apps/webapp-teams/src/components/`, and `web/apps/webapp-spaces/src/components/`
- [x] T016 [P] [US1] Add GraphQL contract tests for selected single-field and grouped edit-unit saves in `booking/domain/Booking.Domain.IntegrationTests/Api/GraphQL/`, `customer/domain/Customer.Domain.IntegrationTests/Api/GraphQL/`, `location/domain/Location.Domain.IntegrationTests/Api/GraphQL/`, `marketplace/domain/Marketplace.Domain.IntegrationTests/Api/GraphQL/`, and `team/domain/Team.Domain.IntegrationTests/Api/GraphQL/`

### Implementation for User Story 1

- [x] T017 [P] [US1] Add booking patch models and GraphQL mapping for booking edit units in `booking/apis/Booking.Api/Models/`, `booking/apis/Booking.Api/Mappers/GraphQlMapper.cs`, and `booking/apis/Booking.Api/GraphQL/Booking/RootMutation.cs`
- [x] T018 [P] [US1] Add customer patch models and GraphQL mapping for profile and billing edit units in `customer/apis/Customer.Api/Models/`, `customer/apis/Customer.Api/Mappers/GraphQlMapper.cs`, `customer/apis/Customer.Api/GraphQL/Customer/RootMutation.cs`, and `customer/apis/Customer.Api/GraphQL/Billing/RootMutation.cs`
- [x] T019 [P] [US1] Add location patch models and GraphQL mapping for location, restricted-information, physical-address, floor-plan, and resource edit units in `location/apis/Location.Api/Models/`, `location/apis/Location.Api/Mappers/GraphQlMapper.cs`, `location/apis/Location.Api/GraphQL/Location/RootMutation.cs`, `location/apis/Location.Api/GraphQL/PhysicalAddress/RootMutation.cs`, `location/apis/Location.Api/GraphQL/FloorPlan/RootMutation.cs`, and `location/apis/Location.Api/GraphQL/Resource/RootMutation.cs`
- [x] T020 [P] [US1] Add marketplace product patch models and GraphQL mapping in `marketplace/apis/Marketplace.Api/Models/`, `marketplace/apis/Marketplace.Api/Mappers/GraphQlMapper.cs`, and `marketplace/apis/Marketplace.Api/GraphQL/Product/RootMutation.cs`
- [x] T021 [P] [US1] Add team patch models and GraphQL mapping for team and member edit units in `team/apis/Team.Api/Models/`, `team/apis/Team.Api/Mappers/GraphQlMapper.cs`, `team/apis/Team.Api/GraphQL/Team/RootMutation.cs`, and `team/apis/Team.Api/GraphQL/Member/RootMutation.cs`
- [x] T022 [P] [US1] Implement booking autosave by coherent edit unit and remove redundant update buttons in `web/apps/webapp/src/components/booking/`, `web/apps/webapp-teams/src/components/booking/`, and `web/apps/webapp-spaces/src/components/booking/`
- [x] T023 [P] [US1] Implement customer profile and billing autosave edit units and remove redundant update buttons in `web/apps/webapp/src/components/mySettings/`, `web/apps/webapp/src/components/myBillingAndPayment/`, `web/apps/webapp/src/components/organization/organizationUser/`, `web/apps/webapp-spaces/src/components/mySettings/`, `web/apps/webapp-spaces/src/components/myBillingAndPayment/`, and `web/apps/webapp-spaces/src/components/organization/organizationUser/`
- [x] T024 [P] [US1] Implement location, floor-plan, and resource autosave edit units and remove redundant update buttons in `web/apps/webapp/src/components/organization/organizationLocation/`, `web/apps/webapp-teams/src/components/organization/organizationLocation/`, `web/apps/webapp-spaces/src/components/organization/organizationLocation/`, `web/apps/webapp/src/components/floorPlan/editFloorPlan/`, `web/apps/webapp-teams/src/components/floorPlan/editFloorPlan/`, `web/apps/webapp-spaces/src/components/floorPlan/editFloorPlan/`, and `web/apps/webapp-spaces/src/components/resource/editResource/`
- [x] T025 [P] [US1] Implement product autosave grouped edit units and remove redundant update actions in `web/apps/webapp-spaces/src/components/product/editProduct/` and `web/apps/webapp-spaces/src/components/product/product-editor-form.tsx`
- [x] T025A [P] [US1] Implement team and team-member autosave edit units and remove redundant update buttons in affected team components under `web/apps/webapp/src/components/`, `web/apps/webapp-teams/src/components/`, and `web/apps/webapp-spaces/src/components/`
- [x] T026 [US1] Regenerate backend GraphQL schemas and Relay callers for migrated GraphQL inputs with `scripts/generate-graphql.sh`, `web/apps/webapp/src/queries/__generated__/`, `web/apps/webapp-teams/src/queries/__generated__/`, and `web/apps/webapp-spaces/src/queries/__generated__/`
- [x] T027 [US1] Add structured autosave start, completion, failure, and edit-unit logging where the owning backend update workflows are invoked in `booking/apis/Booking.Api/Services/`, `customer/apis/Customer.Api/Services/`, `location/apis/Location.Api/Services/`, `marketplace/apis/Marketplace.Api/Services/`, and `team/apis/Team.Api/Services/`

**Checkpoint**: Migrated user-facing edit units can autosave through field-selected GraphQL contracts and can be tested independently.

---

## Phase 4: User Story 2 - Migrate Partial Callers Without Data Loss (Priority: P2)

**Goal**: All remaining backend update callers preserve omitted values, support explicit clears, and handle validation, no-op, and concurrency branches safely.

**Independent Test**: Submit a partial GraphQL or gRPC update with omitted values, explicit clear/default values, invalid selected values, and a detected concurrency conflict; only selected valid fields persist.

### Tests for User Story 2

- [x] T028 [P] [US2] Add booking unit tests for allowlists, omitted values, explicit clears, no-op saves, permission rejection, concurrency retry, and logging in `booking/apis/Booking.Api.UnitTests/Mappers/` and `booking/apis/Booking.Api.UnitTests/Services/`
- [x] T029 [P] [US2] Add customer unit tests for details, billing, admin identity patch validation, permission rejection, no-op saves, concurrency retry, and logging in `customer/apis/Customer.Api.UnitTests/Mappers/` and `customer/apis/Customer.Api.UnitTests/Services/`
- [x] T030 [P] [US2] Add location unit tests for location, restricted-information, physical-address, floor-plan, resource, and available-hours patch application, permission rejection, and logging in `location/apis/Location.Api.UnitTests/Mappers/` and `location/apis/Location.Api.UnitTests/Services/`
- [x] T031 [P] [US2] Add marketplace and team unit tests for product, team, and member selected-field application, permission rejection, and logging in `marketplace/apis/Marketplace.Api.UnitTests/` and `team/apis/Team.Api.UnitTests/`
- [x] T032 [P] [US2] Add gRPC integration coverage for booking, customer admin identity, location core/resources, and team field selection in `booking/domain/Booking.Domain.IntegrationTests/Api/Grpc/`, `customer/domain/Customer.Domain.IntegrationTests/Api/Grpc/`, `location/domain/Location.Domain.IntegrationTests/Api/Grpc/`, and `team/domain/Team.Domain.IntegrationTests/Api/Grpc/`

### Implementation for User Story 2

- [x] T033 [P] [US2] Apply selected-field-only booking updates with no-op and concurrency retry in `booking/apis/Booking.Api/Services/PrivateBookingService.cs`, `booking/apis/Booking.Api/Services/MarketplaceBookingService.cs`, and `booking/apis/Booking.Api/Services/PrivateRecurringBookingService.cs`
- [x] T034 [P] [US2] Apply selected-field-only customer updates in `customer/apis/Customer.Api/Services/CustomerDetailsService.cs`, `customer/apis/Customer.Api/Services/BillingService.cs`, `customer/apis/Customer.Api/Services/CustomerService.cs`, `customer/apis/Customer.Api/Mappers/GraphQlMapper.cs`, and `customer/apis/Customer.Api/Mappers/GrpcMapper.cs`
- [x] T035 [P] [US2] Apply selected-field-only location updates in `location/apis/Location.Api/Services/LocationService.cs`, `location/apis/Location.Api/Services/LocationOpeningHoursService.cs`, `location/apis/Location.Api/Services/OrganizationPhysicalAddressService.cs`, `location/apis/Location.Api/Services/LocationRestrictedInformationService.cs`, `location/apis/Location.Api/Services/FloorPlanService.cs`, `location/apis/Location.Api/Services/ResourceService.cs`, and `location/apis/Location.Api/Services/ResourceAvailableHoursService.cs`
- [x] T036 [P] [US2] Apply selected-field-only marketplace product updates in `marketplace/apis/Marketplace.Api/Services/ProductService.cs` while preserving product validation and version update behaviour
- [x] T037 [P] [US2] Apply selected-field-only team and member updates in `team/apis/Team.Api/Services/TeamService.cs` and `team/apis/Team.Api/Services/TeamMemberService.cs`
- [x] T038 [P] [US2] Map gRPC field selections into owning patch models in `booking/apis/Booking.Api/Grpc/BookingGrpcService.cs`, `customer/apis/Customer.Api/Grpc/CustomerAdminGrpcService.cs`, `location/apis/Location.Api/Grpc/LocationGrpcService.cs`, `location/apis/Location.Api/Grpc/LocationResourcesGrpcService.cs`, and `team/apis/Team.Api/Grpc/TeamGrpcService.cs`
- [x] T039 [US2] Update affected Slack gRPC callers to populate selected fields in `slack/shared/Slack.Shared/Services/CrossDomains/BookingService.cs`, `slack/shared/Slack.Shared/Services/CrossDomains/LocationResourceService.cs`, `slack/shared/Slack.Shared/Services/CrossDomains/CustomerService.cs`, and `slack/shared/Slack.Shared/Services/CrossDomains/TeamService.cs`
- [x] T040 [US2] Remove migrated full-replacement service paths or duplicate patch aliases from update code in `booking/apis/Booking.Api/`, `customer/apis/Customer.Api/`, `location/apis/Location.Api/`, `marketplace/apis/Marketplace.Api/`, and `team/apis/Team.Api/`
- [x] T041 [US2] Regenerate changed GraphQL schema exports and verify gRPC consumer builds from `scripts/generate-graphql.sh`, `api-definitions/grpc/skedular/`, `booking/domain/Booking.Domain.IntegrationTests/schema.graphql`, `customer/domain/Customer.Domain.IntegrationTests/schema.graphql`, `location/domain/Location.Domain.IntegrationTests/schema.graphql`, `marketplace/domain/Marketplace.Domain.IntegrationTests/schema.graphql`, and `team/domain/Team.Domain.IntegrationTests/schema.graphql`

**Checkpoint**: Partial update callers are safe independently of the autosave UI.

---

## Phase 5: User Story 3 - Apply One Update Experience Across the Rollout (Priority: P3)

**Goal**: Remaining domains present one consistent patch/autosave migration outcome and completion evidence.

**Independent Test**: Review each remaining domain update inventory and verify migrated surfaces expose one field-selected update contract, affected screens show consistent save/failure behaviour, and any domain with no editable owner surface is documented.

### Tests for User Story 3

- [x] T042 [P] [US3] Add contract-shape regression tests that assert one public `Update*` mutation/input family per migrated GraphQL surface in `booking/domain/Booking.Domain.IntegrationTests/`, `customer/domain/Customer.Domain.IntegrationTests/`, `location/domain/Location.Domain.IntegrationTests/`, `marketplace/domain/Marketplace.Domain.IntegrationTests/`, and `team/domain/Team.Domain.IntegrationTests/`
- [x] T043 [P] [US3] Add UI regression tests for saved-state and failed-state feedback consistency across affected edit screens in `web/apps/webapp/src/components/`, `web/apps/webapp-teams/src/components/`, and `web/apps/webapp-spaces/src/components/`

### Implementation for User Story 3

- [x] T044 [P] [US3] Normalise patch field names, payload reconciliation, and structured log property names across GraphQL update roots in `booking/apis/Booking.Api/GraphQL/`, `customer/apis/Customer.Api/GraphQL/`, `location/apis/Location.Api/GraphQL/`, `marketplace/apis/Marketplace.Api/GraphQL/`, and `team/apis/Team.Api/GraphQL/`
- [x] T045 [P] [US3] Normalise autosave success/failure UI state and keep only explicit non-save actions in affected booking, customer, location, marketplace product, resource, and team components in `web/apps/webapp/src/components/`, `web/apps/webapp-teams/src/components/`, and `web/apps/webapp-spaces/src/components/`
- [x] T046 [US3] Update the migrated-surface completion matrix and any no-surface domain findings in `specs/011-cross-domain-patch-updates/contracts/cross-domain-patch-contracts.md`
- [x] T047 [US3] Update verification and regeneration notes to match the implemented domain set in `specs/011-cross-domain-patch-updates/quickstart.md`

**Checkpoint**: The rollout can be reviewed as one consistent update pattern across remaining domains.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Regenerate outputs, run broad verification, and keep planning documents aligned with implementation reality.

- [x] T048 Run final generated-surface regeneration and reconcile checked-in schema/Relay outputs through `scripts/generate-graphql.sh`, `web/apps/webapp/src/queries/__generated__/`, `web/apps/webapp-teams/src/queries/__generated__/`, and `web/apps/webapp-spaces/src/queries/__generated__/`
- [x] T049 [P] Run affected backend unit and integration verification from `specs/011-cross-domain-patch-updates/quickstart.md`
- [x] T050 [P] Run affected web tests and lint verification from `specs/011-cross-domain-patch-updates/quickstart.md`
- [x] T051 Review changed logs for permission-rejection coverage, sensitive-value leakage, and searchable property consistency in `booking/apis/`, `customer/apis/`, `location/apis/`, `marketplace/apis/`, `team/apis/`, and `slack/shared/`
- [x] T052 Reconcile delivery notes, migrated inventory, and verification commands in `specs/011-cross-domain-patch-updates/plan.md`, `specs/011-cross-domain-patch-updates/contracts/cross-domain-patch-contracts.md`, and `specs/011-cross-domain-patch-updates/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Starts immediately and fixes the surface inventory.
- **Foundational (Phase 2)**: Depends on Setup and establishes typed field-selection contracts across domains.
- **User Story 1 (Phase 3)**: Depends on Foundational; delivers the autosave-facing MVP through migrated GraphQL contracts.
- **User Story 2 (Phase 4)**: Depends on Foundational; can overlap with User Story 1 after field-selection inputs exist.
- **User Story 3 (Phase 5)**: Depends on the desired US1 and US2 domain slices so it can normalise and document the final rollout.
- **Final Phase**: Depends on all selected story phases.

### User Story Dependencies

- **User Story 1 (P1)**: Independent after field-selection GraphQL inputs exist; it can land autosave in slices by domain.
- **User Story 2 (P2)**: Independent after field-selection contracts exist; it owns backend safety and gRPC consumer work.
- **User Story 3 (P3)**: Integrates US1 and US2 outcomes into consistent contract/UI review and completion evidence.

### Within Each User Story

- Write story tests before the matching implementation files.
- Add typed patch models and field selection before service application logic.
- Keep owning-domain service changes before cross-domain caller updates.
- Regenerate GraphQL and Relay artifacts after schema/input changes, never by hand.

### Parallel Opportunities

- Foundational GraphQL input work can run in parallel by owning domain after Setup.
- US1 booking, customer, location/resource, marketplace product, and team GraphQL/UI slices have disjoint code ownership.
- US2 backend service work can run in parallel by domain; gRPC consumer updates wait for the protobuf and owning mapper slices they consume.
- US3 UI and contract-shape regressions can run in parallel once the corresponding migrated surfaces exist.

---

## Parallel Example: User Story 1

```text
Task T017: Booking patch models and GraphQL mapping in booking/apis/Booking.Api/
Task T018: Customer patch models and GraphQL mapping in customer/apis/Customer.Api/
Task T019: Location patch models and GraphQL mapping in location/apis/Location.Api/
Task T020: Marketplace product patch models and GraphQL mapping in marketplace/apis/Marketplace.Api/
Task T022: Booking autosave UI in web/apps/webapp/src/components/booking/, web/apps/webapp-teams/src/components/booking/, and web/apps/webapp-spaces/src/components/booking/
Task T023: Customer autosave UI in web/apps/webapp/src/components/mySettings/, web/apps/webapp/src/components/myBillingAndPayment/, web/apps/webapp-spaces/src/components/mySettings/, and web/apps/webapp-spaces/src/components/myBillingAndPayment/
```

## Parallel Example: User Story 2

```text
Task T033: Booking selected-field services in booking/apis/Booking.Api/Services/
Task T034: Customer selected-field services in customer/apis/Customer.Api/Services/
Task T035: Location selected-field services in location/apis/Location.Api/Services/
Task T036: Marketplace selected-field services in marketplace/apis/Marketplace.Api/Services/
Task T037: Team selected-field services in team/apis/Team.Api/Services/
```

## Parallel Example: User Story 3

```text
Task T042: Contract-shape regression tests in domain integration-test projects
Task T043: UI save/failure regression tests in web/apps/webapp/src/components/, web/apps/webapp-teams/src/components/, and web/apps/webapp-spaces/src/components/
Task T044: GraphQL field/log normalisation in owning API GraphQL roots
Task T045: Autosave UI state normalisation in web edit components
```

---

## Implementation Strategy

### MVP First (User Story 1)

1. Complete Setup and Foundational contract work.
2. Pick one screen-backed domain slice, preferably customer profile or private booking, and complete its US1 tests, GraphQL field selection, autosave edit unit, generated artifacts, and logging.
3. Validate single-field or grouped-edit autosave without waiting for gRPC migration work.
4. Repeat US1 domain slices before broad consistency polish.

### Incremental Delivery

1. Establish field-selection contracts and generated-surface discipline.
2. Deliver US1 autosave slices by domain.
3. Deliver US2 backend/gRPC safety slices by domain and migrate consumers.
4. Complete US3 consistency regressions and coverage evidence.
5. Run final regeneration, backend verification, web verification, and documentation reconciliation.

### Parallel Team Strategy

1. Complete Setup and Foundational input work together.
2. Split domain ownership:
   - Booking owner: booking GraphQL/gRPC/services and booking web editors.
   - Customer owner: customer GraphQL/gRPC/services and profile/billing screens.
   - Location owner: location GraphQL/gRPC/services and location/resource screens.
   - Marketplace/team owner: marketplace product and team/member contract/service/UI slices.
3. Integrate Slack caller updates after owning gRPC changes are ready.
4. Finish shared regeneration, regression, logging, and documentation checks.

## Notes

- Tasks marked `[P]` own different files or directories and can be delegated after their phase dependencies are met.
- User-story tasks keep backend safety and UI autosave independently testable.
- Repository rules prohibit hand-editing generated GraphQL, Relay, or protobuf-generated output.
- Integration-test persistence assertions must use repository or query-layer methods instead of raw `DbContext`.
