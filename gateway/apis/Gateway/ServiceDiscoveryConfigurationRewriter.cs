using Gateway.Configurations;
using HotChocolate.Fusion.Metadata;

namespace Gateway;

public class ServiceDiscoveryConfigurationRewrite(SubgraphsConfigurations subgraphsConfigurations) : ConfigurationRewriter
{
    protected override ValueTask<HttpClientConfiguration> RewriteAsync(HttpClientConfiguration configuration, CancellationToken cancellationToken) =>
        new(configuration.SubgraphName switch
        {
            nameof(subgraphsConfigurations.Booking) => configuration with
            {
                EndpointUri = subgraphsConfigurations.Booking.Uri ?? new Uri("https+http://bookingapi/v1/graphql")
            },
            nameof(subgraphsConfigurations.Customer) => configuration with
            {
                EndpointUri = subgraphsConfigurations.Customer.Uri ?? new Uri("https+http://customerapi/v1/graphql")
            },
            nameof(subgraphsConfigurations.Location) => configuration with
            {
                EndpointUri = subgraphsConfigurations.Location.Uri ?? new Uri("https+http://locationapi/v1/graphql")
            },
            nameof(subgraphsConfigurations.Marketplace) => configuration with
            {
                EndpointUri = subgraphsConfigurations.Marketplace.Uri ?? new Uri("https+http://marketplaceapi/v1/graphql")
            },
            nameof(subgraphsConfigurations.MsTeams) => configuration with
            {
                EndpointUri = subgraphsConfigurations.MsTeams.Uri ?? new Uri("https+http://msteamsapi/v1/graphql")
            },
            nameof(subgraphsConfigurations.Notification) => configuration with
            {
                EndpointUri = subgraphsConfigurations.Notification.Uri ?? new Uri("https+http://notificationapi/v1/graphql")
            },
            nameof(subgraphsConfigurations.Organization) => configuration with
            {
                EndpointUri = subgraphsConfigurations.Organization.Uri ?? new Uri("https+http://organizationapi/v1/graphql")
            },
            nameof(subgraphsConfigurations.Slack) => configuration with
            {
                EndpointUri = subgraphsConfigurations.Slack.Uri ?? new Uri("https+http://slackapi/v1/graphql")
            },
            nameof(subgraphsConfigurations.Team) => configuration with
            {
                EndpointUri = subgraphsConfigurations.Team.Uri ?? new Uri("https+http://teamapi/v1/graphql")
            },
            nameof(subgraphsConfigurations.Core) => configuration with
            {
                EndpointUri = subgraphsConfigurations.Core.Uri ?? new Uri("https+http://coreapi/v1/graphql")
            },
            _ => throw new NotSupportedException()
        });
}
