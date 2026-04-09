# API Definitions Agent Notes

This file is the entry point for AI agents working in `api-definitions/`.

## Purpose

- `api-definitions/` is the single source of truth for all cross-service contracts in this repository.
- It contains four kinds of contracts, each with its own generator and consumer shape.

## Contract Types

| Subdirectory | Contract format   | Generator                                      | Primary consumers                                                |
|--------------|-------------------|------------------------------------------------|------------------------------------------------------------------|
| `events/`    | Protobuf (`.proto`) | `api-definitions/events/generate.sh` (builds `shared/Api.Shared.Clients`) | Kafka producer/consumer code in all domain processors and jobs |
| `openapi/`   | OpenAPI YAML       | `api-definitions/openapi/generate.sh`          | C# controller bases (`shared/Api.Shared.Services/OpenApi`) and C# API clients (`shared/Api.Shared.Clients/OpenApi`) |
| `graphql/`   | GraphQL schema     | `scripts/generate-graphql.sh`                  | HotChocolate Fusion gateway, Relay web artifacts                 |
| `grpc/`      | Protobuf (`.proto`) | Built by consuming `.csproj` at build time    | gRPC clients/stubs compiled by each consuming service            |

## Golden Rule

**Change the source definition file first, then regenerate. Never patch generated outputs by hand.**

- For events: edit `events/skedular/*.proto`, then build `shared/Api.Shared.Clients`.
- For OpenAPI: edit `openapi/skedular/*_v1.yaml`, then run `api-definitions/openapi/generate.sh`.
- For GraphQL: edit the server-side schema definition in the relevant domain API, then run `scripts/generate-graphql.sh`.
- For gRPC: edit `grpc/skedular/*.proto`; generated C# stubs appear at build time in consuming projects.

## Preferred Umbrella Entry Point

`make generate` runs, in order:
1. `api-definitions/generate.sh` (events + OpenAPI)
2. `scripts/generate-graphql.sh` (GraphQL subgraph export, composition, web relay)
3. `web/apps/webapp/scripts/generate.sh` (web TypeScript API clients)

Run `make generate` from the repo root when multiple surfaces may be affected.

## Where To Read Next

- `api-definitions/events/AGENTS.md`
- `api-definitions/openapi/AGENTS.md`
- `api-definitions/graphql/AGENTS.md`
- `api-definitions/grpc/AGENTS.md`

## Agent Rule

- Any change under `api-definitions/` almost certainly requires regeneration.
- If a generated file changed unexpectedly, trace back to the source definition rather than patching the output.
