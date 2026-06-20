using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models.PricingCatalog;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;
using Organization.Shared.Services.Pricing;
using OrganizationEntity = Organization.Shared.Database.Entities.Organization;
using OrganizationOfferingPlanDto = Organization.Shared.Models.PricingCatalog.OrganizationOfferingPlan;
using OrganizationOfferingEntity = Organization.Shared.Database.Entities.OrganizationOffering;

namespace Organization.Api.Services.Pricing;

public interface IOrganizationTeamsSubscriptionService
{
    Task<OrganizationOfferingPlanDto?> GetAsync(string organizationId, CancellationToken cancellationToken);

    Task<OrganizationOfferingPlanDto> UpdateAsync(
        string organizationId,
        PricingCatalogSubscriptionPlanCode planCode,
        int? purchasedUserCapacity,
        int? purchasedLocationCapacity,
        int? purchasedTeamCapacity,
        CancellationToken cancellationToken);
}

public class OrganizationTeamsSubscriptionService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IOrganizationOfferingCompatibilityService compatibilityService,
    IRandomHelper randomHelper,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IGraphQlMapper graphQlMapper,
    TimeProvider timeProvider,
    ILogger<OrganizationTeamsSubscriptionService> logger,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICachedCustomerService cachedCustomerService) : IOrganizationTeamsSubscriptionService
{
    public async Task<OrganizationOfferingPlanDto?> GetAsync(string organizationId, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return await compatibilityService.GetTeamsOfferingPlanAsync(
            organizationId,
            organization.OrganizationOfferings.SingleOrDefault(),
            now,
            cancellationToken);
    }

    public async Task<OrganizationOfferingPlanDto> UpdateAsync(
        string organizationId,
        PricingCatalogSubscriptionPlanCode planCode,
        int? purchasedUserCapacity,
        int? purchasedLocationCapacity,
        int? purchasedTeamCapacity,
        CancellationToken cancellationToken)
    {
        ValidateRequest(planCode, purchasedUserCapacity, purchasedLocationCapacity, purchasedTeamCapacity);

        var now = timeProvider.GetUtcNow();
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        // Get customer ID for authorization check
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);

        // Check authorization - only owners/admins can modify offering
        if (!await organizationAuthorizationService.CanModifyAsync(organization, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        // Check payment method is attached for paid plans
        if (planCode == PricingCatalogSubscriptionPlanCode.PayAsYouGo && organization.OrganizationStripePaymentMethods.Count == 0)
        {
            throw new PaymentMethodRequired();
        }

        var currentOffering = organization.OrganizationOfferings.SingleOrDefault();

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var offering = currentOffering is null
            ? CreateOffering(organization, planCode, purchasedUserCapacity, now)
            : UpdateCurrentOffering(currentOffering, planCode, purchasedUserCapacity, purchasedLocationCapacity, purchasedTeamCapacity, now);

        if (currentOffering is null)
        {
            repositoryFactory.OrganizationOfferingRepository.Add(offering);
        }
        else
        {
            repositoryFactory.OrganizationOfferingRepository.Update(offering);
        }

        organizationOutboxPublisher.PublishOrganizations(
            [graphQlMapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            repositoryFactory.UnitOfWork);

        logger.LogInformation(
            "{EventName}: saved Teams offering for {OrganizationId} as {SubscriptionPlanCode}",
            currentOffering is null ? PricingLogEvents.OrganizationOfferingPlanCreated : PricingLogEvents.OrganizationOfferingPlanUpdated,
            organizationId,
            planCode);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return MapTo(offering);
    }

    private OrganizationOfferingEntity CreateOffering(
        OrganizationEntity organization,
        PricingCatalogSubscriptionPlanCode planCode,
        int? purchasedUserCapacity,
        DateTimeOffset now)
    {
        var organizationOffering = new OrganizationOfferingEntity
        {
            Id = randomHelper.Generate(),
            Organization = organization,
            Start = now,
            End = now.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
            AutoRenew = true
        };
        organizationOffering.ApplyOfferingTemplate(planCode.ToTeamsOfferingCode());
        organizationOffering.PurchasedUserCapacity = purchasedUserCapacity ?? organizationOffering.PurchasedUserCapacity;
        return organizationOffering;
    }

    private static void ValidateRequest(
        PricingCatalogSubscriptionPlanCode planCode,
        int? purchasedUserCapacity,
        int? purchasedLocationCapacity,
        int? purchasedTeamCapacity)
    {
        if (planCode is PricingCatalogSubscriptionPlanCode.LegacyEarlyBird or PricingCatalogSubscriptionPlanCode.NotSet
            or PricingCatalogSubscriptionPlanCode.EnterpriseCapacity)
        {
            throw new ArgumentOutOfRangeException(
                nameof(planCode),
                planCode,
                "This Teams plan cannot be selected through the customer-facing offering update.");
        }

        if (purchasedUserCapacity.HasValue)
        {
            throw new ArgumentException(
                "Purchased user capacity is only supported for Skedular-admin Enterprise offering updates.",
                nameof(purchasedUserCapacity));
        }

        if (purchasedLocationCapacity.HasValue)
        {
            throw new ArgumentException(
                "Purchased location capacity is only supported for Skedular-admin Enterprise offering updates.",
                nameof(purchasedLocationCapacity));
        }

        if (purchasedTeamCapacity.HasValue)
        {
            throw new ArgumentException(
                "Purchased team capacity is only supported for Skedular-admin Enterprise offering updates.",
                nameof(purchasedTeamCapacity));
        }
    }

    private static OrganizationOfferingEntity UpdateCurrentOffering(
        OrganizationOfferingEntity src,
        PricingCatalogSubscriptionPlanCode planCode,
        int? purchasedUserCapacity,
        int? purchasedLocationCapacity,
        int? purchasedTeamCapacity,
        DateTimeOffset now)
    {
        src.Code = planCode.ToTeamsOfferingCode();
        src.ApplyRenewalTemplate(src.Code);
        src.PurchasedUserCapacity = purchasedUserCapacity ?? src.PurchasedUserCapacity;
        src.PurchasedLocationCapacity = purchasedLocationCapacity ?? src.PurchasedLocationCapacity;
        src.PurchasedTeamCapacity = purchasedTeamCapacity ?? src.PurchasedTeamCapacity;
        src.Start = now;
        src.End = now.GetOfferingPeriodStart().GetOfferingPeriodEnd();
        src.AutoRenew = true;

        return src;
    }

    private static OrganizationOfferingPlanDto MapTo(OrganizationOfferingEntity offering) =>
        new(
            offering.Id,
            offering.Organization.Id,
            PricingCatalogProductOfferingCode.Teams,
            offering.Code.ToPricingCatalogSubscriptionPlanCode(),
            offering.UnitPrice,
            offering.FixedPrice,
            offering.Currency.ToCurrency(),
            offering.PurchasedUserCapacity,
            offering.PurchasedLocationCapacity,
            offering.PurchasedTeamCapacity,
            offering.CatalogVersion ?? offering.Code.GetCurrentCatalogVersion(),
            offering.Code.IsEarlyBirdOffering() ? OrganizationOfferingPlanStatus.Legacy : OrganizationOfferingPlanStatus.Active,
            offering.Start,
            offering.End,
            offering.AutoRenew,
            offering.CreatedAt,
            offering.ModifiedAt ?? offering.CreatedAt);
}

public static class OrganizationTeamsSubscriptionServiceExtensions
{
    extension(PricingCatalogSubscriptionPlanCode planCode)
    {
        public OfferingCode ToTeamsOfferingCode() =>
            planCode switch
            {
                PricingCatalogSubscriptionPlanCode.Free => OfferingCode.FreeTierV1,
                PricingCatalogSubscriptionPlanCode.PayAsYouGo => OfferingCode.PayAsYouGoV1,
                PricingCatalogSubscriptionPlanCode.EnterpriseCapacity => OfferingCode.EnterpriseCustomV1,
                PricingCatalogSubscriptionPlanCode.LegacyEarlyBird => OfferingCode.EarlyBirdV1,
                _ => throw new ArgumentOutOfRangeException(nameof(planCode), planCode,
                    $"Unexpected value for {nameof(planCode)}: {planCode}. Update enum mapping or caller input.")
            };
    }
}
