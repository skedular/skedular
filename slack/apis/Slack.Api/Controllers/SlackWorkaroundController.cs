using Api.Shared.Services.OpenApi.Skedular.Slack.Workaround.V1;
using Microsoft.AspNetCore.Mvc;
using Slack.Api.Services;

namespace Slack.Api.Controllers;

[ApiController]
public class SlackWorkaroundController(IWorkaroundService workaroundService) : SlackWorkaroundControllerBase
{
    public override async Task<IActionResult> ReSyncAllSlackWorkspaces(CancellationToken cancellationToken = default)
    {
        await workaroundService.ReSyncAllSlackWorkspacesAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> ReSyncSlackWorkspace(string workspaceId, CancellationToken cancellationToken = default)
    {
        await workaroundService.ReSyncSlackWorkspaceAsync(workspaceId, cancellationToken);

        return Ok();
    }
}
