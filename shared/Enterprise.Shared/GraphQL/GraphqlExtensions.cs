using Enterprise.Shared.GraphQL.Configurations;
using HotChocolate.AspNetCore;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Descriptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.GraphQL;

public static class GraphqlExtensions
{
    public static IServiceCollection AddGraphql<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRequestExecutorBuilder> configure) where TDbContext : DbContext
    {
        var graphqlConfig = configuration.GetSection(GraphqlConfig.Key)
            .Get<GraphqlConfig>();

        if (graphqlConfig is null)
        {
            throw new ArgumentException($"GraphQL settings not found at key: {GraphqlConfig.Key}",
                nameof(configuration));
        }

        var builder = services
            .AddErrorFilter<GraphqlErrorFilter>()
            .AddSingleton<INamingConventions, CustomNamingConventions>()
            .AddGraphQLServer()
            .RegisterDbContext<TDbContext>(DbContextKind.Pooled)
            .InitializeOnStartup()
            .AllowIntrospection(graphqlConfig.IntrospectionEnabled)
            .AddCustomGraphqlInstrumentation();

        configure(builder);

        builder.InitializeOnStartup();

        return services;
    }

    public static void MapGraphqlEndpoints(
        this IEndpointRouteBuilder endpoints,
        IConfiguration configuration)
    {
        var graphqlConfig = configuration.GetSection(GraphqlConfig.Key)
            .Get<GraphqlConfig>();

        if (graphqlConfig is null)
        {
            return;
        }

        var pathString = graphqlConfig.Path;

        if (string.IsNullOrWhiteSpace(pathString))
        {
            return;
        }

        endpoints.MapGraphQL(pathString)
            .WithOptions(new GraphQLServerOptions { Tool = { Enable = graphqlConfig.ClientEnabled } });
    }
}
