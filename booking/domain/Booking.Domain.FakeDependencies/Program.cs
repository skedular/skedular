using Booking.Domain.FakeDependencies.Fakes;
using Enterprise.Shared;

namespace Booking.Domain.FakeDependencies;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();

        builder.Services.AddGrpc();
        builder.Services.AddFakeDependencyServices();

        var app = builder.Build().UseWebApplicationDefaults<Program>();
        app.MapGrpcService<FakeCoreGrpcService>();
        app.MapGrpcService<FakeOrganizationGrpcService>();
        app.MapGrpcService<InfrastructureTestGrpcService>();
        return app;
    }
}
