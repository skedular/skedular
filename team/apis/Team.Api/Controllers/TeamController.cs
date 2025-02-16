using Api.Shared.Services.OpenApi.Skedular.Team.V1;
using Microsoft.AspNetCore.Mvc;
using Team.Api.Services;

namespace Team.Api.Controllers;

[ApiController]
public class TeamController(IWorkaroundService workaroundService) : TeamControllerBase
{
    public override async Task<IActionResult> Republish(string teamId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishTeamAsync(teamId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllTeamsAsync(cancellationToken);

        return Ok();
    }
}
