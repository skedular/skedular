using Api.Shared.Services.OpenApi.Skedular.Team.V1;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Team.Api.Services;
using Version = Api.Shared.Services.OpenApi.Skedular.Team.V1.Version;

namespace Team.Api.Controllers;

[ApiController]
public class TeamController(IVersionService versionService, IWorkaroundService workaroundService) : TeamControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

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
