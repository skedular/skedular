# Research: Skedular Teams Pricing Catalog Redesign

## Decision: Extend the Existing V1 Organization Offering Model

**Rationale**: The current repository already has `OfferingCode`, `Offerings`, `OrganizationOffering`, active offering GraphQL details, offering renewal workflows, Free limits, Pay As You Go, Enterprise Custom, and Early Bird. The feature brief explicitly says not to introduce a new version unless there is a clear reason. Extending V1 lets the feature preserve existing Free/Early Bird behavior, avoid touching current subscriptions, and add catalog-shaped read models around current concepts.

**Alternatives considered**:

- Introduce Pricing Catalog V2 immediately. Rejected for this feature because the current model can represent the initial Teams plan set with additive fields and mappings, and V2 would increase coexistence work before a proven need.
- Reuse marketplace `ProductPricing` as the Teams commercial model. Rejected because marketplace product pricing is product/resource sale pricing for Spaces workflows, while Teams pricing is organization subscription and entitlement state.

## Decision: Organization Owns Teams Subscription and Catalog State

**Rationale**: The existing Organization domain already owns organization offerings, billing connection state, default invoice terms, and organization-facing subscription behavior. Team and Location enforce limits based on organization state, while Booking participates in active-user qualification. Keeping Teams subscription state in Organization respects domain ownership and prevents direct database coupling.

**Alternatives considered**:

- Put catalog ownership in Booking because Booking already has product pricing. Rejected because Teams subscription is organization-level, not marketplace booking-level.
- Put catalog ownership in a new pricing domain. Deferred because the initial redesign can evolve existing Organization ownership without adding a service boundary.

## Decision: Model Enterprise Capacity as One Contact Us Plan With Admin-Negotiated Terms

**Rationale**: The public pricing page should stay simple: Free, Pay As You Go, and Enterprise Contact Us. Enterprise terms are negotiated by Skedular, then stored on the organization’s existing `OrganizationOffering` row as per-active-user unit price, currency, and purchased active-user capacity. This preserves stable plan semantics without exposing negotiated prices publicly.

**Alternatives considered**:

- Separate plan codes for each enterprise size. Rejected because it contradicts the commercial model and would make future capacity changes require plan proliferation.
- Public self-service 100, 500, and 1,000 user packages. Rejected because negotiated Enterprise pricing should not be exposed on the public website.

## Decision: Keep Early Bird Unchanged

**Rationale**: Clarification confirmed existing Early Bird organizations should not be touched. Early Bird remains honored for existing organizations and must not be modified. It can remain hidden from public catalog purchase flows while continuing to appear where existing organization subscription state must be shown.

**Alternatives considered**:

- Migrate Early Bird to Free, Pay As You Go, or Enterprise Capacity. Rejected by user clarification.

## Decision: Shared Entitlement Code With Existing Event/JSON Projections

**Rationale**: The feature needs consistent entitlement results across Organization, Booking, Team, and Location. The existing pattern is that Organization publishes pricing/subscription state through events, other domains store that projected state locally as a JSON block/projection, and enforcement logic lives in shared `Api.Shared.Services` models/code. Enhancing that pattern preserves domain boundaries and avoids runtime calls back to Organization.

**Alternatives considered**:

- Duplicate independent business rules in each domain. Rejected because the rules should live in shared `Api.Shared.Services` code even though each domain executes them locally against its projected JSON state.
- One shared service with direct access to every domain database. Rejected because it violates domain boundaries and integration-test persistence rules.
- Runtime calls from every domain back to Organization. Rejected because the existing architecture uses event-projected organization state and local enforcement.

## Decision: Active User Qualification Is Event/Workflow Friendly

**Rationale**: Monthly active users are unique organization users per billing period who perform qualifying actions. The minimum qualifying actions are booking creation, booking update, booking ownership, and participation in confirmed bookings. Capturing qualification through a repository-backed Organization service with public/event-driven inputs keeps entitlement checks deterministic and auditable.

**Alternatives considered**:

- Count all organization members as active. Rejected because the feature defines active usage, not membership.
- Count only sign-ins. Rejected because the brief calls out booking-related meaningful usage.

## Decision: Public-Web Pricing Must Stop Owning Commercial Values

**Rationale**: `src/web/apps/public-web/src/data/pricing.ts` currently hardcodes Teams and Spaces prices, plan names, summaries, and tiers. The feature requires plan names, ordering, visibility, features, prices, capacity options, Contact Us behavior, and recommendations to come from backend catalog data. Public-web can render from a runtime catalog response or a generated/static build-time catalog artifact, but the source of truth must be backend-owned catalog data.

**Alternatives considered**:

- Keep static public-web pricing data and manually sync it. Rejected because the spec explicitly forbids hardcoded pricing information in frontend code.

## Decision: Contract Changes Require GraphQL Regeneration and Possibly OpenAPI Regeneration

**Rationale**: The primary client-facing integration surface is GraphQL/Fusion. New catalog queries and subscription lifecycle mutations must update GraphQL schema outputs through `scripts/generate-graphql.sh`. If OpenAPI organization endpoints are added for public/static pricing consumption, update `api-definitions/openapi/skedular/organization/*.yaml`, run `api-definitions/openapi/generate.sh`, and regenerate web clients if consumed by web apps.

**Alternatives considered**:

- Hand-edit generated schemas or Relay files. Rejected by repository constitution and AGENTS rules.
