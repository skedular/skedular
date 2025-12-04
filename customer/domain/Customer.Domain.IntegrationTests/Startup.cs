using Api.Shared.Clients.OpenApi.Skedular.Customer.V1;
using Api.Shared.Services;
using Aspire.Hosting.Testing;
using Customer.Shared;
using Customer.Shared.Database;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Projects;
using Testing.Shared.IntegrationTests.Aspire;

namespace Customer.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Customer_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(CancellationToken.None).Result;
        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var dbConnectionString = distributedApp.GetConnectionStringAsync("customerdb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        var pgadmin = distributedApp.GetEndpoint("pgadmin");
        var kafkaui = distributedApp.GetEndpoint("kafka-ui");

        Console.WriteLine($"pgadmin: {pgadmin}");
        Console.WriteLine($"kafkaui: {kafkaui}");

        var customerApiClient = distributedApp.CreateHttpClient("customerapi");

        ArgumentException.ThrowIfNullOrWhiteSpace(dbConnectionString);

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);

        services.TryAddSingleton(TimeProvider.System);

        services
            .AddKafkaWithConnectionString(configuration, kafkaConnectionString)
            .WithPooledDbContextFactoryWithConnectionString<CustomerDbContext>(configuration, environment, dbConnectionString, true)
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddSingleton<ICustomerClient>(_ => new CustomerClient(customerApiClient));
    }
}
