using Api.Shared.Services.OpenApi.Skedular.MsTeams.Workaround.V1;
using Microsoft.AspNetCore.Mvc;
using MsTeams.Api.Services;

namespace MsTeams.Api.Controllers;

[ApiController]
public class MsTeamsWorkaroundController(IWorkaroundService workaroundService) : MsTeamsWorkaroundControllerBase
{
    public override async Task<IActionResult> ReSyncAllMsTeams(CancellationToken cancellationToken = default)
    {
        await workaroundService.ReSyncAllMsTeamsAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> ReSyncMsTeams(string tenantId, CancellationToken cancellationToken = default)
    {
        await workaroundService.ReSyncMsTeamsAsync(tenantId, cancellationToken);

        return Ok();
    }
}
