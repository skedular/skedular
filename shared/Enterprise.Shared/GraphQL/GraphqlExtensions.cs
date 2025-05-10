using Enterprise.Shared.GraphQL.Configurations;
using HotChocolate.AspNetCore;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Descriptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.GraphQL;

public static class GraphqlExtensions
{
    public static IServiceCollection AddGraphql(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRequestExecutorBuilder> configure)
    {
        var graphqlConfig = configuration.GetSection(GraphqlConfig.Key).Get<GraphqlConfig>();
        ArgumentNullException.ThrowIfNull(graphqlConfig);

        var builder = services
            .AddErrorFilter<GraphqlErrorFilter>()
            .AddSingleton<INamingConventions, CustomNamingConventions>()
            .AddGraphQLServer()
            .InitializeOnStartup()
            .DisableIntrospection(!graphqlConfig.IntrospectionEnabled)
            .AddCustomGraphqlInstrumentation();

        configure(builder);

        builder.InitializeOnStartup();

        return services;
    }

    public static void MapGraphqlEndpoints(this IEndpointRouteBuilder endpoints, IConfiguration configuration)
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

        endpoints.MapGraphQL(pathString).WithOptions(new GraphQLServerOptions { Tool = { Enable = graphqlConfig.ClientEnabled } });
    }
}
