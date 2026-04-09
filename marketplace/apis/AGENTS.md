# Marketplace API Agent Notes

This file covers `marketplace/apis/`.

## GraphQL Surface

- The marketplace API exposes a HotChocolate GraphQL subgraph consumed via the federation gateway.
- Product listing queries, pricing choices, and marketplace purchase flows are exposed here.
- Follow the GraphQL choice types pattern: expose selectable enum values via a `...Details` type with `type` and `name`
  fields, and provide a query that returns available choices for UI controls.

## OpenAPI Surface

- Any REST routes that belong to the marketplace API should be declared in
  `api-definitions/openapi/skedular/marketplace_v1.yaml` first, then regenerated before implementing.
- Do not add controller routes outside of the generated controller base.

## GraphQL Schema Changes

- After any GraphQL type or field change in the marketplace API, run `scripts/generate-graphql.sh` to recompose the
  gateway schema and update Relay artifacts.

## Agent Rule

- Keep API endpoints thin.
- If the issue is about pricing or listing semantics, the fix likely belongs in `marketplace/shared/`.
- Do not encode pricing-cadence or booking-cadence rules in the API layer; those belong in shared logic and ultimately
  in the booking domain for invoice behavior.
- Do not reintroduce booking-derived state to marketplace API surfaces.
- Run `scripts/generate-graphql.sh` after any schema change; do not hand-edit `schema.graphql`.
