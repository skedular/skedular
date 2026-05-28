using Api.Shared.Services.OpenApi.Skedular.Team.Workaround.V1;
using Microsoft.AspNetCore.Mvc;
using Team.Api.Services;

namespace Team.Api.Controllers;

[ApiController]
public class TeamWorkaroundController(IWorkaroundService workaroundService, ILogger<TeamWorkaroundController> logger) : TeamWorkaroundControllerBase
{
    public override async Task<IActionResult> Republish(string teamId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting {Operation} for Team {TeamId}", nameof(Republish), teamId);

        await workaroundService.RepublishTeamAsync(teamId, cancellationToken);

        logger.LogInformation("Completed {Operation} for Team {TeamId}", nameof(Republish), teamId);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting {Operation}", nameof(RepublishAll));

        await workaroundService.RepublishAllTeamsAsync(cancellationToken);

        logger.LogInformation("Completed {Operation}", nameof(RepublishAll));

        return Ok();
    }
}
