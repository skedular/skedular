using Api.Shared.Services.OpenApi.UnityHub.Slack.V1;
using Microsoft.AspNetCore.Mvc;
using Slack.Api.Services;

namespace Slack.Api.Controllers;

[ApiController]
public class SlackController(IWorkspaceService workspaceService) : SlackControllerBase
{
    public override async Task<IActionResult> Callback(
        string code,
        string? state,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(state);

        return Redirect((await workspaceService.InstallAsync(code, state, cancellationToken)).ToString());
    }
}
