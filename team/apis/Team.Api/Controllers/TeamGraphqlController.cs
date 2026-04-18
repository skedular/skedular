using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.Team.Graphql.V1;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Mvc;

namespace Team.Api.Controllers;

[ApiController]
public class TeamGraphqlController(TeamConfiguration teamConfiguration, ITopicEventSender topicEventSender, ILogger<TeamGraphqlController> logger)
    : TeamGraphqlControllerBase
{
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
            logger.LogWarning("Unauthorised {Operation} for Topic {TopicName}", nameof(RaiseGraphqlChange), topicName);
            return Unauthorized();
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);

        logger.LogInformation("Completed {Operation} for Topic {TopicName}", nameof(RaiseGraphqlChange), topicName);

        return Ok();
    }
}
