using Api.Shared.Services;
using Booking.Api.Grpc;
using Booking.Api.Services;
using Booking.Shared;
using Booking.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Payment;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Sso;
using Enterprise.Shared.Security.Token;
using Enterprise.Shared.Temporal;

namespace Booking.Api;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        _ = services.AddKafka(configuration, "kafka");

        services
            .AddRedis(configuration, "redis")
            .AddSso()
            .AddSecurity()
            .WithPooledPostgreSqlDbContextFactory<BookingDbContext>(configuration, environment, "bookingdb", true)
            .AddGraphql(configuration, "booking-api", requestExecutorBuilder => { requestExecutorBuilder.AddApiTypes(); })
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddXeroServices(configuration)
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddMappers()
            .AddJobs()
            .AddServices()
            .AddGrpcServices(configuration)
            .AddSharedCrossDomainClients(configuration)
            .AddStripe(configuration)
            .AddTemporalClient(configuration, "temporal")
            .AddGrpc();

        if (DomainAppHostEnvironmentVariables.IsFakeDependenciesEnabled())
        {
            services.AddSingleton<IEnumerable<ITokenService>>(sp =>
                [new FakeTokenService(sp.GetRequiredService<IContext>())]);
        }

        var app = builder
            .Build()
            .UseWebApplicationDefaults<Program>()
            .UseSso()
            .UseSecurity();

        app.MapGrpcService<BookingGrpcService>();
        app.MapGrpcService<BookingGraphqlGrpcService>();

        return app;
    }
}
