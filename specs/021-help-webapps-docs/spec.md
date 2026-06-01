# Feature Specification: Help Webapps Documentation

**Feature Branch**: `021-help-webapps-docs`  
**Created**: 2026-06-01  
**Status**: Draft  
**Input**: User description: "I need you to start working on three help webapps. each one meant to be explaining what each app supposed to do. come up with explaination and start documenting differnet functionalities in each webapps help projects"

## Clarifications

### Session 2026-06-01

- Q: What content depth should the first version include for each help app? → A: Full help center with topic pages plus step-by-step task guides for every major workflow.
- Q: What should count as a major workflow for step-by-step help coverage? → A: Every route, detail page, form, status, and major component state counts as a major workflow.
- Q: How should screenshots be handled in the first version? → A: Include screenshot placeholders for later capture.
- Q: Who should be able to read the first-version help centers? → A: Public help; avoid sensitive/internal details.
- Q: How should unclear or risky flows be handled while drafting help? → A: Document clear flows; mark unclear or risky flows as content gaps.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Build help from the real product (Priority: P1)

A product owner or support lead needs the first help content for the three help webapps to be based on the actual product split, current route surfaces, existing UI pages, and approved feature specs, not on generic assumptions about what the apps might do.

**Why this priority**: The help sites will be wrong if they describe imagined features. The first step is to understand what already exists and what each app owns.

**Independent Test**: Review the source inventory used for the help work and confirm it covers the current customer, Teams, and Spaces app pages, navigation areas, and relevant feature specs.

**Acceptance Scenarios**:

1. **Given** the customer, Teams, and Spaces products already have route surfaces and product split specs, **When** the help inventory is created, **Then** each help app has a documented list of product areas it must explain.
2. **Given** a product area appears in more than one app, **When** it is documented, **Then** the help explains the responsibility from the correct app's point of view.
3. **Given** a feature is still transitional, incomplete, or planned, **When** it appears in the help inventory, **Then** the help marks it clearly instead of presenting it as fully available.

---

### User Story 2 - Explain what each app is for (Priority: P1)

A new user can open any one of the three help webapps and quickly understand what that app is for, who should use it, and which work belongs somewhere else.

**Why this priority**: The three products are being split. Users need a clear, simple explanation before they can choose the right help site or product area.

**Independent Test**: Ask users who do not know the product split to read each help home page and explain the app's audience, purpose, and main responsibilities in their own words.

**Acceptance Scenarios**:

1. **Given** a visitor opens Customer help, **When** they read the overview, **Then** they understand that WebApp is for public discovery, location browsing, marketplace booking, subscriptions, and personal customer self-service.
2. **Given** a visitor opens Teams help, **When** they read the overview, **Then** they understand that Teams is for private organization work, including private bookings, locations, resources, teams, members, settings, notifications, integrations, and analytics.
3. **Given** a visitor opens Spaces help, **When** they read the overview, **Then** they understand that Spaces is for co-working and marketplace operators, including marketplace setup, locations, resources, products, bookings, subscriptions, refunds, payments, integrations, availability, and analytics.

---

### User Story 3 - Document functionality in useful detail (Priority: P1)

A customer, admin, operator, or support person can browse the right help webapp and find detailed but easy-to-read explanations of the main things that app does.

**Why this priority**: A thin overview is not enough. The first version must give users practical coverage of the major workflows so the help sites are useful from the start.

**Independent Test**: Give reviewers a list of common tasks for each app and confirm they can find a relevant help topic that explains the task purpose, when to use it, what to expect, and where the responsibility belongs.

**Acceptance Scenarios**:

1. **Given** a customer wants to understand a booking, subscription, payment state, cancellation, or refund request, **When** they use Customer help, **Then** the content explains the concept in customer language and points out when an action depends on policy or payment state.
2. **Given** a private organization admin wants to manage bookings, locations, resources, teams, members, SSO, notifications, integrations, or analytics, **When** they use Teams help, **Then** the content explains the workflow and keeps marketplace selling concepts out of Teams-owned help.
3. **Given** a marketplace operator wants to manage setup, locations, resources, products, bookings, subscriptions, refunds, payments, integrations, availability, or analytics, **When** they use Spaces help, **Then** the content explains the operator workflow and separates it from customer self-service.
4. **Given** a user needs to complete a major workflow, **When** they open the matching help area, **Then** they can read both the concept explanation and a step-by-step task guide for that workflow.
5. **Given** an app has a route, detail page, form, status, or major component state, **When** the first-version help scope is reviewed, **Then** that surface is treated as a major workflow unless the source inventory marks it as out of scope or a documented content gap.
6. **Given** a step-by-step guide would benefit from a visual reference, **When** the first-version help is written, **Then** the guide includes a clear screenshot placeholder so screenshots can be captured and inserted later.
7. **Given** any visitor opens a help center without signing in, **When** they browse help content, **Then** the content is readable publicly and does not expose sensitive customer, payment, security, or internal operator details.

---

### User Story 4 - Keep the writing simple and human (Priority: P2)

A user can read the help without feeling like it was written as marketing copy or generated filler. The writing is plain, direct, and complete.

**Why this priority**: Help content should reduce confusion. It needs to be comprehensive, but not dense.

**Independent Test**: Review sample pages for reading clarity and confirm they use short headings, plain sentences, concrete examples, and practical explanations.

**Acceptance Scenarios**:

1. **Given** a help topic explains a complex workflow, **When** a user reads it, **Then** the topic uses simple words, short sections, and examples instead of abstract product language.
2. **Given** a topic needs detail, **When** it is written, **Then** it covers the important cases without becoming vague or repetitive.
3. **Given** a shared concept appears across multiple help apps, **When** users compare the pages, **Then** the wording is consistent but still written for each app's audience.

### Edge Cases

- A user may open the wrong help site first and need to understand which app owns the work.
- A user may be both a customer and an organization admin, so the help must separate personal customer actions from operator actions.
- A workflow may exist in code but be transitional, hidden, or not fully owned by the target app yet.
- A workflow may not be clear enough from the reviewed source inventory to document safely; those flows must be recorded as content gaps instead of guessed.
- A term such as "booking", "subscription", "location", or "product" may mean different things depending on whether the audience is a customer, private organization admin, or marketplace operator.
- A workflow may touch billing, refunds, SSO, payments, or integrations; the help must explain user-facing behavior without exposing sensitive internal details.
- Existing content may be duplicated across the three help apps and needs to be replaced with app-specific content.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The feature MUST produce a source inventory before writing implementation content, covering existing product split specs, current app routes, current help app pages, and visible UI/navigation areas for Customer, Teams, and Spaces.
- **FR-002**: The feature MUST define one clear purpose statement for each help webapp: Customer help, Teams help, and Spaces help.
- **FR-003**: Customer help MUST explain WebApp as the public customer app for location discovery, location details, marketplace product browsing, one-time bookings, recurring subscriptions, personal bookings, personal subscriptions, notifications, settings, cancellation, and refund request guidance.
- **FR-004**: Teams help MUST explain Teams as the private organization app for organization entry, private bookings, recurring private bookings, locations, resources, zones, floor plans, teams, members, invitations, settings, SSO, notifications, integrations, availability, and analytics.
- **FR-005**: Spaces help MUST explain Spaces as the co-working and marketplace operator app for marketplace organization entry, marketplace setup, locations, resources, zones, floor plans, product publishing, marketplace bookings, subscriptions, refunds, payment setup, integrations, availability, and analytics.
- **FR-006**: Each help app MUST include a comprehensive first-version topic map grouped by product area.
- **FR-007**: Each help app MUST include separate topic pages for its product areas and step-by-step task guides for every major workflow in the first version.
- **FR-008**: For first-version help coverage, major workflows MUST include every route, detail page, form, status, and major component state found in the reviewed source inventory unless explicitly marked out of scope or recorded as a content gap.
- **FR-009**: Each topic MUST explain the audience, purpose, common tasks, expected result, important states, and when to use another app instead.
- **FR-010**: Step-by-step guides MUST include screenshot placeholders where a later screenshot is needed to make the workflow easier to follow.
- **FR-011**: The help centers MUST be readable without sign-in.
- **FR-012**: Public help content MUST avoid sensitive customer data, payment secrets, security configuration details, internal operator procedures, and any information that would weaken account, billing, integration, or organization security.
- **FR-013**: The writing MUST be simple, human, and practical, using short sections, plain language, American spelling, and concrete examples where they help.
- **FR-014**: The writing MUST avoid generic marketing language, exaggerated claims, placeholder text, and unexplained internal terms.
- **FR-015**: Shared concepts MUST use consistent names across the three help apps while explaining the difference between customer, private organization, and marketplace operator responsibilities.
- **FR-016**: Customer help MUST not present private organization administration or marketplace operator setup as customer-owned functionality.
- **FR-017**: Teams help MUST not present public marketplace discovery, customer storefronts, customer subscriptions, marketplace refunds, payment setup, or marketplace product publishing as Teams-owned functionality.
- **FR-018**: Spaces help MUST not present private team management or customer personal self-service as Spaces-owned functionality.
- **FR-019**: The feature MUST identify known content gaps for each help app so deeper detail can be planned when a workflow cannot be fully documented from the available source inventory.
- **FR-020**: Clear workflows MUST be documented from the reviewed source inventory; unclear, risky, transitional, or insufficiently supported workflows MUST be marked as content gaps instead of being described from best guesses.
- **FR-021**: The feature MUST provide review criteria so product, engineering, and support reviewers can confirm the help matches current product behavior before implementation is accepted.
- **FR-022**: The feature MUST preserve current app behavior; this documentation feature is about help content and navigation inside the help webapps, not changing product workflows.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Because this feature adds static public help content and no new runtime business workflow, the feature MUST preserve the existing help app platform/build diagnostics and record lint/build verification results for each help app.
- **LOG-002**: Source inventory and review notes MUST make meaningful documentation decisions traceable, including app-boundary decisions, out-of-scope decisions, and content-gap decisions.
- **LOG-003**: Unclear, risky, transitional, or insufficiently supported flows MUST be recorded as content gaps instead of being guessed in public help.
- **LOG-004**: Public help and verification notes MUST avoid sensitive customer data, payment secrets, security configuration details, integration secrets, and internal operator procedures.

### Key Entities

- **Help Webapp**: One of the three product-specific help experiences: Customer help, Teams help, or Spaces help.
- **Source Inventory**: The set of specs, routes, UI pages, navigation areas, and existing help pages reviewed before content is written.
- **Help Topic**: A documented area of product functionality with audience, purpose, common tasks, expected result, important states, and ownership guidance.
- **Product Area**: A group of related help topics, such as bookings, subscriptions, locations, resources, teams, products, refunds, payments, integrations, settings, or analytics.
- **Ownership Boundary**: The explanation of whether a workflow belongs to Customer, Teams, or Spaces.
- **Content Gap**: A known missing or incomplete help topic that should be planned for a later documentation slice.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of the first-version help topic maps are traceable to the reviewed source inventory or marked as a content gap.
- **SC-002**: 100% of routes, detail pages, forms, statuses, and major component states found in the reviewed source inventory are covered by a help topic, step-by-step guide, out-of-scope decision, or documented content gap.
- **SC-003**: At least 90% of review tasks selected by product and support reviewers can be matched to a help topic, step-by-step guide, or documented content gap.
- **SC-004**: At least 85% of test readers can identify the correct help app for customer, private organization, and marketplace operator tasks after reading the three overview pages.
- **SC-005**: Product, engineering, and support review finds zero contradictions between the three help apps for shared concepts.
- **SC-006**: 100% of drafted help topics use plain-language headings and avoid placeholder text, except explicit screenshot placeholders marked for later capture.
- **SC-007**: 100% of step-by-step guides that need a visual reference include a screenshot placeholder.
- **SC-008**: Security and product review finds zero sensitive customer, payment, security, integration, or internal operator details exposed in public help content.
- **SC-009**: 100% of unclear, risky, transitional, or insufficiently supported workflows found during review are listed as content gaps rather than undocumented assumptions.

## Assumptions

- The three help webapps correspond to the existing Customer, Teams, and Spaces product help projects.
- The first version should include detailed step-by-step guides for major workflows, with content gaps recorded only where the available source inventory does not support accurate guidance.
- Existing product split specs and current app route/UI surfaces are the starting point for content scope.
- The work should go through Spec Kit planning and task generation before help app implementation continues.
- User-facing help copy should use American spelling and grammar.
- The help centers are public documentation surfaces unless a later feature explicitly adds private or role-specific help.
