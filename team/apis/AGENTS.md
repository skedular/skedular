# Team API Agent Notes

This file covers `team/apis/`.

## GraphQL Surface

- The team API exposes a HotChocolate GraphQL subgraph consumed via the federation gateway.
- Team membership, invitation, and team-location association queries and mutations are surfaced here.
- Follow the GraphQL choice types pattern for any selectable enum values.

## OpenAPI Surface

- Any REST routes should be declared in `api-definitions/openapi/skedular/team_v1.yaml` first, then regenerated.
- Do not add controller routes outside of the generated controller base.

## Agent Rule

- Keep API surfaces thin.
- If the bug is about team data semantics, fix shared/domain logic instead of only patching transport.
- Do not reintroduce `hasFutureBooking` to team API surfaces.
- Do not reintroduce booking-derived read models into `team/apis/`; team booking questions belong to the booking domain.
- Run `scripts/generate-graphql.sh` after any schema change; do not hand-edit `schema.graphql`.
