# Core API Agent Notes

This file covers `core/apis/`.

## Purpose

- `Core.Api` is the HTTP/GraphQL entry point for the core domain.
- It serves cross-cutting capabilities including user/member management, file management, and platform-level settings.
- Authentication context is replicated into other domains from here.

## GraphQL Surface

- The core API exposes a HotChocolate GraphQL subgraph consumed via the federation gateway.
- Follow the GraphQL choice types pattern for any selectable enum values: expose `...Details` types with `type` and
  `name`, and provide a query field returning available choices.

## OpenAPI Surface

- REST routes split across three specs in `api-definitions/openapi/skedular/core/`:
  - `core_v1.yaml` — business operations (version, file services)
  - `core_graphql_v1.yaml` — GraphQL change notification
  - `core_workaround_v1.yaml` — workaround/admin/maintenance (currently unused)
- Declare new routes in the appropriate spec, then regenerate via `bash api-definitions/openapi/generate.sh`.
- Do not add controller routes outside of the generated controller base.

## Agent Rule

- Keep transport code thin.
- If the issue is about core shared behavior, fix it in `core/shared/` rather than only in the API edge.
- Do not make request-time cross-domain calls for state that should be replicated or precomputed locally.
- Run `scripts/generate-graphql.sh` after any schema change; do not hand-edit `schema.graphql`.
- Changes here can affect all other domains since core handles cross-cutting auth and identity concerns.
