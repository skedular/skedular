using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Configurations;
using HotChocolate.AspNetCore;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Descriptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Enterprise.Shared.GraphQL;

public static class GraphqlExtensions
{
    public static IServiceCollection AddGraphql<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRequestExecutorBuilder> configure) where TDbContext : DbContext
    {
        var graphqlConfig = configuration.GetSection(GraphqlConfig.Key).Get<GraphqlConfig>();
        ArgumentNullException.ThrowIfNull(graphqlConfig);

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

        var applicationConfiguration =
            configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        ArgumentNullException.ThrowIfNull(applicationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationConfiguration.Domain);
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationConfiguration.Environment);

        builder.PublishSchemaDefinition(descriptor =>
            descriptor
                .SetName(applicationConfiguration.Domain)
                .PublishToRedis(
                    applicationConfiguration.Environment,
                    sp => sp.GetRequiredService<ConnectionMultiplexer>()));

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
