# GraphQL Module — Agent Notes

## Purpose

Registers and configures a HotChocolate GraphQL server with cost analysis, Redis or in-memory
subscriptions, introspection controls, and per-request context propagation.

## Registration

```csharp
services.AddGraphql(
    configuration,
    configure: builder =>
    {
        builder
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddSubscriptionType<Subscription>();
    },
    useRedisSubscriptions: true);   // false → in-memory (local dev / tests)

// Map the GraphQL HTTP endpoint
app.MapGraphqlEndpoints(configuration);
```

**Config section key:** `GraphQL` — see `GraphQL/Configurations/GraphqlConfig.cs`.

`MapGraphqlEndpoints` is a no-op when the `GraphQL` configuration section is absent or `Path` is empty,
so it is safe to call from non-GraphQL hosts (processors, job runners).

## Configuration Reference

```json
{
  "GraphQL": {
    "Path": "/graphql",
    "IntrospectionEnabled": true,
    "NitroEnabled": true,
    "IncludeCookies": true,
    "DisableTelemetry": false,
    "AllowQueryPlan": false,
    "IncludeDebugInfo": false,
    "IncludeExceptionDetails": false
  }
}
```

## Subscriptions

- Redis subscriptions require a registered `IConnectionMultiplexer` (via `Cache/Extensions.cs`
  `AddRedis`). Topics are prefixed with `{environment}:{domain}:` from `ApplicationConfiguration`.
- Use `IGraphQlTopicEventSender` (wrapper around `ITopicEventSender`) to send subscription events
  from domain services without a direct HotChocolate dependency.

## Pagination

The `Pagination/` module provides cursor-based (keyset) pagination helpers:

```csharp
await query.ToPaginatedAsync(paginationInput, keysetFields, cancellationToken);
```

Returns `PaginatedInfo<T>` with edges, `PageInfo`, and total count. Use `Connection<T>` / `Node<T>` /
`PageInfo` types from `GraphQL/Types/` as the GraphQL-facing output types.

## Error Handling

`GraphqlErrorFilter` (registered by `AddGraphql`) catches unhandled exceptions and converts them to
GraphQL error responses. Do not let domain exceptions propagate unfiltered to the client.

## Context Propagation

`RequestContextPropagationHandler` (`GraphQL/Handlers/`) is an `IHttpMessageHandler` that forwards
request-scoped headers (e.g. correlation ID) to downstream HTTP calls made during GraphQL field
resolution.

## Rules

- Always add `HotChocolate.Fusion.SourceSchema` annotations when the schema is part of a federated
  gateway — do not expose raw types without subgraph annotations.
- Do not hand-edit exported `schema.graphql` files; regenerate via `scripts/generate-graphql.sh`.
- Follow the choice-type pattern (see root `AGENTS.md` → *GraphQL Choice Types*) for enum-backed
  selection fields.
