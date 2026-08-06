using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Services.Authorization;
using Organization.Api.Services.Pricing;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Services.Cache;
using Organization.Shared.Workflows;
using PricingCatalogSubscriptionPlanCodeAlias = Organization.Shared.Models.PricingCatalog.PricingCatalogSubscriptionPlanCode;

namespace Organization.Api.UnitTests.Services.Pricing.OrganizationSpacesSubscriptionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateAsyncShould
{
    [Fact]
    public void Reject_Invalid_PlanCode()
    {
        var exception = Record.Exception(() =>
        {
            var planCode = PricingCatalogSubscriptionPlanCodeAlias.NotSet;
            if (planCode is not (PricingCatalogSubscriptionPlanCodeAlias.Free or
                PricingCatalogSubscriptionPlanCodeAlias.Growth or
                PricingCatalogSubscriptionPlanCodeAlias.Business or
                PricingCatalogSubscriptionPlanCodeAlias.ContactUs))
            {
                throw new ArgumentOutOfRangeException(nameof(planCode));
            }
        });
        exception.ShouldBeOfType<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Reject_Custom_Capacity_For_Non_ContactUs_Plan()
    {
        var exception = Record.Exception(() =>
        {
            int? capacity = 2000;
            var planCode = PricingCatalogSubscriptionPlanCodeAlias.Free;
            if (capacity.HasValue && planCode != PricingCatalogSubscriptionPlanCodeAlias.ContactUs)
            {
                throw new ArgumentException("Custom Spaces capacity is only supported for Contact Us subscriptions.");
            }
        });
        exception.ShouldBeOfType<ArgumentException>();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Initialize_Trial_Once_On_First_Spaces_Enablement(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        TimeProvider timeProvider,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        OrganizationSpacesSubscriptionService sut,
        IDbContextTransaction transaction,
        string organizationId,
        string customerId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = organizationId,
            Type = OrganizationTypeConstants.Marketplace,
            OrganizationOfferings = [],
        };
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                organizationId, null, A<CancellationToken>._))
            .Returns(organization);
        A.CallTo(() => cachedCustomerService.GetIdAsync(A<CancellationToken>._)).Returns(customerId);
        A.CallTo(() => authorizationService.CanModifyAsync(organization, customerId, A<CancellationToken>._)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, A<CancellationToken>._))
            .Returns(transaction);

        await sut.UpdateAsync(
            organizationId,
            PricingCatalogSubscriptionPlanCodeAlias.Free,
            null,
            cancellationToken);

        organization.SpacesTrialStartedAt.ShouldBe(now);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Existing_Trial_Anchor_Across_Plan_Changes(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        TimeProvider timeProvider,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        OrganizationSpacesSubscriptionService sut,
        IDbContextTransaction transaction,
        OrganizationOffering existingOffering,
        OrganizationStripePaymentMethod paymentMethod,
        string organizationId,
        string customerId,
        DateTimeOffset trialStartedAt,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        existingOffering.Code = OfferingCode.SpacesFreeTierV1;
        existingOffering.DeletedAt = null;
        var organization = new Shared.Database.Entities.Organization
        {
            Id = organizationId,
            Type = OrganizationTypeConstants.Marketplace,
            SpacesTrialStartedAt = trialStartedAt,
            OrganizationOfferings = [existingOffering],
            OrganizationStripePaymentMethods = [paymentMethod],
        };
        existingOffering.Organization = organization;
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                organizationId, null, A<CancellationToken>._))
            .Returns(organization);
        A.CallTo(() => cachedCustomerService.GetIdAsync(A<CancellationToken>._)).Returns(customerId);
        A.CallTo(() => authorizationService.CanModifyAsync(organization, customerId, A<CancellationToken>._)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, A<CancellationToken>._))
            .Returns(transaction);

        await sut.UpdateAsync(
            organizationId,
            PricingCatalogSubscriptionPlanCodeAlias.Growth,
            null,
            cancellationToken);

        organization.SpacesTrialStartedAt.ShouldBe(trialStartedAt);
        existingOffering.SpacesBillingStartsAt.ShouldBe(now.GetOfferingPeriodStart().GetOfferingPeriodEnd());
        existingOffering.Start.ShouldBe(now);
        existingOffering.End.ShouldBe(existingOffering.SpacesBillingStartsAt!.Value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_Creation_Date_Fallback_When_Existing_Free_Organization_Upgrades(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        TimeProvider timeProvider,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        OrganizationSpacesSubscriptionService sut,
        IDbContextTransaction transaction,
        OrganizationOffering existingOffering,
        OrganizationStripePaymentMethod paymentMethod,
        string organizationId,
        string customerId,
        DateTimeOffset organizationCreatedAt,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        existingOffering.Code = OfferingCode.SpacesFreeTierV1;
        existingOffering.DeletedAt = null;
        var organization = new Shared.Database.Entities.Organization
        {
            Id = organizationId,
            CreatedAt = organizationCreatedAt,
            Type = OrganizationTypeConstants.Marketplace,
            SpacesTrialStartedAt = null,
            OrganizationOfferings = [existingOffering],
            OrganizationStripePaymentMethods = [paymentMethod],
        };
        existingOffering.Organization = organization;
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                organizationId, null, A<CancellationToken>._))
            .Returns(organization);
        A.CallTo(() => cachedCustomerService.GetIdAsync(A<CancellationToken>._)).Returns(customerId);
        A.CallTo(() => authorizationService.CanModifyAsync(organization, customerId, A<CancellationToken>._)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, A<CancellationToken>._))
            .Returns(transaction);

        await sut.UpdateAsync(
            organizationId,
            PricingCatalogSubscriptionPlanCodeAlias.Growth,
            null,
            cancellationToken);

        organization.SpacesTrialStartedAt.ShouldBe(organizationCreatedAt);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Require_Payment_Method_When_Upgrading_Free_Trial(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        OrganizationSpacesSubscriptionService sut,
        OrganizationOffering existingOffering,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        existingOffering.Code = OfferingCode.SpacesFreeTierV1;
        existingOffering.DeletedAt = null;
        var organization = new Shared.Database.Entities.Organization
        {
            Id = organizationId,
            Type = OrganizationTypeConstants.Marketplace,
            OrganizationOfferings = [existingOffering],
            OrganizationStripePaymentMethods = [],
        };
        existingOffering.Organization = organization;
        A.CallTo(() => repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                organizationId, null, A<CancellationToken>._))
            .Returns(organization);
        A.CallTo(() => cachedCustomerService.GetIdAsync(A<CancellationToken>._)).Returns(customerId);
        A.CallTo(() => authorizationService.CanModifyAsync(organization, customerId, A<CancellationToken>._)).Returns(true);

        await Should.ThrowAsync<PaymentMethodRequired>(() => sut.UpdateAsync(
            organizationId,
            PricingCatalogSubscriptionPlanCodeAlias.Growth,
            null,
            cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Keep_Paid_Update_Idempotent_Without_Starting_Another_Bridge(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        TimeProvider timeProvider,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        ITemporalOutboxService temporalOutboxService,
        OrganizationSpacesSubscriptionService sut,
        IDbContextTransaction transaction,
        OrganizationOffering existingOffering,
        string organizationId,
        string customerId,
        DateTimeOffset trialStartedAt,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        existingOffering.Code = OfferingCode.SpacesGrowthV1;
        existingOffering.DeletedAt = null;
        existingOffering.SpacesBillingStartsAt = null;
        var organization = new Shared.Database.Entities.Organization
        {
            Id = organizationId,
            Type = OrganizationTypeConstants.Marketplace,
            SpacesTrialStartedAt = trialStartedAt,
            OrganizationOfferings = [existingOffering],
        };
        existingOffering.Organization = organization;
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                organizationId, null, A<CancellationToken>._))
            .Returns(organization);
        A.CallTo(() => cachedCustomerService.GetIdAsync(A<CancellationToken>._)).Returns(customerId);
        A.CallTo(() => authorizationService.CanModifyAsync(organization, customerId, A<CancellationToken>._)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, A<CancellationToken>._))
            .Returns(transaction);

        var result = await sut.UpdateAsync(
            organizationId,
            PricingCatalogSubscriptionPlanCodeAlias.Growth,
            null,
            cancellationToken);

        result.Id.ShouldBe(existingOffering.Id);
        organization.SpacesTrialStartedAt.ShouldBe(trialStartedAt);
        existingOffering.SpacesBillingStartsAt.ShouldBeNull();
        A.CallTo(() => temporalOutboxService.StartWorkflowScheduleRenewOrganizationOffering(
                A<ScheduleRenewOrganizationOfferingInput>._,
                A<IUnitOfWork>._))
            .MustNotHaveHappened();
    }
}
