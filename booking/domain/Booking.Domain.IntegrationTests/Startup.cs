using Api.Shared.Clients.OpenApi.Skedular.Booking.V1;
using Api.Shared.Services;
using Aspire.Hosting.Testing;
using Booking.Shared;
using Booking.Shared.Database;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Flurl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Projects;
using Testing.Shared.IntegrationTests.Aspire;

namespace Booking.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Booking_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(CancellationToken.None).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var bookingDbConnectionString = distributedApp.GetConnectionStringAsync("bookingdb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(bookingDbConnectionString);

        var pgadmin = distributedApp.GetEndpoint("pgadmin");
        var kafkaUi = distributedApp.GetEndpoint("kafka-ui");

        Console.WriteLine($"pgadmin: {pgadmin}");
        Console.WriteLine($"kafkaUi: {kafkaUi}");

        var bookingApiClient = distributedApp.CreateHttpClient("bookingapi");

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);

        services.TryAddSingleton(TimeProvider.System);

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledDbContextFactoryWithConnectionString<BookingDbContext>(
                configuration,
                environment,
                bookingDbConnectionString,
                true,
                "bookingdb")
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddSingleton<IBookingClient>(_ => new BookingClient(bookingApiClient));

        services
            .AddSkedularGraphQlBookingClientV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = bookingApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
