using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.Slack.V1;
using Enterprise.Shared.Version;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using Slack.Api.Services;
using Version = Api.Shared.Services.OpenApi.Skedular.Slack.V1.Version;

namespace Slack.Api.Controllers;

[ApiController]
public class SlackController(
    IVersionService versionService,
    SlackConfiguration slackConfiguration,
    IWorkspaceService workspaceService,
    IWorkaroundService workaroundService,
    ITopicEventSender topicEventSender)
    : SlackControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<IActionResult> RaiseGraphqlChange(
        string topicName,
        string id,
        // ReSharper disable once InconsistentNaming
        string x_API_Key,
        CancellationToken cancellationToken = default)
    {
        if (x_API_Key != slackConfiguration.ApiKey)
        {
            return Unauthorized();
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> Callback(string code, string? state, CancellationToken cancellationToken = default) =>
        Redirect(await workspaceService.InstallAsync(code, state, cancellationToken));

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
