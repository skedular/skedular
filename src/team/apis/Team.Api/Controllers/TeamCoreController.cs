using Api.Shared.Services.OpenApi.Skedular.Team.Core.V1;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Team.Core.V1.Version;

namespace Team.Api.Controllers;

[ApiController]
public class TeamCoreController(IVersionService versionService, ILogger<TeamCoreController> logger) : TeamCoreControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting {Operation}", nameof(GetVersion));

        var version = versionService.GetVersion();

        logger.LogInformation("Completed {Operation}", nameof(GetVersion));

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }
}
