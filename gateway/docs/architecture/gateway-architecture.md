# Gateway Architecture

This document is a high-level architecture view of the Gateway as it exists today.

It is intentionally C4-style rather than implementation-complete. The goal is to explain:

- how the gateway acts as the single entry point for all clients
- how GraphQL federation is composed using HotChocolate Fusion
- how REST traffic is routed via YARP reverse proxy
- how authentication is handled and propagated to subgraphs
- how Aspire service-discovery names are rewritten to real URLs at runtime
- how schema changes flow from domain APIs through to the composed gateway schema

## Scope

This document covers the Gateway surfaces under:

- `gateway/apis/Gateway`

It references all domain API subgraphs that the gateway composes:

- Booking, Core, Customer, Location, Marketplace, MsTeams, Organization, Slack, Team

## Purpose and Scope

The Gateway is the **single entry point** for all Skedular clients (web app, Teams app, Slack app, and external
integrations). It contains **no domain business logic** — it is a pure routing and composition layer.

Two traffic paths flow through the gateway:

1. **GraphQL** — HotChocolate Fusion composes all domain GraphQL subgraphs into one federated schema. Clients send a
   single query to the gateway and Fusion fans out to the relevant subgraphs, stitches results, and returns a unified
   response.

2. **REST** — YARP reverse proxy routes requests by URL path prefix to the matching domain API. The gateway adds no
   transformation; it is transparent pass-through with auth context forwarded.

## System Context

```mermaid
flowchart LR
    WebApp["Web App"]
    TeamsApp["MS Teams App"]
    SlackApp["Slack App"]
    External["External / MCP Clients"]

    subgraph Gateway["Gateway  (port 9000 / 9001)"]
        GQL["HotChocolate Fusion\nGraphQL  /v1/graphql"]
        YARP["YARP Reverse Proxy\n/v1/{domain}/{**catch-all}"]
        MCP["MCP Server\n/v1/mcp/sse"]
        Auth["JWT Auth Middleware\n(Enterprise.Shared)"]
    end

    WebApp --> Auth
    TeamsApp --> Auth
    SlackApp --> Auth
    External --> Auth

    Auth --> GQL
    Auth --> YARP
    Auth --> MCP

    GQL -->|"HTTP (Fusion)"| DomainAPIs["Domain API Subgraphs"]
    YARP -->|"HTTP (pass-through)"| DomainAPIs
```

## Architecture Diagram

```mermaid
flowchart TB
    Client["Client\n(Web / Teams / Slack / External)"]

    subgraph GatewayLayer["Gateway"]
        direction TB
        JWTMw["JWT Validation\n(Enterprise.Shared security middleware)"]
        GqlFusion["HotChocolate Fusion\n(reads gateway.fgp at startup)"]
        YARPProxy["YARP Reverse Proxy\n(routes by path prefix)"]
        SDRewriter["ServiceDiscoveryConfigurationRewriter\n(resolves Aspire names → real URLs)"]
        McpSrv["MCP Server"]

        JWTMw --> GqlFusion
        JWTMw --> YARPProxy
        JWTMw --> McpSrv
        GqlFusion --> SDRewriter
    end

    subgraph DomainSubgraphs["Domain API Subgraphs"]
        Booking["Booking API\n:10300"]
        Core["Core API\n:11100"]
        Customer["Customer API\n:10000"]
        Location["Location API\n:10600"]
        Marketplace["Marketplace API\n:11000"]
        MsTeams["MsTeams API\n:10900"]
        Organization["Organization API\n:10200"]
        Slack["Slack API\n:10700"]
        Team["Team API\n:10500"]
    end

    Client --> JWTMw
    GqlFusion -->|"POST /v1/graphql\n(per-subgraph fan-out)"| DomainSubgraphs
    YARPProxy -->|"transparent pass-through"| DomainSubgraphs
```

## GraphQL Fusion Composition

```mermaid
flowchart LR
    subgraph SchemaGeneration["Schema Generation  (offline / CI)"]
        direction TB
        ProtoGen["api-definitions/generate.sh\n(OpenAPI + protobuf)"]
        GqlScript["scripts/generate-graphql.sh\n(per-domain schema export\n+ fusion compose)"]
        FgpFile["gateway/apis/Gateway/gateway.fgp\n(compiled fusion package)"]

        ProtoGen --> GqlScript
        GqlScript --> FgpFile
    end

    subgraph GatewayRuntime["Gateway Runtime"]
        direction TB
        EmbeddedFgp["gateway.fgp embedded\nin Gateway assembly"]
        FusionServer["AddFusionGatewayServer()\n.ConfigureFromFile(fgp)"]
        SDRw["ServiceDiscoveryConfigurationRewriter\nrewrites subgraph endpoint URIs\nfrom appsettings Subgraphs config"]

        EmbeddedFgp --> FusionServer
        FusionServer --> SDRw
    end

    subgraph Subgraphs["Subgraph APIs (each exports schema.graphql)"]
        B["Booking"]
        Co["Core"]
        Cu["Customer"]
        L["Location"]
        M["Marketplace"]
        MT["MsTeams"]
        O["Organization"]
        S["Slack"]
        T["Team"]
    end

    Subgraphs -->|"dotnet run -- schema export\n(via generate-graphql.sh)"| GqlScript
    FgpFile -->|"bundled into assembly"| EmbeddedFgp
    SDRw -->|"HTTP requests"| Subgraphs
```

### How a GraphQL Request is Processed

```mermaid
sequenceDiagram
    participant Client as Client
    participant GW as Gateway (Fusion)
    participant B as Booking API
    participant C as Customer API
    participant O as Organization API

    Client->>GW: POST /v1/graphql { query }
    Note over GW: Parse query plan from .fgp
    GW->>B: POST /v1/graphql (partial query)
    GW->>C: POST /v1/graphql (partial query)
    GW->>O: POST /v1/graphql (partial query)
    B-->>GW: partial result
    C-->>GW: partial result
    O-->>GW: partial result
    Note over GW: Stitch results
    GW-->>Client: unified response
```

## YARP Routing

The gateway configures YARP from `appsettings.json` under the `ReverseProxy` key. Each domain has a route (path
prefix) and a cluster (destination address).

```mermaid
flowchart LR
    Client["Client"]

    subgraph YARP["YARP Routes"]
        R1["v1/booking/{**catch-all}"]
        R2["v1/core/{**catch-all}"]
        R3["v1/customer/{**catch-all}"]
        R4["v1/location/{**catch-all}"]
        R5["v1/marketplace/{**catch-all}"]
        R6["v1/msteams/{**catch-all}"]
        R7["v1/organization/{**catch-all}"]
        R8["v1/slack/{**catch-all}"]
        R9["v1/team/{**catch-all}"]
    end

    subgraph Clusters["YARP Clusters (Destinations)"]
        C1[":10300  Booking"]
        C2[":11100  Core"]
        C3[":10000  Customer"]
        C4[":10600  Location"]
        C5[":11000  Marketplace"]
        C6[":10900  MsTeams"]
        C7[":10200  Organization"]
        C8[":10700  Slack"]
        C9[":10500  Team"]
    end

    Client --> R1 --> C1
    Client --> R2 --> C2
    Client --> R3 --> C3
    Client --> R4 --> C4
    Client --> R5 --> C5
    Client --> R6 --> C6
    Client --> R7 --> C7
    Client --> R8 --> C8
    Client --> R9 --> C9
```

YARP routing is transparent — the gateway forwards the full path and headers, including the JWT bearer token. No
response transformation is applied.

## Auth Flow

JWT validation is handled by Enterprise.Shared security middleware registered in `AddDefaultServices<Program>()`.
The gateway does not issue tokens — it only validates them and forwards auth context to subgraphs.

```mermaid
sequenceDiagram
    participant Client as Client
    participant GW as Gateway
    participant Middleware as JWT Middleware\n(Enterprise.Shared)
    participant Subgraph as Domain API Subgraph

    Client->>GW: Request + Authorization: Bearer <token>
    GW->>Middleware: Validate JWT signature + claims
    alt valid token
        Middleware->>GW: ClaimsPrincipal populated
        GW->>Subgraph: Forward request\n(Authorization header propagated via\nRequestContextPropagationHandler)
        Subgraph-->>GW: response
        GW-->>Client: response
    else invalid / missing token
        Middleware-->>Client: 401 Unauthorized
    end
```

Auth context is propagated to subgraphs by `RequestContextPropagationHandler`, which is registered as an HTTP message
handler on the `Fusion` HTTP client. Subgraphs receive the original `Authorization` header and resolve the caller
identity independently using the same JWT validation middleware.

## ServiceDiscovery — Aspire Name Resolution

In local development (Aspire-hosted), subgraph URLs are configured by service-discovery names. At runtime,
`ServiceDiscoveryConfigurationRewriter` intercepts each subgraph's `HttpClientConfiguration` and replaces the
endpoint URI with the resolved address from `appsettings.json` under `Subgraphs`:

```mermaid
flowchart LR
    FusionConfig["HotChocolate Fusion\n(reads compiled .fgp)"]
    SDRewriter["ServiceDiscoveryConfigurationRewriter\n(IConfigurationRewriter)"]
    SubgraphsConfig["appsettings.json\nSubgraphs:\n  Booking.Uri: ...\n  Core.Uri: ...\n  ..."]

    FusionConfig -->|"RewriteAsync per subgraph"| SDRewriter
    SDRewriter -->|"reads"| SubgraphsConfig
    SDRewriter -->|"returns resolved HttpClientConfiguration"| FusionConfig
```

This decouples the compiled `.fgp` schema package (which may embed compile-time URIs) from the actual runtime
endpoints, enabling both local Aspire dev and production deployments to share the same compiled artifact.

## Schema Regeneration Workflow

Any change to a domain GraphQL schema requires regenerating the composed fusion schema and the embedded `.fgp` file.

```mermaid
flowchart TD
    A["Change domain GraphQL type or field\n(e.g. add query, mutation, subscription)"] --> B["Run scripts/generate-graphql.sh"]
    B --> C["Per-domain schema.graphql files regenerated\n(dotnet run -- schema export per API)"]
    C --> D["gateway.fgp recompiled\n(HotChocolate Fusion compose)"]
    D --> E["web/apps/webapp/scripts/generate.sh\n(Relay artifact regeneration)"]
    E --> F["Commit all changed generated files\n(schema.graphql · gateway.fgp · Relay artifacts)"]
```

> **Do not hand-edit** `gateway.fgp`, per-domain `schema.graphql` files, or
> `api-definitions/graphql/skedular/v1/schema.graphql`. Always use `scripts/generate-graphql.sh`
> (or `make generate` for the full regeneration pipeline).

The full pipeline via `make generate` runs:

1. `api-definitions/generate.sh` — regenerates OpenAPI controller bases, API clients, and protobuf event classes.
2. `scripts/generate-graphql.sh` — exports per-API schemas, composes the gateway fusion package, updates the composed
   schema, and regenerates GraphQL init/schema files.
3. `web/apps/webapp/scripts/generate.sh` — regenerates TypeScript API clients and Relay artifacts.

## Reading Guide

| You want to understand… | Start here |
|---|---|
| Gateway startup and Fusion wiring | `gateway/apis/Gateway/Program.cs` |
| YARP route and cluster configuration | `gateway/apis/Gateway/appsettings.json` → `ReverseProxy` section |
| Subgraph URL configuration | `gateway/apis/Gateway/appsettings.json` → `Subgraphs` section |
| How subgraph URIs are resolved at runtime | `gateway/apis/Gateway/ServiceDiscoveryConfigurationRewriter.cs` |
| Subgraph configuration model | `gateway/apis/Gateway/Configurations/SubgraphsConfigurations.cs` |
| Auth context propagation to subgraphs | `gateway/apis/Gateway/Extensions.cs` → `RequestContextPropagationHandler` |
| GraphQL Fusion schema regeneration | `scripts/generate-graphql.sh` |
| Full regeneration pipeline | `Makefile` → `make generate` |
| Compiled fusion schema package | `gateway/apis/Gateway/gateway.fgp` (binary; regenerate, do not edit) |
