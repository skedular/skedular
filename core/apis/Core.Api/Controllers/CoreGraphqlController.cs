using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.Core.Graphql.V1;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

[ApiController]
public class CoreGraphqlController(CoreConfiguration coreConfiguration, ITopicEventSender topicEventSender) : CoreGraphqlControllerBase
{
    public override async Task<IActionResult> RaiseGraphqlChange(
        string topicName,
        string id,
        // ReSharper disable once InconsistentNaming
        string x_API_Key,
        CancellationToken cancellationToken = default)
    {
        if (x_API_Key != coreConfiguration.ApiKey)
        {
            return Unauthorized();
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);

        return Ok();
    }
}
