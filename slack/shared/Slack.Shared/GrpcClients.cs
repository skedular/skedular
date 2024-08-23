using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using Slack.Shared.Configurations;

namespace Slack.Shared;

public static class GrpcClients
{
    public static void ConfigureBilling(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<BillingConfiguration>().GrpcUrl;

    public static void ConfigureBooking(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<BookingConfiguration>().GrpcUrl;

    public static void ConfigureCustomer(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<CustomerConfiguration>().GrpcUrl;

    public static void ConfigureLocation(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<LocationConfiguration>().GrpcUrl;

    public static void ConfigureNotification(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<NotificationConfiguration>().GrpcUrl;

    public static void ConfigureOrganization(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<OrganizationConfiguration>().GrpcUrl;

    public static void ConfigurePayment(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<PaymentConfiguration>().GrpcUrl;

    public static void ConfigureTeam(IServiceProvider provider, GrpcClientFactoryOptions client) =>
        client.Address = provider.GetRequiredService<TeamConfiguration>().GrpcUrl;
}
