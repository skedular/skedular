using Api.Shared.Services.GraphQL.UnityHub.V1.Billing;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.GraphQL;
using Gateway.Configurations;
using Gateway.Handlers;

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

        services
            .AddHttpContextAccessor()
            .AddScoped<ApiAuthenticationHttpClientHandler>();

        services
            .AddGraphQLServer()
            .AddCustomGraphqlInstrumentation()
            .AddRemoteSchemaFromString(
                nameof(subgraphsConfigurations.Billing),
                Metadata.Schema)
            .AddRemoteSchemaFromString(
                nameof(subgraphsConfigurations.Booking),
                Api.Shared.Services.GraphQL.UnityHub.V1.Booking.Metadata.Schema)
            .AddRemoteSchemaFromString(
                nameof(subgraphsConfigurations.Customer),
                Api.Shared.Services.GraphQL.UnityHub.V1.Customer.Metadata.Schema)
            .AddRemoteSchemaFromString(
                nameof(subgraphsConfigurations.Location),
                Api.Shared.Services.GraphQL.UnityHub.V1.Location.Metadata.Schema)
            .AddRemoteSchemaFromString(
                nameof(subgraphsConfigurations.MsTeams),
                Api.Shared.Services.GraphQL.UnityHub.V1.MsTeams.Metadata.Schema)
            .AddRemoteSchemaFromString(
                nameof(subgraphsConfigurations.Notification),
                Api.Shared.Services.GraphQL.UnityHub.V1.Notification.Metadata.Schema)
            .AddRemoteSchemaFromString(
                nameof(subgraphsConfigurations.Organization),
                Api.Shared.Services.GraphQL.UnityHub.V1.Organization.Metadata.Schema)
            .AddRemoteSchemaFromString(
                nameof(subgraphsConfigurations.Payment),
                Api.Shared.Services.GraphQL.UnityHub.V1.Payment.Metadata.Schema)
            .AddRemoteSchemaFromString(
                nameof(subgraphsConfigurations.Slack),
                Api.Shared.Services.GraphQL.UnityHub.V1.Slack.Metadata.Schema)
            .AddRemoteSchemaFromString(
                nameof(subgraphsConfigurations.Team),
                Api.Shared.Services.GraphQL.UnityHub.V1.Team.Metadata.Schema);

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
