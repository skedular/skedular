# Shared Libraries Agent Notes

This file is the entry point for AI agents working in `shared/`.

## Structure

```
shared/
  Api.Shared/                 # Shared event contracts, Kafka topic attributes (netstandard2.0, .NET Framework compatible)
  Api.Shared.UnitTests/       # Unit tests for Api.Shared event contracts
  Api.Shared.Clients/         # Generated event client metadata, OpenAPI clients, gRPC client helpers (netstandard2.0)
  Api.Shared.Clients.UnitTests/
  Api.Shared.Services/        # Generated OpenAPI controller bases, gRPC service helpers
  Api.Shared.Services.UnitTests/
  Enterprise.Shared/          # Composable infrastructure library (Kafka, Temporal, Redis, GraphQL, …)
  Enterprise.Shared.UnitTests/
  Infrastructure.Shared/      # Local Aspire-based infrastructure bootstrapping for dev/tests
  Skedularctl/                # CLI tool for repo-level maintenance tasks
  Testing.Shared/             # Shared test helpers (non-integration)
  Testing.Shared.IntegrationTests/ # Shared Aspire integration-test base
  WebScrapper/                # Web scraping utility
  infrastructure/             # Low-level infrastructure scripts/configs
  infrastructure-azure-entra/ # Azure Entra-specific infrastructure configs
```

## Key Libraries At A Glance

### `Api.Shared`

- **Shared event contracts library** targeting **netstandard2.0** for cross-platform compatibility (.NET Framework, .NET Core, .NET 5+).
- Defines `IEvent` interface, `IEventExtensions` helper methods, `KafkaTopicAttribute`, `KafkaTopicHelper`, and `EventMetadataFactory`.
- Used by `Api.Shared.Clients`, `Enterprise.Shared`, and any domain that needs to consume Kafka events.
- Do not add framework-specific dependencies; keep this library portable.
- See `Api.Shared/AGENTS.md` for details.

### `Api.Shared.Clients`

- Hosts handwritten event metadata companions over protobuf-generated partial classes (references `Api.Shared` for event contracts).
- Hosts generated OpenAPI C# client wrappers.
- Targets **netstandard2.0** for cross-platform compatibility.
- Do not check in protobuf-generated `*V1Key.g.cs` / `*V1Value.g.cs` files; those are build-time outputs under `obj/`.
- See `Api.Shared.Clients/AGENTS.md` for details.

### `Api.Shared.Services`

- Hosts generated OpenAPI controller base classes consumed by domain API projects.
- Do not hand-edit files under `OpenApi/`; regenerate with `api-definitions/openapi/generate.sh`.
- See `Api.Shared.Services/AGENTS.md` for details.

### `Enterprise.Shared`

- The composable infrastructure library: Kafka, Temporal, Redis, GraphQL (HotChocolate), Stripe, Xero, security, telemetry, file storage, AI/MCP.
- Each capability is a separately adoptable module with its own `Add*`/`Use*` extension methods.
- See `Enterprise.Shared/AGENTS.md` and the per-module `AGENTS.md` files for full details.

### `Infrastructure.Shared`

- Aspire-based local infrastructure bootstrapping used by domain app hosts and integration test hosts.
- See `Infrastructure.Shared/AGENTS.md`.

### `Testing.Shared.IntegrationTests`

- Base classes and helpers for Aspire-hosted integration test projects across domains.
- See `Testing.Shared.IntegrationTests/AGENTS.md`.

### `Skedularctl``, `Api.Shared.Clients`, `Api.Shared.Services`, or `Enterprise.Shared`.

- `Api.Shared` targets netstandard2.0; do not add framework-specific dependencies.
- If updating event contracts in `Api.Shared`, rebuild `Api.Shared.Clients` and all domains that consume events.
- Generated surfaces under `Api.Shared.Clients/OpenApi/` and `Api.Shared.Services/OpenApi/` must not be patched by hand; edit the YAML and regenerate.
- Event metadata companions under `Api.Shared.Clients/Events/` are handwritten; keep them under the correct versioned directory.
- Event contract interfaces live in `Api.Shared`; do not redefine them in `Api.Shared.Clients` or domain projects
- See `Skedularctl/AGENTS.md`.

## Agent Rule

- Changes here can affect every domain. Prefer small, compatible changes.
- Check downstream domain builds after changing anything in `Api.Shared.Clients`, `Api.Shared.Services`, or `Enterprise.Shared`.
- Generated surfaces under `Api.Shared.Clients/OpenApi/` and `Api.Shared.Services/OpenApi/` must not be patched by hand; edit the YAML and regenerate.
- Event metadata companions under `Api.Shared.Clients/Events/` are handwritten; keep them under the correct versioned directory.
