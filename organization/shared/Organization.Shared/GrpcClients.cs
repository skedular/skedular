using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using Organization.Shared.Configurations;

namespace Organization.Shared;

public static class GrpcClients
{
    public static void ConfigureCustomer(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<CustomerConfiguration>().GrpcUrl;

    public static void ConfigureLocation(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<LocationConfiguration>().GrpcUrl;
}
