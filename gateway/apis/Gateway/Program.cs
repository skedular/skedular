using Enterprise.Shared;
using Gateway.Configurations;
using Gateway.Handlers;
using HotChocolate.Fusion.Metadata;

namespace Gateway;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .AddDefaultServices<Program>();

        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        var subgraphsConfigurations = configuration.GetSection(SubgraphsConfigurations.Key).Get<SubgraphsConfigurations>();
        ArgumentNullException.ThrowIfNull(subgraphsConfigurations);
        services.AddSingleton(subgraphsConfigurations);

        services
            .AddHttpClient("Fusion")
            .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();

        services
            .AddSingleton<IConfigurationRewriter, ServiceDiscoveryConfigurationRewrite>()
            .AddScoped<ApiAuthenticationHttpClientHandler>();

        var filename = Path.GetTempFileName();
        using (var embeddedGatewayFileStream = typeof(Program).Assembly.GetManifestResourceStream($"{typeof(Program).Namespace}.gateway.fgp"))
        {
            ArgumentNullException.ThrowIfNull(embeddedGatewayFileStream);
            using (var fileStream = File.OpenWrite(filename))
            {
                embeddedGatewayFileStream.CopyTo(fileStream);
            }
        }

        var fusionGatewayBuilder =
            services
                .AddFusionGatewayServer()
                .ConfigureFromFile(filename);

        if (environment.IsDevelopment())
        {
            fusionGatewayBuilder.ModifyFusionOptions(options =>
            {
                options.AllowQueryPlan = true;
                options.IncludeDebugInfo = true;
            });
        }

        services
            .AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));

        services.AddHealthChecks();

        var app = builder.Build().UseWebApplicationDefaults<Program>();

        app.MapReverseProxy();

        return app;
    }
}
