# Data Model: Remove Shared Specification

## Overview

This feature does not introduce new persisted entities. It reshapes ownership of query composition by moving active specification-based reads into explicit repository methods owned by domain shared layers.

## Entities

### Shared Specification Usage

**Description**: A current production-code lookup that constructs `Specification<T>` or depends on `IRepository.Query(ISpecification<T>)`.

**Attributes**:

- `consumerPath`: Source file and calling workflow.
- `owningDomain`: Domain that owns the queried entity.
- `entityType`: Queried entity type such as `TermsOfUse`, `Tag`, `Team`, `Location`, `DailyDeskCountRecording`.
- `queryCharacteristics`: Criteria such as filters, includes, paging, ordering, grouping, and tracking mode.
- `migrationTarget`: Named repository method that replaces the shared specification usage.

**Relationships**:

- Maps to exactly one `Domain Repository Method`.
- Is executed by one or more `Domain Query Consumers`.

### Domain Repository Method

**Description**: A named repository-layer method in a domain shared repository that encapsulates a business query contract.

**Attributes**:

- `repositoryInterface`: The domain repository interface that exposes the method.
- `methodName`: Explicit business-oriented name such as `GetActiveTermsOfUseAsync` or `GetByIdsWithDailyUpdateChannelAsync`.
- `parameters`: Inputs representing business filters or paging values.
- `returnShape`: Entity, collection, nullable entity, or paginated tuple.
- `trackingMode`: Tracked, untracked, or identity-resolution behaviour.
- `includePolicy`: Related data included by default for that method.

**Relationships**:

- Owned by one `Repository Ownership Boundary`.
- Replaces one or more `Shared Specification Usage` instances.

### Domain Query Consumer

**Description**: An API service, job, processor, subscriber, workflow activity, or page handler that needs data from a domain repository.

**Attributes**:

- `consumerType`: API, job, processor, activity, page, subscriber, service.
- `callingDomain`: Domain where the consumer lives.
- `dependencyPath`: Repository factory and repository interface used by the consumer.
- `behaviouralExpectation`: Business outcome that depends on the query result.

**Relationships**:

- Calls one or more `Domain Repository Method` instances.
- Previously depended on `Shared Specification Usage` in affected workflows.

### Repository Ownership Boundary

**Description**: The rule that the query contract belongs to the repository layer of the domain that owns the queried data.

**Attributes**:

- `dataOwningDomain`: The domain that owns the underlying entity and repository.
- `allowedConsumers`: Other domains or services that may call the repository through supported interfaces.
- `ownershipRule`: Query composition stays in the owning repository, not in the consumer.

**Relationships**:

- Governs all `Domain Repository Method` definitions.
- Constrains how `Domain Query Consumer` instances access cross-domain data.

## State Transitions

### Shared Specification Usage Lifecycle

1. `Identified`: Active production usage is located.
2. `Mapped`: Owning domain repository target is chosen.
3. `Implemented`: Named repository method exists.
4. `Redirected`: Consumer calls repository method instead of building a specification.
5. `Validated`: Behaviour preserved by tests and workflow checks.
6. `Retired`: Shared specification path is no longer needed for that usage.

### Shared Infrastructure Cleanup Lifecycle

1. `In Use`: Enterprise.Shared specification/evaluator path still has active consumers.
2. `Draining`: Consumers are migrated domain by domain.
3. `Unused`: No active production consumers remain.
4. `Removed`: Shared abstraction and generic repository query path are deleted from production use.

## Current Usage Inventory

### Organization-owned query migrations

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
  - offering/payment-related offering lookups -> `OrganizationOfferingRepository`

### Location- and team-owned query migrations

- `location/apis/Location.Api/Services/LocationService.cs`
  - `OrganizationTag` by ids / filtered lookups -> `OrganizationTagRepository`
- `location/apis/Location.Api/Services/ResourceService.cs`
  - `Resource` and `OrganizationTag` filtered lookups -> `ResourceRepository`, `OrganizationTagRepository`
- `location/apis/Location.Api/Services/LocationAnalyticsService.cs`
  - desk/room counts by date range -> `DailyDeskCountRecordingRepository`, `DailyRoomCountRecordingRepository`
- `location/shared/Location.Shared/Activities/LocationBookingDerivedState.cs`
  - resources by id lookup -> `ResourceRepository`
- `booking/apis/Booking.Api/Services/BookingService.cs`
  - location and team by ids lookups -> `LocationRepository`, `TeamRepository`
- `booking/apis/Booking.Api/Services/RecurringBookingService.cs`
  - team by ids lookup -> `TeamRepository`
- `booking/apis/Booking.Api/Services/MarketplaceBookingSubscriptionService.cs`
  - team by ids lookup -> `TeamRepository`

### Marketplace, customer, and Slack query migrations

- `marketplace/apis/Marketplace.Api/Services/ProductService.cs`
  - `OrganizationTag` filtered lookups -> `OrganizationTagRepository`
- `customer/processors/Customer.Processors/Subscribers/OrganizationSubscriber.cs`
  - organization member by id lookup -> `OrganizationMemberRepository`
- `slack/jobs/Slack.Jobs/Jobs/TeamDailyUpdateJob.cs`
  - team by ids and daily update channel lookup -> `TeamRepository`
- `slack/jobs/Slack.Jobs/Jobs/LocationDailyUpdateJob.cs`
  - location by ids and daily update channel lookup -> `LocationRepository`
- `slack/jobs/Slack.Jobs/Jobs/UpdateWorkspaceMemberProfileStatusJob.cs`
  - workspace member lookup -> owning Slack shared repository for `WorkspaceMember`
- `slack/apis/Slack.Api/Handlers/ActionHandlers/Team/EditTeamButtonHandler.cs`
  - team by id lookup -> `TeamRepository`
- `slack/apis/Slack.Api/Handlers/ActionHandlers/Location/EditLocationButtonHandler.cs`
  - location by id lookup -> `LocationRepository`
- `slack/apis/Slack.Api/Pages/TeamsPage.cs`
  - team by ids / team by id lookups -> `TeamRepository`
- `slack/apis/Slack.Api/Pages/LocationsPage.cs`
  - location by ids / location by id lookups -> `LocationRepository`

## Validation Rules

- A migrated query must preserve existing business-visible behaviour.
- Cross-domain query rules must be defined in the data-owning domain repository.
- Integration tests must assert through repository methods, not `DbContext`.
- No new production code may be introduced on top of the shared specification abstraction during the migration.
