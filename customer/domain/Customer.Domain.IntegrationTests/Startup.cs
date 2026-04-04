using Api.Shared.Clients.OpenApi.Skedular.Customer.V1;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Aspire.Hosting.Testing;
using Customer.Shared;
using Customer.Shared.Database;
using Enterprise.Shared;
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
using Testing.Shared.IntegrationTests;
using Testing.Shared.IntegrationTests.Aspire;
using Constants = Enterprise.Shared.HealthCheck.Constants;

namespace Customer.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        DomainAppHostEnvironmentVariables.SetSharedInfrastructureGrpc(true);
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Customer_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(CancellationToken.None).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var customerDbConnectionString = distributedApp.GetConnectionStringAsync("customerdb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(customerDbConnectionString);

        var pgadmin = distributedApp.GetEndpoint("pgadmin");
        var kafkaUi = distributedApp.GetEndpoint("kafka-ui");

        Console.WriteLine($"pgadmin: {pgadmin}");
        Console.WriteLine($"kafkaUi: {kafkaUi}");

        var customerFakeDependenciesHttpClient = distributedApp.CreateHttpClient("customerfakedependencies");
        ArgumentNullException.ThrowIfNull(customerFakeDependenciesHttpClient.BaseAddress);

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        customerFakeDependenciesHttpClient
            .WaitForSuccessfulGetAsync(Constants.LivenessPath, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        var customerApiGrpcEndpoint = distributedApp.GetEndpoint("customerapi", "Grpc").ToString();
        ArgumentException.ThrowIfNullOrWhiteSpace(customerApiGrpcEndpoint);

        var customerApiClient = distributedApp.CreateHttpClient("customerapi");
        ArgumentNullException.ThrowIfNull(customerApiClient.BaseAddress);

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);
        var customerApiGrpcChannel = GrpcChannelFactory.Create(customerApiGrpcEndpoint);

        services.TryAddSingleton(TimeProvider.System);
        services
            .AddKeyedSingleton("customer-api-grpc-channel", customerApiGrpcChannel)
            .AddTestingSharedIntegrationTests()
            .AddSingleton(_ => new CustomerService.CustomerServiceClient(customerApiGrpcChannel));

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledDbContextFactoryWithConnectionString<CustomerDbContext>(
                configuration,
                environment,
                customerDbConnectionString,
                true,
                "customerdb")
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddSingleton<ICustomerClient>(_ => new CustomerClient(customerApiClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = customerApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
