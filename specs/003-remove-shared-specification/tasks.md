# Tasks: Remove Shared Specification

**Input**: Design documents from `specs/003-remove-shared-specification/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Repository unit tests and domain integration tests are required because the spec and plan explicitly require repository-owned query validation and repository-layer persistence assertions.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each migration slice.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this belongs to (`[US1]`, `[US2]`, etc.)
- Every task includes exact file paths

## Phase 1: Setup (Shared Implementation Preparation)

**Purpose**: Align the implementation docs and shared database guidance with the clarified removal and repository-contract decisions before code migration starts.

- [x] T001 Update migration execution notes in `specs/003-remove-shared-specification/quickstart.md` for full removal of `Specification<T>`, `SpecificationEvaluator`, and `IRepository.Query(ISpecification<T>)`
- [x] T002 [P] Update repository ownership guidance in `shared/Enterprise.Shared/Database/AGENTS.md` and `specs/003-remove-shared-specification/contracts/repository-ownership-contract.md` to require narrow public repository methods with concrete return types

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Put shared validation and contract scaffolding in place before any domain migration starts.

**⚠️ CRITICAL**: No user story work should begin until this phase is complete.

- [x] T003 Record the inventory of active `Specification<T>`, `ISpecification<T>`, and `IRepository.Query(ISpecification<T>)` usages with owner-repository mappings in `specs/003-remove-shared-specification/quickstart.md` and `specs/003-remove-shared-specification/data-model.md`
- [x] T004 [P] Review and update repository factory touchpoints in `organization/shared/Organization.Shared/Repositories/RepositoryFactory.cs`, `location/shared/Location.Shared/Repositories/RepositoryFactory.cs`, and `team/shared/Team.Shared/Repositories/RepositoryFactory.cs` for upcoming explicit-method expansions
- [x] T005 [P] Add repo-wide validation commands, no-`DbContext` integration-test guardrails, and explicit logging verification steps to `specs/003-remove-shared-specification/quickstart.md`

**Checkpoint**: Shared guidance, factories, and validation scaffolding are ready for domain-by-domain migration.

---

## Phase 3: User Story 1 - Migrate Organization Queries (Priority: P1) 🎯 MVP

**Goal**: Move organization-owned specification usages into explicit organization repository methods and update organization services to consume those methods.

**Independent Test**: Organization services can run against repository-owned lookup methods with the same business results for terms of use, categories, tags, offerings, analytics, and Azure tenant lookups.

### Tests for User Story 1

- [ ] T006 [P] [US1] Add organization repository unit tests in `organization/shared/Organization.Shared.UnitTests/Repositories/TermsOfUseRepositoryTests/GetActiveShould.cs`, `organization/shared/Organization.Shared.UnitTests/Repositories/IndustrySubCategoryRepositoryTests/GetByIdsShould.cs`, `organization/shared/Organization.Shared.UnitTests/Repositories/TagRepositoryTests/GetFilteredTagsShould.cs`, `organization/shared/Organization.Shared.UnitTests/Repositories/OrganizationOfferingRepositoryTests/GetFilteredOfferingsShould.cs`, `organization/shared/Organization.Shared.UnitTests/Repositories/DailyMemberCountRecordingRepositoryTests/GetByDateRangeShould.cs`, and `organization/shared/Organization.Shared.UnitTests/Repositories/AzureTenantRepositoryTests/GetActiveTenantByIdShould.cs`
- [ ] T007 [P] [US1] Add organization integration coverage in `organization/domain/Organization.Domain.IntegrationTests/Services/OrganizationTermsOfUseServiceTests/GetCurrentShould.cs`, `organization/domain/Organization.Domain.IntegrationTests/Services/OrganizationAnalyticsServiceTests/GetDailyMemberCountsShould.cs`, `organization/domain/Organization.Domain.IntegrationTests/Services/TagServiceTests/GetTagsShould.cs`, and `organization/domain/Organization.Domain.IntegrationTests/Services/PaymentServiceTests/GetOrganizationOfferingShould.cs`
- [ ] T008 [P] [US1] Add organization logging verification coverage in `organization/apis/Organization.Api.UnitTests/Services/OrganizationTermsOfUseServiceTests/LoggingShould.cs`, `organization/apis/Organization.Api.UnitTests/Services/OrganizationAnalyticsServiceTests/LoggingShould.cs`, and `organization/apis/Organization.Api.UnitTests/Services/PaymentServiceTests/LoggingShould.cs`

### Implementation for User Story 1

- [x] T009 [P] [US1] Add explicit terms and category lookup methods in `organization/shared/Organization.Shared/Repositories/TermsOfUseRepository.cs`, `organization/shared/Organization.Shared/Repositories/IndustrySubCategoryRepository.cs`, and `organization/shared/Organization.Shared/Repositories/IndustryMainCategoryRepository.cs`
- [x] T010 [P] [US1] Add explicit tag, offering, and tenant lookup methods in `organization/shared/Organization.Shared/Repositories/TagRepository.cs`, `organization/shared/Organization.Shared/Repositories/OrganizationOfferingRepository.cs`, and `organization/shared/Organization.Shared/Repositories/AzureTenantRepository.cs`
- [x] T011 [P] [US1] Add analytics-specific repository methods in `organization/shared/Organization.Shared/Repositories/DailyMemberCountRecordingRepository.cs` and supporting shared query helpers in the same repository files touched by T009-T010
- [x] T012 [US1] Replace organization service specification reads in `organization/apis/Organization.Api/Services/OrganizationService.cs`, `organization/apis/Organization.Api/Services/OrganizationTermsOfUseService.cs`, `organization/apis/Organization.Api/Services/IndustryMainCategoryService.cs`, `organization/apis/Organization.Api/Services/TagService.cs`, and `organization/apis/Organization.Api/Services/AzureTenantService.cs`
- [x] T013 [US1] Replace remaining organization consumer specification reads in `organization/apis/Organization.Api/Services/OrganizationAnalyticsService.cs`, `organization/apis/Organization.Api/Services/OrganizationOfferingService.cs`, and `organization/apis/Organization.Api/Services/PaymentService.cs`
- [ ] T014 [US1] Preserve or adjust structured logging around changed organization service branches in `organization/apis/Organization.Api/Services/OrganizationTermsOfUseService.cs`, `organization/apis/Organization.Api/Services/OrganizationAnalyticsService.cs`, and `organization/apis/Organization.Api/Services/PaymentService.cs`

**Checkpoint**: Organization-owned queries no longer depend on the shared specification abstraction and are independently testable.

---

## Phase 4: User Story 2 - Migrate Location And Team Queries (Priority: P1)

**Goal**: Move location-, resource-, analytics-, and team-owned specification usages into explicit repository methods while preserving existing include and soft-delete behaviour.

**Independent Test**: Location and team services can execute their queries through repository-owned methods and still return the same location, resource, analytics, and team results.

### Tests for User Story 2

- [ ] T015 [P] [US2] Add location and team repository unit tests in `location/shared/Location.Shared.UnitTests/Repositories/LocationRepositoryTests/GetByIdsShould.cs`, `location/shared/Location.Shared.UnitTests/Repositories/ResourceRepositoryTests/GetFilteredResourcesShould.cs`, `location/shared/Location.Shared.UnitTests/Repositories/DailyDeskCountRecordingRepositoryTests/GetByDateRangeShould.cs`, `location/shared/Location.Shared.UnitTests/Repositories/DailyRoomCountRecordingRepositoryTests/GetByDateRangeShould.cs`, and `team/shared/Team.Shared.UnitTests/Repositories/TeamRepositoryTests/GetByIdsWithRelatedDataShould.cs`
- [ ] T016 [P] [US2] Add location and team integration coverage in `location/domain/Location.Domain.IntegrationTests/Services/LocationServiceTests/GetLocationsShould.cs`, `location/domain/Location.Domain.IntegrationTests/Services/ResourceServiceTests/GetResourcesShould.cs`, `location/domain/Location.Domain.IntegrationTests/Services/LocationAnalyticsServiceTests/GetCountsShould.cs`, and `team/domain/Team.Domain.IntegrationTests/Services/TeamServiceTests/GetTeamsShould.cs`
- [ ] T017 [P] [US2] Add location and team logging verification coverage in `location/apis/Location.Api.UnitTests/Services/LocationServiceTests/LoggingShould.cs`, `location/apis/Location.Api.UnitTests/Services/ResourceServiceTests/LoggingShould.cs`, `location/apis/Location.Api.UnitTests/Services/LocationAnalyticsServiceTests/LoggingShould.cs`, and `team/apis/Team.Api.UnitTests/Services/TeamServiceTests/LoggingShould.cs`

### Implementation for User Story 2

- [ ] T018 [P] [US2] Add explicit location and resource lookup methods in `location/shared/Location.Shared/Repositories/LocationRepository.cs`, `location/shared/Location.Shared/Repositories/ResourceRepository.cs`, and `location/shared/Location.Shared/Repositories/OrganizationTagRepository.cs`
- [ ] T019 [P] [US2] Add explicit analytics and team lookup methods in `location/shared/Location.Shared/Repositories/DailyDeskCountRecordingRepository.cs`, `location/shared/Location.Shared/Repositories/DailyRoomCountRecordingRepository.cs`, and `team/shared/Team.Shared/Repositories/TeamRepository.cs`
- [x] T020 [US2] Replace location-domain specification reads in `location/apis/Location.Api/Services/LocationService.cs`, `location/apis/Location.Api/Services/ResourceService.cs`, and `location/apis/Location.Api/Services/LocationAnalyticsService.cs`
- [ ] T021 [US2] Preserve include chains and soft-delete rules in `location/shared/Location.Shared/Repositories/LocationRepository.cs`, `location/shared/Location.Shared/Repositories/ResourceRepository.cs`, and `team/shared/Team.Shared/Repositories/TeamRepository.cs`
- [ ] T022 [US2] Preserve or adjust service-boundary logging in `location/apis/Location.Api/Services/LocationService.cs`, `location/apis/Location.Api/Services/ResourceService.cs`, `location/apis/Location.Api/Services/LocationAnalyticsService.cs`, and `team/apis/Team.Api/Services/TeamService.cs`

**Checkpoint**: Location- and team-owned query logic is repository-owned and no longer expressed as shared specifications.

---

## Phase 5: User Story 3 - Migrate Booking And Marketplace Queries (Priority: P1)

**Goal**: Replace booking and marketplace consumer-side specification construction with explicit calls into the new owner repository methods.

**Independent Test**: Booking and marketplace services continue to make the same domain decisions for booking, recurring booking, subscriptions, and product/tag lookups without inline specifications.

### Tests for User Story 3

- [ ] T023 [P] [US3] Add booking and marketplace service coverage in `booking/apis/Booking.Api.UnitTests/Services/BookingServiceTests/GetTeamsShould.cs`, `booking/apis/Booking.Api.UnitTests/Services/RecurringBookingServiceTests/GetTeamsShould.cs`, `booking/apis/Booking.Api.UnitTests/Services/MarketplaceBookingSubscriptionServiceTests/GetTeamsShould.cs`, `marketplace/apis/Marketplace.Api.UnitTests/Services/ProductServiceTests/GetOrganizationTagsShould.cs`, `booking/domain/Booking.Domain.IntegrationTests/Services/BookingServiceTests/UsesExplicitRepositoryMethodsShould.cs`, and `marketplace/domain/Marketplace.Domain.IntegrationTests/Services/ProductServiceTests/UsesExplicitRepositoryMethodsShould.cs`
- [ ] T024 [P] [US3] Add booking and marketplace logging verification coverage in `booking/apis/Booking.Api.UnitTests/Services/BookingServiceTests/LoggingShould.cs`, `booking/apis/Booking.Api.UnitTests/Services/RecurringBookingServiceTests/LoggingShould.cs`, `booking/apis/Booking.Api.UnitTests/Services/MarketplaceBookingSubscriptionServiceTests/LoggingShould.cs`, and `marketplace/apis/Marketplace.Api.UnitTests/Services/ProductServiceTests/LoggingShould.cs`

### Implementation for User Story 3

- [x] T025 [P] [US3] Add any remaining owner-repository methods needed by booking and marketplace consumers in `booking/shared/Booking.Shared/Repositories/TeamRepository.cs`, `booking/shared/Booking.Shared/Repositories/LocationRepository.cs`, and `marketplace/shared/Marketplace.Shared/Repositories/OrganizationTagRepository.cs`
- [x] T026 [US3] Replace booking consumer specification reads in `booking/apis/Booking.Api/Services/BookingService.cs`, `booking/apis/Booking.Api/Services/RecurringBookingService.cs`, and `booking/apis/Booking.Api/Services/MarketplaceBookingSubscriptionService.cs`
- [x] T027 [US3] Replace marketplace consumer specification reads in `marketplace/apis/Marketplace.Api/Services/ProductService.cs`
- [ ] T028 [US3] Preserve structured logging and error-path diagnostics in `booking/apis/Booking.Api/Services/BookingService.cs`, `booking/apis/Booking.Api/Services/RecurringBookingService.cs`, `booking/apis/Booking.Api/Services/MarketplaceBookingSubscriptionService.cs`, and `marketplace/apis/Marketplace.Api/Services/ProductService.cs`

**Checkpoint**: Booking and marketplace no longer build shared specifications and consume only explicit owner repository methods.

---

## Phase 6: User Story 4 - Migrate Customer And Slack Queries (Priority: P2)

**Goal**: Replace customer and Slack specification usage with calls to explicit repository methods owned by the organization, location, and team data domains.

**Independent Test**: Customer subscribers, Slack jobs, Slack pages, and Slack handlers continue to find the same organization members, teams, locations, and workspace-related records without inline specifications.

### Tests for User Story 4

- [ ] T029 [P] [US4] Add customer and Slack coverage in `customer/processors/Customer.Processors.UnitTests/Subscribers/OrganizationSubscriberTests/HandleShould.cs`, `slack/jobs/Slack.Jobs.UnitTests/Jobs/TeamDailyUpdateJobTests/ExecuteShould.cs`, `slack/jobs/Slack.Jobs.UnitTests/Jobs/LocationDailyUpdateJobTests/ExecuteShould.cs`, `slack/jobs/Slack.Jobs.UnitTests/Jobs/UpdateWorkspaceMemberProfileStatusJobTests/ExecuteShould.cs`, `slack/apis/Slack.Api.UnitTests/Pages/TeamsPageTests/RenderShould.cs`, and `slack/apis/Slack.Api.UnitTests/Pages/LocationsPageTests/RenderShould.cs`
- [ ] T030 [P] [US4] Add customer and Slack logging verification coverage in `customer/processors/Customer.Processors.UnitTests/Subscribers/OrganizationSubscriberTests/LoggingShould.cs`, `slack/jobs/Slack.Jobs.UnitTests/Jobs/TeamDailyUpdateJobTests/LoggingShould.cs`, `slack/jobs/Slack.Jobs.UnitTests/Jobs/LocationDailyUpdateJobTests/LoggingShould.cs`, `slack/jobs/Slack.Jobs.UnitTests/Jobs/UpdateWorkspaceMemberProfileStatusJobTests/LoggingShould.cs`, `slack/apis/Slack.Api.UnitTests/Pages/TeamsPageTests/LoggingShould.cs`, and `slack/apis/Slack.Api.UnitTests/Pages/LocationsPageTests/LoggingShould.cs`

### Implementation for User Story 4

- [x] T031 [P] [US4] Add any remaining owner-repository methods needed by customer and Slack consumers in `customer/shared/Customer.Shared/Repositories/OrganizationMemberRepository.cs`, `slack/shared/Slack.Shared/Repositories/TeamRepository.cs`, `slack/shared/Slack.Shared/Repositories/LocationRepository.cs`, and `slack/shared/Slack.Shared/Repositories/WorkspaceMemberRepository.cs`
- [x] T032 [US4] Replace customer processor specification reads in `customer/processors/Customer.Processors/Subscribers/OrganizationSubscriber.cs`
- [x] T033 [US4] Replace Slack job specification reads in `slack/jobs/Slack.Jobs/Jobs/TeamDailyUpdateJob.cs`, `slack/jobs/Slack.Jobs/Jobs/LocationDailyUpdateJob.cs`, and `slack/jobs/Slack.Jobs/Jobs/UpdateWorkspaceMemberProfileStatusJob.cs`
- [x] T034 [US4] Replace Slack page and handler specification reads in `slack/apis/Slack.Api/Pages/TeamsPage.cs`, `slack/apis/Slack.Api/Pages/LocationsPage.cs`, `slack/apis/Slack.Api/Handlers/ActionHandlers/Team/EditTeamButtonHandler.cs`, and `slack/apis/Slack.Api/Handlers/ActionHandlers/Location/EditLocationButtonHandler.cs`
- [ ] T035 [US4] Preserve structured logging and failure diagnostics in `customer/processors/Customer.Processors/Subscribers/OrganizationSubscriber.cs`, `slack/jobs/Slack.Jobs/Jobs/UpdateWorkspaceMemberProfileStatusJob.cs`, `slack/apis/Slack.Api/Pages/TeamsPage.cs`, and `slack/apis/Slack.Api/Pages/LocationsPage.cs`

**Checkpoint**: Customer and Slack integration flows no longer depend on the shared specification abstraction.

---

## Phase 7: User Story 5 - Remove Shared Specification Infrastructure (Priority: P2)

**Goal**: Remove the shared specification abstraction and repository entry points from Enterprise.Shared after all domain consumers have been migrated.

**Independent Test**: The solution builds and affected tests pass with no production code path left for `Specification<T>`, `SpecificationEvaluator`, or `IRepository.Query(ISpecification<T>)`.

### Tests for User Story 5

- [x] T036 [P] [US5] Replace shared specification abstraction tests with removal and repository-contract coverage in `shared/Enterprise.Shared.UnitTests/Database/RepositoryContracts/RepositoryInterfacesShould.cs`
- [ ] T037 [P] [US5] Add public repository interface contract tests in `organization/shared/Organization.Shared.UnitTests/Repositories/RepositoryInterfaceTests/OrganizationRepositoriesShould.cs`, `location/shared/Location.Shared.UnitTests/Repositories/RepositoryInterfaceTests/LocationRepositoriesShould.cs`, and `team/shared/Team.Shared.UnitTests/Repositories/RepositoryInterfaceTests/TeamRepositoriesShould.cs`
- [ ] T038 [P] [US5] Add shared cleanup logging verification coverage in `organization/apis/Organization.Api.UnitTests/Services/OrganizationTermsOfUseServiceTests/LoggingShould.cs`, `location/apis/Location.Api.UnitTests/Services/LocationAnalyticsServiceTests/LoggingShould.cs`, `booking/apis/Booking.Api.UnitTests/Services/BookingServiceTests/LoggingShould.cs`, and `customer/processors/Customer.Processors.UnitTests/Subscribers/OrganizationSubscriberTests/LoggingShould.cs` to verify unchanged structured logs after shared abstraction removal

### Implementation for User Story 5

- [x] T039 [US5] Remove the shared specification contract from `shared/Enterprise.Shared/Database/IRepository.cs`, `shared/Enterprise.Shared/Database/Specification.cs`, and `shared/Enterprise.Shared/Database/SpecificationEvaluator.cs`
- [x] T040 [US5] Remove `Query(ISpecification<T>)` support from `shared/Enterprise.Shared/Database/PostgreSql/RepositoryBase.cs` and `shared/Enterprise.Shared/Database/SqlServer/RepositoryBase.cs`, updating any affected repository inheritance signatures in the active domain repository interfaces and implementations touched in earlier phases
- [ ] T041 [US5] Update shared database guidance and repository ownership documentation in `shared/Enterprise.Shared/Database/AGENTS.md`, `specs/003-remove-shared-specification/quickstart.md`, and `specs/003-remove-shared-specification/contracts/repository-ownership-contract.md`

**Checkpoint**: Enterprise.Shared no longer exposes the shared specification abstraction or query entry point.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Final cleanup, validation, and documentation alignment across all migrated stories.

- [ ] T042 [P] Run a final repository-wide cleanup for remaining specification references in `shared/Enterprise.Shared/Database/`, `organization/`, `location/`, `team/`, `booking/`, `marketplace/`, `slack/`, and `customer/`, documenting any discovered follow-up items in `specs/003-remove-shared-specification/quickstart.md`
- [ ] T043 Execute the full validation matrix captured in `specs/003-remove-shared-specification/quickstart.md` against the affected build and test projects
- [ ] T044 [P] Verify structured start/completion, branch-decision, failure-path, and correlation-context logs for the migrated workflows using `specs/003-remove-shared-specification/quickstart.md` and the logging verification test files introduced in T008, T017, T024, T030, and T038

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; starts immediately.
- **Foundational (Phase 2)**: Depends on Setup; blocks user story work until shared guidance, factory review, and validation scaffolding are ready.
- **User Story 1 (Phase 3)**: Starts after Foundational; delivers the MVP slice.
- **User Story 2 (Phase 4)**: Starts after Foundational; can run in parallel with User Story 1.
- **User Story 3 (Phase 5)**: Depends on User Story 1 and User Story 2 because booking and marketplace consumers need the owner repository methods first.
- **User Story 4 (Phase 6)**: Depends on User Story 1 and User Story 2 because customer and Slack consumers rely on organization, location, and team owner methods.
- **User Story 5 (Phase 7)**: Depends on User Stories 1-4 because shared cleanup must happen only after all active consumers are migrated.
- **Polish (Phase 8)**: Depends on all desired user stories being complete.

### User Story Dependencies

- **US1 (Organization)**: Independent after Foundational; recommended MVP slice.
- **US2 (Location and Team)**: Independent after Foundational, but creates owner methods needed by later consumer stories.
- **US3 (Booking and Marketplace)**: Depends on repository method additions from US1 and US2.
- **US4 (Customer and Slack)**: Depends on repository method additions from US1 and US2.
- **US5 (Shared Cleanup)**: Depends on all prior stories.

### Within Each User Story

- Tests should be written before or alongside implementation and must fail until the new repository-owned behaviour exists.
- Repository method additions come before consumer rewiring.
- Consumer rewiring comes before logging adjustments and final story validation.

### Parallel Opportunities

- `T002`, `T004`, and `T005` can run in parallel during Setup/Foundational work.
- `T006`, `T007`, and `T008` can run in parallel for US1.
- `T009`, `T010`, and `T011` can run in parallel for US1 because they touch different repository files.
- `T015`, `T016`, and `T017` can run in parallel for US2.
- `T018` and `T019` can run in parallel for US2.
- `T023`, `T024`, and `T025` can run in parallel for US3.
- `T029`, `T030`, and `T031` can run in parallel for US4.
- `T036`, `T037`, and `T038` can run in parallel for US5.

---

## Parallel Example: User Story 1

```bash
# Repository-first test work
Task: "Add organization repository unit tests in organization/shared/Organization.Shared.UnitTests/Repositories/TermsOfUseRepositoryTests/GetActiveShould.cs, organization/shared/Organization.Shared.UnitTests/Repositories/IndustrySubCategoryRepositoryTests/GetByIdsShould.cs, organization/shared/Organization.Shared.UnitTests/Repositories/TagRepositoryTests/GetFilteredTagsShould.cs, organization/shared/Organization.Shared.UnitTests/Repositories/OrganizationOfferingRepositoryTests/GetFilteredOfferingsShould.cs, organization/shared/Organization.Shared.UnitTests/Repositories/DailyMemberCountRecordingRepositoryTests/GetByDateRangeShould.cs, and organization/shared/Organization.Shared.UnitTests/Repositories/AzureTenantRepositoryTests/GetActiveTenantByIdShould.cs"
Task: "Add organization integration coverage in organization/domain/Organization.Domain.IntegrationTests/Services/OrganizationTermsOfUseServiceTests/GetCurrentShould.cs, organization/domain/Organization.Domain.IntegrationTests/Services/OrganizationAnalyticsServiceTests/GetDailyMemberCountsShould.cs, organization/domain/Organization.Domain.IntegrationTests/Services/TagServiceTests/GetTagsShould.cs, and organization/domain/Organization.Domain.IntegrationTests/Services/PaymentServiceTests/GetOrganizationOfferingShould.cs"

# Repository implementation work
Task: "Add explicit terms and category lookup methods in organization/shared/Organization.Shared/Repositories/TermsOfUseRepository.cs, organization/shared/Organization.Shared/Repositories/IndustrySubCategoryRepository.cs, and organization/shared/Organization.Shared/Repositories/IndustryMainCategoryRepository.cs"
Task: "Add explicit tag, offering, and tenant lookup methods in organization/shared/Organization.Shared/Repositories/TagRepository.cs, organization/shared/Organization.Shared/Repositories/OrganizationOfferingRepository.cs, and organization/shared/Organization.Shared/Repositories/AzureTenantRepository.cs"
```

---

## Parallel Example: User Story 2

```bash
# Repository tests in parallel
Task: "Add location and team repository unit tests in location/shared/Location.Shared.UnitTests/Repositories/LocationRepositoryTests/GetByIdsShould.cs, location/shared/Location.Shared.UnitTests/Repositories/ResourceRepositoryTests/GetFilteredResourcesShould.cs, location/shared/Location.Shared.UnitTests/Repositories/DailyDeskCountRecordingRepositoryTests/GetByDateRangeShould.cs, location/shared/Location.Shared.UnitTests/Repositories/DailyRoomCountRecordingRepositoryTests/GetByDateRangeShould.cs, and team/shared/Team.Shared.UnitTests/Repositories/TeamRepositoryTests/GetByIdsWithRelatedDataShould.cs"
Task: "Add location and team integration coverage in location/domain/Location.Domain.IntegrationTests/Services/LocationServiceTests/GetLocationsShould.cs, location/domain/Location.Domain.IntegrationTests/Services/ResourceServiceTests/GetResourcesShould.cs, location/domain/Location.Domain.IntegrationTests/Services/LocationAnalyticsServiceTests/GetCountsShould.cs, and team/domain/Team.Domain.IntegrationTests/Services/TeamServiceTests/GetTeamsShould.cs"

# Owner repository changes in parallel
Task: "Add explicit location and resource lookup methods in location/shared/Location.Shared/Repositories/LocationRepository.cs, location/shared/Location.Shared/Repositories/ResourceRepository.cs, and location/shared/Location.Shared/Repositories/OrganizationTagRepository.cs"
Task: "Add explicit analytics and team lookup methods in location/shared/Location.Shared/Repositories/DailyDeskCountRecordingRepository.cs, location/shared/Location.Shared/Repositories/DailyRoomCountRecordingRepository.cs, and team/shared/Team.Shared/Repositories/TeamRepository.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate organization repository methods and organization service workflows independently
5. Use the validated organization slice as the migration template for later domains

### Incremental Delivery

1. Complete Setup + Foundational
2. Deliver US1 (organization)
3. Deliver US2 (location and team ownership)
4. Deliver US3 (booking and marketplace consumers)
5. Deliver US4 (customer and Slack consumers)
6. Deliver US5 (shared abstraction removal)
7. Finish with repo-wide cleanup and validation

### Parallel Team Strategy

With multiple developers:

1. One developer handles shared guidance and foundational validation tasks.
2. One developer owns organization repositories and services.
3. One developer owns location/team repositories and analytics.
4. After owner repositories land, separate developers can migrate booking/marketplace and customer/Slack consumers in parallel.
5. Shared cleanup and final validation happen last.

---

## Notes

- `[P]` tasks touch different files and can run without waiting on each other once their prerequisites are met.
- User stories are intentionally sequenced so data-owner repository methods land before consumer rewiring.
- Repository interfaces must remain explicit and concrete; no new `IQueryable` or replacement generic query abstraction should be introduced.
- Integration tests must assert through repository methods rather than `DbContext`.
