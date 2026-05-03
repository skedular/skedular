namespace Enterprise.Shared.GraphQL.Configurations;

/// <summary>
///     Marker registration recording a GraphQL schema that was wired up via
///     <c>AddGraphql</c>. Used by <c>MapGraphqlEndpoints</c> to know which named
///     schema(s) to map onto HTTP routes.
/// </summary>
/// <param name="SchemaName">
///     HotChocolate schema name. Must match the name passed to <c>AddGraphQLServer(name)</c>
///     and to the Fusion subgraph's <c>schema-settings.json</c> <c>name</c> field.
/// </param>
/// <param name="Path">
///     HTTP route path the schema is mapped onto (e.g., <c>/v1/graphql</c>).
/// </param>
public sealed record GraphqlSchemaRegistration(string SchemaName, string Path);
