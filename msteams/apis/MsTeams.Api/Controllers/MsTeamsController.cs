using Api.Shared.Services.OpenApi.Skedular.MsTeams.V1;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.MsTeams.V1.Version;

namespace MsTeams.Api.Controllers;

[ApiController]
public class MsTeamsController(IVersionService versionService) : MsTeamsControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }
}
