using Api.Shared.Services.OpenApi.Skedular.MsTeams.V1;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using MsTeams.Api.Services;
using Version = Api.Shared.Services.OpenApi.Skedular.MsTeams.V1.Version;

namespace MsTeams.Api.Controllers;

[ApiController]
public class MsTeamsController(IVersionService versionService, IWorkaroundService workaroundService) : MsTeamsControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

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
