using Api.Shared.Services.OpenApi.Skedular.Slack.Callback.V1;
using Microsoft.AspNetCore.Mvc;
using Slack.Api.Services;

namespace Slack.Api.Controllers;

[ApiController]
public class SlackCallbackController(IWorkspaceService workspaceService) : SlackCallbackControllerBase
{
    public override async Task<IActionResult> Callback(string code, string? state, CancellationToken cancellationToken = default) =>
        Redirect(await workspaceService.InstallAsync(code, state, cancellationToken));
}
