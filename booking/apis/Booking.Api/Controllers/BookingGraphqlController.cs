using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.Booking.Graphql.V1;
using Enterprise.Shared.GraphQL;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
public class BookingGraphqlController(BookingConfiguration bookingConfiguration, IGraphQlTopicEventSender graphQlTopicEventSender)
    : BookingGraphqlControllerBase
{
    public override async Task<IActionResult> RaiseGraphqlChange(
        string topicName,
        string id,
        // ReSharper disable once InconsistentNaming
        string x_API_Key,
        CancellationToken cancellationToken = default)
    {
        if (x_API_Key != bookingConfiguration.ApiKey)
        {
            return Unauthorized();
        }

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(topicName, id, cancellationToken);

        return Ok();
    }
}
