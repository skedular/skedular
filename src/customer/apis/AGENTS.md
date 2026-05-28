# Customer API Agent Notes

This file covers `customer/apis/`.

## GraphQL Surface

- The customer API exposes a HotChocolate GraphQL subgraph consumed via the federation gateway.
- Customer identity, profile, and authentication-related queries and mutations are surfaced here.

## OpenAPI Surface

- Any REST routes should be declared in `api-definitions/openapi/skedular/customer_v1.yaml` first, then regenerated.
- Do not add controller routes outside of the generated controller base.

## Agent Rule

- Keep API code thin.
- If the bug is about customer persistence or shared semantics, the fix is usually in `customer/shared/`.
- Customer identity changes can cascade to other domains that replicate this state for authorization — be careful.
- Run `scripts/generate-graphql.sh` after any schema change; do not hand-edit `schema.graphql`.
