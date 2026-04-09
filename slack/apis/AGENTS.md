# Slack API Agent Notes

This file covers `slack/apis/`.

## GraphQL Surface

- The Slack API exposes a HotChocolate GraphQL subgraph consumed via the federation gateway.
- Workspace connection status and channel configuration queries are surfaced here.

## OpenAPI Surface

- Any REST routes (e.g. Slack OAuth callback, event webhook ingress) should be declared in
  `api-definitions/openapi/skedular/slack_v1.yaml` first, then regenerated before implementing.
- Do not add controller routes outside of the generated controller base.

## Webhook Ingress Pattern

- Slack event webhook routes should be fast-ingress only: validate the request signature, then publish to Kafka.
- Do not perform synchronous Slack API calls in the webhook request path; delegate to `slack/processors/` or
  `slack/shared/` workflows.

## Agent Rule

- Keep API endpoints thin.
- Fix shared Slack behavior in the shared layer rather than burying logic in transport code.
- Run `scripts/generate-graphql.sh` after any schema change; do not hand-edit `schema.graphql`.
