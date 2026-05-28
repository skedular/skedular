using Enterprise.Shared;
using Enterprise.Shared.Ai;
using Enterprise.Shared.GraphQL;
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
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddTransient<RequestContextPropagationHandler>();

        var graphqlConfig = configuration.GetSection(GraphqlConfig.Key).Get<GraphqlConfig>();
        ArgumentNullException.ThrowIfNull(graphqlConfig);

        var subgraphsConfigurations = configuration.GetSection(SubgraphsConfigurations.Key).Get<SubgraphsConfigurations>()
                                      ?? new SubgraphsConfigurations();

        foreach (var (name, subgraph) in subgraphsConfigurations)
        {
            if (string.IsNullOrWhiteSpace(subgraph.ClientName))
            {
                throw new InvalidOperationException($"Subgraph '{name}' is missing a ClientName in configuration.");
            }

            var targetUrl = subgraph.Url;
            var httpClientBuilder = services
                .AddHttpClient(subgraph.ClientName)
                .ConfigureHttpClient(httpClient => httpClient.Timeout = Timeout.InfiniteTimeSpan)
                .AddHttpMessageHandler(_ => new RewriteHostHandler(targetUrl));

            httpClientBuilder.AddStandardResilienceHandler(options =>
            {
                options.AttemptTimeout.Timeout = graphqlConfig.SubgraphAttemptTimeout ?? TimeSpan.FromSeconds(15);
                options.TotalRequestTimeout.Timeout = graphqlConfig.ExecutionTimeout ?? TimeSpan.FromSeconds(60);
            });

            httpClientBuilder.AddHttpMessageHandler<RequestContextPropagationHandler>();
        }

        // Register so UseWebApplicationDefaults/MapGraphqlEndpoints maps the Fusion gateway endpoint.
        services.AddSingleton(new GraphqlSchemaRegistration(ISchemaDefinition.DefaultName, graphqlConfig.Path));

        _ = services
            .AddGraphQLGatewayServer(disableDefaultSecurity: true)
            .AddFileSystemConfiguration(Path.Combine(AppContext.BaseDirectory, "gateway.far"))
            .AddWarmupTask<ConfiguredGraphqlWarmupTask>(
                serviceProvider =>
                    new ConfiguredGraphqlWarmupTask(
                        ISchemaDefinition.DefaultName,
                        graphqlConfig.WarmupQueries.Count == 0 ? ["{ __typename }"] : graphqlConfig.WarmupQueries,
                        serviceProvider.GetRootServiceProvider().GetRequiredService<ILogger<ConfiguredGraphqlWarmupTask>>()),
                _ => false)
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
