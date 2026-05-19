# Feature Specification: Split Web Products

**Feature Branch**: `009-split-web-products`  
**Created**: 2026-05-19  
**Status**: Ready for Planning  
**Input**: User description: "The current web product contains the complete implementation for private organisations, marketplace organisations, and customer-facing journeys in one app. Start splitting that product into three apps: WebApp for customer-facing discovery and booking across public locations and products, WebApp Spaces for co-working spaces and marketplace organisations, and WebApp Teams for enterprise/private organisations. This is a large migration: analyse what belongs in each app, move product-specific experiences into each app's own area, and move genuinely common components into a shared component, shared UI, or design-system foundation."

## Clarifications

### Session 2026-05-19

- Q: Does the product split include backend service or API changes? → A: No, web application only; backend stays unchanged.
- Q: What boundary should shared code follow during the split? → A: Share UI primitives plus product-neutral hooks/utilities; keep product rules in each app.
- Q: How should old WebApp routes be retired during migration? → A: Retire routes per completed slice with redirects or blocks, but only after backend-originated redirects/callbacks that target those routes have a target-app URL strategy.
- Q: Does WebApp Teams include marketplace product or marketplace concepts? → A: No, Teams is for private organisation and team workflows only.
- Q: How should users with multiple organisation memberships see organisations across the three apps? → A: Preserve multiple organisation membership, but filter selectable organisations by app: Teams shows private organisations, Spaces shows marketplace/co-working organisations, and WebApp remains customer-facing with public and subdomain-specific experiences.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Classify Existing Product Journeys (Priority: P1)

A product owner and web developer need a clear classification of the current mixed web application journeys so every page, workflow, and reusable surface is assigned to WebApp, WebApp Spaces, WebApp Teams, or a shared layer before extraction begins.

**Why this priority**: Product separation cannot be delivered safely while ownership remains implicit. Classification is the foundation for deciding what moves, what stays, and what becomes shared.

**Independent Test**: Review a representative set of current web journeys and confirm each one has exactly one target ownership decision or a documented temporary exception with a reason and removal condition.

**Acceptance Scenarios**:

1. **Given** the current web application contains customer, marketplace, and private-organisation journeys, **When** the classification review is completed, **Then** every targeted journey is assigned to WebApp, WebApp Spaces, WebApp Teams, or shared ownership.
2. **Given** a journey includes mixed concerns, **When** it is classified, **Then** the app-specific parts and shared parts are identified separately rather than assigning the whole mixed journey to a shared bucket.
3. **Given** a developer needs to place future work, **When** they consult the ownership guidance, **Then** they can determine the correct target app or shared layer without relying on prior tribal knowledge.

---

### User Story 2 - Move Product-Owned Code to Target Apps (Priority: P1)

A web developer needs app-owned pages, modules, navigation, and supporting behaviour to live in the owning web app area so WebApp, WebApp Spaces, and WebApp Teams can evolve as separate apps rather than as one mixed web codebase.

**Why this priority**: The desired outcome is not only a conceptual split. The current mixed web application must be physically separated into the three web app products, with only common foundations left shared.

**Independent Test**: Select a classified web journey, move its app-owned surfaces into the target web app area, verify the journey works from that app, and verify the old mixed web location no longer owns the journey.

**Acceptance Scenarios**:

1. **Given** a journey is classified as belonging to one app, **When** it is migrated, **Then** its app-owned pages, navigation, copy, permissions, and orchestration live with that app.
2. **Given** supporting components are used only by one app, **When** the journey is migrated, **Then** those supporting components move with the owning app rather than remaining in a shared location.
3. **Given** supporting components are genuinely common across apps, **When** the journey is migrated, **Then** those components move to or remain in an approved shared component foundation and are consumed by the apps that need them.

---

### User Story 3 - Extract Customer-Facing Experiences to WebApp (Priority: P1)

A customer needs the customer-facing WebApp to provide public discovery, landing, location browsing, product exploration, and booking entry journeys without exposing private organisation or marketplace administration experiences.

**Why this priority**: The customer-facing surface is the public product entry point and must become cleanly separated from organisation administration concerns.

**Independent Test**: A tester can open the customer-facing WebApp, complete the primary discovery flow from landing entry to viewing locations and products, and verify that private and marketplace management journeys are not reachable from that product surface.

**Acceptance Scenarios**:

1. **Given** a visitor lands on a public co-working space page, **When** they browse that page, **Then** they see only customer-relevant space, location, product, and booking information.
2. **Given** a visitor opens the general customer-facing URL, **When** they browse available locations, **Then** they can discover and inspect public locations and products across the marketplace.
3. **Given** a customer-facing user navigates the WebApp, **When** they use menus, links, or direct route entry, **Then** private organisation and marketplace administration experiences are not exposed as customer WebApp journeys.
4. **Given** a visitor opens the general customer-facing URL without a subdomain, **When** the landing experience loads, **Then** they see public discovery across marketplace-available locations.
5. **Given** a visitor opens a co-working space subdomain, **When** the customer-facing space experience loads, **Then** they see that co-working space and its customer-facing products.
6. **Given** a visitor opens a private organisation subdomain, **When** the customer-facing private organisation experience loads, **Then** they see only the customer-facing experience built for that private organisation and are not given access to other organisations.

---

### User Story 4 - Extract Marketplace Organisation Experiences to WebApp Spaces (Priority: P1)

A co-working space or marketplace organisation operator needs WebApp Spaces to own the experiences used to manage public space presence, marketplace-facing locations, products, and related operational workflows.

**Why this priority**: Marketplace organisation functionality has a different audience, purpose, and operating model from both customer discovery and private enterprise administration.

**Independent Test**: A marketplace organisation operator can access the Spaces app, complete a representative management flow for their public marketplace presence, and verify that unrelated private organisation journeys are absent.

**Acceptance Scenarios**:

1. **Given** a marketplace organisation operator signs in to WebApp Spaces, **When** they access their workspace, **Then** they see experiences for managing marketplace-facing space, location, product, and operational content.
2. **Given** marketplace organisation functionality currently exists inside the mixed web application, **When** it is extracted, **Then** the equivalent operator workflow is available from WebApp Spaces with no customer-facing behaviour regression.
3. **Given** a marketplace operator uses WebApp Spaces, **When** they navigate the product, **Then** private enterprise-only journeys and generic customer browsing journeys are not presented as Spaces-owned workflows.
4. **Given** a signed-in user belongs to multiple organisations, **When** they use WebApp Spaces, **Then** only marketplace or co-working space organisations they belong to are selectable.
5. **Given** a signed-in user belongs to no marketplace or co-working space organisations, **When** they use WebApp Spaces, **Then** they cannot select an organisation and are offered the appropriate path to create or join one.

---

### User Story 5 - Extract Enterprise and Private Organisation Experiences to WebApp Teams (Priority: P1)

An enterprise or private organisation user needs WebApp Teams to own private organisation setup, team-facing administration, and member workflows without marketplace organisation management, marketplace product concepts, or public customer discovery concerns.

**Why this priority**: Private organisation workflows serve a separate app with different expectations from marketplace spaces and public customers.

**Independent Test**: A private organisation user can access the Teams app, complete a representative organisation or team workflow, and verify that public marketplace and customer discovery surfaces are absent.

**Acceptance Scenarios**:

1. **Given** a private organisation user signs in to WebApp Teams, **When** they access their workspace, **Then** they see private organisation and team-specific experiences only.
2. **Given** private organisation functionality currently exists inside the mixed web application, **When** it is extracted, **Then** the equivalent workflow is available from WebApp Teams with no approved private-organisation behaviour lost.
3. **Given** a Teams user navigates the app, **When** they use menus, links, or direct route entry, **Then** marketplace organisation administration, marketplace product management, and public customer discovery journeys are not presented as Teams-owned workflows.
4. **Given** a signed-in user belongs to multiple organisations, **When** they use WebApp Teams, **Then** only private organisations they belong to are selectable.
5. **Given** a signed-in user belongs to no private organisations, **When** they use WebApp Teams, **Then** they cannot select an organisation and are offered the appropriate path to create or join one.

---

### User Story 6 - Preserve and Share Common Product Foundations (Priority: P2)

A design system maintainer and web developer need genuinely common components, interaction patterns, and visual foundations to remain shared so the three apps do not duplicate stable common behaviour or drift visually.

**Why this priority**: Shared foundations reduce maintenance cost, but they must be introduced after product ownership is clear so shared code does not become a new mixed-product dumping ground.

**Independent Test**: A developer can identify a repeated cross-app UI or behaviour, move it into the approved shared layer, consume it from at least two apps, and verify each app still presents its app-specific content and behaviour correctly.

**Acceptance Scenarios**:

1. **Given** a component or pattern is used by more than one app, **When** it has no app-specific copy, permissions, or business rules, **Then** it is eligible for the shared UI or design-system layer.
2. **Given** a repeated module includes app-specific decisions, **When** it is reviewed for sharing, **Then** only the genuinely common foundation is shared and the app-specific behaviour remains in the owning app.
3. **Given** a shared component is consumed by multiple apps, **When** one app configures it for its own journey, **Then** the configuration does not change the behaviour or presentation of the other apps.

---

### User Story 7 - Maintain Transition Safety (Priority: P2)

A maintainer needs the split to proceed incrementally while keeping existing customer, marketplace, and private organisation workflows verifiable at each completed extraction slice.

**Why this priority**: This is a high-blast-radius refactor. Incremental verification prevents the product split from creating hidden regressions across audiences.

**Independent Test**: After each extraction slice, a maintainer can run the agreed verification path and confirm affected user journeys still pass in their target product, with temporary exceptions documented.

**Acceptance Scenarios**:

1. **Given** a journey is moved from the mixed web application to its target app, **When** verification is completed, **Then** the moved journey works in the target app and no stale entry point remains in the wrong product unless explicitly documented as transitional.
2. **Given** a shared dependency is introduced or changed, **When** affected apps are verified, **Then** all consuming apps retain their expected app-specific behaviour.
3. **Given** an extraction cannot be completed in one slice, **When** temporary duplication or adapters remain, **Then** they are documented with an owner, reason, and removal condition.
4. **Given** an existing backend-originated redirect or callback targets a WebApp route, **When** that route is considered for retirement, **Then** the route is retained or replaced only after the callback can return users to the correct target app URL.

### Edge Cases

- A current page combines customer browsing with organisation management controls in one experience.
- A component appears visually common but contains app-specific copy, permissions, or business decisions.
- A route or navigation item is reachable by direct URL even after it has been removed from the visible navigation for a product.
- A payment, authentication, notification, or external callback flow returns to a route that is being moved or retired.
- One user can belong to both private and marketplace organisations and must see only the organisation types relevant to the current app.
- A user has access to no organisations of the type required by the current app.
- An app switcher may be introduced later, but users can still access each app directly by URL.
- A shared component change affects all three apps even though only one app extraction slice was intended.
- A feature has no clear immediate target because it supports platform administration or cross-product account management.
- A temporary compatibility path is needed while customers or operators migrate bookmarks or active sessions to the new app.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: System MUST maintain an ownership inventory for the targeted current web journeys, pages, navigation entries, components, hooks, utilities, and shared surfaces.
- **FR-002**: System MUST classify each targeted item as WebApp, WebApp Spaces, WebApp Teams, shared design/UI foundation, shared application foundation, or documented temporary exception.
- **FR-003**: System MUST define WebApp as the customer-facing product for public landing pages, public co-working space pages, cross-location discovery, location browsing, product exploration, and booking entry journeys.
- **FR-004**: System MUST define WebApp Spaces as the product for co-working space and marketplace organisation operator experiences, including management of public marketplace presence, locations, products, and related operational workflows.
- **FR-005**: System MUST define WebApp Teams as the app for enterprise and private organisation experiences, including private organisation administration, team workflows, and member-facing private organisation journeys.
- **FR-006**: System MUST NOT introduce marketplace organisation concepts, marketplace product concepts, or public customer discovery journeys into WebApp Teams.
- **FR-007**: System MUST extract app-specific web experiences from the current mixed web application into the owning app without changing approved user-facing behaviour.
- **FR-008**: System MUST move app-owned pages, navigation entries, components, hooks, utilities, and workflow orchestration into the owning app area when they are not genuinely shared.
- **FR-009**: System MUST remove, redirect, or block stale access paths from the wrong app once a journey has been extracted, except where a documented transition path is intentionally retained.
- **FR-010**: System MUST move common visual primitives, design tokens, layout foundations, and neutral UI components into the appropriate shared component, shared UI, or design-system layer.
- **FR-011**: System MUST move neutral hooks, utilities, and reusable behavioural foundations into an approved shared application layer rather than duplicating them across apps.
- **FR-012**: System MUST keep app-specific copy, permissions, navigation decisions, business rules, and workflow orchestration inside the owning app rather than in shared component foundations.
- **FR-013**: System MUST NOT move broad feature modules or app-specific rules into shared foundations solely because multiple apps need similar behaviour.
- **FR-014**: System MUST document allowed transitional adapters, duplicated modules, redirects, or compatibility paths with an owner, reason, affected apps, and removal condition.
- **FR-015**: System MUST preserve existing authentication and account-entry expectations while ensuring post-authentication users land in the correct app experience for their organisation and role.
- **FR-016**: System MUST identify backend-originated redirects, callbacks, base URL references, and return URL assumptions before retiring or deleting any web route they target.
- **FR-017**: System MUST provide a target-app URL strategy for backend-originated redirects and callbacks that need to return users to WebApp, WebApp Spaces, or WebApp Teams after a route moves.
- **FR-018**: System MUST preserve the ability for one user to belong to multiple organisations.
- **FR-019**: System MUST filter organisation selection by app: WebApp Teams shows only private organisations, and WebApp Spaces shows only marketplace or co-working space organisations.
- **FR-020**: System MUST provide an empty organisation-selection state when the signed-in user has no organisations of the type required by the current app, including an appropriate path to create or join one.
- **FR-021**: System MUST keep WebApp customer-facing: the root URL shows public marketplace discovery, a co-working space subdomain shows that space and customer-facing products, and a private organisation subdomain shows only that private organisation's customer-facing experience.
- **FR-022**: System MAY provide app-switching navigation between WebApp, WebApp Spaces, and WebApp Teams, but direct URL access to each app MUST remain valid even when app-switching navigation is not yet available.
- **FR-023**: System MUST update ownership guidance so future web work can be placed in WebApp, WebApp Spaces, WebApp Teams, shared components, shared UI, design-system foundations, or shared application foundations consistently.
- **FR-024**: System MUST verify each completed extraction slice against the affected customer, marketplace organisation, and private organisation journeys before that slice is considered complete.
- **FR-025**: System MUST avoid introducing new user-facing American English copy while moving or updating user-facing text.
- **FR-026**: System MUST keep backend services, backend APIs, backend data contracts, and backend ownership unchanged as part of this feature.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: App routing, app selection, and transition paths MUST emit structured diagnostics that identify the selected app surface and reason without exposing sensitive organisation or user data.
- **LOG-002**: Extraction verification workflows MUST produce actionable output that identifies the affected app and journey when a moved surface fails.
- **LOG-003**: Existing user-facing workflow diagnostics MUST be preserved for moved journeys unless a documented extraction change replaces them with equivalent diagnostics.
- **LOG-004**: Shared foundations introduced by the split MUST preserve correlation context used by affected journeys for support and troubleshooting.

### Key Entities _(include if feature involves data)_

- **Product App**: One of the three target web applications: WebApp, WebApp Spaces, or WebApp Teams. This term does not mean a marketplace bookable product.
- **Marketplace Product**: A customer-facing or marketplace organisation offering that belongs to WebApp or WebApp Spaces journeys, not WebApp Teams.
- **Product Journey**: A user-facing workflow, route group, page, or navigation path currently contained in the mixed web application.
- **Organisation Membership**: A user's relationship to one or more organisations; membership remains shared across the platform, but each app filters which organisation types are selectable.
- **Private Organisation**: An organisation shown in WebApp Teams for private organisation and team workflows.
- **Marketplace Organisation**: A co-working space or marketplace organisation shown in WebApp Spaces for operator workflows.
- **Customer-Facing Subdomain**: A WebApp entry point scoped to one organisation, showing a co-working space public experience for marketplace organisations or a private organisation customer-facing experience for private organisations.
- **Ownership Inventory**: The maintained classification of targeted journeys and supporting modules into app-owned, shared, or temporary-exception categories.
- **Shared Component Foundation**: A neutral visual primitive, layout pattern, design token, or UI component used across multiple apps.
- **Shared Application Foundation**: A neutral non-visual helper, hook, provider, utility, or reusable behavioural foundation used across multiple apps without owning app-specific rules.
- **App-Owned Module**: A page, component, hook, utility, or workflow that belongs to exactly one app because it contains app-specific behaviour, permissions, copy, or orchestration.
- **Backend-Originated Return URL**: A frontend route or base URL used by backend-driven flows, such as payment or callback redirects, to return users to the web experience.
- **Transition Path**: A temporary route, redirect, adapter, or duplicate surface retained to keep existing users and backend-originated return flows working while the split is completed.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of targeted current web journeys reviewed in this initiative have exactly one documented owner or a documented temporary exception.
- **SC-002**: 100% of extracted customer-facing discovery and booking-entry journeys are reachable from WebApp and are not presented as organisation administration journeys.
- **SC-003**: 100% of extracted marketplace organisation journeys are reachable from WebApp Spaces and absent from WebApp Teams unless a documented shared or transition path applies.
- **SC-004**: 100% of extracted private organisation journeys are reachable from WebApp Teams and absent from WebApp Spaces unless a documented shared or transition path applies.
- **SC-005**: For every completed extraction slice, affected primary journeys can be completed successfully by the intended audience with no unapproved user-facing behaviour regression.
- **SC-006**: Developers can determine the correct owner for a touched page, workflow, component, hook, or utility in under 5 minutes using the ownership inventory and guidance.
- **SC-007**: No completed extraction slice leaves undocumented duplicate app-specific implementations across the three apps.
- **SC-008**: At least 90% of repeated neutral UI foundations targeted by the initiative are consumed from a shared UI or design-system layer after extraction.
- **SC-009**: Support or QA can identify the app involved in a moved-journey failure from verification output or diagnostics in under 10 minutes.
- **SC-010**: 100% of completed extraction slices move app-owned web code out of the mixed web application ownership area and into the owning app or an explicitly documented transition path.
- **SC-011**: 0 backend service, API contract, or backend data ownership changes are required to complete this web application split.
- **SC-012**: 100% of retired or deleted web routes are checked for backend-originated redirect and callback usage before removal.
- **SC-013**: 100% of organisation-selection views in WebApp Teams and WebApp Spaces show only organisations of the type owned by that app.
- **SC-014**: Users with no selectable organisations in the current app see an empty state with a create or join path instead of an unrelated organisation list.
- **SC-015**: WebApp root, marketplace organisation subdomain, and private organisation subdomain entry paths each expose only their intended customer-facing scope.

## Assumptions

- The three intended products are WebApp for customer-facing public discovery and booking, WebApp Spaces for co-working space and marketplace organisation operators, and WebApp Teams for enterprise and private organisations.
- Earlier scaffolding and modularisation work provides the starting app shells and shared-layer direction; this feature focuses on deciding and moving the real product experiences from the current mixed web application.
- Backend services, backend APIs, and backend data contracts already support these journeys and are outside the scope of this feature.
- Backend configuration may already contain web base URL or redirect URL values; route retirement must account for those frontend URL dependencies without changing backend ownership or data contracts.
- The split will be delivered incrementally by journey or product slice rather than as a single all-at-once replacement.
- Existing user-facing behaviour should remain unchanged unless the change is explicitly required to separate products or remove an incorrect cross-product exposure.
- Shared foundations are allowed only when they are genuinely neutral; app-specific copy, permissions, and business decisions stay with the owning app.
- The detailed implementation plan will map these product and shared ownership decisions to the existing app directories and shared component locations.
- Platform administration, authentication entry, and cross-product account concerns may require shared or special ownership decisions, but those decisions must be explicit in the ownership inventory.
- Existing users may need temporary redirects, adapters, or compatibility paths while routes and entry points move to the target apps.
- App-switching navigation is likely needed eventually, but the split can proceed while users access each app directly by URL.
