using Api.Shared.Services.OpenApi.UnityHub.Organization.V1;
using Microsoft.AspNetCore.Mvc;
using Organization.Api.Services;

namespace Organization.Api.Controllers;

[ApiController]
public class OrganizationController(
    IWorkaroundService workaroundService,
    IAzureTenantService azureTenantService) : OrganizationControllerBase
{
    public override async Task<IActionResult>
        Republish(string organizationId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishOrganizationAsync(organizationId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllOrganizationsAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult>
        AzureTenantAdminConsentUrl(CancellationToken cancellationToken = default) =>
        Redirect(await azureTenantService.GenerateAdminConsentUrlAsync(cancellationToken));

    public override async Task<IActionResult> OnboardAzureTenant(
        string tenant,
        // ReSharper disable once InconsistentNaming
        bool admin_consent,
        string state,
        string? error,
        // ReSharper disable once InconsistentNaming
        string? error_description,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            throw new InvalidOperationException(
                $"Azure tenant onboarding went wrong with error {error} and message {error_description}.");
        }

        var redirectUri = await azureTenantService.InstallAsync(tenant, state, cancellationToken);

        return Redirect(redirectUri.AbsoluteUri);
    }
}
