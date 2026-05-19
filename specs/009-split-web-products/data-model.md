# Data Model: Split Web Products

This feature is frontend-only. The entities below are planning and UI-state concepts, not new backend persistence models.

## Product App

Represents one target web application.

Fields:

- `id`: `webapp`, `webapp-spaces`, or `webapp-teams`
- `purpose`: customer-facing discovery, marketplace/co-working operator workflows, or private organisation/team workflows
- `route_scope`: the routes and customer/operator entry points owned by the app
- `allowed_organisation_types`: organisation types selectable inside the app, where applicable

Rules:

- `webapp-teams` must not expose marketplace organisation concepts, marketplace products, or public discovery journeys.
- `webapp-spaces` must not expose private organisation/team-only journeys.
- `webapp` remains customer-facing and may show public root discovery, co-working subdomain experiences, and private organisation customer-facing subdomain experiences.

## Organisation Membership

Represents a user's membership in one or more organisations.

Fields:

- `user_id`: existing user identity
- `organisation_id`: existing organisation identity
- `organisation_type`: private or marketplace/co-working
- `membership_status`: existing membership state used by the current frontend

Rules:

- Multiple organisation membership remains supported.
- Teams filters selectable organisations to private organisations.
- Spaces filters selectable organisations to marketplace/co-working organisations.
- Empty selection states must provide a create or join path.

## Customer-Facing Entry Point

Represents WebApp customer entry behaviour.

Fields:

- `entry_kind`: root URL, marketplace organisation subdomain, or private organisation subdomain
- `organisation_scope`: none for root discovery, one organisation for subdomain entry
- `visible_scope`: public marketplace discovery, co-working customer-facing space/products, or private organisation customer-facing experience

Rules:

- Root URL shows public marketplace discovery.
- Co-working space subdomain shows only that space and customer-facing products.
- Private organisation subdomain shows only that organisation's customer-facing experience.

## Ownership Inventory Item

Represents a route, page, component, hook, utility, provider, query, generated artefact, or shared surface being classified.

Fields:

- `item_path`: current repository path or route
- `item_type`: route, page, component, hook, utility, provider, query, generated artefact, configuration, or documentation
- `current_owner`: current app/package location
- `target_owner`: WebApp, WebApp Spaces, WebApp Teams, `@skedular/ui`, `@skedular/shared`, or transition path
- `reason`: why the target owner is correct
- `backend_return_url_risk`: yes/no/unknown
- `relay_impact`: yes/no/unknown
- `tests_required`: affected test scope
- `transition_condition`: required only for temporary adapters, duplicate modules, or redirects

Rules:

- Every targeted item must have exactly one target owner or a documented transition path.
- Broad feature modules and app-specific rules cannot move into shared foundations solely because more than one app has similar behaviour.

## Migration Slice

Represents one reviewable unit of migration.

Fields:

- `slice_id`: stable label for planning and review
- `target_app`: WebApp, WebApp Spaces, WebApp Teams, or shared foundation
- `journey`: user-visible flow or tightly related journey group
- `inventory_items`: ownership inventory items included in the slice
- `route_retirement_plan`: keep, redirect, block, delete, or transition
- `return_url_audit`: backend-originated return URL check result
- `verification_commands`: lint, tests, build, Relay generation/checks
- `manual_review_notes`: what the user should inspect before the next slice
- `status`: proposed, in progress, ready for review, accepted, or blocked

State transitions:

```text
proposed -> in progress -> ready for review -> accepted
                              |
                              v
                           blocked
```

Rules:

- A slice cannot be ready for review until route retirement and return URL risk are addressed.
- A slice cannot be accepted until the user has had a chance to inspect the moved journey.

## Backend-Originated Return URL

Represents a frontend URL that backend-driven flows may redirect to.

Fields:

- `source`: payment, authentication, notification, external callback, or other backend-driven flow
- `current_url`: existing route or base URL
- `target_app_url`: target app route/base URL after migration
- `configuration_source`: environment variable, existing frontend config, or documented backend assumption
- `retirement_action`: keep, redirect, block, delete later, or configure target app URL

Rules:

- Routes with return URL usage cannot be deleted until a target-app URL strategy exists.
- The strategy must not require backend service, API contract, or backend data ownership changes for this feature.
