using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using MsTeams.Shared.Configurations;

namespace MsTeams.Shared;

public static class GrpcClients
{
    public static void ConfigureCustomer(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<CustomerConfiguration>().GrpcUrl;
}
