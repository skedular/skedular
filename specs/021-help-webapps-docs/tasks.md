# Tasks: Help Webapps Documentation

**Input**: Design documents from `specs/021-help-webapps-docs/`
**Prerequisites**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contracts/help-content-contract.md](./contracts/help-content-contract.md), [quickstart.md](./quickstart.md)

**Tests**: No TDD/unit test tasks are required because this is static help content. Verification tasks are included in the final phase for lint, build, source-inventory review, public-safety review, and product-boundary review.

**Organization**: Tasks are grouped by user story so each story can be implemented and reviewed independently.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it touches different files or independent review artifacts.
- **[Story]**: Maps the task to the user story from `spec.md`.
- Every task includes an exact repository path.

## Phase 1: Setup (Shared Documentation Structure)

**Purpose**: Create shared planning artifacts and help content directories used by later stories.

- [X] T001 Create source inventory directory in `specs/021-help-webapps-docs/source-inventory/`
- [X] T002 Create customer source inventory shell in `specs/021-help-webapps-docs/source-inventory/customer.md`
- [X] T003 [P] Create Teams source inventory shell in `specs/021-help-webapps-docs/source-inventory/teams.md`
- [X] T004 [P] Create Spaces source inventory shell in `specs/021-help-webapps-docs/source-inventory/spaces.md`
- [X] T005 [P] Create shared concepts inventory shell in `specs/021-help-webapps-docs/source-inventory/shared-concepts.md`
- [X] T006 [P] Create content gap register shell in `specs/021-help-webapps-docs/source-inventory/content-gaps.md`
- [X] T007 [P] Create screenshot placeholder guide in `specs/021-help-webapps-docs/source-inventory/screenshot-placeholders.md`
- [X] T008 [P] Verify Customer help content directory baseline in `src/web/apps/webapp-help/src/content/`
- [X] T009 [P] Verify Teams help content directory baseline in `src/web/apps/webapp-teams-help/src/content/`
- [X] T010 [P] Verify Spaces help content directory baseline in `src/web/apps/webapp-spaces-help/src/content/`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish the source-based inventory and shared content rules that all help writing depends on.

**CRITICAL**: No app-specific help drafting should begin until this phase is complete.

- [X] T011 Populate product split references from `specs/009-split-web-products/spec.md` and `specs/020-customer-landing-cleanup/spec.md` into `specs/021-help-webapps-docs/source-inventory/shared-concepts.md`
- [X] T012 Populate Customer route and root page inventory from `src/web/apps/webapp/src/app/` and `src/web/apps/webapp/src/rootPages/` into `specs/021-help-webapps-docs/source-inventory/customer.md`
- [X] T013 Populate Teams route and root page inventory from `src/web/apps/webapp-teams/src/app/` and `src/web/apps/webapp-teams/src/rootPages/` into `specs/021-help-webapps-docs/source-inventory/teams.md`
- [X] T014 Populate Spaces route and root page inventory from `src/web/apps/webapp-spaces/src/app/` and `src/web/apps/webapp-spaces/src/rootPages/` into `specs/021-help-webapps-docs/source-inventory/spaces.md`
- [X] T015 Classify shared concepts and app boundaries in `specs/021-help-webapps-docs/source-inventory/shared-concepts.md`
- [X] T016 Record unclear, risky, transitional, or insufficiently supported workflows in `specs/021-help-webapps-docs/source-inventory/content-gaps.md`
- [X] T017 Define the screenshot placeholder wording and replacement rules in `specs/021-help-webapps-docs/source-inventory/screenshot-placeholders.md`
- [X] T018 Update help navigation naming conventions for all three apps in `specs/021-help-webapps-docs/source-inventory/shared-concepts.md`

**Checkpoint**: Source inventory, shared concepts, content gaps, and screenshot placeholder rules are ready for app-specific help writing.

---

## Phase 3: User Story 1 - Build Help From The Real Product (Priority: P1) MVP

**Goal**: Ensure the first help content is grounded in current specs, routes, UI pages, help shells, and product boundaries.

**Independent Test**: Review `specs/021-help-webapps-docs/source-inventory/` and confirm every identified route, detail page, form, status, and major component state maps to help content, out-of-scope, or a content gap.

### Implementation for User Story 1

- [X] T019 [US1] Audit existing Customer help shell content in `src/web/apps/webapp-help/src/app/page.mdx` and record disposition in `specs/021-help-webapps-docs/source-inventory/customer.md`
- [X] T020 [P] [US1] Audit existing Teams help shell content in `src/web/apps/webapp-teams-help/src/app/page.mdx` and record disposition in `specs/021-help-webapps-docs/source-inventory/teams.md`
- [X] T021 [P] [US1] Audit existing Spaces help shell content in `src/web/apps/webapp-spaces-help/src/app/page.mdx` and record disposition in `specs/021-help-webapps-docs/source-inventory/spaces.md`
- [X] T022 [US1] Map Customer marketplace, booking, subscription, notification, settings, auth, welcome, and unsupported route surfaces in `specs/021-help-webapps-docs/source-inventory/customer.md`
- [X] T023 [P] [US1] Map Teams organization, booking, location, resource, team, member, analytics, notification, settings, Slack, and Microsoft Teams route surfaces in `specs/021-help-webapps-docs/source-inventory/teams.md`
- [X] T024 [P] [US1] Map Spaces marketplace setup, location, resource, product, booking, subscription, refund, analytics, payment, Slack, and Microsoft Teams route surfaces in `specs/021-help-webapps-docs/source-inventory/spaces.md`
- [X] T025 [US1] Mark Customer unclear or risky workflow gaps in `specs/021-help-webapps-docs/source-inventory/content-gaps.md`
- [X] T026 [P] [US1] Mark Teams unclear or risky workflow gaps in `specs/021-help-webapps-docs/source-inventory/content-gaps.md`
- [X] T027 [P] [US1] Mark Spaces unclear or risky workflow gaps in `specs/021-help-webapps-docs/source-inventory/content-gaps.md`
- [X] T028 [US1] Add inventory-to-help coverage table for Customer in `specs/021-help-webapps-docs/source-inventory/customer.md`
- [X] T029 [P] [US1] Add inventory-to-help coverage table for Teams in `specs/021-help-webapps-docs/source-inventory/teams.md`
- [X] T030 [P] [US1] Add inventory-to-help coverage table for Spaces in `specs/021-help-webapps-docs/source-inventory/spaces.md`

**Checkpoint**: User Story 1 is complete when the inventory proves the help scope is based on real product surfaces and gaps are explicit.

---

## Phase 4: User Story 2 - Explain What Each App Is For (Priority: P1)

**Goal**: Each help app has a clear public overview that explains audience, purpose, ownership, and when to use another help site.

**Independent Test**: A reviewer unfamiliar with the split can read the three home pages and identify which app owns customer, private organization, and marketplace operator tasks.

### Implementation for User Story 2

- [X] T031 [US2] Rewrite Customer help home page with purpose, audience, app boundaries, and documentation entry points in `src/web/apps/webapp-help/src/app/page.mdx`
- [X] T032 [P] [US2] Rewrite Teams help home page with purpose, audience, app boundaries, and documentation entry points in `src/web/apps/webapp-teams-help/src/app/page.mdx`
- [X] T033 [P] [US2] Rewrite Spaces help home page with purpose, audience, app boundaries, and documentation entry points in `src/web/apps/webapp-spaces-help/src/app/page.mdx`
- [X] T034 [US2] Create Customer overview page with purpose, audience, major product areas, and cross-app guidance in `src/web/apps/webapp-help/src/content/index.mdx`
- [X] T035 [P] [US2] Create Teams overview page with purpose, audience, major product areas, and cross-app guidance in `src/web/apps/webapp-teams-help/src/content/index.mdx`
- [X] T036 [P] [US2] Create Spaces overview page with purpose, audience, major product areas, and cross-app guidance in `src/web/apps/webapp-spaces-help/src/content/index.mdx`
- [X] T037 [US2] Update Customer help navigation metadata for overview and planned topic groups in `src/web/apps/webapp-help/src/content/_meta.ts`
- [X] T038 [P] [US2] Update Teams help navigation metadata for overview and planned topic groups in `src/web/apps/webapp-teams-help/src/content/_meta.ts`
- [X] T039 [P] [US2] Update Spaces help navigation metadata for overview and planned topic groups in `src/web/apps/webapp-spaces-help/src/content/_meta.ts`
- [X] T040 [US2] Remove or replace stock Nextra/banner/repository wording from Customer help layout in `src/web/apps/webapp-help/src/app/layout.tsx`
- [X] T041 [P] [US2] Remove or replace stock Nextra/banner/repository wording from Teams help layout in `src/web/apps/webapp-teams-help/src/app/layout.tsx`
- [X] T042 [P] [US2] Remove or replace stock Nextra/banner/repository wording from Spaces help layout in `src/web/apps/webapp-spaces-help/src/app/layout.tsx`

**Checkpoint**: User Story 2 is complete when all three help apps clearly explain their purpose and product boundary without relying on generic duplicated copy.

---

## Phase 5: User Story 3 - Document Functionality In Useful Detail (Priority: P1)

**Goal**: Each help app has comprehensive topic pages and step-by-step guides for every major workflow or an explicit content gap.

**Independent Test**: Given representative tasks from the source inventory, a reviewer can find the matching topic, task guide, screenshot placeholder, expected result, and content gap where applicable.

### Implementation for User Story 3

- [X] T043 [US3] Create Customer discovery and location browsing topic page in `src/web/apps/webapp-help/src/content/discovery.mdx`
- [X] T044 [P] [US3] Create Customer marketplace product browsing topic page in `src/web/apps/webapp-help/src/content/products.mdx`
- [X] T045 [P] [US3] Create Customer bookings and subscriptions topic page in `src/web/apps/webapp-help/src/content/bookings-and-subscriptions.mdx`
- [X] T046 [P] [US3] Create Customer account, notifications, settings, and unsupported routes topic page in `src/web/apps/webapp-help/src/content/account-and-support.mdx`
- [X] T047 [US3] Create Customer step-by-step task guide page for discovery, location detail, product detail, booking, subscription, and self-service flows in `src/web/apps/webapp-help/src/content/customer-guides.mdx`
- [X] T048 [US3] Create Customer content gaps page tied to source inventory in `src/web/apps/webapp-help/src/content/content-gaps.mdx`
- [X] T049 [US3] Update Customer `_meta.ts` to include topic pages, task guides, and content gaps in `src/web/apps/webapp-help/src/content/_meta.ts`
- [X] T050 [P] [US3] Create Teams private organization entry and administration topic page in `src/web/apps/webapp-teams-help/src/content/organization-admin.mdx`
- [X] T051 [P] [US3] Create Teams bookings, locations, resources, zones, and floor plans topic page in `src/web/apps/webapp-teams-help/src/content/bookings-locations-resources.mdx`
- [X] T052 [P] [US3] Create Teams teams, members, notifications, settings, Slack, and Microsoft Teams topic page in `src/web/apps/webapp-teams-help/src/content/people-settings-integrations.mdx`
- [X] T053 [P] [US3] Create Teams analytics, availability, and SSO topic page in `src/web/apps/webapp-teams-help/src/content/analytics-availability-sso.mdx`
- [X] T054 [US3] Create Teams step-by-step task guide page for organization entry, bookings, locations, resources, teams, members, settings, integrations, analytics, and SSO flows in `src/web/apps/webapp-teams-help/src/content/teams-guides.mdx`
- [X] T055 [US3] Create Teams content gaps page tied to source inventory in `src/web/apps/webapp-teams-help/src/content/content-gaps.mdx`
- [X] T056 [US3] Update Teams `_meta.ts` to include topic pages, task guides, and content gaps in `src/web/apps/webapp-teams-help/src/content/_meta.ts`
- [X] T057 [P] [US3] Create Spaces marketplace setup and organization entry topic page in `src/web/apps/webapp-spaces-help/src/content/marketplace-setup.mdx`
- [X] T058 [P] [US3] Create Spaces locations, resources, zones, and floor plans topic page in `src/web/apps/webapp-spaces-help/src/content/locations-resources.mdx`
- [X] T059 [P] [US3] Create Spaces products, bookings, subscriptions, and refunds topic page in `src/web/apps/webapp-spaces-help/src/content/commerce-operations.mdx`
- [X] T060 [P] [US3] Create Spaces analytics, availability, payments, Slack, and Microsoft Teams topic page in `src/web/apps/webapp-spaces-help/src/content/analytics-payments-integrations.mdx`
- [X] T061 [US3] Create Spaces step-by-step task guide page for marketplace setup, locations, resources, products, bookings, subscriptions, refunds, payments, integrations, analytics, and availability flows in `src/web/apps/webapp-spaces-help/src/content/spaces-guides.mdx`
- [X] T062 [US3] Create Spaces content gaps page tied to source inventory in `src/web/apps/webapp-spaces-help/src/content/content-gaps.mdx`
- [X] T063 [US3] Update Spaces `_meta.ts` to include topic pages, task guides, and content gaps in `src/web/apps/webapp-spaces-help/src/content/_meta.ts`
- [X] T064 [US3] Add screenshot placeholders to Customer guides where visual capture is needed in `src/web/apps/webapp-help/src/content/customer-guides.mdx`
- [X] T065 [P] [US3] Add screenshot placeholders to Teams guides where visual capture is needed in `src/web/apps/webapp-teams-help/src/content/teams-guides.mdx`
- [X] T066 [P] [US3] Add screenshot placeholders to Spaces guides where visual capture is needed in `src/web/apps/webapp-spaces-help/src/content/spaces-guides.mdx`
- [X] T067 [US3] Cross-check Customer guide coverage against `specs/021-help-webapps-docs/source-inventory/customer.md` and update `src/web/apps/webapp-help/src/content/content-gaps.mdx`
- [X] T068 [P] [US3] Cross-check Teams guide coverage against `specs/021-help-webapps-docs/source-inventory/teams.md` and update `src/web/apps/webapp-teams-help/src/content/content-gaps.mdx`
- [X] T069 [P] [US3] Cross-check Spaces guide coverage against `specs/021-help-webapps-docs/source-inventory/spaces.md` and update `src/web/apps/webapp-spaces-help/src/content/content-gaps.mdx`

**Checkpoint**: User Story 3 is complete when every inventoried workflow maps to a topic, guide, out-of-scope decision, or content gap.

---

## Phase 6: User Story 4 - Keep The Writing Simple And Human (Priority: P2)

**Goal**: Make the help comprehensive without sounding like generic marketing or generated filler.

**Independent Test**: Reviewers can read sample pages from all three apps and confirm they use short headings, simple wording, concrete examples, app-boundary clarity, and American spelling.

### Implementation for User Story 4

- [X] T070 [US4] Edit Customer overview, topics, guides, and gaps for plain human language in `src/web/apps/webapp-help/src/content/`
- [X] T071 [P] [US4] Edit Teams overview, topics, guides, and gaps for plain human language in `src/web/apps/webapp-teams-help/src/content/`
- [X] T072 [P] [US4] Edit Spaces overview, topics, guides, and gaps for plain human language in `src/web/apps/webapp-spaces-help/src/content/`
- [X] T073 [US4] Normalize shared terminology across all help apps using `specs/021-help-webapps-docs/source-inventory/shared-concepts.md`
- [X] T074 [US4] Review Customer help for public-safety issues and remove sensitive or internal-only details in `src/web/apps/webapp-help/src/content/`
- [X] T075 [P] [US4] Review Teams help for public-safety issues and remove sensitive or internal-only details in `src/web/apps/webapp-teams-help/src/content/`
- [X] T076 [P] [US4] Review Spaces help for public-safety issues and remove sensitive or internal-only details in `src/web/apps/webapp-spaces-help/src/content/`
- [X] T077 [US4] Update content gap register after copy and safety review in `specs/021-help-webapps-docs/source-inventory/content-gaps.md`

**Checkpoint**: User Story 4 is complete when the help is simple, public-safe, consistent, and still comprehensive.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Verify the complete documentation feature and prepare it for implementation review.

- [X] T078 Run Customer help lint and record result in `specs/021-help-webapps-docs/source-inventory/review-notes.md`
- [X] T079 [P] Run Teams help lint and record result in `specs/021-help-webapps-docs/source-inventory/review-notes.md`
- [X] T080 [P] Run Spaces help lint and record result in `specs/021-help-webapps-docs/source-inventory/review-notes.md`
- [X] T081 Run Customer help build and record result or sandbox limitation in `specs/021-help-webapps-docs/source-inventory/review-notes.md`
- [X] T082 [P] Run Teams help build and record result or sandbox limitation in `specs/021-help-webapps-docs/source-inventory/review-notes.md`
- [X] T083 [P] Run Spaces help build and record result or sandbox limitation in `specs/021-help-webapps-docs/source-inventory/review-notes.md`
- [X] T084 Verify all task-guide screenshot placeholders follow the contract in `specs/021-help-webapps-docs/contracts/help-content-contract.md`
- [X] T085 Verify every source inventory item maps to content, out-of-scope, or gap in `specs/021-help-webapps-docs/source-inventory/`
- [X] T086 Verify no public help page exposes sensitive details using `specs/021-help-webapps-docs/contracts/help-content-contract.md`
- [X] T087 Verify American spelling, grammar, and unauthenticated readability across help content in `src/web/apps/webapp-help/src/content/`, `src/web/apps/webapp-teams-help/src/content/`, and `src/web/apps/webapp-spaces-help/src/content/`
- [X] T088 Update final review notes, reader-review sample, and static-doc diagnostics decision in `specs/021-help-webapps-docs/source-inventory/review-notes.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No dependencies.
- **Phase 2 Foundational**: Depends on Phase 1. Blocks all help drafting.
- **Phase 3 US1**: Depends on Phase 2. MVP scope.
- **Phase 4 US2**: Depends on Phase 2 and can start after US1 inventory is sufficiently stable.
- **Phase 5 US3**: Depends on Phase 3 and Phase 4 so topics and guides can trace to inventory and overview boundaries.
- **Phase 6 US4**: Depends on drafted content from Phase 4 and Phase 5.
- **Phase 7 Polish**: Depends on all desired user stories being complete.

### User Story Dependencies

- **US1 (P1)**: Can start after Foundational. No dependency on other stories.
- **US2 (P1)**: Can start after Foundational, but should use US1 inventory decisions where possible.
- **US3 (P1)**: Depends on US1 inventory and US2 app-boundary overview decisions.
- **US4 (P2)**: Depends on drafted content from US2 and US3.

### Parallel Opportunities

- Setup shell files for Customer, Teams, Spaces, shared concepts, content gaps, and screenshot placeholders can run in parallel after T001.
- Teams and Spaces audits can run in parallel with Customer audits when they write separate files.
- Customer, Teams, and Spaces overview pages can be drafted in parallel after source inventory is available.
- Customer, Teams, and Spaces topic pages can be drafted in parallel because they live in separate help apps.
- Lint/build verification for the three help apps can run in parallel.

---

## Parallel Example: User Story 3

```text
Task: "Create Customer marketplace product browsing topic page in src/web/apps/webapp-help/src/content/products.mdx"
Task: "Create Teams bookings, locations, resources, zones, and floor plans topic page in src/web/apps/webapp-teams-help/src/content/bookings-locations-resources.mdx"
Task: "Create Spaces products, bookings, subscriptions, and refunds topic page in src/web/apps/webapp-spaces-help/src/content/commerce-operations.mdx"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational.
3. Complete Phase 3: User Story 1.
4. Stop and validate the source inventory before writing help copy.

### Incremental Delivery

1. Inventory first: build the source-of-truth map and content gaps.
2. Explain each app: update home and overview pages.
3. Add detailed topics and task guides per app.
4. Edit for simple human language and public safety.
5. Run lint/build and manual review.

### Parallel Team Strategy

1. One person owns shared inventory and content-gap rules.
2. One person writes Customer help.
3. One person writes Teams help.
4. One person writes Spaces help.
5. A final reviewer checks terminology, public safety, and coverage across all three apps.

## Notes

- Do not implement product app workflow changes in this feature.
- Do not change GraphQL, Relay, OpenAPI, protobuf, or generated artifacts.
- Do not guess unclear workflows; record them in `specs/021-help-webapps-docs/source-inventory/content-gaps.md`.
- Screenshot placeholders are allowed; final screenshots are intentionally deferred.
- Every user-facing help page must use American spelling and grammar.
