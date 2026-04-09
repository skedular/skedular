# MsTeams API Agent Notes

This file covers `msteams/apis/`.

## GraphQL Surface

- The MsTeams API exposes a HotChocolate GraphQL subgraph consumed via the federation gateway.
- Azure tenant connection status and Teams channel configuration queries are surfaced here.

## OpenAPI Surface

- Any REST routes (e.g. Teams webhook ingress, OAuth callback) should be declared in
  `api-definitions/openapi/skedular/msteams_v1.yaml` first, then regenerated before implementing.
- Do not add controller routes outside of the generated controller base.

## Webhook Ingress Pattern

- Teams event webhook routes should be fast-ingress only: validate the request, then publish to Kafka.
- Do not perform synchronous Microsoft Teams API calls in the webhook request path; delegate to
  `msteams/processors/` or `msteams/shared/` workflows.

## Agent Rule

- Keep API code thin.
- If the issue is about Teams integration logic, fix shared/domain code instead of only transport.
- External Microsoft Teams API contracts can break production integrations quickly — prefer behavior-preserving changes.
- Run `scripts/generate-graphql.sh` after any schema change; do not hand-edit `schema.graphql`.
