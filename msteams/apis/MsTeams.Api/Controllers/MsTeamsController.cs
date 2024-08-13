using Api.Shared.Services.OpenApi.UnityHub.MsTeams.V1;
using Microsoft.AspNetCore.Mvc;
using MsTeams.Api.Services;

namespace MsTeams.Api.Controllers;

[ApiController]
public class MsTeamsController(ITenantService tenantService) : MsTeamsControllerBase
{
    public override async Task<IActionResult> AdminConsentUrl(CancellationToken cancellationToken = default) =>
        Redirect(await tenantService.GenerateAdminConsentUrlAsync(cancellationToken));

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
                $"onboarding went wrong with error {error} and message {error_description}.");
        }

        var redirectUri = await tenantService.InstallAsync(tenant, state, cancellationToken);

        return Redirect(redirectUri.AbsoluteUri);
    }
}
