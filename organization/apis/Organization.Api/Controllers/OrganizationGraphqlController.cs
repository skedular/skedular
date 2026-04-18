using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.Organization.Graphql.V1;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Mvc;

namespace Organization.Api.Controllers;

[ApiController]
public class OrganizationGraphqlController(OrganizationConfiguration organizationConfiguration, ITopicEventSender topicEventSender)
    : OrganizationGraphqlControllerBase
{
    public override async Task<IActionResult> RaiseGraphqlChange(
        string topicName,
        string id,
        // ReSharper disable once InconsistentNaming
        string x_API_Key,
        CancellationToken cancellationToken = default)
    {
        if (x_API_Key != organizationConfiguration.ApiKey)
        {
            return Unauthorized();
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);

        return Ok();
    }
}
