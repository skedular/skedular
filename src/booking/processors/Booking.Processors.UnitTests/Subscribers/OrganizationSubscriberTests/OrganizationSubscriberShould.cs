using Api.Shared.Clients.Events.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Booking.Processors.Mappers;
using Booking.Processors.Subscribers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows;
using Enterprise.Shared.Kafka.Consume;
using Organization = Booking.Shared.Database.Entities.Organization;
using OrganizationBillingCycleModel = Api.Shared.Services.Models.OrganizationBillingCycle;
using OrganizationTypeModel = Api.Shared.Services.Models.OrganizationType;
using ValueMetadata = Api.Shared.Clients.Events.Skedular.Organization.V1.Metadata;
using ValueType = Api.Shared.Clients.Events.Skedular.Organization.V1.Type;

namespace Booking.Processors.UnitTests.Subscribers.OrganizationSubscriberTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OrganizationSubscriberShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Arrears_Billing_Workflow_For_New_Marketplace_Organization(
        [Frozen] IEventMapper eventMapper,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ITemporalService temporalService,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] EventContext eventContext,
        OrganizationSubscriber sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Models.Organization
        {
            Id = "org-1",
            Type = OrganizationTypeModel.Marketplace,
            BillingCycle = OrganizationBillingCycleModel.Monthly,
            EventRaisedAt = new DateTimeOffset(2026, 3, 23, 0, 0, 0, TimeSpan.Zero)
        };
        var organizationEntity = new Organization
        {
            Id = "org-1",
            CreatedAt = new DateTimeOffset(2026, 3, 23, 0, 0, 0, TimeSpan.Zero),
            Type = OrganizationTypeConstants.Private,
            BillingCycle = OrganizationBillingCycleConstants.Monthly
        };
        var updatedEntity = new Organization
        {
            Id = "org-1",
            CreatedAt = organizationEntity.CreatedAt,
            Type = OrganizationTypeConstants.Marketplace,
            BillingCycle = OrganizationBillingCycleConstants.Monthly
        };
        var @event = new Event { Metadata = new ValueMetadata { Type = ValueType.OrganizationUpserted } };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => eventMapper.MapTo(@event)).Returns(organization);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, true, true, cancellationToken))
            .Returns(Task.FromResult<Organization?>(null));
        A.CallTo(() => organizationRepository.UpsertNakedAsync("org-1", cancellationToken)).Returns(organizationEntity);
        A.CallTo(() => eventMapper.MergeToEntity(organization, organizationEntity)).Returns(updatedEntity);
        A.CallTo(() => organizationRepository.Update(updatedEntity)).Returns(updatedEntity);

        var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

        result.ShouldBe(EventSubscriberResults.Success);
        A.CallTo(() => temporalService.SignalRunOrganizationArrearsBillingWorkflowUpdateConfigurationAsync(
                "org-1",
                A<OrganizationArrearsBillingConfiguration>.That.Matches(configuration =>
                    configuration.OrganizationId == "org-1" &&
                    configuration.BillingCycle == OrganizationBillingCycleModel.Monthly),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalService.StartWorkflowRunOrganizationArrearsBillingAsync(
                A<RunOrganizationArrearsBillingInput>._,
                cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => cachedOrganizationService.UpdateByIdOrCustomDomainAsync("org-1", updatedEntity.CustomDomain, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Signal_Arrears_Billing_Workflow_When_Marketplace_Billing_Cycle_Changes(
        [Frozen] IEventMapper eventMapper,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ITemporalService temporalService,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] EventContext eventContext,
        OrganizationSubscriber sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Models.Organization
        {
            Id = "org-1",
            Type = OrganizationTypeModel.Marketplace,
            BillingCycle = OrganizationBillingCycleModel.Fortnightly,
            EventRaisedAt = new DateTimeOffset(2026, 3, 23, 0, 0, 0, TimeSpan.Zero)
        };
        var existingEntity = new Organization
        {
            Id = "org-1",
            CreatedAt = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            Type = OrganizationTypeConstants.Marketplace,
            BillingCycle = OrganizationBillingCycleConstants.Monthly,
            EventRaisedAt = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var updatedEntity = new Organization
        {
            Id = "org-1",
            CreatedAt = existingEntity.CreatedAt,
            Type = OrganizationTypeConstants.Marketplace,
            BillingCycle = OrganizationBillingCycleConstants.Fortnightly,
            EventRaisedAt = organization.EventRaisedAt
        };
        var @event = new Event { Metadata = new ValueMetadata { Type = ValueType.OrganizationUpserted } };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => eventMapper.MapTo(@event)).Returns(organization);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, true, true, cancellationToken)).Returns(existingEntity);
        A.CallTo(() => eventMapper.MergeToEntity(organization, existingEntity)).Returns(updatedEntity);
        A.CallTo(() => organizationRepository.Update(updatedEntity)).Returns(updatedEntity);

        var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

        result.ShouldBe(EventSubscriberResults.Success);
        A.CallTo(() => temporalService.SignalRunOrganizationArrearsBillingWorkflowUpdateConfigurationAsync(
                "org-1",
                A<OrganizationArrearsBillingConfiguration>.That.Matches(configuration =>
                    configuration.OrganizationId == "org-1" &&
                    configuration.BillingCycle == OrganizationBillingCycleModel.Fortnightly),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.UpdateByIdOrCustomDomainAsync("org-1", updatedEntity.CustomDomain, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Stop_Arrears_Billing_Workflow_When_Marketplace_Organization_Is_Deleted(
        [Frozen] IEventMapper eventMapper,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ITemporalService temporalService,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] EventContext eventContext,
        OrganizationSubscriber sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Models.Organization
        {
            Id = "org-1", Type = OrganizationTypeModel.Marketplace, EventRaisedAt = new DateTimeOffset(2026, 3, 23, 0, 0, 0, TimeSpan.Zero)
        };
        var existingEntity = new Organization
        {
            Id = "org-1", Type = OrganizationTypeConstants.Marketplace, EventRaisedAt = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var @event = new Event { Metadata = new ValueMetadata { Type = ValueType.OrganizationDeleted } };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => eventMapper.MapTo(@event)).Returns(organization);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, true, true, cancellationToken)).Returns(existingEntity);
        A.CallTo(() => organizationRepository.Remove(existingEntity)).Returns(existingEntity);

        var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

        result.ShouldBe(EventSubscriberResults.Success);
        A.CallTo(() => temporalService.SignalRunOrganizationArrearsBillingWorkflowStopAsync("org-1", cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveByIdOrCustomDomainAsync("org-1", null, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Stop_Arrears_Billing_Workflow_When_Organization_Stops_Being_Marketplace(
        [Frozen] IEventMapper eventMapper,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ITemporalService temporalService,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] EventContext eventContext,
        OrganizationSubscriber sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Models.Organization
        {
            Id = "org-1",
            Type = OrganizationTypeModel.Private,
            BillingCycle = OrganizationBillingCycleModel.Monthly,
            EventRaisedAt = new DateTimeOffset(2026, 3, 23, 0, 0, 0, TimeSpan.Zero)
        };
        var existingEntity = new Organization
        {
            Id = "org-1",
            CreatedAt = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            Type = OrganizationTypeConstants.Marketplace,
            BillingCycle = OrganizationBillingCycleConstants.Monthly,
            EventRaisedAt = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var updatedEntity = new Organization
        {
            Id = "org-1",
            CreatedAt = existingEntity.CreatedAt,
            Type = OrganizationTypeConstants.Private,
            BillingCycle = OrganizationBillingCycleConstants.Monthly,
            EventRaisedAt = organization.EventRaisedAt
        };
        var @event = new Event { Metadata = new ValueMetadata { Type = ValueType.OrganizationUpserted } };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => eventMapper.MapTo(@event)).Returns(organization);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, true, true, cancellationToken)).Returns(existingEntity);
        A.CallTo(() => eventMapper.MergeToEntity(organization, existingEntity)).Returns(updatedEntity);
        A.CallTo(() => organizationRepository.Update(updatedEntity)).Returns(updatedEntity);

        var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

        result.ShouldBe(EventSubscriberResults.Success);
        A.CallTo(() => temporalService.SignalRunOrganizationArrearsBillingWorkflowStopAsync("org-1", cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalService.StartWorkflowRunOrganizationArrearsBillingAsync(
                A<RunOrganizationArrearsBillingInput>._,
                cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => temporalService.SignalRunOrganizationArrearsBillingWorkflowUpdateConfigurationAsync(
                A<string>._,
                A<OrganizationArrearsBillingConfiguration>._,
                cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => cachedOrganizationService.UpdateByIdOrCustomDomainAsync("org-1", updatedEntity.CustomDomain, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
