# Research: Customer Landing Cleanup

## Decision: Treat no-subdomain webapp as an aggregate marketplace layer

**Rationale**: The current webapp root already branches between public discovery and custom-domain customer-facing entry points through `customer-facing-subdomain` resolution. The aggregate marketplace should live in the no-subdomain path and reuse the existing marketplace mental model: browse locations, inspect products, book or subscribe, and manage customer-owned purchases. This satisfies the clarified requirement that custom-subdomain coworking-space owner marketplaces remain unchanged.

**Alternatives considered**:

- Redirect no-subdomain visitors to owner-specific marketplace pages: rejected because the specification explicitly forbids URL redirects in this phase.
- Merge owner-specific and aggregate marketplace routing into one generic route: rejected because it risks changing existing custom-subdomain behavior.
- Build a separate new web app: rejected because the feature is specifically about simplifying `webapp` and preserving shared account/customer entry points.

## Decision: Use existing marketplace locations and location-level product surfaces as the foundation

**Rationale**: Webapp already contains `MarketplaceLocations`, `MarketplaceLocation`, product detail, booking, subscription, booking detail, and subscription detail pages. Planning should focus on filtering eligibility, cross-organization presentation, route ownership cleanup, and customer self-service coverage rather than inventing a parallel booking model.

**Alternatives considered**:

- Use private organization booking flows for customer purchases: rejected because private booking interfaces now belong in `webapp-teams` and are not valid for aggregate customer booking.
- Build a purely content-driven public site first: rejected because the specification requires customers to reach marketplace-style product purchase paths.
- Delay booking/subscription self-service: rejected during clarification; full policy-bound customer self-service is in scope.

## Decision: Keep owner-specific custom-subdomain marketplace behavior unchanged

**Rationale**: Custom-subdomain marketplace pages currently represent an owner-specific storefront. The aggregate marketplace is an abstraction on top of the same customer-facing capability, not a redesign of those owner-specific experiences. The plan must include regression validation that custom-subdomain browse and purchase flows remain stable.

**Alternatives considered**:

- Re-skin owner-specific pages as part of the aggregate marketplace: rejected because the clarified requirement says no change at all for current customer-facing custom-subdomain marketplace behavior.
- Redirect aggregate location selections into custom-subdomain URLs: rejected because no URL redirect behavior is allowed in this phase.

## Decision: Model cleanup as a route/workflow responsibility inventory before removals

**Rationale**: Webapp still contains private/admin route trees such as MS Teams organization pages, location/resource management, admin settings, product setup, bookings, and user management. The first implementation slice needs an inventory with owner app, disposition, rationale, customer impact, and no-redirect handling before removing or hiding anything.

**Alternatives considered**:

- Remove obvious admin routes directly: rejected because the spec requires stakeholder approval and protection of customer-owned booking/subscription history.
- Keep compatibility pages for all old admin routes: rejected because that preserves the mixed app surface and slows simplification.

## Decision: Use in-place unavailable/customer-safe states instead of redirects

**Rationale**: The clarified requirement forbids URL redirects from webapp for now, including marketplace customer-facing paths. Unsupported, removed, or owner-specific paths opened under webapp must resolve in place with customer-safe messaging and no private administration controls.

**Alternatives considered**:

- Redirect known owner routes to `webapp-teams` or `webapp-spaces`: rejected by explicit clarification.
- Hard 404 removed routes: rejected because it creates a poor user/support experience and does not document customer-safe handling.

## Decision: Use existing GraphQL/Relay patterns and regenerate generated artifacts only if schema/query selections change

**Rationale**: The constitution requires contract-first/generated-code discipline. Existing webapp marketplace pages use Relay colocated queries and generated artifacts. If aggregate marketplace needs new GraphQL fields or filters for marketplace eligibility, location insights, or self-service actions, the source schema/query changes must be regenerated through the repo's established scripts.

**Alternatives considered**:

- Hand-edit generated Relay artifacts: rejected by constitution.
- Fetch cross-domain data through ad hoc REST calls from components: rejected because federated GraphQL is the primary client-facing integration path.

## Decision: Test with focused web component tests plus integration/contract checks if backend contract changes are needed

**Rationale**: This is primarily a web product-boundary and marketplace UI feature. Vitest + React Testing Library should cover route resolution, aggregate marketplace rendering, no-redirect behavior, customer-safe states, and self-service affordances. If backend GraphQL fields or mutations are added or changed, backend unit/integration tests and generated artifact validation become required.

**Alternatives considered**:

- Only manual QA: rejected by constitution and risk to owner-specific marketplace regression.
- Full end-to-end suite as the only validation: rejected as too broad for planning; use targeted high-value e2e only if tasks introduce flows that component tests cannot cover.
