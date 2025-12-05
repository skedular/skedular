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
                EndpointUri = string.IsNullOrWhiteSpace(subgraphsConfigurations.Booking.Uri?.AbsolutePath)
                    ? new Uri("https+http://bookingapi/v1/graphql")
                    : subgraphsConfigurations.Booking.Uri
            },
            nameof(subgraphsConfigurations.Core) => configuration with
            {
                EndpointUri = string.IsNullOrWhiteSpace(subgraphsConfigurations.Core.Uri?.AbsolutePath)
                    ? new Uri("https+http://coreapi/v1/graphql")
                    : subgraphsConfigurations.Core.Uri
            },
            nameof(subgraphsConfigurations.Customer) => configuration with
            {
                EndpointUri = string.IsNullOrWhiteSpace(subgraphsConfigurations.Customer.Uri?.AbsolutePath)
                    ? new Uri("https+http://customerapi/v1/graphql")
                    : subgraphsConfigurations.Customer.Uri
            },
            nameof(subgraphsConfigurations.Location) => configuration with
            {
                EndpointUri = string.IsNullOrWhiteSpace(subgraphsConfigurations.MsTeams.Uri?.AbsolutePath)
                    ? new Uri("https+http://locationapi/v1/graphql")
                    : subgraphsConfigurations.MsTeams.Uri
            },
            nameof(subgraphsConfigurations.Marketplace) => configuration with
            {
                EndpointUri = string.IsNullOrWhiteSpace(subgraphsConfigurations.MsTeams.Uri?.AbsolutePath)
                    ? new Uri("https+http://marketplaceapi/v1/graphql")
                    : subgraphsConfigurations.MsTeams.Uri
            },
            nameof(subgraphsConfigurations.MsTeams) => configuration with
            {
                EndpointUri = string.IsNullOrWhiteSpace(subgraphsConfigurations.MsTeams.Uri?.AbsolutePath)
                    ? new Uri("https+http://msteamsapi/v1/graphql")
                    : subgraphsConfigurations.MsTeams.Uri
            },
            nameof(subgraphsConfigurations.Organization) => configuration with
            {
                EndpointUri = string.IsNullOrWhiteSpace(subgraphsConfigurations.Organization.Uri?.AbsolutePath)
                    ? new Uri("https+http://organizationapi/v1/graphql")
                    : subgraphsConfigurations.Organization.Uri
            },
            nameof(subgraphsConfigurations.Slack) => configuration with
            {
                EndpointUri = string.IsNullOrWhiteSpace(subgraphsConfigurations.Slack.Uri?.AbsolutePath)
                    ? new Uri("https+http://slackapi/v1/graphql")
                    : subgraphsConfigurations.Slack.Uri
            },
            nameof(subgraphsConfigurations.Team) => configuration with
            {
                EndpointUri = string.IsNullOrWhiteSpace(subgraphsConfigurations.Team.Uri?.AbsolutePath)
                    ? new Uri("https+http://teamapi/v1/graphql")
                    : subgraphsConfigurations.Team.Uri
            },
            _ => throw new NotSupportedException()
        });
}
