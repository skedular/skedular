# Research: Split Web Products

## Decision: Foundation-first delivery

**Decision**: Build a usable foundation for `webapp`, `webapp-spaces`, and `webapp-teams` before moving feature journeys.

**Rationale**: The user needs to inspect each app before feature slices move. Empty target apps make it hard to validate whether a migrated journey is broken because of the journey itself or because the app shell is incomplete.

**Alternatives considered**:

- Move features first into empty apps: rejected because it creates high review risk and makes regressions hard to isolate.
- Complete all foundations and all feature moves in one large pass: rejected because this migration requires careful manual checking and would create a high-blast-radius change.

## Decision: One reviewable migration slice at a time

**Decision**: After foundation, migrate one journey or tightly related journey group at a time, then stop for review before the next slice.

**Rationale**: The split is high risk and user-facing. A slice-by-slice loop makes ownership, route retirement, and verification explicit, while keeping review small enough for manual confirmation.

**Alternatives considered**:

- App-by-app full migration: rejected because it would move too much before feedback.
- File-type migration, such as all components then all routes: rejected because it does not produce user-reviewable behaviour.

## Decision: Shared code boundary

**Decision**: Put neutral visual foundations in `@skedular/ui`, neutral hooks/utilities/providers in `@skedular/shared`, and keep app-specific rules, copy, permissions, navigation, and workflow orchestration in the owning app.

**Rationale**: This matches the constitution and prevents the shared layer from becoming a new mixed-product dumping ground.

**Alternatives considered**:

- Share only UI primitives and duplicate all hooks/utilities: rejected because it increases drift for neutral runtime behaviour.
- Share broad feature modules across apps: rejected because it blurs product boundaries and makes Teams vulnerable to marketplace concepts.

## Decision: Backend unchanged with frontend URL dependency audit

**Decision**: Do not change backend services, APIs, data contracts, or backend ownership. Before retiring a web route, audit backend-originated redirects/callbacks/base URL assumptions that target the route and provide a target-app URL strategy.

**Rationale**: Backend behaviour already drives payment/auth/callback flows back to known frontend URLs. Deleting a route without checking those return paths would break user flows while still appearing frontend-only.

**Alternatives considered**:

- Ignore backend return URL references during route retirement: rejected because payment and callback flows can break.
- Redesign backend redirect ownership as part of this feature: rejected because backend changes are explicitly out of scope.

## Decision: App-filtered organisation selection

**Decision**: Preserve multiple organisation membership but filter selectable organisations by app. Teams shows private organisations; Spaces shows marketplace/co-working organisations; WebApp remains customer-facing for public discovery and subdomain-specific experiences.

**Rationale**: The same user can belong to multiple organisations, but each app has a distinct purpose. Filtering keeps Teams free of marketplace product concepts and Spaces free of private organisation workflows.

**Alternatives considered**:

- Global organisation selector shared across all apps: rejected because it would surface irrelevant organisation types in each app.
- Separate membership models per app: rejected because existing platform membership remains valid and backend ownership is unchanged.

## Decision: App-switching is optional for early slices

**Decision**: App-switching navigation may be introduced later, but each app must remain accessible directly by URL and usable without a switcher.

**Rationale**: The user is unsure whether to enable a switcher in the first phase. Direct URL access preserves progress and avoids blocking the foundation on a nonessential navigation decision.

**Alternatives considered**:

- Require switcher before migration: rejected because it blocks foundation and first slices.
- Forbid switcher entirely: rejected because multi-app users likely need it eventually.
