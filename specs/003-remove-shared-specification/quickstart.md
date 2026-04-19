# Quickstart: Remove Shared Specification

## Goal

Replace active `Enterprise.Shared` specification-based query composition with explicit repository methods in each data-owning domain shared layer, then retire the shared specification abstraction.

## Preconditions

- Work on branch `003-remove-shared-specification`.
- Start from the current spec and plan in `specs/003-remove-shared-specification/`.
- Keep domain ownership boundaries intact: query methods belong to the repository of the domain that owns the data.
- Do not assert persistence through `DbContext` in integration tests.

## Suggested Execution Order

1. Inventory active production usages of `Specification<T>`, `ISpecification<T>`, and `IRepository.Query(ISpecification<T>)` by domain.
2. For each usage, map it to the repository interface in the data-owning domain shared layer.
3. Add explicit repository methods that preserve existing filters, includes, ordering, grouping, paging, and tracking behaviour.
4. Update consuming services, jobs, processors, subscribers, activities, and handlers to call the new methods.
5. Add or update unit tests for repository method behaviour and integration tests for affected workflows.
6. Once all active consumers are migrated, remove the shared specification/evaluator path in `Enterprise.Shared`.
7. Validate the affected solution areas with focused builds and tests.

## Active Usage Inventory

### Organization

- `organization/apis/Organization.Api/Services/OrganizationService.cs`
  - `TermsOfUse` active lookup -> `TermsOfUseRepository`
  - `IndustrySubCategory` by ids / by main category lookups -> `IndustrySubCategoryRepository`
- `organization/apis/Organization.Api/Services/OrganizationTermsOfUseService.cs`
  - active terms lookup -> `TermsOfUseRepository`
- `organization/apis/Organization.Api/Services/IndustryMainCategoryService.cs`
  - active main categories lookup -> `IndustryMainCategoryRepository`
- `organization/apis/Organization.Api/Services/TagService.cs`
  - filtered tag lookups -> `TagRepository`
- `organization/apis/Organization.Api/Services/AzureTenantService.cs`
  - active tenant by id lookup -> `AzureTenantRepository`
- `organization/apis/Organization.Api/Services/OrganizationAnalyticsService.cs`
  - member counts by date range lookup -> `DailyMemberCountRecordingRepository`
- `organization/apis/Organization.Api/Services/OrganizationOfferingService.cs`
  - active offering lookups -> `OrganizationOfferingRepository`
- `organization/apis/Organization.Api/Services/PaymentService.cs`
  - offering/payment lookups -> `OrganizationOfferingRepository`

### Location and Team Ownership

- `location/apis/Location.Api/Services/LocationService.cs`
  - `OrganizationTag` filtered lookups -> `OrganizationTagRepository`
- `location/apis/Location.Api/Services/ResourceService.cs`
  - `Resource` and `OrganizationTag` filtered lookups -> `ResourceRepository`, `OrganizationTagRepository`
- `location/apis/Location.Api/Services/LocationAnalyticsService.cs`
  - desk/room counts by date range -> `DailyDeskCountRecordingRepository`, `DailyRoomCountRecordingRepository`
- `location/shared/Location.Shared/Activities/LocationBookingDerivedState.cs`
  - resources by ids lookup -> `ResourceRepository`
- `booking/apis/Booking.Api/Services/BookingService.cs`
  - location and team by ids -> `LocationRepository`, `TeamRepository`
- `booking/apis/Booking.Api/Services/RecurringBookingService.cs`
  - team by ids -> `TeamRepository`
- `booking/apis/Booking.Api/Services/MarketplaceBookingSubscriptionService.cs`
  - team by ids -> `TeamRepository`

### Marketplace, Customer, and Slack Consumers

- `marketplace/apis/Marketplace.Api/Services/ProductService.cs`
  - `OrganizationTag` filtered lookups -> `OrganizationTagRepository`
- `customer/processors/Customer.Processors/Subscribers/OrganizationSubscriber.cs`
  - organization member by id -> `OrganizationMemberRepository`
- `slack/jobs/Slack.Jobs/Jobs/TeamDailyUpdateJob.cs`
  - team lookups -> `TeamRepository`
- `slack/jobs/Slack.Jobs/Jobs/LocationDailyUpdateJob.cs`
  - location lookups -> `LocationRepository`
- `slack/jobs/Slack.Jobs/Jobs/UpdateWorkspaceMemberProfileStatusJob.cs`
  - workspace member lookup -> owning Slack shared repository for `WorkspaceMember`
- `slack/apis/Slack.Api/Handlers/ActionHandlers/Team/EditTeamButtonHandler.cs`
  - team by id -> `TeamRepository`
- `slack/apis/Slack.Api/Handlers/ActionHandlers/Location/EditLocationButtonHandler.cs`
  - location by id -> `LocationRepository`
- `slack/apis/Slack.Api/Pages/TeamsPage.cs`
  - team by ids / team by id -> `TeamRepository`
- `slack/apis/Slack.Api/Pages/LocationsPage.cs`
  - location by ids / location by id -> `LocationRepository`

## Domain Delivery Slices

### Slice 1: Organization

- Migrate organization-owned lookups such as terms of use, industry categories, offerings, analytics, tags, and related payment queries.
- Add repository methods in `organization/shared/Organization.Shared/Repositories/`.
- Update organization API services and dependent callers.

### Slice 2: Location and Team Ownership

- Migrate location-, resource-, analytics-, and team-related lookups into the owning repositories under `location/shared/Location.Shared/Repositories/` and `team/shared/Team.Shared/Repositories/`.
- Preserve existing include chains and soft-delete handling via repository extension helpers.

### Slice 3: Booking and Marketplace Consumers

- Replace booking and marketplace consumer-side specification construction with calls to the new organization/location/team-owned repository methods.
- Keep business behaviour unchanged for recurring booking, subscription, and product workflows.

### Slice 4: Customer and Slack Consumers

- Replace customer and Slack consumer-side specification usage with repository methods owned by the relevant data domains.
- Validate jobs, pages, handlers, and subscribers that depend on these lookups.

### Slice 5: Enterprise.Shared Cleanup

- Remove `Specification<T>`, `ISpecification<T>`, `SpecificationEvaluator<TEntity>`, and `IRepository.Query(ISpecification<T>)` only after all active consumers are migrated.
- Replace abstraction-focused tests with repository-focused coverage where needed.
- Update documentation to codify the repository ownership rule.

## Validation

Run focused validation as each slice lands:

1. Build the affected domain projects.
2. Run unit tests for touched repository and service layers.
3. Run integration tests for workflows that cross persistence or integration boundaries.
4. Verify logging tests cover start/completion, branch decisions, failure paths, and correlation context for the changed workflows.
5. Confirm no active production references to shared specification classes remain before final cleanup.

## Validation Commands

- `dotnet test organization/apis/Organization.Api.UnitTests/Organization.Api.UnitTests.csproj`
- `dotnet test organization/domain/Organization.Domain.IntegrationTests/Organization.Domain.IntegrationTests.csproj`
- `dotnet test location/apis/Location.Api.UnitTests/Location.Api.UnitTests.csproj`
- `dotnet test location/domain/Location.Domain.IntegrationTests/Location.Domain.IntegrationTests.csproj`
- `dotnet test team/apis/Team.Api.UnitTests/Team.Api.UnitTests.csproj`
- `dotnet test team/domain/Team.Domain.IntegrationTests/Team.Domain.IntegrationTests.csproj`
- `dotnet test booking/apis/Booking.Api.UnitTests/Booking.Api.UnitTests.csproj`
- `dotnet test booking/domain/Booking.Domain.IntegrationTests/Booking.Domain.IntegrationTests.csproj`
- `dotnet test marketplace/apis/Marketplace.Api.UnitTests/Marketplace.Api.UnitTests.csproj`
- `dotnet test marketplace/domain/Marketplace.Domain.IntegrationTests/Marketplace.Domain.IntegrationTests.csproj`
- `dotnet test customer/processors/Customer.Processors.UnitTests/Customer.Processors.UnitTests.csproj`
- `dotnet test slack/jobs/Slack.Jobs.UnitTests/Slack.Jobs.UnitTests.csproj`
- `dotnet test slack/apis/Slack.Api.UnitTests/Slack.Api.UnitTests.csproj`

## Guardrails

- Do not assert persistence through `DbContext` in integration tests; assert through repository methods.
- Do not add new production usages of `Specification<T>`, `ISpecification<T>`, `SpecificationEvaluator<TEntity>`, or `IRepository.Query(ISpecification<T>)`.
- Keep workflow logging at service, job, processor, or activity boundaries rather than adding broad repository-level logs.

## Expected Outcome

- Query composition is explicit and discoverable in domain shared repositories.
- Cross-domain consumers use supported repository contracts instead of inline generic specifications.
- Enterprise.Shared no longer owns the active query abstraction for production code.
