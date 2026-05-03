using Enterprise.Shared;
using Enterprise.Shared.Ai;
using Enterprise.Shared.GraphQL.Configurations;
using Enterprise.Shared.GraphQL.Handlers;
using Gateway.Configurations;
using HotChocolate;
using Path = System.IO.Path;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

namespace Gateway;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>(true);
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddTransient<RequestContextPropagationHandler>();

        var subgraphsConfigurations = configuration.GetSection(SubgraphsConfigurations.Key).Get<SubgraphsConfigurations>()
                                      ?? new SubgraphsConfigurations();

        foreach (var (name, subgraph) in subgraphsConfigurations)
        {
            if (string.IsNullOrWhiteSpace(subgraph.ClientName))
            {
                throw new InvalidOperationException($"Subgraph '{name}' is missing a ClientName in configuration.");
            }

            var targetUrl = subgraph.Url;
            services
                .AddHttpClient(subgraph.ClientName)
                .AddHttpMessageHandler(_ => new RewriteHostHandler(targetUrl))
                .AddHttpMessageHandler<RequestContextPropagationHandler>();
        }

        var graphqlConfig = configuration.GetSection(GraphqlConfig.Key).Get<GraphqlConfig>();
        ArgumentNullException.ThrowIfNull(graphqlConfig);

        // Register so UseWebApplicationDefaults/MapGraphqlEndpoints maps the Fusion gateway endpoint.
        services.AddSingleton(new GraphqlSchemaRegistration(ISchemaDefinition.DefaultName, graphqlConfig.Path));

        _ = services
            .AddGraphQLGatewayServer(disableDefaultSecurity: true)
            .AddFileSystemConfiguration(Path.Combine(AppContext.BaseDirectory, "gateway.far"))
            .ModifyRequestOptions(options =>
            {
                options.IncludeExceptionDetails = graphqlConfig.IncludeExceptionDetails;
                options.CollectOperationPlanTelemetry = graphqlConfig.CollectOperationPlanTelemetry;
                options.AllowErrorHandlingModeOverride = graphqlConfig.AllowErrorHandlingModeOverride;

                if (graphqlConfig.ExecutionTimeout is { } executionTimeout)
                {
                    options.ExecutionTimeout = executionTimeout;
                }
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
