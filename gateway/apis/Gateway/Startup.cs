using Enterprise.Shared.Application.WebHostService;
using Gateway.Configurations;
using Gateway.Handlers;
using HotChocolate.Fusion.Metadata;

namespace Gateway;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        var subgraphsConfigurations =
            Configuration.GetSection(SubgraphsConfigurations.Key).Get<SubgraphsConfigurations>();
        ArgumentNullException.ThrowIfNull(subgraphsConfigurations);
        services.AddSingleton(subgraphsConfigurations);

        services
            .AddHttpClient("Fusion")
            .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();

        services
            .AddSingleton<IConfigurationRewriter, ServiceDiscoveryConfigurationRewrite>()
            .AddScoped<ApiAuthenticationHttpClientHandler>();

        var fusionGatewayBuilder = services
            .AddFusionGatewayServer()
            .ConfigureFromFile("gateway.fgp");

        if (Environment.IsDevelopment())
        {
            fusionGatewayBuilder.ModifyFusionOptions(options =>
            {
                options.AllowQueryPlan = true;
                options.IncludeDebugInfo = true;
            });
        }

        services
            .AddReverseProxy()
            .LoadFromConfig(Configuration.GetSection("ReverseProxy"));

        services.AddHealthChecks();
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(
            Environment,
            Configuration,
            configureEndpointRouteBuilder: endpointRouteBuilder =>
            {
                 endpointRouteBuilder.MapReverseProxy();
            }
        );
}
