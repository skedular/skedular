using Enterprise.Shared.GraphQL.Configurations;
using HotChocolate.AspNetCore;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.GraphQL;

public static class GraphqlExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddGraphql(IConfiguration configuration,
            Action<IRequestExecutorBuilder> configure)
        {
            var graphqlConfig = configuration.GetSection(GraphqlConfig.Key).Get<GraphqlConfig>();
            ArgumentNullException.ThrowIfNull(graphqlConfig);

            var builder = services
                .AddErrorFilter<GraphqlErrorFilter>()
                .AddGraphQLServer()
                .AddCostAnalyzer()
                .InitializeOnStartup()
                .DisableIntrospection(!graphqlConfig.IntrospectionEnabled)
                .AddCustomGraphqlInstrumentation();

            configure(builder);

            builder.InitializeOnStartup();

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

            var pathString = graphqlConfig.Path;
            if (string.IsNullOrWhiteSpace(pathString))
            {
                return;
            }

            endpoints.MapGraphQL(pathString).WithOptions(new GraphQLServerOptions
            {
                Tool =
                {
                    IncludeCookies = graphqlConfig.IncludeCookies,
                    Enable = graphqlConfig.NitroEnabled,
                    DisableTelemetry = graphqlConfig.DisableTelemetry
                }
            });
        }
    }
}
