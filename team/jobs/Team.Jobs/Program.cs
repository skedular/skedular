using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Temporal;
using Team.Shared;
using Team.Shared.Activities;
using Team.Shared.Configurations;
using Team.Shared.Database;
using Team.Shared.Workflows.InviteToJoinTeamExistingCustomer;
using Team.Shared.Workflows.InviteToJoinTeamNewCustomer;
using Temporalio.Extensions.Hosting;

namespace Team.Jobs;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
        ArgumentNullException.ThrowIfNull(emailConfiguration);
        services.AddSingleton(emailConfiguration);

        services
            .AddKafka(configuration)
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<TeamDbContext>(configuration, environment, "teamdb", true)
            .AddKafkaOutboxBackgroundService<TeamDbContext>()
            .AddTemporalOutboxBackgroundService<TeamDbContext>()
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices()
            .AddGrpcClients(configuration);

        services
            .AddTemporalWorker(configuration)
            .AddWorkflow<InviteToJoinTeamExistingCustomer>()
            .AddWorkflow<InviteToJoinTeamNewCustomer>()
            .AddScopedActivities<EmailIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
