using Enterprise.Shared;
using Enterprise.Shared.Ai;
using Enterprise.Shared.GraphQL.Configurations;
using Enterprise.Shared.GraphQL.Handlers;
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
            .AddHttpMessageHandler<RequestContextPropagationHandler>();

        services
            .AddSingleton<IConfigurationRewriter, ServiceDiscoveryConfigurationRewrite>()
            .AddScoped<RequestContextPropagationHandler>();

        var graphqlConfig = configuration.GetSection(GraphqlConfig.Key).Get<GraphqlConfig>();
        ArgumentNullException.ThrowIfNull(graphqlConfig);

        var embeddedFgpStream = typeof(Program).Assembly.GetManifestResourceStream($"{typeof(Program).Namespace}.gateway.fgp");
        if (embeddedFgpStream is null && !builder.Environment.IsDevelopment())
        {
            throw new InvalidOperationException(
                "gateway.fgp embedded resource is required in non-development environments. Run scripts/generate-graphql.ps1 to generate it.");
        }

        if (embeddedFgpStream is not null)
        {
            var filename = Path.GetTempFileName();
            using (embeddedFgpStream)
            using (var fileStream = File.OpenWrite(filename))
            {
                embeddedFgpStream.CopyTo(fileStream);
            }

            _ = services
                .AddFusionGatewayServer()
                .ConfigureFromFile(filename)
                .ModifyRequestOptions(options =>
                {
                    options.ExecutionTimeout = TimeSpan.FromMinutes(1);
                    options.IncludeExceptionDetails = graphqlConfig.IncludeExceptionDetails;
                })
                .ModifyFusionOptions(options =>
                {
                    options.AllowQueryPlan = graphqlConfig.AllowQueryPlan;
                    options.IncludeDebugInfo = graphqlConfig.IncludeDebugInfo;
                });
        }

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
