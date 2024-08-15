using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using MsTeams.Shared.Configurations;

namespace MsTeams.Shared;

public static class GrpcClients
{
    public static void ConfigureCustomer(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<CustomerConfiguration>().GrpcUrl;

    public static void ConfigureLocation(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<LocationConfiguration>().GrpcUrl;

    public static void ConfigureOrganization(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<OrganizationConfiguration>().GrpcUrl;
}
