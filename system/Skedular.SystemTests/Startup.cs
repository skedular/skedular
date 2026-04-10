using Api.Shared.Clients.OpenApi.Skedular.Booking.V1;
using Api.Shared.Clients.OpenApi.Skedular.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Customer.V1;
using Api.Shared.Clients.OpenApi.Skedular.Gateway.V1;
using Api.Shared.Clients.OpenApi.Skedular.Location.V1;
using Api.Shared.Clients.OpenApi.Skedular.Marketplace.V1;
using Api.Shared.Clients.OpenApi.Skedular.MsTeams.V1;
using Api.Shared.Clients.OpenApi.Skedular.Organization.V1;
using Api.Shared.Clients.OpenApi.Skedular.Slack.V1;
using Api.Shared.Clients.OpenApi.Skedular.Team.V1;
using Aspire.Hosting.Testing;
using Booking.Shared.Database;
using Core.Shared.Database;
using Customer.Shared.Database;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Kafka;
using Flurl;
using Location.Shared.Database;
using Marketplace.Shared.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using MsTeams.Shared.Database;
using Organization.Shared.Database;
using Projects;
using Slack.Shared.Database;
using Team.Shared.Database;
using Testing.Shared.IntegrationTests.Aspire;

namespace Skedular.SystemTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Skedular_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(TestContext.Current.CancellationToken).Result;
        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;

        var bookingDbConnectionString = distributedApp.GetConnectionStringAsync("bookingdb").Result;
        var coreDbConnectionString = distributedApp.GetConnectionStringAsync("coredb").Result;
        var customerDbConnectionString = distributedApp.GetConnectionStringAsync("customerdb").Result;
        var locationDbConnectionString = distributedApp.GetConnectionStringAsync("locationdb").Result;
        var marketplaceDbConnectionString = distributedApp.GetConnectionStringAsync("marketplacedb").Result;
        var msteamsDbConnectionString = distributedApp.GetConnectionStringAsync("msteamsdb").Result;
        var organizationDbConnectionString = distributedApp.GetConnectionStringAsync("organizationdb").Result;
        var slackDbConnectionString = distributedApp.GetConnectionStringAsync("slackdb").Result;
        var teamDbConnectionString = distributedApp.GetConnectionStringAsync("teamdb").Result;
#pragma warning restore CA2012
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(bookingDbConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(coreDbConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerDbConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(locationDbConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(marketplaceDbConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(msteamsDbConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationDbConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(slackDbConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(teamDbConnectionString);

        var pgadmin = distributedApp.GetEndpoint("pgadmin");
        var kafkaUi = distributedApp.GetEndpoint("kafka-ui");

        Console.WriteLine($"pgadmin: {pgadmin}");
        Console.WriteLine($"kafkaUi: {kafkaUi}");

        var bookingApiClient = distributedApp.CreateHttpClient("bookingapi");
        var coreApiClient = distributedApp.CreateHttpClient("coreapi");
        var customerApiClient = distributedApp.CreateHttpClient("customerapi");
        var locationApiClient = distributedApp.CreateHttpClient("locationapi");
        var marketplaceApiClient = distributedApp.CreateHttpClient("marketplaceapi");
        var msteamsApiClient = distributedApp.CreateHttpClient("msteamsapi");
        var organizationApiClient = distributedApp.CreateHttpClient("organizationapi");
        var slackApiClient = distributedApp.CreateHttpClient("slackapi");
        var teamApiClient = distributedApp.CreateHttpClient("teamapi");
        var gatewayClient = distributedApp.CreateHttpClient("gateway");

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);

        services.TryAddSingleton(TimeProvider.System);

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledDbContextFactoryWithConnectionString<BookingDbContext>(
                configuration,
                environment,
                bookingDbConnectionString,
                true,
                "bookingnpgsql")
            .WithPooledDbContextFactoryWithConnectionString<CoreDbContext>(
                configuration,
                environment,
                coreDbConnectionString,
                true,
                "coredb")
            .WithPooledDbContextFactoryWithConnectionString<CustomerDbContext>(
                configuration,
                environment,
                customerDbConnectionString,
                true,
                "customerdb")
            .WithPooledDbContextFactoryWithConnectionString<LocationDbContext>(
                configuration,
                environment,
                locationDbConnectionString,
                true,
                "locationdb")
            .WithPooledDbContextFactoryWithConnectionString<MarketplaceDbContext>(
                configuration,
                environment, marketplaceDbConnectionString,
                true,
                "marketplacedb")
            .WithPooledDbContextFactoryWithConnectionString<MsTeamsDbContext>(
                configuration,
                environment, msteamsDbConnectionString,
                true,
                "msteeamsdb")
            .WithPooledDbContextFactoryWithConnectionString<OrganizationDbContext>(
                configuration,
                environment,
                organizationDbConnectionString,
                true,
                "organizationdb")
            .WithPooledDbContextFactoryWithConnectionString<SlackDbContext>(
                configuration,
                environment,
                slackDbConnectionString,
                true,
                "slackdb")
            .WithPooledDbContextFactoryWithConnectionString<TeamDbContext>(
                configuration,
                environment,
                teamDbConnectionString,
                true,
                "teamdb")
            .AddSingleton<IBookingClient>(_ => new BookingClient(bookingApiClient))
            .AddSingleton<ICoreClient>(_ => new CoreClient(coreApiClient))
            .AddSingleton<ICustomerClient>(_ => new CustomerClient(customerApiClient))
            .AddSingleton<ILocationClient>(_ => new LocationClient(locationApiClient))
            .AddSingleton<IMarketplaceClient>(_ => new MarketplaceClient(marketplaceApiClient))
            .AddSingleton<IMsTeamsClient>(_ => new MsTeamsClient(msteamsApiClient))
            .AddSingleton<IOrganizationClient>(_ => new OrganizationClient(organizationApiClient))
            .AddSingleton<ISlackClient>(_ => new SlackClient(slackApiClient))
            .AddSingleton<ITeamClient>(_ => new TeamClient(teamApiClient))
            .AddSingleton<IGatewayClient>(_ => new GatewayClient(gatewayClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = gatewayClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
