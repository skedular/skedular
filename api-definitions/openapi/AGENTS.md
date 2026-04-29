# OpenAPI Definitions Agent Notes

This file applies to `api-definitions/openapi/`.

## Source Of Truth

- REST/HTTP contract source files live under `skedular/*_v1.yaml`.
- One YAML file per domain API, named `{domain}_v1.yaml`.
- Change these YAML files first. Do not hand-edit generated controller bases or client code.

## Generation Flow

Run `api-definitions/openapi/generate.sh` after any YAML change:

- Regenerates C# controller base classes under `shared/Api.Shared.Services/OpenApi/`.
- Regenerates C# API client wrappers under `shared/Api.Shared.Clients/OpenApi/`.

If the web app consumes a changed API, also run `web/apps/webapp/scripts/generate.sh` to update the TypeScript client.

Use `make generate` from the repo root to run all three generation steps in the correct order.

## Adding A New Route

1. Add or modify the route in the appropriate `skedular/*_v1.yaml` file.
2. Run `api-definitions/openapi/generate.sh`.
3. Implement the generated controller base surface in the domain API project.
4. Do not add controller routes that are not declared in the YAML first.

## Versioning Convention

- Files are named `{domain}_v1.yaml`.
- All current APIs are at v1. Introducing a v2 requires a new file and a new generated surface.

## Agent Rule

- Change YAML first; regenerate before implementing.
- Do not add route logic to a controller without a matching YAML declaration.
- Do not patch generated files under `shared/Api.Shared.Services/OpenApi/` or `shared/Api.Shared.Clients/OpenApi/` by
  hand.
