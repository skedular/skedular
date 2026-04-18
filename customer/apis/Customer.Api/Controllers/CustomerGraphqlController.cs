using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.Customer.Graphql.V1;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers;

[ApiController]
public class CustomerGraphqlController(
    CustomerConfiguration customerConfiguration,
    ITopicEventSender topicEventSender)
    : CustomerGraphqlControllerBase
{
    public override async Task<IActionResult> RaiseGraphqlChange(
        string topicName,
        string id,
        // ReSharper disable once InconsistentNaming
        string x_API_Key,
        CancellationToken cancellationToken = default)
    {
        if (x_API_Key != customerConfiguration.ApiKey)
        {
            return Unauthorized();
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);

        return Ok();
    }
}
