using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Configurations;
using HotChocolate.Execution.Configuration;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace Enterprise.Shared.GraphQL;

public static class GraphqlExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddGraphql(
            IConfiguration configuration,
            string schemaName,
            Action<IRequestExecutorBuilder> configure,
            bool useRedisSubscriptions = true,
            bool useAuthorization = false)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(schemaName);

            var graphqlConfig = configuration.GetSection(GraphqlConfig.Key).Get<GraphqlConfig>();
            ArgumentNullException.ThrowIfNull(graphqlConfig);

            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            ArgumentNullException.ThrowIfNull(applicationConfiguration);

            // Record this schema so MapGraphqlEndpoints can map it onto an HTTP route later.
            services.AddSingleton(new GraphqlSchemaRegistration(schemaName, graphqlConfig.Path));

            var builder = services
                .AddErrorFilter<GraphqlErrorFilter>()
                .AddGraphQLServer(schemaName);

            if (useAuthorization)
            {
                builder = builder.AddAuthorization();
            }

            builder = builder
                .AddCostAnalyzer()
                .DisableIntrospection(!graphqlConfig.IntrospectionEnabled)
                .AddCustomGraphqlInstrumentation();

            var subscriptionOptions = new SubscriptionOptions { TopicPrefix = $"{applicationConfiguration.Environment}:{schemaName}:" };

            builder = useRedisSubscriptions
                ? builder.AddRedisSubscriptions(subscriptionOptions)
                : builder.AddInMemorySubscriptions(subscriptionOptions);

            configure(builder);

            if (!graphqlConfig.DisableTelemetry)
            {
                services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddHotChocolateInstrumentation());
            }

            return services;
        }
    }

    extension(IEndpointRouteBuilder endpoints)
    {
        public void MapGraphqlEndpoints(IConfiguration configuration)
        {
            var graphqlConfig = configuration.GetSection(GraphqlConfig.Key).Get<GraphqlConfig>();
            if (graphqlConfig is null)
            {
                return;
            }

            var registrations = endpoints.ServiceProvider.GetServices<GraphqlSchemaRegistration>();
            foreach (var registration in registrations)
            {
                if (string.IsNullOrWhiteSpace(registration.Path))
                {
                    continue;
                }

                endpoints
                    .MapGraphQL(registration.Path, registration.SchemaName)
                    .WithOptions(graphQlServerOptions =>
                    {
                        graphQlServerOptions.Tool.IncludeCookies = graphqlConfig.IncludeCookies;
                        graphQlServerOptions.Tool.Enable = graphqlConfig.NitroEnabled;
                        graphQlServerOptions.Tool.DisableTelemetry = graphqlConfig.DisableTelemetry;
                    });
            }
        }
    }
}
