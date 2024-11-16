using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Gateway.Configurations;
using Gateway.Handlers;
using StackExchange.Redis;

namespace Gateway;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        var subgraphsConfigurations =
            Configuration.GetSection(SubgraphsConfigurations.Key).Get<SubgraphsConfigurations>();
        ArgumentNullException.ThrowIfNull(subgraphsConfigurations);

        if (subgraphsConfigurations.Billing.Uri is not null)
        {
            services.AddHttpClient(nameof(subgraphsConfigurations.Billing),
                    c => c.BaseAddress = subgraphsConfigurations.Billing.Uri)
                .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();
        }

        if (subgraphsConfigurations.Booking.Uri is not null)
        {
            services.AddHttpClient(nameof(subgraphsConfigurations.Booking),
                    c => c.BaseAddress = subgraphsConfigurations.Booking.Uri)
                .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();
        }

        if (subgraphsConfigurations.Customer.Uri is not null)
        {
            services.AddHttpClient(nameof(subgraphsConfigurations.Customer),
                    c => c.BaseAddress = subgraphsConfigurations.Customer.Uri)
                .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();
        }

        if (subgraphsConfigurations.Location.Uri is not null)
        {
            services.AddHttpClient(nameof(subgraphsConfigurations.Location),
                    c => c.BaseAddress = subgraphsConfigurations.Location.Uri)
                .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();
        }

        if (subgraphsConfigurations.MsTeams.Uri is not null)
        {
            services.AddHttpClient(nameof(subgraphsConfigurations.MsTeams),
                    c => c.BaseAddress = subgraphsConfigurations.MsTeams.Uri)
                .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();
        }

        if (subgraphsConfigurations.Notification.Uri is not null)
        {
            services.AddHttpClient(nameof(subgraphsConfigurations.Notification),
                    c => c.BaseAddress = subgraphsConfigurations.Notification.Uri)
                .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();
        }

        if (subgraphsConfigurations.Organization.Uri is not null)
        {
            services.AddHttpClient(nameof(subgraphsConfigurations.Organization),
                    c => c.BaseAddress = subgraphsConfigurations.Organization.Uri)
                .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();
        }

        if (subgraphsConfigurations.Payment.Uri is not null)
        {
            services.AddHttpClient(nameof(subgraphsConfigurations.Payment),
                    c => c.BaseAddress = subgraphsConfigurations.Payment.Uri)
                .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();
        }

        if (subgraphsConfigurations.Slack.Uri is not null)
        {
            services.AddHttpClient(nameof(subgraphsConfigurations.Slack),
                    c => c.BaseAddress = subgraphsConfigurations.Slack.Uri)
                .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();
        }

        if (subgraphsConfigurations.Team.Uri is not null)
        {
            services.AddHttpClient(nameof(subgraphsConfigurations.Team),
                    c => c.BaseAddress = subgraphsConfigurations.Team.Uri)
                .AddHttpMessageHandler<ApiAuthenticationHttpClientHandler>();
        }

        services.AddRedis(Configuration);

        services.AddScoped<ApiAuthenticationHttpClientHandler>();

        var applicationConfiguration =
            Configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        ArgumentNullException.ThrowIfNull(applicationConfiguration);

        services
            .AddGraphQLServer()
            .AddCustomGraphqlInstrumentation()
            .AddRemoteSchemasFromRedis(
                applicationConfiguration.Environment,
                sp => sp.GetRequiredService<ConnectionMultiplexer>());

        // services
        //     .AddReverseProxy()
        //     .LoadFromConfig(Configuration.GetSection("ReverseProxy"));

        services.AddHealthChecks();
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(
            Environment,
            Configuration,
            configureEndpointRouteBuilder: endpointRouteBuilder =>
            {
                //  endpointRouteBuilder.MapReverseProxy();
            }
        );
}
