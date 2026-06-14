using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.Organization.Workaround.V1;
using Microsoft.AspNetCore.Mvc;
using Organization.Api.Services;

namespace Organization.Api.Controllers;

[ApiController]
public class OrganizationWorkaroundController(
    OrganizationConfiguration organizationConfiguration,
    IWorkaroundService workaroundService,
    IOrganizationOfferingService organizationOfferingService)
    : OrganizationWorkaroundControllerBase
{
    public override async Task<IActionResult> Republish(string organizationId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishOrganizationAsync(organizationId, cancellationToken);

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
        string organizationId,
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
            organizationId,
            body.FixedPrice,
            body.Currency switch
            {
                Currency.Nzd => global::Api.Shared.Services.Models.Currency.Nzd,
                Currency.Usd => global::Api.Shared.Services.Models.Currency.Usd,
                _ => throw new ArgumentOutOfRangeException()
            },
            body.PurchasedUserCapacity,
            body.PurchasedLocationCapacity,
            body.PurchasedTeamCapacity,
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

    public override async Task<IActionResult> RegenerateDailyAnalytics(string organizationId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RegenerateDailyAnalyticsAsync(organizationId, cancellationToken);

        return Ok();
    }
}
