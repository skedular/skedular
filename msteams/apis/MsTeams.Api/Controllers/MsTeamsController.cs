using Api.Shared.Services.OpenApi.UnityHub.MsTeams.V1;
using Microsoft.AspNetCore.Mvc;
using MsTeams.Api.Services;

namespace MsTeams.Api.Controllers;

[ApiController]
public class MsTeamsController(IAzureTenantService azureTenantService) : MsTeamsControllerBase
{
    public override async Task<IActionResult> AdminConsentUrl(CancellationToken cancellationToken = default) =>
        Redirect(await azureTenantService.GenerateAdminConsentUrlAsync(cancellationToken));

    public override async Task<IActionResult> OnBoardTenant(
        // ReSharper disable InconsistentNaming
        string tenant,
        bool admin_consent,
        string state,
        string? error,
        string? error_description,
        // ReSharper restore InconsistentNaming
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
