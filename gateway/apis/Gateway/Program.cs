using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Configurations;
using Gateway.Configurations;
using Gateway.Handlers;
using HotChocolate.Fusion.Metadata;

namespace Gateway;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;

        var subgraphsConfigurations = configuration.GetSection(SubgraphsConfigurations.Key).Get<SubgraphsConfigurations>();
        ArgumentNullException.ThrowIfNull(subgraphsConfigurations);
        services.AddSingleton(subgraphsConfigurations);

        var graphqlConfig = configuration.GetSection(GraphqlConfig.Key).Get<GraphqlConfig>();
        ArgumentNullException.ThrowIfNull(graphqlConfig);

        services
            .AddHttpClient("Fusion", options => options.Timeout = TimeSpan.FromSeconds(30))
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

        _ = services
            .AddFusionGatewayServer()
            .ConfigureFromFile(filename)
            .ModifyRequestOptions(options =>
            {
                options.ExecutionTimeout = TimeSpan.FromSeconds(30);
                options.IncludeExceptionDetails = graphqlConfig.IncludeExceptionDetails;
            }).ModifyFusionOptions(options =>
            {
                options.AllowQueryPlan = graphqlConfig.AllowQueryPlan;
                options.IncludeDebugInfo = graphqlConfig.IncludeDebugInfo;
            });

        services
            .AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));

        services.AddHealthChecks();

        var app = builder.Build().UseWebApplicationDefaults<Program>();

        app.MapReverseProxy();

        return app;
    }
}
