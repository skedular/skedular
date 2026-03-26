using Api.Shared.Clients.OpenApi.Skedular.Slack.V1;
using Api.Shared.Services;
using Aspire.Hosting.Testing;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Kafka;
using Flurl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Projects;
using Slack.Shared;
using Slack.Shared.Database;
using Testing.Shared.IntegrationTests.Aspire;

namespace Slack.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Slack_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(CancellationToken.None).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var slackDbConnectionString = distributedApp.GetConnectionStringAsync("slackdb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(slackDbConnectionString);

        var pgadmin = distributedApp.GetEndpoint("pgadmin");
        var kafkaUi = distributedApp.GetEndpoint("kafka-ui");

        Console.WriteLine($"pgadmin: {pgadmin}");
        Console.WriteLine($"kafkaUi: {kafkaUi}");

        var slackApiClient = distributedApp.CreateHttpClient("slackapi");

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);

        services.TryAddSingleton(TimeProvider.System);

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledDbContextFactoryWithConnectionString<SlackDbContext>(
                configuration,
                environment,
                slackDbConnectionString,
                true,
                "slackdb")
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddSingleton<ISlackClient>(_ => new SlackClient(slackApiClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = slackApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
