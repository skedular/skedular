using Api.Shared.Clients.OpenApi.Skedular.Organization.V1;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Aspire.Hosting.Testing;
using Enterprise.Shared;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Kafka;
using Flurl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Organization.Shared;
using Organization.Shared.Database;
using Projects;
using Testing.Shared.IntegrationTests;
using Testing.Shared.IntegrationTests.Aspire;

namespace Organization.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        DomainAppHostEnvironmentVariables.SetFakeDependencies(true);
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Organization_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(TestContext.Current.CancellationToken).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var organizationDbConnectionString = distributedApp.GetConnectionStringAsync("organizationdb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(organizationDbConnectionString);

        var pgadmin = distributedApp.GetEndpoint("pgadmin");
        var kafkaUi = distributedApp.GetEndpoint("kafka-ui");

        Console.WriteLine($"pgadmin: {pgadmin}");
        Console.WriteLine($"kafkaUi: {kafkaUi}");

        var organizationApiGrpcEndpoint = distributedApp.GetEndpoint("organizationapi", "Grpc").ToString();
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationApiGrpcEndpoint);

        var organizationApiClient = distributedApp.CreateHttpClient("organizationapi");
        ArgumentNullException.ThrowIfNull(organizationApiClient.BaseAddress);

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);
        var organizationApiGrpcChannel = GrpcChannelFactory.Create(organizationApiGrpcEndpoint);

        services.TryAddSingleton(TimeProvider.System);
        services
            .AddKeyedSingleton("organization-api-grpc-channel", organizationApiGrpcChannel)
            .AddTestingSharedIntegrationTests()
            .AddSingleton(_ => new OrganizationService.OrganizationServiceClient(organizationApiGrpcChannel));

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<OrganizationDbContext>(
                configuration,
                environment,
                organizationDbConnectionString,
                true,
                "organizationdb")
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddXeroServices(configuration)
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddSingleton<IOrganizationClient>(_ => new OrganizationClient(organizationApiClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = organizationApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
