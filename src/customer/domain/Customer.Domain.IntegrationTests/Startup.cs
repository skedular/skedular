using Api.Shared.Clients.OpenApi.Skedular.Customer.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Customer.Graphql.V1;
using Api.Shared.Clients.OpenApi.Skedular.Customer.Stripe.V1;
using Api.Shared.Clients.OpenApi.Skedular.Customer.Workaround.V1;
using Api.Shared.Grpc.Skedular.Customer.Admin.V1;
using Api.Shared.Grpc.Skedular.Customer.Core.V1;
using Api.Shared.Services;
using Api.Shared.Services.Configurations.Grpc;
using Aspire.Hosting.Testing;
using Customer.Shared;
using Customer.Shared.Database;
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
using Testing.Shared.IntegrationTests;
using Testing.Shared.IntegrationTests.Aspire;

namespace Customer.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment
        {
            EnvironmentName = Environments.Production,
        };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        DomainAppHostEnvironmentVariables.SetFakeDependencies(true);
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Customer_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(TestContext.Current.CancellationToken).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var customerDbConnectionString = distributedApp.GetConnectionStringAsync("customerdb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(customerDbConnectionString);
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
            .AddSingleton(_ => new CustomerService.CustomerServiceClient(customerApiGrpcChannel))
            .AddSingleton(_ => new CustomerAdminService.CustomerAdminServiceClient(customerApiGrpcChannel));

        var customerConfiguration = configuration.GetSection(CustomerConfiguration.Key).Get<CustomerConfiguration>() ??
                                    new CustomerConfiguration
                                    {
                                        ApiKey = "XXX",
                                    };
        services.AddSingleton(customerConfiguration);

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<CustomerDbContext>(
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
            .AddSingleton<ICustomerCoreClient>(_ => new CustomerCoreClient(customerApiClient))
            .AddSingleton<ICustomerGraphqlClient>(_ => new CustomerGraphqlClient(customerApiClient))
            .AddSingleton<ICustomerStripeClient>(_ => new CustomerStripeClient(customerApiClient))
            .AddSingleton<ICustomerWorkaroundClient>(_ => new CustomerWorkaroundClient(customerApiClient));

        services.AddTransient<TestBearerTokenHandler>();

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(
                httpClient => httpClient.BaseAddress = customerApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri(),
                clientBuilder => clientBuilder.AddHttpMessageHandler<TestBearerTokenHandler>());
    }
}
