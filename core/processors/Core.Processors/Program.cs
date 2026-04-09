using Api.Shared.Clients.Events.Skedular.Organization.V1;
using Api.Shared.Services;
using Core.Processors.Subscribers;
using Core.Shared;
using Core.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.FileStorage;
using Enterprise.Shared.Kafka;

namespace Core.Processors;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        var kafkaConfiguration = services.AddKafka(configuration, "kafka");

        services
            .AddRedis(configuration, "redis")
            .AddFileStorage(configuration, CoreApiHelper.GetPublicCdnFileEndpoint(), CoreApiHelper.GetPrivateFileEndpoint())
            .WithPooledDbContextFactory<CoreDbContext>(configuration, environment, "coredb", true)
            .AddKafkaReliableEventConsumers<
                CustomerSubscriber,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Key,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                OrganizationSubscriber,
                Key,
                Event>(kafkaConfiguration)
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddMappers()
            .AddJobs()
            .AddServices();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
