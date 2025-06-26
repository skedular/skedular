using Api.Shared.Clients.Configurations.Grpc;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Shared.Clients.Grpc;

public static class GrpcClients
{
    public static void ConfigureBooking(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<BookingConfiguration>().GrpcUrl;

    public static void ConfigureCore(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<CoreConfiguration>().GrpcUrl;

    public static void ConfigureCustomer(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<CustomerConfiguration>().GrpcUrl;

    public static void ConfigureLocation(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<LocationConfiguration>().GrpcUrl;

    public static void ConfigureMarketplace(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<MarketplaceConfiguration>().GrpcUrl;

    public static void ConfigureMsTeams(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<MsTeamsConfiguration>().GrpcUrl;

    public static void ConfigureNotification(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<NotificationConfiguration>().GrpcUrl;

    public static void ConfigureOrganization(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<OrganizationConfiguration>().GrpcUrl;

    public static void ConfigureSlack(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<SlackConfiguration>().GrpcUrl;

    public static void ConfigureTeam(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<TeamConfiguration>().GrpcUrl;
}
