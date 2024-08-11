using Api.Shared.Services.OpenApi.UnityHub.MsTeams.V1;
using Microsoft.AspNetCore.Mvc;
using MsTeams.Api.Services;

namespace MsTeams.Api.Controllers;

[ApiController]
public class MsTeamsController(
    ITenantService tenantService,
    ITenantOnboardingService tenantOnboardingService) : MsTeamsControllerBase
{
    public override Task<IActionResult> ProcessBotMessage(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public override Task<IActionResult> AdminConsent(CancellationToken cancellationToken = default)
    {
        var authorizationRequest = tenantService.GenerateAdminConsentUrl();

        return Task.FromResult((IActionResult)Redirect(authorizationRequest));
    }

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
        await tenantOnboardingService.OnBoardTenantAsync(tenant, error, error_description, cancellationToken);

        return Redirect("https://teams.microsoft.com/v2/");
    }
}
