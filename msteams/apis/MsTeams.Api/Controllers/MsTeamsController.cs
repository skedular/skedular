using Api.Shared.Services.OpenApi.UnityHub.MsTeams.V1;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MsTeams.Api.Services;

namespace MsTeams.Api.Controllers;

[ApiController]
public class MsTeamsController(IMsTeamsService msTeamsService) : MsTeamsControllerBase
{
    public override Task<IActionResult> ProcessBotMessage(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public override async Task<IActionResult> GenerateTemporaryAuthorizationCode(
        CancellationToken cancellationToken = default)
    {
        var currentUri = UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Request.PathBase);
        var authorizationRequest =
            await msTeamsService.GenerateTemporaryAuthorizationCode(currentUri, cancellationToken);

        //return Redirect(authorizationRequest);
        return Ok(new { redirectUrl = authorizationRequest });
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
        await msTeamsService.OnBoardTenant(tenant, error, error_description, admin_consent, state, cancellationToken);

        return Redirect("https://teams.microsoft.com/v2/");
    }
}
