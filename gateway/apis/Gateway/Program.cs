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

        var fusionGatewayBuilder =
            services
                .AddFusionGatewayServer()
                .ConfigureFromFile("gateway.fgp");

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

        var app = builder.Build().UseWebApplicationDefaults();

        app.MapReverseProxy();

        return app;
    }
}
