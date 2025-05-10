using Booking.Api.Grpc;
using Booking.Shared;
using Booking.Shared.Database;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;

namespace Booking.Api;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddServiceDefaults<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        services
            .WithPooledDbContextFactory<BookingDbContext>(configuration, environment, "BookingPostgresConnection")
            .AddGraphql(configuration, requestExecutorBuilder => { requestExecutorBuilder.AddApiTypes(); })
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddMappers()
            .AddJobs()
            .AddServices()
            .AddGrpcServices(configuration);

        var app = builder.Build().AddWebApplicationDefaults();

        app.MapGrpcService<BookingGrpcService>();

        return app;
    }
}
