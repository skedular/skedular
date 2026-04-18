using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.MsTeams.Graphql.V1;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Mvc;

namespace MsTeams.Api.Controllers;

[ApiController]
public class MsTeamsGraphqlController(MsTeamsConfiguration msTeamsConfiguration, ITopicEventSender topicEventSender) : MsTeamsGraphqlControllerBase
{
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
}
