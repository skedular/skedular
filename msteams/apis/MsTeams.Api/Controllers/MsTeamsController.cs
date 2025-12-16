using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.MsTeams.V1;
using Enterprise.Shared.Version;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using MsTeams.Api.Services;
using Version = Api.Shared.Services.OpenApi.Skedular.MsTeams.V1.Version;

namespace MsTeams.Api.Controllers;

[ApiController]
public class MsTeamsController(
    IVersionService versionService,
    MsTeamsConfiguration msTeamsConfiguration,
    IWorkaroundService workaroundService,
    ITopicEventSender topicEventSender)
    : MsTeamsControllerBase
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
        if (x_API_Key != msTeamsConfiguration.ApiKey)
        {
            return Unauthorized();
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);

        return Ok();
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
