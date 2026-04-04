using Enterprise.Shared;
using Enterprise.Shared.Kafka;

namespace Infrastructure.Shared;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        _ = services.AddKafka(configuration, "kafka");

        services.AddServices().AddJobs();

        var app = builder.Build().UseWebApplicationDefaults<Program>();
        return app;
    }
}
