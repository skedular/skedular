using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.Organization.Workaround.V1;
using Microsoft.AspNetCore.Mvc;
using Organization.Api.Services;
using OfferingCode = Api.Shared.Services.OpenApi.Skedular.Organization.Workaround.V1.OfferingCode;

namespace Organization.Api.Controllers;

[ApiController]
public class OrganizationWorkaroundController(
    OrganizationConfiguration organizationConfiguration,
    IWorkaroundService workaroundService,
    IOrganizationOfferingService organizationOfferingService)
    : OrganizationWorkaroundControllerBase
{
    public override async Task<IActionResult> Republish(string customDomain, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishOrganizationAsync(customDomain, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllOrganizationsAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RegenerateAllOfferings(CancellationToken cancellationToken = default)
    {
        await organizationOfferingService.RegenerateAllOfferingsAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RerunAllOfferingsWorkflows(CancellationToken cancellationToken = default)
    {
        await organizationOfferingService.RerunAllOfferingsWorkflowsAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> SetEnterpriseOffering(
        // ReSharper disable once InconsistentNaming
        string x_API_Key,
        SetEnterpriseOfferingRequest body,
        CancellationToken cancellationToken = default)
    {
        if (x_API_Key != organizationConfiguration.ApiKey)
        {
            return Unauthorized();
        }

        await organizationOfferingService.SetEnterpriseOfferingAsync(
            body.OrganizationId,
            body.CustomDomain,
            body.OfferingCode switch
            {
                OfferingCode.EARLY_BIRD_V1 => global::Api.Shared.Services.Offering.OfferingCode.EarlyBirdV1,
                OfferingCode.FREE_TIER_V1 => global::Api.Shared.Services.Offering.OfferingCode.FreeTierV1,
                OfferingCode.PAY_AS_YOU_GO_V1 => global::Api.Shared.Services.Offering.OfferingCode.PayAsYouGoV1,
                OfferingCode.ENTERPRISE_CUSTOM_V1 => global::Api.Shared.Services.Offering.OfferingCode.EnterpriseCustomV1,
                OfferingCode.SPACES_FREE_TIER_V1 => global::Api.Shared.Services.Offering.OfferingCode.SpacesFreeTierV1,
                OfferingCode.SPACES_GROWTH_V1 => global::Api.Shared.Services.Offering.OfferingCode.SpacesGrowthV1,
                OfferingCode.SPACES_BUSINESS_V1 => global::Api.Shared.Services.Offering.OfferingCode.SpacesBusinessV1,
                OfferingCode.SPACES_CONTACT_US_V1 => global::Api.Shared.Services.Offering.OfferingCode.SpacesContactUsV1,
                OfferingCode.HOST_STANDARD_V1 => global::Api.Shared.Services.Offering.OfferingCode.HostStandardV1,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(body.OfferingCode),
                    body.OfferingCode,
                    $"Unexpected value for {nameof(body.OfferingCode)}: {body.OfferingCode}. Update enum mapping or caller input."),
            },
            body.FixedPrice,
            body.Currency switch
            {
                Currency.Nzd => global::Api.Shared.Services.Models.Currency.Nzd,
                Currency.Usd => global::Api.Shared.Services.Models.Currency.Usd,
                _ => throw new ArgumentOutOfRangeException(nameof(body.Currency), body.Currency,
                    $"Unexpected value for {nameof(body.Currency)}: {body.Currency}. Update enum mapping or caller input."),
            },
            body.PurchasedUserCapacity,
            body.PurchasedLocationCapacity,
            body.PurchasedTeamCapacity,
            body.MonthlyBookingInstanceQuota,
            body.DiscountPercentage,
            cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> ReSyncAllAzureTenants(CancellationToken cancellationToken = default)
    {
        await workaroundService.ReSyncAllAzureTenantsAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> ReSyncAzureTenant(string tenantId, CancellationToken cancellationToken = default)
    {
        await workaroundService.ReSyncAzureTenantAsync(tenantId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RegenerateAllDailyAnalytics(CancellationToken cancellationToken = default)
    {
        await workaroundService.RegenerateAllDailyAnalyticsAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RegenerateDailyAnalytics(string customDomain, CancellationToken cancellationToken = default)
    {
        await workaroundService.RegenerateDailyAnalyticsAsync(customDomain, cancellationToken);

        return Ok();
    }
}
