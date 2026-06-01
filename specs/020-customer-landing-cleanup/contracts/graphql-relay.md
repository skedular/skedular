# Contract: GraphQL And Relay Boundaries

## Purpose

Documents the expected client-facing data contract boundaries for aggregate marketplace implementation. This is a planning contract; exact GraphQL fields should follow existing schema patterns during implementation.

## Existing Surfaces To Prefer

Use existing marketplace GraphQL/Relay surfaces where they already satisfy the feature:

- Marketplace location discovery query/fragments used by `MarketplaceLocations`.
- Marketplace location detail query/fragments used by `MarketplaceLocation`.
- Marketplace product detail, booking, subscription, booking detail, and subscription detail surfaces.
- Existing customer-facing booking/subscription policy and status fields when available.

## Potential Contract Needs

If existing schema fields are insufficient, planning should add contract work for:

- Marketplace eligibility filtering across organizations.
- Customer-bookable location indicators.
- Customer-facing location insight fields.
- Cross-organization customer booking and subscription summaries.
- Policy-derived eligible self-service actions for bookings and subscriptions.
- Customer-safe unavailable action reasons.

## Generation Rules

- Do not hand-edit Relay artifacts under `src/web/apps/webapp/src/queries/__generated__/`.
- Do not hand-edit generated API clients under `src/web/apps/webapp/src/clients/`.
- If backend GraphQL schema changes are required, run `scripts/generate-graphql.sh` and regenerate web Relay artifacts through the established generation flow.
- If OpenAPI contracts change and webapp consumes them, run `src/web/apps/webapp/scripts/generate.sh` or the repo-level `make generate` umbrella command.

## Testing Rules

- Relay queries/fragments must stay colocated with the components that consume them.
- Web tests should mock or provide Relay data through existing test utilities rather than coupling to generated artifact internals.
- Backend contract changes require backend tests at the owning domain boundary and web generated artifact validation.
