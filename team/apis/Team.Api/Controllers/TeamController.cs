using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.Team.V1;
using Enterprise.Shared.Version;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using Team.Api.Services;
using Version = Api.Shared.Services.OpenApi.Skedular.Team.V1.Version;

namespace Team.Api.Controllers;

[ApiController]
public class TeamController(
    IVersionService versionService,
    TeamConfiguration teamConfiguration,
    IWorkaroundService workaroundService,
    ITopicEventSender topicEventSender)
    : TeamControllerBase
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
        if (x_API_Key != teamConfiguration.ApiKey)
        {
            return Unauthorized();
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);

        return Ok();
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
