# GraphQL Schema Definitions Agent Notes

This file applies to `api-definitions/graphql/`.

## What Lives Here

- `skedular/v1/schema.graphql` — the composed federation schema for the entire platform.
- This file is the output of `scripts/generate-graphql.sh`; it is **generated**, not hand-edited.

## How The Schema Is Built

The generation script (`scripts/generate-graphql.sh`) does the following in order:

1. For each domain API, runs `dotnet run -- schema export --output schema.graphqls` to export the subgraph schema.
2. Runs `dotnet nitro fusion compose` to produce the composed `gateway.far` and the composed `schema.graphql` here.
3. Updates GraphQL init/schema files used by integration and system tests.
4. Updates the server-side GraphQL schema surfaces that web Relay artifacts depend on.

## Subgraph Schema Locations

Each domain API owns its own `schema.graphql` file adjacent to the API project:

- `booking/apis/Booking.Api/schema.graphqls`
- `core/apis/Core.Api/schema.graphqls`
- `customer/apis/Customer.Api/schema.graphqls`
- `location/apis/Location.Api/schema.graphqls`
- `marketplace/apis/Marketplace.Api/schema.graphqls`
- `msteams/apis/MsTeams.Api/schema.graphqls`
- `organization/apis/Organization.Api/schema.graphqls`
- `slack/apis/Slack.Api/schema.graphqls`
- `team/apis/Team.Api/schema.graphqls`

These are also generated outputs. Do not edit them by hand.

## Agent Rule

- Never hand-edit `api-definitions/graphql/skedular/v1/schema.graphql` or any per-API `schema.graphqls` file.
- After any backend GraphQL type, field, enum, or resolver change, run `scripts/generate-graphql.sh`.
- After that, if the change affects fields consumed by the web app, regenerate the Relay artifacts too by running `web/apps/webapp/scripts/generate.sh` (or use `make generate`).
- Always run the full pipeline with `make generate` when the scope is uncertain.
