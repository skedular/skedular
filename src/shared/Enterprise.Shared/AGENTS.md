# Enterprise.Shared Agent Notes

This file applies to everything under `shared/Enterprise.Shared`.

## Library Purpose

`Enterprise.Shared` is a composable, technology-agnostic infrastructure library. It is designed to be consumed by any
.NET application — not only by this repository. Each module is independently adoptable:
consumers register only the modules they need. No module is forced on any host.

The root `Extensions.cs` provides a repository-level convenience bundle (`AddDefaultServices`,
`UseWebApplicationDefaults`) that wires everything together for hosts in this repo. External consumers **should not call
those bundle methods**; they should compose the individual module registrations instead.

## Modularity Design — Registration Entry Points

Every module exposes its own `Add*` / `Use*` extension methods. Call only the ones your application needs.

### Builder Modules (`WebApplicationBuilder` / `IServiceCollection` extensions)

| Method                                                      | Where defined                   | What it registers                                                                                                                                             |
|-------------------------------------------------------------|---------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `builder.AddCoreServices<TProgram>()`                       | `Extensions.cs`                 | Config, `ApplicationConfiguration`, OpenTelemetry, service discovery, HTTP timeout, auth/authz, CORS, problem details, core singletons, liveness health check |
| `builder.AddIdentityTokenProviders()`                       | `Extensions.cs`                 | Each token provider registered only when its config section is present: WorkOS, Cognito, Google, Azure Entra; also aggregates the registered `ITokenService`s |
| `builder.AddCookieServices()`                               | `Extensions.cs`                 | `CookieConfiguration` + `ICookieEncryptionService` when the `Cookie` config section is present                                                                |
| `builder.AddHybridCaching()`                                | `Extensions.cs`                 | Redis-backed `HybridCache` with GeoJSON-aware JSON options — requires Redis to be registered separately                                                       |
| `builder.AddApiControllers<TProgram>()`                     | `Extensions.cs`                 | MVC controllers, camelCase JSON, Swagger/NSwag, validation error shapes                                                                                       |
| `builder.AddSerilogLogging<TProgram>()`                     | `Extensions.cs`                 | Serilog host integration, clears default log providers                                                                                                        |
| `builder.AddDefaultServices<TProgram>()`                    | `Extensions.cs`                 | **In-repo bundle** — calls all five above in order. Do not use in external projects.                                                                          |
| `services.AddKafka(...)`                                    | `Kafka/Extensions.cs`           | Full Kafka producer/consumer stack                                                                                                                            |
| `services.AddTemporalWorker(...)`                           | `Temporal/Extensions.cs`        | Temporal worker + outbox writers                                                                                                                              |
| `services.AddTemporalClient(...)`                           | `Temporal/Extensions.cs`        | Temporal client only (no hosted worker)                                                                                                                       |
| `services.AddGraphql(...)`                                  | `GraphQL/GraphqlExtensions.cs`  | HotChocolate GraphQL server                                                                                                                                   |
| `services.AddRedis(...)`                                    | `Cache/Extensions.cs`           | `IConnectionMultiplexer` + `IDistributedCache` via StackExchange.Redis                                                                                        |
| `services.AddFileStorage(...)`                              | `FileStorage/Extensions.cs`     | `ICdnService` + `IFileService` (local dev or Cloudflare)                                                                                                      |
| `services.AddStripe(...)`                                   | `Payment/Extensions.cs`         | Stripe SDK service interfaces                                                                                                                                 |
| `services.AddXeroServices(...)`                             | `Accounting/Extensions.cs`      | `IXeroSdkClientFactory`, `IXeroTokenEncryptionService`                                                                                                        |
| `services.AddMcpServer(...)`                                | `Ai/Extentions.cs`              | Model Context Protocol server                                                                                                                                 |
| `services.AddSecurity()`                                    | `Security/Extensions.cs`        | gRPC authenticator and the security middleware surface; expects token providers to already be registered                                                      |
| `services.AddSso()`                                         | `Security/Sso/Extensions.cs`    | SAML assertion consumer + login request factory                                                                                                               |
| `services.AddKafkaOutboxBackgroundService<TDbContext>()`    | `Outbox/Kafka/Extensions.cs`    | Background service that drains `KafkaOutbox` rows to Kafka                                                                                                    |
| `services.AddKafkaOutboxService()`                          | `Outbox/Kafka/Extensions.cs`    | Open-generic `IKafkaOutboxEventPublisher<,>` — already called by `AddKafka`                                                                                   |
| `services.AddTemporalOutboxBackgroundService<TDbContext>()` | `Outbox/Temporal/Extensions.cs` | Background services for workflow-start and signal outbox draining                                                                                             |
| `services.AddTemporalOutboxService()`                       | `Outbox/Temporal/Extensions.cs` | `ITemporalOutboxWorkflowExecutor` + `ITemporalSignalOutboxWorkflowExecutor` — already called by `AddTemporalWorker`                                           |

### App Modules (`WebApplication` extensions)

| Method                                      | Where defined                  | What it does                                                                                         |
|---------------------------------------------|--------------------------------|------------------------------------------------------------------------------------------------------|
| `app.UseApplicationCore<TProgram>()`        | `Extensions.cs`                | Exception handling, CORS, routing, auth, health checks, context middleware, controllers — no GraphQL |
| `app.UseWebApplicationDefaults<TProgram>()` | `Extensions.cs`                | **In-repo bundle** — `UseApplicationCore` + `MapGraphqlEndpoints` (no-op when GraphQL config absent) |
| `app.MapGraphqlEndpoints(configuration)`    | `GraphQL/GraphqlExtensions.cs` | Maps the HotChocolate GraphQL route                                                                  |
| `app.UseSecurity()`                         | `Security/Extensions.cs`       | Adds `SecurityContextEnricherMiddleware`                                                             |
| `app.UseSso()`                              | `Security/Sso/Extensions.cs`   | Adds `SsoContextEnricherMiddleware`                                                                  |
| `app.UseMcpServer()`                        | `Ai/Extentions.cs`             | Maps the MCP HTTP transport route                                                                    |

## Module Folders

Each subfolder has its own `AGENTS.md` with module-specific rules:

- `Accounting/` — Xero OAuth2 accounting integration
- `Ai/` — AI agent session helpers and MCP server
- `Cache/` — Redis connection and `HybridCache`
- `Database/` — EF Core base types, Postgres and SQL Server registration helpers
- `FileStorage/` — CDN and private file upload (local or Cloudflare)
- `GraphQL/` — HotChocolate server setup and endpoint mapping
- `Grpc/` — gRPC metadata helpers
- `Kafka/` — Kafka producer/consumer/outbox infrastructure; depends on `Api.Shared` for event contracts
- `Outbox/` — Transactional outbox pattern (Kafka sub-module and Temporal sub-module)
- `Payment/` — Stripe SDK service registration
- `Cookie/` — Cookie configuration and `ICookieEncryptionService`
- `Encryption/` — Shared encryption primitives and `IStringEncryptionAlgorithm`
- `IdentityProviders/` — Provider-specific token validators and related configuration
- `Security/` — Security middleware pipeline, SAML SSO, gRPC authentication, and shared token contracts
- `Telemetry/` — OpenTelemetry traces, metrics, context propagation
- `Temporal/` — Temporal workflow worker and client registration

## Auth And Encryption Layout

- Keep auth and encryption code split by concern:
    - `Cookie/` for cookie-specific encryption wiring
    - `Encryption/` for reusable low-level encryption primitives
    - `IdentityProviders/` for WorkOS, Cognito, Google, and Azure Entra token validators
    - `Security/` for middleware, SSO, gRPC auth, and `ITokenService` consumption
- Do not re-couple cookie services into `AddIdentityTokenProviders()`.
- Do not move provider implementations into `Security/`; that folder is now the consumer/pipeline boundary rather than
  the provider-implementation home.

## Cross-Cutting Helpers

- Helpers that are not owned by a single domain or runtime host belong here.
- If a toggle or utility is used by more than one host or integration-test project, add it here rather than duplicating
  it per-host.
- `DomainAppHostEnvironmentVariables` and `Constants` live at the root because they are shared by hosts and test
  projects alike.

## Adding a New Module

1. Create a subfolder with a clearly scoped `Extensions.cs` containing `Add*` / `Use*` extension methods.
2. Add an `AGENTS.md` in the new folder documenting: purpose, registration entry point, config section key,
   prerequisites (other `Add*` that must run first), and NuGet dependencies.
3. Add the new method to the table above.
4. Do **not** add the new module to `AddDefaultServices` or `UseWebApplicationDefaults`. Consumers opt in explicitly;
   nothing is wired by default.

## Unit Test File Shape

- Keep one test class/file per public method rather than one large `...Should.cs` covering the whole service.
- Group under the service namespace, e.g. `TemporalHelperServiceTests/ToIdShould.cs`.
- Prefer injected test inputs over hardcoded strings unless testing a specific literal contract.
- Order test method parameters: frozen/injected constructor dependencies → `sut` → random inputs and expected values.
