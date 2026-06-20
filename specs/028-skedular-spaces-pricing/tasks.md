# Tasks: Skedular Spaces Pricing Implementation

**Input**: Design documents from `/specs/028-skedular-spaces-pricing/`
**Prerequisites**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contracts/](./contracts/), [quickstart.md](./quickstart.md)

**Tests**: Included because the specification, constitution, and quickstart require unit, integration, logging, and frontend validation.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3, US4, US5)
- All tasks include exact file paths

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish concrete file locations and generated-contract boundaries before feature work starts.

- [x] T001 Review existing product-specific pricing constants and Spaces placeholder catalog in `src/organization/shared/Organization.Shared/Models/PricingCatalog/PricingCatalogConstants.cs` and `src/organization/shared/Organization.Shared/Services/Pricing/SpacesPricingCatalogProvider.cs`
- [x] T002 Review Booking creation and recurring generation entry points in `src/booking/apis/Booking.Api/Services/PrivateBookingService.cs`, `src/booking/shared/Booking.Shared/Services/PrivateBookingService.cs`, and `src/booking/shared/Booking.Shared/Workflows/BookPrivateRecurringResources.cs`
- [x] T003 [P] Create Spaces pricing implementation notes in `specs/028-skedular-spaces-pricing/current-state-review.md` covering graphify findings for `SpacesPricingCatalogProvider`, `PrivateBookingService`, and `BookPrivateRecurringResources`
- [x] T004 [P] Inventory generated surfaces affected by Spaces pricing in `specs/028-skedular-spaces-pricing/generated-surface-inventory.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core models, contracts, persistence, and service wiring required by every user story.

**Critical**: No user story work should begin until this phase is complete.

- [x] T005 Extend Spaces plan codes, names, and availability mappings in `src/organization/shared/Organization.Shared/Models/PricingCatalog/PricingCatalogConstants.cs`
- [x] T006 Update Spaces catalog data for Free, Growth, Business, and Contact Us in `src/organization/shared/Organization.Shared/Services/Pricing/SpacesPricingCatalogProvider.cs`
- [x] T007 [P] Add unit coverage for Spaces Free/Growth/Business/Contact Us catalog data in `src/organization/apis/Organization.Api.UnitTests/Services/Pricing/PricingCatalogServiceTests/GetCatalogShould.cs`
- [x] T008 Add Spaces subscription read model in `src/organization/shared/Organization.Shared/Models/PricingCatalog/OrganizationSpacesSubscription.cs`
- [x] T009 Add Spaces quota decision and reason models in `src/shared/Api.Shared.Services/Offering/SpacesQuotaDecision.cs` and `src/shared/Api.Shared.Services/Offering/EntitlementReasonCode.cs`
- [x] T010 Add booking usage period entity in `src/booking/shared/Booking.Shared/Database/Entities/SpacesBookingUsagePeriod.cs` (removed — using Organization.Offering JSONB instead)
- [x] T011 Add EF configuration for Spaces booking usage periods in `src/booking/shared/Booking.Shared/Database/BookingDbContext.cs` (not needed — JSONB column on Organization)
- [x] T012 Create Booking EF migration for Spaces booking usage periods in `src/booking/shared/Booking.Shared/Database/Migrations/` (removed — no new migration needed)
- [x] T013 Define repository contract for current-period usage reads in `src/booking/shared/Booking.Shared/Repositories/SpacesBookingUsageRepository.cs` (counts persisted Booking rows for the current billing period)
- [x] T014 Implement repository methods for current-period usage reads in `src/booking/shared/Booking.Shared/Repositories/SpacesBookingUsageRepository.cs` (async EF booking-row count; no raw SQL counter updates)
- [x] T015 Register Spaces booking usage repository and quota services in `src/booking/shared/Booking.Shared/Extensions.cs`
- [x] T016 Add structured log event ids for Spaces quota decisions and rollover in `src/booking/shared/Booking.Shared/Services/SpacesPricingLogEvents.cs`
- [x] T017 Add Organization GraphQL pricing details for Spaces subscription in `src/organization/apis/Organization.Api/GraphQL/Pricing/OrganizationSpacesSubscriptionDetails.cs`
- [x] T018 Add Booking GraphQL quota status details and root query in `src/booking/apis/Booking.Api/GraphQL/Booking/BookingSpacesQuotaStatusDetails.cs` and `src/booking/apis/Booking.Api/GraphQL/Booking/RootQuery.cs`
- [x] T019 Update pricing GraphQL mutation input/payload for Spaces subscription changes in `src/organization/apis/Organization.Api/GraphQL/Pricing/RootMutation.cs`
- [x] T020 [P] Add GraphQL choice coverage for Spaces plans and quota reason codes in `src/organization/apis/Organization.Api.UnitTests/GraphQL/Pricing/RootQueryTests/PricingCatalogChoicesShould.cs`
- [x] T021 Add migration/default-Free assignment service for organizations without Spaces subscription state in `src/organization/shared/Organization.Shared/Services/Pricing/OrganizationSpacesSubscriptionMigrationService.cs`

**Checkpoint**: Foundation ready. User story implementation can now begin.

---

## Phase 3: User Story 1 - Free Plan with Monthly Booking Instance Quota (Priority: P1) MVP

**Goal**: Free organizations can create bookings until 100 current-period booking instances are reached, then receive a quota-exceeded response with upgrade options.

**Independent Test**: Create current-period bookings up to 100 for a Free organization; verify the 101st current-period instance is rejected with current usage, quota, and backend catalog upgrade options. Verify out-of-period instances are excluded.

### Tests for User Story 1

- [x] T022 [P] [US1] Add quota decision unit tests for Free allow/block and out-of-period exclusion in `src/booking/shared/Booking.Shared.UnitTests/Services/SpacesBookingQuotaServiceTests/CanCreateBookingInstancesShould.cs`
- [x] T023 [P] [US1] Add repository unit tests for current-period booking count behavior in `src/booking/shared/Booking.Shared.UnitTests/Repositories/SpacesBookingUsageRepositoryTests/CountCurrentPeriodBookingInstancesShould.cs`
- [x] T024 [P] [US1] Add Booking API unit tests for Free quota rejection in `src/booking/apis/Booking.Api.UnitTests/Services/PrivateBookingServiceTests/AddAsyncShould.cs`
- [x] T025 [P] [US1] Add Booking integration test for Free quota persistence through repositories in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/SpacesBookingUsageRepositoryShould.cs`
- [x] T026 [P] [US1] Add Booking GraphQL quota status integration test in `src/booking/domain/Booking.Domain.IntegrationTests/Api/GraphQL/SpacesQuotaStatusShould.cs`

### Implementation for User Story 1

- [x] T027 [US1] Implement `SpacesBookingQuotaService` current-period calculation and catalog upgrade-plan lookup in `src/booking/shared/Booking.Shared/Services/SpacesBookingQuotaService.cs`
- [x] T028 [US1] Add `ISpacesBookingQuotaService` interface in `src/booking/shared/Booking.Shared/Services/SpacesBookingQuotaService.cs`
- [x] T029 [US1] Add current-period scheduled-instance extraction helper in `src/booking/shared/Booking.Shared/Services/SpacesBookingInstanceCounter.cs`
- [x] T030 [US1] Enforce quota before private booking creation in `src/booking/apis/Booking.Api/Services/PrivateBookingService.cs`
- [x] T031 [US1] Count Spaces usage from persisted booking rows so failed booking creation is naturally excluded in `src/booking/shared/Booking.Shared/Services/PrivateBookingService.cs`
- [x] T032 [US1] Map quota exceeded decisions to API-safe errors in `src/booking/shared/Booking.Shared/Exceptions/SpacesBookingQuotaExceeded.cs`
- [x] T033 [US1] Return current usage, quota limit, attempted current-period count, excluded out-of-period count, remaining quota, and upgrade plans from quota errors in `src/booking/apis/Booking.Api/GraphQL/Booking/RootMutation.cs`
- [x] T034 [US1] Add structured logs for Free quota allow/block and out-of-period exclusion in `src/booking/shared/Booking.Shared/Services/SpacesBookingQuotaService.cs`

**Checkpoint**: US1 is independently functional and is the MVP.

---

## Phase 4: User Story 2 - Paid Plan with Usage-Based Billing (Priority: P1)

**Goal**: Growth and Business organizations use catalog-driven monthly booking-instance quotas, and quota enforcement continues to block at the effective limit using current-period booking counts.

**Independent Test**: Assign Growth to an organization, create 400 current-period bookings successfully, block the 501st, and verify quota status changes when the billing period boundaries move to a period with no matching bookings.

### Tests for User Story 2

- [x] T035 [P] [US2] Add unit tests for Growth and Business effective quota decisions in `src/booking/shared/Booking.Shared.UnitTests/Services/SpacesBookingQuotaServiceTests/CanCreateBookingInstancesShould.cs`
- [x] T036 [P] [US2] Add no-op rollover unit tests in `src/booking/shared/Booking.Shared.UnitTests/Services/SpacesBookingUsageRolloverServiceTests/RolloverCurrentPeriodsShould.cs`
- [x] T037 [P] [US2] Add Organization subscription unit tests for Growth and Business assignment in `src/organization/apis/Organization.Api.UnitTests/Services/Pricing/OrganizationSpacesSubscriptionServiceTests/UpdateAsyncShould.cs`
- [x] T038 [P] [US2] Add integration test documenting that rollover no longer mutates usage counters in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/SpacesBookingUsageRolloverShould.cs`

### Implementation for User Story 2

- [x] T039 [US2] Implement Organization Spaces subscription service in `src/organization/shared/Organization.Shared/Services/Pricing/OrganizationSpacesSubscriptionService.cs`
- [x] T040 [US2] Add subscription repository methods for Spaces plan assignment in `src/organization/shared/Organization.Shared/Repositories/OrganizationOfferingRepository.cs`
- [x] T041 [US2] Keep Spaces usage rollover service as a compatibility no-op because usage is counted from booking rows in `src/booking/shared/Booking.Shared/Services/SpacesBookingUsageRolloverService.cs`
- [x] T042 [US2] Add Temporal activity for Spaces monthly usage rollover in `src/booking/shared/Booking.Shared/Activities/SpacesBookingUsageRolloverIntegrations.cs`
- [x] T043 [US2] Wire Spaces rollover scheduling with existing Temporal startup/worker registration in `src/booking/shared/Booking.Shared/BookingSharedServiceCollectionExtensions.cs`
- [x] T044 [US2] Add GraphQL mutation mapping for Growth and Business plan changes in `src/organization/apis/Organization.Api/GraphQL/Pricing/RootMutation.cs`
- [x] T045 [US2] Add structured logs for subscription changes and quota-period compatibility flow in `src/organization/shared/Organization.Shared/Services/Pricing/OrganizationSpacesSubscriptionService.cs` and `src/booking/shared/Booking.Shared/Services/SpacesBookingUsageRolloverService.cs`

**Checkpoint**: US2 works independently after the foundation and US1 quota service are present.

---

## Phase 5: User Story 4 - Recurring Booking Instance Generation (Priority: P1)

**Goal**: Recurring booking generation validates each generated current-period instance against remaining quota and blocks only instances that exceed quota.

**Independent Test**: With 95/100 current-period usage, generate three recurring current-period instances and verify only two are created while the third returns a quota-exceeded outcome.

### Tests for User Story 4

- [x] T046 [P] [US4] Add recurring quota unit tests in `src/booking/shared/Booking.Shared.UnitTests/Activities/BookingIntegrationsTests/AdjustRequiredResourcesForPrivateRecurringBookingAsyncShould.cs`
- [x] T047 [P] [US4] Add workflow unit tests for blocked recurring generation logging in `src/booking/shared/Booking.Shared.UnitTests/Activities/BookingIntegrationsTests/AdjustRequiredResourcesForPrivateRecurringBookingAsyncShould.cs` (activity owns the recurring quota allow/block logging)
- [x] T048 [P] [US4] Add integration test for partial recurring generation at quota boundary in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/SpacesRecurringBookingQuotaShould.cs`

### Implementation for User Story 4

- [x] T049 [US4] Apply Spaces quota checks inside recurring booking adjustment activity in `src/booking/shared/Booking.Shared/Activities/PrivateRecurringBookingIntegrations.cs`
- [x] T050 [US4] Preserve daily workflow cadence while blocking over-quota recurring instances in `src/booking/shared/Booking.Shared/Workflows/BookPrivateRecurringResources.cs`
- [x] T051 [US4] Ensure recurring generation increments usage per successful current-period instance in `src/booking/shared/Booking.Shared/Services/PrivateBookingService.cs`
- [x] T052 [US4] Add structured logs for recurring allow/block decisions with recurring booking id and quota reason in `src/booking/shared/Booking.Shared/Activities/PrivateRecurringBookingIntegrations.cs`

**Checkpoint**: Recurring generation enforces the same quota contract as one-off creation.

---

## Phase 6: User Story 3 - Rebooking Within Quota (Priority: P2)

**Goal**: Updating or rebooking an existing booking record does not consume quota unless it creates a distinct new current-period booking instance.

**Independent Test**: With 80/100 current-period usage, update an existing booking schedule/resources/participants and verify success with no usage increment.

### Tests for User Story 3

- [x] T053 [P] [US3] Add update-no-increment unit tests in `src/booking/apis/Booking.Api.UnitTests/Services/PrivateBookingServiceTests/UpdatePatchAsyncShould.cs`
- [x] T054 [P] [US3] Add shared private booking update tests in `src/booking/shared/Booking.Shared.UnitTests/Services/PrivateBookingServiceTests/UpdateAsyncShould.cs`

### Implementation for User Story 3

- [x] T055 [US3] Ensure private booking update path bypasses quota increment for existing records in `src/booking/apis/Booking.Api/Services/PrivateBookingService.cs`
- [x] T056 [US3] Ensure recurring instance override update path only counts newly created records in `src/booking/shared/Booking.Shared/Services/PrivateBookingService.cs`
- [x] T057 [US3] Add structured logs for update paths that are intentionally excluded from quota usage in `src/booking/shared/Booking.Shared/Services/PrivateBookingService.cs`

**Checkpoint**: Existing booking updates remain usable independently of quota.

---

## Phase 7: User Story 5 - Admin Override for Enterprise Customers (Priority: P3)

**Goal**: Admin-managed Contact Us/Enterprise-style organizations can receive custom Spaces capacity, and missing subscription state is rejected at booking time after baseline migration/default assignment.

**Independent Test**: Set a negotiated 2000-instance capacity for an organization and verify quota service uses it; verify missing subscription state rejects booking after migration/default assignment should have completed.

### Tests for User Story 5

- [x] T058 [P] [US5] Add custom capacity unit tests in `src/organization/apis/Organization.Api.UnitTests/Services/Pricing/OrganizationSpacesSubscriptionServiceTests/SetCustomCapacityShould.cs`
- [x] T059 [P] [US5] Add missing-subscription quota tests in `src/booking/shared/Booking.Shared.UnitTests/Services/SpacesBookingQuotaServiceTests/CanCreateBookingInstancesShould.cs`
- [x] T060 [P] [US5] Add admin GraphQL mutation tests in `src/organization/apis/Organization.Api.UnitTests/GraphQL/Pricing/RootMutationTests/UpdateOrganizationSpacesSubscriptionShould.cs`

### Implementation for User Story 5

- [x] T061 [US5] Implement Contact Us/custom capacity update path in `src/organization/shared/Organization.Shared/Services/Pricing/OrganizationSpacesSubscriptionService.cs`
- [x] T062 [US5] Ensure missing Spaces subscription state is rejected at booking time after migration/default assignment should have completed in `src/booking/shared/Booking.Shared/Services/SpacesBookingQuotaService.cs`
- [x] T063 [US5] Add workaround/admin REST model for custom Spaces capacity if required by OpenAPI contract in `api-definitions/openapi/skedular/organization/workaround-v1.yaml`
- [x] T064 [US5] Add structured logs for admin override, missing subscription, and default-Free assignment in `src/organization/shared/Organization.Shared/Services/Pricing/OrganizationSpacesSubscriptionService.cs`

**Checkpoint**: Enterprise/admin capacity can be managed without weakening standard quota checks.

---

## Phase 8: Frontend Pricing, Quota Status, and Upgrade Prompts

**Purpose**: Show server-driven Spaces pricing and quota state in the Spaces app after backend contracts exist.

- [x] T065 [P] Add Relay queries for Organization Spaces pricing/subscription and Booking quota status in `src/web/apps/webapp-spaces/src/queries/spaces-pricing-quota.graphql.ts`
- [x] T066 [P] Add quota status component using `@skedular/ui` typography in `src/web/apps/webapp-spaces/src/components/organization/organizationAdmin/organization-spaces-quota-status.tsx`
- [x] T067 [P] Add upgrade/contact prompt component using backend upgrade plans in `src/web/apps/webapp-spaces/src/components/booking/spaces-quota-upgrade-prompt.tsx`
- [x] T068 Add quota status to organization subscription/admin view in `src/web/apps/webapp-spaces/src/components/organization/organizationAdmin/organization-admin-subscriptions-section.tsx`
- [x] T069 Add quota-exceeded handling to private booking create UI in `src/web/apps/webapp-spaces/src/components/booking/addBooking/new-booking-dialog.tsx` and `src/web/apps/webapp-spaces/src/components/booking/addBooking/add-private-booking.tsx`
- [x] T070 [P] Add Vitest/RTL coverage for quota status and upgrade prompt rendering in `src/web/apps/webapp-spaces/src/components/booking/__tests__/spaces-quota-upgrade-prompt.test.tsx`

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Regeneration, validation, operational checks, and cleanup.

- [x] T071 Run final GraphQL regeneration after all schema source changes and commit generated schema/Relay artifacts with `scripts/generate-graphql.sh`
- [x] T072 Run OpenAPI generation if `api-definitions/openapi/skedular/organization/workaround-v1.yaml` changed using `api-definitions/openapi/generate.sh`
- [x] T073 Run web client generation if OpenAPI changes are consumed by Spaces using `src/web/apps/webapp/scripts/generate.sh` (not required; Spaces changes use Relay, regenerated with `pnpm --dir src/web --filter webapp-spaces relay`)
- [x] T074 [P] Run backend unit validation from `specs/028-skedular-spaces-pricing/quickstart.md`
- [x] T075 [P] Run Organization and Booking integration validation from `specs/028-skedular-spaces-pricing/quickstart.md`
- [x] T076 [P] Run Spaces web lint/build validation from `specs/028-skedular-spaces-pricing/quickstart.md`
- [x] T077 Update latest validation results in `specs/028-skedular-spaces-pricing/quickstart.md`
- [x] T078 Run `graphify update .` after implementation changes to refresh `graphify-out/graph.json`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies.
- **Foundational (Phase 2)**: Depends on Setup and blocks all user stories.
- **US1 Free quota MVP (Phase 3)**: Depends on Foundational.
- **US2 Paid usage/rollover (Phase 4)**: Depends on Foundational and reuses US1 quota primitives.
- **US4 Recurring generation (Phase 5)**: Depends on Foundational and US1 quota primitives.
- **US3 Rebooking (Phase 6)**: Depends on Foundational; can run after US1 because it verifies update paths do not consume quota.
- **US5 Admin override (Phase 7)**: Depends on Foundational; can run after catalog/subscription primitives and baseline migration/default-Free assignment exist.
- **Frontend (Phase 8)**: Depends on Organization catalog/subscription and Booking quota GraphQL/Relay contract tasks in Foundation and relevant story APIs.
- **Polish (Phase 9)**: Depends on desired user stories and frontend scope.

### User Story Dependencies

- **US1 (P1)**: MVP; no dependency on other stories after Foundation.
- **US2 (P1)**: Uses the same quota service/repository as US1 and adds subscription/rollover behavior.
- **US4 (P1)**: Uses the same quota service/repository as US1 and integrates recurring generation.
- **US3 (P2)**: Independent update behavior, but easiest after US1 establishes quota usage paths.
- **US5 (P3)**: Independent admin/custom-capacity behavior after Foundation.

### Parallel Opportunities

- Setup inventory tasks T003-T004 can run in parallel.
- Foundation test/model tasks T007, T020 can run in parallel with non-conflicting model/service tasks.
- US1 tests T022-T026 can run in parallel before implementation.
- US2 tests T035-T038 can run in parallel after Foundation.
- US4 tests T046-T048 can run in parallel after Foundation.
- US3 tests T053-T054 can run in parallel after Foundation.
- US5 tests T058-T060 can run in parallel after Foundation.
- Frontend components T065-T067 and tests T070 can run in parallel once GraphQL shapes are settled.
- Polish validations T074-T076 can run in parallel.

---

## Parallel Example: User Story 1

```bash
Task: "T022 [US1] Add quota decision unit tests in src/booking/shared/Booking.Shared.UnitTests/Services/SpacesBookingQuotaServiceTests/CanCreateBookingInstancesShould.cs"
Task: "T023 [US1] Add repository unit tests in src/booking/shared/Booking.Shared.UnitTests/Repositories/SpacesBookingUsageRepositoryTests/CountCurrentPeriodBookingInstancesShould.cs"
Task: "T024 [US1] Add Booking API unit tests in src/booking/apis/Booking.Api.UnitTests/Services/PrivateBookingServiceTests/AddAsyncShould.cs"
Task: "T025 [US1] Add Booking integration test in src/booking/domain/Booking.Domain.IntegrationTests/Repositories/SpacesBookingUsageRepositoryShould.cs"
Task: "T026 [US1] Add Booking GraphQL quota status integration test in src/booking/domain/Booking.Domain.IntegrationTests/Api/GraphQL/SpacesQuotaStatusShould.cs"
```

## Parallel Example: User Story 2

```bash
Task: "T035 [US2] Add Growth and Business quota decision tests in src/booking/shared/Booking.Shared.UnitTests/Services/SpacesBookingQuotaServiceTests/CanCreateBookingInstancesShould.cs"
Task: "T036 [US2] Add no-op rollover unit tests in src/booking/shared/Booking.Shared.UnitTests/Services/SpacesBookingUsageRolloverServiceTests/RolloverCurrentPeriodsShould.cs"
Task: "T037 [US2] Add subscription assignment tests in src/organization/apis/Organization.Api.UnitTests/Services/Pricing/OrganizationSpacesSubscriptionServiceTests/UpdateAsyncShould.cs"
Task: "T038 [US2] Add integration tests documenting rollover does not mutate usage counters in src/booking/domain/Booking.Domain.IntegrationTests/Repositories/SpacesBookingUsageRolloverShould.cs"
```

## Implementation Strategy

### MVP First (US1 Only)

1. Complete Phase 1 and Phase 2.
2. Complete Phase 3 (US1 Free quota).
3. Validate Free quota blocks the 101st current-period booking instance, excludes out-of-period instances, and returns upgrade options.
4. Stop and demo before adding paid rollover or recurring behavior.

### Incremental Delivery

1. US1 delivers Free plan enforcement.
2. US2 adds paid quotas and monthly rollover.
3. US4 adds recurring booking enforcement.
4. US3 hardens rebooking/update semantics.
5. US5 adds admin custom capacity and missing-state rejection after baseline defaulting.
6. Frontend work can land after Organization catalog/subscription and Booking quota GraphQL shapes are stable.

### Validation Commands

Use [quickstart.md](./quickstart.md) as the source of truth for validation commands and expected outcomes. Generated files must be produced through repository scripts, never hand-edited.

## Notes

- `[P]` tasks use different files or can be performed independently after their phase dependencies.
- Each user story has independent test criteria and a checkpoint.
- Integration tests must assert persistence through repositories, not raw `DbContext`.
- Structured logging is required for quota decisions, subscription changes, and failure paths.
- Current-period quota counts only booking instances whose scheduled start falls inside the current UTC billing period.
