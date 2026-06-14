# Tasks: Skedular Teams Pricing Catalog Redesign

**Input**: Design documents from `/specs/027-teams-pricing-catalogue/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Automated tests are required by FR-030 and the constitution.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to, such as US1, US2, US3
- Include exact file paths in descriptions

## Phase 1: Setup (Organization-Owned Pricing Infrastructure)

**Purpose**: Confirm current code ownership, create feature scaffolding, and prepare source-definition-first contract work.

- [X] T001 Inventory existing Teams offering, subscription, renewal, and entitlement behavior in `specs/027-teams-pricing-catalogue/current-state-review.md`
- [X] T002 [P] Inventory static pricing dependencies in `specs/027-teams-pricing-catalogue/frontend-pricing-inventory.md`
- [X] T003 [P] Inventory generated GraphQL/OpenAPI/Relay surfaces affected by pricing changes in `specs/027-teams-pricing-catalogue/generated-surface-inventory.md`
- [X] T004 Create pricing catalog model folder in `src/organization/shared/Organization.Shared/Models/PricingCatalog/`
- [X] T005 Create organization subscription service folder in `src/organization/shared/Organization.Shared/Services/Pricing/`
- [X] T006 Create organization pricing GraphQL folder in `src/organization/apis/Organization.Api/GraphQL/Pricing/`
- [X] T007 Create web pricing catalog folder in `src/web/apps/public-web/src/data/pricing-catalog/`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Organization-owned read models, source definitions, persistence foundations for new pricing support, and logging conventions that block all user stories.

**Critical**: No user story implementation should start until this phase is complete.

- [X] T008 Define pricing catalog organization enums and constants in `src/organization/shared/Organization.Shared/Models/PricingCatalog/PricingCatalogConstants.cs`
- [X] T009 Define pricing catalog organization read models in `src/organization/shared/Organization.Shared/Models/PricingCatalog/PricingCatalog.cs`
- [X] T010 Define organization offering plan read model in `src/organization/shared/Organization.Shared/Models/PricingCatalog/OrganizationOfferingPlan.cs`
- [X] T011 Define organization entitlement reason codes in `src/organization/shared/Organization.Shared/Models/PricingCatalog/EntitlementReasonCode.cs`
- [X] T012 Extend V1 offering catalog mapping without changing Early Bird semantics in `src/organization/shared/Organization.Shared/Services/Pricing/OfferingPricingCatalogMappingExtensions.cs`
- [X] T013 [P] Cover catalog mapping through organization pricing service tests in `src/organization/apis/Organization.Api.UnitTests/Services/Pricing/`
- [X] T014 [P] Cover entitlement reason code choices through organization GraphQL pricing choice tests in `src/organization/apis/Organization.Api.UnitTests/GraphQL/Pricing/RootQueryTests/PricingCatalogChoicesShould.cs`
- [X] T015 Extend organization offering entity with nullable catalog fields in `src/organization/shared/Organization.Shared/Database/Entities/OrganizationOffering.cs`
- [X] T016 Reuse existing active-member entity in `src/organization/shared/Organization.Shared/Database/Entities/OrganizationOfferingActiveMember.cs`
- [X] T017 Wire pricing subscription entities into organization database context in `src/organization/shared/Organization.Shared/Database/OrganizationDbContext.cs`
- [X] T018 Reuse organization offering repository for subscription records in `src/organization/shared/Organization.Shared/Repositories/OrganizationOfferingRepository.cs`
- [X] T019 Reuse existing active-member repository in `src/organization/shared/Organization.Shared/Repositories/OrganizationOfferingActiveMemberRepository.cs`
- [X] T020 Add repository factory accessors for pricing repositories in `src/organization/shared/Organization.Shared/Repositories/RepositoryFactory.cs`
- [X] T021 Add EF migration for subscription persistence in `src/organization/shared/Organization.Shared/Database/Migrations/`
- [X] T022 Define structured logging event names, performance metric names, and property constants in `src/organization/shared/Organization.Shared/Services/Pricing/PricingLogEvents.cs`
- [X] T023 Add dependency injection registration for pricing services and repositories in `src/organization/shared/Organization.Shared/Extensions.cs`
- [X] T024 Add GraphQL pricing choice details types in `src/organization/apis/Organization.Api/GraphQL/Pricing/PricingChoiceDetails.cs`

**Checkpoint**: Foundation ready for user story implementation.

---

## Phase 3: User Story 1 - Review and Evolve Existing Pricing Safely (Priority: P1) MVP

**Goal**: Document the current state, make the V1 extension decision explicit, and preserve Free/Early Bird behavior before broader catalog work.

**Independent Test**: Review the completed decision artifacts and run shared/organization tests proving V1 extension preserves Free and Early Bird behavior.

### Tests for User Story 1

- [X] T025 [P] [US1] Add V1 version decision tests in organization pricing tests
- [X] T026 [P] [US1] Add Early Bird read-only compatibility tests in `src/organization/apis/Organization.Api.UnitTests/Services/Pricing/EarlyBirdOfferingCompatibilityShould.cs`
- [X] T027 [P] [US1] Add Free offering compatibility tests in `src/organization/apis/Organization.Api.UnitTests/Services/Pricing/FreeOfferingCompatibilityShould.cs`
- [X] T028 [P] [US1] Add version decision and existing-subscription unchanged integration test in `src/organization/domain/Organization.Domain.IntegrationTests/Api/GraphQL/PricingCatalogVersionShould.cs`

### Implementation for User Story 1

- [X] T029 [US1] Record V1 extension decision and reviewed alternatives in `specs/027-teams-pricing-catalogue/current-state-review.md`
- [X] T030 [US1] Implement pricing catalog version service in `src/organization/shared/Organization.Shared/Services/Pricing/PricingCatalogVersionService.cs`
- [X] T031 [US1] Implement legacy offering compatibility mapper in `src/organization/shared/Organization.Shared/Services/Pricing/LegacyOfferingCompatibilityMapper.cs`
- [X] T032 [US1] Implement existing-offering read-only compatibility service in `src/organization/shared/Organization.Shared/Services/Pricing/OrganizationOfferingCompatibilityService.cs`
- [X] T033 [US1] Add structured logs for version selection and legacy compatibility decisions in `src/organization/shared/Organization.Shared/Services/Pricing/OrganizationOfferingCompatibilityService.cs`
- [X] T034 [US1] Expose catalog version details in GraphQL in `src/organization/apis/Organization.Api/GraphQL/Pricing/PricingCatalogVersionDetails.cs`

**Checkpoint**: US1 is complete when current-state review, V1 decision, Free compatibility, and Early Bird unchanged behavior are independently testable.

---

## Phase 4: User Story 2 - Render Pricing From a Server-Driven Product Catalog (Priority: P1)

**Goal**: Provide catalog data from backend-owned sources and render Teams pricing without hardcoded pricing values in public-web.

**Independent Test**: Change backend catalog data and verify public-web pricing renders plan names, ordering, features, prices, capacity options, recommendations, and Contact Us behavior from catalog data.

### Tests for User Story 2

- [X] T035 [P] [US2] Add pricing catalog service unit tests in `src/organization/apis/Organization.Api.UnitTests/Services/Pricing/PricingCatalogServiceShould.cs`
- [X] T036 [P] [US2] Add GraphQL catalog query integration tests in `src/organization/domain/Organization.Domain.IntegrationTests/Api/GraphQL/PricingCatalogQueryShould.cs`
- [X] T037 [P] [US2] Add public-web catalog rendering tests in `src/web/apps/public-web/tests/pricing-catalog-rendering.test.ts`
- [X] T038 [P] [US2] Add static pricing removal regression tests in `src/web/apps/public-web/tests/public-site-content.test.ts`

### Implementation for User Story 2

- [X] T039 [P] [US2] Implement Teams catalog seed/mapping data in `src/organization/shared/Organization.Shared/Services/Pricing/TeamsPricingCatalogProvider.cs`
- [X] T040 [P] [US2] Implement product-aware catalog service in `src/organization/apis/Organization.Api/Services/Pricing/PricingCatalogService.cs`
- [X] T041 [US2] Add pricing catalog GraphQL details types in `src/organization/apis/Organization.Api/GraphQL/Pricing/PricingCatalogDetails.cs`
- [X] T042 [US2] Add `pricingCatalog` GraphQL query in `src/organization/apis/Organization.Api/GraphQL/Pricing/RootQuery.cs`
- [X] T043 [US2] Add catalog mapper methods in `src/organization/apis/Organization.Api/Mappers/GraphQlMapper.cs`
- [X] T044 [US2] Add product offering, plan availability, offering plan status, and reason-code choice queries in `src/organization/apis/Organization.Api/GraphQL/Pricing/RootQuery.cs`
- [X] T045 [US2] Replace hardcoded public-web pricing data with catalog adapter in `src/web/apps/public-web/src/data/pricing-catalog/pricing-catalog.ts`
- [X] T046 [US2] Update public-web pricing page data usage in `src/web/apps/public-web/src/data/pricing.ts`
- [X] T047 [US2] Update public-web pricing tests for Free, Pay As You Go, Enterprise Capacity, and Contact Us rendering in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [X] T048 [US2] Add structured logs and catalog read duration metrics for retrieval, product filtering, and Contact Us threshold decisions in `src/organization/apis/Organization.Api/Services/Pricing/PricingCatalogService.cs`
- [X] T049 [US2] Regenerate GraphQL schema artifacts with `scripts/generate-graphql.sh`

**Checkpoint**: US2 is complete when catalog retrieval and public pricing rendering work from backend-owned catalog data.

---

## Phase 5: User Story 3 - Subscribe Organizations to the Correct Teams Offering (Priority: P2)

**Goal**: Create and maintain durable Teams organization offering state for new Free and Pay As You Go self-service selections while setting Enterprise Capacity only through Skedular-admin negotiated updates.

**Independent Test**: Create new organization offerings and verify product, plan, capacity snapshots, catalog version, effective dates, and status are persisted and exposed; verify existing offerings are read-only compatibility inputs.

### Tests for User Story 3

- [X] T050 [P] [US3] Add organization offering plan lifecycle unit tests in `src/organization/apis/Organization.Api.UnitTests/Services/Pricing/OrganizationTeamsSubscriptionServiceShould.cs`
- [X] T051 [P] [US3] Reuse existing organization offering repository tests for offering persistence in `src/organization/shared/Organization.Shared/Repositories/OrganizationOfferingRepository.cs`
- [X] T052 [P] [US3] Confirm active-member repository reuse in `src/organization/shared/Organization.Shared/Repositories/OrganizationOfferingActiveMemberRepository.cs`
- [X] T053 [P] [US3] Add subscription GraphQL integration tests in `src/organization/domain/Organization.Domain.IntegrationTests/Api/GraphQL/OrganizationTeamsSubscriptionShould.cs`
- [X] T054 [P] [US3] Add read-only compatibility integration tests for existing Free and Early Bird organizations in `src/organization/domain/Organization.Domain.IntegrationTests/Api/GraphQL/OrganizationSubscriptionCompatibilityShould.cs`

### Implementation for User Story 3

- [X] T055 [US3] Implement organization Teams offering-plan service in `src/organization/apis/Organization.Api/Services/Pricing/OrganizationTeamsSubscriptionService.cs`
- [X] T056 [US3] Implement offering creation for Free and Pay As You Go in `src/organization/apis/Organization.Api/Services/Pricing/OrganizationTeamsSubscriptionService.cs`
- [X] T057 [US3] Implement Skedular-admin Enterprise offering update through the Organization workaround REST API in `src/organization/apis/Organization.Api/Services/OrganizationOfferingService.cs`
- [X] T058 [US3] Implement read-only compatibility for existing Free organizations in `src/organization/shared/Organization.Shared/Services/Pricing/OrganizationOfferingCompatibilityService.cs`
- [X] T059 [US3] Preserve Early Bird offerings unchanged in `src/organization/shared/Organization.Shared/Services/Pricing/OrganizationOfferingCompatibilityService.cs`
- [X] T060 [US3] Add organization Teams subscription GraphQL details type in `src/organization/apis/Organization.Api/GraphQL/Pricing/OrganizationTeamsSubscriptionDetails.cs`
- [X] T061 [US3] Add `organizationTeamsSubscription` GraphQL query in `src/organization/apis/Organization.Api/GraphQL/Pricing/RootQuery.cs`
- [X] T062 [US3] Add `updateOrganizationTeamsSubscription` GraphQL mutation in `src/organization/apis/Organization.Api/GraphQL/Pricing/RootMutation.cs`
- [X] T063 [US3] Publish organization offering changes through existing organization outbox path in `src/organization/shared/Organization.Shared/Publishers/OrganizationOutboxPublisher.cs`
- [X] T064 [US3] Add structured logs for offering create, update, read-only compatibility, and validation failures in `src/organization/apis/Organization.Api/Services/Pricing/OrganizationTeamsSubscriptionService.cs`
- [X] T065 [US3] Regenerate GraphQL schema artifacts with `scripts/generate-graphql.sh`

**Checkpoint**: US3 is complete when new offering rows are durable and queryable, existing offerings remain unchanged, and Early Bird remains honored.

---

## Phase 6: User Story 4 - Enforce User Capacity and Plan Entitlements Consistently (Priority: P2)

**Goal**: Centralize entitlement decisions and apply Free, Pay As You Go, and Enterprise Capacity rules consistently across Organization, Booking, Team, and Location workflows.

**Independent Test**: Exercise organization, booking, team, and location workflows under each plan and verify the same entitlement decision reason codes and outcomes.

### Tests for User Story 4

- [X] T066 [P] [US4] Add entitlement decision unit tests in `src/shared/Api.Shared.Services.UnitTests/Offering/PricingEntitlementEvaluatorTests/`
- [X] T067 [P] [US4] Add active member qualification unit tests in `src/organization/apis/Organization.Api.UnitTests/Services/Pricing/OrganizationOfferingActiveMemberQualificationServiceShould.cs`
- [X] T068 [P] [US4] Cover Free active-user limit with shared entitlement and booking authorization unit tests instead of integration tests
- [X] T069 [P] [US4] Add Team entitlement tests in `src/team/apis/Team.Api.UnitTests/Services/Authorization/OrganizationOfferingServiceTests/CanCreateTeamShould.cs`
- [X] T070 [P] [US4] Add Location entitlement tests in `src/location/apis/Location.Api.UnitTests/Services/Authorization/OrganizationOfferingServiceTests/CanCreateLocationShould.cs`
- [X] T071 [P] [US4] Add Booking active-user qualification tests in `src/booking/apis/Booking.Api.UnitTests/Services/Authorization/OrganizationOfferingServiceTests/IsMoreInteractionAllowedAsyncShould.cs`
- [X] T072 [P] [US4] Cover cross-domain entitlement behavior with Team, Location, and Booking authorization unit tests instead of system tests

### Implementation for User Story 4

- [X] T073 [US4] Implement shared entitlement evaluator against offering capacity fields in `src/shared/Api.Shared.Services/Offering/PricingEntitlementEvaluator.cs`
- [X] T074 [US4] Implement active member qualification service in `src/organization/shared/Organization.Shared/Services/Pricing/OrganizationOfferingActiveMemberQualificationService.cs`
- [X] T075 [US4] Add entitlement GraphQL/internal service surface in `src/organization/apis/Organization.Api/GraphQL/Pricing/EntitlementDecisionDetails.cs`
- [X] T076 [US4] Update Team authorization to enforce shared entitlement outcomes from local projected JSON state in `src/team/apis/Team.Api/Services/Authorization/OrganizationOfferingService.cs`
- [X] T077 [US4] Update Location authorization to enforce shared entitlement outcomes from local projected JSON state in `src/location/apis/Location.Api/Services/Authorization/OrganizationOfferingService.cs`
- [X] T078 [US4] Update Booking interaction authorization to record monthly active users and enforce shared entitlement outcomes from local projected JSON state in `src/booking/apis/Booking.Api/Services/Authorization/OrganizationOfferingService.cs`
- [X] T079 [US4] Add organization event pricing/subscription JSON projection fields needed by Team, Location, and Booking in `api-definitions/events/skedular/organization_v1_value.proto`
- [X] T080 [US4] Update organization event mapper for subscription and entitlement projection fields in `src/organization/shared/Organization.Shared/Mappers/EventMapper.cs`
- [X] T081 [US4] Update Team organization subscriber projection for entitlement fields in `src/team/processors/Team.Processors/Mappers/EventMapper.cs`
- [X] T082 [US4] Update Location organization subscriber projection for entitlement fields in `src/location/processors/Location.Processors/Mappers/EventMapper.cs`
- [X] T083 [US4] Update Booking organization subscriber projection for entitlement fields in `src/booking/processors/Booking.Processors/Mappers/EventMapper.cs`
- [X] T084 [US4] Add structured logs for active-user qualification and entitlement allow/block decisions in `src/shared/Api.Shared.Services/Offering/PricingEntitlementEvaluator.cs`
- [X] T085 [US4] Regenerate GraphQL artifacts with `scripts/generate-graphql.sh` and validate event protobuf generation through `Api.Shared.Clients` builds

**Checkpoint**: US4 is complete when all relevant domains apply the same entitlement outcomes and reason codes.

---

## Phase 7: User Story 5 - Prepare the Pricing Framework for Spaces Reuse (Priority: P3)

**Goal**: Ensure the catalog can represent Teams and Spaces without implementing full Spaces commercial behavior.

**Independent Test**: Query all-product, Teams-only, and Spaces-only catalog views and verify Spaces responses are valid but do not leak Teams-only assumptions.

### Tests for User Story 5

- [X] T086 [P] [US5] Add product filtering unit tests in `src/organization/apis/Organization.Api.UnitTests/Services/Pricing/PricingCatalogServiceTests/GetCatalogShould.cs`
- [X] T087 [P] [US5] Cover Spaces catalog shape with unit tests instead of integration tests because provider/filtering behavior is in-memory catalog construction
- [X] T088 [P] [US5] Add public-web product chooser rendering tests in `src/web/apps/public-web/tests/pricing-product-chooser.test.ts`

### Implementation for User Story 5

- [X] T089 [US5] Add framework-level Spaces product offering metadata in `src/organization/shared/Organization.Shared/Services/Pricing/SpacesPricingCatalogProvider.cs`
- [X] T090 [US5] Update product-aware catalog service to return all-product, Teams-only, and Spaces-only views in `src/organization/apis/Organization.Api/Services/Pricing/PricingCatalogService.cs`
- [X] T091 [US5] Update public-web pricing product chooser to render product offerings from catalog data in `src/web/apps/public-web/src/data/pricing-catalog/pricing-catalog.ts`
- [X] T092 [US5] Add structured logs for product catalog filtering in `src/organization/apis/Organization.Api/Services/Pricing/PricingCatalogService.cs`

**Checkpoint**: US5 is complete when the framework supports Spaces catalog representation without Teams-specific leakage.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Regeneration, validation, documentation, and release readiness across all stories.

- [X] T093 Run full generation from repo root with `make generate`
- [X] T094 [P] Run shared and organization unit tests listed in `specs/027-teams-pricing-catalogue/quickstart.md`
- [X] T095 [P] Run team, location, and booking unit tests listed in `specs/027-teams-pricing-catalogue/quickstart.md`
- [X] T096 Run domain integration tests listed in `specs/027-teams-pricing-catalogue/quickstart.md`
- [X] T097 Run public-web test suite from `specs/027-teams-pricing-catalogue/quickstart.md`
- [X] T098 Run webapp lint/build validation from `specs/027-teams-pricing-catalogue/quickstart.md`
- [X] T099 Verify generated GraphQL, OpenAPI, event, and Relay artifacts are not hand-edited in `api-definitions/`
- [X] T100 Update validation outcomes and known gaps in `specs/027-teams-pricing-catalogue/quickstart.md`
- [X] T101 Review user-facing and operator-facing copy for American spelling in `src/web/apps/public-web/src/data/pricing-catalog/pricing-catalog.ts`
- [X] T102 Review structured logging coverage for all primary workflows in `specs/027-teams-pricing-catalogue/current-state-review.md`
- [X] T103 Validate catalog read p95 under 500 ms and entitlement check p95 under 100 ms in `specs/027-teams-pricing-catalogue/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No dependencies.
- **Phase 2 Foundational**: Depends on Phase 1 and blocks all user stories.
- **US1 and US2**: Can start after Phase 2. These form the MVP because the business needs the version decision and server-driven catalog first.
- **US3**: Depends on Phase 2 and benefits from US1 catalog/version decisions.
- **US4**: Depends on US3 new subscription state support and foundational entitlement DTOs.
- **US5**: Depends on US2 catalog service shape.
- **Phase 8 Polish**: Depends on selected user stories being complete.

### User Story Dependencies

- **US1 Review and Evolve Existing Pricing Safely**: No dependency on other stories after foundation.
- **US2 Render Pricing From Server-Driven Product Catalog**: No dependency on other stories after foundation.
- **US3 Subscribe Organizations to Correct Teams Offering**: Depends on US1 decisions for V1 extension and Early Bird preservation.
- **US4 Enforce Entitlements Consistently**: Depends on US3 new subscription state support, event-projected JSON state, and active-user tracking.
- **US5 Prepare Spaces Reuse**: Depends on US2 product-aware catalog shape.

### Parallel Opportunities

- Setup inventory tasks T002 and T003 can run in parallel.
- Foundational shared DTO tests T013 and T014 can run in parallel with persistence scaffolding after DTO files exist.
- Tests within each user story marked `[P]` can be written in parallel before implementation.
- US1 and US2 can proceed in parallel after Phase 2.
- US3 and US5 can proceed in parallel after US1/US2 prerequisites are satisfied.

---

## Parallel Example: User Story 2

```text
Task: "T035 Add pricing catalog service unit tests in src/organization/apis/Organization.Api.UnitTests/Services/Pricing/PricingCatalogServiceShould.cs"
Task: "T036 Add GraphQL catalog query integration tests in src/organization/domain/Organization.Domain.IntegrationTests/Api/GraphQL/PricingCatalogQueryShould.cs"
Task: "T037 Add public-web catalog rendering tests in src/web/apps/public-web/tests/pricing-catalog-rendering.test.ts"
Task: "T038 Add static pricing removal regression tests in src/web/apps/public-web/tests/public-site-content.test.ts"
```

## Parallel Example: User Story 4

```text
Task: "T066 Add entitlement decision unit tests in src/organization/apis/Organization.Api.UnitTests/Services/Pricing/OrganizationEntitlementServiceShould.cs"
Task: "T069 Add Team entitlement tests in src/team/apis/Team.Api.UnitTests/Services/Authorization/OrganizationOfferingServiceTests/CanCreateTeamShould.cs"
Task: "T070 Add Location entitlement tests in src/location/apis/Location.Api.UnitTests/Services/Authorization/OrganizationOfferingServiceTests/CanCreateLocationShould.cs"
Task: "T071 Add Booking active-user qualification tests in src/booking/apis/Booking.Api.UnitTests/Services/Authorization/OrganizationOfferingServiceTests/IsMoreInteractionAllowedAsyncShould.cs"
```

---

## Implementation Strategy

### MVP First

1. Complete Phase 1 and Phase 2.
2. Complete US1 to lock the V1 extension decision and preserve Early Bird.
3. Complete US2 to deliver server-driven Teams pricing rendering.
4. Stop and validate catalog retrieval, public-web pricing, and generated schema outputs before subscription lifecycle and entitlement expansion.

### Incremental Delivery

1. US1: review and safe evolution decision.
2. US2: backend-owned catalog and public pricing rendering.
3. US3: durable organization Teams subscriptions.
4. US4: centralized active-user and entitlement enforcement.
5. US5: framework-level Spaces catalog reuse.
6. Phase 8: full generation, tests, and operational readiness.

### Validation Discipline

- Write story tests before implementation and confirm they fail for missing behavior.
- Regenerate source-derived artifacts only after changing source definitions.
- Do not hand-edit generated GraphQL, OpenAPI, protobuf, or Relay outputs.
- Use repository-layer queries in integration tests instead of raw `DbContext`.
- Include structured log assertions for critical success and failure paths.
