using Gateway.Configurations;
using HotChocolate.Fusion.Metadata;

namespace Gateway;

public class ServiceDiscoveryConfigurationRewrite(SubgraphsConfigurations subgraphsConfigurations) : ConfigurationRewriter
{
    protected override ValueTask<HttpClientConfiguration> RewriteAsync(HttpClientConfiguration configuration, CancellationToken cancellationToken) =>
        new(configuration.SubgraphName switch
        {
            nameof(subgraphsConfigurations.Billing) => configuration with { EndpointUri = subgraphsConfigurations.Billing.Uri! },
            nameof(subgraphsConfigurations.Booking) => configuration with { EndpointUri = subgraphsConfigurations.Booking.Uri! },
            nameof(subgraphsConfigurations.Customer) => configuration with { EndpointUri = subgraphsConfigurations.Customer.Uri! },
            nameof(subgraphsConfigurations.Location) => configuration with { EndpointUri = subgraphsConfigurations.Location.Uri! },
            nameof(subgraphsConfigurations.Marketplace) => configuration with { EndpointUri = subgraphsConfigurations.Marketplace.Uri! },
            nameof(subgraphsConfigurations.MsTeams) => configuration with { EndpointUri = subgraphsConfigurations.MsTeams.Uri! },
            nameof(subgraphsConfigurations.Notification) => configuration with { EndpointUri = subgraphsConfigurations.Notification.Uri! },
            nameof(subgraphsConfigurations.Organization) => configuration with { EndpointUri = subgraphsConfigurations.Organization.Uri! },
            nameof(subgraphsConfigurations.Payment) => configuration with { EndpointUri = subgraphsConfigurations.Payment.Uri! },
            nameof(subgraphsConfigurations.Slack) => configuration with { EndpointUri = subgraphsConfigurations.Slack.Uri! },
            nameof(subgraphsConfigurations.Team) => configuration with { EndpointUri = subgraphsConfigurations.Team.Uri! },
            _ => throw new NotSupportedException()
        });
}
