using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using MsTeams.Shared;
using MsTeams.Shared.Database;

namespace MsTeams.Jobs;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .AddDefaultServices<Program>();

        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        services
            .AddKafka(configuration)
            .WithPooledDbContextFactory<MsTeamsDbContext>(configuration, environment, "msteamsdb")
            .AddKafkaOutboxBackgroundService<MsTeamsDbContext>()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices()
            .AddGrpcServices(configuration);

        return builder.Build().UseWebApplicationDefaults();
    }
}
