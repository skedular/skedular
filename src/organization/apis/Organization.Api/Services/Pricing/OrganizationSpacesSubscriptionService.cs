using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Logging;
using Organization.Shared.Mappers;
using Organization.Shared.Models.PricingCatalog;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Services.Cache;
using Organization.Shared.Services.Pricing;
using Organization.Shared.Workflows;
using Offering = Api.Shared.Services.Models.Offering;
using OrganizationSpacesSubscriptionDto = Organization.Shared.Models.PricingCatalog.OrganizationSpacesSubscription;
using OrganizationOfferingEntity = Organization.Shared.Database.Entities.OrganizationOffering;

namespace Organization.Api.Services.Pricing;

public interface IOrganizationSpacesSubscriptionService
{
    Task<OrganizationSpacesSubscriptionDto?> GetAsync(string organizationId, CancellationToken cancellationToken);

    Task<OrganizationSpacesSubscriptionDto> UpdateAsync(
        string organizationId,
        PricingCatalogSubscriptionPlanCode planCode,
        int? customCapacity,
        CancellationToken cancellationToken);
}

public class OrganizationSpacesSubscriptionService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    ILogger<OrganizationSpacesSubscriptionService> logger,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICachedCustomerService cachedCustomerService,
    IGraphQlMapper graphQlMapper,
    IEntityMapper entityMapper,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    ISpacesAccessEvaluator spacesAccessEvaluator,
    ITemporalOutboxService temporalOutboxService) : IOrganizationSpacesSubscriptionService
{
    public async Task<OrganizationSpacesSubscriptionDto?> GetAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await GetOrganizationAsync(organizationId, cancellationToken) ??
                           throw new OrganizationNotFound();
        ValidateMarketplaceOrganization(organization);
        var subscription = GetSpacesSubscription(organization);

        return subscription is null ? null : MapTo(subscription);
    }

    public async Task<OrganizationSpacesSubscriptionDto> UpdateAsync(
        string organizationId,
        PricingCatalogSubscriptionPlanCode planCode,
        int? customCapacity,
        CancellationToken cancellationToken)
    {
        Validate(planCode, customCapacity);

        var organization = await GetOrganizationAsync(organizationId, cancellationToken) ??
                           throw new OrganizationNotFound();
        ValidateMarketplaceOrganization(organization);
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        if (!await organizationAuthorizationService.CanModifyAsync(organization, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var currentSubscription = GetSpacesSubscription(organization);
        if ((currentSubscription is null || currentSubscription.Code == OfferingCode.SpacesFreeTierV1) &&
            planCode != PricingCatalogSubscriptionPlanCode.Free &&
            organization.OrganizationStripePaymentMethods.Count == 0)
        {
            throw new PaymentMethodRequired();
        }

        return await UpdateAsync(organization, planCode, customCapacity, cancellationToken);
    }

    private async Task<OrganizationSpacesSubscriptionDto> UpdateAsync(
        Shared.Database.Entities.Organization organization,
        PricingCatalogSubscriptionPlanCode planCode,
        int? customCapacity,
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        var current = GetAssignableSubscription(organization);
        var now = timeProvider.GetUtcNow();
        if (!organization.SpacesTrialStartedAt.HasValue)
        {
            var useCreationDateFallback = current?.Code == OfferingCode.SpacesFreeTierV1;
            var trialStartedAt = useCreationDateFallback ? organization.CreatedAt : now;
            logger.LogInformation(
                useCreationDateFallback
                    ? SpacesTrialLogEvents.CreationDateFallbackApplied
                    : SpacesTrialLogEvents.InitializationStarted,
                "Initializing Spaces trial for organization {OrganizationId}. UsedCreationDateFallback: {UsedCreationDateFallback}",
                organization.Id,
                useCreationDateFallback);
            organization.SpacesTrialStartedAt = trialStartedAt;
        }

        var isFreeToPaidUpgrade = current?.Code == OfferingCode.SpacesFreeTierV1 &&
                                  planCode != PricingCatalogSubscriptionPlanCode.Free;
        var subscription = current is null
            ? Create(organization, planCode, customCapacity, now)
            : Update(current, planCode, customCapacity, now);

        if (isFreeToPaidUpgrade)
        {
            logger.LogInformation(
                SpacesTrialLogEvents.UpgradeRequested,
                "Spaces paid upgrade requested for organization {OrganizationId}. PlanCode: {PlanCode}",
                organization.Id,
                planCode);
            var nextBillingAt = now.GetOfferingPeriodStart().GetOfferingPeriodEnd();
            subscription.Start = now;
            subscription.End = nextBillingAt;
            subscription.SpacesBillingStartsAt = nextBillingAt;
            temporalOutboxService.StartWorkflowScheduleRenewOrganizationOffering(
                new ScheduleRenewOrganizationOfferingInput(
                    organization.Id,
                    subscription.Id,
                    nextBillingAt,
                    true),
                repositoryFactory.UnitOfWork);
            logger.LogInformation(
                SpacesTrialLogEvents.ComplimentaryBridgeStarted,
                "Spaces complimentary bridge started for organization {OrganizationId}. NextBillingAt: {NextBillingAt}",
                organization.Id,
                nextBillingAt);
            logger.LogInformation(
                SpacesTrialLogEvents.BillingBoundaryScheduled,
                "Spaces first paid billing boundary scheduled for organization {OrganizationId}. NextBillingAt: {NextBillingAt}",
                organization.Id,
                nextBillingAt);
        }
        else if (planCode == PricingCatalogSubscriptionPlanCode.Free)
        {
            subscription.SpacesBillingStartsAt = null;
        }

        if (current is null)
        {
            repositoryFactory.OrganizationOfferingRepository.Add(subscription);
        }
        else
        {
            repositoryFactory.OrganizationOfferingRepository.Update(subscription);
        }

        organization.OrganizationOfferings = [subscription];
        organizationOutboxPublisher.PublishOrganizations(
            [graphQlMapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            repositoryFactory.UnitOfWork);

        logger.LogInformation(
            "{EventName}: saved Spaces subscription for {OrganizationId} as {SubscriptionPlanCode} with custom capacity {CustomCapacity}",
            current is null ? PricingLogEvents.OrganizationSpacesSubscriptionCreated : PricingLogEvents.OrganizationSpacesSubscriptionUpdated,
            organization.Id,
            planCode,
            customCapacity);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return MapTo(subscription);
    }

    private OrganizationOfferingEntity Create(
        Shared.Database.Entities.Organization organization,
        PricingCatalogSubscriptionPlanCode planCode,
        int? customCapacity,
        DateTimeOffset now) =>
        Apply(
            new OrganizationOfferingEntity
            {
                Id = randomHelper.Generate(),
                Organization = organization,
                CreatedAt = now,
                Start = now,
                End = now.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
                AutoRenew = true
            },
            planCode,
            customCapacity,
            now);

    private static OrganizationOfferingEntity Update(
        OrganizationOfferingEntity subscription,
        PricingCatalogSubscriptionPlanCode planCode,
        int? customCapacity,
        DateTimeOffset now) =>
        Apply(subscription, planCode, customCapacity, now);

    private static OrganizationOfferingEntity Apply(
        OrganizationOfferingEntity subscription,
        PricingCatalogSubscriptionPlanCode planCode,
        int? customCapacity,
        DateTimeOffset now)
    {
        subscription.Code = planCode.ToSpacesOfferingCode();
        subscription.ApplyRenewalTemplate(subscription.Code);
        subscription.PurchasedTeamCapacity = customCapacity ?? GetDefaultLimit(planCode);
        subscription.Start = now.GetOfferingPeriodStart();
        subscription.End = subscription.Start.GetOfferingPeriodEnd();
        subscription.AutoRenew = true;
        return subscription;
    }

    private static void Validate(PricingCatalogSubscriptionPlanCode planCode, int? customCapacity)
    {
        if (planCode is not (PricingCatalogSubscriptionPlanCode.Free or PricingCatalogSubscriptionPlanCode.Growth
            or PricingCatalogSubscriptionPlanCode.Business or PricingCatalogSubscriptionPlanCode.ContactUs))
        {
            throw new ArgumentOutOfRangeException(nameof(planCode), planCode, "This Spaces plan cannot be assigned.");
        }

        if (customCapacity.HasValue && customCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(customCapacity), customCapacity, "Custom Spaces capacity must be positive.");
        }

        if (customCapacity.HasValue && planCode != PricingCatalogSubscriptionPlanCode.ContactUs)
        {
            throw new ArgumentException("Custom Spaces capacity is only supported for Contact Us subscriptions.", nameof(customCapacity));
        }
    }

    private static void ValidateMarketplaceOrganization(Shared.Database.Entities.Organization organization)
    {
        if (organization.Type != OrganizationTypeConstants.Marketplace)
        {
            throw new ArgumentException("Spaces subscriptions can only be assigned to marketplace organizations.", nameof(organization));
        }
    }

    private async Task<Shared.Database.Entities.Organization?> GetOrganizationAsync(
        string idOrCustomDomain,
        CancellationToken cancellationToken) =>
        await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(idOrCustomDomain, null, cancellationToken) ??
        await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(null, idOrCustomDomain, cancellationToken);

    private static PricingCatalogCommercialModel GetCommercialModel(PricingCatalogSubscriptionPlanCode planCode) =>
        planCode switch
        {
            PricingCatalogSubscriptionPlanCode.Free or PricingCatalogSubscriptionPlanCode.LegacyEarlyBird =>
                PricingCatalogCommercialModel.Free,
            PricingCatalogSubscriptionPlanCode.Growth or PricingCatalogSubscriptionPlanCode.Business => PricingCatalogCommercialModel.UsageBased,
            PricingCatalogSubscriptionPlanCode.ContactUs => PricingCatalogCommercialModel.CapacityBased,
            _ => throw new ArgumentOutOfRangeException(nameof(planCode), planCode,
                $"Unexpected value for {nameof(planCode)}: {planCode}. Update enum mapping or caller input.")
        };

    private static int? GetDefaultLimit(PricingCatalogSubscriptionPlanCode planCode) =>
        planCode.ToSpacesOfferingCode().GetOffering().MaxBookingInstanceCount;

    private static int? GetEffectiveLimit(OrganizationOfferingEntity subscription) =>
        subscription.Code switch
        {
            OfferingCode.EarlyBirdV1 => null,
            OfferingCode.SpacesContactUsV1 => subscription.PurchasedTeamCapacity,
            _ => subscription.PurchasedTeamCapacity ?? subscription.Code.GetOffering().MaxBookingInstanceCount
        };

    private static OrganizationOfferingEntity? GetSpacesSubscription(Shared.Database.Entities.Organization organization) =>
        organization.OrganizationOfferings.SingleOrDefault(IsSpacesSubscription);

    private static OrganizationOfferingEntity? GetAssignableSubscription(Shared.Database.Entities.Organization organization) =>
        GetSpacesSubscription(organization) ?? organization.OrganizationOfferings.SingleOrDefault(offering => !offering.DeletedAt.HasValue);

    private static bool IsSpacesSubscription(OrganizationOfferingEntity offering) =>
        !offering.DeletedAt.HasValue &&
        offering.Code is OfferingCode.EarlyBirdV1
            or OfferingCode.SpacesFreeTierV1
            or OfferingCode.SpacesGrowthV1
            or OfferingCode.SpacesBusinessV1
            or OfferingCode.SpacesContactUsV1;

    private OrganizationSpacesSubscriptionDto MapTo(OrganizationOfferingEntity subscription)
    {
        var trialStartedAt = subscription.Organization.SpacesTrialStartedAt ??
                             (subscription.Code == OfferingCode.SpacesFreeTierV1
                                 ? subscription.Organization.CreatedAt
                                 : null);
        var accessDecision = spacesAccessEvaluator.Evaluate(
            timeProvider.GetUtcNow(),
            new Offering
            {
                Id = subscription.Id,
                Code = subscription.Code,
                Start = subscription.Start,
                End = subscription.End,
                SpacesProductEnabled = true,
                SpacesTrialStartedAt = trialStartedAt,
                SpacesTrialEndsAt = trialStartedAt?.Add(SpacesAccessEvaluator.TrialDuration),
                SpacesNextBillingAt = subscription.SpacesBillingStartsAt
            },
            SpacesAccessAction.Read);

        logger.LogInformation(
            SpacesTrialLogEvents.StatusEvaluated,
            "Spaces subscription status evaluated for organization {OrganizationId}. Status: {Status}, ReasonCode: {ReasonCode}, RemainingTrialDays: {RemainingTrialDays}",
            subscription.Organization.Id,
            accessDecision.Status,
            accessDecision.ReasonCode,
            accessDecision.RemainingTrialDays);
        if (accessDecision.Status == SpacesSubscriptionStatus.TrialExpiring)
        {
            logger.LogWarning(
                SpacesTrialLogEvents.WarningObserved,
                "Spaces trial expiry warning observed for organization {OrganizationId}. RemainingTrialDays: {RemainingTrialDays}",
                subscription.Organization.Id,
                accessDecision.RemainingTrialDays);
        }
        else if (accessDecision.Status == SpacesSubscriptionStatus.TrialExpired)
        {
            logger.LogWarning(
                SpacesTrialLogEvents.ExpiryObserved,
                "Spaces trial expiry observed for organization {OrganizationId}",
                subscription.Organization.Id);
        }

        return new OrganizationSpacesSubscriptionDto
        {
            Id = subscription.Id,
            CreatedAt = subscription.CreatedAt,
            ModifiedAt = subscription.ModifiedAt,
            Organization = entityMapper.MapTo(subscription.Organization),
            PlanCode = subscription.Code.ToPricingCatalogSubscriptionPlanCode(),
            CommercialModel = GetCommercialModel(subscription.Code.ToPricingCatalogSubscriptionPlanCode()),
            CurrentPeriodStart = subscription.Start,
            CurrentPeriodEnd = subscription.End,
            UsageLimit = GetEffectiveLimit(subscription),
            RolloverDate = subscription.End,
            CustomCapacity = subscription.Code == OfferingCode.SpacesContactUsV1 ? subscription.PurchasedTeamCapacity : null,
            CatalogVersion = subscription.CatalogVersion ?? subscription.Code.GetCurrentCatalogVersion(),
            Status = subscription.Code.IsEarlyBirdOffering() ? OrganizationOfferingPlanStatus.Legacy : OrganizationOfferingPlanStatus.Active,
            SubscriptionStatus = accessDecision.Status,
            AccessReason = accessDecision.ReasonCode,
            TrialStartedAt = accessDecision.TrialStartedAt,
            TrialEndsAt = accessDecision.TrialEndsAt,
            RemainingTrialDays = accessDecision.RemainingTrialDays,
            CanUseProduct = accessDecision.CanUseProduct,
            CanAcceptBookings = accessDecision.CanAcceptBookings,
            CanProtectExistingCommitments = accessDecision.CanProtectExistingCommitments,
            UpgradeRequired = accessDecision.UpgradeRequired,
            IsComplimentaryBridge = accessDecision.IsComplimentaryBridge,
            NextBillingAt = accessDecision.NextBillingAt
        };
    }
}

public static class OrganizationSpacesSubscriptionServiceExtensions
{
    extension(PricingCatalogSubscriptionPlanCode planCode)
    {
        public OfferingCode ToSpacesOfferingCode() =>
            planCode switch
            {
                PricingCatalogSubscriptionPlanCode.Free => OfferingCode.SpacesFreeTierV1,
                PricingCatalogSubscriptionPlanCode.Growth => OfferingCode.SpacesGrowthV1,
                PricingCatalogSubscriptionPlanCode.Business => OfferingCode.SpacesBusinessV1,
                PricingCatalogSubscriptionPlanCode.ContactUs => OfferingCode.SpacesContactUsV1,
                PricingCatalogSubscriptionPlanCode.LegacyEarlyBird => OfferingCode.EarlyBirdV1,
                _ => throw new ArgumentOutOfRangeException(nameof(planCode), planCode,
                    $"Unexpected value for {nameof(planCode)}: {planCode}. Update enum mapping or caller input.")
            };
    }
}
