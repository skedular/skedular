using Api.Shared.Clients.OpenApi.Skedular.Booking.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Booking.Graphql.V1;
using Api.Shared.Clients.OpenApi.Skedular.Booking.StripeWebhook.V1;
using Api.Shared.Clients.OpenApi.Skedular.Booking.XeroWebhook.V1;
using Api.Shared.Clients.OpenApi.Skedular.BookingWorkaround.V1;
using Api.Shared.Clients.OpenApi.Skedular.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Customer.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Customer.Graphql.V1;
using Api.Shared.Clients.OpenApi.Skedular.Customer.Stripe.V1;
using Api.Shared.Clients.OpenApi.Skedular.Customer.Workaround.V1;
using Api.Shared.Clients.OpenApi.Skedular.Gateway.V1;
using Api.Shared.Clients.OpenApi.Skedular.Location.V1;
using Api.Shared.Clients.OpenApi.Skedular.Marketplace.V1;
using Api.Shared.Clients.OpenApi.Skedular.MsTeams.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.MsTeams.Graphql.V1;
using Api.Shared.Clients.OpenApi.Skedular.MsTeams.Workaround.V1;
using Api.Shared.Clients.OpenApi.Skedular.Organization.V1;
using Api.Shared.Clients.OpenApi.Skedular.Slack.Callback.V1;
using Api.Shared.Clients.OpenApi.Skedular.Slack.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Slack.Graphql.V1;
using Api.Shared.Clients.OpenApi.Skedular.Slack.Workaround.V1;
using Api.Shared.Clients.OpenApi.Skedular.Team.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Team.Graphql.V1;
using Api.Shared.Clients.OpenApi.Skedular.Team.Workaround.V1;
using Aspire.Hosting.Testing;
using Booking.Shared.Database;
using Core.Shared.Database;
using Customer.Shared.Database;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.PostgreSql;
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
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<BookingDbContext>(
                configuration,
                environment,
                bookingDbConnectionString,
                true,
                "bookingdb")
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<CoreDbContext>(
                configuration,
                environment,
                coreDbConnectionString,
                true,
                "coredb")
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<CustomerDbContext>(
                configuration,
                environment,
                customerDbConnectionString,
                true,
                "customerdb")
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<LocationDbContext>(
                configuration,
                environment,
                locationDbConnectionString,
                true,
                "locationdb")
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<MarketplaceDbContext>(
                configuration,
                environment, marketplaceDbConnectionString,
                true,
                "marketplacedb")
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<MsTeamsDbContext>(
                configuration,
                environment, msteamsDbConnectionString,
                true,
                "msteeamsdb")
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<OrganizationDbContext>(
                configuration,
                environment,
                organizationDbConnectionString,
                true,
                "organizationdb")
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<SlackDbContext>(
                configuration,
                environment,
                slackDbConnectionString,
                true,
                "slackdb")
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<TeamDbContext>(
                configuration,
                environment,
                teamDbConnectionString,
                true,
                "teamdb")
            .AddSingleton<IGatewayClient>(_ => new GatewayClient(gatewayClient))
            .AddSingleton<IBookingCoreClient>(_ => new BookingCoreClient(bookingApiClient))
            .AddSingleton<IBookingGraphqlClient>(_ => new BookingGraphqlClient(bookingApiClient))
            .AddSingleton<IBookingStripeWebhookClient>(_ => new BookingStripeWebhookClient(bookingApiClient))
            .AddSingleton<IBookingWorkaroundClient>(_ => new BookingWorkaroundClient(bookingApiClient))
            .AddSingleton<IBookingXeroWebhookClient>(_ => new BookingXeroWebhookClient(bookingApiClient))
            .AddSingleton<ICustomerCoreClient>(_ => new CustomerCoreClient(customerApiClient))
            .AddSingleton<ICustomerGraphqlClient>(_ => new CustomerGraphqlClient(customerApiClient))
            .AddSingleton<ICustomerStripeClient>(_ => new CustomerStripeClient(customerApiClient))
            .AddSingleton<ICustomerWorkaroundClient>(_ => new CustomerWorkaroundClient(customerApiClient))
            .AddSingleton<ICoreClient>(_ => new CoreClient(coreApiClient))
            .AddSingleton<ILocationClient>(_ => new LocationClient(locationApiClient))
            .AddSingleton<IMarketplaceClient>(_ => new MarketplaceClient(marketplaceApiClient))
            .AddSingleton<IMsTeamsCoreClient>(_ => new MsTeamsCoreClient(msteamsApiClient))
            .AddSingleton<IMsTeamsGraphqlClient>(_ => new MsTeamsGraphqlClient(msteamsApiClient))
            .AddSingleton<IMsTeamsWorkaroundClient>(_ => new MsTeamsWorkaroundClient(msteamsApiClient))
            .AddSingleton<IOrganizationClient>(_ => new OrganizationClient(organizationApiClient))
            .AddSingleton<ISlackCoreClient>(_ => new SlackCoreClient(slackApiClient))
            .AddSingleton<ISlackGraphqlClient>(_ => new SlackGraphqlClient(slackApiClient))
            .AddSingleton<ISlackCallbackClient>(_ => new SlackCallbackClient(slackApiClient))
            .AddSingleton<ISlackWorkaroundClient>(_ => new SlackWorkaroundClient(slackApiClient))
            .AddSingleton<ITeamCoreClient>(_ => new TeamCoreClient(teamApiClient))
            .AddSingleton<ITeamGraphqlClient>(_ => new TeamGraphqlClient(teamApiClient))
            .AddSingleton<ITeamWorkaroundClient>(_ => new TeamWorkaroundClient(teamApiClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = gatewayClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
