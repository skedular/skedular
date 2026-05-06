using Api.Shared.Clients.OpenApi.Skedular.Slack.Callback.V1;
using Api.Shared.Clients.OpenApi.Skedular.Slack.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Slack.Graphql.V1;
using Api.Shared.Clients.OpenApi.Skedular.Slack.Workaround.V1;
using Api.Shared.Grpc.Skedular.Slack.Core.V1;
using Api.Shared.Services;
using Aspire.Hosting.Testing;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.PostgreSql;
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
using Testing.Shared.IntegrationTests;
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
        DomainAppHostEnvironmentVariables.SetFakeDependencies(true);
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Slack_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(TestContext.Current.CancellationToken).Result;

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

        var slackApiGrpcEndpoint = distributedApp.GetEndpoint("slackapi", "Grpc").ToString();
        ArgumentException.ThrowIfNullOrWhiteSpace(slackApiGrpcEndpoint);

        var slackApiClient = distributedApp.CreateHttpClient("slackapi");
        ArgumentNullException.ThrowIfNull(slackApiClient.BaseAddress);

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);
        var slackApiGrpcChannel = GrpcChannelFactory.Create(slackApiGrpcEndpoint);

        services.TryAddSingleton(TimeProvider.System);
        services
            .AddKeyedSingleton("slack-api-grpc-channel", slackApiGrpcChannel)
            .AddTestingSharedIntegrationTests()
            .AddSingleton(_ => new SlackService.SlackServiceClient(slackApiGrpcChannel));

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<SlackDbContext>(
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
            .AddSingleton<ISlackCoreClient>(_ => new SlackCoreClient(slackApiClient))
            .AddSingleton<ISlackGraphqlClient>(_ => new SlackGraphqlClient(slackApiClient))
            .AddSingleton<ISlackCallbackClient>(_ => new SlackCallbackClient(slackApiClient))
            .AddSingleton<ISlackWorkaroundClient>(_ => new SlackWorkaroundClient(slackApiClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = slackApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
