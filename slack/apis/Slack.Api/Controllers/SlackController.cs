using Api.Shared.Services.OpenApi.Skedular.Slack.V1;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Slack.Api.Services;
using Version = Api.Shared.Services.OpenApi.Skedular.Slack.V1.Version;

namespace Slack.Api.Controllers;

[ApiController]
public class SlackController(IVersionService versionService, IWorkspaceService workspaceService) : SlackControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<IActionResult> Callback(string code, string? state, CancellationToken cancellationToken = default) =>
        Redirect(await workspaceService.InstallAsync(code, state, cancellationToken));
}
