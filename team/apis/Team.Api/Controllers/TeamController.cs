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
    ITopicEventSender topicEventSender,
    ILogger<TeamController> logger)
    : TeamControllerBase
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

    public override async Task<IActionResult> RaiseGraphqlChange(
        string topicName,
        string id,
        // ReSharper disable once InconsistentNaming
        string x_API_Key,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting {Operation} for Topic {TopicName}", nameof(RaiseGraphqlChange), topicName);

        if (x_API_Key != teamConfiguration.ApiKey)
        {
            logger.LogWarning("Unauthorized {Operation} for Topic {TopicName}", nameof(RaiseGraphqlChange), topicName);
            return Unauthorized();
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);

        logger.LogInformation("Completed {Operation} for Topic {TopicName}", nameof(RaiseGraphqlChange), topicName);

        return Ok();
    }

    public override async Task<IActionResult> Republish(string teamId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting {Operation} for Team {TeamId}", nameof(Republish), teamId);

        await workaroundService.RepublishTeamAsync(teamId, cancellationToken);

        logger.LogInformation("Completed {Operation} for Team {TeamId}", nameof(Republish), teamId);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting {Operation}", nameof(RepublishAll));

        await workaroundService.RepublishAllTeamsAsync(cancellationToken);

        logger.LogInformation("Completed {Operation}", nameof(RepublishAll));

        return Ok();
    }
}
