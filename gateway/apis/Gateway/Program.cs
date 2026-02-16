using Enterprise.Shared;
using Enterprise.Shared.Ai;
using Enterprise.Shared.GraphQL.Configurations;
using Gateway.Handlers;
using HotChocolate.Fusion.Metadata;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

namespace Gateway;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .AddConfigurations(configuration);

        services
            .AddHttpClient("Fusion", options => options.Timeout = TimeSpan.FromMinutes(1))
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

        var graphqlConfig = configuration.GetSection(GraphqlConfig.Key).Get<GraphqlConfig>();
        ArgumentNullException.ThrowIfNull(graphqlConfig);

        _ = services
            .AddFusionGatewayServer()
            .ConfigureFromFile(filename)
            .ModifyRequestOptions(options =>
            {
                options.ExecutionTimeout = TimeSpan.FromMinutes(1);
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

        services.AddMcpServer(configuration, [typeof(Program)]);

        var app = builder.Build().UseWebApplicationDefaults<Program>();

        app.MapReverseProxy();
        app.UseMcpServer();

        return app;
    }
}
