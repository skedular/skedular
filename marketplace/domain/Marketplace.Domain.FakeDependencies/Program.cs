using Enterprise.Shared;

namespace Marketplace.Domain.FakeDependencies;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();

        builder.Services.AddGrpc();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
